/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
// =====================================================================
//  This file is part of the Microsoft Dynamics CRM SDK code samples.
//
//  Copyright (C) Microsoft Corporation.  All rights reserved.
//
//  This source code is intended only as a supplement to Microsoft
//  Development Tools and/or on-line documentation.  See these other
//  materials for detailed information regarding Microsoft code samples.
//
//  THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
//  PARTICULAR PURPOSE.
// =====================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics.Utilities;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.Csr.CitrixIntegration;
using Microsoft.Uii.Csr;
using System.Threading;
using System.Xml;
using System.Windows.Forms;

namespace Microsoft.USD.ComponentLibrary.Adapters.Citrix
{
    /// <summary>
    /// See UII API documentation for full API Information available regarding Citrix integration requirements.
    /// Especially look at the Readme.html in the UII/Citrix Server Component folder
    /// </summary>
    public class CitrixPortalApplicationHostedControl : CitrixApplicationHostedControl
    {
        #region Vars
        /// <summary>
        /// Log writer for USD 
        /// </summary>
        private TraceLogger LogWriter = null;
        System.Threading.Timer tAttach = null;
        string _initString;
        System.Windows.Forms.WebBrowser webBrowser1;

        #endregion

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public CitrixPortalApplicationHostedControl(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            // This will create a log writer with the default provider for Unified Service desk
            LogWriter = new TraceLogger();

            _initString = initString;
            CheckForVirtualChannels();
            FirstHeartbeatReceived += delegate
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new WaitCallback(DoDefaultAction));
            };

        }

        void CheckForVirtualChannels()
        {
            try
            {
                Microsoft.Win32.RegistryKey r = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Citrix\ICA Client\Engine\Lockdown Profiles\All Regions\Lockdown\Virtual Channels\Third Party\CustomVC");
                if (r == null || r.GetValue("VirtualChannels") == null)
                {
                    Microsoft.Win32.Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Citrix\ICA Client\Engine\Lockdown Profiles\All Regions\Lockdown\Virtual Channels\Third Party\CustomVC", "VirtualChannels", "");
                }
            }
            catch
            {
                // we may not be able to access the registry on the user machine
            }
        }

        void DoDefaultAction(object obj)
        {
            System.Threading.Thread.Sleep(2000); // wait a little bit for the server to be ready to handle the default action
            DoDefaultAction();
        }

        System.Threading.Timer tInitializeDelay;
        // This value should match the href of the link to auto-select from the citrix portal
        // This URL could be pushed to the launcher frame after login if yuo append the timestamp
        string autoSelect = "";
        public override void Initialize()
        {
            XmlDocument docInitString = new XmlDocument();
            docInitString.LoadXml(_initString);
            XmlNode nodePortalUrl = docInitString.SelectSingleNode("initstring/CitrixIntegration/portalurl");
            if (nodePortalUrl == null || String.IsNullOrEmpty(nodePortalUrl.InnerText))
            {
                // connect using a configured ICA file
                tInitializeDelay = new System.Threading.Timer(new TimerCallback(InitializeDelay), null, TimeSpan.FromSeconds(10), TimeSpan.Zero);
            }
            else
            {
                XmlNode nodeAutoSelect = docInitString.SelectSingleNode("initstring/CitrixIntegration/autoselecturl");
                if (nodeAutoSelect != null)
                    autoSelect = nodeAutoSelect.InnerText;
                // connect using the Citrix web portal
                ICAClient.Visible = false;
                webBrowser1 = new System.Windows.Forms.WebBrowser();
                this.Controls.Add(webBrowser1);
                webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
                webBrowser1.Visible = true;
                webBrowser1.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(webBrowser1_Navigating);
                webBrowser1.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(webBrowser1_DocumentCompleted);
                webBrowser1.Navigate(nodePortalUrl.InnerText);

                InitializeEx();
            }
        }

        public void InitializeEx()
        {
            int num = 0x7fffffff;
            this.AddAction(--num, "__FindControl__", "<ActionInit/>");
            this.AddAction(--num, "__GetControlValue__", "<ActionInit/>");
            this.AddAction(--num, "__SetControlValue__", "<ActionInit/>");
            this.AddAction(--num, "__ExecuteControlAction__", "<ActionInit/>");
            this.AddAction(--num, "__AddDoActionEventTrigger__", "<ActionInit/>");
            this.AddAction(--num, "__RemoveDoActionEventTrigger__", "<ActionInit/>");
            XmlDocument docInitString = new XmlDocument();
            docInitString.LoadXml(_initString);
            XmlNode citrixIntegrationNode = docInitString.SelectSingleNode("initstring/CitrixIntegration");
            XmlNodeList list = (citrixIntegrationNode == null) ? null : citrixIntegrationNode.SelectNodes("Actions/Action");
            if (list != null)
            {
                foreach (XmlNode node in list)
                {
                    this.AddAction(--num, node.InnerText, "<ActionInit/>");
                }
            }
        }

        private bool _autoLogonAttempted = false;
        private bool _firstLogonAttempt = false;
        private bool sessionClicked = false;

        // supply the users AD credentials from your SSO provider
        private string PortalLoginName = "";
        private string PortalLoginPassword = "";
        private string PortalLoginDomain = "";
        void webBrowser1_DocumentCompleted(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
        {
            switch (e.Url.AbsolutePath.ToLower())
            {
                case "/citrix/metaframe/auth/login.aspx":
                    // Auto login to the Citrix portal

                    //HtmlElement heUser = webBrowser1.Document.GetElementById("user");
                    //if (heUser != null)
                    //    heUser.SetAttribute("value", PortalLoginName);
                    //HtmlElement hePass = webBrowser1.Document.GetElementById("password");
                    //if (hePass != null)
                    //    hePass.SetAttribute("value", PortalLoginPassword);
                    //HtmlElement heDom = webBrowser1.Document.GetElementById("domain");
                    //if (heDom != null)
                    //    heDom.SetAttribute("value", PortalLoginDomain);
                    //if (_firstLogonAttempt == true && _autoLogonAttempted == false)
                    //{
                    //    _autoLogonAttempted = true;
                    //    foreach (HtmlElement heForm in webBrowser1.Document.Forms.GetElementsByName("CitrixForm"))
                    //    {
                    //        heForm.InvokeMember("submit");
                    //        break;
                    //    }
                    //}
                    _firstLogonAttempt = true;
                    break;
                case "/citrix/metaframe/site/applist.aspx":
                    try
                    {
                        // This section will auto click a session icon in Citrix if the user has it configured.
                        // The user may need to navigate to the right Citrix folder before it will click this.
                        // An alternative to this would be to simply push the autoSelectURL to the Citrix Portal
                        // launcher frame but that could cause errors if the users is not configured for the
                        // particular application.  We choose to do it this way for reliability in a general
                        // case but pushing the URL to the launcher frame would be better if the customer situation
                        // can ensure that the app it is always configured for the USD users.

                        //if (sessionClicked == false)
                        //{
                        //    sessionClicked = true;
                        //    if (!String.IsNullOrEmpty(autoSelect))
                        //    {
                        //        //elem.Parent
                        //        HtmlWindow CitrixFrame = webBrowser1.Document.Window.Frames[0];
                        //        foreach (HtmlElement elem in CitrixFrame.Document.All)
                        //        {
                        //            if (elem.GetAttribute("title").Contains(autoSelect) && elem.Parent != null)
                        //            {
                        //                elem.Parent.InvokeMember("click");
                        //            }
                        //        }
                        //    }
                        //}
                    }
                    catch
                    {
                        // if any errors occur in auto-clicking an item, it doesn't hurt to ignore it.
                        // In the worst case scenario, the user will just have to click the link.
                        // That would decidedly be a better result than displaying an error box.
                    }
                    break;
                default:
                    _autoLogonAttempted = false;
                    break;
            }
        }

        void webBrowser1_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            if (e.Url.PathAndQuery.ToLower().Contains("/citrix/metaframe/site/launcher.aspx?"))
            {
                e.Cancel = true;
                string icaFileName = e.Url.AbsoluteUri.Replace("launcher.aspx?", "launch.ica?");
                webBrowser1.Visible = false;

                ICAClient.Visible = true;
                ICAClient.Width = base.Width;
                ICAClient.Height = base.Height;
                ICAClient.LoadIcaFile(icaFileName);
                VCClientComm.Start();
                // AutoAppResize makes the app full screen even though
                // the control is smaller.  It also removes the scrollbars
                // and clips the display.  Citrix is was working on a fix a while back (case #60065812)
                ICAClient.AutoAppResize = false;
                ICAClient.Connect();

                webBrowser1.Dispose();
                webBrowser1 = null;
            }
        }

        delegate void InitializeDelayDelegate();
        void InitializeDelay(object obj)
        {
            this.Invoke((System.Windows.Forms.MethodInvoker)delegate
            {
                this.ICAClient.AutoAppResize = true;
                base.Initialize();
            });
        }

        /// <summary>
        // If the citrix connection isn't yet established or has been disconnected, then the Citrix virtual channel
        // will timeout or otherwise hang.  This helps avoid this scenario.
        /// </summary>
        /// <param name="args">args for the action</param>
        protected override void DoAction(RequestActionEventArgs args)
        {
            // Log process.
            LogWriter.Log(string.Format(CultureInfo.CurrentCulture, "{0} -- DoAction called for action: {1}", this.ApplicationName, args.Action), System.Diagnostics.TraceEventType.Information);

            if (this.VCClientComm.IsServerAlive)
            {
                base.DoAction(args);
            }

        }

        #region User Code Area

        #endregion
    }
}
