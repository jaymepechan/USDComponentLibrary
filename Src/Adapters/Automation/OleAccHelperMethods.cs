using Accessibility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    internal enum OleAccStateSystem
    {
        STATE_SYSTEM_ALERT_HIGH = 0x10000000,
        STATE_SYSTEM_ALERT_LOW = 0x4000000,
        STATE_SYSTEM_ALERT_MEDIUM = 0x8000000,
        STATE_SYSTEM_ANIMATED = 0x4000,
        STATE_SYSTEM_BUSY = 0x800,
        STATE_SYSTEM_CHECKED = 0x10,
        STATE_SYSTEM_COLLAPSED = 0x400,
        STATE_SYSTEM_DEFAULT = 0x100,
        STATE_SYSTEM_EXPANDED = 0x200,
        STATE_SYSTEM_EXTSELECTABLE = 0x2000000,
        STATE_SYSTEM_FLOATING = 0x1000,
        STATE_SYSTEM_FOCUSABLE = 0x100000,
        STATE_SYSTEM_FOCUSED = 4,
        STATE_SYSTEM_HOTTRACKED = 0x80,
        STATE_SYSTEM_INDETERMINATE = 0x20,
        STATE_SYSTEM_INVISIBLE = 0x8000,
        STATE_SYSTEM_LINKED = 0x400000,
        STATE_SYSTEM_MARQUEED = 0x2000,
        STATE_SYSTEM_MIXED = 0x20,
        STATE_SYSTEM_MOVEABLE = 0x40000,
        STATE_SYSTEM_MULTISELECTABLE = 0x1000000,
        STATE_SYSTEM_NORMAL = 0,
        STATE_SYSTEM_OFFSCREEN = 0x10000,
        STATE_SYSTEM_PRESSED = 8,
        STATE_SYSTEM_PROTECTED = 0x20000000,
        STATE_SYSTEM_READONLY = 0x40,
        STATE_SYSTEM_SELECTABLE = 0x200000,
        STATE_SYSTEM_SELECTED = 2,
        STATE_SYSTEM_SELFVOICING = 0x80000,
        STATE_SYSTEM_SIZEABLE = 0x20000,
        STATE_SYSTEM_TRAVERSED = 0x800000,
        STATE_SYSTEM_UNAVAILABLE = 1,
        STATE_SYSTEM_VALID = 0x7fffffff
    }

    internal static class OleAccHelperMethods
    {
        // Fields
        private static object[] _AccessibleChildren = new object[0x40];

        // Methods
        internal static void AccSelector_Execute(IAccessible accObj)
        {
            int num;
            RefreshAccessibleChildren(accObj, out num);
            int num2 = -1;
            for (int i = 0; i < num; i++)
            {
                if (!TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_SELECTED))
                {
                    num2 = i;
                    break;
                }
            }
            if (num2 >= 0)
            {
                try
                {
                    accObj.accSelect(8, num2 + 1);
                }
                catch (Exception exception)
                {
                    if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal static string AccSelector_Get(IAccessible accObj)
        {
            int num;
            StringBuilder builder = new StringBuilder();
            RefreshAccessibleChildren(accObj, out num);
            for (int i = 0; i < num; i++)
            {
                if (TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_SELECTED))
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(GetName(accObj, i + 1));
                }
            }
            return builder.ToString();
        }

        internal static void AccSelector_Set(IAccessible accObj, string controlValue)
        {
            int num;
            RefreshAccessibleChildren(accObj, out num);
            for (int i = 0; i < num; i++)
            {
                try
                {
                    if (controlValue.Length.Equals(0))
                    {
                        if (TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_SELECTED))
                        {
                            accObj.accSelect(0x10, i + 1);
                        }
                    }
                    else if (GetName(accObj, i + 1).Equals(controlValue))
                    {
                        if (TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_MULTISELECTABLE))
                        {
                            int flagsSelect = TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_SELECTED) ? 0x10 : 8;
                            accObj.accSelect(flagsSelect, i + 1);
                        }
                        else
                        {
                            int num4 = TestAccState(accObj, i + 1, OleAccStateSystem.STATE_SYSTEM_SELECTED) ? 0x10 : 8;
                            try
                            {
                                accObj.accSelect(num4, i + 1);
                            }
                            catch (ArgumentException)
                            {
                                if (num4 == 8)
                                {
                                    accObj.accSelect(2, i + 1);
                                }
                            }
                        }
                        break;
                    }
                }
                catch (Exception exception)
                {
                    if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal static string BuildIAccessibleKey(IAccessible accObj, int childId)
        {
            StringBuilder builder = new StringBuilder();
            IntPtr windowFromIAccessible = GetWindowFromIAccessible(accObj);
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3:X8}", new object[] { GetRoleId(accObj, childId), childId, GetName(accObj, childId), windowFromIAccessible.ToInt32() });
            Rectangle relAccLocation = GetRelAccLocation(accObj, childId);
            builder.AppendFormat(CultureInfo.InvariantCulture, ".{0},{1},{2},{3}", new object[] { relAccLocation.X, relAccLocation.Y, relAccLocation.Width, relAccLocation.Height });
            return builder.ToString();
        }

        private static string FixBstr(string bstr)
        {
            if (bstr == null)
            {
                return null;
            }
            int index = bstr.IndexOf('\0');
            if (index != -1)
            {
                return bstr.Substring(0, index);
            }
            return bstr;
        }

        internal static string GetAccExplorerLabel(IAccessible accObj, int childId)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetName(accObj, childId));
            builder.Append("[");
            builder.Append(GetRole(accObj, childId));
            builder.Append(" - ");
            builder.Append((GetStateValue(accObj, childId) == 0x8000) ? "Invisible" : "Visible");
            builder.Append("] ");
            builder.AppendFormat("hWnd:0x{0:X8}", GetWindowFromIAccessible(accObj).ToInt32());
            return builder.ToString();
        }

        internal static Rectangle GetAccLocation(IAccessible accObj, int childId)
        {
            int pxLeft = 0;
            int pyTop = 0;
            int pcxWidth = 0;
            int pcyHeight = 0;
            try
            {
                accObj.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            return new Rectangle(pxLeft, pyTop, pcxWidth, pcyHeight);
        }

        internal static int GetChildCount(IAccessible accObj)
        {
            int accChildCount = 0;
            try
            {
                accChildCount = accObj.accChildCount;
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            if (accChildCount >= 0)
            {
                return accChildCount;
            }
            return 0;
        }

        internal static string GetCookedValue(IAccessible accObj, int childId)
        {
            string name;
            switch (GetRoleId(accObj, childId))
            {
                case 0x2c:
                case 0x2d:
                    name = TestAccState(accObj, childId, OleAccStateSystem.STATE_SYSTEM_CHECKED) ? bool.TrueString : bool.FalseString;
                    break;

                default:
                    name = GetValue(accObj, childId);
                    if (name.Length.Equals(0))
                    {
                        name = GetName(accObj, childId);
                    }
                    break;
            }
            return (name ?? string.Empty);
        }

        internal static IAccessible GetIAccessibleFromWindow(IntPtr hWnd)
        {
            IAccessible ppvObject = null;
            if (!hWnd.Equals(IntPtr.Zero))
            {
                OleAccNativeMethods.AccessibleObjectFromWindow(hWnd, OleAccObjectId.OBJID_CLIENT, ref OleAccNativeMethods.IID_IAccessible, ref ppvObject);
            }
            return ppvObject;
        }

        internal static string GetName(IAccessible accObj, int childId)
        {
            string bstr = null;
            try
            {
                bstr = accObj.get_accName(childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            return (FixBstr(bstr) ?? string.Empty);
        }

        internal static void GetNextChildByName(string name, int count, int offset, ref IAccessible accObj, ref int childId)
        {
            if (accObj != null)
            {
                int num;
                RefreshAccessibleChildren(accObj, out num);
                int index = -1;
                for (int i = 0; (i < num) && (index < 0); i++)
                {
                    if (_AccessibleChildren[i] != null)
                    {
                        if (count == 0)
                        {
                            index = 0;
                        }
                        else if (_AccessibleChildren[i] is int)
                        {
                            if (GetName(accObj, Convert.ToInt32(_AccessibleChildren[i], CultureInfo.InvariantCulture)).Equals(name) && (--count <= 0))
                            {
                                index = i;
                            }
                        }
                        else if (((_AccessibleChildren[i] is IAccessible) && GetName(_AccessibleChildren[i] as IAccessible, childId).Equals(name)) && (--count <= 0))
                        {
                            index = i;
                        }
                    }
                }
                if (index >= 0)
                {
                    index = (index + offset) % num;
                    if (_AccessibleChildren[index] is int)
                    {
                        childId = Convert.ToInt32(_AccessibleChildren[index], CultureInfo.InvariantCulture);
                    }
                    else if (_AccessibleChildren[index] is IAccessible)
                    {
                        accObj = _AccessibleChildren[index] as IAccessible;
                    }
                    else
                    {
                        accObj = null;
                    }
                }
                else
                {
                    accObj = null;
                }
            }
        }

        internal static void GetNextChildByState(string state, int count, int offset, ref IAccessible accObj, ref int childId)
        {
            if (accObj != null)
            {
                int num;
                RefreshAccessibleChildren(accObj, out num);
                int index = -1;
                for (int i = 0; (i < num) && (index < 0); i++)
                {
                    if (_AccessibleChildren[i] != null)
                    {
                        if (count == 0)
                        {
                            index = 0;
                        }
                        else if (_AccessibleChildren[i] is int)
                        {
                            string str = GetState(accObj, Convert.ToInt32(_AccessibleChildren[i], CultureInfo.InvariantCulture));
                            if (str.Contains(state))
                            {
                                foreach (string str2 in str.Split(new char[] { ',' }))
                                {
                                    if (str2.Trim().Equals(state) && (--count <= 0))
                                    {
                                        index = i;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (_AccessibleChildren[i] is IAccessible)
                        {
                            string str3 = GetState(_AccessibleChildren[i] as IAccessible, childId);
                            if (str3.Contains(state))
                            {
                                foreach (string str4 in str3.Split(new char[] { ',' }))
                                {
                                    if (str4.Trim().Equals(state) && (--count <= 0))
                                    {
                                        index = i;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (index >= 0)
                {
                    index = (index + offset) % num;
                    if (_AccessibleChildren[index] is int)
                    {
                        childId = Convert.ToInt32(_AccessibleChildren[index], CultureInfo.InvariantCulture);
                    }
                    else if (_AccessibleChildren[index] is IAccessible)
                    {
                        accObj = _AccessibleChildren[index] as IAccessible;
                    }
                    else
                    {
                        accObj = null;
                    }
                }
                else
                {
                    accObj = null;
                }
            }
        }

        internal static Rectangle GetRelAccLocation(IAccessible accObj, int childId)
        {
            IntPtr windowFromIAccessible = GetWindowFromIAccessible(accObj);
            LPRECT lpRect = new LPRECT();
            Win32NativeMethods.GetWindowRect(windowFromIAccessible, ref lpRect);
            Rectangle accLocation = GetAccLocation(accObj, childId);
            return new Rectangle(lpRect.Left - accLocation.X, lpRect.Top - accLocation.Y, accLocation.Width, accLocation.Height);
        }

        internal static string GetRole(IAccessible accObj, int childId)
        {
            int roleId = GetRoleId(accObj, childId);
            if (roleId < 0)
            {
                return string.Empty;
            }
            return GetRoleText((uint)roleId);
        }

        internal static int GetRoleId(IAccessible accObj, int childId)
        {
            object obj2 = -1;
            try
            {
                obj2 = accObj.get_accRole(childId);
            }
            catch (NullReferenceException)
            {
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            if (obj2 is int)
            {
                return (int)obj2;
            }
            return 10;
        }

        internal static string GetRoleText(uint roleId)
        {
            StringBuilder lpszRole = new StringBuilder(0x80);
            OleAccNativeMethods.GetRoleText(roleId, lpszRole, (uint)lpszRole.Capacity);
            return lpszRole.ToString();
        }

        internal static string GetState(IAccessible accObj, int childId)
        {
            int state = -1;
            try
            {
                state = (int)accObj.get_accState(childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            return StringifyState(state);
        }

        internal static string GetStateText(uint stateBits)
        {
            StringBuilder lpszStateBit = new StringBuilder(0x80);
            OleAccNativeMethods.GetStateText(stateBits, lpszStateBit, (uint)lpszStateBit.Capacity);
            return lpszStateBit.ToString();
        }

        internal static int GetStateValue(IAccessible accObj, int childId)
        {
            int num = -1;
            try
            {
                num = (int)accObj.get_accState(childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
                return num;
            }
            return num;
        }

        internal static string GetValue(IAccessible accObj, int childId)
        {
            string bstr = null;
            try
            {
                bstr = accObj.get_accValue(childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            return (FixBstr(bstr) ?? string.Empty);
        }

        internal static IntPtr GetWindowFromIAccessible(IAccessible accObj)
        {
            IntPtr zero = IntPtr.Zero;
            if (accObj != null)
            {
                OleAccNativeMethods.WindowFromAccessibleObject(accObj, out zero);
            }
            return zero;
        }

        internal static bool IsOleAccException(Exception ex) =>
            ex.TargetSite.DeclaringType.GUID.Equals(OleAccNativeMethods.IID_IAccessible);

        internal static bool IsOleAccExceptionMaskable(Exception ex) =>
            ((!(ex is StackOverflowException) && !(ex is ThreadAbortException)) && ((!(ex is ThreadInterruptedException) && !(ex is ThreadStateException)) && !(ex is SecurityException)));

        internal static void PerformDefaultAction(IAccessible accObj, int childId)
        {
            try
            {
                accObj.accDoDefaultAction(childId);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
        }

        internal static void RefreshAccessibleChildren(IAccessible accObj, out int childCount)
        {
            int num;
            childCount = GetChildCount(accObj);
            if (childCount > _AccessibleChildren.Length)
            {
                _AccessibleChildren = new object[childCount];
            }
            if ((childCount > 0) && (OleAccNativeMethods.AccessibleChildren(accObj, 0, childCount, _AccessibleChildren, out num) == 1))
            {
                childCount = num;
            }
        }

        internal static bool SetCookedValue(IAccessible accObj, int childId, string controlValue)
        {
            switch (GetRoleId(accObj, childId))
            {
                case 0x2c:
                    {
                        bool flag2;
                        bool flag = TestAccState(accObj, childId, OleAccStateSystem.STATE_SYSTEM_CHECKED);
                        if (!bool.TryParse(controlValue, out flag2))
                        {
                            throw new DataDrivenAdapterException("DDA0110_VALUE_MUST_BE_TRUE_OR_FALSE");
                        }
                        if (flag ^ flag2)
                        {
                            PerformDefaultAction(accObj, childId);
                        }
                        return true;
                    }
                case 0x2d:
                    return false;
            }
            SetValue(accObj, childId, controlValue);
            return true;
        }

        internal static void SetValue(IAccessible accObj, int childId, string textValue)
        {
            try
            {
                accObj.set_accValue(childId, textValue);
            }
            catch (Exception exception)
            {
                if (!IsOleAccException(exception) || !IsOleAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
        }

        internal static string StringifyState(int state)
        {
            StringBuilder builder = new StringBuilder();
            if (state >= 0)
            {
                if (state == 0)
                {
                    builder.Append(GetStateText(0));
                }
                else
                {
                    for (uint i = 0x80000000; !i.Equals((uint)0); i = i >> 1)
                    {
                        uint stateBits = i & ((uint)state);
                        if (!stateBits.Equals((uint)0))
                        {
                            if (builder.Length > 0)
                            {
                                builder.Append(", ");
                            }
                            builder.Append(GetStateText(stateBits));
                        }
                    }
                }
            }
            return builder.ToString();
        }

        internal static bool TestAccState(int state, OleAccStateSystem mask)
        {
            if (mask != OleAccStateSystem.STATE_SYSTEM_NORMAL)
            {
                int num = state & (int)mask;
                return num.Equals((int)mask);
            }
            return state.Equals(0);
        }

        internal static bool TestAccState(IAccessible accObj, int childId, OleAccStateSystem mask) =>
            TestAccState(GetStateValue(accObj, childId), mask);
    }
}
