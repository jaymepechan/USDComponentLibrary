/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.USD.ComponentLibrary.Utilities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Tooling.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using Microsoft.Uii.Desktop.UI.Controls;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.EntitySearch;
using System.Web;
using System.Globalization;
using System.Windows.Interop;

namespace Microsoft.USD.ComponentLibrary
{
    public class Controller : MicrosoftBase
    {
        #region Constructors
        private TraceLogger diagLogger = new TraceLogger("Dynamics");

        public Controller()
        {

        }

        public Controller(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {

        }
        #endregion

        protected override void DesktopReady()
        {
            base.DesktopReady();

            try
            {
                // populate system information
                List<LookupRequestItem> lri = new List<LookupRequestItem>();
                lri.Add(new LookupRequestItem("ScreenCount", System.Windows.Forms.Screen.AllScreens.Count().ToString()));
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).AddReplaceableParameter("$SystemInfo", lri);

                RegisterAction("SetPopupProperties", SetPopupProperties);
                RegisterAction("ShowPopup", ShowPopup);
                RegisterAction("HidePopup", HidePopup);
                RegisterAction("CopyToClipboard", CopyToClipboard);
                RegisterAction("SetReplacementParameter", SetReplacementParameter);
                RegisterAction("SetTimedEvent", SetTimedEvent);
                RegisterAction("CreateEntityAssociation", CreateEntityAssociation);
                RegisterAction("New_CRM_Page", New_CRM_Page);
                RegisterAction("CloseIncident", CloseIncident);
                RegisterAction("CloseOpportunity", CloseOpportunity);
                RegisterAction("CloseQuote", CloseQuote);
                RegisterAction("RetrieveEntityOptionSet", RetrieveEntityOptionSet);
                RegisterAction("RetrieveGlobalOptionSet", RetrieveGlobalOptionSet);
                RegisterAction("CopyReplacementParameter", CopyReplacementParameter);
                RegisterAction("ForceDataParameterUpdate", ForceDataParameterUpdate);
                RegisterAction("LookupQueueItem", LookupQueueItem);
                RegisterAction("HookErrorEvents", HookErrorEvents);
                RegisterAction("SetAppBar", SetAppBar);
                RegisterAction("ExitApplication", ExitApplication);
                RegisterAction("ShellExecute", ShellExecute);
                RegisterAction("DetermineFocus", DetermineFocus);
                RegisterAction("LoadCRMTheme", LoadCRMTheme);
                RegisterAction("ApplyCRMTheme", ApplyCRMTheme);
                RegisterAction("Alert", Alert);
                RegisterAction("DoSearch", DoSearchAction);
                RegisterAction("EvaluateJavascript", EvaluateJavascriptAction);
                RegisterAction("ExecuteAction", ExecuteAction);
                RegisterAction("DisableClose", DisableClose);
                RegisterAction("EnableClose", EnableClose);
                RegisterAction("BlurControl", BlurControl);
                RegisterAction("UnblurControl", UnblurControl);
            }
            catch
            {   // ignore errors
            }
        }

        private void UnblurControl(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string appName = Utility.GetAndRemoveParameter(parms, "appname");
            System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
            IHostedApplication app = GetApp(appName);
            if (app != null && app is FrameworkElement)
            {
                ((FrameworkElement)app).Effect = null;
            }
        }

        private void BlurControl(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string appName = Utility.GetAndRemoveParameter(parms, "appname");
            System.Windows.Media.Effects.BlurEffect objBlur = new System.Windows.Media.Effects.BlurEffect();
            IHostedApplication app = GetApp(appName);
            if (app != null && app is FrameworkElement)
            {
                ((FrameworkElement)app).Effect = objBlur;
            }
        }

        [DllImport("User32.dll")]
        private static extern uint GetClassLong(IntPtr hwnd, int nIndex);

        [DllImport("User32.dll")]
        private static extern uint SetClassLong(IntPtr hwnd, int nIndex, uint dwNewLong);
        private const int GCL_STYLE = -26;
        private const uint CS_NOCLOSE = 0x0200;


        public const int SC_CLOSE = 0xF060;
        public const int MF_BYCOMMAND = 0;
        public const int MF_ENABLED = 0;
        public const int MF_GRAYED = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

        [DllImport("user32.dll")]
        public static extern int EnableMenuItem(IntPtr hMenu, int IDEnableItem, int enable);


        private void EnableClose(RequestActionEventArgs args)
        {
            Window w = Application.Current.MainWindow;
            var hwnd = new WindowInteropHelper(w).Handle;
            var style = GetClassLong(hwnd, GCL_STYLE);
            SetClassLong(hwnd, GCL_STYLE, style & ~CS_NOCLOSE);
            SetCloseButton(true, hwnd);
        }

        private void DisableClose(RequestActionEventArgs args)
        {
            Window w = Application.Current.MainWindow;
            var hwnd = new WindowInteropHelper(w).Handle;
            var style = GetClassLong(hwnd, GCL_STYLE);
            SetClassLong(hwnd, GCL_STYLE, style | CS_NOCLOSE);
            SetCloseButton(false, hwnd);
        }

        // If "enable" is true, the close button will be enabled (the default state).
        // If "enable" is false, the Close button will be disabled.
        void SetCloseButton(bool enable, IntPtr hwnd)
        {
            IntPtr hMenu = GetSystemMenu(hwnd, false);
            if (hMenu != IntPtr.Zero)
            {
                EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | (enable ? MF_ENABLED : MF_GRAYED));
            }
        }


        private void ExecuteAction(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string actionName = Utility.GetAndRemoveParameter(parms, "Name");
            string logMessageTag = Utility.GetAndRemoveParameter(parms, "logMessageTag");
            string global = Utility.GetAndRemoveParameter(parms, "global");
            string paramname = Utility.GetAndRemoveParameter(parms, "paramname");
            bool saveInGlobalSession = false;
            if (!String.IsNullOrEmpty(global))
                saveInGlobalSession = bool.Parse(global);

            OrganizationRequest orgReq = new OrganizationRequest(actionName);
            foreach (KeyValuePair<string, string> pair in parms)
            {
                object data = GetCRMEntityValue(pair.Value);
                orgReq.Parameters.Add(pair.Key, data);
            }

            OrganizationResponse resp1 = null;
            if (String.IsNullOrEmpty(logMessageTag))
                resp1 = _client.CrmInterface.Execute(orgReq);
            else
                resp1 = _client.CrmInterface.ExecuteCrmOrganizationRequest(orgReq, logMessageTag);

            if (resp1 != null)
            {
                string responseName = resp1.ResponseName;
                if (String.IsNullOrEmpty(paramname))
                    paramname = responseName;
                Dictionary<string, CRMApplicationData> appData = new Dictionary<string, CRMApplicationData>();
                foreach (string p in resp1.Results.Keys)
                {
                    object obj = resp1.Results[p];
                    appData.Add(p, UsdEntityDescription(p, obj));
                }

                if (saveInGlobalSession)
                {
                    ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).MergeReplacementParameter(paramname, appData, true);
                }
                else
                {
                    ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).MergeReplacementParameter(paramname, appData, true);
                }
                try
                {
                    // CURRENTLY BUGGED FOR GLOBAL CONTROLS
                    //ICRMCustomerSearch CRMCustomerSearch = AifServiceContainer.Instance.GetService<ICRMCustomerSearch>();
                    //CRMCustomerSearch.CheckUpdateContact(); // notify the system about the updated replacement parameter

                    // WORKAROUND FOR ABOVE BUG
                    Context c = ((AgentDesktopSession)localSessionManager.ActiveSession).AppHost.GetContext();
                    Context context = new Context(c.GetContext());
                    if (((AgentDesktopSession)localSessionManager.ActiveSession).Customer == null)
                        return;
                    context["ForceChange"] = Guid.NewGuid().ToString();
                    FireChangeContext(new ContextEventArgs(context));
                }
                catch
                {
                }

                if (appData.Count > 0)
                    args.ActionReturnValue = appData.Values.First().value;
            }
        }

        private void EvaluateJavascriptAction(RequestActionEventArgs args)
        {
            string scriptRun = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            if (Utility.IsAllReplacementValuesReplaced(scriptRun))
            {
                object Result = Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities.Javascript.EvaluateScript(scriptRun, false);
                if (Result == null)
                    return;
                args.ActionReturnValue = Result.ToString();
            }
        }

        #region Action Handlers
        void ShellExecute(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parms = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string file = Utility.GetAndRemoveParameter(parms, "file");
            string parameters = Utility.GetAndRemoveParameter(parms, "parameters");
            string directory = Utility.GetAndRemoveParameter(parms, "directory");

            NativeMethods.SHELLEXECUTEINFO shellexecuteinfo;
            shellexecuteinfo = new NativeMethods.SHELLEXECUTEINFO
            {
                lpVerb = "open",
                lpFile = file,
                lpParameters = parameters,
                lpDirectory = directory,
                nShow = 1,
                fMask = 0x440,
                hwnd = System.IntPtr.Zero
            };
            shellexecuteinfo.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(shellexecuteinfo);
            args.ActionReturnValue = NativeMethods.ShellExecuteEx(ref shellexecuteinfo).ToString();
        }

        void SetPopupProperties(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            IDesktopFeatureAccess desktopFeatures = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            string panelName = Utility.GetAndRemoveParameter(parameters, "panelname");
            PopupPanel popupPanel = desktopFeatures.GetPanel(panelName) as PopupPanel;
            if (popupPanel == null)
                throw new Exception("Unknown panel specified: " + panelName);
        }

        void ShowPopup(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            IDesktopFeatureAccess desktopFeatures = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            string panelName = Utility.GetAndRemoveParameter(parameters, "panelname");
            PopupPanel popupPanel = desktopFeatures.GetPanel(panelName) as PopupPanel;
            if (popupPanel == null)
                throw new Exception("Unknown panel specified: " + panelName);
            popupPanel.IsOpen = true;
        }

        void HidePopup(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            IDesktopFeatureAccess desktopFeatures = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            string panelName = Utility.GetAndRemoveParameter(parameters, "panelname");
            PopupPanel popupPanel = desktopFeatures.GetPanel(panelName) as PopupPanel;
            if (popupPanel == null)
                throw new Exception("Unknown panel specified: " + panelName);
            popupPanel.IsOpen = false;
        }

        void CopyToClipboard(Uii.Csr.RequestActionEventArgs args)
        {
            string replacedData = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            if (Utility.IsAllReplacementValuesReplaced(replacedData))
            {
                System.Windows.Clipboard.SetText(replacedData);
                Trace.WriteLine("CLIPBOARD: " + replacedData);
            }
            else
            {
                throw new Exception("Not all replacement parameters replaced: " + replacedData);
            }
        }

        void SetReplacementParameter(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string global = Utility.GetAndRemoveParameter(parameters, "global");
            bool saveInGlobalSession = false;
            if (!String.IsNullOrEmpty(global))
                saveInGlobalSession = bool.Parse(global);
            string appName = Utility.GetAndRemoveParameter(parameters, "appname");
            if (String.IsNullOrEmpty(appName))
            {
                throw new Exception("Appname must be specified in parameters");
            }
            List<LookupRequestItem> lri = new List<LookupRequestItem>();
            foreach (KeyValuePair<string, string> keypair in parameters)
            {
                lri.Add(new LookupRequestItem(keypair.Key, keypair.Value));
            }
            if (saveInGlobalSession)
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).MergeReplacementParameter(appName, lri, true);
            }
            else
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).MergeReplacementParameter(appName, lri, true);
            }
            try
            {
                // CURRENTLY BUGGED FOR GLOBAL CONTROLS
                //ICRMCustomerSearch CRMCustomerSearch = AifServiceContainer.Instance.GetService<ICRMCustomerSearch>();
                //CRMCustomerSearch.CheckUpdateContact(); // notify the system about the updated replacement parameter

                // WORKAROUND FOR ABOVE BUG
                Context c = ((AgentDesktopSession)localSessionManager.ActiveSession).AppHost.GetContext();
                Context context = new Context(c.GetContext());
                if (((AgentDesktopSession)localSessionManager.ActiveSession).Customer == null)
                    return;
                context["ForceChange"] = Guid.NewGuid().ToString();
                FireChangeContext(new ContextEventArgs(context));
            }
            catch
            {
            }

        }

        void SetTimedEvent(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            eventTimer et = new eventTimer() { session = localSessionManager.ActiveSession };
            et.appname = Utility.GetAndRemoveParameter(parameters, "appname");
            et.eventname = Utility.GetAndRemoveParameter(parameters, "eventname");
            et.milliseconds = Utility.GetAndRemoveParameter(parameters, "milliseconds");
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                et.parameters.Add(pair.Key, pair.Value);
            }
            double msecs = double.Parse(et.milliseconds);
            System.Threading.Timer t = new System.Threading.Timer(new System.Threading.TimerCallback(EventTimer), et, TimeSpan.FromMilliseconds(msecs), TimeSpan.Zero);
            et.timer = t;
            lock (eventTimerList)
                eventTimerList.Add(et);
        }

        void CreateEntityAssociation(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string entityname1 = Utility.GetAndRemoveParameter(parameters, "entityname1");
            Guid entityguid1 = Guid.Parse(Utility.GetAndRemoveParameter(parameters, "entityguid1"));
            string entityname2 = Utility.GetAndRemoveParameter(parameters, "entityname2");
            Guid entityguid2 = Guid.Parse(Utility.GetAndRemoveParameter(parameters, "entityguid2"));
            string relationshipname = Utility.GetAndRemoveParameter(parameters, "relationshipname");
            bool ret = _client.CrmInterface.CreateEntityAssociation(entityname1, entityguid1, entityname2, entityguid2, relationshipname);
            if (!ret)
                throw new Exception("Error creating association: " + _client.CrmInterface.LastCrmError);
        }

        void New_CRM_Page(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string appname = Utility.GetAndRemoveParameter(parameters, "appname");
            bool ShowTab = true;
            string url = String.Empty;
            string sessionId = Utility.GetAndRemoveParameter(parameters, "sessionid");
            if (String.IsNullOrEmpty(sessionId))
                sessionId = localSessionManager.ActiveSession.SessionId.ToString();
            Session s = localSessionManager.GetSession(Guid.Parse(sessionId));
            string extraqs = Utility.GetAndRemoveParameter(parameters, "extraqs");
            if (!String.IsNullOrEmpty(extraqs))
            {
                string extraqsDecoded = System.Web.HttpUtility.UrlDecode(extraqs);
                url = System.Web.HttpUtility.UrlEncode(Utility.GetContextReplacedString(extraqsDecoded, CurrentContext, localSession));
            }
            string extraqs_decoded = Utility.GetAndRemoveParameter(parameters, "extraqs_decoded");
            if (!String.IsNullOrEmpty(extraqs_decoded))
            {
                url = System.Web.HttpUtility.UrlEncode(Utility.GetContextReplacedString(extraqs_decoded, CurrentContext, localSession));
            }
            string showtab = Utility.GetAndRemoveParameter(parameters, "showtab");
            if (!String.IsNullOrEmpty(showtab))
            {
                ShowTab = bool.Parse(showtab);
            }
            string frame = Utility.GetAndRemoveParameter(parameters, "frame");
            string allowreplace = Utility.GetAndRemoveParameter(parameters, "allowreplace");
            string entity = Utility.GetAndRemoveParameter(parameters, "entity");
            if (String.IsNullOrEmpty(entity))
                entity = Utility.GetAndRemoveParameter(parameters, "LogicalName");
            if (String.IsNullOrEmpty(entity))
            {
                entity = Utility.GetAndRemoveParameter(parameters, 0);
            }
            string id = Utility.GetAndRemoveParameter(parameters, "id");
            url = CreateEntityUrl(entity, id, url, true);
            CRMWindowRouter.DoRoutePopup(s, !String.IsNullOrEmpty(appname) ? appname : this.ApplicationName, url, frame, ShowTab, true);
            PostWindowRoute(url);
        }

        void CloseIncident(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            Guid entityguid1 = Guid.Parse(Utility.GetAndRemoveParameter(parameters, "id"));
            string StatusCodeString = Utility.GetAndRemoveParameter(parameters, "statuscode");
            int incidentStatusCode = 5;
            if (!String.IsNullOrEmpty(StatusCodeString))
                incidentStatusCode = int.Parse(StatusCodeString);

            // Error String
            StringBuilder sbErrorList = new StringBuilder();
            // Create Update Record data.. 
            Dictionary<string, CrmDataTypeWrapper> updateData = new Dictionary<string, CrmDataTypeWrapper>();
            // parse update entity data. 
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                object data = GetCRMEntityValue(pair.Value);
                if (data == null && !string.IsNullOrEmpty(pair.Value))
                {
                    // If there are prams that could not be parsed for the update.. drop them, log them.
                    sbErrorList.AppendLine(string.Format("Cannot resolve value for update request. Field name: {0} requested value: {1}", pair.Key.ToLower(), pair.Value));
                    continue;
                }
                if (String.IsNullOrEmpty(pair.Key) || data == null)
                {
                    continue;
                }
                updateData.Add(pair.Key.ToLower(), new CrmDataTypeWrapper(data, CrmFieldType.Raw));
            }

            if (sbErrorList.Length > 0)
            {
                // if there is a value here, one or more prams failed to parse,  fail the whole create event with an error indicating what is wrong. 
                // Error happened 
                diagLogger.Log(string.Format("CloseIncident failed, some values failed to convert\n\t{0}\nFailed to convert", sbErrorList.ToString()), TraceEventType.Error);
                throw (new Exception("Failed to CloseIncident requested. Some values were not understood"));
            }
            Guid ret = _client.CrmInterface.CloseIncident(entityguid1, updateData, incidentStatusCode);
            if (ret != null)
                args.ActionReturnValue = ret.ToString();
        }

        void CloseOpportunity(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            Guid entityguid1 = Guid.Parse(Utility.GetAndRemoveParameter(parameters, "id"));
            string StatusCodeString = Utility.GetAndRemoveParameter(parameters, "statuscode");
            int opportunityStatusCode = 3;
            if (!String.IsNullOrEmpty(StatusCodeString))
                opportunityStatusCode = int.Parse(StatusCodeString);

            // Error String
            StringBuilder sbErrorList = new StringBuilder();
            // Create Update Record data.. 
            Dictionary<string, CrmDataTypeWrapper> updateData = new Dictionary<string, CrmDataTypeWrapper>();
            // parse update entity data. 
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                object data = GetCRMEntityValue(pair.Value);
                if (data == null && !string.IsNullOrEmpty(pair.Value))
                {
                    // If there are prams that could not be parsed for the update.. drop them, log them.
                    sbErrorList.AppendLine(string.Format("Cannot resolve value for update request. Field name: {0} requested value: {1}", pair.Key.ToLower(), pair.Value));
                    continue;
                }
                if (String.IsNullOrEmpty(pair.Key) || data == null)
                {
                    continue;
                }
                updateData.Add(pair.Key.ToLower(), new CrmDataTypeWrapper(data, CrmFieldType.Raw));
            }

            if (sbErrorList.Length > 0)
            {
                // if there is a value here, one or more prams failed to parse,  fail the whole create event with an error indicating what is wrong. 
                // Error happened 
                diagLogger.Log(string.Format("CloseOpportunity failed, some values failed to convert\n\t{0}\nFailed to convert", sbErrorList.ToString()), TraceEventType.Error);
                throw (new Exception("Failed to CloseOpportunity requested. Some values were not understood"));
            }
            Guid ret = _client.CrmInterface.CloseOpportunity(entityguid1, updateData, opportunityStatusCode);
            
            if (ret != null)
                args.ActionReturnValue = ret.ToString();
        }

        void CloseQuote(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            Guid entityguid1 = Guid.Parse(Utility.GetAndRemoveParameter(parameters, "id"));
            string StatusCodeString = Utility.GetAndRemoveParameter(parameters, "statuscode");
            int quoteStatusCode = 3;
            if (!String.IsNullOrEmpty(StatusCodeString))
                quoteStatusCode = int.Parse(StatusCodeString);

            // Error String
            StringBuilder sbErrorList = new StringBuilder();
            // Create Update Record data.. 
            Dictionary<string, CrmDataTypeWrapper> updateData = new Dictionary<string, CrmDataTypeWrapper>();
            // parse update entity data. 
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                object data = GetCRMEntityValue(pair.Value);
                if (data == null && !string.IsNullOrEmpty(pair.Value))
                {
                    // If there are prams that could not be parsed for the update.. drop them, log them.
                    sbErrorList.AppendLine(string.Format("Cannot resolve value for update request. Field name: {0} requested value: {1}", pair.Key.ToLower(), pair.Value));
                    continue;
                }
                if (String.IsNullOrEmpty(pair.Key) || data == null)
                {
                    continue;
                }
                updateData.Add(pair.Key.ToLower(), new CrmDataTypeWrapper(data, CrmFieldType.Raw));
            }

            if (sbErrorList.Length > 0)
            {
                // if there is a value here, one or more prams failed to parse,  fail the whole create event with an error indicating what is wrong. 
                // Error happened 
                diagLogger.Log(string.Format("CloseQuote failed, some values failed to convert\n\t{0}\nFailed to convert", sbErrorList.ToString()), TraceEventType.Error);
                throw (new Exception("Failed to CloseQuote requested. Some values were not understood"));
            }
            Guid ret = _client.CrmInterface.CloseQuote(entityguid1, updateData, quoteStatusCode);
            if (ret != null)
                args.ActionReturnValue = ret.ToString();
        }

        void RetrieveEntityOptionSet(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string logicalName = Utility.GetAndRemoveParameter(parameters, "LogicalName");
            string fieldName = Utility.GetAndRemoveParameter(parameters, "FieldName");
            string global = Utility.GetAndRemoveParameter(parameters, "global");
            bool saveInGlobalSession = false;
            if (!String.IsNullOrEmpty(global))
                saveInGlobalSession = bool.Parse(global);
            string appName = Utility.GetAndRemoveParameter(parameters, "appname");
            if (String.IsNullOrEmpty(appName))
            {
                throw new Exception("Appname must be specified in parameters");
            }

            Microsoft.Xrm.Tooling.Connector.CrmServiceClient.PickListMetaElement pickListData = (Microsoft.Xrm.Tooling.Connector.CrmServiceClient.PickListMetaElement)_client.CrmInterface.GetPickListElementFromMetadataEntity(logicalName, fieldName);
            List<LookupRequestItem> lri = new List<LookupRequestItem>();
            foreach (Microsoft.Xrm.Tooling.Connector.CrmServiceClient.PickListItem val in pickListData.Items)
            {
                lri.Add(new LookupRequestItem(val.PickListItemId.ToString(), val.DisplayLabel));
            }

            if (saveInGlobalSession)
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).AddReplaceableParameter(appName, lri);
            }
            else
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).AddReplaceableParameter(appName, lri);
            }
        }

        void RetrieveGlobalOptionSet(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string optionsetName = Utility.GetAndRemoveParameter(parameters, "OptionsetName");
            string global = Utility.GetAndRemoveParameter(parameters, "global");
            bool saveInGlobalSession = false;
            if (!String.IsNullOrEmpty(global))
                saveInGlobalSession = bool.Parse(global);
            string appName = Utility.GetAndRemoveParameter(parameters, "appname");
            if (String.IsNullOrEmpty(appName))
            {
                throw new Exception("Appname must be specified in parameters");
            }

            OptionSetMetadata pickListData = (OptionSetMetadata)_client.CrmInterface.GetGlobalOptionSetMetadata(optionsetName);
            List<LookupRequestItem> lri = new List<LookupRequestItem>();
            foreach (OptionMetadata val in pickListData.Options)
            {
                if (val.Value == null && val.Value.HasValue)
                    continue;
                lri.Add(new LookupRequestItem(val.Value.Value.ToString(), val.Label.UserLocalizedLabel.Label));
            }

            if (saveInGlobalSession)
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.GlobalSession).Customer.DesktopCustomer).AddReplaceableParameter(appName, lri);
            }
            else
            {
                ((DynamicsCustomerRecord)((AgentDesktopSession)localSessionManager.ActiveSession).Customer.DesktopCustomer).AddReplaceableParameter(appName, lri);
            }
        }

        void CopyReplacementParameter(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string fromsessionid = Utility.GetAndRemoveParameter(parameters, "fromsession");
            string tosessionid = Utility.GetAndRemoveParameter(parameters, "tosession");
            string appfrom = Utility.GetAndRemoveParameter(parameters, "appfrom");
            string appto = Utility.GetAndRemoveParameter(parameters, "appto");
            if (String.IsNullOrEmpty(appfrom) || String.IsNullOrEmpty(appto))
            {
                throw new Exception("appfrom and appto must be specified in parameters");
            }
            Session fromsession = localSessionManager.GetSession(Guid.Parse(fromsessionid));
            Session tosession = localSessionManager.GetSession(Guid.Parse(tosessionid));
            Dictionary<string, CRMApplicationData> data = ((DynamicsCustomerRecord)((AgentDesktopSession)fromsession).Customer.DesktopCustomer).GetReplaceableParametersByType(appfrom);
            ((DynamicsCustomerRecord)((AgentDesktopSession)tosession).Customer.DesktopCustomer).MergeReplacementParameter(appto, data, true);
            try
            {
                // CURRENTLY BUGGED FOR GLOBAL CONTROLS
                //ICRMCustomerSearch CRMCustomerSearch = AifServiceContainer.Instance.GetService<ICRMCustomerSearch>();
                //CRMCustomerSearch.CheckUpdateContact(); // notify the system about the updated replacement parameter

                // WORKAROUND FOR ABOVE BUG
                Context c = ((AgentDesktopSession)localSessionManager.ActiveSession).AppHost.GetContext();
                Context context = new Context(c.GetContext());
                if (((AgentDesktopSession)localSessionManager.ActiveSession).Customer == null)
                    return;
                context["ForceChange"] = Guid.NewGuid().ToString();
                FireChangeContext(new ContextEventArgs(context));
            }
            catch
            {
            }

        }

        void ForceDataParameterUpdate(Uii.Csr.RequestActionEventArgs args)
        {
            Context c = ((AgentDesktopSession)localSessionManager.ActiveSession).AppHost.GetContext();
            Context context = new Context(c.GetContext());
            if (((AgentDesktopSession)localSessionManager.ActiveSession).Customer == null)
                return;
            context["ForceChange"] = Guid.NewGuid().ToString();
            FireChangeContext(new ContextEventArgs(context));
        }

        void LookupQueueItem(Uii.Csr.RequestActionEventArgs args)
        {
            #region GetQueueItem

            diagLogger.Log("lookupQueueItem action called", TraceEventType.Verbose);
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string id = Utility.GetAndRemoveParameter(parameters, "id");
            string entityType = Utility.GetAndRemoveParameter(parameters, "entitytype");
            string sessionId = Utility.GetAndRemoveParameter(parameters, "sessionid");
            if (String.IsNullOrEmpty(sessionId))
                sessionId = localSessionManager.ActiveSession.SessionId.ToString();
            Session session = localSessionManager.GetSession(Guid.Parse(sessionId));
            DynamicsCustomerRecord custRec = ((AgentDesktopSession)session).Customer.DesktopCustomer as DynamicsCustomerRecord;

            Guid guId = Guid.Empty;
            if (Guid.TryParse(id, out guId))
            {
                Dictionary<string, string> searchParam = new Dictionary<string, string>();
                searchParam.Add("objectid", guId.ToString());
                var results = _client.CrmInterface.GetEntityDataBySearchParams("queueitem", searchParam, CrmServiceClient.LogicalSearchOperator.None, null);
                if (results != null && results.Count > 0)
                {
                    Entity queueItem = _client.CrmInterface.ConvertResponseToEntity("queueitem", "queueitemid", results.FirstOrDefault().Value);
                    custRec.AddReplaceableParameter(this.ApplicationName, "queueitem", EntityDescription.FromEntity(queueItem));
                }
                else
                {
                    // not found...
                    throw (new Exception(string.Format("No queue item found for id {0} . Error was: {1}", id, "No data returned from search, or no Id was present")));
                }
            }
            else
            {
                // could not resolve id... 
                throw (new Exception(string.Format("Failed to resolve queue item id. Error was: {0}", entityType, "Unable Parse Id data, or no Id was present")));
            }
            #endregion
        }

        void HookErrorEvents(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string stop = Utility.GetAndRemoveParameter(parameters, "stop");
            if (stop == "true")
                CRMWindowRouter.actionHistory.CollectionChanged -= actionHistory_CollectionChanged;
            else
                CRMWindowRouter.actionHistory.CollectionChanged += actionHistory_CollectionChanged;
        }

        void SetAppBar(Uii.Csr.RequestActionEventArgs args)
        {
            #region SetAppBar
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSessionManager);
            string appName = Utility.GetAndRemoveParameter(parameters, "ApplicationName");
            string width = Utility.GetAndRemoveParameter(parameters, "Width");
            string height = Utility.GetAndRemoveParameter(parameters, "Height");
            string edge = Utility.GetAndRemoveParameter(parameters, "Edge");
            string ScreenString = Utility.GetAndRemoveParameter(parameters, "Screen");
            int Screen = 0;
            int.TryParse(ScreenString, out Screen);
            Window w = Application.Current.MainWindow;
            if (!String.IsNullOrEmpty(appName))
            {
                IHostedApplication app = GetApp(appName);
                w = Utilities.Win32.GetParentWindow(app as System.Windows.DependencyObject);
            }
            if (String.IsNullOrEmpty(edge))
                edge = "Top";

            if (w != null)
            {
                double temp;
                if (double.TryParse(width, out temp))
                    w.Width = temp;
                if (double.TryParse(height, out temp))
                    w.Height = temp;
                Application.Current.MainWindow.WindowState = WindowState.Normal;
                if (edge.Equals("Bottom", StringComparison.InvariantCultureIgnoreCase))
                    Microsoft.USD.ComponentLibrary.Utilities.AppBarFunctions.SetAppBar(w, Microsoft.USD.ComponentLibrary.Utilities.ABEdge.Bottom, Screen);
                else if (edge.Equals("Left", StringComparison.InvariantCultureIgnoreCase))
                    Microsoft.USD.ComponentLibrary.Utilities.AppBarFunctions.SetAppBar(w, Microsoft.USD.ComponentLibrary.Utilities.ABEdge.Left, Screen);
                else if (edge.Equals("Right", StringComparison.InvariantCultureIgnoreCase))
                    Microsoft.USD.ComponentLibrary.Utilities.AppBarFunctions.SetAppBar(w, Microsoft.USD.ComponentLibrary.Utilities.ABEdge.Right, Screen);
                else
                    Microsoft.USD.ComponentLibrary.Utilities.AppBarFunctions.SetAppBar(w, Microsoft.USD.ComponentLibrary.Utilities.ABEdge.Top, Screen);
            }
            #endregion
        }

        void ExitApplication(Uii.Csr.RequestActionEventArgs args)
        {
            Application.Current.Shutdown();
        }

        void DetermineFocus(Uii.Csr.RequestActionEventArgs args)
        {
            var fe = FocusManager.GetFocusedElement(Application.Current.MainWindow);
            if (fe == null)
                args.ActionReturnValue = "Focus not found";
            else if (fe is UIElement)
                args.ActionReturnValue = ((UIElement)fe).ToString();
            else
                args.ActionReturnValue = "Type = " + fe.GetType();
        }

        private void DoSearchAction(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> lines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            if (lines.Count < 1)
                return;
            string global = Utility.GetAndRemoveParameter(lines, "global");
            bool saveInGlobalSession = false;
            if (!String.IsNullOrEmpty(global))
                bool.TryParse(global, out saveInGlobalSession);
            string resetString = Utility.GetAndRemoveParameter(lines, "reset");
            bool reset = false;
            if (!String.IsNullOrEmpty(resetString))
                bool.TryParse(resetString, out reset);
            string srecordlimit = Utility.GetAndRemoveParameter(lines, "maxcount");
            int maxCount = -1;
            if (!String.IsNullOrEmpty(srecordlimit))
                int.TryParse(srecordlimit, out maxCount);
            string spageNumber = Utility.GetAndRemoveParameter(lines, "pageNumber");
            int pageNumber = -1;
            if (!String.IsNullOrEmpty(spageNumber))
                int.TryParse(spageNumber, out pageNumber);

            string spageCount = Utility.GetAndRemoveParameter(lines, "pageCount");
            int pageCount = -1;
            if (!String.IsNullOrEmpty(spageCount))
                int.TryParse(spageCount, out pageCount);

            string spageCookie = Utility.GetAndRemoveParameter(lines, "pageCookie");
            string pageCookie = string.Empty;
            if (!String.IsNullOrEmpty(spageCookie))
                pageCookie = spageCookie;

            string sessionId = Utility.GetAndRemoveParameter(lines, "sessionid");
            if (String.IsNullOrEmpty(sessionId))
                sessionId = localSessionManager.ActiveSession.SessionId.ToString();
            Session session = localSessionManager.GetSession(Guid.Parse(sessionId));

            string fetchXml = Utility.RemainderParameter(lines);
            fetchXml = Utility.GetContextReplacedString(fetchXml, CurrentContext, localSession);
            if (Utility.IsAllReplacementValuesReplaced(fetchXml))
            {
                bool isMoreRecords = false;
                EntityCollection result = _client.CrmInterface.GetEntityDataByFetchSearchEC(fetchXml, 1, pageNumber, pageCookie, out pageCookie, out isMoreRecords);
                Trace.WriteLine("DoSearch Fetch: " + fetchXml);
                if (result == null && _client.CrmInterface.LastCrmException != null)
                {
                    diagLogger.Log(_client.CrmInterface.LastCrmException);
                    throw _client.CrmInterface.LastCrmException;
                }
                foreach (Entity c in result.Entities)
                {
                    // update replacement parameters
                    EntityDescription entityDesc = EntityDescription.FromEntity(c);


                    // do onload
                    Trace.WriteLine("----------------------------------------------");
                    foreach (CRMApplicationData appdata in entityDesc.data.Values)
                    {
                        Trace.WriteLine("\t" + appdata.name + ":" + appdata.value);
                    }
                }
            }
        }

        /// <summary>
        /// Action designed to load the current CRM theme settings from the CRM server and place the values into data parameters.
        /// This action does NOT automatically apply them in case the admin wants to modify or add to them before they are applied to the style
        /// </summary>
        /// <param name="args"></param>
        void LoadCRMTheme(Uii.Csr.RequestActionEventArgs args)
        {
            string fetchXml = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                              <entity name=""{0}"">
                               <all-attributes/>
                              </entity>
                              </fetch>";

            fetchXml = String.Format(fetchXml, "theme");
            string pageCookie = "";
            bool isMoreRecords = false;
            EntityCollection result = _client.CrmInterface.GetEntityDataByFetchSearchEC(fetchXml, 50, 1, pageCookie, out pageCookie, out isMoreRecords);
            if (result == null && _client.CrmInterface.LastCrmException != null)
            {
                diagLogger.Log(_client.CrmInterface.LastCrmException);
                throw _client.CrmInterface.LastCrmException;
            }
            Entity defaultTheme = null;
            foreach (Entity c in result.Entities)
            {
                foreach (KeyValuePair<string, object> a in c.Attributes)
                {
                    if (a.Value != null)
                    {
                        if (a.Key.Equals("isdefaulttheme", StringComparison.InvariantCultureIgnoreCase)
                            && (bool)a.Value == true)
                            defaultTheme = c;
                    }
                }
            }


            DynamicsCustomerRecord custRec = ((AgentDesktopSession)localSession).Customer.DesktopCustomer as DynamicsCustomerRecord;
            Dictionary<string, CRMApplicationData> data;
            if (defaultTheme != null)
            {
                data = EntityDescription.FromEntity(defaultTheme).data;
            }
            else if (result.Entities.Count > 0)
            {
                data = EntityDescription.FromEntity(result.Entities[0]).data;
            }
            else
            {
                throw new Exception("No selectable theme found in CRM");
            }

            System.Drawing.ColorConverter cc = new System.Drawing.ColorConverter();
            data.Add("navbarshelftextcolor", new CRMApplicationData() { name = "navbarshelftextcolor", type = "string", value = cc.ConvertToString(IdealTextColor((System.Drawing.Color)cc.ConvertFromString(data["navbarshelfcolor"].value))) });
            data.Add("navbarbackgroundtextcolor", new CRMApplicationData() { name = "navbarbackgroundtextcolor", type = "string", value = cc.ConvertToString(IdealTextColor((System.Drawing.Color)cc.ConvertFromString(data["navbarbackgroundcolor"].value))) });
            custRec.MergeReplacementParameter(this.ApplicationName, data);
        }

        /// <summary>
        /// Function that attempts to pick an ideal text color, either white or black
        /// </summary>
        /// <param name="bg"></param>
        /// <returns></returns>
        public System.Drawing.Color IdealTextColor(System.Drawing.Color bg)
        {
            int nThreshold = 105;
            int bgDelta = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) +
                                            (bg.B * 0.114));

            System.Drawing.Color foreColor = (255 - bgDelta < nThreshold) ? System.Drawing.Color.Black : System.Drawing.Color.White;
            return foreColor;
        }

        /// <summary>
        /// This action loads the style from the XML web resource named msdyusd_CRMResourceMerge
        /// If this web resource exists on the server, it will be loaded, otherwise a default one embedded in the resources of this DLL will be loaded
        /// This resource dictionary has replacement parameters that can be used to substitute colors into the file.  
        /// This function passes the resources for the local hosted control as parameters so that your replacement parameters don't require the controllers applicationname, thus making it more portable.
        /// Once loaded, it will replace the replacement parameters, then apply the style
        /// Once the style is loaded, it will look for a control in your control tree with the name=Logo (usually found in your custom XAML layout).  This should be an image
        /// If this image control is found, it will load up the image configured for the CRM style and set the tooltip according to the CRM theme.
        /// </summary>
        /// <param name="args"></param>
        void ApplyCRMTheme(Uii.Csr.RequestActionEventArgs args)
        {
            string resourceMerge = String.Empty;

            object objRes = ((CRMGlobalManager)CRMWindowRouter).GetCRMWebResource("msdyusd_CRMResourceMerge");
            if (objRes != null)
            {
                byte[] b = Convert.FromBase64String((string)objRes);
                resourceMerge = System.Text.Encoding.UTF8.GetString(b);
                // trim the first char - not sure why CRM is adding it
                resourceMerge = resourceMerge.Remove(0, 1);
            }

            if (String.IsNullOrEmpty(resourceMerge))
            {
                using (System.IO.Stream resourceStream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("Microsoft.USD.ComponentLibrary.Resources.CRMResourceMerge.xml"))
                {
                    if (resourceStream != null)
                    {
                        using (StreamReader sr = new StreamReader(resourceStream))
                        {
                            resourceMerge = sr.ReadToEnd();
                        }
                    }
                }
            }

            // use the replacement parameters for the local hosted control as default ones
            List<LookupRequestItem> lri = new List<LookupRequestItem>();
            DynamicsCustomerRecord custRec = ((AgentDesktopSession)localSession).Customer.DesktopCustomer as DynamicsCustomerRecord;
            if (custRec.CapturedReplacementVariables.ContainsKey(this.ApplicationName))
            {
                Dictionary<string, CRMApplicationData> data = custRec.CapturedReplacementVariables[ApplicationName];
                if (data != null)
                {
                    foreach (CRMApplicationData item in data.Values)
                    {
                        lri.Add(new LookupRequestItem() { Key = item.name, Value = item.value });
                    }
                }
            }
            resourceMerge = Utility.GetContextReplacedString(resourceMerge, CurrentContext, localSession, lri);
            ActionDefinition ad = new ActionDefinition()
            {
                Application = ((DynamicsBaseHostedControl)CRMWindowRouter).ApplicationName,
                Action = "SetTheme",
                ActionData = resourceMerge,
                Condition = ""
            };
            CRMWindowRouter.ExecuteActions(new List<ActionDefinition>() { { ad } }, String.Empty, new Dictionary<string, string>());

            //logotooltip = Microsoft Dynamics CRM
            //headercolor = #1160B7
            //selectedlinkeffect = #B1D6F0
            //defaultcustomentitycolor = #006551
            //globallinkcolor = #1160B7
            //processcontrolcolor = #0755BE
            //controlborder = #CCCCCC
            //navbarbackgroundcolor = #002050
            //hoverlinkeffect = #D7EBF9
            //defaultentitycolor = #001CA5
            //navbarshelfcolor = #DFE2E8
            //controlshade = #F3F1F1
            //logoid.name = CRM Web Resource image

            Window w = Application.Current.MainWindow;
            System.Windows.Controls.Image objLogo = (System.Windows.Controls.Image)FindVisualChildByName(w, "Logo");
            if (objLogo != null)
            {
                CRMImageConverter ic = new CRMImageConverter();
                string logoimagename = Utility.GetContextReplacedString("[[" + this.ApplicationName + ".logoid.name]g]", CurrentContext, localSession, lri);
                ((System.Windows.Controls.Image)objLogo).Source = (BitmapImage)ic.Convert(logoimagename, this.GetType(), null, System.Globalization.CultureInfo.CurrentCulture);

                string logotooltip = Utility.GetContextReplacedString("[[" + this.ApplicationName + ".logotooltip]g]", CurrentContext, localSession, lri);
                //[[USDController.logotooltip]g]
                ((System.Windows.Controls.Image)objLogo).ToolTip = logotooltip;
            }
        }

        /// <summary>
        /// Helper function for ApplyCRMTheme
        /// </summary>
        /// <param name="child"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Object FindVisualChildByName(UIElement child, string name)
        {
            if (child is DependencyObject)
            {
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                    return child;
            }

            FrameworkElement fe = child as FrameworkElement;
            if (fe != null)
            {
                try { fe.ApplyTemplate(); }
                catch { }

                int childrenCount = VisualTreeHelper.GetChildrenCount(fe);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child1 = VisualTreeHelper.GetChild(child, i);
                    Object obj = FindVisualChildByName(child1 as UIElement, name);
                    if (obj != null)
                        return obj;
                }
            }
            ContentControl c = child as ContentControl;
            if (c != null)
            {
                Object obj = FindVisualChildByName(c.Content as UIElement, name);
                if (obj != null)
                    return obj;
            }
            ItemsControl ic = child as ItemsControl;
            if (ic != null)
            {
                foreach (Object elem in ic.Items)
                {
                    if (elem is UIElement)
                    {
                        Object obj = FindVisualChildByName(elem as UIElement, name);
                        if (obj != null)
                            return obj;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Support Functions
        void actionHistory_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                foreach (ActionResult ar in e.NewItems)
                {
                    if (ar.ConditionResult == ConditionResult.ActionFailed
                        || ar.ConditionResult == ConditionResult.ConditionFailed)
                    {
                        Dictionary<string, string> arParams = new Dictionary<string, string>();
                        arParams.Add("Action", ar.Action);
                        arParams.Add("ActionData", ar.ActionData);
                        if (ar.AgentScriptActionId != null)
                            arParams.Add("AgentScriptActionId", ar.AgentScriptActionId.ToString());
                        arParams.Add("Application", ar.Application);
                        arParams.Add("Condition", ar.Condition);
                        arParams.Add("Created", ar.Created.ToString());
                        arParams.Add("DurationMS", ar.Duration.Milliseconds.ToString());
                        if (ar.exception != null)
                        {
                            arParams.Add("ExceptionSource", ar.exception.Source);
                            arParams.Add("ExceptionHResult", ar.exception.HResult.ToString());
                            arParams.Add("ExceptionMessage", ar.exception.Message);
                            arParams.Add("ExceptionStackTrace", ar.exception.StackTrace);
                            if (ar.exception.InnerException != null)
                            {
                                arParams.Add("ExceptionInnerExceptionSource", ar.exception.InnerException.Source);
                                arParams.Add("ExceptionInnerExceptionSource", ar.exception.InnerException.HResult.ToString());
                                arParams.Add("ExceptionInnerExceptionMessage", ar.exception.InnerException.Message);
                                arParams.Add("ExceptionInnerExceptionStackTrace", ar.exception.InnerException.StackTrace);
                            }
                        }
                        arParams.Add("Name", ar.Name);
                        arParams.Add("Parameters", ar.Parameters);
                        arParams.Add("Result", ar.Result);
                        arParams.Add("SessionId", ar.SessionId);
                        arParams.Add("Source", ar.Source);
                        arParams.Add("Type", ar.Type.ToString());
                        arParams.Add("ConditionResult", ar.ConditionResult.ToString());
                        if (ar.ConditionResult == ConditionResult.ActionFailed)
                        {
                            FireEvent("ActionFailed", arParams);
                        }
                        else if (ar.ConditionResult == ConditionResult.ConditionFailed)
                        {
                            FireEvent("ConditionFailed", arParams);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in ActionHistory collectionChanged " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public object GetValueFromEntity(string id, string logicalname, string fieldValue)
        {
            string fetchXml = @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                              <entity name=""{0}"">
                               <attribute name=""{2}"" />
                               <filter type=""and"">
                                 <condition attribute=""{0}id"" operator=""eq"" uitype=""{0}"" value=""{1}"" />
                                </filter>
                              </entity>
                              </fetch>";
            fetchXml = String.Format(fetchXml, logicalname, id, fieldValue);
            string pageCookie = "";
            bool isMoreRecords = false;
            EntityCollection result = _client.CrmInterface.GetEntityDataByFetchSearchEC(fetchXml, 1, 1, pageCookie, out pageCookie, out isMoreRecords);
            if (result == null && _client.CrmInterface.LastCrmException != null)
            {
                diagLogger.Log(_client.CrmInterface.LastCrmException);
                throw _client.CrmInterface.LastCrmException;
            }
            Entity c = result.Entities.FirstOrDefault();
            return c[fieldValue];
        }

        /// <summary>
        /// Parses and creates the CRM entity data Value for insert. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal static object GetCRMEntityValue(string value)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            // PartyList(email["test@test.com"], email["test2@test.com"], er["contact", guid])
            // EntityReference("logicalname", "id")
            // OptionSetValue(value)
            // Boolean(value)
            // value

            try
            {
                if (value.Trim().StartsWith("EntityReference"))
                {
                    string temp = value.Trim().Substring("EntityReference".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    if (parms.Length == 2)
                    {
                        temp = parms[1].Trim(new char[] { '\"', ' ' });
                        string logicalName = parms[0].Trim('\"');
                        int iTemp = 0;
                        if (int.TryParse(logicalName, out iTemp)) // numeric version
                        {
                            Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter>();
                            logicalName = CRMWindowRouter.EntityNameFromType(iTemp);
                        }
                        return new EntityReference(logicalName, Guid.Parse(temp));
                    }
                    else if (parms.Length == 3)
                    {
                        temp = parms[1].Trim(new char[] { '\"', ' ' });
                        string logicalName = parms[0].Trim('\"');
                        int iTemp = 0;
                        if (int.TryParse(logicalName, out iTemp)) // numeric version
                        {
                            Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter>();
                            logicalName = CRMWindowRouter.EntityNameFromType(iTemp);
                        }
                        EntityReference entref = new EntityReference(logicalName, Guid.Parse(temp));
                        entref.Name = parms[2].Trim(new char[] { '\"', ' ' });

                        return entref;
                    }
                    else
                        return null;
                }
                else if (value.Trim().StartsWith("PartyList"))
                {
                    string temp = value.Trim().Substring("PartyList".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = @"(er|email)\[[^\]]+\]";
                    Regex r = new Regex(pattern);

                    MatchCollection parms = r.Matches(temp);
                    List<Entity> partyList = new List<Entity>();
                    foreach (Match party in parms)
                    {
                        if (party.Value.StartsWith("email"))
                        {
                            //email["test@test.com"]
                            string partyedit = party.Value.Remove(0, 5);
                            partyedit = partyedit.Trim(new char[] { '[', ']' });
                            partyedit = partyedit.Trim(new char[] { '\"', ' ' });
                            Entity em = new Entity("activityparty");
                            em.Attributes.Add("addressused", partyedit);
                            partyList.Add(em);
                        }
                        else if (party.Value.StartsWith("er"))
                        {
                            //email["test@test.com"]
                            string partyedit = party.Value.Remove(0, 2);
                            partyedit = partyedit.Trim(new char[] { '[', ']' });
                            Object obj = GetCRMEntityValue("EntityReference(" + partyedit + ")");
                            if (obj != null)
                            {
                                Entity em = new Entity("activityparty");
                                em.Attributes.Add("partyid", obj);
                                partyList.Add(em);
                            }
                        }
                    }
                    return partyList.ToArray();
                }
                else if (value.Trim().StartsWith("OptionSetValue"))
                {
                    string temp = value.Trim().Substring("OptionSetValue".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return new OptionSetValue(int.Parse(parms[0]));
                }
                else if (value.Trim().StartsWith("Boolean"))
                {
                    string temp = value.Trim().Substring("Boolean".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return bool.Parse(parms[0]);
                }
                else if (value.Trim().StartsWith("DateTime"))
                {
                    string temp = value.Trim().Substring("DateTime".Length);
                    try
                    {
                        temp = temp.Trim(new char[] { '(', ')', ' ' });
                        DateTime dtTemp;
                        if (temp.Equals("NOW", StringComparison.InvariantCultureIgnoreCase))
                            dtTemp = DateTime.Now;
                        else if (temp.Equals("TODAY", StringComparison.InvariantCultureIgnoreCase))
                            dtTemp = DateTime.Today;
                        else
                            dtTemp = DateTime.Parse(temp);
                        return dtTemp.ToUniversalTime();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format("{0}\r\n{1}", ex.Message, temp));
                    }
                }
                else if (value.Trim().StartsWith("Decimal"))
                {
                    string temp = value.Trim().Substring("Decimal".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return double.Parse(parms[0]);
                }
                else if (value.Trim().StartsWith("Float"))
                {
                    string temp = value.Trim().Substring("Float".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return float.Parse(parms[0]);
                }
                else if (value.Trim().StartsWith("Money"))
                {
                    string temp = value.Trim().Substring("Money".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return double.Parse(parms[0]);
                }
                else if (value.Trim().StartsWith("Int"))
                {
                    string temp = value.Trim().Substring("Int".Length);
                    temp = temp.Trim(new char[] { '(', ')', ' ' });
                    string pattern = ",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))";
                    Regex r = new Regex(pattern);
                    string[] parms = r.Split(temp);
                    return int.Parse(parms[0]);
                }
                else if (value.Trim().StartsWith("$Escaped"))
                {
                    return Utility.Unescape(value.Trim());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                return null;
            }
            return value;
        }

        internal static CRMApplicationData UsdEntityDescription(string crmObjName, Object crmObj)
        {
            if (crmObj.GetType().Name == "EntityReference")
            {
                //transactioncurrencyid:lookup:transactioncurrency,%7B25C269A4-A05C-E011-B859-00155DAAA605%7D,US%20Dollar&
                string val = HttpUtility.UrlEncode(((EntityReference)crmObj).LogicalName) + "," +
                HttpUtility.UrlEncode(((EntityReference)crmObj).Id.ToString()) + "," +
                HttpUtility.UrlEncode(((EntityReference)crmObj).Name);
                return new CRMApplicationData() { name = crmObjName, type = "lookup", value = val };
            }
            else if (crmObj.GetType().Name == "EntityCollection")
            {
                //Dictionary<string, int> participantCount = new Dictionary<string, int>();
                //foreach (Entity e in ((EntityCollection)crmObj).Entities)
                //{
                //    try
                //    {
                //        string participationtypemask = e.FormattedValues["participationtypemask"];
                //        if (!participantCount.ContainsKey(participationtypemask))
                //            participantCount.Add(participationtypemask, 0);
                //        int participantNum = participantCount[participationtypemask]++;
                //        EntityReference er = (EntityReference)e.Attributes["partyid"];
                //        string val = HttpUtility.UrlEncode(er.LogicalName) + "," +
                //        HttpUtility.UrlEncode(er.Id.ToString()) + "," +
                //        HttpUtility.UrlEncode(er.Name);
                //        appdataList.Add(participationtypemask + "_" + participantNum.ToString(CultureInfo.InvariantCulture), new CRMApplicationData() { name = participationtypemask + "_" + participantNum.ToString(CultureInfo.InvariantCulture), type = "lookup", value = val });

                //        if (key != participationtypemask)
                //        {
                //            if (!participantCount.ContainsKey(key))
                //                participantCount.Add(key, 0);
                //            participantNum = participantCount[key]++;
                //            appdataList.Add(key + "_" + participantNum.ToString(CultureInfo.InvariantCulture), new CRMApplicationData() { name = key + "_" + participantNum.ToString(CultureInfo.InvariantCulture), type = "lookup", value = val });
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        DynamicsLogger.Logger.Log(string.Format(CultureInfo.InvariantCulture, "Error parsing {0} of type {1}\r\n{2}", key, "EntityCollection", ex.Message));
                //    }
                //}
                return null;
            }
            else if (crmObj.GetType().Name == "OptionSetValue")
            {
                string valueText = ((OptionSetValue)crmObj).Value.ToString(CultureInfo.InvariantCulture) + ",";
                valueText = valueText.Replace("\r", "\\r").Replace("\n", "\\n"); // escape newlines
                return new CRMApplicationData() { name = crmObjName, type = crmObj.GetType().Name, value = valueText };
            }
            else if (crmObj.GetType().Name == "Money")
            {
                string valueText = ((Money)crmObj).Value.ToString(CultureInfo.InvariantCulture);
                valueText = valueText.Replace("\r", "\\r").Replace("\n", "\\n"); // escape newlines
                return new CRMApplicationData() { name = crmObjName, type = crmObj.GetType().Name, value = valueText };
            }
            else if (crmObj.GetType().Name == "DateTime")
            {
                string valueText = ((DateTime)crmObj).ToLocalTime().ToString();
                return new CRMApplicationData() { name = crmObjName, type = crmObj.GetType().Name, value = valueText };
            }
            else if (crmObj.GetType().Name == "AliasedValue")
            {
                CRMApplicationData dataParams = new CRMApplicationData
                {
                    name = ((Microsoft.Xrm.Sdk.AliasedValue)crmObj).EntityLogicalName + "." + ((Microsoft.Xrm.Sdk.AliasedValue)crmObj).AttributeLogicalName,
                    type = crmObj.GetType().Name,
                    value = ((Microsoft.Xrm.Sdk.AliasedValue)crmObj).Value.ToString()
                };
                return dataParams;
            }
            else
            {
                string valueText = crmObj.ToString();
                return new CRMApplicationData() { name = crmObjName, type = crmObj.GetType().Name, value = valueText };
            }
        }

        void Alert(Uii.Csr.RequestActionEventArgs args)
        {
            MessageBox.Show(args.Data);
        }
    
        public string CreateEntityUrl(string entity, string id, string extraqs, bool displayRibbon)
        {
            if (String.IsNullOrEmpty(entity))
                return "";
            string baseurl = Utility.GetCrmUiUrl();
            if (displayRibbon == false)
                baseurl += "/"; // is this a hack?  
               //http://rushen1591/rushen1591Org/main.aspx?etc=112&extraqs=%3fetc%3d112%26id%3d%257b848ADA38-9F77-E311-8CC5-00155DB725FC%257d%26preloadcache%3d1389905237572&histKey=856001708&newWindow=true&pagetype=entityrecord#218029546
            return String.Format("{0}/main.aspx?etn={1}&id={2}&extraqs={3}&pagetype=entityrecord", baseurl.TrimEnd('/'), entity, System.Web.HttpUtility.UrlEncode(id), extraqs);
        }

        List<eventTimer> eventTimerList = new List<eventTimer>();
        class eventTimer
        {
            public string appname;
            public string eventname;
            public string milliseconds;
            public System.Threading.Timer timer;
            public Session session;
            public Dictionary<string, string> parameters = new Dictionary<string, string>();
        }
        private void EventTimer(object obj)
        {
            eventTimer t = obj as eventTimer;

            Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new System.Action(() =>
            {
                // do work
                try
                {
                    lock (eventTimerList)
                        eventTimerList.Remove(t);
                    t.timer.Dispose();
                    CRMWindowRouter.FireEvent(t.session, !String.IsNullOrEmpty(t.appname) ? t.appname : this.ApplicationName, t.eventname, t.parameters);
                }
                catch { }
            }));
        }
        #endregion
    }
}
