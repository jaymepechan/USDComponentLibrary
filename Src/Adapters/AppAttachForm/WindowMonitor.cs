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
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms;

    internal class WindowMonitor : NativeWindow, IDisposable
    {
        private readonly HandleRef _MonitoredWindow;
        private readonly DPHandler _WindowPositioner;
        private const int GA_ROOT = 2;
        private const int SWP_HIDEWINDOW = 0x80;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_NOMOVE = 2;
        private const int SWP_NOSIZE = 1;
        private const int SWP_NOZORDER = 4;
        private const int SWP_SHOWWINDOW = 0x40;
        private const int WA_ACTIVE = 1;
        private const int WA_CLICKACTIVE = 2;
        private const int WA_INACTIVE = 0;
        private const int WM_ACTIVATE = 6;
        private const int WM_EXITSIZEMOVE = 0x232;
        private const int WM_NCLBUTTONDBLCLK = 0xa3;
        private const int WM_PAINT = 15;
        private const int WM_SYSCOMMAND = 0x112;
        private const int WM_WINDOWPOSCHANGED = 0x47;

        private WindowMonitor()
        {
        }

        public WindowMonitor(HandleRef monitoredWindow, DPHandler windowPositioner)
        {
            if (monitoredWindow.Handle.Equals(IntPtr.Zero))
            {
                throw new ArgumentOutOfRangeException("monitoredWindow");
            }
            if (windowPositioner == null)
            {
                throw new ArgumentNullException("windowPositioner");
            }
            this._WindowPositioner = windowPositioner;
            this._MonitoredWindow = monitoredWindow;
            base.AssignHandle(_MonitoredWindow.Handle); // GetAncestor(this._MonitoredWindow, 2));
        }

        public void Dispose()
        {
            this.ReleaseHandle();
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetAncestor(HandleRef hWnd, uint gaFlags);
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            //Trace.WriteLine("\tWndProc:" + m.Msg.ToString("X8") + ":" + m.WParam.ToString("X8"));
            switch (m.Msg)
            {
                case 6:
                    switch ((m.WParam.ToInt32() & 0xffff))
                    {
                        case 0:
                            this._WindowPositioner.Deactivate();
                            return;

                        case 1:
                            this._WindowPositioner.Activate();
                            return;

                        case 2:
                            this._WindowPositioner.Activate();
                            return;
                    }
                    return;
                case 0x46: // WM_POSCHANGING
                case 0x47: // WM_POSCHANGED
                    {
                        WINDOWPOS windowpos = (WINDOWPOS)Marshal.PtrToStructure(m.LParam, typeof(WINDOWPOS));
                        //Trace.WriteLine("IsHidden:" + windowpos.IsHidden.ToString() + " IsShown:" + windowpos.IsShown.ToString() + " IsMoved:" + windowpos.IsMoved.ToString()
                        //    + " IsResized:" + windowpos.IsResized.ToString() + " IsZOrder:" + windowpos.IsZOrder.ToString()
                        //    + " IsActivate:" + windowpos.IsActivate.ToString());
                        if (windowpos.IsHidden)
                        {
                            this._WindowPositioner.HideWindow(0x4e20, 0x4e20);
                        }
                        else if (windowpos.IsShown)
                        {
                            _WindowPositioner.ShowWindow();
                        }
                        else if (windowpos.IsResized)
                        {
                            if (this._WindowPositioner.isActivated)
                            {
                                this._WindowPositioner.Activate(0x4e20, 0x4e20);
                            }
                            else
                            {
                                this._WindowPositioner.Activate();
                            }
                        }
                        break;
                    }
                case 0xa3:
                case 0x112:
                case 0x232:
                    this._WindowPositioner.Activate();
                    break;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPOS
        {
            public IntPtr hwnd;
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public uint flags;
            public bool IsMoved
            {
                get
                {
                    return !this.IsSet(2);
                }
            }
            public bool IsResized
            {
                get
                {
                    return !this.IsSet(1);
                }
            }
            public bool IsZOrder
            {
                get
                {
                    return !this.IsSet(4);
                }
            }
            public bool IsActivate
            {
                get
                {
                    return !this.IsSet(0x10);
                }
            }
            public bool IsShown
            {
                get
                {
                    return this.IsSet(0x40);
                }
            }
            public bool IsHidden
            {
                get
                {
                    return this.IsSet(0x80);
                }
            }
            private bool IsSet(uint flag)
            {
                return ((this.flags & flag) == flag);
            }
        }
    }
}
