/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using Microsoft.Uii.Csr.Win32Api;
using Microsoft.Uii.Desktop.Cti.Core;
using Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm;
using Microsoft.Xrm.Tooling.WebResourceUtility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Interop;

namespace Microsoft.USD.ComponentLibrary.Adapters
{

    public class SetParentBase : MicrosoftBase
    {
        protected enum WindowHostingMode
        {
            SETPARENT,
            DYNAMICPOSITIONING
        }
        protected WindowHostingMode hostingMode = WindowHostingMode.SETPARENT;

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public event System.EventHandler<EventArgs> WindowAttached;
        private string windowCaptionToLocate = String.Empty;
        protected HandleRef win;
        private WindowParenter _WindowParenter;
        private System.Threading.Timer timerFindApp;
        private WindowsFormsHost myhost = null;
        private Process proc;
        private IntPtr windowHandle = IntPtr.Zero;
        protected TraceLogger LogWriter = null;
        protected bool removeFrameAndCaption = true;
        protected bool removeFromTaskbar = true;
        protected bool removeSizingControls = true;
        protected bool removeSizingMenu = true;
        protected bool removeSystemMenu = true;
        DPHandler wp = null;

        /// <summary>
        /// UII Constructor 
        /// </summary>
        /// <param name="appID">ID of the application</param>
        /// <param name="appName">Name of the application</param>
        /// <param name="initString">Initializing XML for the application</param>
        public SetParentBase(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {
            LogWriter = new TraceLogger("Microsoft.USD.ComponentLibrary");
            this.SizeChanged += SetParentBrowser_SizeChanged;
            this.GotFocus += SetParentBase_GotFocus;
            //this.
        }

        void SetParentBase_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (wp != null)
                    wp.Activate();
                else if (win.Handle != IntPtr.Zero)
                {
                    Win32API.RECT rect = new Win32API.RECT();
                    Win32API.GetWindowRect(win.Handle, ref rect);
                    if (rect.left != 0 || rect.top != 0 || Math.Abs(rect.right - (int)this.ActualWidth) > 5 || Math.Abs(rect.bottom - (int)this.ActualHeight) > 5)
                        Win32API.SetWindowPos(win.Handle, IntPtr.Zero, 0, 0, (int)this.ActualWidth, (int)this.ActualHeight, (int)0x0040);
                }

            }
            catch
            {
            }           
        }

        void SetParentBrowser_SizeChanged(object sender, EventArgs e)
        {
            try
            {
                if (win.Handle != IntPtr.Zero)
                    Win32API.SetWindowPos(win.Handle, IntPtr.Zero, 0, 0, (int)this.ActualWidth, (int)this.ActualHeight, (int)0x0040);

            }
            catch
            {
            }
        }

        #region Attach App

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumWindows(EnumedWindow lpEnumFunc, Object lParam);
        private delegate bool EnumedWindow(IntPtr handleWindow, Object handles);

        private ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);
            return windowHandles;
        }

        protected virtual bool GetWindowHandle(IntPtr windowHandle, Object windowHandlesObj)
        {
            try
            { 
                ArrayList windowHandles = windowHandlesObj as ArrayList;
                Regex reg = new Regex(windowCaptionToLocate);
                if (reg.IsMatch(Win32API.GetWindowText(windowHandle)))
                {
                    windowHandles.Add(windowHandle);
                }
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
                return false; 
            }
        }

        public class WindowMetadata
        {
            public string name;
            public IntPtr handle;
        }
        string getWindowMetadataRegex = String.Empty;
        protected List<WindowMetadata> GetWindowMetaData(string regex)
        {
            List< WindowMetadata> windowData = new List<WindowMetadata>();
            EnumedWindow callBackPtr = GetWindowMetaDataCallback;
            getWindowMetadataRegex = regex;
            EnumWindows(callBackPtr, windowData);
            return windowData;
        }
        private bool GetWindowMetaDataCallback(IntPtr windowHandle, Object windowObjs)
        {
            try
            {
                List<WindowMetadata> windowMetaObjs = windowObjs as List<WindowMetadata>;
                Regex reg = new Regex(getWindowMetadataRegex);
                string windowname = Win32API.GetWindowText(windowHandle);
                if (reg.IsMatch(windowname))
                {
                    windowMetaObjs.Add(new WindowMetadata() { handle = windowHandle, name = windowname });
                }
                return true;
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
                return false;   // try next one anyway
            }
        }
        #endregion

        protected void LookForAndAttachApplicationWindow(string windowRegex, WindowsFormsHost myhost)
        {
            CloseWindow();
            this.myhost = myhost;
            windowCaptionToLocate = windowRegex;
            timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(LocateApplicationWindow), null, TimeSpan.FromMilliseconds(100), TimeSpan.Zero);
        }

        protected void ProcessAttachApplicationWindow(Process p, WindowsFormsHost myhost)
        {
            CloseWindow();
            this.myhost = myhost;
            this.proc = p;
            timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessApplicationWindow), null, TimeSpan.FromMilliseconds(100), TimeSpan.Zero);
        }
        protected void WindowAttachApplicationWindow(IntPtr p, WindowsFormsHost myhost)
        {
            CloseWindow();
            this.myhost = myhost;
            this.windowHandle = p;
            win = new HandleRef(this, p);
            timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessWindowAttach), null, TimeSpan.FromMilliseconds(100), TimeSpan.Zero);
        }

        protected void UseNoAttachApplicationWindow(IntPtr p, WindowsFormsHost myhost)
        {
            CloseWindow();
            this.myhost = myhost;
            this.windowHandle = p;
            win = new HandleRef(this, p);
                    if (WindowAttached != null)
                        WindowAttached(this, EventArgs.Empty);
        }

        protected void DisplayVisualTree(IntPtr p, WindowsFormsHost myhost)
        {
            CloseWindow();
            this.myhost = myhost;
            this.windowHandle = p;
            win = new HandleRef(this, p);
                    if (WindowAttached != null)
                        WindowAttached(this, EventArgs.Empty);
        }

        protected void DetachWindow()
        {
            if (hostingMode == WindowHostingMode.SETPARENT)
            {
                SetParent(win.Handle, IntPtr.Zero);
                NativeMethods.SetWindowLong(win.Handle, NativeMethods.WinUserConstant.GWL_STYLE, oldWindowStyle);
            }
            if (_WindowParenter != null)
            {
                _WindowParenter.Dispose();
                win = new HandleRef();
            }
        }

        protected void CloseWindow()
        {
            if (timerFindApp != null)
            {
                timerFindApp.Dispose();
                timerFindApp = null;
            }
            if (_WindowParenter != null)
            {
                _WindowParenter.Dispose();
                _WindowParenter = null;
            }
            Win32API.CloseWindow(win.Handle);
            win = new HandleRef();
        }

        public override void Close()
        {
            base.Close();

            if (timerFindApp != null)
            {
                timerFindApp.Dispose();
                timerFindApp = null;
                CloseWindow();
            }
        }

        private void ProcessWindowAttach(object obj)
        {
            try
            {
                timerFindApp.Dispose();
                if (myhost.Handle == IntPtr.Zero)
                {
                    LogWriter.Log("Host handle is 0: Waiting for creation but hiding app", TraceEventType.Verbose);
                    Win32API.ShowWindow(win.Handle, Win32API.ShowWindowValues.SW_HIDE);
                    timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(ProcessWindowAttach), null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
                }
                else
                {
                    ParentWindow();
                }
            }
            catch (Exception ex)
            {
                LogWriter.Log(ex);
            }
        }
        
        private void ProcessApplicationWindow(object obj)
        {
            timerFindApp.Dispose();
            this.Dispatcher.Invoke(() =>
            {
                if (proc.MainWindowHandle != null)
                    Win32API.ShowWindow(proc.MainWindowHandle, Win32API.ShowWindowValues.SW_HIDE);
                if (proc.MainWindowHandle == null || myhost.Handle == IntPtr.Zero)
                    timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(LocateApplicationWindow), null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
                else if (proc.MainWindowHandle != null)
                {
                    win = new HandleRef(this, proc.MainWindowHandle);
                    ParentWindow();
                }
            });
        }

        private void LocateApplicationWindow(object obj)
        {
            timerFindApp.Dispose();
            this.Dispatcher.Invoke(() =>
            {

                ArrayList wins = GetWindows();
                if (wins.Count == 0 || myhost.Handle == IntPtr.Zero)
                    timerFindApp = new System.Threading.Timer(new System.Threading.TimerCallback(LocateApplicationWindow), null, TimeSpan.FromSeconds(1), TimeSpan.Zero);
                else if (wins.Count > 0)
                {
                    win = new HandleRef(this, (IntPtr)wins[0]);
                    ParentWindow();
                }
            });
        }
        uint oldWindowStyle = 0;
        private void ParentWindow()
        {
            LogWriter.Log("ParentWindow", TraceEventType.Verbose);
            if (win.Handle == IntPtr.Zero)
            {
                LogWriter.Log("\tWindow handle is 0: quitting", TraceEventType.Error);
                return;
            }
            Dispatcher.Invoke(() =>
            {
                try
                {
                    hookupWinMsgDetection();

                    List<LookupRequestItem> lri = new List<LookupRequestItem>();
                    lri.Add(new LookupRequestItem("Title", Win32API.GetWindowText(win.Handle)));
                    ((DynamicsCustomerRecord)localSession.Customer.DesktopCustomer).MergeReplacementParameter(this.ApplicationName, lri, true);

                    Window _RootWindow = System.Windows.Application.Current.MainWindow;
                    WindowInteropHelper wrapper = new WindowInteropHelper(_RootWindow);
                    HandleRef shellWindow = new System.Runtime.InteropServices.HandleRef(wrapper, wrapper.Handle);

                    if (removeFrameAndCaption)
                        RemoveFrameAndCaption(win);
                    if (removeFromTaskbar)
                        RemoveFromTaskbar(win);
                    if (removeSizingControls)
                        RemoveSizingControls(win);
                    if (removeSizingMenu)
                        RemoveSizingMenu(win);
                    if (removeSystemMenu)
                        RemoveSystemMenu(win);
                    RestoreIfMinimized(win);

                    if (hostingMode == WindowHostingMode.SETPARENT)
                    {
                        LogWriter.Log("\tSetting Window Position Width:" + this.ActualWidth.ToString() + " Height:" + this.ActualHeight.ToString(), TraceEventType.Verbose);
                        Win32API.SetWindowPos(win.Handle, IntPtr.Zero, 0, 0, (int)this.ActualWidth, (int)this.ActualHeight, (int)0x0040);

                        LogWriter.Log("\tReparenting", TraceEventType.Verbose);
                        // parenting the window makes the automation interface
                        //_WindowParenter = new WindowParenter(new HandleRef(this, myhost.Handle), win, true, false);
                        // JAYME: I chose to use the below because knowledge of the return value of SetParent is useful
                        //        but it gets hidden in WindowParenter
                        IntPtr prevParent = SetParent(win.Handle, myhost.Handle);
                        if (prevParent != IntPtr.Zero)
                        {
                            LogWriter.Log("\tReparenting complete. " + prevParent.ToString(), TraceEventType.Verbose);
                            // CLEAR WS_POPUP and SET WS_CHILD
                            // WS_POPUP     0x80000000L
                            // WS_CHILD     0x40000000L
                            uint dwNewLong = NativeMethods.GetWindowLong(win.Handle, NativeMethods.WinUserConstant.GWL_STYLE);
                            oldWindowStyle = dwNewLong;
                            dwNewLong ^= 0x80000000;
                            dwNewLong |= 0x40000000;
                            NativeMethods.SetWindowLong(win.Handle, NativeMethods.WinUserConstant.GWL_STYLE, dwNewLong);
                            LogWriter.Log("\tAdjusted WS_POPUP and WS_CHILD.", TraceEventType.Verbose);
                        }
                        else
                        {
                            LogWriter.Log("\tReparenting complete. NULL", TraceEventType.Verbose);
                            int errorResult = Marshal.GetLastWin32Error();
                            LogWriter.Log("\tReparenting error. " + errorResult.ToString(), TraceEventType.Verbose);
                        }
                    }
                    else if (hostingMode == WindowHostingMode.DYNAMICPOSITIONING)
                    {
                        wp = new DPHandler(myhost, win, true);
                        RestoreIfMinimized(win);
                        wp.Activate();
                    }


                    if (WindowAttached != null)
                        WindowAttached(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    LogWriter.Log(ex);
                }
            });
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(HandleRef hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DeleteMenu(HandleRef hMenu, uint uPosition, uint uFlags);
        private static void ChangeWindowExStyles(HandleRef hWnd, int setExStyles, int clearExStyles)
        {
            SetWindowLong(hWnd, -20, (GetWindowLong(hWnd, -20) | setExStyles) & ~clearExStyles);
        }

        private static void ChangeWindowStyles(HandleRef hWnd, int setStyles, int clearStyles)
        {
            SetWindowLong(hWnd, -16, (GetWindowLong(hWnd, -16) | setStyles) & ~clearStyles);
        }

        public void RemoveFrameAndCaption(HandleRef hwnd)
        {
            ChangeWindowStyles(hwnd, 0, -2134638592);
        }

        public void RemoveFromTaskbar(HandleRef hwnd)
        {
            ChangeWindowExStyles(hwnd, 0x80, 0x40000);
        }

        public void RemoveSizingControls(HandleRef hwnd)
        {
            ChangeWindowStyles(hwnd, 0, 0x30000);
        }

        public void RemoveSizingMenu(HandleRef hwnd)
        {
            HandleRef hMenu = new HandleRef(this, GetSystemMenu(hwnd, false));
            DeleteMenu(hMenu, 0xf030, 0);
            DeleteMenu(hMenu, 0xf020, 0);
            DeleteMenu(hMenu, 0xf000, 0);
            DeleteMenu(hMenu, 0xf010, 0);
        }

        public void RemoveSystemMenu(HandleRef hwnd)
        {
            HandleRef hMenu = new HandleRef(this, GetSystemMenu(hwnd, false));
            DeleteMenu(hMenu, 0xf060, 0);
            DeleteMenu(hMenu, 0xf00f, 0);
            DeleteMenu(hMenu, 0xf030, 0);
            DeleteMenu(hMenu, 0xf020, 0);
            DeleteMenu(hMenu, 0xf000, 0);
            DeleteMenu(hMenu, 0xf010, 0);
            DeleteMenu(hMenu, 0xf120, 0);
        }

        public void RestoreIfMinimized(HandleRef hwnd)
        {
            if ((GetWindowLong(hwnd, -16) & 0x20000000) != 0)
            {
                Win32API.ShowWindow(hwnd.Handle, Win32API.ShowWindowValues.SW_RESTORE);
            }
        }
        #region Window Hooks
        HwndSource hwndSource = null;
        List<HwndSource> hookedWindows = new List<HwndSource>();
        HwndSourceHook hwndSourceHook = null;
        private void hookupWinMsgDetection()
        {
            try
            {
                hwndSource = PresentationSource.FromVisual(this) as HwndSource;
                if (hwndSource == null)
                    return;
                if (hookedWindows.Contains(hwndSource))
                {
                    Debug.WriteLine("hookupCloseDetection: Hook already found for this HwndSource");
                    return;
                }
                hookedWindows.Add(hwndSource);
                if (hwndSourceHook == null)
                    hwndSourceHook = new HwndSourceHook(WndProc);
                hwndSource.AddHook(hwndSourceHook);
            }
            catch
            {
            }
        }

        private void unHookupWinMsgDetection()
        {
            try
            {
                if (hwndSource != null && hookedWindows.Contains(hwndSource))
                {
                    hwndSource.RemoveHook(hwndSourceHook);
                    hookedWindows.Remove(hwndSource);
                    hwndSourceHook = null;
                }
            }
            catch
            {
            }
        }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
            unHookupWinMsgDetection();
            hookupWinMsgDetection();
        }

        delegate IntPtr WndProcDelegate(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            //Trace.WriteLine("WndProc message: " + msg.ToString());
            if (msg == WM_PARENTNOTIFY)
            {
                try
                {
                    if (wParam.ToInt32() == WM_DESTROY)
                    {

                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("WndProc exception: " + ex.Message + "\r\n" + ex.StackTrace);
                }
            }
            return IntPtr.Zero;
        }

        public const int WM_PARENTNOTIFY = 0x0210;
        public const int WM_DESTROY = 2;
        public delegate void ClosingEventHandler();
        public event ClosingEventHandler Closing;
        #endregion
    }
}
