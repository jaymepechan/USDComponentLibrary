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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters
{
    public class ChromeBrowser : MicrosoftBase, ICRMWindowContainer, IBrowserCallback, IDisposable
    {
        #region Fields
        public string BrowserId = String.Empty;
        public IntPtr _webBrowser;
        string initialUrl = String.Empty;
        private Process _Process;
        EventHandler processExitedHandler;
        private volatile Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState _WebBrowserExtendedState;
        #endregion

        #region Construction / Destruction
        [WebBrowserPermissionAttribute(SecurityAction.Demand, Level = WebBrowserPermissionLevel.Unrestricted)]
        public ChromeBrowser()
        {
        }

        public ChromeBrowser(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            try
            {
                CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                thisApplication = CRMWindowRouter.LoadApplicationEntity(this.ApplicationName);
                processExitedHandler = new EventHandler(this._Process_Exited);
                this.IsVisibleChanged += FloatingBrowser_IsVisibleChanged;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ChromeBrowser: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("Navigate", Navigate);
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
                    Win32API.ShowWindow(_webBrowser, Win32API.ShowWindowValues.SW_SHOW);
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
                    Win32API.ShowWindow(_webBrowser, Win32API.ShowWindowValues.SW_HIDE);
                }
            }
            catch (Exception ex)
            {

            }
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

        void FloatingBrowser_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        public void _CloseBrowser()
        {
            this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserNotAcquired;
            Dispose();
        }

        private void _Process_Exited(object sender, EventArgs e)
        {
            Trace.WriteLine("Process_Exited handler: browser process exited");
            Dispatcher.BeginInvoke(new System.Action(() =>
            {
                CloseActive();
            }));
        }

        private void _Process_Exited2(object state)
        {
            this._CloseBrowser();
        }

        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);


        internal class ChromeLauncher
        {
            public Process ChromeProcess;
            public IntPtr ChromeWindow;

            private const string ChromeAppKey = @"\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe";

            public IEnumerable<IntPtr> EnumerateProcessWindowHandles()
            {
                var handles = new List<IntPtr>();

                foreach (ProcessThread thread in PrimaryChromeProcess.Threads)
                    EnumThreadWindows(thread.Id,
                        (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

                return handles;
            }

            private string ChromeAppFileName
            {
                get
                {
                    return (string)(Registry.GetValue("HKEY_LOCAL_MACHINE" + ChromeAppKey, "", null) ??
                                        Registry.GetValue("HKEY_CURRENT_USER" + ChromeAppKey, "", null));
                }
            }

            private static Process PrimaryChromeProcess = null;

            public Process OpenLink(string url)
            {
                string chromeAppFileName = ChromeAppFileName;
                if (string.IsNullOrEmpty(chromeAppFileName))
                {
                    throw new Exception("Could not find chrome.exe!");
                }

                ProcessStartInfo si = new ProcessStartInfo()
                {
                    FileName = chromeAppFileName,
                    // --dom-automation
                    Arguments = " --new-window " + url, //--process-per-tab
                    //UseShellExecute = false
                };
                ChromeProcess = Process.Start(si);
                //if (PrimaryChromeProcess == null)
                //    PrimaryChromeProcess = ChromeProcess;
                ChromeWindow = ChromeProcess.MainWindowHandle;

                return ChromeProcess;
            }
        }

        System.Threading.Timer browserAquiredTimer;
        DateTime browserAquiredStart;
        private void StartBrowser(string url)
        {
            this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserAcquiring;
            ChromeLauncher cl = new ChromeLauncher();
            _Process = cl.OpenLink(url);
            if (_Process == null)
                throw new Exception("Unable to attach to browser.   Protected Mode may be disable");
            _webBrowser = cl.ChromeWindow;
            this._Process.Exited += _Process_Exited;
            this._Process.EnableRaisingEvents = true;
            this._WebBrowserExtendedState = Microsoft.Uii.Csr.Browser.Web.WebBrowserExtendedState.BrowserAcquired;
            if (_webBrowser != IntPtr.Zero)
            {
                ManipulateWindow();
            }
            else
            {
                browserAquiredStart = DateTime.Now;
                browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), null, TimeSpan.FromMilliseconds(300), TimeSpan.Zero);
            }

        }

        void ManipulateWindow()
        {
            Win32API.ShowWindow(_webBrowser, Win32API.ShowWindowValues.SW_MAXIMIZE);
            //string extensionsXml = (string)this.thisApplication["uii_extensionsxml"];
            //if (!String.IsNullOrEmpty(extensionsXml))
            //{
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(extensionsXml);
            //    if (GetXmlSetting(doc, "AddressBar") == "false") { this.web.AddressBar = false; } else { this.web.AddressBar = true; }
            //    if (GetXmlSetting(doc, "MenuBar") == "false") { this.web.MenuBar = false; } else { this.web.MenuBar = true; }
            //    if (GetXmlSetting(doc, "StatusBar") == "false") { this.web.StatusBar = false; } else { this.web.StatusBar = true; }
            //    if (GetXmlSetting(doc, "ToolBar") == "0") { this.web.ToolBar = 0; }
            //}
            bool userCanClose = (bool)this.thisApplication["uii_usercanclose"];
            if (userCanClose == false)
                RemoveBrowserClose(_webBrowser);
        }

        void BrowserAquiredCheck(object obj)
        {
            if (_Process != null && _Process.MainWindowHandle != IntPtr.Zero)
            {
                _webBrowser = _Process.MainWindowHandle;
                ManipulateWindow();
                if (browserAquiredTimer != null)
                    browserAquiredTimer.Dispose();
                browserAquiredTimer = null;
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
                    browserAquiredTimer = new System.Threading.Timer(new TimerCallback(BrowserAquiredCheck), null, TimeSpan.FromMilliseconds(500), TimeSpan.Zero);
                }
            }
        }

        string GetXmlSetting(XmlDocument doc, string setting)
        {
            try
            {
                XmlNode nodeSetting = doc.SelectSingleNode("//settings/" + setting);
                if (nodeSetting != null)
                {
                    return nodeSetting.Attributes["value"].Value;
                }
            }
            catch
            {
            }
            return null;
        }

        void RemoveBrowserClose(IntPtr winHandle)
        {
            uint dwNewLong = NativeMethods.GetWindowLong(winHandle, NativeMethods.WinUserConstant.GWL_STYLE);
            //dwNewLong ^= 0x00800000;    // WS_BORDER
            //dwNewLong ^= 0x00C00000;    // WS_CAPTION
            //dwNewLong ^= 0x00010000;    // WS_MAXIMIZEBOX
            //dwNewLong ^= 0x00020000;    // WS_MINIMIZEBOX
            dwNewLong ^= 0x00080000;    // WS_SYSMENU
            //dwNewLong ^= 0x00040000;    // WS_SIZEFRAME
            //dwNewLong |= 0x40000000;    // WS_CHILD
            NativeMethods.SetWindowLong(winHandle, NativeMethods.WinUserConstant.GWL_STYLE, dwNewLong);
        }

        #region Events
        // The following setup is done instead of passing through the event calls so that we can handle the events in here too.

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
            try
            {
                if (_webBrowser != IntPtr.Zero)
                    Win32API.CloseWindow(_webBrowser);
                _webBrowser = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("WpfBrowser Dispose: " + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
        #endregion

        #region Misc

        private void PerformNavigate(string urlString, bool newWindow, string targetFrameName, byte[] postData, string headers)
        {
            if (String.IsNullOrEmpty(initialUrl))
                initialUrl = urlString;
            BrowserId = urlString;
            StartBrowser(urlString);
        }
        #endregion

        #region Basic Browser Commands
        public void GoBack()
        {

        }

        public void GoForward()
        {

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
                return false;
            }
        }

        public System.Windows.Forms.WebBrowserReadyState ReadyState
        {
            get
            {
                return System.Windows.Forms.WebBrowserReadyState.Complete;
            }
        }

        //// Properties
        public object Document
        {
            get
            {
                return null;
            }
        }

        public string StatusText
        {
            get
            {
                return "";
            }
        }
        #endregion

        #region IBrowserCallback
        public IWebBrowser2 WebBrowser
        {
            get
            {
                return null;
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
            return true;
        }

        public void ReRoute(string frame)
        {

        }
        #endregion

    }
}
