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

namespace Microsoft.USD.ComponentLibrary.Adapters.AppAttachForm
{
    using Microsoft.Uii.Csr;
    using Xrm.Tooling.WebResourceUtility;
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms.Integration;

    public class DPHandler : IDisposable
    {
        private bool _InputQueuesAttached;
        private long _LastSnapshotTick;
        private readonly HandleRef _MonitoredWindow;
        private readonly WindowsFormsHost _MonitoredWindowHost;
        private readonly HandleRef _PositionedTopWindow;
        private readonly WindowMonitor _WindowMonitor;
        private const int GWL_EXSTYLE = -20;
        private const int GWL_HWNDPARENT = -8;
        private const int GWL_STYLE = -16;
        private const int HWND_NOTTOPMOST = -2;
        private const int HWND_TOP = 0;
        private const int HWND_TOPMOST = -1;
        public bool isActivated;
        public bool isAppActivated;
        private const int MF_BYCOMMAND = 0;
        private const int MF_BYPOSITION = 0x400;
        private const int SC_CLOSE = 0xf060;
        private const int SC_MAXIMIZE = 0xf030;
        private const int SC_MINIMIZE = 0xf020;
        private const int SC_MOVE = 0xf010;
        private const int SC_RESTORE = 0xf120;
        private const int SC_SEPARATOR = 0xf00f;
        private const int SC_SIZE = 0xf000;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_HIDEWINDOW = 0x80;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_NOMOVE = 2;
        private const int SWP_NOSIZE = 1;
        private const int SWP_SHOWWINDOW = 0x40;
        private const int WS_CAPTION = 0xc00000;
        private const int WS_EX_APPWINDOW = 0x40000;
        private const int WS_EX_TOOLWINDOW = 0x80;
        private const int WS_POPUP = -2147483648;
        private const int WS_THICKFRAME = 0x40000;
        protected TraceLogger LogWriter = null;
        Window topLevelWindow;

        private DPHandler()
        {
            LogWriter = new TraceLogger("Microsoft.USD.ComponentLibrary");
        }

        public DPHandler(WindowsFormsHost monitoredWindow, HandleRef positionedTopWindow, bool attachThreadInput)
        {
            LogWriter = new TraceLogger("Microsoft.USD.ComponentLibrary");
            if (monitoredWindow.Handle.Equals(IntPtr.Zero))
            {
                throw new ArgumentOutOfRangeException("monitoredWindow");
            }
            if (positionedTopWindow.Handle.Equals(IntPtr.Zero))
            {
                throw new ArgumentOutOfRangeException("positionedTopWindow");
            }
            this._MonitoredWindowHost = monitoredWindow;
            this._MonitoredWindow = new HandleRef(this, monitoredWindow.Handle);
            this._PositionedTopWindow = positionedTopWindow;
            SetWindowLong(this._PositionedTopWindow, -8, this._MonitoredWindow.Handle.ToInt32());
            if (attachThreadInput)
            {
                this.AttachThreadInput(true);
            }

            FrameworkElement objWin = monitoredWindow.Parent as FrameworkElement;
            while (objWin.Parent != null)
                objWin = objWin.Parent as FrameworkElement;
            this.topLevelWindow = objWin as Window;
            topLevelWindow.SizeChanged += TopLevelWindow_SizeChanged;
            topLevelWindow.LocationChanged += TopLevelWindow_LocationChanged;
            topLevelWindow.StateChanged += TopLevelWindow_StateChanged;
            this._WindowMonitor = new WindowMonitor(this._MonitoredWindow, this);
            if (IsWindowVisible(_MonitoredWindow))
                ShowWindow();
            else
                HideWindow();
        }

        private void TopLevelWindow_StateChanged(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void TopLevelWindow_LocationChanged(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void TopLevelWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Activate();
        }

        bool windowVisible = false;
        public void Activate()
        {
            if (windowVisible == false)
                return;
            RECT lpRect = new RECT();
            GetWindowRect(this._MonitoredWindow, ref lpRect);
            int hWndInsertAfter = this._InputQueuesAttached ? 0 : -1;
            int uFlags = this._InputQueuesAttached ? 0x10 : 0x4050;
            uFlags |= 0x40; // SW_SHOW
            SetWindowPos(this._PositionedTopWindow, hWndInsertAfter, lpRect.Left, lpRect.Top, lpRect.Width, lpRect.Height, uFlags);
            if ((Environment.TickCount - this._LastSnapshotTick) > 100L)
            {
                this._LastSnapshotTick = Environment.TickCount;
            }
        }

        public void Activate(bool isWinprocCall)
        {
            if (windowVisible == false)
                return;
            if ((!this.isAppActivated && isWinprocCall) || !isWinprocCall)
            {
                this.Activate2();
                this.isAppActivated = true;
            }
            else
            {
                this.Activate(0x4e20, 0x4e20);
            }
        }

        public void Activate(int left, int top)
        {
            if (windowVisible == false)
                return;
            RECT lpRect = new RECT();
            GetWindowRect(this._MonitoredWindow, ref lpRect);
            int hWndInsertAfter = this._InputQueuesAttached ? 0 : -1;
            int uFlags = this._InputQueuesAttached ? 0x10 : 0x4050;
            SetWindowPos(this._PositionedTopWindow, hWndInsertAfter, left, top, lpRect.Width, lpRect.Height, uFlags);
        }

        public void Activate2()
        {
            if (windowVisible == false)
                return;
            RECT lpRect = new RECT();
            GetWindowRect(this._MonitoredWindow, ref lpRect);
            int hWndInsertAfter = this._InputQueuesAttached ? 0 : -1;
            int uFlags = this._InputQueuesAttached ? 0x10 : 0x4050;
            SetWindowPos(this._PositionedTopWindow, hWndInsertAfter, lpRect.Left, lpRect.Top, lpRect.Width, lpRect.Height, uFlags);
            if ((Environment.TickCount - this._LastSnapshotTick) > 100L)
            {
                this._LastSnapshotTick = Environment.TickCount;
            }
        }

        private void AttachThreadInput(bool attach)
        {
            int num;
            int windowThreadProcessId = GetWindowThreadProcessId(this._PositionedTopWindow, out num);
            int idAttachTo = GetWindowThreadProcessId(this._MonitoredWindow, out num);
            if (!AttachThreadInput(windowThreadProcessId, idAttachTo, attach ? 1 : 0).Equals(0))
            {
                this._InputQueuesAttached = attach;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int AttachThreadInput(int idAttach, int idAttachTo, int fAttach);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(HandleRef hWnd, int nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        private static void ChangeWindowExStyles(HandleRef hWnd, int setExStyles, int clearExStyles)
        {
            SetWindowLong(hWnd, -20, (GetWindowLong(hWnd, -20) | setExStyles) & ~clearExStyles);
        }

        private static void ChangeWindowStyles(HandleRef hWnd, int setStyles, int clearStyles)
        {
            SetWindowLong(hWnd, -16, (GetWindowLong(hWnd, -16) | setStyles) & ~clearStyles);
        }
        public void Deactivate()
        {
            if (!this._InputQueuesAttached)
            {
                SetWindowPos(this._PositionedTopWindow, -2, 0, 0, 0, 0, 0x13);
            }
        }

        public void Deactivate(int left, int top)
        {
            if (!this._InputQueuesAttached)
            {
                SetWindowPos(this._PositionedTopWindow, -2, 0, left, top, 0, 0x13);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool DeleteMenu(HandleRef hMenu, uint uPosition, uint uFlags);
        public void Dispose()
        {
            this._WindowMonitor.Dispose();
            topLevelWindow.SizeChanged -= TopLevelWindow_SizeChanged;
            topLevelWindow.LocationChanged -= TopLevelWindow_LocationChanged;
            if (this._InputQueuesAttached)
            {
                this.AttachThreadInput(false);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetSystemMenu(HandleRef hWnd, bool bRevert);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(HandleRef hWnd, ref RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowThreadProcessId(HandleRef hWnd, out int lpdwProcessId);
        public void HideWindow()
        {
            windowVisible = false;
            SetWindowPos(this._PositionedTopWindow, 0, 0, 0, 0, 0, 0x80);
            FocusOutside();
        }

        public void HideWindow(int left, int top)
        {
            windowVisible = false;
            SetWindowPos(this._PositionedTopWindow, 0, left, top, 0, 0, 0x80);
            FocusOutside();
        }

        void FocusOutside()
        {
            FrameworkElement elem = this._MonitoredWindowHost as FrameworkElement;
            while (!(elem is Crm.UnifiedServiceDesk.Dynamics.PanelLayouts.IUSDPanel) && elem.Parent != null)
                elem = elem.Parent as FrameworkElement;
            if (elem != null)
                elem.Focus();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(HandleRef hWnd);


        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowPos(HandleRef hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowPos(HandleRef hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);
        public void ShowWindow()
        {
            windowVisible = true;
            //SetWindowPos(this._PositionedTopWindow, 0, 0, 0, 0, 0, 0x60);
            Activate();
        }

        public void ShowWindow(int left, int top)
        {
            windowVisible = true;
            //SetWindowPos(this._PositionedTopWindow, 0, left, top, 0, 0, 0x60);
            Activate();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public int Width
            {
                get
                {
                    return (this.Right - this.Left);
                }
            }
            public int Height
            {
                get
                {
                    return (this.Bottom - this.Top);
                }
            }
        }
    }
}
