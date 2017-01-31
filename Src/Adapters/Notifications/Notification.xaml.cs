using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Controls;
using Microsoft.Uii.AifServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;

namespace Microsoft.USD.ComponentLibrary.Adapters.Notifications
{
    /// <summary>
    /// Interaction logic for Notification.xaml
    /// </summary>
    public partial class Notification : MicrosoftBase
    {
        System.Threading.Timer CheckForActivitiesTimer;
        public Notification()
        {
            InitializeComponent();
        }

        public Notification(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            CheckForActivitiesTimer = new System.Threading.Timer(new System.Threading.TimerCallback(CheckForNewActivities), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));

            RegisterAction("ShowReminders", ShowReminders);
        }

        string snoozeText = "SNOOZE";
        string dismissText = "DISMISS";
        string dismissallText = "DISMISS ALL";
        string openitemText = "OPEN ITEM";
        string clicksnoozeText = "SNOOZE";
        string dueText = "DUE";
        string subjectText = "SUBJECT";
        private void ShowReminders(Microsoft.Uii.Csr.RequestActionEventArgs args = null)
        {
            if (args != null)
            {
                List<KeyValuePair<string, string>> argList = Utility.SplitLines(args.Data, CurrentContext, localSession);
                snoozeText = Utility.GetAndRemoveParameter(argList, "snooze");
                dismissText = Utility.GetAndRemoveParameter(argList, "dismiss");
                dismissallText = Utility.GetAndRemoveParameter(argList, "dismissall");
                openitemText = Utility.GetAndRemoveParameter(argList, "openitem");
                clicksnoozeText = Utility.GetAndRemoveParameter(argList, "clicksnooze");
                dueText = Utility.GetAndRemoveParameter(argList, "due");
                subjectText = Utility.GetAndRemoveParameter(argList, "subject");
            }
            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    if (outlookNotificationWindow != null)
                    {
                        outlookNotificationWindow.Activate();
                        return;
                    }
                    outlookNotificationWindow = new OutlookNotification(this, notifyItems
                        ,snoozeText, dismissText, dismissallText, openitemText, clicksnoozeText, dueText, subjectText);
                    outlookNotificationWindow.ShowInTaskbar = true;
                    outlookNotificationWindow.Closing += new System.ComponentModel.CancelEventHandler(outlookNotificationWindow_Closing);
                    outlookNotificationWindow.Show();
                }
                catch (Exception ex)
                {
                    DynamicsLogger.Logger.Log("ShowReminders: " + ex.Message + "\r\n" + ex.StackTrace);
                    outlookNotificationWindow = null;
                }
            }));
        }

        internal void DismissAll()
        {
            foreach (NotificationItem item in handledItems)
            {
                item.dismissed = true;
            }
            notifyItems.Clear();
            if (outlookNotificationWindow != null)
                outlookNotificationWindow.Close();
        }

        internal void OpenItem(Notification.NotificationItem item)
        {
            CRMWindowRouter.DoRoutePopup(localSession, this.ApplicationName, item.LogicalName, item.id.ToString(), String.Empty);
        }

        internal void Dismiss(Notification.NotificationItem item)
        {
            item.dismissed = true;
            if (notifyItems.Contains(item))
                notifyItems.Remove(item);
            ShowReminders();
        }

        internal void Snooze(DateTime dtReschedule, Notification.NotificationItem item)
        {
            snoozeItems.Add(new KeyValuePair<DateTime, NotificationItem>(dtReschedule, item));
            if (notifyItems.Contains(item))
                notifyItems.Remove(item);
            ShowReminders();
        }

        OutlookNotification outlookNotificationWindow;
        List<NotificationItem> handledItems = new List<NotificationItem>();
        ObservableCollection<Notification.NotificationItem> notifyItems = new ObservableCollection<Notification.NotificationItem>();
        List<KeyValuePair<DateTime, NotificationItem>> snoozeItems = new List<KeyValuePair<DateTime, NotificationItem>>();
        private void CheckForNewActivities(object obj)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                try
                {
                    bool changes = false;
                    string searchXml = fetchXml.Replace("[[DATETIME]]", (DateTime.Now + TimeSpan.FromMinutes(15)).ToString());
                    Dictionary<string, Dictionary<string, object>> response;
                    lock (_client.CrmInterface.ConnectionLockObject)
                        response = _client.CrmInterface.GetEntityDataByFetchSearch(searchXml);

                    // remove ones that are no longer needed
                    for (int i = handledItems.Count - 1; i >= 0; i--)
                    {
                        NotificationItem ni = handledItems[i];
                        if (response.Where(a => a.Value.ContainsKey("activityid") && (Guid)a.Value["activityid"] == handledItems[i].id).Count() == 0)
                        {
                            if (notifyItems.Contains(handledItems[i]))
                                notifyItems.Remove(handledItems[i]);
                            handledItems.RemoveAt(i);
                        }
                    }

                    // add new ones
                    if (response != null && response.Keys != null)
                    {
                        foreach (string s in response.Keys)
                        {
                            if (response[s].ContainsKey("activityid") && handledItems.Where(b => b.id == (Guid)response[s]["activityid"]).Count() == 0
                                && notifyItems.Where(b => b.id == (Guid)response[s]["activityid"]).Count() == 0)
                            {
                                NotificationItem ni = new NotificationItem(response[s]);
                                notifyItems.Add(ni);
                                handledItems.Add(ni);
                                changes = true;
                            }
                        }
                    }
                    for (int i = snoozeItems.Count - 1; i >= 0; i--)
                    {
                        KeyValuePair<DateTime, NotificationItem> item = snoozeItems[i];
                        if (DateTime.Now > item.Key)
                        {
                            notifyItems.Add(item.Value);
                            snoozeItems.RemoveAt(i);
                            changes = true;
                        }
                    }
                    if (notifyItems.Count > 0 && changes == true)
                    {
                        ShowReminders();
                    }
                }
                catch (Exception ex)
                {
                    DynamicsLogger.Logger.Log("CheckForNewActivities: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            }));
        }

        void outlookNotificationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            outlookNotificationWindow = null;
        }


        public static string fetchXml = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"">
											<entity name=""activitypointer"">
											<attribute name=""subject"" />
											<attribute name=""activityid"" />
											<attribute name=""scheduledstart"" />
											<attribute name=""regardingobjectid"" />
											<attribute name=""prioritycode"" />
											<attribute name=""scheduledend"" />
											<attribute name=""activitytypecode"" />
											<attribute name=""instancetypecode"" />
											<order attribute=""scheduledend"" descending=""false"" />
											<filter type=""and"">
											  <condition attribute=""statecode"" operator=""in"">
												<value>0</value>
												<value>3</value>
											  </condition>
											  <condition attribute=""isregularactivity"" operator=""eq"" value=""1"" />
											</filter>
											<link-entity name=""activityparty"" from=""activityid"" to=""activityid"" alias=""ab"">
											  <filter type=""and"">
												<condition attribute=""partyid"" operator=""eq-userid"" />
											  </filter>
											  <link-entity name=""activitypointer"" from=""activityid"" to=""activityid"" alias=""ad"">
												<filter type=""and"">
												  <condition attribute=""scheduledend"" operator=""on-or-before"" value=""[[DATETIME]]"" />
												</filter>
											  </link-entity>
											</link-entity>
										  </entity>
										</fetch>";

        public class NotificationItem
        {
            Dictionary<string, object> items;
            public NotificationItem(Dictionary<string, object> items)
            {
                this.items = items;
            }
            public string subject
            {
                get
                {
                    return (string)items["subject"];
                }
            }
            public DateTime due
            {
                get
                {
                    return DateTime.Parse((string)items["scheduledend"]);
                }
            }
            public Guid id
            {
                get
                {
                    return (Guid)items["activityid"];
                }
            }
            public string LogicalName
            {
                get
                {
                    if (items["activitytypecode_Property"] == null)
                        return String.Empty;
                    return ((KeyValuePair<string, object>)items["activitytypecode_Property"]).Value as string;
                }
            }
            public string IconUrl
            {
                get
                {
                    ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                    if (items.ContainsKey("activitytypecode"))
                    {
                        //https://targetcemprototype.crm.dynamics.com/_imgs/Ribbon/entity16_2013.png
                        int code = CRMWindowRouter.EntityTypeFromName(((KeyValuePair<string, object>)items["activitytypecode_Property"]).Value as string);
                        return Utility.EnsureQualifiedRootUrl("/_imgs/Ribbon/entity16_" + code.ToString() + ".png");
                    }
                    return "";
                }
            }
            public bool dismissed = false;
        }

    }
}
