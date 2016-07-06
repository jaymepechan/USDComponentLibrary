/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Uii.Csr.Browser.Web;
using System.Windows.Forms;
using System.Diagnostics;

namespace Microsoft.USD.ComponentLibrary
{
    //[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
    public class MCSBrowserEventSink : StandardOleMarshalObject, IDisposable, DWebBrowserEvents2, DWebBrowserEvents
    {
        // Fields
        private int _Cookie;
        private int _Cookie2;
        private readonly Dictionary<WebBrowserEvent, Delegate> _Handlers;
        private IConnectionPoint _Icp;
        private IConnectionPoint _Icp2;
        private readonly IBrowserCallback _WebBrowserControl;

        // Methods
        private MCSBrowserEventSink()
        {
            this._Cookie = -1;
            this._Cookie2 = -1;
            this._Handlers = new Dictionary<WebBrowserEvent, Delegate>();
        }

        public MCSBrowserEventSink(IBrowserCallback webBrowserControl)
            : this()
        {
            this._WebBrowserControl = webBrowserControl;
        }

        public void AddHandler(WebBrowserEvent evt, Delegate handler)
        {
            lock (this._Handlers)
            {
                Delegate delegate2;
                this._Handlers.TryGetValue(evt, out delegate2);
                this._Handlers[evt] = Delegate.Combine(delegate2, handler);
            }
        }

        public void Advise(IWebBrowser2 ie)
        {
            //if (this._Cookie.Equals(-1))
            //{
            //    Trace.WriteLine("Advise (1)");
            //    IConnectionPointContainer container = (IConnectionPointContainer)ie;
            //    Guid gUID = typeof(SHDocVw.DWebBrowserEvents).GUID;
            //    container.FindConnectionPoint(ref gUID, out this._Icp);
            //    this._Icp.Advise(this, out this._Cookie);
            //}
            if (this._Cookie2.Equals(-1))
            {
                Trace.WriteLine("Advise (2)");
                IConnectionPointContainer container2 = (IConnectionPointContainer)ie;
                Guid riid = typeof(DWebBrowserEvents2).GUID;
                container2.FindConnectionPoint(ref riid, out this._Icp2);
                this._Icp2.Advise(this, out this._Cookie2);
            }
        }

        public void BeforeNavigate(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.BeforeNavigate);
                if (handler != null)
                {
                    BeforeNavigateEventArgs e = new BeforeNavigateEventArgs();
                    e._URL = URL;
                    e._Flags = Flags;
                    e._TargetFrameName = TargetFrameName;
                    e.PostData = PostData;
                    e._Headers = Headers;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    PostData = e.PostData;
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("BeforeNavigate exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void BeforeNavigate2(object pDisp, ref object URL, ref object Flags, ref object TargetFrameName, ref object PostData, ref object Headers, ref bool Cancel)
        {
            try
            {
                if (TargetFrameName == null)
                {
                    TargetFrameName = string.Empty;
                }
                if (Headers == null)
                {
                    Headers = string.Empty;
                }
                Delegate handler = this.GetHandler(WebBrowserEvent.BeforeNavigate2);
                if (handler != null)
                {
                    BeforeNavigate2EventArgs e = new BeforeNavigate2EventArgs();
                    e.pDisp = pDisp;
                    e.URL = URL;
                    e.Flags = Flags;
                    e.TargetFrameName = TargetFrameName;
                    e.PostData = PostData;
                    e.Headers = Headers;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    URL = e.URL;
                    Flags = e.Flags;
                    TargetFrameName = e.TargetFrameName;
                    PostData = e.PostData;
                    Headers = e.Headers;
                    Cancel = e.Cancel;
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedBeforeNavigate);
                if (handler != null)
                {
                    BrowserExtendedNavigatingEventArgs args2 = new BrowserExtendedNavigatingEventArgs();
                    args2.pDisp = pDisp;
                    args2.Url = URL as string;
                    args2.Flags = Flags;
                    args2._Frame = TargetFrameName as string;
                    args2.Postdata = PostData;
                    args2.Headers = Headers;
                    args2._NavigationContext = UrlContext.None;
                    this.Raise(handler, args2);
                    URL = args2.Url;
                    Flags = args2.Flags;
                    TargetFrameName = args2.Frame;
                    PostData = args2.Postdata;
                    Headers = args2.Headers;
                    Cancel = args2.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("BeforeNavigate2 exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void BrowserObjectAppeared(IWebBrowser2 ie, bool isPopup, int pId)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.BrowserObjectAppeared);
                if (handler != null)
                {
                    BrowserObjectAppearedEventArgs e = new BrowserObjectAppearedEventArgs();
                    e._IE = ie;
                    e._IsPopup = isPopup;
                    e._pId = pId;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("BrowserObjectAppeared exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void BrowserProcessExited()
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.BrowserProcessExited);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
            }
            catch (Exception ex) { Trace.WriteLine("BrowserProcessExited exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        private void ClearHandlers()
        {
            lock (this._Handlers)
            {
                this._Handlers.Clear();
            }
        }

        public void ClientToHostWindow(ref int CX, ref int CY)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.ClientToHostWindow);
                if (handler != null)
                {
                    ClientToHostWindowEventArgs e = new ClientToHostWindowEventArgs();
                    e.CX = CX;
                    e.CY = CY;
                    this.Raise(handler, e);
                    CX = e.CX;
                    CY = e.CY;
                }
            }
            catch (Exception ex) { Trace.WriteLine("ClientToHostWindow exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void CommandStateChange(int Command, bool Enable)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.CommandStateChange);
                if (handler != null)
                {
                    CommandStateChangeEventArgs e = new CommandStateChangeEventArgs();
                    e._Command = Command;
                    e._Enable = Enable;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("CommandStateChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Unadvise();
                this.ClearHandlers();
                this._Icp = null;
                this._Icp2 = null;
            }
        }

        public void DocumentComplete(object pDisp, ref object URL)
        {
            try
            {
                Delegate d = null;
                d = this.GetHandler(WebBrowserEvent.DocumentComplete);
                if (d != null)
                {
                    DocumentCompleteEventArgs e = new DocumentCompleteEventArgs();
                    e._pDisp = pDisp;
                    e.URL = URL;
                    this.Raise(d, e);
                    URL = e.URL;
                }
                d = null;
                if (((IWebBrowser2)pDisp).Equals(this._WebBrowserControl.WebBrowser))
                {
                    d = this.GetHandler(WebBrowserEvent.PageLoadComplete);
                }
                if (d != null)
                {
                    DocumentCompleteEventArgs args2 = new DocumentCompleteEventArgs();
                    args2._pDisp = pDisp;
                    args2.URL = URL;
                    this.Raise(d, args2);
                    URL = args2.URL;
                }
                d = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedWebBrowserDocumentCompleted);
                if (d != null)
                {
                    WebBrowserDocumentCompletedEventArgs args3 = new WebBrowserDocumentCompletedEventArgs(new Uri(URL.ToString()));
                    this.Raise(d, args3);
                    URL = args3.Url.ToString();
                }
            }
            catch (Exception ex) { Trace.WriteLine("DocumentComplete exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void DownloadBegin()
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.DownloadBegin);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedDownloadStarted);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
            }
            catch (Exception ex) { Trace.WriteLine("DownloadBegin exception: " + ex.Message + "\r\n" + ex.StackTrace); }

        }

        public void DownloadComplete()
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.DownloadComplete);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedDownloadCompleted);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
            }
            catch (Exception ex) { Trace.WriteLine("DownloadComplete exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void FileDownload(bool ActiveDocument, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.FileDownload);
                if (handler != null)
                {
                    FileDownloadEventArgs e = new FileDownloadEventArgs();
                    e._ActiveDocument = ActiveDocument;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("FileDownload exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void FrameBeforeNavigate(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.FrameBeforeNavigate);
                if (handler != null)
                {
                    BeforeNavigateEventArgs e = new BeforeNavigateEventArgs();
                    e._URL = URL;
                    e._Flags = Flags;
                    e._TargetFrameName = TargetFrameName;
                    e.PostData = PostData;
                    e._Headers = Headers;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    PostData = e.PostData;
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("FrameBeforeNavigate exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void FrameNavigateComplete(string URL)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.FrameNavigateComplete);
                if (handler != null)
                {
                    NavigateCompleteEventArgs e = new NavigateCompleteEventArgs();
                    e._URL = URL;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("FrameNavigateComplete exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void FrameNewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.FrameNewWindow);
                if (handler != null)
                {
                    NewWindowEventArgs e = new NewWindowEventArgs();
                    e._URL = URL;
                    e._Flags = Flags;
                    e._TargetFrameName = TargetFrameName;
                    e.PostData = PostData;
                    e._Headers = Headers;
                    e.Processed = Processed;
                    this.Raise(handler, e);
                    PostData = e.PostData;
                    Processed = e.Processed;
                }
            }
            catch (Exception ex) { Trace.WriteLine("FrameNewWindow exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        private Delegate GetHandler(WebBrowserEvent evt)
        {
            Delegate delegate2;
            lock (this._Handlers)
            {
                this._Handlers.TryGetValue(evt, out delegate2);
            }
            return delegate2;
        }

        public void NavigateComplete(string URL)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NavigateComplete);
                if (handler != null)
                {
                    NavigateCompleteEventArgs e = new NavigateCompleteEventArgs();
                    e._URL = URL;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("NavigateComplete exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void NavigateComplete2(object pDisp, ref object URL)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NavigateComplete2);
                if (handler != null)
                {
                    NavigateComplete2EventArgs e = new NavigateComplete2EventArgs();
                    e._pDisp = pDisp;
                    e.URL = URL;
                    this.Raise(handler, e);
                    URL = e.URL;
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedWebBrowserNavigated);
                if (handler != null)
                {
                    WebBrowserNavigatedEventArgs args2 = new WebBrowserNavigatedEventArgs(new Uri(URL.ToString()));
                    this.Raise(handler, args2);
                    URL = args2.Url.ToString();
                }
            }
            catch (Exception ex) { Trace.WriteLine("NavigateComplete2 exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void NavigateError(object pDisp, ref object URL, ref object Frame, ref object StatusCode, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NavigateError);
                if (handler != null)
                {
                    NavigateErrorEventArgs e = new NavigateErrorEventArgs();
                    e._pDisp = pDisp;
                    e.URL = URL;
                    e.Frame = Frame;
                    e.StatusCode = StatusCode;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    URL = e.URL;
                    Frame = e.Frame;
                    StatusCode = e.StatusCode;
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("NavigateError exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void NewWindow(string URL, int Flags, string TargetFrameName, ref object PostData, string Headers, ref bool Processed)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NewWindow);
                if (handler != null)
                {
                    NewWindowEventArgs e = new NewWindowEventArgs();
                    e._URL = URL;
                    e._Flags = Flags;
                    e._TargetFrameName = TargetFrameName;
                    e.PostData = PostData;
                    e._Headers = Headers;
                    e.Processed = Processed;
                    this.Raise(handler, e);
                    PostData = e.PostData;
                    Processed = e.Processed;
                }
            }
            catch (Exception ex) { Trace.WriteLine("NewWindow exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void NewWindow2(ref object ppDisp, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NewWindow2);
                if (handler != null)
                {
                    NewWindow2EventArgs e = new NewWindow2EventArgs();
                    e.ppDisp = ppDisp;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    ppDisp = e.ppDisp;
                    Cancel = e.Cancel;
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedBeforeNewWindow);
                if (handler != null)
                {
                    BrowserExtendedNewWindowEventArgs args2 = new BrowserExtendedNewWindowEventArgs();
                    args2.pDisp = ppDisp;
                    this.Raise(handler, args2);
                    ppDisp = args2.pDisp;
                    Cancel = args2.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("NewWindow2 exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void NewWindow3(ref object ppDisp, ref bool Cancel, uint dwFlags, string bstrUrlContext, string bstrUrl)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.NewWindow3);
                if (handler != null)
                {
                    NewWindow3EventArgs e = new NewWindow3EventArgs();
                    e.ppDisp = ppDisp;
                    e.Cancel = Cancel;
                    e._dwFlags = dwFlags;
                    e._bstrUrlContext = bstrUrlContext;
                    e._bstrUrl = bstrUrl;
                    this.Raise(handler, e);
                    ppDisp = e.ppDisp;
                    Cancel = e.Cancel;
                }
                handler = this.GetHandler(WebBrowserEvent.LegacyBrowserExtendedBeforeNewWindow);
                if (handler != null)
                {
                    BrowserExtendedNewWindowEventArgs args2 = new BrowserExtendedNewWindowEventArgs();
                    args2.pDisp = ppDisp;
                    args2.Flags = dwFlags;
                    args2.Url = bstrUrl;
                    args2._UrlContext = bstrUrlContext;
                    this.Raise(handler, args2);
                    ppDisp = args2.pDisp;
                    Cancel = args2.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("NewWindow3 exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnFullScreen(bool FullScreen)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnFullScreen);
                if (handler != null)
                {
                    OnFullScreenEventArgs e = new OnFullScreenEventArgs();
                    e._FullScreen = FullScreen;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnFullScreen exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnMenuBar(bool MenuBar)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnMenuBar);
                if (handler != null)
                {
                    OnMenuBarEventArgs e = new OnMenuBarEventArgs();
                    e._MenuBar = MenuBar;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnMenuBar exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnQuit()
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnQuit);
                if (handler != null)
                {
                    this.Raise(handler, EventArgs.Empty);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnQuit exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnStatusBar(bool StatusBar)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnStatusBar);
                if (handler != null)
                {
                    OnStatusBarEventArgs e = new OnStatusBarEventArgs();
                    e._StatusBar = StatusBar;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnStatusBar exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnTheaterMode(bool TheaterMode)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnTheaterMode);
                if (handler != null)
                {
                    OnTheaterModeEventArgs e = new OnTheaterModeEventArgs();
                    e._TheaterMode = TheaterMode;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnTheaterMode exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnToolBar(bool ToolBar)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnToolBar);
                if (handler != null)
                {
                    OnToolBarEventArgs e = new OnToolBarEventArgs();
                    e._ToolBar = ToolBar;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnToolBar exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void OnVisible(bool Visible)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.OnVisible);
                if (handler != null)
                {
                    OnVisibleEventArgs e = new OnVisibleEventArgs();
                    e._Visible = Visible;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("OnVisible exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void PrintTemplateInstantiation(object pDisp)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.PrintTemplateInstantiation);
                if (handler != null)
                {
                    PrintTemplateInstantiationEventArgs e = new PrintTemplateInstantiationEventArgs();
                    e._pDisp = pDisp;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("PrintTemplateInstantiation exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void PrintTemplateTeardown(object pDisp)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.PrintTemplateTeardown);
                if (handler != null)
                {
                    PrintTemplateTeardownEventArgs e = new PrintTemplateTeardownEventArgs();
                    e._pDisp = pDisp;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("PrintTemplateTeardown exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void PrivacyImpactedStateChange(bool bImpacted)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.PrivacyImpactedStateChange);
                if (handler != null)
                {
                    PrivacyImpactedStateChangeEventArgs e = new PrivacyImpactedStateChangeEventArgs();
                    e._bImpacted = bImpacted;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("PrivacyImpactedStateChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void ProgressChange(int Progress, int ProgressMax)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.ProgressChange);
                if (handler != null)
                {
                    ProgressChangeEventArgs e = new ProgressChangeEventArgs();
                    e._Progress = Progress;
                    e._ProgressMax = ProgressMax;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("ProgressChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void PropertyChange(string szProperty)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.PropertyChange);
                if (handler != null)
                {
                    PropertyChangeEventArgs e = new PropertyChangeEventArgs();
                    e._szProperty = szProperty;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("PropertyChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void Quit(ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.Quit);
                if (handler != null)
                {
                    QuitEventArgs e = new QuitEventArgs();
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("Quit exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        private void Raise(Delegate d, EventArgs e)
        {
            d.DynamicInvoke(new object[] { this._WebBrowserControl, e });
        }

        public void RemoveHandler(WebBrowserEvent evt, Delegate handler)
        {
            lock (this._Handlers)
            {
                Delegate delegate2;
                if (this._Handlers.TryGetValue(evt, out delegate2))
                {
                    delegate2 = Delegate.Remove(delegate2, handler);
                    if (delegate2 != null)
                    {
                        this._Handlers[evt] = delegate2;
                    }
                    else
                    {
                        this._Handlers.Remove(evt);
                    }
                }
            }
        }

        public void SetPhishingFilterStatus(int PhishingFilterStatus)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.SetPhishingFilterStatus);
                if (handler != null)
                {
                    SetPhishingFilterStatusEventArgs e = new SetPhishingFilterStatusEventArgs();
                    e._PhishingFilterStatus = PhishingFilterStatus;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("SetPhishingFilterStatus exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void SetSecureLockIcon(int SecureLockIcon)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.SetSecureLockIcon);
                if (handler != null)
                {
                    SetSecureLockIconEventArgs e = new SetSecureLockIconEventArgs();
                    e._SecureLockIcon = SecureLockIcon;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("SetSecureLockIcon exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void StatusTextChange(string Text)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.StatusTextChange);
                if (handler != null)
                {
                    StatusTextChangeEventArgs e = new StatusTextChangeEventArgs();
                    e._Text = Text;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("StatusTextChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void TitleChange(string Text)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.TitleChange);
                if (handler != null)
                {
                    TitleChangeEventArgs e = new TitleChangeEventArgs();
                    e._Text = Text;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("TitleChange exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void Unadvise()
        {
            if ((this._Icp != null) && (this._Cookie != -1))
            {
                try
                {
                    this._Icp.Unadvise(this._Cookie);
                    //Marshal.FinalReleaseComObject(_Icp);
                    Marshal.ReleaseComObject(_Icp);
                }
                catch (COMException)
                {
                }
                catch (InvalidComObjectException)
                {
                }
                catch (InvalidCastException)
                {
                }
                finally
                {
                    this._Icp = null;
                    this._Cookie = -1;
                }
            }
            if ((this._Icp2 != null) && (this._Cookie2 != -1))
            {
                try
                {
                    this._Icp2.Unadvise(this._Cookie2);
                    //Marshal.FinalReleaseComObject(_Icp2);
                    int iCount = Marshal.ReleaseComObject(_Icp2);
                }
                catch (COMException)
                {
                }
                catch (InvalidComObjectException)
                {
                }
                catch (InvalidCastException)
                {
                }
                finally
                {
                    this._Icp2 = null;
                    this._Cookie2 = -1;
                }
            }
        }

        public void UpdatePageStatus(object pDisp, ref object nPage, ref object fDone)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.UpdatePageStatus);
                if (handler != null)
                {
                    UpdatePageStatusEventArgs e = new UpdatePageStatusEventArgs();
                    e._pDisp = pDisp;
                    e.nPage = nPage;
                    e.fDone = fDone;
                    this.Raise(handler, e);
                    nPage = e.nPage;
                    fDone = e.fDone;
                }
            }
            catch (Exception ex) { Trace.WriteLine("UpdatePageStatus exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void WindowActivate()
        {
        }

        public void WindowClosing(bool IsChildWindow, ref bool Cancel)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowClosing);
                if (handler != null)
                {
                    WindowClosingEventArgs e = new WindowClosingEventArgs();
                    e._IsChildWindow = IsChildWindow;
                    e.Cancel = Cancel;
                    this.Raise(handler, e);
                    Cancel = e.Cancel;
                }
            }
            catch (Exception ex) { Trace.WriteLine("WindowClosing exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void WindowMove()
        {
        }

        public void WindowResize()
        {
        }

        public void WindowSetHeight(int Height)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowSetHeight);
                if (handler != null)
                {
                    WindowSetHeightEventArgs e = new WindowSetHeightEventArgs();
                    e._Height = Height;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("WindowSetHeight exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void WindowSetLeft(int Left)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowSetLeft);
                if (handler != null)
                {
                    WindowSetLeftEventArgs e = new WindowSetLeftEventArgs();
                    e._Left = Left;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("WindowSetLeft exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }

        public void WindowSetResizable(bool Resizable)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowSetResizable);
                if (handler != null)
                {
                    WindowSetResizableEventArgs e = new WindowSetResizableEventArgs();
                    e._Resizable = Resizable;
                    this.Raise(handler, e);
                }
            }
            catch { }
        }

        public void WindowSetTop(int Top)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowSetTop);
                if (handler != null)
                {
                    WindowSetTopEventArgs e = new WindowSetTopEventArgs();
                    e._Top = Top;
                    this.Raise(handler, e);
                }
            }
            catch { }
        }

        public void WindowSetWidth(int Width)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowSetWidth);
                if (handler != null)
                {
                    WindowSetWidthEventArgs e = new WindowSetWidthEventArgs();
                    e._Width = Width;
                    this.Raise(handler, e);
                }
            }
            catch { }
        }

        public void WindowStateChanged(uint dwWindowStateFlags, uint dwValidFlagsMask)
        {
            try
            {
                Delegate handler = this.GetHandler(WebBrowserEvent.WindowStateChanged);
                if (handler != null)
                {
                    WindowStateChangedEventArgs e = new WindowStateChangedEventArgs();
                    e._dwWindowStateFlags = dwWindowStateFlags;
                    e._dwValidFlagsMask = dwValidFlagsMask;
                    this.Raise(handler, e);
                }
            }
            catch (Exception ex) { Trace.WriteLine("WindowStateChanged exception: " + ex.Message + "\r\n" + ex.StackTrace); }
        }


        public void NewProcess(int lCauseFlag, object pWB2, bool Cancel)
        {
        }

        public void RedirectXDomainBlocked(object pDisp, ref object StartURL, ref object RedirectURL, ref object Frame, ref object StatusCode)
        {
        }

        public void ThirdPartyUrlBlocked(ref object URL, uint dwCount)
        {
        }

        /// <summary>
        /// Unused handler for windows 8 / IE 10
        /// </summary>
        /// <param name="pDispWindow"></param>
        public void BeforeScriptExecute(object pDispWindow)
        { }

        public void BeforeScriptExecute(object pDispWindow, object pActiveScript)
        {

        }

        public void WebWorkerFinsihed(uint dwUniqueID)
        {

        }

        public void WebWorkerStarted(uint dwUniqueID, string bstrWorkerLabel)
        {

        }
    }
}
