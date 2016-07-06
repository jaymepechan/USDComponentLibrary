using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts;
using Microsoft.Uii.Desktop.UI.Controls;
using C1.WPF;
namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    public partial class C1Layout : PanelLayoutBase
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private TraceLogger LogWriter = null;

        #endregion

        public C1Layout(Guid appID, string appName, string initString)
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
        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            base.DesktopReady();
        }

        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            // Process Actions. 
            if (args.Action.Equals("SetExpanderState", StringComparison.OrdinalIgnoreCase))
            {
                List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext, localSession);
                string ExpanderName = Utility.GetAndRemoveParameter(actionDataList, "ExpanderName");
                string ExpanderRequestedState = Utility.GetAndRemoveParameter(actionDataList, "State");

                // Call back to process the event to make sure we are on the same thread.
                Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    // Only act if we have the right data and the panel is in the right state. 
                    var frameworkItem = this.FindName(ExpanderName);
                    if (frameworkItem != null && frameworkItem is Expander)
                    {

                        if (ExpanderRequestedState.Equals("expanded", StringComparison.OrdinalIgnoreCase))
                            if (!((Expander)frameworkItem).IsExpanded)
                                ((Expander)frameworkItem).IsExpanded = true;

                        if (ExpanderRequestedState.Equals("collapsed", StringComparison.OrdinalIgnoreCase))
                            if (((Expander)frameworkItem).IsExpanded)
                                ((Expander)frameworkItem).IsExpanded = false;
                    }
                }));


            }

            #region Example process action

            //if (args.Action.Equals("your action name", StringComparison.OrdinalIgnoreCase))
            //{
            //    // Do some work

            //    // Access CRM and fetch a Record
            //    Microsoft.Xrm.Sdk.Messages.RetrieveRequest req = new Microsoft.Xrm.Sdk.Messages.RetrieveRequest(); 
            //    req.Target = new Microsoft.Xrm.Sdk.EntityReference( "account" , Guid.Parse("0EF05F4F-0D39-4219-A3F5-07A0A5E46FD5")); 
            //    req.ColumnSet = new Microsoft.Xrm.Sdk.Query.ColumnSet("accountid" , "name" );
            //    Microsoft.Xrm.Sdk.Messages.RetrieveResponse response = (Microsoft.Xrm.Sdk.Messages.RetrieveResponse)this._client.CrmInterface.ExecuteCrmOrganizationRequest(req, "Requesting Account"); 


            //    // Example of pulling some data out of the passed in data array
            //    List<KeyValuePair<string, string>> actionDataList = Utility.SplitLines(args.Data, CurrentContext, localSession);
            //    string valueIwant = Utility.GetAndRemoveParameter(actionDataList, "mykey"); // assume there is a myKey=<value> in the data. 



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
            //}
            #endregion

            base.DoAction(args);
        }

        /// <summary>
        /// Raised when a context change occurs in USD
        /// </summary>
        /// <param name="context"></param>
        public override void NotifyContextChange(Microsoft.Uii.Csr.Context context)
        {
            base.NotifyContextChange(context);
        }

        /// <summary>
        /// Raised when the panel is closed. 
        /// </summary>
        public override void Close()
        {
            base.Close();
        }

        /// <summary>
        /// Raised when an app in panel changes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void SelectedAppChangedHander(object sender, EventArgs e)
        {
            string selectedAppName = String.Empty;
            if (DesktopApplicationUIBase.AppWithFocus != null)
                selectedAppName = DesktopApplicationUIBase.AppWithFocus.ApplicationName;

            if (sender is IUSDPanel)
            {
                string panelName = ((IUSDPanel)sender).Name;
                if (!string.IsNullOrEmpty(panelName))
                    FireEvent("SelectedAppChanged", new Dictionary<string, string>() { { "Panel", panelName }, { "Selection", selectedAppName } });
            }
        }

        /// <summary>
        /// Raised when the left expander expands
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            HandleExpanderStateChange(sender, e);
        }

        /// <summary>
        /// Raised when the left expander collapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            HandleExpanderStateChange(sender, e);
        }

        /// <summary>
        /// Raised an event when the state of the expander changes. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleExpanderStateChange(object sender, RoutedEventArgs e)
        {
            if (sender is Expander)
            {
                string expanderName = ((Expander)sender).Name;
                if (!string.IsNullOrEmpty(expanderName))
                    FireEvent("ExpanderStateChanged", new Dictionary<string, string>() { { "ExpanderName", expanderName }, { "IsExpanded", ((Expander)sender).IsExpanded.ToString() } });
            }
        }


        #region User Code Area

        #endregion

    }
}
