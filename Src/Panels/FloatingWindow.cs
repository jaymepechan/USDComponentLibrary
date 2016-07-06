/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using Microsoft.Uii.Desktop.UI.Controls;
using Microsoft.Uii.Csr;
using System.Windows;
using System.Threading;
using System.Xml;
using System.Windows.Automation;
using Microsoft.Uii.Csr.Aif.HostedApplication;
using System.ComponentModel;
using Microsoft.Uii.Desktop.UI.Controls.Wpf;
using System.Windows.Forms.Integration;
using System.IO;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using Microsoft.Uii.AifServices;
using Microsoft.Xrm.Sdk;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using Microsoft.Uii.Desktop.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;

namespace Microsoft.USD.ComponentLibrary
{
    public class FloatingPanel : System.Windows.Controls.Control, IPanel, IUSDPanel
    {
        // Fields
#pragma warning disable 169
        private CloseApplicationClickEventHandler CloseApplicationClickEvent;
        private SelectedAppChangedEventHandler SelectedAppChangedEvent;
        private ICRMWindowRouter CRMWindowRouter;
        private List<Window> floatingWindows = new List<Window>();
        private string _panelName = String.Empty;
        AgentDesktopSessions SessionManager = null;

        #region Initialization
        public FloatingPanel()
        {
            base.Name = string.Empty;
            try
            {
                ((System.Windows.Application)System.Windows.Application.Current).MainWindow.Closed += new EventHandler(MainWindow_Closed);
            }
            catch
            {
            }

        }

        bool SessionManager_SessionCloseEvent(Session session)
        {
            foreach (IHostedApplication application2 in session.AppHost.GetApplications())
            {
                if (application2 is IHostedApplication5)
                {
                    if (((IHostedApplication5)application2).TopLevelWpfWindow.Parent != null)
                    {
                        Window w = ((IHostedApplication5)application2).TopLevelWpfWindow.Parent as Window;
                        if (w != null)
                        {
                            w.Content = null;
                            w.Close();
                        }
                    }
                }
                else if (application2 is IHostedApplication)
                {
                    //IHostedApplication application3 = application2 as IHostedApplication;
                    //if (application3.TopLevelWindow != null)
                    //{
                    //    AifWindowsFormsHost uiElement = new AifWindowsFormsHost(application3.TopLevelWindow);
                    //    application3.TopLevelWindow.Tag = uiElement;
                    //}
                }
                else if (application2 is System.Windows.Forms.Control)
                {
                    //System.Windows.Forms.Control c = application2 as System.Windows.Forms.Control;
                    //AifWindowsFormsHost host2 = new AifWindowsFormsHost(c);
                    //c.Tag = host2;
                }

            }
            return true;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            while (floatingWindows.Count > 0)
            {
                floatingWindows[0].Close();
            }
        }

        public new string Name
        {
            get
            {
                if (String.IsNullOrEmpty(_panelName))
                    return base.Name;
                return _panelName;
            }
            set
            {
                _panelName = value;
            }
        }
        #endregion

        #region IPanel
        public object Add(object child, bool closeButton)
        {
            return this.Add(child, null, false, closeButton);
        }

        public object Add(object child, string initializationXml, bool useToolbar, bool closeButton)
        {
            if (!string.IsNullOrEmpty(initializationXml))
            {
                XmlDocument document = new XmlDocument();
                document.LoadXml(initializationXml);
                if (document.DocumentElement.SelectSingleNode("descendant::toolbar") != null)
                {
                    useToolbar = true;
                }
            }
            if (!(child is IHostedApplication5))
            {
                throw new Exception("Floating Panel does not support legacy hosted controls");
            }
            IHostedApplication app = child as IHostedApplication;
            return this.ShowControl(child);
        }

        [Browsable(false)]
        public bool Floating
        {
            get
            {
                return true;   // does not support floating panels
            }
            set
            {
                //this.floating = value;
            }
        }

        public bool IsApplicationOnPanel(Guid id)
        {
            foreach (Window appwindow in this.floatingWindows)
            {
                if (appwindow.Content is IHostedApplication)
                {
                    IHostedApplication application = appwindow.Content as IHostedApplication;
                    if ((application != null) && (application.ApplicationID == id))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Remove(object app)
        {
            return this.RemoveAppFromControl(app);
        }

        // Events
#pragma warning disable 67
        public event CloseApplicationClickEventHandler CloseApplicationClick;
        public event SelectedAppChangedEventHandler SelectedAppChanged;
        protected void CloseAppClickHandler(IHostedApplication app)
        {
            if (this.CloseApplicationClick != null)
            {
                this.CloseApplicationClick(app);
            }
        }

        private void CloseButtonClickHandler(object application)
        {
            this.CloseAppClickHandler(application as IHostedApplication);
        }

        #endregion

        #region IPanel Entry Functions
        private object ShowControl(object app)
        {
            return this.AddApplicationToControl(app);
        }

        private bool RemoveAppFromControl(object app)
        {
            foreach (Window appwindow in this.floatingWindows)
            {
                if (appwindow.Tag != app)
                {
                    continue;
                }
                object application = appwindow.Tag;
                //appwindow.Tag = null;
                if (application is IHostedApplication5)
                {
                    //((IHostedApplication5)application).Close();
                    appwindow.Hide();
                }
                //appwindow.Close();
                floatingWindows.Remove(appwindow);
                return true;
            }
            return false;
        }
        #endregion

        #region Panel Manipulation

        private Window AddApplicationToControl(object app)
        {
            Window appwindow = null;
            IHostedApplication application = app as IHostedApplication;
            if (application != null)
            {
                string applicationName = Utility.GetApplicationDisplayName(application);
                appwindow = ShowApplication(app, applicationName);
                if (application == DesktopApplicationUIBase.AppWithFocus)
                {
                    appwindow.Focus();
                }
            }
            else if (app is System.Windows.Forms.Control)
            {
                appwindow = ShowApplication(app, "");
            }
            return appwindow;
        }

        private Window ShowApplication(object child, string applicationName)
        {
            if (child is IHostedApplication5)
            {
                if (((IHostedApplication5)child).TopLevelWpfWindow.Parent != null)
                {
                    Window w = ((IHostedApplication5)child).TopLevelWpfWindow.Parent as Window;
                    if (w != null)
                        w.Show();
                    floatingWindows.Add(w);
                    return w;
                }
                return (Window)ShowApplicationOnUI(child, ((IHostedApplication5)child).TopLevelWpfWindow, applicationName);
            }
            else if (child is IHostedApplication)
            {
                IHostedApplication application3 = child as IHostedApplication;
                if (application3.TopLevelWindow != null)
                {
                    AifWindowsFormsHost uiElement = new AifWindowsFormsHost(application3.TopLevelWindow);
                    application3.TopLevelWindow.Tag = uiElement;
                    return (Window)ShowApplicationOnUI(child, uiElement, applicationName);
                }
            }
            else if (child is System.Windows.Forms.Control)
            {
                System.Windows.Forms.Control c = child as System.Windows.Forms.Control;
                AifWindowsFormsHost host2 = new AifWindowsFormsHost(c);
                c.Tag = host2;
                return (Window)ShowApplicationOnUI(child, host2, applicationName);
            }

            return null;
        }

        Object ShowApplicationOnUI(Object tabTag, UIElement child, string applicationName)
        {
            VerifyGlobalManagerConnection();
            Window appwindow = new Window();
            appwindow.Style = (System.Windows.Style)FindResource("FloatingWindow");
            appwindow.Resources.MergedDictionaries.Clear();
            foreach (ResourceDictionary dict in ((System.Windows.Application)System.Windows.Application.Current).MainWindow.Resources.MergedDictionaries)
            {
                appwindow.Resources.MergedDictionaries.Add(dict);
            }
            appwindow.Content = child;
            appwindow.Title = applicationName;
            //appwindow.Owner = ((System.Windows.Application)System.Windows.Application.Current).MainWindow;
            appwindow.Tag = tabTag;
            appwindow.Closed += new EventHandler(appwindow_Closed);
            floatingWindows.Add(appwindow);
            Entity appEntity = CRMWindowRouter.LoadApplicationEntity(applicationName);
            appwindow.Show();
            if (appEntity != null)
            {
                if (appEntity.Contains("uii_isappdynamic")
                  && (bool)appEntity["uii_isappdynamic"] == false)
                {
                    HwndSource hwndSource = PresentationSource.FromVisual(appwindow) as HwndSource;
                    if (hwndSource != null)
                    {
                        IntPtr hMenu = GetSystemMenu(hwndSource.Handle, false);
                        if (hMenu != IntPtr.Zero)
                        {
                            EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                        }
                        hwndSource.AddHook(new HwndSourceHook(this.hwndSourceHook));
                    }

                }
            }
            return appwindow;
        }

        public void PropogateResources()
        {
            foreach (Window w in floatingWindows)
            {
                w.Resources.MergedDictionaries.Clear();
                foreach (ResourceDictionary dict in ((System.Windows.Application)System.Windows.Application.Current).MainWindow.Resources.MergedDictionaries)
                {
                    w.Resources.MergedDictionaries.Add(dict);
                }
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);
        const uint MF_BYCOMMAND = 0x00000000;
        const uint MF_GRAYED = 0x00000001;
        const uint SC_CLOSE = 0xF060;
        const int WM_SHOWWINDOW = 0x00000018;
        const int WM_CLOSE = 0x10;

        IntPtr hwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SHOWWINDOW)
            {
                IntPtr hMenu = GetSystemMenu(hwnd, false);
                if (hMenu != IntPtr.Zero)
                {
                    EnableMenuItem(hMenu, SC_CLOSE, MF_BYCOMMAND | MF_GRAYED);
                }
            }
            else if (msg == WM_CLOSE)
            {
                handled = true;
            }
            return IntPtr.Zero;
        }

        void appwindow_Closed(object sender, EventArgs e)
        {
            if (((Window)sender).Tag != null)
            {
                ((Window)sender).Content = null;
                CloseAppClickHandler((IHostedApplication)((Window)sender).Tag);
            }
        }
        #endregion

        #region Misc
        bool VerifyGlobalManagerConnection()
        {
            if (CRMWindowRouter == null)
            {
                CRMWindowRouter = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
            }
            if (SessionManager == null)
            {
                SessionManager = AifServiceContainer.Instance.GetService<AgentDesktopSessions>();
                SessionManager.SessionCloseEvent += SessionManager_SessionCloseEvent;
            }
            if (CRMWindowRouter == null)
                return false;
            return true;
        }

        private static BitmapSource BitmapSourceFromImage(System.Drawing.Image img)
        {
            MemoryStream stream = new MemoryStream();
            img.Save(stream, ImageFormat.Png);
            PngBitmapDecoder decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        #endregion

        #region ICCDPanel
        public void NotifySessionHiding(Session session)
        {
        }

        public void NotifySessionHidden(Session session)
        {
        }

        public void NotifySessionShowing(Session session)
        {
        }

        public void NotifySessionShown(Session session)
        {
        }

        public IHostedApplication GetAppById(Guid id)
        {
            IHostedApplication tag;
            foreach (Window appwindow in floatingWindows)
            {
                tag = appwindow.Tag as IHostedApplication;
                if ((tag != null) && (tag.ApplicationID == id))
                {
                    return tag;
                }
            }
            return null;
        }

        public IHostedApplication GetVisibleApplication()
        {
            foreach (Window ti in floatingWindows)
            {
                if (ti.IsFocused == true
                    && ti.Tag is Microsoft.Uii.Csr.IHostedApplication)
                    return ((Microsoft.Uii.Csr.IHostedApplication)ti.Tag);
            }
            return null;
        }

        public void NotifyContextChange(Context context)
        {
            if (!VerifyGlobalManagerConnection())
                return;

            Dispatcher.BeginInvoke(new System.Action(() =>
            {

                foreach (Window item in this.floatingWindows)
                {
                    IHostedApplication tag = item.Tag as IHostedApplication;
                    if (tag != null)
                    {
                        string applicationName = Utility.GetApplicationDisplayName(tag);
                        item.Title = applicationName;
                    }
                }
            }));
        }

        public void SessionChanged(Session session)
        {
            // TODO
            //try
            //{
            //    ICRMWindowRouter globalMgr = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
            //    AgentDesktopSessions sessions = globalMgr.GetSessionManager();
            //    string activeApp = ((DynamicsCustomerRecord)((AgentDesktopSession)sessions.ActiveSession).Customer.DesktopCustomer).GetActiveAppOnPanel(this._panelName);
            //    IHostedApplication tabApp;
            //    foreach (TabItem item in Items)
            //    {
            //        tabApp = item.Tag as IHostedApplication;
            //        if ((tabApp != null) && (tabApp.ApplicationName == activeApp))
            //        {
            //            SelectedItem = item;
            //            return;
            //        }
            //    }
            //}
            //catch
            //{
            //}
        }

        public int GetControlCount()
        {
            if (floatingWindows.Count > 0)
            {
                return floatingWindows.Count;
            }
            return 0;
        }

        public void SetActiveApplication(IHostedApplication app)
        {
            IHostedApplication tabApp;
            foreach (Window item in floatingWindows)
            {
                tabApp = item.Tag as IHostedApplication;
                if ((tabApp != null) && (tabApp.ApplicationID == app.ApplicationID))
                {
                    item.Activate();
                    return;
                }
            }
        }

        public List<IHostedApplication> ApplicationList
        {
            get
            {
                List<IHostedApplication> apps = new List<IHostedApplication>();
                foreach (Window item in floatingWindows)
                {
                    IHostedApplication app = item.Tag as IHostedApplication;
                    if (app != null)
                    {
                        apps.Add(app);
                    }
                }
                return apps;
            }
        }

        #endregion

        public void CloseApplications()
        {
            List<IHostedApplication> tabApps = new List<IHostedApplication>();

            if (floatingWindows.Count > 0)
            {
                foreach (Window item in floatingWindows)
                {
                    IHostedApplication tabApp = item.Tag as IHostedApplication;
                    if (tabApp != null)
                    {
                        IDesktopUserActions desktop = AifServiceContainer.Instance.GetService<IDesktopUserActions>();
                        if (desktop != null)
                            desktop.CloseDynamicApplication(tabApp.ApplicationName);
                    }
                }
                floatingWindows.Clear();
            }
        }

        public void SendApplicationsToUnknownPanel()
        {
            List<IHostedApplication> tabApps = new List<IHostedApplication>();

            if (floatingWindows.Count > 0)
            {
                foreach (Window item in floatingWindows)
                {
                    IHostedApplication tabApp = item.Tag as IHostedApplication;
                    if (tabApp != null)
                    {
                        IDesktopFeatureAccess desktop = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
                        if (desktop != null)
                            desktop.SendApplicationToUnknownPanel(this, tabApp);
                    }
                }
                floatingWindows.Clear();
            }
        }
    }
}
