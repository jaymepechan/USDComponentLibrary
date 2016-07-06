// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.Desktop.SessionManager;
using System.Diagnostics;
using C1.C1Schedule;
using System.Data;
using Microsoft.Uii.AifServices;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using Microsoft.Uii.Desktop.Cti.Core;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    /// <summary>
    /// Interaction logic for Scheduler.xaml
    /// This is a base control for building Unified Service Desk Aware add-ins
    /// See USD API documentation for full API Information available via this control.
    /// </summary>
    public partial class Scheduler : DynamicsBaseHostedControl
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private TraceLogger LogWriter = null;
        private SchedulerAppointmentList appointmentList = new SchedulerAppointmentList();
        #endregion

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public Scheduler(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();

            // This will create a log writer with the default provider for Unified Service desk
            LogWriter = new TraceLogger();

            #region Enhanced LogProvider Info
            // This will create a log writer with the same name as your hosted control. 
            // LogWriter = new TraceLogger(traceSourceName:"MyTraceSource");

            // If you utilize this feature,  you would need to add a section to the system.diagnostics settings area of the UnifiedServiceDesk.exe.config
            //<source name="MyTraceSource" switchName="MyTraceSwitchName" switchType="System.Diagnostics.SourceSwitch">
            //    <listeners>
            //        <add name="console" type="System.Diagnostics.DefaultTraceListener"/>
            //        <add name="fileListener"/>
            //        <add name="USDDebugListener" />
            //        <remove name="Default"/>
            //    </listeners>
            //</source>

            // and then in the switches area : 
            //<add name="MyTraceSwitchName" value="Verbose"/>

            #endregion

        }

        public SchedulerAppointmentList Appointments
        {
            get
            {
                return appointmentList;
            }
        }

        System.Threading.Timer checkForActivitiesTimer;
        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            // this will populate any toolbars assigned to this control in config. 
            PopulateToolbars(ProgrammableToolbarTray);
            base.DesktopReady();

            scheduler.DataStorage.AppointmentStorage.DataSource = Appointments;
            checkForActivitiesTimer = new System.Threading.Timer(new System.Threading.TimerCallback(CheckForNewActivities), null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            if (args.Action.Equals("SetView", StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string value = Utility.GetAndRemoveParameter(actionDataList, "View"); // asume there is a myKey=<value> in the data. 
                switch (value.ToLower())
                {
                    case "oneday":
                        SetStyle(scheduler.OneDayStyle);
                        break;
                    case "workingweek":
                        SetStyle(scheduler.WorkingWeekStyle);
                        break;
                    case "week":
                        SetStyle(scheduler.WeekStyle);
                        break;
                    case "month":
                        SetStyle(scheduler.MonthStyle);
                        break;
                    case "timeline":
                        SetStyle(scheduler.TimeLineStyle);
                        break;
                }

                #region Example process action
                //    // Access CRM and fetch a Record
            //    Microsoft.Xrm.Sdk.Messages.RetrieveRequest req = new Microsoft.Xrm.Sdk.Messages.RetrieveRequest(); 
            //    req.Target = new Microsoft.Xrm.Sdk.EntityReference( "account" , Guid.Parse("0EF05F4F-0D39-4219-A3F5-07A0A5E46FD5")); 
            //    req.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid" , "name" );
            //    Microsoft.Xrm.Sdk.Messages.RetrieveResponse response = (Microsoft.Xrm.Sdk.Messages.RetrieveResponse)this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting Account"); 


            //    // Example of pulling some data out of the passed in data array
            //    List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext, localSession);
            //    string valueIwant = Utility.GetAndRemoveParameter(actionDataList, "mykey"); // asume there is a myKey=<value> in the data. 



            //    // Example of pushing data to USD
            //    string global = Utility.GetAndRemoveParameter(actionDataList, "global"); // Assume there is a global=true/false in the data
            //    bool saveInGlobalSession = false;
            //    if (!String.IsNullOrEmpty(global))
            //        saveInGlobalSession = bool.Parse(global);

            //    Dictionary<string, CRMApplicationData> myDataToSet = new Dictionary<string, CRMApplicationData>();
            //    // add a string: 
            //    myDataToSet.Add("myNewKey", new CRMApplicationData() { name = "myNewKey", type = "string", value = "TEST" });

            //    // add a entity lookup:
            //    myDataToSet.Add("myNewKey", new CRMApplicationData() { name = "myAccount", type = "lookup", value = "account,0EF05F4F-0D39-4219-A3F5-07A0A5E46FD5,MyAccount" }); 

            //    if (saveInGlobalSession) 
            //    {
            //        // add context item to the global session
            //        ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, myDataToSet, true);
            //    }
            //    else
            //    {
            //        // Add context item to the current session. 
            //        ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, myDataToSet, true);
            //    }
                #endregion
            }

            base.DoAction(args);
        }

        DateTime dateLastChecked = DateTime.MaxValue;
        private void CheckForNewActivities(object obj)
        {
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                try
                {
                    Dictionary<string, Dictionary<string, object>> response;
                    string searchXml = String.Empty;
                    if (appointmentList.Count() == 0)
                        searchXml = fetchXmlInitial;
                    else
                        searchXml = fetchXmlUpdate.Replace("[[DATETIME]]", dateLastChecked.ToShortDateString());
                    dateLastChecked = DateTime.Now;
                    lock (_client.CrmInterface.ConnectionLockObject)
                        response = _client.CrmInterface.GetEntityDataByFetchSearch(searchXml);
                    FillDataSet(appointmentList, response);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("CheckForNewActivities: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            }));
        }

        private void FillDataSet(SchedulerAppointmentList apptList, Dictionary<string, Dictionary<string, object>> response)
        {
            if (response == null)
                return;
            bool checkForDups = true;
            if (apptList.Count() == 0)
                checkForDups = false;
            foreach (string key in response.Keys)
            {
                try
                {
                    SchedulerAppointment schedAppt = new SchedulerAppointment();

                    schedAppt.Subject = "[" + (string)response[key]["activitytypecode"] + "] " + (String)(response[key].ContainsKey("subject") ? response[key]["subject"] : "");
                    schedAppt.Id = (Guid)(response[key]["activityid"]);
                    schedAppt.Start = (DateTime)(response[key].ContainsKey("scheduledstart") ? DateTime.Parse((string)response[key]["scheduledstart"]) : DateTime.Now);
                    schedAppt.End = (DateTime)(response[key].ContainsKey("scheduledend") ? DateTime.Parse((string)response[key]["scheduledend"]) : DateTime.Now);
                    schedAppt.Body = (String)(response[key].ContainsKey("description") ? response[key]["description"] : "");
                    schedAppt.StatusCode = (String)(response[key].ContainsKey("statuscode") ? response[key]["statuscode"] : "");
                    schedAppt.StateCode = (String)(response[key].ContainsKey("statecode") ? response[key]["statecode"] : "");
                    schedAppt.Location = "";
                    schedAppt.Properties = "";
                    if (response[key]["activitytypecode_Property"] is KeyValuePair<string, object>)
                        schedAppt.ActivityType = ((KeyValuePair<string, object>)response[key]["activitytypecode_Property"]).Value as string;
                    else
                        schedAppt.ActivityType = (string)response[key]["activitytypecode"];
                    if (checkForDups == true)
                    {
                        SchedulerAppointment existingAppt =  apptList.FirstOrDefault(a => a.Id == schedAppt.Id);
                        if (existingAppt != null)
                        {
                            apptList.Remove(existingAppt);
                            if (schedAppt.StateCode != "Completed"
                                && schedAppt.StateCode != "Cancelled")
                                apptList.Add(schedAppt);
                            continue;
                        }
                        else
                        {
                            if (schedAppt.StateCode == "Completed"
                                || schedAppt.StateCode == "Cancelled")
                                continue;
                        }
                    }
                    apptList.Add(schedAppt);
                }
                catch
                {
                }
            }
        }

        #region Fetch Queries
        public static string fetchXmlInitial = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"">
                                            <entity name=""activitypointer"">
                                            <attribute name=""subject"" />
                                            <attribute name=""activityid"" />
                                            <attribute name=""scheduledstart"" />
                                            <attribute name=""regardingobjectid"" />
                                            <attribute name=""prioritycode"" />
                                            <attribute name=""scheduledend"" />
                                            <attribute name=""description"" />
                                            <attribute name=""activitytypecode"" />
                                            <attribute name=""statuscode"" />
                                            <attribute name=""statecode"" />
                                            <attribute name=""ownerid"" />
                                            <attribute name=""instancetypecode"" />
                                            <order attribute=""scheduledend"" descending=""false"" />
                                            <filter type=""and"">
                                              <condition attribute=""statecode"" operator=""in"">
                                                <value>0</value>
                                                <value>3</value>
                                              </condition>
                                              <condition attribute=""isregularactivity"" operator=""eq"" value=""1"" />
                                              <condition attribute=""statecode"" operator=""ne"" value=""1"" />
                                              <condition attribute=""statecode"" operator=""ne"" value=""2"" />
                                            </filter>
                                            <link-entity name=""activityparty"" from=""activityid"" to=""activityid"" alias=""ab"">
                                              <filter type=""and"">
                                                <condition attribute=""partyid"" operator=""eq-userid"" />
                                              </filter>
                                            </link-entity>
                                          </entity>
                                        </fetch>";
        public static string fetchXmlUpdate = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""true"">
                                            <entity name=""activitypointer"">
                                            <attribute name=""subject"" />
                                            <attribute name=""activityid"" />
                                            <attribute name=""scheduledstart"" />
                                            <attribute name=""regardingobjectid"" />
                                            <attribute name=""prioritycode"" />
                                            <attribute name=""scheduledend"" />
                                            <attribute name=""description"" />
                                            <attribute name=""activitytypecode"" />
                                            <attribute name=""statuscode"" />
                                            <attribute name=""statecode"" />
                                            <attribute name=""ownerid"" />
                                            <attribute name=""instancetypecode"" />
                                            <order attribute=""scheduledend"" descending=""false"" />
                                            <filter type=""and"">
                                              <condition attribute=""isregularactivity"" operator=""eq"" value=""1"" />
                                              <condition attribute=""modifiedon"" operator=""on-or-after"" value=""[[DATETIME]]"" />
                                            </filter>
                                            <link-entity name=""activityparty"" from=""activityid"" to=""activityid"" alias=""ab"">
                                              <filter type=""and"">
                                                <condition attribute=""partyid"" operator=""eq-userid"" />
                                              </filter>
                                            </link-entity>
                                          </entity>
                                        </fetch>";
        #endregion

        private void scheduler_UserEditingAppointment(object sender, C1.WPF.Schedule.AppointmentActionEventArgs e)
        {
            e.Handled = true;
            ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
            Guid id = (Guid)e.Appointment.Key[0];
            SchedulerAppointment appt = appointmentList.FirstOrDefault(a => a.Id == id);
            if (appt != null)
                CRMWindowRouter.DoRoutePopup(localSession, this.ApplicationName, appt.ActivityType, appt.Id.ToString(), String.Empty);
        }

        private void SetStyle(Style style)
        {
            if (!IsLoaded || scheduler.Style == style)
            {
                return;
            }
            scheduler.BeginUpdate();
            try
            {
                scheduler.ChangeStyle(style);
            }
            finally
            {
                scheduler_LayoutUpdated(null, null);
                // Always call EndUpdate to apply all changes.
                scheduler.EndUpdate();
            }
        }

        Style lastStyle = null;
        // refresh buttons' state if Scheduler's layout has been changed
        // by Scheduler's commands defined in default templates.
        void scheduler_LayoutUpdated(object sender, EventArgs e)
        {
            try
            {
                if (localSession.Customer == null
                    || lastStyle == scheduler.Style)
                    return;
                List<LookupRequestItem> data = new List<LookupRequestItem>();
                if (scheduler.Style == scheduler.MonthStyle)
                {
                    data.Add(new LookupRequestItem("style", "month"));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, data, false);
                }
                else if (scheduler.Style == scheduler.OneDayStyle)
                {
                    data.Add(new LookupRequestItem("style", "oneday"));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, data, false);
                }
                else if (scheduler.Style == scheduler.WorkingWeekStyle)
                {
                    data.Add(new LookupRequestItem("style", "workingweek"));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, data, false);
                }
                else if (scheduler.Style == scheduler.WeekStyle)
                {
                    data.Add(new LookupRequestItem("style", "week"));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, data, false);
                }
                else
                {
                    data.Add(new LookupRequestItem("style", "timeline"));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, data, false);
                }
                lastStyle = scheduler.Style;
            }
            catch
            {
            }
        }

        private void scheduler_UserAddingAppointment(object sender, C1.WPF.Schedule.AddingAppointmentEventArgs e)
        {
            // don't allow appointment creation this way
            // Use a toolbar
            e.Handled = true;
        }
    }
}
