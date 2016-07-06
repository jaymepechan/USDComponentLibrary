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

namespace Microsoft.USD.ComponentLibrary.Adapters.Java
{
    using Uii.HostedApplicationToolkit.DataDrivenAdapter;
    using Microsoft.Win32;
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Text;
    using System.Threading;

    internal static class JavaAccHelperMethods
    {
        private static double requiredVMVersion = 1.6;
        private static int vmVersionLength = 3;

        internal static void ActivateAccHyperlink(System.IntPtr accObj, int vmId)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                AccessibleHypertextInfo hypertextInfo = new AccessibleHypertextInfo();
                try
                {
                    if (JavaAccNativeMethods.getAccessibleHypertext(vmId, accObj, out hypertextInfo))
                    {
                        AccessibleHyperlinkInfo hyperlinkInfo = new AccessibleHyperlinkInfo();
                        if (JavaAccNativeMethods.getAccessibleHyperlink(vmId, hypertextInfo.accessibleHypertext, 0, out hyperlinkInfo))
                        {
                            JavaAccNativeMethods.activateAccessibleHyperlink(vmId, accObj, hyperlinkInfo.accessibleHyperlink);
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal static string BuildAccKey(System.IntPtr accObj, int vmId)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    System.IntPtr ptr = JavaAccNativeMethods.getTopLevelObject(vmId, accObj);
                    System.IntPtr hWnd = JavaAccNativeMethods.getHWNDFromAccessibleContext(vmId, ptr);
                    builder.AppendFormat("{0}.{1}.{2:X8}", (int)GetRoleID(accObj, vmId), (int)vmId, (int)hWnd.ToInt32());
                    LPRECT lpRect = new LPRECT();
                    Win32NativeMethods.GetWindowRect(hWnd, ref lpRect);
                    AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                    JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                    int num = (int)(accContextInfo.x - lpRect.Left);
                    int num2 = (int)(accContextInfo.y - lpRect.Top);
                    int width = accContextInfo.width;
                    int height = accContextInfo.height;
                    builder.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, ".{0},{1},{2},{3}", new object[] { (int)num, (int)num2, (int)width, (int)height });
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
            return builder.ToString();
        }

        internal static void CheckVersion()
        {
            try
            {
                double num = 0.0;
                string str = string.Empty;
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(JavaDataDrivenAdapterConstants.JRE_REG_KEY.ToUpper(System.Globalization.CultureInfo.InvariantCulture));
                if (key == null)
                {
                    throw new DataDrivenAdapterException("Unable to retrieve Java version");
                }
                str = (string)((string)key.GetValue(JavaDataDrivenAdapterConstants.JRE_CURRENT_VER_KEY.ToUpper(System.Globalization.CultureInfo.InvariantCulture)));
                if (!string.IsNullOrEmpty(str))
                {
                    str = str.Substring(0, vmVersionLength);
                }
                double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out num);
                if (num < requiredVMVersion)
                {
                    throw new DataDrivenAdapterException("Unsupported Java VM version");
                }
            }
            catch (DataDrivenAdapterException)
            {
                throw;
            }
            catch (System.ArgumentException)
            {
                throw new DataDrivenAdapterException("Unable to retrieve Java version");
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw new DataDrivenAdapterException("Java Access Bridge not found");
                }
            }
        }

        internal static bool DoAction(System.IntPtr accObj, out int failure, int vmId, bool isDefaultAction, string actionName)
        {
            bool flag = false;
            failure = 0;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                AccessibleActionsToDo[] actionsToDo = new AccessibleActionsToDo[] { new AccessibleActionsToDo() };
                actionsToDo[0].actionsCount = 1;
                actionsToDo[0].actions = new AccessibleActionInfo[0x20];
                actionsToDo[0].actions[0] = new AccessibleActionInfo();
                if (isDefaultAction)
                {
                    actionsToDo[0].actions[0].name = JavaDataDrivenAdapterConstants.DEFAULT_ACTION_NAME;
                }
                else
                {
                    actionsToDo[0].actions[0].name = actionName;
                }
                try
                {
                    flag = JavaAccNativeMethods.doAccessibleActions(vmId, accObj, actionsToDo, out failure);
                    if (flag == false)
                        throw new Exception("doAccessibleActions failed: " + failure.ToString());
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return flag;
                }
            }
            return flag;
        }

        private static string GetAccessibleText(System.IntPtr accObj, int vmId)
        {
            string sentence = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                    JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                    if (accContextInfo.accessibleText)
                    {
                        AccessibleTextInfo accTextInfo = new AccessibleTextInfo();
                        if (JavaAccNativeMethods.getAccessibleTextInfo(vmId, accObj, out accTextInfo, 0, 0))
                        {
                            AccessibleTextItemsInfo accTextItems = new AccessibleTextItemsInfo();
                            if (JavaAccNativeMethods.getAccessibleTextItems(vmId, accObj, out accTextItems, accTextInfo.indexAtPoint))
                            {
                                sentence = accTextItems.sentence;
                            }
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return sentence;
                }
            }
            return sentence;
        }

        private static string GetAccessibleValue(System.IntPtr accObj, int vmId)
        {
            string str = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr zero = System.IntPtr.Zero;
                try
                {
                    zero = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(JavaDataDrivenAdapterConstants.TASK_MEMORY_SIZE);
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                        JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                        if (accContextInfo.accessibleInterfaces && JavaAccNativeMethods.getCurrentAccessibleValueFromContext(vmId, accObj, zero, JavaDataDrivenAdapterConstants.TASK_MEMORY_SIZE))
                        {
                            str = System.Runtime.InteropServices.Marshal.PtrToStringUni(zero);
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return str;
                }
                finally
                {
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem(zero);
                    }
                }
            }
            return str;
        }

        private static string GetAccessibleValueInt(System.IntPtr accObj, int vmId)
        {
            string str = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr zero = System.IntPtr.Zero;
                try
                {
                    zero = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(4);
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                        JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                        if (accContextInfo.accessibleInterfaces && JavaAccNativeMethods.getCurrentAccessibleValueFromContext(vmId, accObj, zero, 4))
                        {
                            str = System.Runtime.InteropServices.Marshal.PtrToStringUni(zero);
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return str;
                }
                finally
                {
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem(zero);
                    }
                }
            }
            return str;
        }

        internal static System.IntPtr GetAccFromWindow(System.IntPtr hWnd, out int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            vmId = 0;
            if (!hWnd.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    JavaAccNativeMethods.getAccessibleContextFromHWND(hWnd, out vmId, out zero);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return zero;
                }
            }
            return zero;
        }

        internal static string GetAccHyperlink(System.IntPtr accObj, int vmId)
        {
            string text = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                AccessibleHypertextInfo hypertextInfo = new AccessibleHypertextInfo();
                try
                {
                    if (JavaAccNativeMethods.getAccessibleHypertext(vmId, accObj, out hypertextInfo))
                    {
                        AccessibleHyperlinkInfo hyperlinkInfo = new AccessibleHyperlinkInfo();
                        if (JavaAccNativeMethods.getAccessibleHyperlink(vmId, hypertextInfo.accessibleHypertext, 0, out hyperlinkInfo))
                        {
                            text = hyperlinkInfo.text;
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return text;
                }
            }
            return text;
        }

        internal static string GetAccNodeLabel(System.IntPtr accObj, int vmId)
        {
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    builder.Append(GetName(accObj, vmId));
                    builder.Append(" [");
                    builder.Append(GetRole(accObj, vmId));
                    builder.Append("] ");
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
            return builder.ToString();
        }

        private static System.IntPtr GetAccSelection(System.IntPtr accObj, int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    if (JavaAccNativeMethods.getAccessibleSelectionCountFromContext(vmId, accObj) != -1)
                    {
                        zero = JavaAccNativeMethods.getAccessibleSelectionFromContext(vmId, accObj, 0);
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return zero;
                }
            }
            return zero;
        }

        internal static string GetAccSelectionName(System.IntPtr accObj, int vmId)
        {
            string name = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr accSelection = GetAccSelection(accObj, vmId);
                try
                {
                    if (!accSelection.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        name = GetName(accSelection, vmId);
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return name;
                }
                finally
                {
                    ReleaseObject(accSelection, vmId);
                }
            }
            return name;
        }

        private static System.IntPtr GetAccTable(System.IntPtr accObj, int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                AccessibleTableInfo accTableInfo = new AccessibleTableInfo();
                try
                {
                    if (JavaAccNativeMethods.getAccessibleTableInfo(vmId, accObj, out accTableInfo))
                    {
                        zero = accTableInfo.accessibleTable;
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return zero;
                }
            }
            return zero;
        }

        internal static int GetAccTableCellIndex(System.IntPtr accObj, int vmId, int row, int column)
        {
            int num = -1;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr accTable = GetAccTable(accObj, vmId);
                if (accTable.Equals((System.IntPtr)System.IntPtr.Zero))
                {
                    return num;
                }
                try
                {
                    num = JavaAccNativeMethods.getAccessibleTableIndex(vmId, accTable, row, column);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return num;
                }
                finally
                {
                    ReleaseObject(accTable, vmId);
                }
            }
            return num;
        }

        private static AccessibleTableCellInfo GetAccTableCellInfo(System.IntPtr accObj, int vmId, int index)
        {
            AccessibleTableCellInfo accTableCellInfo = new AccessibleTableCellInfo();
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr accTable = GetAccTable(accObj, vmId);
                try
                {
                    if (!accTable.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        int row = JavaAccNativeMethods.getAccessibleTableRow(vmId, accTable, index);
                        int column = JavaAccNativeMethods.getAccessibleTableColumn(vmId, accTable, index);
                        if (JavaAccNativeMethods.getAccessibleTableCellInfo(vmId, accTable, row, column, out accTableCellInfo))
                        {
                            return accTableCellInfo;
                        }
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return accTableCellInfo;
                }
                finally
                {
                    ReleaseObject(accTable, vmId);
                }
            }
            return accTableCellInfo;
        }

        internal static string GetAccTableCellValue(System.IntPtr accObj, int vmId, string cellIndex)
        {
            string str = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                int index = 0;
                if (!int.TryParse(cellIndex, out index))
                {
                    return str;
                }
                System.IntPtr accessibleContext = GetAccTableCellInfo(accObj, vmId, index).accessibleContext;
                try
                {
                    str = GetValue(accessibleContext, vmId);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return str;
                }
                finally
                {
                    ReleaseObject(accessibleContext, vmId);
                }
            }
            return str;
        }

        internal static System.IntPtr[] GetChildren(System.IntPtr accObj, int startIndex, int vmId, out int childrenCount)
        {
            childrenCount = 0;
            VisibleChildenInfo visibleChildrenInfo = new VisibleChildenInfo();
            try
            {
                JavaAccNativeMethods.getVisibleChildren(vmId, accObj, startIndex, out visibleChildrenInfo);
                childrenCount = visibleChildrenInfo.returnedChildrenCount;
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
            return visibleChildrenInfo.children;
        }

        internal static int GetChildrenCount(System.IntPtr accObj, int vmId)
        {
            int childrenCount = 0;
            try
            {
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo))
                {
                    childrenCount = accContextInfo.childrenCount;
                }
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
                return childrenCount;
            }
            return childrenCount;
        }

        internal static System.IntPtr GetHWNDFromAccessibleContext(System.IntPtr accObj, int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            System.IntPtr ptr2 = System.IntPtr.Zero;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    zero = JavaAccNativeMethods.getTopLevelObject(vmId, accObj);
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        ptr2 = JavaAccNativeMethods.getHWNDFromAccessibleContext(vmId, zero);
                    }
                }
                finally
                {
                    ReleaseObject(zero, vmId);
                }
            }
            return ptr2;
        }

        internal static string GetName(System.IntPtr accObj, int vmId)
        {
            string str = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                try
                {
                    if (JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo) && !string.IsNullOrEmpty(accContextInfo.name))
                    {
                        return accContextInfo.name;
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return str;
                }
            }
            return str;
        }

        internal static System.IntPtr GetNextChildByName(string name, ref int count, int offset, ref bool nodeFound, System.IntPtr accObj, int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            bool flag = false;
            try
            {
                if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
                {
                    if (GetName(accObj, vmId).Equals(name) && (--count <= 0))
                    {
                        flag = true;
                        return accObj;
                    }
                    AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                    if (!JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo))
                    {
                        return zero;
                    }
                    for (int i = 0; i < accContextInfo.childrenCount; i = (int)(i + 1))
                    {
                        System.IntPtr ptr2 = JavaAccNativeMethods.getAccessibleChildFromContext(vmId, accObj, i);
                        zero = GetNextChildByName(name, ref count, offset, ref nodeFound, ptr2, vmId);
                        if (!nodeFound && !zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            nodeFound = true;
                            if (offset != 0)
                            {
                                if (((i + offset) >= 0) && ((i + offset) < accContextInfo.childrenCount))
                                {
                                    zero = JavaAccNativeMethods.getAccessibleChildFromContext(vmId, accObj, (int)(i + offset));
                                }
                                else
                                {
                                    zero = System.IntPtr.Zero;
                                }
                            }
                        }
                        if (nodeFound)
                        {
                            return zero;
                        }
                    }
                }
                return zero;
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
                return zero;
            }
            finally
            {
                if (!flag || (offset != 0))
                {
                    ReleaseObject(accObj, vmId);
                }
            }
            return zero;
        }

        internal static System.IntPtr GetNextChildByRole(string role, ref int count, int offset, ref bool nodeFound, System.IntPtr accObj, int vmId)
        {
            System.IntPtr zero = System.IntPtr.Zero;
            bool flag = false;
            try
            {
                if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
                {
                    if (GetRole(accObj, vmId).Equals(role.Trim(), System.StringComparison.OrdinalIgnoreCase) && (--count <= 0))
                    {
                        flag = true;
                        return accObj;
                    }
                    AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                    if (!JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo))
                    {
                        return zero;
                    }
                    for (int i = 0; i < accContextInfo.childrenCount; i = (int)(i + 1))
                    {
                        System.IntPtr ptr2 = JavaAccNativeMethods.getAccessibleChildFromContext(vmId, accObj, i);
                        zero = GetNextChildByRole(role, ref count, offset, ref nodeFound, ptr2, vmId);
                        if (!nodeFound && !zero.Equals((System.IntPtr)System.IntPtr.Zero))
                        {
                            nodeFound = true;
                            if (offset != 0)
                            {
                                if (((i + offset) >= 0) && ((i + offset) < accContextInfo.childrenCount))
                                {
                                    zero = JavaAccNativeMethods.getAccessibleChildFromContext(vmId, accObj, (int)(i + offset));
                                }
                                else
                                {
                                    zero = System.IntPtr.Zero;
                                }
                            }
                        }
                        if (nodeFound)
                        {
                            return zero;
                        }
                    }
                }
                return zero;
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
                return zero;
            }
            finally
            {
                if (!flag || (offset != 0))
                {
                    ReleaseObject(accObj, vmId);
                }
            }
            return zero;
        }

        internal static string GetRole(System.IntPtr accObj, int vmId)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                bool flag = false;
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                try
                {
                    flag = JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
                if (flag && !string.IsNullOrEmpty(accContextInfo.role))
                {
                    return accContextInfo.role_en_US;
                }
            }
            return string.Empty;
        }

        internal static int GetRoleID(System.IntPtr accObj, int vmId)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                bool flag = false;
                AccessibleContextInfo accContextInfo = new AccessibleContextInfo();
                try
                {
                    flag = JavaAccNativeMethods.getAccessibleContextInfo(vmId, accObj, out accContextInfo);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
                if (flag && !string.IsNullOrEmpty(accContextInfo.role))
                {
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.PUSH_BUTTON, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 0;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.TOGGLE_BUTTON, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 6;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.CHECK_BOX, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 1;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.RADIO_BUTTON, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 2;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.COMBO_BOX, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 3;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.TEXT, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 4;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.LABEL, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 5;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.MENU, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 7;
                    }
                    if (accContextInfo.role.Equals(JavaDataDrivenAdapterConstants.MENU_ITEM, System.StringComparison.OrdinalIgnoreCase))
                    {
                        return 8;
                    }
                }
            }
            return -1;
        }

        internal static string GetValue(System.IntPtr accObj, int vmId)
        {
            string name = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    string str2;
                    switch (GetRoleID(accObj, vmId))
                    {
                        case 0:
                        case 5:
                            return GetName(accObj, vmId);

                        case 1:
                        case 2:
                        case 6:
                        case 7:
                        case 8:
                            name = GetAccessibleValueInt(accObj, vmId);
                            if (!name.Equals("1"))
                            {
                                break;
                            }
                            return bool.TrueString;

                        case 4:
                            name = GetAccessibleText(accObj, vmId);
                            if (!string.IsNullOrEmpty(name) && ((char)name[(int)(name.Length - 1)]).Equals('\n'))
                            {
                                name = name.Substring(0, (int)(name.Length - 1));
                            }
                            return name;

                        default:
                            goto Label_00D5;
                    }
                    if (name.Equals("0"))
                    {
                        name = bool.FalseString;
                    }
                    return name;
                Label_00D5:
                    str2 = String.Empty; // GetAccessibleValue(accObj, vmId);
                    if (!string.IsNullOrEmpty(str2))
                    {
                        return str2;
                    }
                    name = GetName(accObj, vmId);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return name;
                }
            }
            return name;
        }

        private static string GetVirtualAccessibleName(System.IntPtr accObj, int vmId)
        {
            string str = string.Empty;
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr zero = System.IntPtr.Zero;
                try
                {
                    zero = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(JavaDataDrivenAdapterConstants.TASK_MEMORY_SIZE);
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero) && JavaAccNativeMethods.getVirtualAccessibleName(vmId, accObj, zero, JavaDataDrivenAdapterConstants.TASK_MEMORY_SIZE))
                    {
                        str = System.Runtime.InteropServices.Marshal.PtrToStringUni(zero);
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                    return str;
                }
                finally
                {
                    if (!zero.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem(zero);
                    }
                }
            }
            return str;
        }

        internal static int GetVisibleChildrenCount(System.IntPtr accObj, int vmId)
        {
            int num = 0;
            try
            {
                num = JavaAccNativeMethods.getVisibleChildrenCount(vmId, accObj);
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
                return num;
            }
            return num;
        }

        internal static void InitializeJavaAccessBridge(bool isIgnoreVersionCheck)
        {
            if (!isIgnoreVersionCheck)
            {
                CheckVersion();
            }
        }

        internal static bool IsJavaAccException(System.Exception ex)
        {
            return ex.TargetSite.DeclaringType.GUID.Equals(JavaDataDrivenAdapterConstants.IID_JavaAcc);
        }

        internal static bool IsJavaAccExceptionMaskable(System.Exception ex)
        {
            bool flag = true;
            return (bool)((!(ex is System.SystemException) || (((!(ex is System.OutOfMemoryException) && !(ex is System.StackOverflowException)) && (!(ex is System.Threading.ThreadAbortException) && !(ex is System.Threading.ThreadInterruptedException))) && (!(ex is System.Threading.ThreadStateException) && !(ex is System.Security.SecurityException)))) && flag);
        }

        internal static bool IsWow64ProcessMode(System.IntPtr hwnd)
        {
            int lpdwProcessId = 0;
            bool lpSystemInfo = false;
            Win32NativeMethods.GetWindowThreadProcessId(hwnd, out lpdwProcessId);
            Win32NativeMethods.IsWow64Process(Process.GetProcessById(lpdwProcessId).Handle, out lpSystemInfo);
            return lpSystemInfo;
        }

        internal static void ReleaseObject(System.IntPtr accObj, int vmId)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                try
                {
                    JavaAccNativeMethods.releaseJavaObject(vmId, accObj);
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        Trace.WriteLine("Unable to free memory associated with the java accessible node.");
                    }
                }
            }
        }

        internal static void SetAccSelection(System.IntPtr accSelection, int vmId, string controlValue)
        {
            int num = 0;
            if (!accSelection.Equals((System.IntPtr)System.IntPtr.Zero) && int.TryParse(controlValue, out num))
            {
                try
                {
                    if (num <= 0)
                    {
                        throw new System.ArgumentException("Incorrect Control Value");
                    }
                    JavaAccNativeMethods.addAccessibleSelectionFromContext(vmId, accSelection, (int)(num - 1));
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
            }
        }

        internal static void SetAccTableCellValue(System.IntPtr accObj, int vmId, string cellIndex, string cellValue)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                int index = 0;
                if (int.TryParse(cellIndex, out index))
                {
                    System.IntPtr accessibleContext = GetAccTableCellInfo(accObj, vmId, index).accessibleContext;
                    try
                    {
                        SetValue(accessibleContext, vmId, cellValue);
                    }
                    catch (System.Exception exception)
                    {
                        if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        ReleaseObject(accessibleContext, vmId);
                    }
                }
            }
        }

        private static void SetControlValue(System.IntPtr accObj, int vmId, string controlValue)
        {
            if (!accObj.Equals((System.IntPtr)System.IntPtr.Zero))
            {
                System.IntPtr pControlValue = System.Runtime.InteropServices.Marshal.StringToCoTaskMemUni(controlValue);
                try
                {
                    if (!pControlValue.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        JavaAccNativeMethods.setTextContents(vmId, accObj, pControlValue);
                    }
                }
                catch (System.Exception exception)
                {
                    if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                    {
                        throw;
                    }
                }
                finally
                {
                    if (!pControlValue.Equals((System.IntPtr)System.IntPtr.Zero))
                    {
                        System.Runtime.InteropServices.Marshal.FreeCoTaskMem(pControlValue);
                    }
                }
            }
        }

        internal static void SetValue(System.IntPtr accObj, int vmId, string controlValue)
        {
            try
            {
                if (string.IsNullOrEmpty(controlValue))
                {
                    controlValue = string.Empty;
                }
                SetControlValue(accObj, vmId, controlValue);
            }
            catch (System.Exception exception)
            {
                if (!IsJavaAccException(exception) || !IsJavaAccExceptionMaskable(exception))
                {
                    throw;
                }
            }
        }
    }
}
