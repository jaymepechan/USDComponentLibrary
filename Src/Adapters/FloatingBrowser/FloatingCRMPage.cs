/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr.Browser.Web;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Xrm.Sdk;
using mshtml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
    public class FloatingCRMPage : FloatingBrowser
    {
        #region Constructors
        bool dataCaptured = false;
        private System.Collections.Generic.Dictionary<string, CRMApplicationData> data = null;
        private bool crmUri = false;

        public FloatingCRMPage()
        {
            
        }

        public FloatingCRMPage(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            
        }
        #endregion

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("Navigate", Navigate);
            RegisterAction("RunScript", RunScript);
            RegisterAction("CloseActive", CloseActive);
            RegisterAction("GoHome", GoHome);
            RegisterAction("Refresh", Refresh);
            RegisterAction("GoBack", GoBack);
            RegisterAction("GoForward", GoForward);
            RegisterAction("ReRoute", ReRoute);
            RegisterAction("RunXRMCommand", RunXRMCommand);
            RegisterAction("StartDialog", StartDialog);
            RegisterAction("Find", Find);
            RegisterAction("ScanForDataParameters", ScanForDataParameters);
            RegisterAction("LoadDataParameters", LoadDataParameters);
        }

        #region Browser Events

        protected override void Browser_NewWindow3(object sender, NewWindow3EventArgs e)
        {
            string url = e._bstrUrl;
            if (url.StartsWith("http://event.usd/?"))
            {
                if (crmUri == true)
                {   // this is a popup on a CRM page
                    e.Cancel = !CRMPopupRequested(url, "");
                    return;
                }

                string eventName = url.Substring(14);
                NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(eventName);

                if (queryParams["eventname"] != null)
                {
                    Dictionary<string, string> parms = ExtractParameters(queryParams);
                    FireEvent(queryParams["eventname"], parms);
                }
                e.Cancel = true;
                return;
            }
            else
            {
                ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                Microsoft.Crm.UnifiedServiceDesk.Dynamics.CRMGlobalManager.WindowRoute ruleResult = CRMWindowRouter.DoRouteDetermineRule(localSession, ApplicationName, null, null, url, String.Empty, CRMGlobalManager.RouteType.Popup, null);
                if (ruleResult == null)
                {
                    return;  // due to bug in Global Manager, WindowRouteAction.Default results in this.
                }
                switch (ruleResult.action)
                {
                    case CRMGlobalManager.WindowRouteAction.None:
                        // run actions.
                        CRMWindowRouter.ExecuteActions(localSession, ruleResult.actionDefinitions, String.Empty, new Dictionary<string, string>() { { "SUBJECTURL", url } });
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleResult, new Dictionary<string, string>() { { "SUBJECTURL", url } });
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                        e.Cancel = true;
                        break;
                    case CRMGlobalManager.WindowRouteAction.Default:
                    case CRMGlobalManager.WindowRouteAction.ShowOutside:
                        CRMWindowRouter.ExecuteActions(localSession, ruleResult.actionDefinitions, String.Empty, new Dictionary<string, string>() { { "SUBJECTURL", url } });
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleResult, new Dictionary<string, string>() { { "SUBJECTURL", url } });
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                        e.Cancel = false;
                        break;
                    case CRMGlobalManager.WindowRouteAction.InPlace:
                    case CRMGlobalManager.WindowRouteAction.CreateSession:
                    case CRMGlobalManager.WindowRouteAction.RouteWindow:
                        CRMWindowRouter.DoRoutePopup(localSession, ApplicationName, url, String.Empty, true, true);
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                        e.Cancel = true;
                        return;
                }
            }
        }

        protected override void PreNavigate(string urlString)
        {
            base.PreNavigate(urlString);

            string tempurl = Utility.EnsureQualifiedUrl(urlString);
            Uri uri = new Uri(tempurl);
            NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(uri.Query);
            if (queryParams["etc"] != null || queryParams["etn"] != null || tempurl != urlString || tempurl.IndexOf("Quickfind") > -1 ) // only CRM has url's like this. relative urls are assumed to be CRM url's as well
            {
                crmUri = true;
                FireEvent("CRMPageLoading", new Dictionary<string, string>() { { "url", uri.OriginalString } });
            }
            else
                crmUri = false;
        }

        protected override void Browser_DocumentComplete(object sender, DocumentCompleteEventArgs e)
        {
            DocumentCompleteStandardProcessing();

            if (crmUri == true)
            {
                IWebBrowser2 htmlBrowser = e.pDisp as IWebBrowser2;
                BlockPopupErrors(htmlBrowser);

                if (HasCRMForm((e.pDisp as IWebBrowser2).Document as HTMLDocument))
                {
                    string str = String.Empty;
                    using (System.IO.Stream stream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("Microsoft.USD.ComponentLibrary.Adapters.MCSBrowser.EventHook2013.js"))
                    {
                        if (stream != null)
                        {
                            using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                            {
                                str = reader.ReadToEnd();
                                RunScript(str, string.Empty);
                            }
                        }
                    }
                    browserAquiredStart = DateTime.Now;
                    browserAsyncComplette = new System.Threading.Timer(new TimerCallback(AsyncCompleteEvents), "", TimeSpan.FromMilliseconds(300), TimeSpan.Zero);
                }
            }
        }
        DateTime browserAquiredStart;
        System.Threading.Timer browserAsyncComplette;
        void AsyncCompleteEvents(object obj)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                FireEvent("CRMFormLoaded", new Dictionary<string, string>() { { "url", Url.OriginalString }, { "crmpage", "true" } });
                FireEvent("BrowserDocumentComplete", new Dictionary<string, string>() { { "url", Url.OriginalString }, { "crmpage", "true" } });
            }));
        }

        private void DocumentCompleteStandardProcessing()
        {
            Dispatcher.Invoke(new System.Action(() =>
            {
                try
                {
                    DynamicsCustomerRecord customerRecord = ((DynamicsCustomerRecord)((AgentDesktopSession)localSession).Customer.DesktopCustomer);
                    Dictionary<string, CRMApplicationData> data = new Dictionary<string, CRMApplicationData>();
                    data.Add("url", new CRMApplicationData() { name = "url", type = "string", value = Url.ToString() });
                    data.Add("pagelocation", new CRMApplicationData() { name = "pagelocation", type = "string", value = BrowserId });
                    if (Document != null)
                        data.Add("pagetitle", new CRMApplicationData() { name = "pagetitle", type = "string", value = ((HTMLDocument)Document).title });
                    customerRecord.MergeReplacementParameter(ApplicationName, data);
                }
                catch
                {
                }
                string url = string.Empty;
                try
                {
                    url = web.LocationURL;
                }
                catch
                {
                }

                try
                {
                    FireEvent("PageLoadComplete", new Dictionary<string, string>() { { "url", ((HTMLDocument)Document).url } });
                    if (crmUri == false)    // CRM URI's are processed after data is captured and available
                        FireEvent("BrowserDocumentComplete", new Dictionary<string, string>() { { "url", url } });
                }
                catch
                {
                }
            }));
        }
        #endregion

        #region Action Handlers
        /// <summary>
        /// Raised when an action is sent to this control
        /// </summary>
        /// <param name="args">args for the action</param>
        void Navigate(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string frameName = Utility.GetAndRemoveParameter(parameters, "frame");
            Trace.WriteLine(this.ApplicationName + ":" + args.Action);
            string url = Utility.GetAndRemoveParameter(parameters, "url");
            string remainder = Utility.RemainderParameter(parameters);
            if (String.IsNullOrEmpty(url))
                url = remainder;
            url = Utility.EnsureQualifiedUrl(url);
            Navigate(url);
            return;
        }
        void RunScript(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string frameName = Utility.GetAndRemoveParameter(parameters, "frame");
            string remainder = Utility.RemainderParameter(parameters);
            args.ActionReturnValue = RunScript(Utility.GetContextReplacedString(remainder, CurrentContext, localSession), frameName) as string;
            return;
        }

        void CloseActive(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            desktopAccess.CloseDynamicApplication(this.ApplicationName);
            return;
        }

        void GoHome(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            GoHome();
            return;
        }

        void Refresh(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            if (!this.Refresh())
            {
                if (webBrowser is System.Windows.Controls.WebBrowser)
                    ((System.Windows.Controls.WebBrowser)webBrowser).Refresh(true);
            }
            return;
        }

        void GoBack(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            GoBack();
            return;
        }

        void GoForward(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            GoForward();
            return;
        }

        void ReRoute(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string remainder = Utility.GetAndRemoveParameter(parameters, "frame");
            if (String.IsNullOrEmpty(remainder))
                remainder = Utility.RemainderParameter(parameters);
            ReRoute(remainder);
            return;
        }

        #region CRM Page
        void RunXRMCommand(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string frameName = Utility.GetAndRemoveParameter(parameters, "frame");
            object objRet = RunXrmCommand(Utility.GetContextReplacedString(Utility.RemainderParameter(parameters), CurrentContext, localSession), frameName);
            if (objRet != null && objRet is string)
                args.ActionReturnValue = (string)objRet;
        }

        void StartDialog(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            StartDialog(args.Data);
        }

        void Find(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            #region find
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            Trace.WriteLine(this.ApplicationName + " find called");

            string showRibbonString = Utility.GetAndRemoveParameter(parameters, "showribbon");
            bool showRibbon = true;
            if (!String.IsNullOrEmpty(showRibbonString))
            {
                bool.TryParse(showRibbonString, out showRibbon);
            }
            string remainderParam = Utility.RemainderParameter(parameters).ToLower();
            string listEntity = String.Empty;
            switch (remainderParam)
            {
                case "case":
                case "incident":
                    listEntity = "incident";
                    if (showRibbon)
                    {
                        Navigate(Utility.GetCrmUiUrl() + "/main.aspx?etn=incident&pagetype=entitylist");
                    }
                    else
                    {
                        Navigate(Utility.GetCrmUiUrl() + "/_root/homepage.aspx?etn=incident&pagemode=iframe");
                    }
                    break;
                case "advfind":
                    listEntity = "advfind";
                    Navigate(Utility.GetCrmUiUrl() + "/main.aspx?pagetype=advancedfind");
                    break;
                case "activities":
                case "activity":
                    listEntity = "activity";
                    if (showRibbon)
                    {
                        Navigate(Utility.GetCrmUiUrl() + "/main.aspx?etc=4200&pagetype=entitylist");
                    }
                    else
                    {
                        Navigate(Utility.GetCrmUiUrl() + "/_root/homepage.aspx?etc=4200&pagemode=iframe&sitemappath=Workplace%7cMyWork%7cnav_activities");
                    }
                    break;
                default:
                    if (args.Data.Contains('/'))
                    {
                        listEntity = remainderParam;
                        Navigate(Utility.GetCrmUiUrl() + remainderParam);
                    }
                    else
                    {
                        listEntity = remainderParam;
                        if (showRibbon)
                        {
                            Navigate(Utility.GetCrmUiUrl() + "/main.aspx?etn=" + remainderParam + "&pagetype=entitylist");
                        }
                        else
                        {
                            Navigate(Utility.GetCrmUiUrl() + "/_root/homepage.aspx?etn=" + remainderParam + "&pagemode=iframe");
                        }
                    }
                    break;
            }
            return;
            #endregion
        }

        void ScanForDataParameters(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {   // This control only does manual scanning so this action is important here
            ScanForDataParameters();
        }

        void LoadDataParameters(Microsoft.Uii.Csr.RequestActionEventArgs args)
        {
            #region LoadDataParameters
            NameValueCollection queryParams = System.Web.HttpUtility.ParseQueryString(Url.Query);
            if (crmUri == false)
                throw new Exception("Only works with CRM pages");
            string extraqs = queryParams["extraqs"];
            string id = "";
            if (extraqs != null && !String.IsNullOrEmpty(extraqs) && System.Web.HttpUtility.ParseQueryString(extraqs) != null)
                id = System.Web.HttpUtility.ParseQueryString(extraqs)["id"];
            if (String.IsNullOrEmpty(id) && queryParams["id"] != null)
                id = queryParams["id"];
            Guid gid = Guid.Empty;
            Guid.TryParse(id, out gid);
            string etc = queryParams["etc"];
            string etn = queryParams["etn"];
            string entityName = String.Empty;
            if (!String.IsNullOrEmpty(etn))
            {
                entityName = etn;
            }
            else if (!String.IsNullOrEmpty(etc))
            {
                try
                {
                    entityName = CRMWindowRouter.EntityNameFromType(int.Parse(etc));
                }
                catch
                {
                }
            }

            // we now have entityName and gid
            string fetchXml = String.Empty;
            if (String.IsNullOrEmpty(args.Data))
                fetchXml = String.Format(DefaultEntityRetrieval, entityName, gid.ToString());
            else
                fetchXml = String.Format(Utility.GetContextReplacedString(args.Data, CurrentContext, localSession), entityName, gid.ToString());
            //fetchXml = CRMWindowRouter.ReplaceParametersInCurrentSession(fetchXml);
            if (Utility.IsAllReplacementValuesReplaced(fetchXml))
            {
                string pageCookie = String.Empty;
                bool isMoreRecords;
                EntityCollection result = _client.CrmInterface.GetEntityDataByFetchSearchEC(fetchXml, 1, 1, pageCookie, out pageCookie, out isMoreRecords);
                if (result == null && _client.CrmInterface.LastCrmException != null)
                {
                    throw _client.CrmInterface.LastCrmException;
                }
                Entity c = result.Entities.FirstOrDefault();
                if (c != null)
                {
                    // update replacement parameters
                    EntityDescription entityDesc = EntityDescription.FromEntity(c);

                    List<EntityDescription> ed = new List<EntityDescription>();
                    if (result.Entities.Count > 0)
                    {
                        foreach (Entity e in result.Entities)
                            ed.Add(EntityDescription.FromEntity(e));
                    }

                    List<LookupRequestItem> lri = new List<LookupRequestItem>();
                    ((DynamicsCustomerRecord)((AgentDesktopSession)localSession).Customer.DesktopCustomer).AddReplaceableParameter(ApplicationName, entityDesc.data, true);
                    ((DynamicsCustomerRecord)((AgentDesktopSession)localSession).Customer.DesktopCustomer).MergeReplacementParameter(ApplicationName, lri, true);
                    args.ActionReturnValue = result.Entities.Count.ToString();

                    if (localSession == localSessionManager.ActiveSession)
                    {
                        ICRMCustomerSearch CRMCustomerSearch = AifServiceContainer.Instance.GetService<ICRMCustomerSearch>();
                        CRMCustomerSearch.CheckUpdateContact();
                    }
                    FireEvent("CRMDataAvailable");
                }
                else
                {
                    args.ActionReturnValue = "0";
                    Trace.WriteLine("DoSearch failed to return results");
                }
            }
            else
            {
                Trace.WriteLine("Do Search failed because the replacement parameters in the FetchXML were not replaced:");
                Trace.WriteLine(fetchXml);
                throw new Exception("Do Search failed because the replacement parameters in the FetchXML were not replaced");
            }
            return;
            #endregion
        }
        #endregion
        #endregion

        #region CRM Page
        /// <summary>
        /// Execute hte XRM Command on the current UI. 
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public object RunXrmCommand(string sCommand, string frame)
        {
            return Dispatcher.Invoke(new System.Func<object>(() =>
            {
                try
                {
                    HTMLDocument xrmDoc = GetDocument(frame);
                    object[] parm = new object[] { sCommand };
                    // Find the CRM form. 
                    HTMLDocument doc = GetDocumentContainingId(xrmDoc, "crmFormTabContainer");
                    if (doc == null)
                    {
                        doc = (HTMLDocument)Document; // try passed document
                    }
                    Type scriptType = doc.scripts.GetType();
                    object ret = scriptType.InvokeMember("eval", BindingFlags.InvokeMethod, null, doc.Script, parm);
                    return ret;
                }
                catch (Exception)
                {
                    return null;
                }
            }));
        }

        string _useEntityData = String.Empty;
        public bool UseEntityData
        {
            get
            {
                if (string.IsNullOrEmpty(this._useEntityData))
                {
                    this._useEntityData = Utility.GetConfigurationValue("useEntityData");
                    if (string.IsNullOrEmpty(this._useEntityData))
                    {
                        this._useEntityData = "true";
                    }
                    this._useEntityData = this._useEntityData.ToLower();
                }
                return (bool)(this._useEntityData == "true");
            }
        }

        public void ScanForDataParameters()
        {
            if (dataCaptured)
            {
                this.InjectUSDProcessingScript();
            }
            this.RunScript(string.Format("if (typeof(ScanForData) == 'undefined') window.open('http://event.usd/?eventname=usdreload'); else ScanForData('{0}',{1});", string.Empty, this.UseEntityData ? "true" : "false"), string.Empty);
        }

        public void InjectUSDProcessingScript()
        {
            string script = GetStringResource("msdyusd_CRMPageDocumentComplete.js");
            //string script = String.Empty;
            //using (System.IO.Stream stream = System.Reflection.Assembly.GetCallingAssembly().GetManifestResourceStream("Microsoft.USD.ComponentLibrary.Adapters.MCSBrowser.CRMScrape.js"))
            //{
            //    if (stream != null)
            //    {
            //        using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
            //        {
            //            script = reader.ReadToEnd();
            //        }
            //    }
            //}

            script = script.Replace("http://event/", "http://event.com/");
            this.RunScript(script, string.Empty);
        }

        private Dictionary<string, string> stringResources = new Dictionary<string, string>();
        private string GetResource(string resource)
        {
            lock (stringResources)
            {
                if (!this.stringResources.ContainsKey(resource))
                {
                    stringResources.Add(resource, GetStringResource(resource));
                }
                return this.stringResources[resource];
            }
        }

        private string GetStringResource(string resource)
        {
            string str = string.Empty;

            str = AifServiceContainer.Instance.GetService<ICRMWindowRouter>().GetStringResource(resource);
            if (!String.IsNullOrEmpty(str))
                return str;

            using (System.IO.Stream stream = System.Reflection.Assembly.GetAssembly(typeof(Microsoft.Crm.UnifiedServiceDesk.Dynamics.DynamicsCustomerRecord)).GetManifestResourceStream("Microsoft.Crm.UnifiedServiceDesk.Dynamics.Resources." + resource))
            {
                if (stream != null)
                {
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                    {
                        str = reader.ReadToEnd();
                    }
                }
            }
            return str;
        }

        public void StartDialog(string parameter)
        {
            string url = Utility.GetCrmUiUrl().ToLower();
            string[] lines = parameter.Split('\n');
            string dialogId = "";
            string entityType = "";
            string entityId = "";
            string dialogname = "";
            foreach (string line in lines)
            {
                string[] dialogParamLine = line.Trim().Split('=');
                if (dialogParamLine.Length == 2)
                {
                    if (dialogParamLine[0].ToLower() == "dialogid")
                    {
                        dialogId = Utility.GetContextReplacedString(dialogParamLine[1], CurrentContext, localSession);
                    }
                    else if (dialogParamLine[0].ToLower() == "name")
                    {
                        dialogname = Utility.GetContextReplacedString(dialogParamLine[1], CurrentContext, localSession);
                        ICrmUtilityHostedControl _client = AifServiceContainer.Instance.GetService<ICrmUtilityHostedControl>();

                        // Adapted to Client.
                        Dictionary<string, string> searchStrings = new Dictionary<string, string>();
                        searchStrings.Add("name", dialogname);
                        searchStrings.Add("statuscode", "2");   // Activated
                        searchStrings.Add("category", "1");     // Dialog
                        var results = _client.CrmInterface.GetEntityDataBySearchParams("workflow", searchStrings, Microsoft.Xrm.Tooling.Connector.CrmServiceClient.LogicalSearchOperator.None, null);
                        if (results != null && results.Count > 0)
                        {
                            var workflowToWorkWith = results.First();
                            if (workflowToWorkWith.Value.ContainsKey("parentworkflowid"))
                            {
                                EntityReference er = _client.CrmInterface.GetDataByKeyFromResultsSet<EntityReference>(workflowToWorkWith.Value, "parentworkflowid");
                                if (er != null)
                                    dialogId = er.Id.ToString();
                            }
                            else
                                dialogId = _client.CrmInterface.GetDataByKeyFromResultsSet<Guid>(workflowToWorkWith.Value, "workflowid").ToString();

                            KeyValuePair<string, object> obj = (KeyValuePair<string, object>)_client.CrmInterface.GetDataByKeyFromResultsSet<Object>(workflowToWorkWith.Value, "primaryentity_Property");
                            entityType = obj.Value as string;
                        }
                        else
                        {
                            throw new Exception("Dialog not found: " + dialogname);
                        }
                    }
                    else if (dialogParamLine[0].ToLower() == "entity")
                    {
                        entityType = Utility.GetContextReplacedString(dialogParamLine[1], CurrentContext, localSession);
                    }
                    else if (dialogParamLine[0].ToLower() == "id")
                    {
                        entityId = Utility.GetContextReplacedString(dialogParamLine[1], CurrentContext, localSession);
                    }
                }
            }
            if (String.IsNullOrEmpty(entityId) && !String.IsNullOrEmpty(entityType))
            {
                ICrmUtilityHostedControl _client = AifServiceContainer.Instance.GetService<ICrmUtilityHostedControl>();

                string pgCookie = string.Empty;
                bool isMoreRecs = false;
                var results = _client.CrmInterface.GetEntityDataBySearchParams(entityType, new List<Microsoft.Xrm.Tooling.Connector.CrmServiceClient.CrmSearchFilter>(), Microsoft.Xrm.Tooling.Connector.CrmServiceClient.LogicalSearchOperator.None,
                    new List<string>(), null, 1, 1, pgCookie, out pgCookie, out isMoreRecs);
                Microsoft.Xrm.Sdk.Metadata.EntityMetadata ed = CRMWindowRouter.GetMetadata(entityType);
                if (results != null && results.Count > 0)
                {
                    var firstResult = results.First();
                    var firstResultValue = firstResult.Value;
                    entityId = "{" + _client.CrmInterface.GetDataByKeyFromResultsSet<Guid>(firstResult.Value, ed.PrimaryIdAttribute).ToString() + "}";
                }
            }
            if (String.IsNullOrEmpty(dialogId) && !String.IsNullOrEmpty(dialogname))
            {

            }
            if (String.IsNullOrEmpty(dialogId) || String.IsNullOrEmpty(entityType) || String.IsNullOrEmpty(entityId))
                return;
            url += "/cs/dialog/rundialog.aspx?DialogId=" + System.Web.HttpUtility.UrlEncode(dialogId);
            url += "&EntityName=" + System.Web.HttpUtility.UrlEncode(entityType);
            url += "&ObjectId=" + System.Web.HttpUtility.UrlEncode(entityId);
            Navigate(url);
        }

        static string BlockPopupErrorText = String.Empty;
        public new void BlockPopupErrors(IWebBrowser2 htmlBrowser)
        {
            try
            {
                lock (BlockPopupErrorText)
                {
                    if (String.IsNullOrEmpty(BlockPopupErrorText))
                    {
                        // The following line allows the admin to replace the code that is injected here for cancelling popups.
                        // This was added in anticipation of changing being made to CRM that will affect this code.
                        string crmAutoPageScript = CRMWindowRouter.ReplaceParametersInCurrentSession("[[$Global.CRMAutoPageScript]+]");
                        if (String.IsNullOrEmpty(crmAutoPageScript))
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("if (typeof(deferCancelPopupBlockMessage) == 'undefined')");
                            sb.AppendLine("{");
                            sb.AppendLine("  var cancelCount = 0;");
                            sb.AppendLine("  deferCancelPopupBlockMessage = function()");
                            sb.AppendLine("  {");
                            sb.AppendLine("    if (typeof(handlePopupBlockerError) == 'undefined' && cancelCount < 50)");
                            sb.AppendLine("    {");
                            sb.AppendLine("      cancelCount++;");
                            sb.AppendLine("      setTimeout('deferCancelPopupBlockMessage()',200);");
                            sb.AppendLine("      return;");
                            sb.AppendLine("    }");
                            sb.AppendLine("    handlePopupBlockerError=function(url) {};");
                            // The following line was added to deal with a new caching mechanism in CRM online that causes
                            // the popup window to always be /_static/loading.htm.   This breaks the routing rules engine
                            // in CCD.   The following line replaces the new function and always uses the standard window
                            // opening procedure, which is needed for the popup URL to be consistant with the way it's 
                            // been in the past.
                            //     sb.AppendLine("    openStdWinWithUrlPreload=function(url,name,width,height,customWinFeatures){return openStdWin(url,name,width,height,customWinFeatures);}");
                            // -----------
                            sb.AppendLine("    if (cancelCount < 50)");
                            sb.AppendLine("         setTimeout('deferCancelPopupBlockMessage()',200);");
                            sb.AppendLine("    cancelCount++;");
                            sb.AppendLine("  }");
                            sb.AppendLine("  deferCancelPopupBlockMessage();");
                            sb.AppendLine("}");
                            BlockPopupErrorText = sb.ToString();
                        }
                        else
                        {
                            BlockPopupErrorText = crmAutoPageScript;
                        }
                    }
                }

                Dispatcher.Invoke(new System.Action(() =>
                {
                    try
                    {
                        ((mshtml.HTMLDocument)htmlBrowser.Document).parentWindow.execScript(BlockPopupErrorText, "javascript");
                    }
                    catch
                    {
                    }
                }));
            }
            catch
            {
            }
        }

        protected bool CRMPopupRequested(string url, string frame)
        {
            NameValueCollection queryParams = HttpUtility.ParseQueryString(url.Substring(17));
            if ((queryParams["eventname"] != null) && queryParams["eventname"].Equals("usddataload"))
            {
                this.ProcessUSDData(false, false);
                FireEvent("CRMDataAvailable");
            }
            else if ((queryParams["eventname"] != null) && queryParams["eventname"].Equals("usdrawdataload"))
            {
                this.ProcessUSDData(true, false);
                FireEvent("CRMDataAvailable");
            }
            else if ((queryParams["eventname"] != null) && queryParams["eventname"].Equals("usdreload"))
            {
                InjectUSDProcessingScript();
                RunScript(string.Format("if (typeof(ScanForData) != 'undefined') ScanForData('{0}',{1});", frame, "true"), string.Empty);
            }
            else if (queryParams["eventname"] != null && queryParams["eventname"].Equals("handleNavigateRequestCallback"))
            {
                Dictionary<string, string> parms = ExtractParameters(queryParams);
                if (parms.ContainsKey("uri"))
                {
                    string uri = parms["uri"];
                    uri = Utility.EnsureQualifiedRootUrl(uri);
                    ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                    CRMGlobalManager.WindowRoute ruleResult = CRMWindowRouter.DoRouteDetermineRule(localSession, ApplicationName, null, null, uri, String.Empty, CRMGlobalManager.RouteType.InPlace, null);
                    if (ruleResult != null)
                    {
                        switch (ruleResult.action)
                        {
                            case CRMGlobalManager.WindowRouteAction.None:
                                break;
                            case CRMGlobalManager.WindowRouteAction.Default:
                            case CRMGlobalManager.WindowRouteAction.InPlace:
                                RunScript("SAVE_INSTANCE.oldHandleNavigateRequestCallback(SAVE_PARAMETERS, SAVE_SOURCECOMPONENT);", "");
                                break;
                            case CRMGlobalManager.WindowRouteAction.CreateSession:
                            case CRMGlobalManager.WindowRouteAction.RouteWindow:
                            case CRMGlobalManager.WindowRouteAction.ShowOutside:
                                CRMWindowRouter.DoRoute(localSession, ApplicationName, null, null, uri, String.Empty, CRMGlobalManager.RouteType.InPlace, null);
                                return true;
                        }
                        CRMWindowRouter.ExecuteActions(localSession, ruleResult.actionDefinitions, String.Empty, null);
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleResult, new Dictionary<string, string>() { { "SUBJECTURL", uri } });
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                    }
                    else
                    {
                        CRMGlobalManager.WindowRoute ruleDefault = new CRMGlobalManager.WindowRoute();
                        ruleDefault.fromApp = ApplicationName;
                        ruleDefault.type = CRMGlobalManager.RouteType.InPlace;
                        ruleDefault.action = CRMGlobalManager.WindowRouteAction.Default;
                        ruleDefault.name = "DEFAULT";
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleDefault, new Dictionary<string, string>() { { "SUBJECTURL", uri } });
                        RunScript("SAVE_INSTANCE.oldHandleNavigateRequestCallback(SAVE_PARAMETERS, SAVE_SOURCECOMPONENT);", "");
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                    }
                }
                else
                {   // this shouldn't happen but just in case, use normal 2013 behavior
                    CRMGlobalManager.WindowRoute ruleDefault = new CRMGlobalManager.WindowRoute();
                    ruleDefault.fromApp = ApplicationName;
                    ruleDefault.type = CRMGlobalManager.RouteType.InPlace;
                    ruleDefault.action = CRMGlobalManager.WindowRouteAction.Default;
                    ruleDefault.name = "DEFAULT";
                    ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                    CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleDefault);
                    RunScript("SAVE_INSTANCE.oldHandleNavigateRequestCallback(SAVE_PARAMETERS, SAVE_SOURCECOMPONENT);", "");
                    FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", String.Empty } });
                }
            }
            else if (queryParams["eventname"] != null && queryParams["eventname"].Equals("raiseNavigateRequest"))
            {
                Dictionary<string, string> parms = ExtractParameters(queryParams);
                if (parms.ContainsKey("uri"))
                {
                    string uri = parms["uri"];
                    uri = Utility.EnsureQualifiedRootUrl(uri);
                    ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                    CRMGlobalManager.WindowRoute ruleResult = CRMWindowRouter.DoRouteDetermineRule(localSession, ApplicationName, null, null, uri, String.Empty, CRMGlobalManager.RouteType.InPlace, null);
                    if (ruleResult != null)
                    {
                        switch (ruleResult.action)
                        {
                            case CRMGlobalManager.WindowRouteAction.None:
                                break;
                            case CRMGlobalManager.WindowRouteAction.Default:
                            case CRMGlobalManager.WindowRouteAction.InPlace:
                                RunScript("SAVE_INSTANCE.oldRaiseNavigateRequest(SAVE_PARAMETERS);", "");
                                break;
                            case CRMGlobalManager.WindowRouteAction.CreateSession:
                            case CRMGlobalManager.WindowRouteAction.RouteWindow:
                            case CRMGlobalManager.WindowRouteAction.ShowOutside:
                                CRMWindowRouter.DoRoutePopup(localSession, ApplicationName, uri, String.Empty, false, true);
                                return true;
                        }
                        CRMWindowRouter.ExecuteActions(localSession, ruleResult.actionDefinitions, String.Empty, null);
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleResult, new Dictionary<string, string>() { { "SUBJECTURL", uri } });
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                    }
                    else
                    {
                        CRMGlobalManager.WindowRoute ruleDefault = new CRMGlobalManager.WindowRoute();
                        ruleDefault.fromApp = ApplicationName;
                        ruleDefault.type = CRMGlobalManager.RouteType.MenuChosen;
                        ruleDefault.action = CRMGlobalManager.WindowRouteAction.Default;
                        ruleDefault.name = "DEFAULT";
                        CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleDefault, new Dictionary<string, string>() { { "SUBJECTURL", uri } });
                        RunScript("SAVE_INSTANCE.oldRaiseNavigateRequest(SAVE_PARAMETERS);", "");
                        FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", url } });
                    }
                }
                else
                {   // this shouldn't happen but just in case, use normal 2013 behavior
                    CRMGlobalManager.WindowRoute ruleDefault = new CRMGlobalManager.WindowRoute();
                    ruleDefault.fromApp = ApplicationName;
                    ruleDefault.type = CRMGlobalManager.RouteType.MenuChosen;
                    ruleDefault.action = CRMGlobalManager.WindowRouteAction.Default;
                    ruleDefault.name = "DEFAULT";
                    ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                    CRMWindowRouter.TraceWindowRoute("RoutingRule(" + ApplicationName + ")", ruleDefault);
                    RunScript("SAVE_INSTANCE.oldRaiseNavigateRequest(SAVE_PARAMETERS);", "");
                    FireEvent("PopupRouted", new Dictionary<string, string>() { { "url", String.Empty } });
                }
            }
            else if (queryParams["eventname"] != null)
            {
                Dictionary<string, string> parms = ExtractParameters(queryParams);
                FireEvent(queryParams["eventname"], parms);
            }
            return false;
        }
        private void ProcessUSDData(bool useEntityData, bool noUpdate)
        {
            try
            {
                if (((localSession != null) && (((AgentDesktopSession)localSession).Customer != null)) && (((AgentDesktopSession)localSession).Customer.DesktopCustomer != null))
                {
                    DynamicsCustomerRecord record = (DynamicsCustomerRecord)((AgentDesktopSession)localSession).Customer.DesktopCustomer;
                    if (record != null)
                    {
                        string str = string.Empty;
                        try
                        {
                            str = (string)RunScript("GetUSDFormData();", "");
                        }
                        catch
                        {
                            return;
                        }
                        ICRMWindowRouter service = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                        if (!string.IsNullOrEmpty(str))
                        {
                            string name = string.Empty;
                            string[] strArray = str.Split((char[])new char[] { '\n' });
                            this.data = new System.Collections.Generic.Dictionary<string, CRMApplicationData>();
                            System.Collections.Generic.List<CRMApplicationData> list = new System.Collections.Generic.List<CRMApplicationData>();
                            string[] strArray5 = strArray;
                            for (int i = 0; i < strArray5.Length; i = (int)(i + 1))
                            {
                                string[] strArray2 = strArray5[i].Split((char[])new char[] { '|' });
                                if (strArray2.Length == 3)
                                {
                                    string val = HttpUtility.UrlDecode(strArray2[0]);
                                    string str5 = HttpUtility.UrlDecode(strArray2[1]);
                                    string str6 = HttpUtility.UrlDecode(strArray2[2]);
                                    if (str6 == "null")
                                    {
                                        if (val == "Id")
                                        {
                                            name = str5;
                                        }
                                    }
                                    else
                                    {
                                        if (str5 == "lookup")
                                        {
                                            if (useEntityData)
                                            {
                                                string[] strArray3 = str6.Split((char[])new char[] { ',' });
                                                int num = -1;
                                                try
                                                {
                                                    if (int.TryParse(strArray3[0], out num))
                                                    {
                                                        strArray3[0] = this._client.CrmInterface.GetEntityName(num);
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                                str6 = string.Join(",", strArray3);
                                            }
                                            CRMApplicationData data = new CRMApplicationData
                                            {
                                                name = val,
                                                type = str5,
                                                value = str6
                                            };
                                            this.AddIfMissing(this.data, val, data);
                                            list.Add(data);
                                        }
                                        else if (val == "Id")
                                        {
                                            CRMApplicationData data3 = new CRMApplicationData
                                            {
                                                name = "Id",
                                                type = "string",
                                                value = str6.Trim((char[])new char[] { '{', '}' })
                                            };
                                            this.AddIfMissing(this.data, "Id", data3);
                                        }
                                        else if (val == "LogicalName")
                                        {
                                            CRMApplicationData data5 = new CRMApplicationData
                                            {
                                                name = "LogicalName",
                                                type = "string",
                                                value = str6
                                            };
                                            name = str6;
                                            this.AddIfMissing(this.data, "LogicalName", data5);
                                            int num2 = service.EntityTypeFromName(name);
                                            if (num2 != 0)
                                            {
                                                data5 = new CRMApplicationData
                                                {
                                                    name = "etc",
                                                    type = "string",
                                                    value = ((int)num2).ToString()
                                                };
                                                this.AddIfMissing(this.data, "etc", data5);
                                            }
                                        }
                                        else
                                        {
                                            bool flag1 = val == "name";
                                            CRMApplicationData data8 = new CRMApplicationData
                                            {
                                                name = val,
                                                type = str5,
                                                value = str6
                                            };
                                            this.AddIfMissing(this.data, val, data8);
                                        }
                                    }
                                }
                            }
                            CRMApplicationData data11 = new CRMApplicationData
                            {
                                name = "url",
                                type = "string",
                                value = Url.OriginalString
                            };
                            this.AddIfMissing(this.data, "url", data11);
                            CRMApplicationData data12 = new CRMApplicationData
                            {
                                name = "location",
                                type = "string",
                                value = BrowserId
                            };
                            this.AddIfMissing(this.data, "location", data12);
                            CRMApplicationData data13 = new CRMApplicationData
                            {
                                name = "HostedControlStatus",
                                type = "string",
                                value = "open"
                            };
                            this.AddIfMissing(this.data, "HostedControlStatus", data13);
                            string applicationName = base.ApplicationName;
                            if (record != null)
                            {
                                try
                                {
                                    record.AddReplaceableParameter(applicationName, name, this.data, true);
                                }
                                catch
                                {
                                }
                                try
                                {
                                    record.AddReplaceableParameter(applicationName, this.data, true);
                                }
                                catch
                                {
                                }
                                try
                                {
                                    if (!noUpdate)
                                    {
                                        AifServiceContainer.Instance.GetService<ICRMCustomerSearch>().CheckUpdateContact();
                                    }
                                }
                                catch
                                {
                                }
                            }
                            if (!noUpdate)
                            {
                                foreach (CRMApplicationData data10 in list)
                                {
                                    try
                                    {
                                        string[] strArray4 = data10.value.Split((char[])new char[] { ',' });
                                        if (strArray4.Length >= 3)
                                        {
                                            string entity = HttpUtility.UrlDecode(strArray4[0]);
                                            string str9 = string.Empty;
                                            for (int j = 2; j < strArray4.Length; j = (int)(j + 1))
                                            {
                                                if (!string.IsNullOrEmpty(str9))
                                                {
                                                    str9 = str9 + ",";
                                                }
                                                str9 = strArray4[j];
                                            }
                                            HttpUtility.UrlDecode(str9).TrimEnd((char[])new char[] { '&' });
                                            string id = HttpUtility.UrlDecode(strArray4[1]);
                                            service.DoRouteOnLoad(localSession, ApplicationName, "", data10.name, entity, id, true, false, CRMGlobalManager.RouteType.OnLoad, null);
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }
                                try
                                {
                                    System.Guid guid = service.IdFromUrl(Url.ToString());
                                    if (guid != System.Guid.Empty)
                                    {
                                        service.DoRouteOnLoad(localSession, base.ApplicationName, "", guid, null);
                                    }
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }

        private void AddIfMissing(System.Collections.Generic.Dictionary<string, CRMApplicationData> dataList, string val, CRMApplicationData data)
        {
            if (!dataList.ContainsKey(val))
            {
                dataList.Add(val, data);
            }
        }

        public bool HasCRMForm(HTMLDocument doc)
        {
            return doc.getElementById("crmFormTabContainer") != null;
        }

        public static string DefaultEntityRetrieval =
        @"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
            <entity name=""{0}"">
            <all-attributes/>
            <filter type=""and"">
                <condition attribute=""{0}id"" operator=""eq"" value=""{1}"" />
            </filter>
            </entity>
        </fetch>";

        protected Dictionary<string, string> ExtractParameters(NameValueCollection queryParams)
        {
            Dictionary<string, string> parms = new Dictionary<string, string>();
            foreach (string s in queryParams.Keys)
            {
                if (s == "eventname")
                    continue;
                parms.Add(s, queryParams[s]);
            }
            return parms;
        }

        private bool Refresh()
        {
            bool handled = FireEvent("RefreshRequested", new Dictionary<string, string>() { { "url", Url.OriginalString } });
            if (handled == false)
            {
                return false;
            }
            return true;
        }

        public void ReRoute(string frame)
        {
            if (Url == null)
                return;
            CRMWindowRouter.DoRoutePopup(localSession, ApplicationName, (string)Url.OriginalString, frame, true, true);  // lets not start sessions from here
        }

        #endregion

    }
}
