/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Uii.Csr.Browser.Web;
using Microsoft.Uii.HostedApplicationToolkit.DataDrivenAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.Java
{
    public class JavaDDA : DataDrivenAdapterBase
    {
        private double _DifferenceInMilliSeconds;
        private int _DisableFor;
        private bool _IsApplet;
        private bool _IsIgnoreVersionCheck;
        private static bool _IsWindowsAccessBridgeAvailable = false;
        private JavaKnownControls _KnownControls;
        private static bool _NeedToCallWindowsRun;
        private System.IntPtr hWndMainWindow;
        private System.Threading.Timer JavaAccEventListenerInstallTimer;
        private System.Threading.SynchronizationContext SynchronizationContext;

        public JavaDDA(XmlDocument appInitStr, object appObj) : base(appInitStr)
        {
            int num;
            this._KnownControls = new JavaKnownControls();
            this._DifferenceInMilliSeconds = 100.0;
            this._IsApplet = (bool)(appInitStr.SelectSingleNode("//DataDrivenAdapterBindings/Options/Applet") != null);
            this._IsIgnoreVersionCheck = (bool)(appInitStr.SelectSingleNode("//DataDrivenAdapterBindings/Options/IgnoreVersionCheck") != null);
            WebBrowserExtended extended = appObj as WebBrowserExtended;
            if (extended == null)
            {
                try
                {
                    this.hWndMainWindow = (System.IntPtr)((System.IntPtr)appObj);
                    goto Label_0086;
                }
                catch (System.InvalidCastException)
                {

                }
            }
            this.hWndMainWindow = extended.Handle;
            this._IsApplet = true;
        Label_0086:
            try
            {
                if (!_IsWindowsAccessBridgeAvailable)
                {
                    //if (System.Environment.Is64BitOperatingSystem)
                    //{
                    //    bool lpSystemInfo = false;
                    //    Win32NativeMethods.IsWow64Process(Process.GetCurrentProcess().get_Handle(), out lpSystemInfo);
                    //    if (JavaAccHelperMethods.IsWow64ProcessMode(this.hWndMainWindow) && lpSystemInfo)
                    //    {
                    //        JavaAccNativeMethods.LoadJavaAccessBridge("windowsaccessbridge-32.dll");
                    //    }
                    //    else                    //    {
                    //        JavaAccNativeMethods.LoadJavaAccessBridge("windowsaccessbridge-64.dll");
                    //    }
                    //}
                    //else
                    //{
                        JavaAccNativeMethods.LoadJavaAccessBridge();
                    //}
                }
                JavaAccNativeMethods.Windows_run();
                _IsWindowsAccessBridgeAvailable = true;
            }
            catch (System.DllNotFoundException)
            {
                _IsWindowsAccessBridgeAvailable = false;
                throw new DataDrivenAdapterException("Java Bridge not Found");
            }
            catch (Win32Exception)
            {
                _IsWindowsAccessBridgeAvailable = false;
                throw new DataDrivenAdapterException("Java Bridge not Found");
            }
            catch (DataDrivenAdapterException)
            {
                throw;
            }
            if (this._IsApplet)
            {
                int num2;
                int windowThreadProcessId = Win32NativeMethods.GetWindowThreadProcessId(this.hWndMainWindow, out num2);
                JavaAccHelperMethods.GetAccFromWindow(Win32HelperMethods.FindWindowByCaptionAndClassText(this.hWndMainWindow, num2, windowThreadProcessId, null, FindWindowMatchType.Ignore, JavaDataDrivenAdapterConstants.SUN_AWT_FRAME_CLASS, FindWindowMatchType.Equals, 1, true), out num);
            }
            else
            {
                JavaAccHelperMethods.GetAccFromWindow(this.hWndMainWindow, out num);
            }
            System.Windows.Forms.Application.DoEvents();
            Trace.WriteLine("Initializing the java dda instance..");
            JavaAccHelperMethods.InitializeJavaAccessBridge(this._IsIgnoreVersionCheck);
            this.SynchronizationContext = System.Threading.SynchronizationContext.Current;
            XmlNode node = appInitStr.SelectSingleNode("//DataDrivenAdapterBindings/Options/DisableAccEventListener");
            this._DisableFor = (node == null) ? ((int)0) : DataDrivenAdapterBase.GetAttributeValue(node, "disableFor", -1);
            this.JavaAccEventListenerInstallTimer = new System.Threading.Timer(new System.Threading.TimerCallback(this.InstallAccEventListener_TimerCallback), null, (this._DisableFor >= 0) ? this._DisableFor : ((int)(-1)), 0);
        }

        private string AccControl(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            try
            {
                int num2;
                switch (op)
                {
                    case OperationType.FindControl:
                        zero = this.FindAccObj(controlName, out vmId, false);
                        if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;

                    case OperationType.GetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        return JavaAccHelperMethods.GetValue(zero, vmId);

                    case OperationType.SetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.SetValue(zero, vmId, controlValue);
                        return null;

                    case OperationType.ExecuteControlAction:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        if (!System.Threading.Thread.CurrentThread.CurrentCulture.Name.StartsWith(JavaDataDrivenAdapterConstants.FRENCH_CULTURE_TEXT, System.StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                        JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.FRENCH_DEFAULT_ACTION_NAME);
                        goto Label_00D6;

                    default:
                        goto Label_00E4;
                }
                JavaAccHelperMethods.DoAction(zero, out num2, vmId, true, string.Empty);
            Label_00D6:
                return null;
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(zero, vmId);
            }
        Label_00E4:
            return null;
        }

        private string AccHyperlink(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            try
            {
                switch (op)
                {
                    case OperationType.FindControl:
                        zero = this.FindAccObj(controlName, out vmId, false);
                        if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;

                    case OperationType.GetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        return JavaAccHelperMethods.GetAccHyperlink(zero, vmId);

                    case OperationType.SetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.ActivateAccHyperlink(zero, vmId);
                        return null;

                    case OperationType.ExecuteControlAction:
                        int num2;
                        zero = this.FindAccObj(controlName, out vmId, true);
                        if (!JavaAccHelperMethods.DoAction(zero, out num2, vmId, true, string.Empty))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;
                }
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(zero, vmId);
            }
            return null;
        }

        private string AccSelector(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            try
            {
                switch (op)
                {
                    case OperationType.FindControl:
                        zero = this.FindAccObj(controlName, out vmId, false);
                        if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;

                    case OperationType.GetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        return JavaAccHelperMethods.GetAccSelectionName(zero, vmId);

                    case OperationType.SetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.SetAccSelection(zero, vmId, controlValue);
                        return null;

                    case OperationType.ExecuteControlAction:
                        int num2;
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.TOGGLE_POPUP_ACTION_NAME);
                        return null;
                }
            }
            catch (System.ArgumentException)
            {
                throw new DataDrivenAdapterException("Incorrect control value format");
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(zero, vmId);
            }
            return null;
        }

        private string AccTable(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            try
            {
                switch (op)
                {
                    case OperationType.FindControl:
                        zero = this.FindAccObj(controlName, out vmId, false);
                        if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;

                    case OperationType.GetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        return JavaAccHelperMethods.GetAccTableCellValue(zero, vmId, controlValue);

                    case OperationType.SetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.SetAccTableCellValue(zero, vmId, controlName, controlValue);
                        return null;
                }
            }
            catch (System.ArgumentException)
            {
                throw new DataDrivenAdapterException("Incorrect control value format");
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(zero, vmId);
            }
            return null;
        }

        private string AccTree(OperationType op, string controlName, string controlValue)
        {
            int vmId = 0;
            System.IntPtr zero = System.IntPtr.Zero;
            try
            {
                switch (op)
                {
                    case OperationType.FindControl:
                        zero = this.FindAccObj(controlName, out vmId, false);
                        if (zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            return bool.FalseString;
                        }
                        return bool.TrueString;

                    case OperationType.GetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        return JavaAccHelperMethods.GetAccSelectionName(zero, vmId);

                    case OperationType.SetControlValue:
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.SetAccSelection(zero, vmId, controlValue);
                        return null;

                    case OperationType.ExecuteControlAction:
                        int num2;
                        zero = this.FindAccObj(controlName, out vmId, true);
                        JavaAccHelperMethods.DoAction(zero, out num2, vmId, false, JavaDataDrivenAdapterConstants.TOGGLE_EXPAND_ACTION_NAME);
                        return null;
                }
            }
            catch (System.ArgumentException)
            {
                throw new DataDrivenAdapterException("Incorrect control value format");
            }
            finally
            {
                JavaAccHelperMethods.ReleaseObject(zero, vmId);
            }
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.JavaAccEventListenerInstallTimer != null)
                {
                    this.JavaAccEventListenerInstallTimer.Dispose();
                    this.JavaAccEventListenerInstallTimer = null;
                }
                if (this._DisableFor >= 0)
                {
                    Trace.WriteLine("Unsubscribing from java control changed events..");
                    JavaAccEventListener.JavaControlChanged -= new System.EventHandler<JavaAccEventArgs>(this.OnJavaControlChanged);
                    JavaAccEventListener.FreeEventHandlers();
                }
                _NeedToCallWindowsRun = true;
            }
            base.Dispose(disposing);
        }

        protected System.IntPtr FindAccObj(string controlName, out int vmId, bool throwExceptionIfNotFound)
        {
            XmlNode firstDescendentUnderControlConfig = base.GetFirstDescendentUnderControlConfig(controlName, "Path");
            if (firstDescendentUnderControlConfig == null)
            {
                throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "No Path element found");
            }
            if (_NeedToCallWindowsRun)
            {
                JavaAccNativeMethods.Windows_run();
                _NeedToCallWindowsRun = false;
                System.Windows.Forms.Application.DoEvents();
            }
            System.IntPtr accFromWindow = JavaAccHelperMethods.GetAccFromWindow(this.hWndMainWindow, out vmId);
            string defaultValue = System.Threading.Thread.CurrentThread.CurrentUICulture.ToString();
            for (XmlNode node2 = firstDescendentUnderControlConfig.FirstChild; node2 != null; node2 = node2.NextSibling)
            {
                int num;
                int num2;
                bool flag;
                switch (node2.Name)
                {
                    case "FindWindow":
                        accFromWindow = JavaAccHelperMethods.GetAccFromWindow(this.FindWindowFromControlName(controlName, throwExceptionIfNotFound), out vmId);
                        break;

                    case "Next":
                    case "NextName":
                        num = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        num2 = DataDrivenAdapterBase.GetAttributeValue(node2, "offset", 0);
                        flag = false;
                        if (DataDrivenAdapterBase.GetAttributeValue(node2, "culture", defaultValue).Equals(defaultValue, System.StringComparison.OrdinalIgnoreCase))
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
                        num = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        num2 = DataDrivenAdapterBase.GetAttributeValue(node2, "offset", 0);
                        flag = false;
                        if (DataDrivenAdapterBase.GetAttributeValue(node2, "culture", defaultValue).Equals(defaultValue, System.StringComparison.OrdinalIgnoreCase))
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
                if (throwExceptionIfNotFound)
                {
                    throw new DataDrivenAdapterException("Unable to find control on UI");
                }
                return accFromWindow;
            }
            this.KnownControls.RegisterControl(controlName, accFromWindow, vmId);
            return accFromWindow;
        }

        protected System.IntPtr FindWindowFromControlName(string controlName, bool throwExceptionIfNotFound)
        {
            int num;
            XmlNode firstDescendentUnderControlConfig = base.GetFirstDescendentUnderControlConfig(controlName, "FindWindow");
            if (firstDescendentUnderControlConfig == null)
            {
                throw new DataDrivenAdapterException(OperationType.FindControl, controlName, "No find window element found");
            }
            int windowThreadProcessId = Win32NativeMethods.GetWindowThreadProcessId(this.hWndMainWindow, out num);
            System.IntPtr hWndMainWindow = this.hWndMainWindow;
            for (XmlNode node2 = firstDescendentUnderControlConfig.FirstChild; node2 != null; node2 = node2.NextSibling)
            {
                int num3;
                FindWindowMatchType type;
                string str2;
                string str3;
                FindWindowMatchType ignore;
                FindWindowMatchType type3;
                XmlNode nextSibling;
                switch (node2.Name)
                {
                    case "ControlID":
                    case "ControlId":
                        int num4;
                        num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        int.TryParse(node2.InnerText, System.Globalization.NumberStyles.HexNumber, null, out num4);
                        hWndMainWindow = Win32HelperMethods.FindWindowByControlId(hWndMainWindow, num, windowThreadProcessId, num4, num3, this._IsApplet);
                        goto Label_045C;

                    case "Caption":
                    case "CaptionStartsWith":
                    case "CaptionEndsWith":
                    case "CaptionContains":
                        num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        type = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(DataDrivenAdapterBase.GetAttributeValue(node2, "matchtype", string.Empty));
                        hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, node2.InnerText, type, null, FindWindowMatchType.Ignore, num3, this._IsApplet);
                        goto Label_045C;

                    case "Class":
                    case "ClassStartsWith":
                    case "ClassEndsWith":
                    case "ClassContains":
                        num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        type = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(DataDrivenAdapterBase.GetAttributeValue(node2, "matchtype", string.Empty));
                        hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, null, FindWindowMatchType.Ignore, node2.InnerText, type, num3, this._IsApplet);
                        goto Label_045C;

                    case "Position":
                        {
                            int num5;
                            int num6;
                            string[] strArray = node2.InnerText.Replace(',', ' ').Split((char[])new char[0]);
                            int.TryParse((strArray.Length > 0) ? strArray[0] : ((string)"0"), out num5);
                            int.TryParse((strArray.Length > 1) ? strArray[1] : ((string)"0"), out num6);
                            hWndMainWindow = Win32HelperMethods.FindWindowByPosition(hWndMainWindow, num, windowThreadProcessId, num5, num6, this._IsApplet);
                            goto Label_045C;
                        }
                    case "Find":
                        num3 = DataDrivenAdapterBase.GetAttributeValue(node2, "match", 1);
                        str2 = null;
                        str3 = null;
                        ignore = FindWindowMatchType.Ignore;
                        type3 = FindWindowMatchType.Ignore;
                        nextSibling = node2.FirstChild;
                        goto Label_040C;

                    case "Application":
                        hWndMainWindow = this.hWndMainWindow;
                        goto Label_045C;

                    case "Desktop":
                        hWndMainWindow = Win32NativeMethods.GetDesktopWindow();
                        goto Label_045C;

                    case "Owner":
                        hWndMainWindow = Win32NativeMethods.GetWindow(hWndMainWindow, WinUserConstant.SW_SHOWNOACTIVATE);
                        goto Label_045C;

                    case "RelaxProcessIdRestriction":
                        num = 0;
                        goto Label_045C;

                    case "RelaxThreadIdRestriction":
                        windowThreadProcessId = 0;
                        goto Label_045C;

                    default:
                        throw new DataDrivenAdapterException("Unsupported FindWindow element");
                }
            Label_0307:
                switch (nextSibling.Name)
                {
                    case "Caption":
                    case "CaptionStartsWith":
                    case "CaptionEndsWith":
                    case "CaptionContains":
                        str2 = nextSibling.InnerText;
                        ignore = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(nextSibling.Name.Substring(7));
                        break;

                    case "Class":
                    case "ClassStartsWith":
                    case "ClassEndsWith":
                    case "ClassContains":
                        str3 = nextSibling.InnerText;
                        type3 = Win32HelperMethods.DetermineFindWindowMatchTypeFromText(nextSibling.Name.Substring(5));
                        break;
                }
                nextSibling = nextSibling.NextSibling;
            Label_040C:
                if (nextSibling != null)
                {
                    goto Label_0307;
                }
                hWndMainWindow = Win32HelperMethods.FindWindowByCaptionAndClassText(hWndMainWindow, num, windowThreadProcessId, str2, ignore, str3, type3, num3, this._IsApplet);
            Label_045C:
                if (hWndMainWindow.Equals((System.IntPtr)System.IntPtr.Zero))
                {
                    break;
                }
            }
            if (hWndMainWindow.Equals((System.IntPtr)System.IntPtr.Zero) && throwExceptionIfNotFound)
            {
                throw new DataDrivenAdapterException("Unable to find window");
            }
            return hWndMainWindow;
        }

        public override System.Collections.ObjectModel.ReadOnlyCollection<string> GetAvailableControlEvents()
        {
            return new System.Collections.ObjectModel.ReadOnlyCollection<string>(new string[] { JavaDataDrivenAdapterConstants.ButtonPressedEventName, JavaDataDrivenAdapterConstants.ButtonReleasedEventName, JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName, JavaDataDrivenAdapterConstants.RadioButtonClearedEventName, JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName, JavaDataDrivenAdapterConstants.CheckBoxClearedEventName, JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName, JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName, JavaDataDrivenAdapterConstants.GotFocusEventName, JavaDataDrivenAdapterConstants.LostFocusEventName, JavaDataDrivenAdapterConstants.MenuSelectedEventName, JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, JavaDataDrivenAdapterConstants.MenuCanceledEventName });
        }

        private void InstallAccEventListener_SendOrPostCallback(object state)
        {
            if (this.JavaAccEventListenerInstallTimer != null)
            {
                Trace.WriteLine("Subscribing to java control changed events..");
                JavaAccEventListener.SetEventHandlers();
                JavaAccEventListener.JavaControlChanged += new System.EventHandler<JavaAccEventArgs>(this.OnJavaControlChanged);
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
            if (this.KnownControls.IsKnown(e.Source, e.VMachineId))
            {
                string controlName = this.KnownControls.GetControlName(e.Source, e.VMachineId);
                if (!this.KnownControls.IsControlEventListed(controlName, e.EventTypeName))
                {
                    this.KnownControls.AddControlEvents(controlName, e.EventTypeName);
                    base.RaiseEvent(sender, e.EventTypeName, controlName, string.Empty);
                }
                else
                {
                    System.DateTime time = System.Convert.ToDateTime(this.KnownControls.GetControlEventDateTime(controlName, e.EventTypeName), System.Globalization.CultureInfo.InvariantCulture);
                    if (System.DateTime.Compare(time, System.DateTime.Now) != 0)
                    {
                        bool flag = false;
                        if (time.AddMilliseconds(this._DifferenceInMilliSeconds).CompareTo(System.DateTime.Now) == -1)
                        {
                            flag = true;
                        }
                        this.KnownControls.UpdateControlEvents(controlName, e.EventTypeName);
                        if (flag)
                        {
                            base.RaiseEvent(sender, e.EventTypeName, controlName, string.Empty);
                        }
                    }
                }
            }
        }

        protected override string OperationHandler(OperationType op, string controlName, string controlValue, string data)
        {
            string str2;
            XmlNode controlConfig = base.GetControlConfig(controlName);
            string str = null;
            if (controlConfig != null)
            {
                str = controlConfig.Name;
            }
            try
            {
                string str3 = str;
                if (str3 == null)
                {
                    throw new DataDrivenAdapterException("Named control config not found");
                }
                if (str3 == "JAccControl")
                {
                    return this.AccControl(op, controlName, controlValue);
                }
                if (str3 == "JAccSelector")
                {
                    return this.AccSelector(op, controlName, controlValue);
                }
                if (str3 != "JAccTree")
                {
                    throw new DataDrivenAdapterException("Unsupported control type");
                }
                str2 = this.AccTree(op, controlName, controlValue);
            }
            catch (System.Exception exception)
            {
                if (exception is DataDrivenAdapterException)
                {
                    throw;
                }
                throw new DataDrivenAdapterException(op, controlName, exception);
            }
            return str2;
        }

        public override bool RegisterEventListener(string eventName, string controlName, System.EventHandler<ControlChangedEventArgs> listenerCallback, string data)
        {
            bool flag = false;
            if ((((eventName.Equals(JavaDataDrivenAdapterConstants.ButtonPressedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.ButtonReleasedEventName, System.StringComparison.OrdinalIgnoreCase)) || (eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonClearedEventName, System.StringComparison.OrdinalIgnoreCase))) || ((eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxClearedEventName, System.StringComparison.OrdinalIgnoreCase)) || (eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName, System.StringComparison.OrdinalIgnoreCase)))) || ((eventName.Equals(JavaDataDrivenAdapterConstants.GotFocusEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.LostFocusEventName, System.StringComparison.OrdinalIgnoreCase)) || ((eventName.Equals(JavaDataDrivenAdapterConstants.MenuSelectedEventName, System.StringComparison.OrdinalIgnoreCase) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, System.StringComparison.OrdinalIgnoreCase)) || eventName.Equals(JavaDataDrivenAdapterConstants.MenuCanceledEventName, System.StringComparison.OrdinalIgnoreCase))))
            {
                flag = base.RegisterEventListenerBase(eventName, controlName, listenerCallback);
                if (!flag)
                {
                    return flag;
                }
                if ((controlName != null) && !this.OperationHandler(OperationType.FindControl, controlName, null).Equals(bool.TrueString, System.StringComparison.OrdinalIgnoreCase))
                {
                    throw new DataDrivenAdapterException("Unable to find control on UI");
                }
            }
            return flag;
        }

        public override bool UnregisterEventListener(string eventName, string controlName, System.EventHandler<ControlChangedEventArgs> listenerCallback)
        {
            bool flag = false;
            if ((((!eventName.Equals(JavaDataDrivenAdapterConstants.ButtonPressedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.ButtonReleasedEventName, System.StringComparison.OrdinalIgnoreCase)) && (!eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.RadioButtonClearedEventName, System.StringComparison.OrdinalIgnoreCase))) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.CheckBoxClearedEventName, System.StringComparison.OrdinalIgnoreCase)) && (!eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeExpandedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.TreeNodeCollapsedEventName, System.StringComparison.OrdinalIgnoreCase)))) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.GotFocusEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.LostFocusEventName, System.StringComparison.OrdinalIgnoreCase)) && ((!eventName.Equals(JavaDataDrivenAdapterConstants.MenuSelectedEventName, System.StringComparison.OrdinalIgnoreCase) && !eventName.Equals(JavaDataDrivenAdapterConstants.MenuDeSelectedEventName, System.StringComparison.OrdinalIgnoreCase)) && !eventName.Equals(JavaDataDrivenAdapterConstants.MenuCanceledEventName, System.StringComparison.OrdinalIgnoreCase))))
            {
                return flag;
            }
            return base.UnregisterEventListenerBase(eventName, controlName, listenerCallback);
        }

        public JavaKnownControls KnownControls
        {
            get
            {
                return this._KnownControls;
            }
        }
    }
}
