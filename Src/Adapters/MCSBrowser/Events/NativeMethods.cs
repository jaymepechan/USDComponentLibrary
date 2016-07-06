/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Management;
using Microsoft.Uii.AifServices;

namespace Microsoft.USD.ComponentLibrary
{
    public static class NativeMethods
    {
        // Fields
        public static Guid IID_IHTMLDocument = new Guid("626fc520-a41e-11cf-a731-00a0c9082637");
        public static Guid IID_InternetExplorer = new Guid("0002DF01-0000-0000-C000-000000000046");
        public static Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11D0-8A3E-00C04FC9E26E");
        public static Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");

        // Methods
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(int hObject);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", EntryPoint = "DeleteUrlCacheEntryA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteUrlCacheEntry(IntPtr lpszUrlName);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool DeleteUrlCacheGroup(long GroupId, int dwFlags, IntPtr lpReserved);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumChildWindows(HandleRef hWndParent, EnumThreadWindowsCallback callback, IntPtr extraData);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool EnumWindows(EnumThreadWindowsCallback callback, IntPtr extraData);
        [DllImport("wininet.dll", EntryPoint = "FindFirstUrlCacheEntryA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindFirstUrlCacheEntry([MarshalAs(UnmanagedType.LPWStr)] string lpszUrlSearchPattern, IntPtr lpFirstCacheEntryInfo, ref int lpdwFirstCacheEntryInfoBufferSize);
        [DllImport("wininet.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr FindFirstUrlCacheGroup(int dwFlags, int dwFilter, IntPtr lpSearchCondition, int dwSearchCondition, ref long lpGroupId, IntPtr lpReserved);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", EntryPoint = "FindNextUrlCacheEntryA", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindNextUrlCacheEntry(IntPtr hFind, IntPtr lpNextCacheEntryInfo, ref int lpdwNextCacheEntryInfoBufferSize);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("wininet.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool FindNextUrlCacheGroup(IntPtr hFind, ref long lpGroupId, IntPtr lpReserved);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetAncestor(IntPtr hWnd, WinUserConstant gaFlags);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, [Out] StringBuilder lpClassName, int nMaxCount);
        public static Process GetIEServerProcess(int pId)
        {
            int processId = 0;
            Process processById = null;
            string str = "iexplore.exe";
            ManagementObjectCollection objects = new ManagementObjectSearcher(string.Format("select ProcessID ,CommandLine from Win32_Process where Name='{0}'", str)).Get();
            if ((objects == null) || (objects.Count == 0))
            {
                return null;
            }
            foreach (ManagementObject obj2 in objects)
            {
                if ((obj2 != null) && (obj2["CommandLine"] != null))
                {
                    string str3 = obj2["CommandLine"].ToString();
                    if (!string.IsNullOrEmpty(str3) && str3.Contains("SCODEF:" + pId.ToString()))
                    {
                        processId = int.Parse(obj2["ProcessID"].ToString());
                        break;
                    }
                }
            }
            if (processId != 0)
            {
                processById = Process.GetProcessById(processId);
            }
            return processById;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hwnd);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetProcessId(int hProcess);
        public static int GetProcessIdFromWindow(IntPtr hWnd)
        {
            int num;
            GetWindowThreadProcessId(hWnd, out num);
            return num;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetWindow(HandleRef hWnd, int uCmd);
        public static string GetWindowClass(IntPtr hWnd)
        {
            StringBuilder lpClassName = new StringBuilder(0x80);
            GetClassName(hWnd, lpClassName, lpClassName.Capacity);
            return lpClassName.ToString();
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowLong(IntPtr hWnd, WinUserConstant nIndex);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [DllImport("ieframe.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int IEIsProtectedModeURL([In] string pszUrl);
        [DllImport("ieframe.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int IELaunchURL([In] string pszUrl, ref PROCESS_INFORMATION pProcInfo, ref IELAUNCHURLINFO lpInfo);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int MoveWindow(IntPtr hWnd, int left, int top, int nWidth, int nHeight, bool bRepaint);
        [return: MarshalAs(UnmanagedType.Interface)]
        [DllImport("oleacc.dll", PreserveSig = false)]
        public static extern object ObjectFromLresult(UIntPtr lResult, [MarshalAs(UnmanagedType.LPStruct)] Guid refiid, IntPtr wParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern uint RegisterWindowMessage(string lpString);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SendMessageTimeout(IntPtr hWnd, uint Msg, UIntPtr wParam, IntPtr lParam, SendMessageTimeoutFlags fuFlags, uint uTimeout, out UIntPtr lpdwResult);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr child, IntPtr newParent);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowLong(IntPtr hWnd, WinUserConstant nIndex, uint dwNewLong);
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int ShellExecute(IntPtr hWnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("shell32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool ShowWindow(IntPtr hWnd, WinUserConstant nCmdShow);
        public static Process StartIE(string urlString)
        {
            if (IEIsProtectedModeURL(urlString).Equals(0))
            {
                return StartIE7ViaLaunchURL(urlString);
            }
            return StartIEViaShellExecuteEx(urlString);
        }

        public static Process StartIE7ViaLaunchURL(string url)
        {
            TraceMsg("StartIE7ViaLaunchURL: {0}", new object[] { url });
            PROCESS_INFORMATION pProcInfo = new PROCESS_INFORMATION();
            IELAUNCHURLINFO structure = new IELAUNCHURLINFO();
            structure.cbSize = Marshal.SizeOf(structure);
            structure.dwCreationFlags = 0;
            int num = IELaunchURL(url, ref pProcInfo, ref structure);
            TraceMsg("IELaunchURL(): rc={0}: {1}", new object[] { num, (num != 0) ? "error" : "ok" });
            Process processById = Process.GetProcessById(GetProcessId(pProcInfo.hProcess));
            CloseHandle(pProcInfo.hThread);
            CloseHandle(pProcInfo.hProcess);
            return processById;
        }

        public static Process StartIEViaShellExecuteEx(string url)
        {
            TraceMsg("StartIEViaShellExecuteEx: {0}", new object[] { url });
            SHELLEXECUTEINFO structure = new SHELLEXECUTEINFO();
            structure.cbSize = Marshal.SizeOf(structure);
            structure.lpVerb = "open";
            structure.lpFile = "iexplore";
            //structure.lpParameters = BrowserHost.IsIE8OrHigher ? ("-nomerge " + url) : url;
            structure.lpParameters = "-nohome " + url;  // -nohome is used to id the process later
            structure.nShow = 0;
            structure.fMask = 0x440;
            structure.hwnd = IntPtr.Zero;
            bool flag = ShellExecuteEx(ref structure);
            TraceMsg("ShellExecuteEx(): rc={0}: {1}", new object[] { flag, flag ? "ok" : "error" });
            Process processById = Process.GetProcessById(GetProcessId(structure.hProcess));
            CloseHandle(structure.hProcess);
            return processById;
        }

        private static void TraceMsg(string format, params object[] args)
        {
            Trace.WriteLine(string.Format(format, args));
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int WaitForInputIdle(int hProcess, uint dwMilliseconds);

        // Nested Types
        public delegate bool EnumThreadWindowsCallback(IntPtr hWnd, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct IELAUNCHURLINFO
        {
            public int cbSize;
            public uint dwCreationFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public int hProcess;
            public int hThread;
            public uint dwProcessId;
            public uint dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
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

        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_ABORTIFHUNG = 2,
            SMTO_BLOCK = 1,
            SMTO_NORMAL = 0,
            SMTO_NOTIMEOUTIFNOTHUNG = 8
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            public string lpVerb;
            public string lpFile;
            public string lpParameters;
            public string lpDirectory;
            public int nShow;
            public int hInstApp;
            public int lpIDList;
            public string lpClass;
            public int hkeyClass;
            public uint dwHotKey;
            public int hIcon;
            public int hProcess;
        }

        [Flags]
        public enum WindowStyleFlags : uint
        {
            WS_BORDER = 0x800000,
            WS_CAPTION = 0xc00000,
            WS_CHILD = 0x40000000,
            WS_CLIPCHILDREN = 0x2000000,
            WS_CLIPSIBLINGS = 0x4000000,
            WS_DISABLED = 0x8000000,
            WS_DLGFRAME = 0x400000,
            WS_GROUP = 0x20000,
            WS_HSCROLL = 0x100000,
            WS_MAXIMIZE = 0x1000000,
            WS_MAXIMIZEBOX = 0x10000,
            WS_MINIMIZE = 0x20000000,
            WS_MINIMIZEBOX = 0x20000,
            WS_OVERLAPPED = 0,
            WS_POPUP = 0x80000000,
            WS_SYSMENU = 0x80000,
            WS_TABSTOP = 0x10000,
            WS_THICKFRAME = 0x40000,
            WS_VISIBLE = 0x10000000,
            WS_VSCROLL = 0x200000
        }

        public enum WinUserConstant
        {
            GA_ROOT = 2,
            GW_CHILD = 5,
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
            SW_SHOWNORMAL = 1
        }
    }
}