/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.BaseControl;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.PanelLayouts;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Csr.Aif.HostedApplication;
using Microsoft.Uii.Desktop.Core;
using Microsoft.Uii.Desktop.UI.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary
{
    public class PopupPanel : Popup, IPanel, IUSDPanel
    {
        public event CloseApplicationClickEventHandler CloseApplicationClick;
        public event SelectedAppChangedEventHandler SelectedAppChanged;
        private ICRMWindowRouter CRMWindowRouter;
        private string _panelName = String.Empty;

        public PopupPanel()
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

            return ShowApplication(child, String.Empty, closeButton);
        }

        private ContentPresenter ShowApplication(object child, string applicationName, bool closeButton)
        {
            if (child is IHostedApplication5)
            {
                if (((IHostedApplication5)child).TopLevelWpfWindow.Parent != null)
                    return null;
                return (ContentPresenter)ShowApplicationOnUI(child, ((IHostedApplication5)child).TopLevelWpfWindow, applicationName, closeButton);
            }
            else if (child is IHostedApplication)
            {
                IHostedApplication application3 = child as IHostedApplication;
                if (application3.TopLevelWindow != null)
                {
                    AifWindowsFormsHost uiElement = new AifWindowsFormsHost(application3.TopLevelWindow);
                    application3.TopLevelWindow.Tag = uiElement;
                    return (ContentPresenter)ShowApplicationOnUI(child, uiElement, applicationName, closeButton);
                }
            }
            else if (child is System.Windows.Forms.Control)
            {
                System.Windows.Forms.Control c = child as System.Windows.Forms.Control;
                AifWindowsFormsHost host2 = new AifWindowsFormsHost(c);
                c.Tag = host2;
                return (ContentPresenter)ShowApplicationOnUI(child, host2, applicationName, closeButton);
            }

            return null;
        }

        ContentPresenter _contentPresenter = null;
        ContentPresenter ContentContainer
        {
            get
            {
                if (_contentPresenter == null)
                    _contentPresenter = DiscoverContentControl(this.Child);
                return _contentPresenter;
            }
            set
            {
                if (value == null && _contentPresenter != null)
                {
                    _contentPresenter.Content = value;
                    _contentPresenter.Tag = null;
                }
            }
        }

        protected ContentPresenter DiscoverContentControl(UIElement child)
        {
            FrameworkElement fe = child as FrameworkElement;
            if (fe != null)
            {
                if (fe is ContentPresenter)
                    return fe as ContentPresenter;

                try { fe.ApplyTemplate(); }
                catch { }
                int childrenCount = VisualTreeHelper.GetChildrenCount(fe);
                for (int i = 0; i < childrenCount; i++)
                {
                    DependencyObject child1 = VisualTreeHelper.GetChild(child, i);
                    ContentPresenter cp = DiscoverContentControl(child1 as UIElement);
                    if (cp != null)
                        return cp;
                }
            }
            ContentControl c = child as ContentControl;
            if (c != null)
            {
                ContentPresenter cp = DiscoverContentControl(c.Content as UIElement);
                if (cp != null)
                    return cp;
            }
            ItemsControl ic = child as ItemsControl;
            if (ic != null)
            {
                foreach (Object elem in ic.Items)
                {
                    if (elem is UIElement)
                    {
                        ContentPresenter cp = DiscoverContentControl(elem as UIElement);
                        if (cp != null)
                            return cp;
                    }
                }
            }
            return null;
        }

        Object ShowApplicationOnUI(Object tabTag, UIElement childControl, string applicationName, bool closeButton)
        {
            try
            {
                ContentContainer.Tag = tabTag;
                ContentContainer.Content = childControl;
                IsOpen = true;
                return ContentContainer;
            }
            finally
            {
            }
        }

        public object Add(object child, bool closeButton)
        {
            return this.Add(child, null, false, closeButton);
        }

        public bool Floating
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public bool IsApplicationOnPanel(Guid id)
        {
            if (ContentContainer != null)
            {
                IHostedApplication application = ContentContainer.Tag as IHostedApplication;
                if ((application != null) && (application.ApplicationID == id))
                {
                    IsOpen = true;
                    return true;
                }
            }
            return false;
        }

        public bool Remove(object app)
        {
            IHostedApplication application = ContentContainer.Tag as IHostedApplication;
            if (app == application)
            {
                ContentContainer = null;
                IsOpen = false;
                return true;
            }
            return false;
        }

        public List<Uii.Csr.IHostedApplication> ApplicationList
        {
            get
            {
                List<Uii.Csr.IHostedApplication> list = new List<IHostedApplication>();
                if (this.Child != null)
                {
                    IHostedApplication application = ContentContainer.Tag as IHostedApplication;
                    list.Add(application);
                }
                return list;
            }
        }

        public Uii.Csr.IHostedApplication GetAppById(Guid id)
        {
            ContentPresenter appContainer = ContentContainer as ContentPresenter;
            if (appContainer != null && appContainer.Tag is Microsoft.Uii.Csr.IHostedApplication
                && ((Microsoft.Uii.Csr.IHostedApplication)appContainer.Tag).ApplicationID == id)
                return ((Microsoft.Uii.Csr.IHostedApplication)appContainer.Tag);
            return null;
        }

        public int GetControlCount()
        {
            ContentPresenter appContainer = ContentContainer as ContentPresenter;
            if (appContainer == null)
                return 0;
            return 1;
        }

        public Uii.Csr.IHostedApplication GetVisibleApplication()
        {
            ContentPresenter cc = ContentContainer as ContentPresenter;
            if (cc != null && cc.Tag is Microsoft.Uii.Csr.IHostedApplication)
                return ((Microsoft.Uii.Csr.IHostedApplication)cc.Tag);
            return null;
        }

        public void NotifyContextChange(Uii.Csr.Context context)
        {
        }

        public void NotifySessionHidden(Uii.Csr.Session session)
        {
        }

        public void NotifySessionHiding(Uii.Csr.Session session)
        {
        }

        public void NotifySessionShowing(Uii.Csr.Session session)
        {
        }

        public void NotifySessionShown(Uii.Csr.Session session)
        {
        }

        public void PropogateResources()
        {

        }

        public void SendApplicationsToUnknownPanel()
        {
            List<IHostedApplication> tabApps = new List<IHostedApplication>();

            if (this.Child != null)
            {
                ContentPresenter item = ContentContainer as ContentPresenter;
                IHostedApplication application = item.Tag as IHostedApplication;
                if (application != null)
                {
                    IDesktopFeatureAccess desktop = AifServiceContainer.Instance.GetService<IDesktopFeatureAccess>();
                    if (desktop != null)
                        desktop.SendApplicationToUnknownPanel(this, application);
                }
                ContentContainer = null;
            }
        }

        public void SessionChanged(Uii.Csr.Session session)
        {
        }

        public void SetActiveApplication(Uii.Csr.IHostedApplication app)
        {
        }

        public void CloseApplications()
        {
            if (this.Child != null)
            {
                ContentPresenter item = ContentContainer as ContentPresenter;
                IHostedApplication application = item.Tag as IHostedApplication;
                if (application != null)
                {
                    IDesktopUserActions desktop = AifServiceContainer.Instance.GetService<IDesktopUserActions>();
                    if (desktop != null)
                        desktop.CloseDynamicApplication(application.ApplicationName);
                }
                ContentContainer = null;
            }
        }

    }
}
