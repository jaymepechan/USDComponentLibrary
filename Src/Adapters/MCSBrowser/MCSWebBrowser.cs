/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
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
using mshtml;
using System.Reflection;
using Microsoft.Uii.AifServices;
using System.Diagnostics;
using System.Web;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Interop;
using HtmlEventsSmple;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.Uii.Csr.Browser.Web;
using System.Windows.Forms.Integration;
using Microsoft.Xrm.Sdk;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for WpfBrowser.xaml
    /// </summary>
    public class MCSWebBrowser : UserControl, IBrowserCallback, IDisposable
    {
        #region Fields
        private Entity thisApplication;
        private IntPtr _hWndFromEnumCallback;
        public IWebBrowser2 web = null;
        public string BrowserId = String.Empty;
        public string ApplicationName;
        public object _webBrowser;
        HwndSource hwndHookSource = null;
        bool dataCaptured = false;
        public bool IsDashboard = false;
        public Dictionary<string, HTMLDocument> frames = new Dictionary<string, HTMLDocument>();
        protected readonly MCSBrowserEventSink _WebBrowserEventSink;
        protected static List<Object> activeBrowsers = new List<Object>();
        string initialUrl = String.Empty;

        #endregion

        #region Construction / Destruction
        [WebBrowserPermissionAttribute(SecurityAction.Demand, Level = WebBrowserPermissionLevel.Unrestricted)]
        public MCSWebBrowser()
        {
            try
            {
                System.Windows.Controls.WebBrowser newbrowser = new System.Windows.Controls.WebBrowser();
                lock (activeBrowsers)
                {
                    if (!activeBrowsers.Contains(newbrowser))
                        activeBrowsers.Add(newbrowser);
                    Trace.WriteLine("MCSWebBrowser: ActiveBrowsers (Browser Added) = " + activeBrowsers.Count.ToString());
                }
                _webBrowser = newbrowser;
                this.Content = newbrowser;
                this._WebBrowserEventSink = new MCSBrowserEventSink(this);
                SinkEvents();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void Reset(string applicationName)
        {
            try
            {
                dataCaptured = false;
                ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                thisApplication = CRMWindowRouter.LoadApplicationEntity(applicationName);
                this.ApplicationName = applicationName;
                //HideScriptErrors(true);
                initialUrl = String.Empty;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        public void HaltUse()
        {
            dataCaptured = false;
            thisApplication = null;
            this.ApplicationName = String.Empty;
            if (WebBrowser != null)
            {
                WebBrowser.Stop();
                WebBrowser.Navigate("about:blank");
            }
            initialUrl = String.Empty;
        }

        #region Events
        // The following setup is done instead of passing through the event calls so that we can handle the events in here too.

        public event EventHandler<BeforeNavigate2EventArgs> BeforeNavigate2;
        public event EventHandler<BrowserObjectAppearedEventArgs> BrowserObjectAppeared;
        public event EventHandler<DocumentCompleteEventArgs> DocumentComplete;
        public event EventHandler<NavigateComplete2EventArgs> NavigateComplete2;
        public event EventHandler<NewWindow3EventArgs> NewWindow3;
        public event EventHandler<TitleChangeEventArgs> TitleChange;
        public event EventHandler<StatusTextChangeEventArgs> StatusTextChange;
        public event EventHandler<SetSecureLockIconEventArgs> SetSecureLockIcon;

        EventHandler<BeforeNavigate2EventArgs> beforeNavigateHandler;
        EventHandler<BrowserObjectAppearedEventArgs> browserObjectAppearedHandler;
        EventHandler<DocumentCompleteEventArgs> documentCompleteHandler;
        EventHandler<NavigateComplete2EventArgs> navigateComplete2Handler;
        EventHandler<NewWindow3EventArgs> newWindow3Handler;
        EventHandler<TitleChangeEventArgs> titleChangeHandler;
        EventHandler<StatusTextChangeEventArgs> statusTextChangeHandler;
        EventHandler<SetSecureLockIconEventArgs> setSecureLockIconHandler;

        void SinkEvents()
        {
            beforeNavigateHandler = new EventHandler<BeforeNavigate2EventArgs>(Browser_BeforeNavigate2);
            browserObjectAppearedHandler = new EventHandler<BrowserObjectAppearedEventArgs>(Browser_BrowserObjectAppeared);
            documentCompleteHandler = new EventHandler<DocumentCompleteEventArgs>(Browser_DocumentComplete);
            navigateComplete2Handler = new EventHandler<NavigateComplete2EventArgs>(Browser_NavigateComplete2);
            newWindow3Handler = new EventHandler<NewWindow3EventArgs>(Browser_NewWindow3);
            titleChangeHandler = new EventHandler<TitleChangeEventArgs>(Browser_TitleChange);
            statusTextChangeHandler = new EventHandler<StatusTextChangeEventArgs>(Browser_StatusTextChange);
            setSecureLockIconHandler = new EventHandler<SetSecureLockIconEventArgs>(Browser_SetSecureLockIcon);

            _WebBrowserEventSink.AddHandler(WebBrowserEvent.BeforeNavigate2, beforeNavigateHandler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.BrowserObjectAppeared, browserObjectAppearedHandler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.DocumentComplete, documentCompleteHandler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.NavigateComplete2, navigateComplete2Handler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.NewWindow3, newWindow3Handler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.TitleChange, titleChangeHandler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.StatusTextChange, statusTextChangeHandler);
            _WebBrowserEventSink.AddHandler(WebBrowserEvent.SetSecureLockIcon, setSecureLockIconHandler);
        }

        void UnsinkEvents()
        {
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.BeforeNavigate2, beforeNavigateHandler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.BrowserObjectAppeared, browserObjectAppearedHandler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.DocumentComplete, documentCompleteHandler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.NavigateComplete2, navigateComplete2Handler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.NewWindow3, newWindow3Handler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.TitleChange, titleChangeHandler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.StatusTextChange, statusTextChangeHandler);
            _WebBrowserEventSink.RemoveHandler(WebBrowserEvent.SetSecureLockIcon, setSecureLockIconHandler);
        }

        protected void Browser_BeforeNavigate2(object sender, BeforeNavigate2EventArgs e)
        {
            if (BeforeNavigate2 != null)
                BeforeNavigate2(sender, e);
        }
        protected void Browser_BrowserObjectAppeared(object sender, BrowserObjectAppearedEventArgs e)
        {
            if (BrowserObjectAppeared != null)
                BrowserObjectAppeared(sender, e);
        }
        protected void Browser_DocumentComplete(object sender, DocumentCompleteEventArgs e)
        {
            if (DocumentComplete != null)
                DocumentComplete(sender, e);
        }
        protected void Browser_NavigateComplete2(object sender, NavigateComplete2EventArgs e)
        {
            if (NavigateComplete2 != null)
                NavigateComplete2(sender, e);
        }
        protected void Browser_NewWindow3(object sender, NewWindow3EventArgs e)
        {
            if (NewWindow3 != null)
                NewWindow3(sender, e);
        }
        protected void Browser_TitleChange(object sender, TitleChangeEventArgs e)
        {
            if (TitleChange != null)
                TitleChange(sender, e);
        }
        protected void Browser_StatusTextChange(object sender, StatusTextChangeEventArgs e)
        {
            if (StatusTextChange != null)
                StatusTextChange(sender, e);
        }
        protected void Browser_SetSecureLockIcon(object sender, SetSecureLockIconEventArgs e)
        {
            if (SetSecureLockIcon != null)
                SetSecureLockIcon(sender, e);
        }
        #endregion

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
            try { unHookupCloseDetection(); }
            catch (Exception ex)
            {
                Trace.WriteLine("Close Detection Error: " + ex.Message + "\r\n" + ex.StackTrace);
            }
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
                lock (activeBrowsers)
                {
                    if (activeBrowsers.Contains(webBrowser))
                        activeBrowsers.Remove(webBrowser);
                    Trace.WriteLine("MCSWebBrowser: ActiveBrowsers (Browser Removed) = " + activeBrowsers.Count.ToString());
                }
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
                this.Content = null;
                _webBrowser = null;
                web = null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser Dispose: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        #endregion

        #region HTML Utility
        /// <summary>
        /// Finds a Document containing the id requested. 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private HTMLDocument GetDocumentContainingId(HTMLDocument doc, string id)
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
        private static HTMLDocument CheckForValue(HTMLDocument doc, string id, ref bool bFoundRightFrame)
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

        void LoadFrameList()
        {
            if (IsDashboard == true && web.Document != null)
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

        public Dictionary<string, HTMLDocument> GetFrames()
        {
            Dictionary<string, HTMLDocument> ret = new Dictionary<string, HTMLDocument>();
            if (web.Document != null)
            {
                for (int i = 0; i < ((HTMLDocument)web.Document).frames.length; i++)
                {
                    object itemRef = i;
                    mshtml.HTMLWindow2 frmElement = (HTMLWindow2)((HTMLDocument)web.Document).frames.item(ref itemRef);
                    ret.Add(((HTMLIFrame)frmElement.frameElement).id, (HTMLDocument)frmElement.document);
                }
            }
            return ret;
        }

        public string LocateFrame(string contexturl)
        {
            LoadFrameList();
            lock (frames)
            {
                foreach (KeyValuePair<string, HTMLDocument> frmElement in frames)
                {
                    if (frmElement.Value.url.Equals(contexturl, StringComparison.InvariantCultureIgnoreCase))
                        return frmElement.Key;
                    if (IsFrameInDocument(frmElement.Value, contexturl))
                        return frmElement.Key;
                }
                return String.Empty;
            }
        }

        private bool IsFrameInDocument(HTMLDocument doc, string contexturl)
        {
            for (int i = 0; i < doc.frames.length; i++)
            {
                object itemRef = i;
                mshtml.HTMLWindow2 frmElem = (HTMLWindow2)doc.frames.item(ref itemRef);
                HTMLIFrame frm = (HTMLIFrame)frmElem.frameElement;
                if (frm.src != null)
                {
                    string temp = frm.src;
                    Debug.WriteLine("Found Frame: " + temp);
                    if (contexturl.EndsWith(frm.src, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                }
                if (frmElem.document != null)
                {
                    HTMLDocument subdoc = (HTMLDocument)frmElem.document;
                    if (IsFrameInDocument(subdoc, contexturl))
                        return true;
                }
            }
            return false;
        }

        public HTMLDocument GetDocument(string frame)
        {
            LoadFrameList();
            HTMLDocument xrmDoc = null;
            if (IsDashboard == true && web.Document != null)
            {
                lock (frames)
                    xrmDoc = frames.FirstOrDefault(a => a.Key.Equals(frame, StringComparison.InvariantCultureIgnoreCase)).Value;
            }
            else
                xrmDoc = (HTMLDocument)web.Document;
            if (xrmDoc == null)
                return null;
            return xrmDoc;
        }

        private string GetUrl(string frame)
        {
            HTMLDocument doc = GetDocument(frame);
            return doc.url;
        }

        private void InsertScript(HTMLDocument xrmDoc, string script)
        {
            IHTMLScriptElement scriptErrorSuppressed = (IHTMLScriptElement)xrmDoc.createElement("SCRIPT");
            scriptErrorSuppressed.type = "text/javascript";
            scriptErrorSuppressed.text = script;
            IHTMLElementCollection nodes = xrmDoc.getElementsByTagName("head");
            foreach (IHTMLElement elem in nodes)
            {
                HTMLHeadElement head = (HTMLHeadElement)elem;
                head.appendChild((IHTMLDOMNode)scriptErrorSuppressed);
            }
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

        #region Capture IE
        List<HwndSource> hookedWindows = new List<HwndSource>();
        HwndSourceHook hwndSourceHook = null;
        private void hookupCloseDetection()
        {
            try
            {
                if (this._webBrowser is System.Windows.Controls.WebBrowser)
                {
                    hwndHookSource = PresentationSource.FromVisual(this) as HwndSource;
                    if (hwndHookSource == null)
                        return;
                    if (hookedWindows.Contains(hwndHookSource))
                    {
                        Debug.WriteLine("hookupCloseDetection: Hook already found for this HwndSource");
                        return;
                    }
                    Debug.WriteLine("hookupCloseDetection: Hook added");
                    hookedWindows.Add(hwndHookSource);
                    if (hwndSourceHook == null)
                        hwndSourceHook = new HwndSourceHook(WndProc);
                    hwndHookSource.AddHook(hwndSourceHook);
                }
            }
            catch
            {
            }
        }

        private void unHookupCloseDetection()
        {
            try
            {
                if (hwndHookSource != null && hookedWindows.Contains(hwndHookSource))
                {
                    hwndHookSource.RemoveHook(hwndSourceHook);
                    hookedWindows.Remove(hwndHookSource);
                    hwndSourceHook = null;
                }
            }
            catch
            {
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            Debug.WriteLine("OnVisualParentChanged("+this.ApplicationName+"): To(" + (this.Parent != null ? this.Parent.GetType().ToString() : "") + ")");
            base.OnVisualParentChanged(oldParent);
            unHookupCloseDetection();
            hookupCloseDetection();
        }

        delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_PARENTNOTIFY)
            {
                try
                {
                    if (wParam.ToInt32() == WM_DESTROY)
                    {
                        if (webBrowser != null)
                        {
                            // PresenterControl.cs hooks the window.open script so this will generally be avoided now.
                            if (webBrowser is System.Windows.Controls.WebBrowser)
                            {
                                if (this.Closing != null && ((System.Windows.Controls.WebBrowser)webBrowser).Handle == lParam)
                                {
                                    Trace.WriteLine("Window closed from inside!");
                                    this.Closing();
                                    handled = true;
                                    Trace.WriteLine("Window closed from inside: DONE!");
                                }
                            }
                            else
                            {
                                // TODO:
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("WndProc exception: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            return IntPtr.Zero;
        }

        private const int WM_PARENTNOTIFY = 0x0210;
        private const int WM_DESTROY = 2;
        public delegate void ClosingEventHandler();
        public event ClosingEventHandler Closing;

        private bool EnumChildWindowsCallback(IntPtr hWnd, IntPtr ieFrameWindowHandle)
        {
            if (NativeMethods.GetWindowClass(hWnd).Equals("Internet Explorer_Server", StringComparison.Ordinal))
            {
                this._hWndFromEnumCallback = hWnd;
                return false;
            }
            return true;
        }

        private bool EnumWindowsCallback(IntPtr hWnd, IntPtr pId)
        {
            if (NativeMethods.GetProcessIdFromWindow(hWnd).Equals(pId.ToInt32()) && NativeMethods.GetWindowClass(hWnd).Equals("IEFrame", StringComparison.Ordinal))
            {
                this._hWndFromEnumCallback = hWnd;
                return false;
            }
            return true;
        }

        private IntPtr FindIEServerWindowHandle(IntPtr ieFrameWindowHandle)
        {
            this._hWndFromEnumCallback = IntPtr.Zero;
            if (!ieFrameWindowHandle.Equals(IntPtr.Zero))
            {
                NativeMethods.EnumThreadWindowsCallback callback = null;
                callback = new NativeMethods.EnumThreadWindowsCallback(this.EnumChildWindowsCallback);
                NativeMethods.EnumChildWindows(new HandleRef(this, ieFrameWindowHandle), callback, ieFrameWindowHandle);
                GC.KeepAlive(callback);
            }
            return this._hWndFromEnumCallback;
        }

        private IWebBrowser2 GetIWebBrowser2(IntPtr ieFrameWindowHandle)
        {
            IntPtr ptr;
            long tickCount = Environment.TickCount;
            while (true)
            {
                ptr = this.FindIEServerWindowHandle(ieFrameWindowHandle);
                if (!ptr.Equals(IntPtr.Zero))
                {
                    break;
                }
                if ((Environment.TickCount - tickCount) >= 0x1388L)
                {
                    break;
                }
                Thread.Sleep(0x21);
            }
            tickCount = Environment.TickCount;
            IWebBrowser2 browser = null;
            while (!ptr.Equals(IntPtr.Zero))
            {
                browser = FindIWebBrowser2(ptr);
                if (browser != null)
                {
                    return browser;
                }
                if ((Environment.TickCount - tickCount) >= 0x1388L)
                {
                    return browser;
                }
                Thread.Sleep(0x21);
            }
            return browser;
        }

        private static IWebBrowser2 FindIWebBrowser2(IntPtr ieServerWindowHandle)
        {
            IWebBrowser2 browser = null;
            if (!ieServerWindowHandle.Equals(IntPtr.Zero))
            {
                UIntPtr ptr;
                uint msg = NativeMethods.RegisterWindowMessage("WM_HTML_GETOBJECT");
                if (!(NativeMethods.SendMessageTimeout(ieServerWindowHandle, msg, UIntPtr.Zero, IntPtr.Zero, NativeMethods.SendMessageTimeoutFlags.SMTO_ABORTIFHUNG, 0x3e8, out ptr) != IntPtr.Zero))
                {
                    return browser;
                }
                Microsoft.Uii.Csr.Browser.Web.IServiceProvider provider = null;
                try
                {
                    provider = (Microsoft.Uii.Csr.Browser.Web.IServiceProvider)NativeMethods.ObjectFromLresult(ptr, NativeMethods.IID_IHTMLDocument, IntPtr.Zero);
                }
                catch (InvalidCastException)
                {
                }
                if (provider != null)
                {
                    object obj2;
                    provider.QueryService(ref NativeMethods.IID_IWebBrowserApp, ref NativeMethods.IID_IWebBrowser2, out obj2);
                    browser = obj2 as IWebBrowser2;
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(provider);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj2);
                }
            }
            return browser;
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

            if (web == null)
            {
                Dispatcher.Invoke(new System.Action(() =>
                {
                    try
                    {
                        if (webBrowser is System.Windows.Controls.WebBrowser)
                            ((System.Windows.Controls.WebBrowser)webBrowser).Navigate("about:blank");
                    }
                    catch
                    {
                    }
                }));
            }

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
                if (true == (bool)Dispatcher.Invoke(new System.Func<bool>(() =>
                {
                    try
                    {
                        if (webBrowser is System.Windows.Controls.WebBrowser)
                        {
                            if (((System.Windows.Controls.WebBrowser)webBrowser).Handle == IntPtr.Zero)
                                return false;
                            web = GetIWebBrowser2(((System.Windows.Controls.WebBrowser)webBrowser).Handle);
                        }
                        else
                        {
                            Debug.Assert(false);    // TODO:
                        }
                        if (web == null)
                            return false;
                        return true;
                    }
                    catch { }
                    return true;
                })))
                {
                    if (browserAquiredTimer != null)
                        browserAquiredTimer.Dispose();
                    browserAquiredTimer = null;
                    Dispatcher.Invoke(new System.Action(() =>
                    {
                        try
                        {
                            this._WebBrowserEventSink.BrowserObjectAppeared(web, false, 0);
                            this._WebBrowserEventSink.Advise(web);
                        }
                        catch { }
                        try
                        {
                            web.Navigate(urlString);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("Error in webBrowser.Navigate(" + urlString + ") " + ex.Message);
                        }
                    }));
                    return;
                }
                else
                {
                    if (DateTime.Now - browserAquiredStart > TimeSpan.FromSeconds(45))
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
                }
            }
            else
            {
                web.Navigate(urlString);
            }
        }

        public void HideScriptErrors(bool Hide)
        {
            if (webBrowser is System.Windows.Controls.WebBrowser)
            {
                FieldInfo fiComWebBrowser = typeof(System.Windows.Controls.WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                if (fiComWebBrowser == null)
                    return;
                object objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
                if (objComWebBrowser == null)
                    return;
                objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { Hide });
            }
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
            this.RunScript(string.Format("if (typeof(ScanForData) == 'undefined') window.open('http://event/?eventname=usdreload'); else ScanForData('{0}',{1});", string.Empty, this.UseEntityData ? "true" : "false"), string.Empty);
        }

        public void InjectUSDProcessingScript()
        {
            string script = GetStringResource("msdyusd_CRMPageDocumentComplete.js");
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

        #endregion

    }

}
