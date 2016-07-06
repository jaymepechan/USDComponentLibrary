using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using C1.WPF;
using C1.WPF.Docking;
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility.DataLoader;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Csr.Aif.HostedApplication;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.Desktop.UI.Controls;
using Microsoft.Uii.Desktop.UI.Controls.Wpf;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    public class C1USDTabBasePanel : C1USDDockTabControl, IPanel, IUSDPanel
    {
        // Fields
        private CloseApplicationClickEventHandler CloseApplicationClickEvent;
        private SelectedAppChangedEventHandler SelectedAppChangedEvent;
        private ICRMWindowRouter CRMWindowRouter;
        private IDesktopFeatureAccess desktopFeatureAccess;
        private string _panelName = String.Empty;

        #region Initialization
        public C1USDTabBasePanel()
        {   
            this.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(USDTabBasePanel_MouseDoubleClick);
            desktopFeatureAccess = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
            if (desktopFeatureAccess != null)
                desktopFeatureAccess.AddPanel(this);                  
        }

        void USDTabBasePanel_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
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
            IHostedApplication app = child as IHostedApplication;
            return this.ShowTabControl(child);
        }
        
        [Browsable(false)]
        public bool Floating { get { return false; }
            set { }
        }

        public bool IsApplicationOnPanel(Guid id)
        {
            IHostedApplication application;
            foreach (object obj2 in this.Items)
            {
                application = ((C1USDDockTabItem)obj2).Tag as IHostedApplication;
                if ((application != null) && (application.ApplicationID == id))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Remove(object app)
        {
            return this.RemoveAppFromTabControl(app);
        }

        // Events
        public event CloseApplicationClickEventHandler CloseApplicationClick
        {
            add
            {
                CloseApplicationClickEventHandler handler2;
                CloseApplicationClickEventHandler closeApplicationClick = this.CloseApplicationClickEvent;
                do
                {
                    handler2 = closeApplicationClick;
                    CloseApplicationClickEventHandler handler3 = (CloseApplicationClickEventHandler)Delegate.Combine(handler2, value);
                    closeApplicationClick = Interlocked.CompareExchange<CloseApplicationClickEventHandler>(ref this.CloseApplicationClickEvent, handler3, handler2);
                }
                while (closeApplicationClick != handler2);
            }
            remove
            {
                CloseApplicationClickEventHandler handler2;
                CloseApplicationClickEventHandler closeApplicationClick = this.CloseApplicationClickEvent;
                do
                {
                    handler2 = closeApplicationClick;
                    CloseApplicationClickEventHandler handler3 = (CloseApplicationClickEventHandler)Delegate.Remove(handler2, value);
                    closeApplicationClick = Interlocked.CompareExchange<CloseApplicationClickEventHandler>(ref this.CloseApplicationClickEvent, handler3, handler2);
                }
                while (closeApplicationClick != handler2);
            }
        }

        public event SelectedAppChangedEventHandler SelectedAppChanged
        {
            add
            {
                SelectedAppChangedEventHandler handler2;
                SelectedAppChangedEventHandler selectedAppChanged = this.SelectedAppChangedEvent;
                do
                {
                    handler2 = selectedAppChanged;
                    SelectedAppChangedEventHandler handler3 = (SelectedAppChangedEventHandler)Delegate.Combine(handler2, value);
                    selectedAppChanged = Interlocked.CompareExchange<SelectedAppChangedEventHandler>(ref this.SelectedAppChangedEvent, handler3, handler2);
                }
                while (selectedAppChanged != handler2);
            }
            remove
            {
                SelectedAppChangedEventHandler handler2;
                SelectedAppChangedEventHandler selectedAppChanged = this.SelectedAppChangedEvent;
                do
                {
                    handler2 = selectedAppChanged;
                    SelectedAppChangedEventHandler handler3 = (SelectedAppChangedEventHandler)Delegate.Remove(handler2, value);
                    selectedAppChanged = Interlocked.CompareExchange<SelectedAppChangedEventHandler>(ref this.SelectedAppChangedEvent, handler3, handler2);
                }
                while (selectedAppChanged != handler2);
            }
        }

        protected void CloseAppClickHandler(IHostedApplication app)
        {
            if (this.CloseApplicationClickEvent != null)
            {
                this.CloseApplicationClickEvent(app);
            }
        }

        private void CloseButtonClickHandler(object application)
        {
            this.CloseAppClickHandler(application as IHostedApplication);
        }

        #endregion

        #region IPanel Entry Functions
        bool eventsHooked = false;
        private object ShowTabControl(object app)
        {
            this.SetValue(UIElement.VisibilityProperty, Visibility.Visible);
            if (eventsHooked == false)
            {
                this.SelectionChanged += new SelectionChangedEventHandler(this.TabControl_SelectionChanged);
                this.GotFocus += new RoutedEventHandler(USDTabBasePanel_GotFocus);
                eventsHooked = true;
            }
            return this.AddApplicationToTabControl(app);
        }

        bool HaltSelectionLogic = false;
        private bool RemoveAppFromTabControl(object app)
        {
            try
            {
                foreach (C1USDDockTabItem obj2 in this.Items)
                {
                    if (obj2.Tag != app)
                    {
                        continue;
                    }

                    this.Items.Remove(obj2);
                    object c = obj2.Content;
                    obj2.Content = null;
                    if (this.Items.Count == 0 && this.DockMode == DockMode.Floating)
                    {
                        this.DockMode = DockMode.Hidden;
                    }
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Panel Manipulation

        private C1USDDockTabItem AddApplicationToTabControl(object app)
        {
            //TabItem element = null;

            C1USDDockTabItem element = null;
            Image icon = new Image();
            IHostedApplication application = app as IHostedApplication; 
            if (application != null)
            {
                string applicationName = application.ApplicationName;
                if (VerifyGlobalManagerConnection())
                {
                    Entity thisApplication = CRMWindowRouter.LoadApplicationEntity(applicationName);
                    try
                    {
                        if (thisApplication != null && thisApplication.Contains("mcs_displayname"))
                        {
                            string appNameTemp = CRMWindowRouter.ReplaceParametersInCurrentSession((string)thisApplication["mcs_displayname"]);
                            if (!String.IsNullOrEmpty(appNameTemp) && Utility.IsAllReplacementValuesReplaced(appNameTemp))
                                applicationName = appNameTemp;
                        }
                    }
                    catch { }
                }
                element = ShowApplication(app, applicationName);
                if (application == DesktopApplicationUIBase.AppWithFocus)
                {
                    this.SelectedItem = element;
                }
            }
            else if (app is System.Windows.Forms.Control)
            {
                element = ShowApplication(app, "");
                AutomationProperties.SetName(element, app.ToString() + " Tab Page");
            }
            return element;
        }

        private C1USDDockTabItem ShowApplication(object child, string applicationName)
        {
            if (child is IHostedApplication5)
            {
                if (((IHostedApplication5)child).TopLevelWpfWindow.Parent != null)
                    return null;
                return (C1USDDockTabItem)ShowApplicationOnUI(child, ((IHostedApplication5)child).TopLevelWpfWindow, applicationName);
            }
            else if (child is IHostedApplication)
            {
                IHostedApplication application3 = child as IHostedApplication;
                if (application3.TopLevelWindow != null)
                {
                    AifWindowsFormsHost uiElement = new AifWindowsFormsHost(application3.TopLevelWindow);
                    application3.TopLevelWindow.Tag = uiElement;
                    return (C1USDDockTabItem)ShowApplicationOnUI(child, uiElement, applicationName);
                }
            }
            else if (child is System.Windows.Forms.Control)
            {
                System.Windows.Forms.Control c = child as System.Windows.Forms.Control;
                AifWindowsFormsHost host2 = new AifWindowsFormsHost(c);
                c.Tag = host2;
                return (C1USDDockTabItem)ShowApplicationOnUI(child, host2, applicationName);
            }

            return null;
        }

        Object ShowApplicationOnUI(Object tabTag, UIElement child, string applicationName)
        {
            try
            {
                var controltest = (DynamicsBaseHostedControl) tabTag;
                if (this.DockMode == DockMode.Hidden)
                {
                    this.DockMode = DockMode.Floating;
                }
                VerifyGlobalManagerConnection();
                C1USDDockTabItem ti = new C1USDDockTabItem();
                ti.Tag = tabTag;
                ti.Content = child;
                child.Visibility = System.Windows.Visibility.Visible;
                ti.Header = applicationName;
                Entity thisApplication = CRMWindowRouter.LoadApplicationEntity(applicationName);
                try
                {
                    if (thisApplication != null && thisApplication.Contains("uii_usercanclose"))
                    {
                        var canClose = thisApplication.Attributes["uii_usercanclose"];
                        if ((bool) canClose)
                        {
                            ti.CanUserClose = true;
                        }
                    }
                    if (thisApplication != null && thisApplication.Contains("uii_sortorder"))
                    {
                        for (int i = 0; i < Items.Count; i++)
                        {
                            if (!(((C1USDDockTabItem) Items[i]).Tag is IHostedApplication))
                                continue;
                            string appname =
                                (((C1USDDockTabItem) Items[i]).Tag as IHostedApplication).ApplicationName;
                            Entity appconfig = CRMWindowRouter.LoadApplicationEntity(appname);
                            if (appconfig != null && appconfig.Contains("uii_sortorder"))
                            {
                                if ((int) thisApplication["uii_sortorder"] < (int) appconfig["uii_sortorder"])
                                {
                                    Items.Insert(i, ti);
                                    return ti;
                                }
                            }
                            else
                            {
                                Items.Insert(i, ti);
                                return ti;
                            }
                        }
                    }
                }
                catch
                {
                }
                    //Items.Insert

                try
                {
                    Items.Add(ti);
                }
                catch
                {
                }

                if (Items.Count == 1)
                    SelectedItem = ti;
                return ti;
            }
            finally
            {
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

        void USDTabBasePanel_GotFocus(object sender, RoutedEventArgs e)
        {
            if ((this.SelectedItem != null) && ((this.SelectedItem as C1USDDockTabItem).Tag is IHostedApplication))
            {
                if (DesktopApplicationUIBase.AppWithFocus == (this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication)
                    return;
                DesktopApplicationUIBase.AppWithFocus = (this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication;

                try
                {
                    if (HaltSelectionLogic == false)
                    {
                        CRMGlobalManager.SetAppWithFocus((this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication, this, this._panelName);
                    }
                }
                catch
                {
                }
            }
            WpfDesktopApplicationUI.ActivePanel = this.Parent as WpfPanel;
            if (this.SelectedAppChangedEvent != null)
            {
                this.SelectedAppChangedEvent(sender, e);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 1 && e.AddedItems[0] != SelectedItem)
                return;
            if ((this.SelectedItem != null) && ((this.SelectedItem as C1USDDockTabItem).Tag is IHostedApplication))
            {
                if (DesktopApplicationUIBase.AppWithFocus == (this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication)
                    return;
                DesktopApplicationUIBase.AppWithFocus = (this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication;

                try
                {
                    if (HaltSelectionLogic == false)
                    {
                        CRMGlobalManager.SetAppWithFocus((this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication, this, this._panelName);
                    }
                }
                catch
                {
                }
            }
            WpfDesktopApplicationUI.ActivePanel = this.Parent as WpfPanel;
            if (this.SelectedAppChangedEvent != null)
            {
                this.SelectedAppChangedEvent(sender, e);
            }


        }

        #endregion

        #region IUSDPanel

        public void CloseApplications()
        {
            //throw new NotImplementedException();
        }

        public void PropogateResources()
        {

        }

        public IHostedApplication GetAppById(Guid id)
        {
            IHostedApplication tag;
            foreach (C1USDDockTabItem item in Items)
            {
                tag = item.Tag as IHostedApplication;
                if ((tag != null) && (tag.ApplicationID == id))
                {
                    return tag;
                }
            }
            return null;
        }

        public IHostedApplication GetVisibleApplication()
        {
            foreach (C1USDDockTabItem ti in Items)
            {
                if (ti.IsSelected == true
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
                foreach (C1USDDockTabItem item in this.Items)
                {
                    IHostedApplication tag = item.Tag as IHostedApplication;
                    if (tag != null)
                    {
                        string applicationName = tag.ApplicationName;
                        Entity thisApplication = CRMWindowRouter.LoadApplicationEntity(applicationName);
                        try
                        {
                            if (thisApplication != null && thisApplication.Contains("mcs_displayname"))
                            {
                                string appNameTemp = CRMWindowRouter.ReplaceParametersInCurrentSession((string)thisApplication["mcs_displayname"]);
                                if (!String.IsNullOrEmpty(appNameTemp) && Utility.IsAllReplacementValuesReplaced(appNameTemp))
                                    applicationName = appNameTemp;
                            }
                        }
                        catch { }
                        if (item.Header is Grid)
                        {
                            foreach (UIElement c in ((Grid)item.Header).Children)
                            {
                                if (c is System.Windows.Controls.Label)
                                {
                                    ((System.Windows.Controls.Label)c).Content = applicationName;
                                }
                            }
                        }
                        else if (item.Header is string)
                        {
                            item.Header = applicationName;
                        }
                        else if (item.Header is System.Windows.Controls.Label)
                        {
                            ((System.Windows.Controls.Label)item.Header).Content = applicationName;
                        }
                    }
                }
            }));
        }

        public void NotifySessionHiding(Session session)
        {
            HaltSelectionLogic = true;
        }

        public void NotifySessionHidden(Session session)
        {
            HaltSelectionLogic = false;
        }

        public void NotifySessionShowing(Session session)
        {
            HaltSelectionLogic = true;
        }

        public void NotifySessionShown(Session session)
        {
            HaltSelectionLogic = false;
        }

        public void SessionChanged(Session session)
        {
            try
            {
                ICRMWindowRouter globalMgr = AifServiceContainer.Instance.GetService<ICRMWindowRouter>();
                AgentDesktopSessions sessions = globalMgr.GetSessionManager();
                string activeApp = ((DynamicsCustomerRecord)((AgentDesktopSession)sessions.ActiveSession).Customer.DesktopCustomer).GetActiveAppOnPanel(this._panelName);
                Debug.WriteLine("Restoring App " + activeApp + " for panel " + this._panelName);
                if (activeApp != null)
                {
                    IHostedApplication tabApp;
                    foreach (C1USDDockTabItem item in Items)
                    {
                        tabApp = item.Tag as IHostedApplication;
                        if ((tabApp != null) && (tabApp.ApplicationName == activeApp))
                        {
                            SelectedItem = item;
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public int GetControlCount()
        {
            if (Items.Count > 0)
            {
                return Items.Count;
            }
            return 0;
        }

        public void SetActiveApplication(IHostedApplication app)
        {
            IHostedApplication tabApp;
            foreach (C1USDDockTabItem item in Items)
            {
                tabApp = item.Tag as IHostedApplication;
                if ((tabApp != null) && (tabApp.ApplicationID == app.ApplicationID))
                {
                    SelectedItem = item;
                    try
                    {
                        CRMGlobalManager.SetAppWithFocus((this.SelectedItem as C1USDDockTabItem).Tag as IHostedApplication, this, this._panelName);
                    }
                    catch
                    {
                    }
                    WpfDesktopApplicationUI.ActivePanel = this.Parent as WpfPanel;
                    if (this.SelectedAppChangedEvent != null)
                    {
                        this.SelectedAppChangedEvent(null, null);
                    }
                    return;
                }
            }
        }

        public List<IHostedApplication> ApplicationList
        {
            get
            {
                List<IHostedApplication> tabApps = new List<IHostedApplication>();
                foreach (C1USDDockTabItem item in Items)
                {
                    IHostedApplication tabApp = item.Tag as IHostedApplication;
                    if (tabApp != null)
                    {
                        tabApps.Add(tabApp);
                    }
                }
                return tabApps;
            }
        }

        #endregion

        public void SendApplicationsToUnknownPanel()
        {
            try
            {
                List<IHostedApplication> tabApps = new List<IHostedApplication>();

                while (Items.Count > 0)
                {
                    C1USDDockTabItem item = (C1USDDockTabItem)Items[0];
                    IHostedApplication tabApp = item.Tag as IHostedApplication;
                    if (tabApp != null)
                    {
                        IDesktopFeatureAccess desktop = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
                        if (desktop != null)
                            desktop.SendApplicationToUnknownPanel(this, tabApp);
                    }
                    if (Items.Count > 0)
                        Items.RemoveAt(0);
                }
            }
            catch
            {
            }
        }
    }
}
