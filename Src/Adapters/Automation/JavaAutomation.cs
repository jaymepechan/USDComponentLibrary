using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.Uii.Desktop.SessionManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Xrm.Tooling.WebResourceUtility;
using Microsoft.Uii.HostedApplicationToolkit.DataDrivenAdapter;
using Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm.ApplicationVisualViewer;
using System.Windows;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public class JavaAutomationObject : IUSDAutomationObject
    {
        public int vmId = 0;
        public System.IntPtr zero = System.IntPtr.Zero;
        public string name = String.Empty;

        public JavaAutomationObject(string _name, int _vmId, System.IntPtr _zero)
        {
            vmId = _vmId;
            zero = _zero;
            name = _name;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    JavaAccHelperMethods.ReleaseObject(zero, vmId);
                    System.Windows.Forms.Application.DoEvents();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~JavaAutomationObject() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

    public class JavaAutomation : AutomationControl
    {
        AgentDesktopSession localSession;
        TraceLogger LogWriter;
        string ApplicationName;
        private System.Threading.Timer JavaAccEventListenerInstallTimer;
        IntPtr rootWindow = IntPtr.Zero;

        public JavaAutomation(IntPtr _rootWindow, string _ApplicationName, AgentDesktopSession _localSession, TraceLogger _LogWriter)
        {
            localSession = _localSession;
            LogWriter = _LogWriter;
            ApplicationName = _ApplicationName;
            rootWindow = _rootWindow;
        }

        public override void Initialize()
        {
            try
            {
                JavaAccNativeMethods.LoadJavaAccessBridge();
                JavaAccNativeMethods.Windows_run();
                JavaAccNativeMethods._IsWindowsAccessBridgeAvailable = true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
            }
            try
            {
                int num;
                JavaAccHelperMethods.GetAccFromWindow(rootWindow, out num);
                System.Windows.Forms.Application.DoEvents();
                Trace.WriteLine("Initializing the java dda instance..");
                JavaAccHelperMethods.InitializeJavaAccessBridge(true);
                //this.JavaAccEventListenerInstallTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.InstallAccEventListener_TimerCallback), null, (int)(-1), 0);
            }
            catch
            {
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    //if (this.JavaAccEventListenerInstallTimer != null)
                    //{
                    //    this.JavaAccEventListenerInstallTimer.Dispose();
                    //    this.JavaAccEventListenerInstallTimer = null;
                    //}
                    Trace.WriteLine("Unsubscribing from java control changed events..");
                    JavaAccEventListener.JavaControlChanged -= new System.EventHandler<JavaAccEventArgs>(this.OnJavaControlChanged);
                    JavaAccEventListener.FreeEventHandlers();
                }

                disposedValue = true;
            }
        }
        #endregion

        public bool Tracing { get; set; }

        public override IUSDAutomationObject GetAutomationObject(XmlNode control)
        {
            /*
            <Controls>
                <JAccControl name="clickme"> // this is the control
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
            if (control.Name != "JAccControl")
                throw new Exception("Invalid node type included: " + control.Name + ". Only JAccControl is valid.");

            string name = GetAttributeValue(control, "name", "");
            XmlNode path = control.SelectSingleNode("Path");
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            zero = this.JavaFindAccObj(name, out vmId, path.OuterXml);
            AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
            if (Tracing)
            {
                StringBuilder sb = new StringBuilder();
                if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, zero, out accContextInfo))
                {
                    sb.AppendLine("Java Control Found: " + name);
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
                    string[] stateList = accContextInfo.states.Split(',');
                }

                int count = JavaAccNativeMethods.getAccessibleActions(vmId, zero);
                sb.AppendLine("\tActions: " + count.ToString());
                for (int i = 0; i < count; i++)
                {
                    IntPtr actionname = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(JavaAccNativeMethods.ACTIONNAMESIZE);
                    if (!actionname.Equals((System.IntPtr)System.IntPtr.Zero) && JavaAccNativeMethods.getAccessibleActionItem(i, actionname, JavaAccNativeMethods.ACTIONNAMESIZE))
                    {
                        string action = System.Runtime.InteropServices.Marshal.PtrToStringUni(actionname);
                        sb.AppendLine("\t\t: " + action);
                    }
                }

                LogWriter.Log(sb.ToString(), TraceEventType.Verbose);
                Trace.WriteLine(sb.ToString());
            }
            return new JavaAutomationObject(name, vmId, zero);
        }

        public override LookupRequestItem GetValue(IUSDAutomationObject automationObject)
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
            if (!(automationObject is JavaAutomationObject))
                throw new Exception("Invalid automation object");
            string controlValue = String.Empty;
            controlValue = JavaAccHelperMethods.GetValue(((JavaAutomationObject)automationObject).zero, ((JavaAutomationObject)automationObject).vmId);
            return new LookupRequestItem(((JavaAutomationObject)automationObject).name, controlValue);
        }

        public override void Execute(IUSDAutomationObject automationObject, string action)
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            if (!(automationObject is JavaAutomationObject))
                throw new Exception("Invalid automation object");

            int failure;
            JavaAccHelperMethods.DoAction(((JavaAutomationObject)automationObject).zero, out failure, ((JavaAutomationObject)automationObject).vmId, false, action);
            System.Windows.Forms.Application.DoEvents();
        }

        public override void SetValue(IUSDAutomationObject automationObject, string value)
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
                throw new Exception("Java Bridge not available");
            if (!(automationObject is JavaAutomationObject))
                throw new Exception("Invalid automation object");

            JavaAccHelperMethods.SetValue(((JavaAutomationObject)automationObject).zero, ((JavaAutomationObject)automationObject).vmId, value);
            System.Windows.Forms.Application.DoEvents();
        }

        JavaTreeViewModel jtvm;
        ApplicationVisualViewer avv = null;
        Window w = new Window();
        public override void DisplayVisualTree()
        {
            if (!JavaAccNativeMethods._IsWindowsAccessBridgeAvailable)
            {
                JavaAccNativeMethods.LoadJavaAccessBridge();
                JavaAccNativeMethods.Windows_run();
                Trace.WriteLine("Initializing the java dda instance..");
                JavaAccHelperMethods.InitializeJavaAccessBridge(true);
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Application.DoEvents();
                System.Windows.Forms.Application.DoEvents();
            }
            int vmId = 0;
            System.IntPtr accFromWindow = JavaAccHelperMethods.GetAccFromWindow(rootWindow, out vmId);
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

        private void W_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            w = null;
            avv = null;
        }

        #region JAVA
        /*


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
        */
        protected System.IntPtr JavaFindAccObj(string controlName, out int vmId, string controlXml)
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
            System.IntPtr accFromWindow = JavaAccHelperMethods.GetAccFromWindow(rootWindow, out vmId);
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
            }
            //this.KnownControls.RegisterControl(controlName, accFromWindow, vmId);
            return accFromWindow;
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
            System.IntPtr hWndMainWindow = rootWindow;
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

        /*

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
        */

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

        /*

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
        */
        #endregion


    }
}
