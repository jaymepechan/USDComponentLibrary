/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Uii.AifServices;
using Microsoft.Uii.Csr;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace Microsoft.USD.ComponentLibrary.Utilities
{
    public enum ABEdge : int
    {
        Left = 0,
        Top,
        Right,
        Bottom,
        None
    }

    internal static class AppBarFunctions
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct APPBARDATA
        {
            public int cbSize;
            public IntPtr hWnd;
            public int uCallbackMessage;
            public int uEdge;
            public RECT rc;
            public IntPtr lParam;
        }

        private enum ABMsg : int
        {
            ABM_NEW = 0,
            ABM_REMOVE,
            ABM_QUERYPOS,
            ABM_SETPOS,
            ABM_GETSTATE,
            ABM_GETTASKBARPOS,
            ABM_ACTIVATE,
            ABM_GETAUTOHIDEBAR,
            ABM_SETAUTOHIDEBAR,
            ABM_WINDOWPOSCHANGED,
            ABM_SETSTATE
        }
        private enum ABNotify : int
        {
            ABN_STATECHANGE = 0,
            ABN_POSCHANGED,
            ABN_FULLSCREENAPP,
            ABN_WINDOWARRANGE
        }

        [DllImport("SHELL32", CallingConvention = CallingConvention.StdCall)]
        private static extern uint SHAppBarMessage(int dwMessage, ref APPBARDATA pData);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int RegisterWindowMessage(string msg);

        private class RegisterInfo
        {
            public int CallbackId { get; set; }
            public bool IsRegistered { get; set; }
            public Window Window { get; set; }
            public ABEdge Edge { get; set; }
            public WindowStyle OriginalStyle { get; set; }
            public Point OriginalPosition { get; set; }
            public Size OriginalSize { get; set; }
            public ResizeMode OriginalResizeMode { get; set; }


            public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam,
                                    IntPtr lParam, ref bool handled)
            {
                if (msg == CallbackId)
                {
                    if (wParam.ToInt32() == (int)ABNotify.ABN_POSCHANGED)
                    {
                        ABSetPos(Edge, Window, ScreenNum);
                        handled = true;
                    }
                }
                return IntPtr.Zero;
            }

        }
        private static Dictionary<Window, RegisterInfo> s_RegisteredWindowInfo
            = new Dictionary<Window, RegisterInfo>();
        private static RegisterInfo GetRegisterInfo(Window appbarWindow)
        {
            RegisterInfo reg;
            if (s_RegisteredWindowInfo.ContainsKey(appbarWindow))
            {
                reg = s_RegisteredWindowInfo[appbarWindow];
            }
            else
            {
                reg = new RegisterInfo()
                {
                    CallbackId = 0,
                    Window = appbarWindow,
                    IsRegistered = false,
                    Edge = ABEdge.Top,
                    OriginalStyle = appbarWindow.WindowStyle,
                    OriginalPosition = new Point(appbarWindow.Left, appbarWindow.Top),
                    OriginalSize =
                        new Size(appbarWindow.ActualWidth, appbarWindow.ActualHeight),
                    OriginalResizeMode = appbarWindow.ResizeMode,
                };
                s_RegisteredWindowInfo.Add(appbarWindow, reg);
            }
            return reg;
        }

        private static void RestoreWindow(Window appbarWindow)
        {
            RegisterInfo info = GetRegisterInfo(appbarWindow);

            appbarWindow.WindowStyle = info.OriginalStyle;
            appbarWindow.ResizeMode = info.OriginalResizeMode;
            appbarWindow.Topmost = false;

            Rect rect = new Rect(info.OriginalPosition.X, info.OriginalPosition.Y,
                info.OriginalSize.Width, info.OriginalSize.Height);
            appbarWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                    new ResizeDelegate(DoResize), appbarWindow, rect);

        }
        //public enum DWMNCRENDERINGPOLICY
        //{ 
        //  DWMNCRP_USEWINDOWSTYLE,
        //  DWMNCRP_DISABLED,
        //  DWMNCRP_ENABLED,
        //  DWMNCRP_LAST
        //};
        //public enum DWMWINDOWATTRIBUTE : uint
        //{
        //    NCRenderingEnabled = 1,
        //    NCRenderingPolicy,
        //    TransitionsForceDisabled,
        //    AllowNCPaint,
        //    CaptionButtonBounds,
        //    NonClientRtlLayout,
        //    ForceIconicRepresentation,
        //    Flip3DPolicy,
        //    ExtendedFrameBounds,
        //    HasIconicBitmap,
        //    DisallowPeek,
        //    ExceludedFromPeek,
        //    Cloak,
        //    Cloaked,
        //    FreezeRepresentation
        //}
        //[DllImport("dwmapi.dll", PreserveSig = true)]
        //public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE attr, ref int attrValue, int attrSize);
        
        public static Window AppBarWindow { get; set; }
        public static ABEdge Edge { get; set; }
        public static int ScreenNum { get; set; }

        public static void SetAppBar(Window appbarWindow, ABEdge edge, int screen)
        {
            RegisterInfo info = GetRegisterInfo(appbarWindow);
            info.Edge = edge;

            APPBARDATA abd = new APPBARDATA();
            abd.cbSize = Marshal.SizeOf(abd);
            abd.hWnd = new WindowInteropHelper(appbarWindow).Handle;

            //int renderPolicy;
            if (edge == ABEdge.None)
            {
                if (info.IsRegistered)
                {
                    SHAppBarMessage((int)ABMsg.ABM_REMOVE, ref abd);
                    info.IsRegistered = false;
                }
                RestoreWindow(appbarWindow);
                return;
            }

            if (!info.IsRegistered)
            {
                info.IsRegistered = true;
                info.CallbackId = RegisterWindowMessage("AppBarMessage");
                abd.uCallbackMessage = info.CallbackId;

                uint ret = SHAppBarMessage((int)ABMsg.ABM_NEW, ref abd);

                HwndSource source = HwndSource.FromHwnd(abd.hWnd);
                source.AddHook(new HwndSourceHook(info.WndProc));
            }

            appbarWindow.WindowStyle = WindowStyle.None;
            appbarWindow.ResizeMode = ResizeMode.NoResize;
            appbarWindow.Topmost = true;

            AppBarWindow = appbarWindow;
            Edge = edge;
            ScreenNum = screen;

            appbarWindow.SizeChanged += appbarWindow_SizeChanged;
            ABSetPos(info.Edge, appbarWindow, screen);
        }

        static void appbarWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ABSetPos(Edge, AppBarWindow, ScreenNum);
        }

        private delegate void ResizeDelegate(Window appbarWindow, RECT rect);
        private static void DoResize(Window appbarWindow, RECT rect)
        {
            appbarWindow.Width = rect.right - rect.left;
            appbarWindow.Height = rect.bottom - rect.top;
            appbarWindow.Top = rect.top;
            appbarWindow.Left = rect.left;
        }

        enum DockingCalculationMode
        {
            Standard,
            Surface
        }
        private static DockingCalculationMode UsersDockingCalculationMode
        {
            get
            {
                Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter CRMWindowRouter = AifServiceContainer.Instance.GetService<Microsoft.Crm.UnifiedServiceDesk.Dynamics.ICRMWindowRouter>();
                string modeText = CRMWindowRouter.ReplaceParametersInCurrentSession("[[$Settings.UsersDockingCalculationMode]+]");
                if (String.IsNullOrEmpty(modeText))
                    modeText = CRMWindowRouter.ReplaceParametersInCurrentSession("[[$Global.UsersDockingCalculationMode]+]");
                if (String.IsNullOrEmpty(modeText) || modeText.Equals("Standard", StringComparison.InvariantCultureIgnoreCase))
                    return DockingCalculationMode.Standard;
                else
                    return DockingCalculationMode.Surface;
            }
        }

        private static void ABSetPos(ABEdge edge, Window appbarWindow, int screen)
        {
            APPBARDATA barData = new APPBARDATA();
            barData.cbSize = Marshal.SizeOf(barData);
            barData.hWnd = new WindowInteropHelper(appbarWindow).Handle;
            barData.uEdge = (int)edge;

            foreach (Screen s in Screen.AllScreens)
            {
                Trace.WriteLine(s.Bounds.ToString());
                Trace.WriteLine(Screen.AllScreens[screen].WorkingArea.ToString());
            }
            System.Drawing.Rectangle screenBounds;
            if (Screen.AllScreens.Count() > screen)
                screenBounds = Screen.AllScreens[screen].Bounds;
            else
                screenBounds = Screen.PrimaryScreen.Bounds;

            Window MainWindow = System.Windows.Application.Current.MainWindow;
            PresentationSource MainWindowPresentationSource = PresentationSource.FromVisual(MainWindow);
            Matrix m = MainWindowPresentationSource.CompositionTarget.TransformToDevice;
            double thisDpiWidthFactor = m.M11;
            double thisDpiHeightFactor = m.M22;

            if (UsersDockingCalculationMode == DockingCalculationMode.Surface)
            {
                Trace.WriteLine("Docking in Surface Mode");
                // following is needed for Surface for some reason
                barData.rc.right = (int)(screenBounds.Right * thisDpiWidthFactor);
                barData.rc.top = (int)(screenBounds.Top * thisDpiHeightFactor);
                barData.rc.bottom = (int)(screenBounds.Bottom * thisDpiHeightFactor);
                barData.rc.left = (int)(screenBounds.Left * thisDpiWidthFactor);
                if (barData.uEdge == (int)ABEdge.Left)
                    barData.rc.right = barData.rc.left + (int)(appbarWindow.ActualWidth * thisDpiWidthFactor);
                else if (barData.uEdge == (int)ABEdge.Right)
                    barData.rc.left = barData.rc.right - (int)(appbarWindow.ActualWidth * thisDpiWidthFactor);
                else if (barData.uEdge == (int)ABEdge.Top)
                    barData.rc.bottom = barData.rc.top + (int)(appbarWindow.ActualHeight * thisDpiHeightFactor);
                else if (barData.uEdge == (int)ABEdge.Bottom)
                    barData.rc.top = barData.rc.bottom - (int)(appbarWindow.ActualHeight * thisDpiHeightFactor);
            }
            else if (UsersDockingCalculationMode == DockingCalculationMode.Standard)
            {
                // if !Surface
                Trace.WriteLine("Docking in Standard Mode");
                barData.rc.right = (int)(screenBounds.Right / thisDpiWidthFactor);
                barData.rc.bottom = (int)(screenBounds.Bottom / thisDpiHeightFactor);
                barData.rc.left = (int)(screenBounds.Left / thisDpiWidthFactor);
                barData.rc.top = (int)(screenBounds.Top / thisDpiHeightFactor);
                if (barData.uEdge == (int)ABEdge.Left)
                    barData.rc.right = barData.rc.left + (int)(appbarWindow.ActualWidth);
                else if (barData.uEdge == (int)ABEdge.Right)
                    barData.rc.left = barData.rc.right - (int)(appbarWindow.ActualWidth);
                else if (barData.uEdge == (int)ABEdge.Top)
                    barData.rc.bottom = barData.rc.top + (int)(appbarWindow.ActualHeight);
                else if (barData.uEdge == (int)ABEdge.Bottom)
                    barData.rc.top = barData.rc.bottom - (int)(appbarWindow.ActualHeight);
            }
            uint ret;
            ret = SHAppBarMessage((int)ABMsg.ABM_QUERYPOS, ref barData);
            ret = SHAppBarMessage((int)ABMsg.ABM_SETPOS, ref barData);
            //barData.lParam = (IntPtr)1;
            //ret = SHAppBarMessage((int)ABMsg.ABM_SETAUTOHIDEBAR, ref barData);
            //ret = SHAppBarMessage((int)ABMsg.ABM_GETAUTOHIDEBAR, ref barData);

            if (UsersDockingCalculationMode == DockingCalculationMode.Surface)
            {
                // if Surface = Really not sure why its different on the surface
                barData.rc.top = (int)(barData.rc.top / thisDpiWidthFactor);
                barData.rc.bottom = (int)(barData.rc.bottom / thisDpiWidthFactor);
                barData.rc.left = (int)(barData.rc.left / thisDpiWidthFactor);
                barData.rc.right = (int)(barData.rc.right / thisDpiWidthFactor);

            }
            else if (UsersDockingCalculationMode == DockingCalculationMode.Standard)
            {
                // if !Surface
                if (barData.uEdge == (int)ABEdge.Left)
                    barData.rc.right = barData.rc.left + (int)appbarWindow.ActualWidth;
                else if (barData.uEdge == (int)ABEdge.Right)
                    barData.rc.left = barData.rc.right - (int)appbarWindow.ActualWidth;
                else if (barData.uEdge == (int)ABEdge.Top)
                    barData.rc.bottom = barData.rc.top + (int)appbarWindow.ActualHeight;
                else if (barData.uEdge == (int)ABEdge.Bottom)
                    barData.rc.top = barData.rc.bottom - (int)appbarWindow.ActualHeight;
            }

            //This is done async, because WPF will send a resize after a new appbar is added.  
            //if we size right away, WPFs resize comes last and overrides us.
            appbarWindow.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle,
                new ResizeDelegate(DoResize), appbarWindow, barData.rc);
        }
    }




}
