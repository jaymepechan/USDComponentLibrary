/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Core;
using System.Threading;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.UI.Controls.Wpf;
using Microsoft.Uii.Desktop.UI.Controls;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Xrm.Sdk;
using Microsoft.Uii.Desktop.SessionManager;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
#pragma warning disable 67
    public partial class SessionDropDown : DynamicsBaseHostedControl, ISessionExplorer
    {
        #region Fields
        // A ManualResetEvent to wait till AgentDesktop completes its processing before any UI operation
        // is done on the isolated application in order to synchronize the UI processing of
        // Agent Desktop window and isolated application's hosting form window.
        private ManualResetEvent processIsolatedAppUIEvent = new ManualResetEvent(false);
        // The isolated application whose UI operation is kept pending for above event to get set.
        protected IHostedAppUICommand isolatedAppWithPendingUIOperation;
        #endregion

        #region Properties
        protected WpfDesktopApplicationUI appsUI;
        bool showCloseButton = true;

        public virtual IApplicationUI AppsUI
        {
            set { appsUI = value as WpfDesktopApplicationUI; }
        }

        /// <summary>
        /// Get of set the height of the control
        /// </summary>
        public virtual int ControlHeight
        {
            get
            {
                return (int)this.Height;
            }
            set
            {
                this.Height = value;
            }
        }



        /// <summary>
        /// Get or set the value indicating wheather the control is displayed.
        /// </summary>
        public virtual bool IsControlVisible
        {
            get
            {
                return IsVisible;
            }
            set
            {
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event for when a hosted app is selected
        /// </summary>
        public event SessionExplorerEventHandler SEHostedAppSelected;

        /// <summary>
        /// Event for when a session is closed
        /// </summary>
        public event SessionExplorerEventHandler SessionClosed;

        #endregion

        #region HostedControl Members
        #region Properties

        /// <summary>
        /// Don't list application in SessionExplorer
        /// </summary>
        public override bool IsListed
        {
            get
            {
                return false;
            }
        }

        #endregion

        public SessionDropDown(Guid appID, string appName, string initString) :
            base(appID, appName, initString)
        {
            Init();
        }
        #endregion

        #region Constructor

        public SessionDropDown()
        {
            Init();
        }



        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the session item.
        /// </summary>
        /// <param name="sessionItem">The session item.</param>
        /// <param name="applicationItems">The application items.</param>
        /// <param name="insertIndex">Index of the insert. Inserted at the end if value less than zero.</param>
        private void AddSessionItem(ListBoxItem sessionItem, int insertIndex)
        {
            if (null != sessionItem)
            {
                if (!tcSessions.Items.Contains(sessionItem))
                {
                    if (insertIndex >= 0 && insertIndex < tcSessions.Items.Count)
                        tcSessions.Items.Insert(insertIndex, sessionItem);
                    else
                        tcSessions.Items.Add(sessionItem);
                    HideThisControl();
                }
            }
        }

        /// <summary>
        /// Removes the session item.
        /// </summary>
        /// <param name="sessionItem">The session item.</param>
        private void RemoveSessionItem(ListBoxItem sessionItem)
        {
            if (null != sessionItem)
            {
                if (tcSessions.Items.Contains(sessionItem))
                {
                    ListBoxItem selectedSessionItem = tcSessions.SelectedItem as ListBoxItem;

                    Button SessionCloseButton = ((ListBoxItem)selectedSessionItem).Template.FindName("PART_BTNCLOSE", ((ListBoxItem)selectedSessionItem)) as Button;
                    if (SessionCloseButton != null)
                    {
                        SessionCloseButton.Click -= btnClose_Click;
                    }

                    tcSessions.Items.Remove(sessionItem);
                    if (tcSessions.Items.Count > 0 && (null == selectedSessionItem || selectedSessionItem == sessionItem))
                        tcSessions.SelectedIndex = 0;
                    HideThisControl();
                }
            }
        }

        /// <summary>
        /// Hides the session tab control.
        /// </summary>
        private void HideThisControl()
        {
            if (!this.IsVisible && tcSessions.Items.Count > 0)
                this.Visibility = System.Windows.Visibility.Visible;
            else if (this.IsVisible && tcSessions.Items.Count <= 0)
                this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void Init()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            // This only appears if the sharing a Panel with some other app
            // or control, then this is the tab display text.
            //Text = MCSBaseControls.Properties.Resources.SESSION_TABS_CONTROL_MODULE_NAME;
            this.Visibility = System.Windows.Visibility.Collapsed;

        }

        protected override void DesktopReady()
        {
            ReadCRMRules();
            try
            {
                string hideSessionCloseButton = Utility.GetConfigurationValue("HideSessionCloseButton");
                if (hideSessionCloseButton != null && (hideSessionCloseButton.Equals("true") || hideSessionCloseButton.Equals("1")))
                    showCloseButton = false;
            }
            catch
            {
            }
        }

        class SessionInfo
        {
            public Entity entity;
            public string SelectedEntityName
            {
                get
                {
                    if (entity.Contains("msdyusd_selectedentity"))
                        return ((EntityReference)entity["msdyusd_selectedentity"]).Name;
                    return null;
                }
            }
            public string Display
            {
                get
                {
                    if (entity.Contains("msdyusd_display"))
                        return (string)entity["msdyusd_display"];
                    return null;
                }
            }
        }
        List<SessionInfo> sessionInfoEntries = new List<SessionInfo>();

        public Visibility CloseButtonVisibility
        {
            get
            {
                if (showCloseButton)
                    return System.Windows.Visibility.Visible;
                else
                    return System.Windows.Visibility.Collapsed;
            }
        }

        private void ReadCRMRules()
        {
            var q1 = from c in CRMWindowRouter.usdSessionInfo
                     where c.Contains("msdyusd_type") && ((OptionSetValue)c["msdyusd_type"]).Value == 803750000
                     select c;
            foreach (var sessioninfo in q1)
            {
                sessionInfoEntries.Add(new SessionInfo() { entity = sessioninfo });
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add a single application node.  If the application is global then will add
        /// for all session, else will add only for the active session passed in.
        /// </summary>
        /// <param name="session">The current active session</param>
        /// <param name="app">The application to add</param>
        public virtual void AddApplicationNode(Session session, IHostedApplication app)
        {
            // Handled in WpfApplicationExplorer control.
        }

        /// <summary>
        /// Add tagged global and tagged non-global applications nodes to the Application Explorer
        /// </summary>
        public virtual void AddWorkflowApplicationNodes(Session session, bool selected)
        {
            // Handled in WpfApplicationExplorer control.
        }

        /// <summary>
        /// Select the given application's node in the treeview
        /// </summary>
        /// <param name="appWithFocus">Application to focus on</param>
        public virtual void FocusOnApplication(IHostedApplication appWithFocus)
        {
            // Handled in WpfApplicationExplorer control.
        }

        /// <summary>
        /// Remove a single application node from the session explorer.  If the application is
        /// global then will remove for all session, else will remove only for the active session
        /// passed in.
        /// </summary>
        /// <param name="session">The current active session</param>
        /// <param name="app">The application to remove</param>
        public virtual void RemoveApplicationNode(Session session, IHostedApplication app)
        {
            // Handled in WpfApplicationExplorer control.
        }

        /// <summary>
        /// Remove the applications for a session from the DefaultSessionExplorerControl
        /// </summary>
        /// <param name="session"></param>
        public virtual void RemoveWorkflowApplicationNodes(Session session)
        {
            // Handled in WpfApplicationExplorer control.
        }

        /// <summary>
        /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
        /// </summary>
        public virtual void ControlRefresh()
        {
            this.InvalidateVisual();
        }

        protected override void DoAction(RequestActionEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Sessiontabs: DoAction " + args.Action + " data=" + args.Data);
            if (args.Action.Equals("CloseSession", StringComparison.InvariantCultureIgnoreCase))
            {
                List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
                string sessionId = Utility.GetAndRemoveParameter(parameters, "sessionid");
                if (String.IsNullOrEmpty(sessionId))
                    sessionId = localSessionManager.ActiveSession.SessionId.ToString();
                FireEvent("SessionClosing", new Dictionary<string, string>() { { "SessionId", sessionId } });
                if (null != this.SessionClosed)
                    this.SessionClosed(this, null);
                FireEvent("SessionClosed", new Dictionary<string, string>() { { "SessionId", sessionId } });
                return;
            }
            else if (args.Action.Equals("ResetProgressIndicator", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not implemented");
                //Guid SessionId = Guid.Parse(args.Data);
                //ResetProgressIndicator(SessionId);
            }
            else if (args.Action.Equals("HideProgressIndicator", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not implemented");
                //Guid SessionId = Guid.Parse(args.Data);
                //HideProgressIndicator(SessionId);
            }
            else if (args.Action.Equals("ChatAgentIndicator", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not implemented");
                //Guid SessionId = Guid.Parse(args.Data);
                //StartChatIndicator(SessionId, true);
            }
            else if (args.Action.Equals("ChatCustomerIndicator", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not implemented");
                //Guid SessionId = Guid.Parse(args.Data);
                //StartChatIndicator(SessionId, false);
            }
            else if (args.Action.Equals("HideChatIndicator", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not implemented");
                //Guid SessionId = Guid.Parse(args.Data);
                //HideChatIndicator(SessionId);
            }
            base.DoAction(args);
        }

        /// <summary>
        /// Adds the session to the ComboBox control if its not already there
        /// </summary>
        public virtual void AddSession(Session session, bool selected)
        {
            ListBoxItem sessionItem;

            // if there is no session name, then hide this session
            if (session == null || session.Name == null)
            {
                return;
            }

            // see if active session is already in the session explorer
            foreach (ListBoxItem sessionNode in tcSessions.Items)
            {
                if (sessionNode.Tag == session)
                {
                    return;
                }
            }

            sessionItem = new ListBoxItem();
            sessionItem.Loaded += new RoutedEventHandler(sessionItem_Loaded);
            sessionItem.Content = session.Name;
            sessionItem.Tag = session;
            sessionItem.DataContext = this;

            AddSessionItem(sessionItem, tcSessions.Items.Count);
            // if asked to select this node or if its the active session, then select it
            if (selected || session == localSessionManager.ActiveSession)
            {
                //SetSelected(tcSessions, sessionItem);
                tcSessions.SelectedItem = sessionItem;
            }

            // why isn't this being applied automatically?
            sessionItem.Style = (System.Windows.Style)this.FindResource("USDSessionListItemStyle");

            ((ListBoxItem)sessionItem).ApplyTemplate();
            Button SessionCloseButton = ((ListBoxItem)sessionItem).Template.FindName("PART_BTNCLOSE", ((ListBoxItem)sessionItem)) as Button;
            if (SessionCloseButton != null)
            {
                SessionCloseButton.Click += btnClose_Click;
            }
        }

        /// <summary>
        /// Complete redraw of the session explorer UI.
        /// </summary>
        public virtual void RefreshView()
        {
            try
            {
                // so the node select code doesn't run until we're done.
                tcSessions.SelectionChanged -= this.tcSessions_SelectionChanged;
                // Erase the current tree
                tcSessions.Items.Clear();

                if (localSessionManager != null)
                {
                    foreach (Session session in localSessionManager)
                    {
                        AddSession(session, false);
                    }
                }

            }
            finally
            {
                tcSessions.SelectionChanged += this.tcSessions_SelectionChanged;
            }
        }

        /// <summary>
        /// Removes a session from the session explorer tree.
        /// </summary>
        /// <param name="session"></param>
        public virtual void RemoveSession(Session session)
        {
            foreach (ListBoxItem sessionItem in tcSessions.Items)
            {
                if (session == sessionItem.Tag)
                {
                    RemoveSessionItem(sessionItem);
                    break;
                }
            }
        }

        #endregion

        private void tcSessions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            processIsolatedAppUIEvent.Reset();

            try
            {
                Session session = null;

                // so we know if this is caused by an agent changing the app with the control
                // or if this is happening from some other event within Desktop.
                //Does not seem to work for WPF

                ListBoxItem selectedItem = tcSessions.SelectedItem as ListBoxItem;
                if (selectedItem != null)
                {
                    session = selectedItem.Tag as Session;
                }
                if (session != null && session != localSessionManager.ActiveSession)
                {
                    // go to the session selected in the explorer, let it pick which
                    // app is highlighted.
                    localSessionManager.SetActiveSession(session.SessionId);
                }
            }
            catch (Exception eX)
            {
                //Logging.Error(this.ApplicationName, eX.Message);
            }
            finally
            {
                processIsolatedAppUIEvent.Set();
            }
        }

        private void btnClose_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Session s = null;
            try
            {
                ListViewItem ti = (sender as Button).TemplatedParent as ListViewItem;
                s = ti.Tag as Session;
            }
            catch
            {
                s = localSessionManager.ActiveSession;
            }
            if (!CRMWindowRouter.FireEvent(s, this.ApplicationName, "SessionCloseRequested", null)
                && null != this.SessionClosed)
                this.SessionClosed(this, null);
        }

        public override void NotifyContextChange(Context context)
        {
            base.NotifyContextChange(context);

            // This is a global hosted control so we need to use ActiveSession here
            if (((AgentDesktopSession)localSessionManager.ActiveSession).Customer == null)
                return;
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                try
                {
                    DynamicsCustomerRecord customerRec = (DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer;
                    string InitialLogicalName = "";
                    if (customerRec != null && customerRec.InitialEntity != null)
                        InitialLogicalName = customerRec.InitialEntity.LogicalName;
                    string InitialLogicalActivity = "";
                    if (customerRec != null && customerRec.StartActivity != null)
                        InitialLogicalActivity = customerRec.StartActivity.LogicalName;
                    foreach (SessionInfo si in sessionInfoEntries.Where(a => (!String.IsNullOrEmpty(a.SelectedEntityName) && (a.SelectedEntityName.Equals(InitialLogicalName, StringComparison.InvariantCultureIgnoreCase)
                        || a.SelectedEntityName.Equals(InitialLogicalActivity, StringComparison.InvariantCultureIgnoreCase))) || String.IsNullOrEmpty(a.SelectedEntityName)))
                    {
                        try
                        {
                            string sessionNameCandidate = Utility.GetContextReplacedString(si.Display, context, localSessionManager == null ? null : localSessionManager.ActiveSession);
                            if (Utility.IsAllReplacementValuesReplaced(sessionNameCandidate))
                            {
                                SetSessionName(sessionNameCandidate);
                                return;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
                catch
                {
                }
            }));
        }

        private void SetSessionName(string name)
        {
            if (localSessionManager.ActiveSession.Global)
                return; // Global session can not have a name!
            if (localSessionManager.ActiveSession.Name == name)
                return; // don't need to update it
            Dispatcher.Invoke(new System.Action(() =>
            {

                foreach (ListBoxItem sessionNode in tcSessions.Items)
                {
                    if (sessionNode.Tag == localSessionManager.ActiveSession)
                    {
                        sessionNode.Content = Utility.LoadElement(name);
                        localSessionManager.ActiveSession.Name = name;
                        return;
                    }
                }
            }));
        }

        private void ResetProgressIndicator(Guid SessionId)
        {
            //foreach (ListBoxItem sessionNode in tcSessions.Items)
            //{
            //    if (((Session)sessionNode.Tag).SessionId == SessionId)
            //    {
            //        RoundProgressIndicator.MainControl tProgressIndicator = sessionNode.Template.FindName("ProgressIndicator", sessionNode) as RoundProgressIndicator.MainControl;
            //        if (tProgressIndicator == null)
            //        {
            //            if (!preloadOptions.ContainsKey("ProgressIndicator"))
            //                preloadOptions.Add("ProgressIndicator", SessionId);
            //            continue;
            //        }
            //        tProgressIndicator.ResetIndicator();
            //        return;
            //    }
            //}
        }

        private void HideProgressIndicator(Guid SessionId)
        {
            //foreach (ListBoxItem sessionNode in tcSessions.Items)
            //{
            //    if (((Session)sessionNode.Tag).SessionId == SessionId)
            //    {
            //        RoundProgressIndicator.MainControl tProgressIndicator = sessionNode.Template.FindName("ProgressIndicator", sessionNode) as RoundProgressIndicator.MainControl;
            //        if (tProgressIndicator == null)
            //        {
            //            if (!preloadOptions.ContainsKey("ProgressIndicator"))
            //                preloadOptions.Remove("ProgressIndicator");
            //            continue;
            //        }
            //        tProgressIndicator.Visibility = System.Windows.Visibility.Collapsed;
            //        return;
            //    }
            //}
        }

        Dictionary<string, Guid> preloadOptions = new Dictionary<string, Guid>();

        private void StartChatIndicator(Guid SessionId, bool WaitAgent)
        {
            //foreach (ListBoxItem sessionNode in tcSessions.Items)
            //{
            //    if (((Session)sessionNode.Tag).SessionId == SessionId)
            //    {
            //        RoundProgressIndicator.ChatIndicator tChatIndicator = sessionNode.Template.FindName("ChatIndicator", sessionNode) as RoundProgressIndicator.ChatIndicator;
            //        if (tChatIndicator == null)
            //        {
            //            preloadOptions.Remove("WaitAgent");
            //            preloadOptions.Remove("WaitCustomer");
            //            if (WaitAgent && !preloadOptions.ContainsKey("WaitAgent"))
            //                preloadOptions.Add("WaitAgent", SessionId);
            //            else if (!preloadOptions.ContainsKey("WaitAgent"))
            //                preloadOptions.Add("WaitCustomer", SessionId);
            //            continue;
            //        }
            //        if (WaitAgent)
            //            tChatIndicator.WaitAgent();
            //        else
            //            tChatIndicator.WaitCustomer();
            //        return;
            //    }
            //}
        }

        private void HideChatIndicator(Guid SessionId)
        {
            //foreach (ListBoxItem sessionNode in tcSessions.Items)
            //{
            //    if (((Session)sessionNode.Tag).SessionId == SessionId)
            //    {
            //        RoundProgressIndicator.ChatIndicator tChatIndicator = sessionNode.Template.FindName("ChatIndicator", sessionNode) as RoundProgressIndicator.ChatIndicator;
            //        if (tChatIndicator == null)
            //        {
            //            preloadOptions.Remove("WaitAgent");
            //            preloadOptions.Remove("WaitCustomer");
            //            continue;
            //        }
            //        tChatIndicator.Visibility = System.Windows.Visibility.Collapsed;
            //        return;
            //    }
            //}
        }

        void sessionItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (preloadOptions.ContainsKey("WaitAgent"))
            {
                StartChatIndicator(preloadOptions["WaitAgent"], true);
            }
            else if (preloadOptions.ContainsKey("WaitCustomer"))
            {
                StartChatIndicator(preloadOptions["WaitCustomer"], false);
            }
            preloadOptions.Clear();
        }

    }
}
