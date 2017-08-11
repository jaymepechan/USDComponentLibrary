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
using System.Runtime.CompilerServices;
using Microsoft.Uii.HostedApplicationToolkit.DataDrivenAdapter;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{

    public static class Win32HelperMethods
    {
        private static FindWindowComparer _CurrWindowComparer;
        private static System.IntPtr _FoundWindowHandle;

        internal static FindWindowMatchType DetermineFindWindowMatchTypeFromText(string text)
        {
            if (text == null)
            {
                return FindWindowMatchType.Ignore;
            }
            if (((int)text.Length).Equals(0) || text.Equals("Equals", System.StringComparison.OrdinalIgnoreCase))
            {
                return FindWindowMatchType.Equals;
            }
            if (text.Equals("StartsWith", System.StringComparison.OrdinalIgnoreCase))
            {
                return FindWindowMatchType.StartsWith;
            }
            if (text.Equals("EndsWith", System.StringComparison.OrdinalIgnoreCase))
            {
                return FindWindowMatchType.EndsWith;
            }
            if (!text.Equals("Contains", System.StringComparison.OrdinalIgnoreCase))
            {
                throw new DataDrivenAdapterException("Bad match");
            }
            return FindWindowMatchType.Contains;
        }

        private static bool EnumWindowsCallback(System.IntPtr hWnd, System.IntPtr extraParameter)
        {
            if (_CurrWindowComparer(hWnd))
            {
                _FoundWindowHandle = hWnd;
                return false;
            }
            return true;
        }

        public static System.IntPtr FindChildWindowViaEnumChildWindows(System.IntPtr hWndParent, int processId, int threadId, string captionText, FindWindowMatchType captionMatchType, string classText, FindWindowMatchType classMatchType, int matchCount, bool ignoreProcThread)
        {
            if ((captionMatchType == FindWindowMatchType.Ignore) && (classMatchType == FindWindowMatchType.Ignore))
            {
                return System.IntPtr.Zero;
            }
            _CurrWindowComparer = hWnd => (bool)(((ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)) && (IsMatch(captionMatchType, GetWindowText(hWnd), captionText) && IsMatch(classMatchType, GetWindowClass(hWnd), classText))) && ((bool)((matchCount = (int)(matchCount - 1)) <= 0)));
            Win32NativeMethods.EnumChildWindows(hWndParent, new EnumWindowsProc(Win32HelperMethods.EnumWindowsCallback), System.IntPtr.Zero);
            return _FoundWindowHandle;
        }

        public static System.IntPtr FindTopWindowViaEnumWindows(int processId, int threadId, string captionText, FindWindowMatchType captionMatchType, string classText, FindWindowMatchType classMatchType, int matchCount, bool ignoreProcThread)
        {
            if ((captionMatchType == FindWindowMatchType.Ignore) && (classMatchType == FindWindowMatchType.Ignore))
            {
                return System.IntPtr.Zero;
            }
            _CurrWindowComparer = hWnd => (bool)(((ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)) && (IsMatch(captionMatchType, GetWindowText(hWnd), captionText) && IsMatch(classMatchType, GetWindowClass(hWnd), classText))) && ((bool)((matchCount = (int)(matchCount - 1)) <= 0)));
            Win32NativeMethods.EnumWindows(new EnumWindowsProc(Win32HelperMethods.EnumWindowsCallback), System.IntPtr.Zero);
            return _FoundWindowHandle;
        }

        public static System.IntPtr FindWindowByCaptionAndClassText(System.IntPtr hWndParent, int processId, int threadId, string captionText, FindWindowMatchType captionMatchType, string classText, FindWindowMatchType classMatchType, int matchCount, bool ignoreProcThread)
        {
            if ((captionMatchType == FindWindowMatchType.Ignore) && (classMatchType == FindWindowMatchType.Ignore))
            {
                return System.IntPtr.Zero;
            }
            _CurrWindowComparer = hWnd => (bool)(((ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)) && (IsMatch(captionMatchType, GetWindowText(hWnd), captionText) && IsMatch(classMatchType, GetWindowClass(hWnd), classText))) && ((bool)((matchCount = (int)(matchCount - 1)) <= 0)));
            return FindWindowViaBfs(hWndParent);
        }

        public static System.IntPtr FindWindowByControlId(System.IntPtr hWndParent, int processId, int threadId, int controlId, int matchCount, bool ignoreProcThread)
        {
            _CurrWindowComparer = hWnd => (bool)(((ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)) && ((Win32NativeMethods.GetDlgCtrlID(hWnd) == controlId) && (ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)))) && ((bool)((matchCount = (int)(matchCount - 1)) <= 0)));
            return FindWindowViaBfs(hWndParent);
        }

        public static System.IntPtr FindWindowByPosition(System.IntPtr hWndParent, int processId, int threadId, int x, int y, bool ignoreProcThread)
        {
            _CurrWindowComparer = delegate (System.IntPtr hWnd) {
                LPPOINT lpPoint = new LPPOINT();
                LPRECT lpRect = new LPRECT();
                Win32NativeMethods.GetWindowRect(hWnd, ref lpRect);
                lpPoint.X = lpRect.Left;
                lpPoint.Y = lpRect.Top;
                Win32NativeMethods.ScreenToClient(hWndParent, ref lpPoint);
                return (bool)(((ignoreProcThread || IsHwndOnProcThread(hWnd, processId, threadId)) && ((int)lpPoint.X).Equals(x)) && ((int)lpPoint.Y).Equals(y));
            };
            return FindWindowViaBfs(hWndParent);
        }

        private static System.IntPtr FindWindowViaBfs(System.IntPtr hWndRoot)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            Queue<System.IntPtr> queue = new Queue<System.IntPtr>();
            queue.Enqueue(hWndRoot);
            while (!((int)queue.Count).Equals(0) && zero.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                for (System.IntPtr ptr2 = queue.Dequeue(); !ptr2.Equals((System.IntPtr)System.IntPtr.Zero); ptr2 = Win32NativeMethods.GetWindow(ptr2, WinUserConstant.SW_SHOWMINIMIZED))
                {
                    if (_CurrWindowComparer(ptr2))
                    {
                        zero = ptr2;
                        continue;
                    }
                    System.IntPtr window = Win32NativeMethods.GetWindow(ptr2, WinUserConstant.SW_SHOW);
                    if (!window.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        queue.Enqueue(window);
                    }
                }
            }
            return zero;
        }

        internal static string GetCheck(System.IntPtr hWnd)
        {
            if (IsCheckBoxStateReadable(hWnd))
            {
                switch (((int)Win32NativeMethods.SendMessage(hWnd, ButtonControlMessage.BM_GETCHECK, System.IntPtr.Zero, System.IntPtr.Zero)))
                {
                    case 0:
                        return bool.FalseString;

                    case 1:
                        return bool.TrueString;
                }
                return "indeterminate";
            }
            return "unreadable";
        }

        internal static string GetMenuString(System.IntPtr hMenu, int subMenuIndex)
        {
            System.Text.StringBuilder lpString = new System.Text.StringBuilder(0x80);
            Win32NativeMethods.GetMenuString(hMenu, (uint)subMenuIndex, lpString, lpString.Capacity, WinUserConstant.MF_BYPOSITION);
            return lpString.ToString();
        }

        internal static string GetWindowClass(System.IntPtr hWnd)
        {
            System.Text.StringBuilder lpClassName = new System.Text.StringBuilder(0x80);
            Win32NativeMethods.GetClassName(hWnd, lpClassName, lpClassName.Capacity);
            return lpClassName.ToString();
        }

        internal static string GetWindowText(System.IntPtr hWnd)
        {
            System.Text.StringBuilder lpString = new System.Text.StringBuilder(0x80);
            Win32NativeMethods.GetWindowText(hWnd, lpString, lpString.Capacity);
            return lpString.ToString();
        }

        internal static string GetWindowTextAny(System.IntPtr hWnd)
        {
            System.Text.StringBuilder lParam = new System.Text.StringBuilder(0x80);
            Win32NativeMethods.SendMessage(hWnd, WindowMessage.WM_GETTEXT, (System.IntPtr)lParam.Capacity, lParam);
            return lParam.ToString();
        }

        internal static bool IsCheckBoxStateReadable(System.IntPtr hWnd)
        {
            switch ((Win32NativeMethods.GetWindowLong(hWnd, WinUserConstant.GWL_STYLE) & ((uint)15)))
            {
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 9:
                    return true;
            }
            return false;
        }

        internal static bool IsHwndOnProcThread(System.IntPtr hWnd, int processId, int threadId)
        {
            int num;
            int windowThreadProcessId = Win32NativeMethods.GetWindowThreadProcessId(hWnd, out num);
            processId = ((int)processId).Equals(0) ? num : processId;
            threadId = ((int)threadId).Equals(0) ? windowThreadProcessId : threadId;
            return (bool)((processId == num) && ((bool)(threadId == windowThreadProcessId)));
        }

        private static bool IsMatch(FindWindowMatchType matchType, string compareToText, string compareText)
        {
            switch (matchType)
            {
                case FindWindowMatchType.Ignore:
                    return true;

                case FindWindowMatchType.Equals:
                    return compareToText.Equals(compareText, System.StringComparison.Ordinal);

                case FindWindowMatchType.StartsWith:
                    return compareToText.StartsWith(compareText, System.StringComparison.Ordinal);

                case FindWindowMatchType.EndsWith:
                    return compareToText.EndsWith(compareText, System.StringComparison.Ordinal);

                case FindWindowMatchType.Contains:
                    return compareToText.Contains(compareText);
            }
            return false;
        }

        internal static bool IsWindow(System.IntPtr hWnd)
        {
            return !((int)Win32NativeMethods.IsWindow(hWnd)).Equals((System.IntPtr)System.IntPtr.Zero);
        }

        internal static bool IsWindowDisabled(System.IntPtr hWnd)
        {
            return (bool)((Win32NativeMethods.GetWindowLong(hWnd, WinUserConstant.GWL_STYLE) & 0x8000000) == 0x8000000);
        }

        internal static void SelectComboBoxItem(System.IntPtr hWnd, int index)
        {
            if (!IsWindowDisabled(hWnd))
            {
                Win32NativeMethods.SendMessage(hWnd, ComboBoxMessage.CB_SETCURSEL, (System.IntPtr)index, System.IntPtr.Zero);
            }
        }

        internal static void SelectComboBoxItem(System.IntPtr hWnd, string itemText)
        {
            if (!IsWindowDisabled(hWnd))
            {
                Win32NativeMethods.SendMessage(hWnd, ComboBoxMessage.CB_SELECTSTRING, System.IntPtr.Zero, itemText);
            }
        }

        internal static void SelectMenuItem(System.IntPtr hWnd, int hMenuItemId)
        {
            Win32NativeMethods.PostMessage(hWnd, WindowMessage.WM_COMMAND, (System.IntPtr)hMenuItemId, System.IntPtr.Zero);
        }

        internal static void SelectNextComboBoxItem(System.IntPtr hWnd)
        {
            int num = (int)Win32NativeMethods.SendMessage(hWnd, ComboBoxMessage.CB_GETCOUNT, System.IntPtr.Zero, System.IntPtr.Zero);
            if (num > 0)
            {
                int index = (int)Win32NativeMethods.SendMessage(hWnd, ComboBoxMessage.CB_GETCURSEL, System.IntPtr.Zero, System.IntPtr.Zero);
                if (index >= -1)
                {
                    index = (int)((index + 1) % num);
                    SelectComboBoxItem(hWnd, index);
                }
            }
        }

        internal static void Send_BMCLICK(System.IntPtr hWnd)
        {
            if (!IsWindowDisabled(hWnd))
            {
                Win32NativeMethods.SetActiveWindow(hWnd);
                Win32NativeMethods.SendMessage(hWnd, ButtonControlMessage.BM_CLICK, System.IntPtr.Zero, System.IntPtr.Zero);
            }
        }

        internal static void Send_WMCOMMAND(System.IntPtr hWndMainWindow, System.IntPtr hWnd)
        {
            if (!IsWindowDisabled(hWnd))
            {
                Win32NativeMethods.SendMessage(hWndMainWindow, WindowMessage.WM_COMMAND, System.IntPtr.Zero, hWnd);
            }
        }

        internal static void SetCheck(System.IntPtr hWnd, string checkStateStr)
        {
            if (!IsWindowDisabled(hWnd))
            {
                bool flag;
                ButtonControlMessage message;
                if (bool.TryParse(checkStateStr, out flag))
                {
                    message = flag ? ButtonControlMessage.BST_CHECKED : ButtonControlMessage.BST_UNCHECKED;
                }
                else
                {
                    message = ButtonControlMessage.BST_INDETERMINATE;
                }
                Win32NativeMethods.SendMessage(hWnd, ButtonControlMessage.BM_SETCHECK, (System.IntPtr)((long)message), System.IntPtr.Zero);
            }
        }

        internal static void SetWindowTextAny(System.IntPtr hWnd, string text)
        {
            if (!IsWindowDisabled(hWnd))
            {
                Win32NativeMethods.SendMessage(hWnd, WindowMessage.WM_SETTEXT, System.IntPtr.Zero, text);
            }
        }

        private delegate bool FindWindowComparer(System.IntPtr hWnd);
    }
}
