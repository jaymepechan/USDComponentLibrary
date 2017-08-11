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

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct LPPOINT
    {
        internal int X;
        internal int Y;
    }
    internal delegate bool EnumWindowsProc(System.IntPtr hWnd, System.IntPtr lParam);
    public enum WinUserConstant
    {
        GW_CHILD = 5,
        GW_ENABLEDPOPUP = 6,
        GW_HWNDFIRST = 0,
        GW_HWNDLAST = 1,
        GW_HWNDNEXT = 2,
        GW_HWNDPREV = 3,
        GW_OWNER = 4,
        GWL_EXSTYLE = -20,
        GWL_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4,
        HWND_BOTTOM = 1,
        HWND_NOTOPMOST = -2,
        HWND_TOP = 0,
        HWND_TOPMOST = -1,
        MF_BYCOMMAND = 0,
        MF_BYPOSITION = 0x400,
        MK_LBUTTON = 1,
        MK_RBUTTON = 2,
        SC_CLOSE = 0xf060,
        SM_CYCAPTION = 4,
        SW_FORCEMINIMIZE = 11,
        SW_HIDE = 0,
        SW_MAX = 11,
        SW_MAXIMIZE = 3,
        SW_MINIMIZE = 6,
        SW_NORMAL = 1,
        SW_RESTORE = 9,
        SW_SHOW = 5,
        SW_SHOWDEFAULT = 10,
        SW_SHOWMAXIMIZED = 3,
        SW_SHOWMINIMIZED = 2,
        SW_SHOWMINNOACTIVE = 7,
        SW_SHOWNA = 8,
        SW_SHOWNOACTIVATE = 4,
        SW_SHOWNORMAL = 1,
        SWP_HIDEWINDOW = 0x80,
        SWP_NOACTIVATE = 0x10,
        SWP_NOMOVE = 0,
        SWP_NOSIZE = 1,
        SWP_SHOWWINDOW = 0x40
    }
    internal enum WindowMessage
    {
        WM_ACTIVATE = 6,
        WM_ACTIVATEAPP = 0x1c,
        WM_CANCELMODE = 0x1f,
        WM_CHILDACTIVATE = 0x22,
        WM_CLOSE = 0x10,
        WM_COMMAND = 0x111,
        WM_CREATE = 1,
        WM_DESTROY = 2,
        WM_DEVMODECHANGE = 0x1b,
        WM_ENABLE = 10,
        WM_ENDSESSION = 0x16,
        WM_ERASEBKGND = 20,
        WM_FONTCHANGE = 0x1d,
        WM_GETHOTKEY = 0x33,
        WM_GETMINMAXINFO = 0x24,
        WM_GETTEXT = 13,
        WM_GETTEXTLENGTH = 14,
        WM_KILLFOCUS = 8,
        WM_LBUTTONDBLCLK = 0x203,
        WM_LBUTTONDOWN = 0x201,
        WM_LBUTTONUP = 0x202,
        WM_MOUSEACTIVATE = 0x21,
        WM_MOVE = 3,
        WM_NULL = 0,
        WM_PAINT = 15,
        WM_QUERYENDSESSION = 0x11,
        WM_QUERYOPEN = 0x13,
        WM_QUEUESYNC = 0x23,
        WM_QUIT = 0x12,
        WM_RBUTTONDBLCLK = 0x206,
        WM_RBUTTONDOWN = 0x204,
        WM_RBUTTONUP = 0x205,
        WM_SETCURSOR = 0x20,
        WM_SETFOCUS = 7,
        WM_SETHOTKEY = 50,
        WM_SETREDRAW = 11,
        WM_SETTEXT = 12,
        WM_SETTINGCHANGE = 0x1a,
        WM_SHOWWINDOW = 0x18,
        WM_SIZE = 5,
        WM_SYSCOLORCHANGE = 0x15,
        WM_TIMECHANGE = 30,
        WM_WINDOWPOSCHANGED = 0x47,
        WM_WINDOWPOSCHANGING = 70,
        WM_WININICHANGE = 0x1a
    }
    internal enum ButtonControlMessage
    {
        BM_CLICK = 0xf5,
        BM_GETCHECK = 240,
        BM_GETIMAGE = 0xf6,
        BM_GETSTATE = 0xf2,
        BM_SETCHECK = 0xf1,
        BM_SETIMAGE = 0xf7,
        BM_SETSTATE = 0xf3,
        BM_SETSTYLE = 0xf4,
        BST_CHECKED = 1,
        BST_FOCUS = 8,
        BST_INDETERMINATE = 2,
        BST_PUSHED = 4,
        BST_UNCHECKED = 0
    }
    internal enum ComboBoxMessage
    {
        CB_ADDSTRING = 0x143,
        CB_DELETESTRING = 0x144,
        CB_DIR = 0x145,
        CB_FINDSTRING = 0x14c,
        CB_FINDSTRINGEXACT = 0x158,
        CB_GETCOUNT = 0x146,
        CB_GETCURSEL = 0x147,
        CB_GETDROPPEDCONTROLRECT = 0x152,
        CB_GETDROPPEDSTATE = 0x157,
        CB_GETEDITSEL = 320,
        CB_GETEXTENDEDUI = 0x156,
        CB_GETITEMDATA = 0x150,
        CB_GETITEMHEIGHT = 340,
        CB_GETLBTEXT = 0x148,
        CB_GETLBTEXTLEN = 0x149,
        CB_GETLOCALE = 0x15a,
        CB_INSERTSTRING = 330,
        CB_LIMITTEXT = 0x141,
        CB_RESETCONTENT = 0x14b,
        CB_SELECTSTRING = 0x14d,
        CB_SETCURSEL = 0x14e,
        CB_SETEDITSEL = 0x142,
        CB_SETEXTENDEDUI = 0x155,
        CB_SETITEMDATA = 0x151,
        CB_SETITEMHEIGHT = 0x153,
        CB_SETLOCALE = 0x159,
        CB_SHOWDROPDOWN = 0x14f
    }
    internal static class Win32NativeMethods
    {
        internal static System.Guid IID_IHTMLDocument = new System.Guid("626fc520-a41e-11cf-a731-00a0c9082637");
        internal static System.Guid IID_IWebBrowser2 = new System.Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E");
        internal static System.Guid IID_IWebBrowserApp = new System.Guid("0002DF05-0000-0000-C000-000000000046");

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern bool EnumChildWindows(System.IntPtr hWndParent, EnumWindowsProc callback, System.IntPtr extraData);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern bool EnumWindows(EnumWindowsProc callback, System.IntPtr extraData);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetClassName(System.IntPtr hWnd, [System.Runtime.InteropServices.Out] System.Text.StringBuilder lpClassName, int nMaxCount);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetDesktopWindow();
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetDlgCtrlID(System.IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetMenu(System.IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetMenuItemCount(System.IntPtr hMenu);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetMenuItemID(System.IntPtr hMenu, int nPos);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern int GetMenuString(System.IntPtr hMenu, uint uIDItem, [System.Runtime.InteropServices.Out] System.Text.StringBuilder lpString, int maxCount, WinUserConstant uFlag);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetSubMenu(System.IntPtr hMenu, int nPos);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr GetWindow(System.IntPtr hWnd, WinUserConstant uCmd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowLong(System.IntPtr hWnd, WinUserConstant nIndex);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowRect(System.IntPtr hWnd, ref LPRECT lpRect);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowText(System.IntPtr hWnd, [System.Runtime.InteropServices.Out] System.Text.StringBuilder lpString, int nMaxCount);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(System.IntPtr hWnd, out int lpdwProcessId);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int IsWindow(System.IntPtr hWnd);
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool IsWow64Process([System.Runtime.InteropServices.In] System.IntPtr hProcess, out bool lpSystemInfo);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr PostMessage(System.IntPtr hWnd, WindowMessage uMsg, System.IntPtr wParam, System.IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern uint RegisterWindowMessage(string lpString);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int ScreenToClient(System.IntPtr hWnd, ref LPPOINT lpPoint);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, ButtonControlMessage uMsg, System.IntPtr wParam, System.IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, ComboBoxMessage uMsg, System.IntPtr wParam, System.IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, ComboBoxMessage uMsg, System.IntPtr wParam, string lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, WindowMessage uMsg, System.IntPtr wParam, System.IntPtr lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, WindowMessage uMsg, System.IntPtr wParam, [System.Runtime.InteropServices.Out] string lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern System.IntPtr SendMessage(System.IntPtr hWnd, WindowMessage uMsg, System.IntPtr wParam, [System.Runtime.InteropServices.Out] System.Text.StringBuilder lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
        internal static extern System.IntPtr SendMessageTimeout(System.IntPtr hWnd, uint Msg, System.UIntPtr wParam, System.IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out System.UIntPtr lpdwResult);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SetActiveWindow(System.IntPtr hWnd);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SetFocus(System.IntPtr hWnd);

        internal delegate void WinEventProc(System.IntPtr hWinEventHook, int iEvent, System.IntPtr hWnd, int idObject, int idChild, int dwEventThread, int dwmsEventTime);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern System.IntPtr SetWinEventHook(int eventMin, int eventMax, System.IntPtr hmodWinEventProc, WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        internal static extern int UnhookWinEvent(System.IntPtr hWinEventHook);

        [System.Flags]
        internal enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }
    }
}
