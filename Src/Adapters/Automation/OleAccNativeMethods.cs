using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    internal enum OleAccObjectId
    {
        OBJID_ALERT = -10,
        OBJID_CARET = -8,
        OBJID_CLIENT = -4,
        OBJID_CURSOR = -9,
        OBJID_HSCROLL = -6,
        OBJID_MENU = -3,
        OBJID_NATIVEOM = -16,
        OBJID_QUERYCLASSNAMEIDX = -12,
        OBJID_SIZEGRIP = -7,
        OBJID_SOUND = -11,
        OBJID_SYSMENU = -1,
        OBJID_TITLEBAR = -2,
        OBJID_VSCROLL = -5,
        OBJID_WINDOW = 0
    }

    internal static class OleAccNativeMethods
    {
        // Fields
        internal static Guid IID_IAccessible = new Guid("618736E0-3C3D-11CF-810C-00AA00389B71");

        // Methods
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int AccessibleChildren(IAccessible paccContainer, int iChildStart, int cChildren, [Out] object[] rgvarChildren, out int pcObtained);
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int AccessibleObjectFromEvent(IntPtr hwnd, int dwObjectID, int dwChildID, out IAccessible ppacc, [MarshalAs(UnmanagedType.Struct)] out object pvarChild);
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int AccessibleObjectFromWindow(IntPtr hwnd, OleAccObjectId dwObjectID, ref Guid refID, ref IAccessible ppvObject);
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int GetRoleText(uint dwRole, [Out] StringBuilder lpszRole, uint cchRoleMax);
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int GetStateText(uint dwStateBit, [Out] StringBuilder lpszStateBit, uint cchStateBitMax);
        [return: MarshalAs(UnmanagedType.Interface)]
        [DllImport("oleacc.dll", SetLastError = true, PreserveSig = false)]
        internal static extern object ObjectFromLresult(UIntPtr lResult, [MarshalAs(UnmanagedType.LPStruct)] Guid refiid, IntPtr wParam);
        [DllImport("oleacc.dll", SetLastError = true)]
        internal static extern int WindowFromAccessibleObject(IAccessible pacc, out IntPtr phWnd);
    }
}
