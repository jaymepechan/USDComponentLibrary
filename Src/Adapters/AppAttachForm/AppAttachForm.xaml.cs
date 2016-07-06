/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility;
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using Microsoft.Uii.HostedApplicationToolkit.DataDrivenAdapter;
using Microsoft.USD.ComponentLibrary.Adapters;
using Microsoft.USD.ComponentLibrary.Adapters.Java;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Xml;
using Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer;
using System.Runtime.InteropServices;

namespace Microsoft.USD.ComponentLibrary
{
    /// <summary>
    /// Interaction logic for AppAttachForm.xaml
    /// </summary>
    public partial class AppAttachForm : SetParentBase
    {
        #region Initialization
        private string automationtechnology = String.Empty;
        private System.Threading.SynchronizationContext SynchronizationContext;
        private System.Threading.Timer JavaAccEventListenerInstallTimer;

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
            if (this.JavaAccEventListenerInstallTimer != null)
            {
                this.JavaAccEventListenerInstallTimer.Dispose();
                this.JavaAccEventListenerInstallTimer = null;
            }
            Trace.WriteLine("Unsubscribing from java control changed events..");
            Adapters.Java.JavaAccEventListener.JavaControlChanged -= new System.EventHandler<JavaAccEventArgs>(this.OnJavaControlChanged);
            Adapters.Java.JavaAccEventListener.FreeEventHandlers();

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
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                UseNoAttachApplicationWindow(winPtr, myhost);
            }
        }

        JavaTreeViewModel jtvm;
        ApplicationVisualViewer avv = null;
        Window w = new Window();
        void DisplayVisualTree(Uii.Csr.RequestActionEventArgs args)
        {
            List<KeyValuePair<string, string>> parameters = Utility.SplitLines(args.Data, CurrentContext, localSession);
            string windowhandle = Utility.GetAndRemoveParameter(parameters, "windowhandle");
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java
            if (!String.IsNullOrEmpty(windowhandle))
            {
                IntPtr winPtr;
                long temp;
                if (long.TryParse(windowhandle, out temp))
                    winPtr = new IntPtr(temp);
                else
                    winPtr = new IntPtr(Convert.ToInt64(windowhandle, 16));
                win = new HandleRef(this, winPtr);  // needed action methods to work
                if (automationtechnology.Equals("java", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                    {
                        JavaAccNativeMethods.LoadJavaAccessBridge();
                        JavaAccNativeMethods.Windows_run();
                        Trace.WriteLine("Initializing the java dda instance..");
                        JavaAccHelperMethods.InitializeJavaAccessBridge(true);
                        this.SynchronizationContext = System.Threading.SynchronizationContext.Current;
                        System.Windows.Forms.Application.DoEvents();
                        System.Windows.Forms.Application.DoEvents();
                        System.Windows.Forms.Application.DoEvents();
                        System.Windows.Forms.Application.DoEvents();
                        System.Windows.Forms.Application.DoEvents();
                    }
                    int vmId = 0;
                    System.IntPtr accFromWindow = JavaAccHelperMethods.GetAccFromWindow(winPtr, out vmId);
                    jtvm = new JavaTreeViewModel(new JavaWindow(null, accFromWindow, vmId));
                    if (w != null)
                    {
                        w.Closing -= W_Closing;
                        avv = null;
                        w = null;
                    }
                    avv = new ApplicationVisualViewer(ApplicationName);
                    avv.LoadTree(jtvm);
                    w = new Window();
                    w.Closing += W_Closing;
                    w.Content = avv;
                    w.Show();
                }
            }
        }

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            w = null;
            avv = null;
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
        }

        void CloseWindow(Uii.Csr.RequestActionEventArgs args)
        {
            base.CloseWindow();
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
            automationtechnology = Utility.GetAndRemoveParameter(parameters, "automationtechnology"); //java
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
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            /*
            <Controls>
                <JAccControl name="clickme" value="something">
                    <Path>
                    <NextName offset = "1">Click Me</NextName>
                    </Path>
                </JAccControl>
            </Controls>
            */
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            string parameterdata = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(parameterdata);

            foreach (XmlNode nodeControl in doc.SelectNodes("//Controls/JAccControl"))
            {
                string controlName = GetAttributeValue(nodeControl, "name", "");
                string controlValue = GetAttributeValue(nodeControl, "value", "");
                XmlNode nodePath = nodeControl.SelectSingleNode("Path");
                zero = this.FindAccObj(controlName, out vmId, nodePath.OuterXml);
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, zero, out accContextInfo))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Java Control Found: " + controlName);
                    sb.AppendLine("accessibleText: " + accContextInfo.accessibleText.ToString());
                    sb.AppendLine("accessibleAction: " + accContextInfo.accessibleAction.ToString());
                    sb.AppendLine("accessibleComponent: " + accContextInfo.accessibleComponent.ToString());
                    sb.AppendLine("accessibleInterfaces: " + accContextInfo.accessibleInterfaces.ToString());
                    sb.AppendLine("accessibleSelection: " + accContextInfo.accessibleSelection.ToString());
                    sb.AppendLine("childrenCount: " + accContextInfo.childrenCount.ToString());
                    sb.AppendLine("description: " + accContextInfo.description.ToString());
                    sb.AppendLine("height: " + accContextInfo.height.ToString());
                    sb.AppendLine("indexInParent: " + accContextInfo.indexInParent.ToString());
                    sb.AppendLine("name: " + accContextInfo.name);
                    sb.AppendLine("role: " + accContextInfo.role);
                    sb.AppendLine("role_en_US: " + accContextInfo.role_en_US);
                    sb.AppendLine("states: " + accContextInfo.states);
                    sb.AppendLine("states_en_US: " + accContextInfo.states_en_US);
                    sb.AppendLine("width: " + accContextInfo.width.ToString());
                    sb.AppendLine("x: " + accContextInfo.x.ToString());
                    sb.AppendLine("y: " + accContextInfo.y.ToString());
                    LogWriter.Log(sb.ToString(), TraceEventType.Verbose);
                    string[] stateList = accContextInfo.states.Split(',');
                    if (!stateList.Contains("editable"))    // for text boxes
                        throw new Exception("Control is not editable");
                }
                JavaAccHelperMethods.SetValue(zero, vmId, controlValue);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        void GetValue(Uii.Csr.RequestActionEventArgs args)
        {
            /*
            <Controls>
                <JAccControl name="clickme">
                    <Path>
                    <NextName offset = "1">Click Me</NextName>
                    </Path>
                </JAccControl>
                <JAccControl name="textbox">
                    <Path>
                    <NextRole offset = "0">text</NextRole>
                    </Path>
                </JAccControl>
            </Controls>
            */

            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            string parameterdata = Utility.GetContextReplacedString(args.Data, CurrentContext, localSession);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(parameterdata);

            List<LookupRequestItem> lri = new List<LookupRequestItem>();
            foreach (XmlNode nodeControl in doc.SelectNodes("//Controls/JAccControl"))
            {
                string controlName = GetAttributeValue(nodeControl, "name", "");
                XmlNode nodePath = nodeControl.SelectSingleNode("Path");
                zero = this.FindAccObj(controlName, out vmId, nodePath.OuterXml);
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, zero, out accContextInfo))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Java Control Found: " + controlName);
                    sb.AppendLine("\taccessibleText: " + accContextInfo.accessibleText.ToString());
                    sb.AppendLine("\taccessibleAction: " + accContextInfo.accessibleAction.ToString());
                    sb.AppendLine("\taccessibleComponent: " + accContextInfo.accessibleComponent.ToString());
                    sb.AppendLine("\taccessibleInterfaces: " + accContextInfo.accessibleInterfaces.ToString());
                    sb.AppendLine("\taccessibleSelection: " + accContextInfo.accessibleSelection.ToString());
                    sb.AppendLine("\tchildrenCount: " + accContextInfo.childrenCount.ToString());
                    sb.AppendLine("\tdescription: " + accContextInfo.description.ToString());
                    sb.AppendLine("\theight: " + accContextInfo.height.ToString());
                    sb.AppendLine("\tindexInParent: " + accContextInfo.indexInParent.ToString());
                    sb.AppendLine("\tname: " + accContextInfo.name);
                    sb.AppendLine("\trole: " + accContextInfo.role);
                    sb.AppendLine("\trole_en_US: " + accContextInfo.role_en_US);
                    sb.AppendLine("\tstates: " + accContextInfo.states);
                    sb.AppendLine("\tstates_en_US: " + accContextInfo.states_en_US);
                    sb.AppendLine("\twidth: " + accContextInfo.width.ToString());
                    sb.AppendLine("\tx: " + accContextInfo.x.ToString());
                    sb.AppendLine("\ty: " + accContextInfo.y.ToString());
                    LogWriter.Log(sb.ToString(), TraceEventType.Verbose);
                    string[] stateList = accContextInfo.states.Split(',');
                    Trace.WriteLine(sb.ToString());
                }
                try
                {
                    string controlValue = String.Empty;
                    controlValue = JavaAccHelperMethods.GetValue(zero, vmId);
                    lri.Add(new LookupRequestItem(controlName, controlValue));
                }
                finally
                {
                    JavaAccHelperMethods.ReleaseObject(zero, vmId);
                    System.Windows.Forms.Application.DoEvents();
                }
            }
            ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, lri, false);
        }

        void Execute(Uii.Csr.RequestActionEventArgs args)
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
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

            foreach (XmlNode nodeControl in doc.SelectNodes("//Controls/JAccControl"))
            {
                string controlName = GetAttributeValue(nodeControl, "name", "");
                string controlAction = GetAttributeValue(nodeControl, "action", "");
                XmlNode nodePath = nodeControl.SelectSingleNode("Path");
                zero = this.FindAccObj(controlName, out vmId, nodePath.OuterXml);
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, zero, out accContextInfo))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Java Control Found: " + controlName);
                    sb.AppendLine("accessibleText: " + accContextInfo.accessibleText.ToString());
                    sb.AppendLine("accessibleAction: " + accContextInfo.accessibleAction.ToString());
                    sb.AppendLine("accessibleComponent: " + accContextInfo.accessibleComponent.ToString());
                    sb.AppendLine("accessibleInterfaces: " + accContextInfo.accessibleInterfaces.ToString());
                    sb.AppendLine("accessibleSelection: " + accContextInfo.accessibleSelection.ToString());
                    sb.AppendLine("childrenCount: " + accContextInfo.childrenCount.ToString());
                    sb.AppendLine("description: " + accContextInfo.description.ToString());
                    sb.AppendLine("height: " + accContextInfo.height.ToString());
                    sb.AppendLine("indexInParent: " + accContextInfo.indexInParent.ToString());
                    sb.AppendLine("name: " + accContextInfo.name);
                    sb.AppendLine("role: " + accContextInfo.role);
                    sb.AppendLine("role_en_US: " + accContextInfo.role_en_US);
                    sb.AppendLine("states: " + accContextInfo.states);
                    sb.AppendLine("states_en_US: " + accContextInfo.states_en_US);
                    sb.AppendLine("width: " + accContextInfo.width.ToString());
                    sb.AppendLine("x: " + accContextInfo.x.ToString());
                    sb.AppendLine("y: " + accContextInfo.y.ToString());
                    LogWriter.Log(sb.ToString(), TraceEventType.Verbose);
                    string[] stateList = accContextInfo.states.Split(',');
                    if (accContextInfo.accessibleAction != true)    // for text boxes
                        throw new Exception("Control cannot be executed");
                    Trace.WriteLine(sb.ToString());
                }
                //AccessibleActions actions = new AccessibleActions[] { new AccessibleActions() };
                //actions[0].actionsCount = 256;
                //actions[0].actions = new AccessibleActionInfo[0x100];
                //for (int i = 0; i < 0x100; i++)
                //{
                //    actions[0].actions[i] = new AccessibleActionInfo();
                //    actions[0].actions[i].name = (new char[0x100]).ToString();
                //}
                //bool result = JavaAccNativeMethods.getAccessibleActions(vmId, zero, actions);
                //for (int i = 0; i < actions.actionsCount; i++)
                //{
                //    Trace.WriteLine("Action(" + i.ToString() + "): " + actions.actions[i].name);
                //}

                int failure;
                JavaAccHelperMethods.DoAction(zero, out failure, vmId, false, controlAction);
                System.Windows.Forms.Application.DoEvents();
            }
        }
        #endregion

        #region JAVA
        private void WindowAttached_Handler(object sender, EventArgs args)
        {
            //if (!String.IsNullOrEmpty(automationtechnology))
            //{
            //    if (automationtechnology.Equals("java"))
            //    {
            //        try
            //        {
            //            JavaAccNativeMethods.LoadJavaAccessBridge("windowsaccessbridge.dll");
            //            JavaAccNativeMethods.Windows_run();
            //            _IsWindowsAccessBridgeAvailable = true;
            //        }
            //        catch (Exception ex)
            //        {
            //            LogWriter.Log(ex);
            //        }
            //        try
            //        {
            //            int num;
            //            JavaAccHelperMethods.GetAccFromWindow(win.Handle, out num);
            //            System.Windows.Forms.Application.DoEvents();
            //            Trace.WriteLine("Initializing the java dda instance..");
            //            JavaAccHelperMethods.InitializeJavaAccessBridge(true);
            //            this.SynchronizationContext = System.Threading.SynchronizationContext.Current;
            //            this.JavaAccEventListenerInstallTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.InstallAccEventListener_TimerCallback), null, (int)(-1), 0);
            //        }
            //        catch
            //        {
            //        }

            //    }
            //}
        }

        private string AccControl(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
        //    try
        //    {
        //        int num2;
        //        switch (op)
        //        {
        //            case OperationType.FindControl:
        //                zero = this.FindAccObj(controlName, out vmId, false);
        //                if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
        //                {
        //                    return bool.FalseString;
        //                }
        //                return bool.TrueString;

        //            case OperationType.GetControlValue:
        //                zero = this.FindAccObj(controlName, out vmId, true);
        //                return JavaAccHelperMethods.GetValue(zero, vmId);

        //            case OperationType.SetControlValue:
        //                zero = this.FindAccObj(controlName, out vmId, true);
        //                JavaAccHelperMethods.SetValue(zero, vmId, controlValue);
        //                return null;

        //            case OperationType.ExecuteControlAction:
        //                zero = this.FindAccObj(controlName, out vmId, true);
        //                if (!System.Threading.Thread.CurrentThread.CurrentCulture.Name.StartsWith(JavaDataDrivenAdapterConstants.FRENCH_CULTURE_TEXT, System.StringComparison.OrdinalIgnoreCase))
        //                {
        //                    break;
        //                }
        //                JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.FRENCH_DEFAULT_ACTION_NAME);
        //                goto Label_00D6;

        //            default:
        //                goto Label_00E4;
        //        }
        //        JavaAccHelperMethods.DoAction(zero, out num2, vmId, true, string.Empty);
        //    Label_00D6:
        //        return null;
        //    }
        //    finally
        //    {
        //        JavaAccHelperMethods.ReleaseObject(zero, vmId);
        //    }
        //Label_00E4:
            return null;
        }

        private string AccHyperlink(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            //try
            //{
            //    switch (op)
            //    {
            //        case OperationType.FindControl:
            //            zero = this.FindAccObj(controlName, out vmId, false);
            //            if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
            //            {
            //                return bool.FalseString;
            //            }
            //            return bool.TrueString;

            //        case OperationType.GetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            return JavaAccHelperMethods.GetAccHyperlink(zero, vmId);

            //        case OperationType.SetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            JavaAccHelperMethods.ActivateAccHyperlink(zero, vmId);
            //            return null;

            //        case OperationType.ExecuteControlAction:
            //            int num2;
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            if (!JavaAccHelperMethods.DoAction(zero, out num2, vmId, true, string.Empty))
            //            {
            //                return bool.FalseString;
            //            }
            //            return bool.TrueString;
            //    }
            //}
            //finally
            //{
            //    JavaAccHelperMethods.ReleaseObject(zero, vmId);
            //}
            return null;
        }

        private string AccSelector(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            //try
            //{
            //    switch (op)
            //    {
            //        case OperationType.FindControl:
            //            zero = this.FindAccObj(controlName, out vmId, false);
            //            if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
            //            {
            //                return bool.FalseString;
            //            }
            //            return bool.TrueString;

            //        case OperationType.GetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            return JavaAccHelperMethods.GetAccSelectionName(zero, vmId);

            //        case OperationType.SetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            JavaAccHelperMethods.SetAccSelection(zero, vmId, controlValue);
            //            return null;

            //        case OperationType.ExecuteControlAction:
            //            int num2;
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.TOGGLE_POPUP_ACTION_NAME);
            //            return null;
            //    }
            //}
            //catch (System.ArgumentException)
            //{
            //    throw new DataDrivenAdapterException("Incorrect control value format");
            //}
            //finally
            //{
            //    JavaAccHelperMethods.ReleaseObject(zero, vmId);
            //}
            return null;
        }

        private string AccTable(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            //try
            //{
            //    switch (op)
            //    {
            //        case OperationType.FindControl:
            //            zero = this.FindAccObj(controlName, out vmId, false);
            //            if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
            //            {
            //                return bool.FalseString;
            //            }
            //            return bool.TrueString;

            //        case OperationType.GetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            return JavaAccHelperMethods.GetAccTableCellValue(zero, vmId, controlValue);

            //        case OperationType.SetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId, true);
            //            JavaAccHelperMethods.SetAccTableCellValue(zero, vmId, controlName, controlValue);
            //            return null;
            //    }
            //}
            //catch (System.ArgumentException)
            //{
            //    throw new DataDrivenAdapterException("Incorrect control value format");
            //}
            //finally
            //{
            //    JavaAccHelperMethods.ReleaseObject(zero, vmId);
            //}
            return null;
        }

        private string AccTree(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            //try
            //{
            //    switch (op)
            //    {
            //        case OperationType.FindControl:
            //            zero = this.FindAccObj(controlName, out vmId);
            //            if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
            //            {
            //                return bool.FalseString;
            //            }
            //            return bool.TrueString;

            //        case OperationType.GetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId);
            //            return JavaAccHelperMethods.GetAccSelectionName(zero, vmId);

            //        case OperationType.SetControlValue:
            //            zero = this.FindAccObj(controlName, out vmId);
            //            JavaAccHelperMethods.SetAccSelection(zero, vmId, controlValue);
            //            return null;

            //        case OperationType.ExecuteControlAction:
            //            int num2;
            //            zero = this.FindAccObj(controlName, out vmId);
            //            JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.TOGGLE_EXPAND_ACTION_NAME);
            //            return null;
            //    }
            //}
            //catch (System.ArgumentException)
            //{
            //    throw new DataDrivenAdapterException("Incorrect control value format");
            //}
            //finally
            //{
            //    JavaAccHelperMethods.ReleaseObject(zero, vmId);
            //}
            return null;
        }

        protected System.IntPtr FindAccObj(string controlName, out int vmId, string controlXml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(controlXml);
            XmlNode firstDescendentUnderControlConfig = doc.SelectSingleNode("//Path");
            if (firstDescendentUnderControlConfig == null)
            {
                throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "No Path element found");
            }
            //if (_NeedToCallWindowsRun)
            //{
            //    JavaAccNativeMethods.Windows_run();
            //    _NeedToCallWindowsRun = false;
            //    System.Windows.Forms.Application.DoEvents();
            //}
            System.IntPtr accFromWindow = JavaAccHelperMethods.GetAccFromWindow(win.Handle, out vmId);
            string defaultValue = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            for (XmlNode node2 = firstDescendentUnderControlConfig.FirstChild; node2 != null; node2 = node2.NextSibling)
            {
                int num;
                int num2;
                bool flag;
                switch (node2.Name)
                {
                    case "FindWindow":
                        accFromWindow = JavaAccHelperMethods.GetAccFromWindow(this.FindWindowFromControlName(controlName, true), out vmId);
                        break;

                    case "Next":
                    case "NextName":
                        num = GetAttributeValue(node2, "match", 1);
                        num2 = GetAttributeValue(node2, "offset", 0);
                        flag = false;
                        if (GetAttributeValue(node2, "culture", defaultValue).Equals(defaultValue, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (num <= 0)
                            {
                                accFromWindow = System.IntPtr.Zero;
                            }
                            else
                            {
                                accFromWindow = JavaAccHelperMethods.GetNextChildByName(node2.InnerText, ref num, num2, ref flag, accFromWindow, vmId);
                            }
                        }
                        break;

                    case "NextRole":
                        num = GetAttributeValue(node2, "match", 1);
                        num2 = GetAttributeValue(node2, "offset", 0);
                        flag = false;
                        if (GetAttributeValue(node2, "culture", defaultValue).Equals(defaultValue, System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (num <= 0)
                            {
                                accFromWindow = System.IntPtr.Zero;
                            }
                            else
                            {
                                accFromWindow = JavaAccHelperMethods.GetNextChildByRole(node2.InnerText, ref num, num2, ref flag, accFromWindow, vmId);
                            }
                        }
                        break;

                    default:
                        throw new DataDrivenAdapterException("Unsupported path element");
                }
                if (accFromWindow.Equals((System.IntPtr)System.IntPtr.Zero))
                {
                    break;
                }
            }
            if (accFromWindow.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                throw new DataDrivenAdapterException("Unable to find control on UI");
                return accFromWindow;
            }
            //this.KnownControls.RegisterControl(controlName, accFromWindow, vmId);
            return accFromWindow;
        }

        protected static int GetAttributeValue(XmlNode node, string attributeName, int defaultValue)
        {
            int num = defaultValue;
            if ((node != null) && (node.Attributes != null))
            {
                int num2;
                XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
                if ((namedItem != null) && int.TryParse(namedItem.Value, out num2))
                {
                    num = num2;
                }
            }
            return num;
        }

        protected static string GetAttributeValue(XmlNode node, string attributeName, string defaultValue)
        {
            string str = defaultValue;
            if ((node != null) && (node.Attributes != null))
            {
                XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
                if (namedItem != null)
                {
                    str = namedItem.Value;
                }
            }
            return str;
        }

        protected System.IntPtr FindWindowFromControlName(string controlName, bool throwExceptionIfNotFound)
        {
            //int num;
            //XmlNode firstDescendentUnderControlConfig = base.GetFirstDescendentUnderControlConfig(controlName, "FindWindow");
            //if (firstDescendentUnderControlConfig == null)
            //{
            //    throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "No find window element found");
            //}
            //int windowThreadProcessId = Win32NativeMethods.GetWindowThreadProcessId(this.hWndMainWindow, out num);
            System.IntPtr hWndMainWindow = win.Handle;
            //for (XmlNode node2 = firstDescendentUnderControlConfig.FirstChild; node2 != null; node2 = node2.NextSibling)
            //{
            //    int num3;
            //    FindWindowMatchType type;
            //    string str2;
            //    string str3;
            //    FindWindowMatchType ignore;
            //    FindWindowMatchType type3;
            //    XmlNode nextSibling;
            //    switch (node2.Name)
            //    {
            //        case "ControlID":
            //        case "ControlId":
            //            int num4;
            //            num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
            //            int.TryParse(node2.InnerText, System.Globalization.NumberStyles.HexNumber, null, out num4);
            //            hWndMainWindow = Win32HelperMethods.FindWindowByControlId(hWndMainWindow, num, windowThreadProcessId, num4, num3, this._IsApplet);
            //            goto Label_045C;

            //        case "Caption":
            //        case "CaptionStartsWith":
            //        case "CaptionEndsWith":
            //        case "CaptionContains":
            //            num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
            //            type = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(DataDrivenAdapterBase.GetAttributeValue(node2, "matchtype", string.Empty));
            //            hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, node2.InnerText, type, null, FindWindowMatchType.Ignore, num3, this._IsApplet);
            //            goto Label_045C;

            //        case "Class":
            //        case "ClassStartsWith":
            //        case "ClassEndsWith":
            //        case "ClassContains":
            //            num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
            //            type = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(DataDrivenAdapterBase.GetAttributeValue(node2, "matchtype", string.Empty));
            //            hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, null, FindWindowMatchType.Ignore, node2.InnerText, type, num3, this._IsApplet);
            //            goto Label_045C;

            //        case "Position":
            //            {
            //                int num5;
            //                int num6;
            //                string[] strArray = node2.InnerText.Replace(',', ' ').Split((char[])new char[0]);
            //                int.TryParse((strArray.Length > 0) ? strArray[0] : ((string)"0"), out num5);
            //                int.TryParse((strArray.Length > 1) ? strArray[1] : ((string)"0"), out num6);
            //                hWndMainWindow = Win32HelperMethods.FindWindowByPosition(hWndMainWindow, num, windowThreadProcessId, num5, num6, this._IsApplet);
            //                goto Label_045C;
            //            }
            //        case "Find":
            //            num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
            //            str2 = null;
            //            str3 = null;
            //            ignore = FindWindowMatchType.Ignore;
            //            type3 = FindWindowMatchType.Ignore;
            //            nextSibling = node2.FirstChild;
            //            goto Label_040C;

            //        case "Application":
            //            hWndMainWindow = this.hWndMainWindow;
            //            goto Label_045C;

            //        case "Desktop":
            //            hWndMainWindow = Win32NativeMethods.GetDesktopWindow();
            //            goto Label_045C;

            //        case "Owner":
            //            hWndMainWindow = Win32NativeMethods.GetWindow(hWndMainWindow, WinUserConstant.SW_SHOWNOACTIVATE);
            //            goto Label_045C;

            //        case "RelaxProcessIdRestriction":
            //            num = 0;
            //            goto Label_045C;

            //        case "RelaxThreadIdRestriction":
            //            windowThreadProcessId = 0;
            //            goto Label_045C;

            //        default:
            //            throw new DataDrivenAdapterException("Unsupported FindWindow element");
            //    }
            //Label_0307:
            //    switch (nextSibling.Name)
            //    {
            //        case "Caption":
            //        case "CaptionStartsWith":
            //        case "CaptionEndsWith":
            //        case "CaptionContains":
            //            str2 = nextSibling.InnerText;
            //            ignore = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(nextSibling.Name.Substring(7));
            //            break;

            //        case "Class":
            //        case "ClassStartsWith":
            //        case "ClassEndsWith":
            //        case "ClassContains":
            //            str3 = nextSibling.InnerText;
            //            type3 = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(nextSibling.Name.Substring(5));
            //            break;
            //    }
            //    nextSibling = nextSibling.NextSibling;
            //Label_040C:
            //    if (nextSibling != null)
            //    {
            //        goto Label_0307;
            //    }
            //    hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, str2, ignore, str3, type3, num3, this._IsApplet);
            //Label_045C:
            //    if (hWndMainWindow.Equals((System.IntPtr)System.IntPtr.Zero))
            //    {
            //        break;
            //    }
            //}
            //if (hWndMainWindow.Equals((System.IntPtr)System.IntPtr.Zero) && throwExceptionIfNotFound)
            //{
            //    throw new DataDrivenAdapterException("Unable to find window");
            //}
            return hWndMainWindow;
        }

        private void InstallAccEventListener_SendOrPostCallback(object state)
        {
            if (this.JavaAccEventListenerInstallTimer != null)
            {
                Trace.WriteLine("Subscribing to java control changed events..");
                Adapters.Java.JavaAccEventListener.SetEventHandlers();
                Adapters.Java.JavaAccEventListener.JavaControlChanged += new System.EventHandler<JavaAccEventArgs>(this.OnJavaControlChanged);
            }
        }

        private void InstallAccEventListener_TimerCallback(object state)
        {
            this.SynchronizationContext.Post(new System.Threading.SendOrPostCallback(this.InstallAccEventListener_SendOrPostCallback), null);
        }

        private void OnJavaAccEventOccurred(object sender, JavaAccEventArgs e)
        {
            Trace.Write("__JavaAccEvent:");
            Trace.WriteLine(e.ToString());
        }

        private void OnJavaControlChanged(object sender, JavaAccEventArgs e)
        {
            //if (this.KnownControls.IsKnown(e.Source, e.VMachineId))
            //{
            //    string controlName = this.KnownControls.GetControlName(e.Source, e.VMachineId);
            //    if (!this.KnownControls.IsControlEventListed(controlName, e.EventTypeName))
            //    {
            //        this.KnownControls.AddControlEvents(controlName, e.EventTypeName);
            //        base.RaiseEvent(sender, e.EventTypeName, controlName, string.Empty);
            //    }
            //    else
            //    {
            //        System.DateTime time = System.Convert.ToDateTime(this.KnownControls.GetControlEventDateTime(controlName, e.EventTypeName), System.Globalization.CultureInfo.InvariantCulture);
            //        if (System.DateTime.Compare(time, System.DateTime.Now) != 0)
            //        {
            //            bool flag = false;
            //            if (time.AddMilliseconds(this._DifferenceInMilliSeconds).CompareTo(System.DateTime.Now) == -1)
            //            {
            //                flag = true;
            //            }
            //            this.KnownControls.UpdateControlEvents(controlName, e.EventTypeName);
            //            if (flag)
            //            {
            //                base.RaiseEvent(sender, e.EventTypeName, controlName, string.Empty);
            //            }
            //        }
            //    }
            //}
        }

        //protected override string OperationHandler(OperationType op, string controlName, string controlValue, string data)
        //{
        //    string str2;
        //    XmlNode controlConfig = base.GetControlConfig(controlName);
        //    string str = null;
        //    if (controlConfig != null)
        //    {
        //        str = controlConfig.Name;
        //    }
        //    try
        //    {
        //        string str3 = str;
        //        if (str3 == null)
        //        {
        //            throw new DataDrivenAdapterException("Named control config not found");
        //        }
        //        if (str3 == "JAccControl")
        //        {
        //            return this.AccControl(op, controlName, controlValue);
        //        }
        //        if (str3 == "JAccSelector")
        //        {
        //            return this.AccSelector(op, controlName, controlValue);
        //        }
        //        if (str3 != "JAccTree")
        //        {
        //            throw new DataDrivenAdapterException("Unsupported control type");
        //        }
        //        str2 = this.AccTree(op, controlName, controlValue);
        //    }
        //    catch (System.Exception exception)
        //    {
        //        if (exception is DataDrivenAdapterException)
        //        {
        //            throw;
        //        }
        //        throw new DataDrivenAdapterException(op, controlName, exception);
        //    }
        //    return str2;
        //}

        //public override bool RegisterEventListener(string eventName, string controlName, System.EventHandler<ControlChangedEventArgs> listenerCallback, string data)
        //{
        //    bool flag = false;
        //    if ((((eventName.Equals(JavaDataDrivenAdapterConstants.ButtonPressedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.ButtonReleasedEventName, System.StringComparison.OrdinalIgnoreCase)) || (eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonClearedEventName, System.StringComparison.OrdinalIgnoreCase))) || ((eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxClearedEventName, System.StringComparison.OrdinalIgnoreCase)) || (eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName, System.StringComparison.OrdinalIgnoreCase)))) || ((eventName.Equals(JavaDataDrivenAdapterConstants.GotFocusEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.LostFocusEventName, System.StringComparison.OrdinalIgnoreCase)) || ((eventName.Equals(JavaDataDrivenAdapterConstants.MenuSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, System.StringComparison.OrdinalIgnoreCase)) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuCanceledEventName, System.StringComparison.OrdinalIgnoreCase))))
        //    {
        //        flag = base.RegisterEventListenerBase(eventName, controlName, listenerCallback);
        //        if (!flag)
        //        {
        //            return flag;
        //        }
        //        if ((controlName != null) && !this.OperationHandler(OperationType.FindControl, controlName, null).Equals(bool.TrueString, System.StringComparison.OrdinalIgnoreCase))
        //        {
        //            throw new DataDrivenAdapterException("Unable to find control on UI");
        //        }
        //    }
        //    return flag;
        //}

        //public override bool UnregisterEventListener(string eventName, string controlName, System.EventHandler<ControlChangedEventArgs> listenerCallback)
        //{
        //    bool flag = false;
        //    if ((((!eventName.Equals(JavaDataDrivenAdapterConstants.ButtonPressedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.ButtonReleasedEventName, System.StringComparison.OrdinalIgnoreCase)) && (!eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonClearedEventName, System.StringComparison.OrdinalIgnoreCase))) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxClearedEventName, System.StringComparison.OrdinalIgnoreCase)) && (!eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName, System.StringComparison.OrdinalIgnoreCase)))) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.GotFocusEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.LostFocusEventName, System.StringComparison.OrdinalIgnoreCase)) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.MenuSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, System.StringComparison.OrdinalIgnoreCase)) && !eventName.Equals(JavaDataDrivenAdapterConstants.MenuCanceledEventName, System.StringComparison.OrdinalIgnoreCase))))
        //    {
        //        return flag;
        //    }
        //    return base.UnregisterEventListenerBase(eventName, controlName, listenerCallback);
        //}
        #endregion

    }
}
