/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Csr.Browser.Web;
using Microsoft.Xrm.Sdk;
using mshtml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
    public class FloatingBrowser : MicrosoftBase, ICRMWindowContainer, IBrowserCallback, IDisposable
    {
        #region Fields
        public IWebBrowser2 web = null;
        public string BrowserId = String.Empty;
        public object _webBrowser;
        public Dictionary<string, HTMLDocument> frames = new Dictionary<string, HTMLDocument>();
        protected MCSBrowserEventSink _WebBrowserEventSink;
        string initialUrl = String.Empty;
        private static SynchronizationContext _SynchronizationContext;
        EventHandler processExitedHandler;
        private Object BrowserDestruct = new Object();
        private Process _Process;
        private volatile Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState _WebBrowserExtendedState;
        #endregion

        #region Construction 
        [WebBrowserPermissionAttribute(SecurityAction.Demand, Level = WebBrowserPermissionLevel.Unrestricted)]
        public FloatingBrowser()
        {
        }

        public FloatingBrowser(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            try
            {
                _SynchronizationContext = SynchronizationContext.Current;
                CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                thisApplication = CRMWindowRouter.LoadApplicationEntity(this.ApplicationName);
                processExitedHandler = new EventHandler(this._Process_Exited);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        #endregion

        protected override void DesktopReady()
        {
            base.DesktopReady();

            userCanClose = (bool)this.thisApplication["uii_usercanclose"];

            RegisterAction("Navigate", Navigate);
            RegisterAction("Activate", Activate);
            RegisterAction("SetProperties", SetProperties);
        }

        bool AddressBar = true;
        bool MenuBar = true;
        bool StatusBar = true;
        int ToolBar = 1;
        bool userCanClose;

        private void SetProperties(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> proplines = Utility.SplitLines(args.Data, CurrentContext, localSession);
            bool.TryParse(Utility.GetAndRemoveParameter(proplines, "AddressBar"), out AddressBar);
            bool.TryParse(Utility.GetAndRemoveParameter(proplines, "MenuBar"), out MenuBar);
            bool.TryParse(Utility.GetAndRemoveParameter(proplines, "StatusBar"), out StatusBar);
            int.TryParse(Utility.GetAndRemoveParameter(proplines, "ToolBar"), out ToolBar);

            if (this.web != null)
            {
                this.web.AddressBar = AddressBar;
                this.web.MenuBar = MenuBar;
                this.web.StatusBar = StatusBar;
                this.web.ToolBar = ToolBar;
                if (userCanClose == false)
                    RemoveBrowserClose((IntPtr)web.HWND);
            }
        }

        public override void Close()
        {
            _CloseBrowser();
            base.Close();
        }

        protected override void SessionShowEvent(Session session)
        {
            try
            {
                base.SessionShowEvent(session);

                if (thisSessionId == session.SessionId)
                {
                    Win32API.ShowWindow((IntPtr)web.HWND, Win32API.ShowWindowValues.SW_SHOW);
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override void SessionHideEvent(Session session)
        {
            try
            {
                base.SessionHideEvent(session);

                if (thisSessionId == session.SessionId
                    && session.Global == false)
                {
                    Win32API.ShowWindow((IntPtr)web.HWND, Win32API.ShowWindowValues.SW_HIDE);
                }
            }
            catch (Exception ex)
            {

            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        void Activate(Uii.Csr.RequestActionEventArgs args)
        {
            if (web == null)
            {
                args.ActionReturnValue = "not loaded";
                return;
            }
            SetForegroundWindow((IntPtr)web.HWND);
            args.ActionReturnValue = "activated";
        }

        void Navigate(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string url = Utility.GetAndRemoveParameter(parameters, "url");
            string remainder = Utility.RemainderParameter(parameters);
            if (String.IsNullOrEmpty(url))
                url = remainder;
            Navigate(url);
            return;
        }

        public void _CloseBrowser()
        {
            lock (BrowserDestruct)
            {
                if (this._WebBrowserExtendedState != WebBrowserExtendedState.BrowserAcquired)
                    return;
                if (this.web != null)
                {
                    UnsinkEvents();

                    if (this._Process != null)
                    {
                        this._Process.Exited -= processExitedHandler;
                    }

                    try
                    {
                        this.web.Quit();
                        if (this.web is IDisposable)
                        {
                            ((IDisposable)web).Dispose();
                        }
                    }
                    catch (COMException exception)
                    {
                        Trace.WriteLine(string.Format("COM Exception happened while closing IE {0} ", exception.Message));
                    }
                    finally
                    {
                        this.web = null;
                    }
                }
                this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserNotAcquired;
            }
        }

        private void _Process_Exited(object sender, EventArgs e)
        {
            _SynchronizationContext.Post(new SendOrPostCallback(this._Process_Exited2), null);
            Trace.WriteLine("Process_Exited handler: browser process exited");
        }

        private void _Process_Exited2(object state)
        {
            this._CloseBrowser();
        }

        public IWebBrowser2 GetBrowser()
        {
            if (this._WebBrowserExtendedState == Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserNotAcquired
                || web == null)
            {
                StartBrowser();
            }
            return web;
        }

        private void StartBrowser()
        {
            this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserAcquiring;
            this.web = null;
            this.web = (IWebBrowser2)Activator.CreateInstance(Type.GetTypeFromCLSID(NativeMethods.IID_InternetExplorer));
            object flags = 0;
            object missing = Type.Missing;
            this.web.Navigate("about:blank", ref flags, ref missing, ref missing, ref missing);
            if (web.HWND == 0)
                throw new Exception("Unable to attach to browser.   Protected Mode may be disable");
            this._Process = Process.GetProcessById(NativeMethods.GetProcessIdFromWindow((IntPtr)this.web.HWND));
            if (_Process == null)
                throw new Exception("Unable to attach to browser.   Protected Mode may be disable");
            Win32API.ShowWindow((IntPtr)web.HWND, Win32API.ShowWindowValues.SW_MAXIMIZE);
            this._Process.Exited += _Process_Exited;
            this._Process.EnableRaisingEvents = true;
            this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserAcquired;
            this.web.AddressBar = AddressBar;
            this.web.MenuBar = MenuBar;
            this.web.StatusBar = StatusBar;
            this.web.ToolBar = ToolBar; 
            if (userCanClose == false)
                RemoveBrowserClose((IntPtr)web.HWND);
        }

        void RemoveBrowserClose(IntPtr winHandle)
        {
            uint dwNewLong = NativeMethods.GetWindowLong(winHandle, NativeMethods.WinUserConstant.GWL_STYLE);
            dwNewLong ^= 0x00080000;    // WS_SYSMENU
            NativeMethods.SetWindowLong(winHandle, NativeMethods.WinUserConstant.GWL_STYLE, dwNewLong);
        }

        #region Events
        // The following setup is done instead of passing through the event calls so that we can handle the events in here too.

        EventHandler<BeforeNavigate2EventArgs> beforeNavigateHandler;
        EventHandler<BrowserObjectAppearedEventArgs> browserObjectAppearedHandler;
        EventHandler<DocumentCompleteEventArgs> documentCompleteHandler;
        EventHandler<NavigateComplete2EventArgs> navigateComplete2Handler;
        EventHandler<NewWindow3EventArgs> newWindow3Handler;
        EventHandler<TitleChangeEventArgs> titleChangeHandler;
        EventHandler<StatusTextChangeEventArgs> statusTextChangeHandler;
        EventHandler<SetSecureLockIconEventArgs> setSecureLockIconHandler;
        EventHandler<EventArgs> onQuitHandler;

        void SinkEvents()
        {
            try
            { 
                beforeNavigateHandler = new EventHandler<BeforeNavigate2EventArgs>(Browser_BeforeNavigate2);
                browserObjectAppearedHandler = new EventHandler<BrowserObjectAppearedEventArgs>(Browser_BrowserObjectAppeared);
                documentCompleteHandler = new EventHandler<DocumentCompleteEventArgs>(Browser_DocumentComplete);
                navigateComplete2Handler = new EventHandler<NavigateComplete2EventArgs>(Browser_NavigateComplete2);
                newWindow3Handler = new EventHandler<NewWindow3EventArgs>(Browser_NewWindow3);
                titleChangeHandler = new EventHandler<TitleChangeEventArgs>(Browser_TitleChange);
                statusTextChangeHandler = new EventHandler<StatusTextChangeEventArgs>(Browser_StatusTextChange);
                setSecureLockIconHandler = new EventHandler<SetSecureLockIconEventArgs>(Browser_SetSecureLockIcon);
                onQuitHandler = new EventHandler<EventArgs>(Browser_OnQuit);

                this._WebBrowserEventSink = new MCSBrowserEventSink(this);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.BeforeNavigate2, beforeNavigateHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.BrowserObjectAppeared, browserObjectAppearedHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.DocumentComplete, documentCompleteHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.NavigateComplete2, navigateComplete2Handler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.NewWindow3, newWindow3Handler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.TitleChange, titleChangeHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.StatusTextChange, statusTextChangeHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.SetSecureLockIcon, setSecureLockIconHandler);
                _WebBrowserEventSink.AddHandler(WebBrowserEvent.OnQuit, onQuitHandler);
                _WebBrowserEventSink.Advise(web);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UnsinkEvents: " + ex.Message + "\r\n" + ex.StackTrace);
            }

        }

        void UnsinkEvents()
        {
            try
            {
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.BeforeNavigate2, beforeNavigateHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.BrowserObjectAppeared, browserObjectAppearedHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.DocumentComplete, documentCompleteHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.NavigateComplete2, navigateComplete2Handler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.NewWindow3, newWindow3Handler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.TitleChange, titleChangeHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.StatusTextChange, statusTextChangeHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.SetSecureLockIcon, setSecureLockIconHandler);
                _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.OnQuit, onQuitHandler);
                this._WebBrowserEventSink.Unadvise();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("UnsinkEvents: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        private void Browser_OnQuit(object sender, EventArgs e)
        {
            Close();
            desktopAccess.CloseDynamicApplication(this.ApplicationName);
        }

        protected virtual void Browser_BeforeNavigate2(object sender, BeforeNavigate2EventArgs e)
        {
        }
        protected virtual void Browser_BrowserObjectAppeared(object sender, BrowserObjectAppearedEventArgs e)
        {

        }
        protected virtual void Browser_DocumentComplete(object sender, DocumentCompleteEventArgs e)
        {

        }
        protected virtual void Browser_NavigateComplete2(object sender, NavigateComplete2EventArgs e)
        {

        }
        protected virtual void Browser_NewWindow3(object sender, NewWindow3EventArgs e)
        {

        }
        protected virtual void Browser_TitleChange(object sender, TitleChangeEventArgs e)
        {

        }
        protected virtual void Browser_StatusTextChange(object sender, StatusTextChangeEventArgs e)
        {

        }
        protected virtual void Browser_SetSecureLockIcon(object sender, SetSecureLockIconEventArgs e)
        {

        }
        #endregion

        #region Destruction
        bool _isDisposed = false;
        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            try { UnsinkEvents(); }
            catch
            {
            }
            try { _WebBrowserEventSink.Dispose(); }
            catch (Exception ex)
            {
                Trace.WriteLine("Sink Dispose Error: " + ex.Message + "\r\n" + ex.StackTrace);
            }
            try
            {
                if (this.webBrowser is IKeyboardInputSink)
                {
                    // The following code is required to make the browser control fully release
                    var keyboardInputSite = ((IKeyboardInputSink)webBrowser).KeyboardInputSite;
                    if (keyboardInputSite != null)
                    {
                        keyboardInputSite.Unregister();
                    }
                }
                if (this.Content != null && this.Content is IDisposable)
                {
                    ((IDisposable)this.Content).Dispose();
                    Trace.WriteLine("Browsers (Browser Disposed)");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser Dispose: " + ex.Message + "\r\n" + ex.StackTrace);
            }
            try { this.Content = null; }
            catch { }
            _webBrowser = null;
            web = null;
        }
        #endregion

        #region HTML Utility
        /// <summary>
        /// Finds a Document containing the id requested. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected HTMLDocument GetDocumentContainingId(HTMLDocument doc, string id)
        {
            // Used to check to see if the first field in the search list has been found, 
            // and if so, what frame it was found in. 
            bool bFoundRightFrame = false;

            if (doc == null)
                return null;

            // try to find the first Field,
            // if the first field is not found. check for sub frames on the page 
            // if sub frames found check them for the field,
            // if its found on a sub frame, set the foucus to that frame. 
            if (doc.getElementById(id) == null)
            {
                if (doc.frames.length > 0)
                {
                    doc = CheckForValue(doc, id, ref bFoundRightFrame);
                }
            }
            else
            {
                // Field found. 
                // current document remains in foucs. 
                bFoundRightFrame = true;
            }

            if (!bFoundRightFrame)
            {
                // didn find the first tag name.. treat as failure and abort sign on process.
                System.Diagnostics.Debug.WriteLine(string.Format("Could not find the requested id in {0}", doc.url));
                return null;
            }

            return doc;
        }

        /// <summary>
        /// Checks for a value in a set of frames
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <param name="bFoundRightFrame"></param>
        /// <returns></returns>
        protected static HTMLDocument CheckForValue(HTMLDocument doc, string id, ref bool bFoundRightFrame)
        {
            for (int i = 0; i < doc.frames.length; i++)
            {
                if (bFoundRightFrame)
                    break;

                object iFrameNum = i;
                IHTMLWindow2 doc2 = (IHTMLWindow2)doc.frames.item(ref iFrameNum);
                // Check for the field I want in the subframe. 
                HTMLDocument doc3 = (HTMLDocument)doc2.document;
                if (doc3.getElementById(id) != null)
                {
                    // subDoc Found 
                    doc = (HTMLDocument)doc2.document;
                    bFoundRightFrame = true;
                    return doc3;
                }

                if (doc3.frames.length > 0)
                    return CheckForValue(doc3, id, ref bFoundRightFrame);
            }

            return doc;
        }

        protected void LoadFrameList()
        {
            if (web.Document != null)
            {
                lock (frames)
                {
                    frames.Clear();
                    for (int i = 0; i < ((HTMLDocument)web.Document).frames.length; i++)
                    {
                        object itemRef = i;
                        mshtml.HTMLWindow2 frmElement = (HTMLWindow2)((HTMLDocument)web.Document).frames.item(ref itemRef);
                        frames.Add(((HTMLIFrame)frmElement.frameElement).id, (HTMLDocument)frmElement.document);
                    }
                }
            }
        }

        protected HTMLDocument GetDocument(string frame)
        {
            LoadFrameList();
            if (web.Document == null)
                return null;
            lock (frames)
                return frames.FirstOrDefault(a => a.Key.Equals(frame, StringComparison.InvariantCultureIgnoreCase)).Value;
        }
        #endregion

        #region Common Methods
        /// <summary>
        /// Execute hte XRM Command on the current UI. 
        /// </summary>
        /// <param name="sCommand"></param>
        /// <returns></returns>
        public object RunScript(string sCommand, string frame)
        {
            try
            {
                // Find the CRM form. 
                HTMLDocument doc = GetDocument(frame);
                object[] parm = new object[] { sCommand };
                Type scriptType = doc.scripts.GetType(); 
                object obj = scriptType.InvokeMember("eval", BindingFlags.InvokeMethod, null, doc.Script, parm);
                return obj;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public Uri Url
        {
            get
            {
                if (web == null)
                    return null;

                string locationURL = this.web.LocationURL;
                if ((locationURL != null) && !locationURL.Length.Equals(0))
                {
                    try
                    {
                        return new Uri(locationURL);
                    }
                    catch (UriFormatException)
                    {
                    }
                }
                return null;
            }
            set
            {
                string urlString = PrepareUrl(value);
                if (urlString != null)
                {
                    this.PerformNavigate(urlString, false, null, null, null);
                }
            }
        }

        public Object webBrowser
        {
            get
            {
                return this._webBrowser;
            }
        }
        #endregion

        #region Misc

        System.Threading.Timer browserAquiredTimer;
        DateTime browserAquiredStart;
        private void PerformNavigate(string urlString, bool newWindow, string targetFrameName, byte[] postData, string headers)
        {
            if (String.IsNullOrEmpty(initialUrl))
                initialUrl = urlString;
            BrowserId = urlString;

            browserAquiredStart = DateTime.Now;
            browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), urlString, TimeSpan.FromMilliseconds(300), TimeSpan.Zero);
        }

        void BrowserAquiredCheck(object obj)
        {
            string urlString = obj as string;
            if (urlString != BrowserId)
            {
                if (browserAquiredTimer != null)
                    browserAquiredTimer.Dispose();
                browserAquiredTimer = null;
                return;
            }

            if (web == null)
            {
                if (this._WebBrowserExtendedState == Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserNotAcquired)
                {
                    StartBrowser();
                    SinkEvents();
                    browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), urlString, TimeSpan.FromMilliseconds(2000), TimeSpan.Zero);
                }
                else if (DateTime.Now - browserAquiredStart > TimeSpan.FromSeconds(45))
                {
                    if (browserAquiredTimer != null)
                        browserAquiredTimer.Dispose();
                    browserAquiredTimer = null;
                    return;
                }
                else
                {   // try again in a few moments
                    browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), urlString, TimeSpan.FromMilliseconds(500), TimeSpan.Zero);
                }
                return;
            }
            else
            {
                PreNavigate(urlString);
                try
                {
                    web.Navigate(urlString);
                }
                catch (InvalidCastException)
                {
                    // can happen if the browser has closed but we haven't recognized it yet
                    ResetBrowser(urlString);
                    return;
                }
                PostNavigate(urlString);
            }
        }

        void ResetBrowser(string urlString)
        {
            Dispose();
            StartBrowser();
            SinkEvents();
            browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), urlString, TimeSpan.FromMilliseconds(500), TimeSpan.Zero);
        }

        protected virtual void PostNavigate(string urlString)
        {
            
        }

        protected virtual void PreNavigate(string urlString)
        {

        }

        private static string PrepareUrl(Uri url)
        {
            string str = string.Empty;
            if (url != null)
            {
                if (!url.IsAbsoluteUri)
                {
                    throw new Exception();
                }
                str = url.ToString();
            }
            if (!str.Length.Equals(0))
            {
                return str;
            }
            return null;
        }

        #endregion

        #region Basic Browser Commands
        public void GoBack()
        {
            try
            {
                this.web.GoBack();
            }
            catch
            {
            }
        }

        public void GoForward()
        {
            try
            {
                this.web.GoForward();
            }
            catch
            {

            }
        }

        public void GoHome()
        {
            try
            {
                Navigate(initialUrl);   // original URL that was used in this browser instance
            }
            catch
            {
            }
        }

        public void Navigate(string urlString)
        {
            this.PerformNavigate(urlString, false, null, null, null);
        }

        public void Navigate(string urlString, string targetFrameName, byte[] postData, string additionalHeaders)
        {
            this.PerformNavigate(urlString, false, targetFrameName, postData, additionalHeaders);
        }

        public bool IsBusy
        {
            get
            {
                if (web != null)
                    return web.Busy;
                return false;
            }
        }

        public System.Windows.Forms.WebBrowserReadyState ReadyState
        {
            get
            {
                if (web != null)
                    return (System.Windows.Forms.WebBrowserReadyState)web.ReadyState;
                return System.Windows.Forms.WebBrowserReadyState.Complete;
            }
        }

        //// Properties
        public object Document
        {
            get
            {
                object document = null;
                if (web != null)
                {
                    try
                    {
                        document = this.web.Document;
                    }
                    catch
                    {

                    }
                }
                return document;
            }
        }

        public string StatusText
        {
            get
            {
                string statusText = null;
                try
                {
                    statusText = this.web.StatusText;
                }
                catch
                {
                }
                return statusText;
            }
        }
        #endregion

        #region IBrowserCallback
        public IWebBrowser2 WebBrowser
        {
            get
            {
                return web;
            }
        }

        public void ShowBrowser()
        {
        }

        public void Hide()
        {

        }

        #endregion

        #region ICRMWindowContainer
        public bool BeforeSessionClose()
        {
            return true;
        }

        public void CloseActive()
        {
            desktopAccess.CloseDynamicApplication(this.ApplicationName);
        }

        public void SaveAll()
        {

        }

        public void ShowOutside(string frame)
        {

        }

        public void ShowWindow(string crmurl, string frame, List<string> onLoadHistory, bool hideRibbon, bool hideNav)
        {
            Navigate(crmurl);
        }

        public void ShowWindow(string crmurl, string frame, List<string> onLoadHistory)
        {
            Navigate(crmurl);
        }

        public void ShowWindow(string crmurl, string frame, bool allowReplace)
        {
            Navigate(crmurl);
        }

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
