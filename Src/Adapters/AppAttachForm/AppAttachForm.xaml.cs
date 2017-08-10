/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.USD.ComponentLibrary.Adapters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer;
using System.Runtime.InteropServices;
using Microsoft.USD.ComponentLibrary.Adapters.Automation;
using System.Threading;
using System.Windows.Automation;
using System.Windows;
using System.Collections.ObjectModel;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for AppAttachForm.xaml
    /// </summary>
    public partial class AppAttachForm : SetParentBase
    {
        #region Initialization

        IUSDAutomation automationEngine = null;
        string automationtechnology;
        void StartAutomationtechnology(IntPtr rootWindow)
        {
            if (automationEngine != null)
                return; // already started
            if (automationtechnology.Equals("java", StringComparison.InvariantCultureIgnoreCase))
                automationEngine = new JavaAutomation(rootWindow, ApplicationName, localSession, LogWriter);
            else if (automationtechnology.Equals("windows", StringComparison.InvariantCultureIgnoreCase))
                automationEngine = new UIAutomation(rootWindow, ApplicationName, localSession, LogWriter);
            else
                throw new Exception("Unknown Automation technology: " + automationtechnology);
        }

        void StopAutomationtechnology()
        {
            if (automationEngine == null)
                return; // not started
            automationEngine.Dispose();
            automationEngine = null;
            automationtechnology = String.Empty;
        }

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public AppAttachForm(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            InitializeComponent();
        }

        public override void Close()
        {

            base.Close();
        }

        #endregion

        #region Actions
        /// <summary>
        /// Raised when the Desktop Ready event is fired. 
        /// </summary>
        protected override void DesktopReady()
        {
            // this will populate any toolbars assigned to this control in config. 
            PopulateToolbars(ProgrammableToolbarTray);
            WindowAttached += new EventHandler<EventArgs>(WindowAttached_Handler);
            base.DesktopReady();
            try { ApplyTemplate(); }
            catch { }

            RegisterAction("AttachWindow", AttachWindow);
            RegisterAction("UseWindow", UseWindow);
            RegisterAction("DynamicPositionWindow", DynamicPositionWindow);
            RegisterAction("DetachWindow", DetachWindow);
            RegisterAction("CloseWindow", CloseWindow);
            RegisterAction("Launch", Launch);
            RegisterAction("ListWindows", ListWindows);
            RegisterAction("AttachByTitle", AttachByTitle);
            RegisterAction("SetValue", SetValue);
            RegisterAction("GetValue", GetValue);
            RegisterAction("Execute", Execute);
            RegisterAction("DisplayVisualTree", DisplayVisualTree);

        }

        private void WindowAttached_Handler(object sender, EventArgs args)
        {
            StartAutomationtechnology(win.Handle);
        }

        void AttachWindow(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowhandle = Utility.GetAndRemoveParameter(parameters, "windowhandle");
            string removeFrameAndCaptionParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFrameAndCaption");
            string removeFromTaskbarParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFromTaskbar");
            string removeSizingControlsParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingControls");
            string removeSizingMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingMenu");
            string removeSystemMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSystemMenu");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                StartAutomationtechnology(winPtr);
                hostingMode = WindowHostingMode.SETPARENT;
                if (!bool.TryParse(removeFrameAndCaptionParameter, out removeFrameAndCaption))
                    removeFrameAndCaption = true;
                if (!bool.TryParse(removeFromTaskbarParameter, out removeFromTaskbar))
                    removeFromTaskbar = true;
                if (!bool.TryParse(removeSizingControlsParameter, out removeSizingControls))
                    removeSizingControls = true;
                if (!bool.TryParse(removeSizingMenuParameter, out removeSizingMenu))
                    removeSizingMenu = true;
                if (!bool.TryParse(removeSystemMenuParameter, out removeSystemMenu))
                    removeSystemMenu = true;
                WindowAttachApplicationWindow(winPtr, myhost);
            }
        }

        void UseWindow(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowhandle = Utility.GetAndRemoveParameter(parameters, "windowhandle");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java, windows
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                StartAutomationtechnology(winPtr);
                UseNoAttachApplicationWindow(winPtr, myhost);
            }
        }

        void DisplayVisualTree(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowhandle = Utility.GetAndRemoveParameter(parameters, "windowhandle");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java, windows
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                StartAutomationtechnology(winPtr);
                automationEngine.DisplayVisualTree();
            }
        }

        void DynamicPositionWindow(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowhandle = Utility.GetAndRemoveParameter(parameters, "windowhandle");
            string removeFrameAndCaptionParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFrameAndCaption");
            string removeFromTaskbarParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFromTaskbar");
            string removeSizingControlsParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingControls");
            string removeSizingMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingMenu");
            string removeSystemMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSystemMenu");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                StartAutomationtechnology(winPtr);
                hostingMode = WindowHostingMode.DYNAMICPOSITIONING;
                if (!bool.TryParse(removeFrameAndCaptionParameter, out removeFrameAndCaption))
                    removeFrameAndCaption = true;
                if (!bool.TryParse(removeFromTaskbarParameter, out removeFromTaskbar))
                    removeFromTaskbar = true;
                if (!bool.TryParse(removeSizingControlsParameter, out removeSizingControls))
                    removeSizingControls = true;
                if (!bool.TryParse(removeSizingMenuParameter, out removeSizingMenu))
                    removeSizingMenu = true;
                if (!bool.TryParse(removeSystemMenuParameter, out removeSystemMenu))
                    removeSystemMenu = true;
                WindowAttachApplicationWindow(winPtr, myhost);
            }
        }

        void DetachWindow(Uii.Csr.RequestActionEventArgs args)
        {
            base.DetachWindow();
            StopAutomationtechnology();
        }

        void CloseWindow(Uii.Csr.RequestActionEventArgs args)
        {
            base.CloseWindow();
            StopAutomationtechnology();
        }

        void Launch(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string path = Utility.GetAndRemoveParameter(parameters, "path"); //C:\Program Files\Java\jre1.8.0_51\bin\java
            string arguments = Utility.GetAndRemoveParameter(parameters, "arguments");  // -jar SimpleSwing.jar
            string workingfolder = Utility.GetAndRemoveParameter(parameters, "workingfolder"); // C:\Users\jaymep\OneDrive\DEVELOPMENT\SimpleSwing
            string mainwindowtitle = Utility.GetAndRemoveParameter(parameters, "mainwindowtitle"); //Swing Simple Application
            string hostingmoderequested = Utility.GetAndRemoveParameter(parameters, "hostingmode"); //setparent or dynamicpositioning
            string removeFrameAndCaptionParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFrameAndCaption");
            string removeFromTaskbarParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFromTaskbar");
            string removeSizingControlsParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingControls");
            string removeSizingMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingMenu");
            string removeSystemMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSystemMenu");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java, windows
            Process appprocess = new Process();
            appprocess.StartInfo.WorkingDirectory = workingfolder;
            appprocess.StartInfo.FileName = path;
            appprocess.StartInfo.Arguments = arguments;
            appprocess.Start();
            if (String.IsNullOrEmpty(hostingmoderequested)
                || hostingmoderequested.Equals("setparent", StringComparison.InvariantCultureIgnoreCase))
                hostingMode = WindowHostingMode.SETPARENT;
            else if (hostingmoderequested.Equals("dp", StringComparison.InvariantCultureIgnoreCase)
                || hostingmoderequested.Equals("dynamicpositioning", StringComparison.InvariantCultureIgnoreCase))
                hostingMode = WindowHostingMode.DYNAMICPOSITIONING;
            if (!bool.TryParse(removeFrameAndCaptionParameter, out removeFrameAndCaption))
                removeFrameAndCaption = true;
            if (!bool.TryParse(removeFromTaskbarParameter, out removeFromTaskbar))
                removeFromTaskbar = true;
            if (!bool.TryParse(removeSizingControlsParameter, out removeSizingControls))
                removeSizingControls = true;
            if (!bool.TryParse(removeSizingMenuParameter, out removeSizingMenu))
                removeSizingMenu = true;
            if (!bool.TryParse(removeSystemMenuParameter, out removeSystemMenu))
                removeSystemMenu = true;
            if (String.IsNullOrEmpty(mainwindowtitle))
            {
                ProcessAttachApplicationWindow(appprocess, myhost);
            }
            else
            {
                LookForAndAttachApplicationWindow(mainwindowtitle, myhost);
            }
        }

        void ListWindows(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowRegex = Utility.GetAndRemoveParameter(parameters, "regex");
            if (windowRegex == null)
                windowRegex = String.Empty;
            List<WindowMetadata> windowMeta = GetWindowMetaData(windowRegex);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ListWindows Found: ");
            foreach (WindowMetadata wm in windowMeta)
            {
                sb.AppendLine("\tWindow Handle:[" + wm.handle.ToString() + "] Name:[" + wm.name + "] ");
            }
            LogWriter.Log(sb.ToString(), TraceEventType.Verbose);
        }

        void AttachByTitle(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string mainwindowtitle = Utility.GetAndRemoveParameter(parameters, "mainwindowtitle");
            string hostingmoderequested = Utility.GetAndRemoveParameter(parameters, "hostingmode"); //setparent or dynamicpositioning
            string removeFrameAndCaptionParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFrameAndCaption");
            string removeFromTaskbarParameter = Utility.GetAndRemoveParameter(parameters, "RemoveFromTaskbar");
            string removeSizingControlsParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingControls");
            string removeSizingMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSizingMenu");
            string removeSystemMenuParameter = Utility.GetAndRemoveParameter(parameters, "RemoveSystemMenu");
            if (String.IsNullOrEmpty(hostingmoderequested)
                || hostingmoderequested.Equals("setparent", StringComparison.InvariantCultureIgnoreCase))
                hostingMode = WindowHostingMode.SETPARENT;
            else if (hostingmoderequested.Equals("dp", StringComparison.InvariantCultureIgnoreCase)
                || hostingmoderequested.Equals("dynamicpositioning", StringComparison.InvariantCultureIgnoreCase))
                hostingMode = WindowHostingMode.DYNAMICPOSITIONING;
            if (!bool.TryParse(removeFrameAndCaptionParameter, out removeFrameAndCaption))
                removeFrameAndCaption = true;
            if (!bool.TryParse(removeFromTaskbarParameter, out removeFromTaskbar))
                removeFromTaskbar = true;
            if (!bool.TryParse(removeSizingControlsParameter, out removeSizingControls))
                removeSizingControls = true;
            if (!bool.TryParse(removeSizingMenuParameter, out removeSizingMenu))
                removeSizingMenu = true;
            if (!bool.TryParse(removeSystemMenuParameter, out removeSystemMenu))
                removeSystemMenu = true;
            LookForAndAttachApplicationWindow(mainwindowtitle, myhost);
        }

        void SetValue(Uii.Csr.RequestActionEventArgs args)
        {
            string parameterdata = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(parameterdata);
            foreach (XmlNode nodeControl in doc.SelectSingleNode("//Controls").ChildNodes) ///JAccControl or AccControl
            {
                using (IUSDAutomationObject automationObject = automationEngine.GetAutomationObject(nodeControl))
                {
                    try
                    {
                        string controlValue = AutomationControl.GetAttributeValue(nodeControl, "value", ""); 
                        automationEngine.SetValue(automationObject, controlValue);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Log(ex);
                    }
                }
            }
        }

        void GetValue(Uii.Csr.RequestActionEventArgs args)
        {
            string parameterdata = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(parameterdata);
            List<LookupRequestItem> lritems = new List<LookupRequestItem>();
            foreach (XmlNode nodeControl in doc.SelectSingleNode("//Controls").ChildNodes) ///JAccControl or AccControl
            {
                using (IUSDAutomationObject automationObject = automationEngine.GetAutomationObject(nodeControl))
                {
                    try
                    {
                        lritems.Add(automationEngine.GetValue(automationObject));
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Log(ex);
                    }
                }
            }
            ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, lritems, false);
        }

        void Execute(Uii.Csr.RequestActionEventArgs args)
        {
            /*
            <Controls>
                <JAccControl name="clickme" action="click">
                    <Path>
                    <NextName offset = "1">Click Me</NextName>
                    </Path>
                </JAccControl>
            </Controls>
            */


            string parameterdata = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(parameterdata);

            foreach (XmlNode nodeControl in doc.SelectSingleNode("//Controls").ChildNodes) ///JAccControl or AccControl
            {
                using (IUSDAutomationObject automationObject = automationEngine.GetAutomationObject(nodeControl))
                {
                    try
                    {
                        string controlAction = AutomationControl.GetAttributeValue(nodeControl, "action", "");
                        automationEngine.Execute(automationObject, controlAction);
                    }
                    catch (Exception ex)
                    {
                        LogWriter.Log(ex);
                    }
                }
            }
        }
        #endregion
        

    }
}
