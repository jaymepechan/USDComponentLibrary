/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Uii.Csr;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for NotifyIcon.xaml
    /// </summary>
    public partial class NotifyIconControl : MicrosoftBase
    {
        NotifyIcon notifyIcon = null;
        System.Windows.Forms.ContextMenu contextMenu = null;

        public NotifyIconControl()
        {
            contextMenu = new System.Windows.Forms.ContextMenu();
        }

        public NotifyIconControl(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            contextMenu = new System.Windows.Forms.ContextMenu();
        }

        protected override void DesktopReady()
        {
            base.DesktopReady();

            RegisterAction("ShowTrayIcon", ShowTrayIcon);
            RegisterAction("HideTrayIcon", HideTrayIcon);
            RegisterAction("HideApplication", HideApplication);
            RegisterAction("ShowApplication", ShowApplication);
            RegisterAction("AddMenuItem", AddMenuItem);
            RegisterAction("ResetMenu", ResetMenu);
            RegisterAction("ExitApplication", ExitApplication);

            System.Windows.Application.Current.MainWindow.WindowStyle = WindowStyle.None;
        }

        public override void Close()
        {
            base.Close();
            HideTrayIcon(null);
        }

        private void ShowApplication(RequestActionEventArgs args)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
            System.Windows.Application.Current.MainWindow.ShowInTaskbar = true;
        }

        private void HideApplication(RequestActionEventArgs args)
        {
            System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
            System.Windows.Application.Current.MainWindow.ShowInTaskbar = false;
        }

        private void HideTrayIcon(RequestActionEventArgs args)
        {
            if (notifyIcon == null)
                return;
            notifyIcon.Visible = false;
        }

        void ExitApplication(Uii.Csr.RequestActionEventArgs args)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ResetMenu(RequestActionEventArgs args)
        {
            contextMenu = new System.Windows.Forms.ContextMenu();
        }

        private void AddMenuItem(RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> argList = Utility.SplitLines(args.Data, CurrentContext, localSession);
            foreach (KeyValuePair<string, string> arg in argList)
            {
                System.Windows.Forms.MenuItem mi = new System.Windows.Forms.MenuItem();
                mi.Text = arg.Key;
                mi.Tag = arg.Value;
                mi.Click += Mi_Click;
                contextMenu.MenuItems.Add(mi);
            }
            
        }

        private void Mi_Click(object sender, EventArgs e)
        {
            if (!(sender is System.Windows.Forms.MenuItem))
                return;
            FireEvent(((System.Windows.Forms.MenuItem)sender).Tag as string);
        }

        private void ShowTrayIcon(RequestActionEventArgs args)
        {
            if (notifyIcon == null)
            {
                notifyIcon = new NotifyIcon();
                Stream s = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(@"Microsoft.USD.ComponentLibrary.Resources.icoCRM.ico");
                notifyIcon.Icon = new System.Drawing.Icon(s);
                notifyIcon.ContextMenu = contextMenu;
                notifyIcon.DoubleClick += NotifyIcon_DoubleClick;
            }
            notifyIcon.Visible = true;
        }

        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            FireEvent("DoubleClick");
        }
    }
}
