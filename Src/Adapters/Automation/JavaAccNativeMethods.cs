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
    using Uii.HostedApplicationToolkit.DataDrivenAdapter;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Text;

    #region imports
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    internal struct LPRECT
    {
        internal int Left;
        internal int Top;
        internal int Right;
        internal int Bottom;
    }

    [System.Flags]
    internal enum SendMessageTimeoutFlags
    {
        SMTO_ABORTIFHUNG = 2,
        SMTO_BLOCK = 1,
        SMTO_NORMAL = 0,
        SMTO_NOTIMEOUTIFNOTHUNG = 8
    }

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool initializeAccessBridgeDelegate();

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool activateAccessibleHyperlinkDelegate(int vmId, System.IntPtr accObj, System.IntPtr accessibleHyperlink);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void addAccessibleSelectionFromContextDelegate(int vmId, System.IntPtr accSelection, int i);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void clearAccessibleSelectionFromContextDelegate(int vmId, System.IntPtr accSelection);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public struct AccessibleActionInfo
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public String name;
    }
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public struct AccessibleActionsToDo
    {
        public int actionsCount;
        [System.Runtime.InteropServices.MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x20)]
        public AccessibleActionInfo[] actions;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool doAccessibleActionsDelegate(int vmId, System.IntPtr accObj, AccessibleActionsToDo[] actionsToDo, out int failure);


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct AccessibleActions
    {
        [MarshalAs(UnmanagedType.I4)]
        public int actionsCount;    // number of actions to do  

        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.LPStruct, SizeConst = 0x100)]
        public AccessibleActionInfo[] actions;// the accessible actions to do  
    };

    //[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    //public struct AccessibleActions
    //{
    //    public int actionsCount;
    //    [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Struct)]
    //    public AccessibleActionInfo actionInfo;
    //}
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleActionsDelegate(int vmId, System.IntPtr accObj);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleActionItemDelegate(int index, System.IntPtr action, int len);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getAccessibleChildFromContextDelegate(int vmId, System.IntPtr accObj, int index);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleContextAtDelegate(int vmId, System.IntPtr acParent, int x, int y, out System.IntPtr accObj);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleContextFromHWNDDelegate(System.IntPtr hWnd, out int vmId, out System.IntPtr accObj);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public struct AccessibleContextInfo
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x400)]
        public string name;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x400)]
        public string description;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string role;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string role_en_US;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string states;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string states_en_US;
        public int indexInParent;
        public int childrenCount;
        public int x;
        public int y;
        public int width;
        public int height;
        public bool accessibleComponent;
        public bool accessibleAction;
        public bool accessibleSelection;
        public bool accessibleText;
        public bool accessibleInterfaces;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleContextInfoDelegate(int vmId, System.IntPtr accObj, out AccessibleContextInfo accContextInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleContextWithFocusDelegate(System.IntPtr window, out int vmId, out System.IntPtr accObj);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public struct AccessibleHyperlinkInfo
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        public string text;
        public int startIndex;
        public int endIndex;
        public System.IntPtr accessibleHyperlink;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleHyperlinkDelegate(int vmId, System.IntPtr hypertext, int index, out AccessibleHyperlinkInfo hyperlinkInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleHyperlinkCountDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    public struct AccessibleHypertextInfo
    {
        public int linkCount;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Struct)]
        public AccessibleHyperlinkInfo links;
        public System.IntPtr accessibleHypertext;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleHypertextDelegate(int vmId, System.IntPtr accObj, out AccessibleHypertextInfo hypertextInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getAccessibleParentFromContextDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleSelectionCountFromContextDelegate(int vmId, System.IntPtr accSelection);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getAccessibleSelectionFromContextDelegate(int vmId, System.IntPtr accSelection, int i);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTableCellInfo
    {
        internal System.IntPtr accessibleContext;
        internal int index;
        internal int row;
        internal int column;
        internal int rowExtent;
        internal int columnExtent;
        internal bool isSelected;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTableCellInfoDelegate(int vmId, System.IntPtr accessibleTable, int row, int column, out AccessibleTableCellInfo accTableCellInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleTableColumnDelegate(int vmId, System.IntPtr table, int index);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleTableColumnSelectionCountDelegate(int vmId, System.IntPtr table);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTableColumnSelectionsDelegate(int vmId, System.IntPtr table, int count, int[] selections);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleTableIndexDelegate(int vmId, System.IntPtr table, int row, int column);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTableInfo
    {
        internal System.IntPtr caption;
        internal System.IntPtr summary;
        internal int rowCount;
        internal int columnCount;
        internal System.IntPtr accessibleContext;
        internal System.IntPtr accessibleTable;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTableInfoDelegate(int vmId, System.IntPtr accObj, out AccessibleTableInfo accTableInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleTableRowDelegate(int vmId, System.IntPtr table, int index);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getAccessibleTableRowSelectionCountDelegate(int vmId, System.IntPtr table);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTableRowSelectionsDelegate(int vmId, System.IntPtr table, int count, int[] selections);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTextAttributesInfo
    {
        internal bool bold;
        internal bool italic;
        internal bool underline;
        internal bool strikethrough;
        internal bool superscript;
        internal bool subscript;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string backgroundColor;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string foregroundColor;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string fontFamily;
        internal int fontSize;
        internal int alignment;
        internal int bidiLevel;
        internal float firstLineIndent;
        internal float leftIndent;
        internal float rightIndent;
        internal float lineSpacing;
        internal float spaceAbove;
        internal float spaceBelow;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x400)]
        internal string fullAttributesString;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTextAttributesDelegate(int vmId, System.IntPtr at, int index, out AccessibleTextAttributesInfo attributes);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTextInfo
    {
        internal int charCount;
        internal int caretIndex;
        internal int indexAtPoint;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTextInfoDelegate(int vmId, System.IntPtr accObj, out AccessibleTextInfo accTextInfo, int x, int y);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTextItemsInfo
    {
        internal char letter;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string word;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x400)]
        internal string sentence;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTextItemsDelegate(int vmId, System.IntPtr accObj, out AccessibleTextItemsInfo accTextItems, int index);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTextRectInfo
    {
        internal int x;
        internal int y;
        internal int width;
        internal int height;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTextRectDelegate(int vmId, System.IntPtr at, out AccessibleTextRectInfo rectInfo, int index);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct AccessibleTextSelectionInfo
    {
        internal int selectionStartIndex;
        internal int selectionEndIndex;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x400)]
        internal string selectedText;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getAccessibleTextSelectionInfoDelegate(int vmId, System.IntPtr at, out AccessibleTextSelectionInfo textSelection);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getActiveDescendentDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getCaretLocationDelegate(int vmId, System.IntPtr accObj, out AccessibleTextRectInfo rectInfo, int index);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getCurrentAccessibleValueFromContextDelegate(int vmId, System.IntPtr accObj, System.IntPtr accValue, int length);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getEventsWaitingDelegate();

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getHWNDFromAccessibleContextDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getParentWithRoleDelegate(int vmId, System.IntPtr accObj, string role);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate System.IntPtr getTopLevelObjectDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct VersionInfo
    {
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string VMversion;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string bridgeJavaClassVersion;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string bridgeJavaDLLVersion;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValTStr, SizeConst = 0x100)]
        internal string bridgeWinDLLVersion;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void getVersionInfoDelegate(int vmId, out VersionInfo versionInfo);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getVirtualAccessibleNameDelegate(int vmId, System.IntPtr accObj, System.IntPtr accNameString, int length);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate int getVisibleChildrenCountDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
    internal struct VisibleChildenInfo
    {
        internal int returnedChildrenCount;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray, SizeConst = 0x100)]
        internal System.IntPtr[] children;
    }
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool getVisibleChildrenDelegate(int vmId, System.IntPtr accObj, int startIndex, out VisibleChildenInfo visibleChildrenInfo);

    internal enum HookType
    {
        WH_CALLWNDPROC = 4,
        WH_CALLWNDPROCRET = 12,
        WH_CBT = 5,
        WH_DEBUG = 9,
        WH_FOREGROUNDIDLE = 11,
        WH_GETMESSAGE = 3,
        WH_HARDWARE = 8,
        WH_JOURNALPLAYBACK = 1,
        WH_JOURNALRECORD = 0,
        WH_KEYBOARD = 2,
        WH_KEYBOARD_LL = 13,
        WH_MAX = 14,
        WH_MIN = -1,
        WH_MOUSE = 7,
        WH_MOUSE_LL = 14,
        WH_MSGFILTER = -1,
        WH_SHELL = 10,
        WH_SYSMSGFILTER = 6
    }
    internal delegate int HookProc(int code, System.IntPtr wParam, System.IntPtr lParam);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool isAccessibleTableColumnSelectedDelegate(int vmId, System.IntPtr table, int column);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool isAccessibleTableRowSelectedDelegate(int vmId, System.IntPtr table, int row);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool isJavaWindowDelegate(System.IntPtr hWnd);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool isSameObjectDelegate(int vmId, System.IntPtr obj1, System.IntPtr obj2);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void releaseJavaObjectDelegate(int vmId, System.IntPtr jObject);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool requestFocusDelegate(int vmId, System.IntPtr accObj);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate bool setCaretPositionDelegate(int vmId, System.IntPtr accObj, int position);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaCaretUpdateEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setCaretUpdateFPDelegate(JavaCaretUpdateEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setFocusGainedFPDelegate(JavaFocusGainEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaFocusGainEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaFocusLostEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setFocusLostFPDelegate(JavaFocusLostEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaShutdownEventHandler(int vmId);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setJavaShutdownFPDelegate(JavaShutdownEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMenuCancelEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMenuCanceledFPDelegate(JavaMenuCancelEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMenuDeselectEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMenuDeselectedFPDelegate(JavaMenuDeselectEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMenuSelectEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMenuSelectedFPDelegate(JavaMenuSelectEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMouseClickEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMouseClickedFPDelegate(JavaMouseClickEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMouseEnterEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMouseEnteredFPDelegate(JavaMouseEnterEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMouseExitEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMouseExitedFPDelegate(JavaMouseExitEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMousePressEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMousePressedFPDelegate(JavaMousePressEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaMouseReleaseEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setMouseReleasedFPDelegate(JavaMouseReleaseEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPopupMenuCanceledEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPopupMenuCanceledFPDelegate(JavaPopupMenuCanceledEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPopupMenuInvisibleEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPopupMenuWillBecomeInvisibleFPDelegate(JavaPopupMenuInvisibleEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPopupMenuVisibleEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPopupMenuWillBecomeVisibleFPDelegate(JavaPopupMenuVisibleEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyActiveDescendentChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldActiveDescendent, System.IntPtr newActiveDescendent);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyActiveDescendentChangeFPDelegate(JavaPropertyActiveDescendentChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyCaretChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, int oldPosition, int newPosition);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyCaretChangeFPDelegate(JavaPropertyCaretChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyChildChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldChild, System.IntPtr newChild);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyChildChangeFPDelegate(JavaPropertyChildChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyDescriptionChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldDescription, System.IntPtr newDescription);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyDescriptionChangeFPDelegate(JavaPropertyDescriptionChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyNameChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldName, System.IntPtr newName);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyNameChangeFPDelegate(JavaPropertyNameChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertySelectionChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertySelectionChangeFPDelegate(JavaPropertySelectionChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyStateChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldState, System.IntPtr newState);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyStateChangeFPDelegate(JavaPropertyStateChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyTableModelChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldValue, System.IntPtr newValue);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyTableModelChangeFPDelegate(JavaPropertyTableModelChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyTextChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyTextChangeFPDelegate(JavaPropertyTextChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyValueChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source, System.IntPtr oldValue, System.IntPtr newValue);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyValueChangeFPDelegate(JavaPropertyValueChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void JavaPropertyVisibleDataChangeEventHandler(int vmId, System.IntPtr jEvent, System.IntPtr source);
    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setPropertyVisibleDataChangeFPDelegate(JavaPropertyVisibleDataChangeEventHandler handler);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void setTextContentsDelegate(int vmId, System.IntPtr accObj, System.IntPtr pControlValue);

    [System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    internal delegate void Windows_runDelegate();
    #endregion

    //internal static class JavaAccNativeMethods
    //{
    //    internal static activateAccessibleHyperlinkDelegate activateAccessibleHyperlink;
    //    internal static addAccessibleSelectionFromContextDelegate addAccessibleSelectionFromContext;
    //    internal static clearAccessibleSelectionFromContextDelegate clearAccessibleSelectionFromContext;
    //    internal static doAccessibleActionsDelegate doAccessibleActions;
    //    internal static System.Collections.Generic.List<System.Reflection.FieldInfo> fieldsInfo = null;
    //    internal static getAccessibleActionsDelegate getAccessibleActions;
    //    internal static getAccessibleChildFromContextDelegate getAccessibleChildFromContext;
    //    internal static getAccessibleContextAtDelegate getAccessibleContextAt;
    //    internal static getAccessibleContextFromHWNDDelegate getAccessibleContextFromHWND;
    //    internal static getAccessibleContextInfoDelegate getAccessibleContextInfo;
    //    internal static getAccessibleContextWithFocusDelegate getAccessibleContextWithFocus;
    //    internal static getAccessibleHyperlinkDelegate getAccessibleHyperlink;
    //    internal static getAccessibleHyperlinkCountDelegate getAccessibleHyperlinkCount;
    //    internal static getAccessibleHypertextDelegate getAccessibleHypertext;
    //    internal static getAccessibleParentFromContextDelegate getAccessibleParentFromContext;
    //    internal static getAccessibleSelectionCountFromContextDelegate getAccessibleSelectionCountFromContext;
    //    internal static getAccessibleSelectionFromContextDelegate getAccessibleSelectionFromContext;
    //    internal static getAccessibleTableCellInfoDelegate getAccessibleTableCellInfo;
    //    internal static getAccessibleTableColumnDelegate getAccessibleTableColumn;
    //    internal static getAccessibleTableColumnSelectionCountDelegate getAccessibleTableColumnSelectionCount;
    //    internal static getAccessibleTableColumnSelectionsDelegate getAccessibleTableColumnSelections;
    //    internal static getAccessibleTableIndexDelegate getAccessibleTableIndex;
    //    internal static getAccessibleTableInfoDelegate getAccessibleTableInfo;
    //    internal static getAccessibleTableRowDelegate getAccessibleTableRow;
    //    internal static getAccessibleTableRowSelectionCountDelegate getAccessibleTableRowSelectionCount;
    //    internal static getAccessibleTableRowSelectionsDelegate getAccessibleTableRowSelections;
    //    internal static getAccessibleTextAttributesDelegate getAccessibleTextAttributes;
    //    internal static getAccessibleTextInfoDelegate getAccessibleTextInfo;
    //    internal static getAccessibleTextItemsDelegate getAccessibleTextItems;
    //    internal static getAccessibleTextRectDelegate getAccessibleTextRect;
    //    internal static getAccessibleTextSelectionInfoDelegate getAccessibleTextSelectionInfo;
    //    internal static getActiveDescendentDelegate getActiveDescendent;
    //    internal static getCaretLocationDelegate getCaretLocation;
    //    internal static getCurrentAccessibleValueFromContextDelegate getCurrentAccessibleValueFromContext;
    //    internal static getEventsWaitingDelegate getEventsWaiting;
    //    internal static getHWNDFromAccessibleContextDelegate getHWNDFromAccessibleContext;
    //    internal static getParentWithRoleDelegate getParentWithRole;
    //    internal static getTopLevelObjectDelegate getTopLevelObject;
    //    internal static getVersionInfoDelegate getVersionInfo;
    //    internal static getVirtualAccessibleNameDelegate getVirtualAccessibleName;
    //    internal static getVisibleChildrenDelegate getVisibleChildren;
    //    internal static getVisibleChildrenCountDelegate getVisibleChildrenCount;
    //    internal static isAccessibleTableColumnSelectedDelegate isAccessibleTableColumnSelected;
    //    internal static isAccessibleTableRowSelectedDelegate isAccessibleTableRowSelected;
    //    internal static isJavaWindowDelegate isJavaWindow;
    //    internal static isSameObjectDelegate isSameObject;
    //    internal static System.Collections.Generic.List<System.Exception> loopExceptions = null;
    //    internal static releaseJavaObjectDelegate releaseJavaObject;
    //    internal static requestFocusDelegate requestFocus;
    //    internal static setCaretPositionDelegate setCaretPosition;
    //    internal static setCaretUpdateFPDelegate setCaretUpdateFP;
    //    internal static setFocusGainedFPDelegate setFocusGainedFP;
    //    internal static setFocusLostFPDelegate setFocusLostFP;
    //    internal static setJavaShutdownFPDelegate setJavaShutdownFP;
    //    internal static setMenuCanceledFPDelegate setMenuCanceledFP;
    //    internal static setMenuDeselectedFPDelegate setMenuDeselectedFP;
    //    internal static setMenuSelectedFPDelegate setMenuSelectedFP;
    //    internal static setMouseClickedFPDelegate setMouseClickedFP;
    //    internal static setMouseEnteredFPDelegate setMouseEnteredFP;
    //    internal static setMouseExitedFPDelegate setMouseExitedFP;
    //    internal static setMousePressedFPDelegate setMousePressedFP;
    //    internal static setMouseReleasedFPDelegate setMouseReleasedFP;
    //    internal static setPopupMenuCanceledFPDelegate setPopupMenuCanceledFP;
    //    internal static setPopupMenuWillBecomeInvisibleFPDelegate setPopupMenuWillBecomeInvisibleFP;
    //    internal static setPopupMenuWillBecomeVisibleFPDelegate setPopupMenuWillBecomeVisibleFP;
    //    internal static setPropertyActiveDescendentChangeFPDelegate setPropertyActiveDescendentChangeFP;
    //    internal static setPropertyCaretChangeFPDelegate setPropertyCaretChangeFP;
    //    internal static setPropertyChildChangeFPDelegate setPropertyChildChangeFP;
    //    internal static setPropertyDescriptionChangeFPDelegate setPropertyDescriptionChangeFP;
    //    internal static setPropertyNameChangeFPDelegate setPropertyNameChangeFP;
    //    internal static setPropertySelectionChangeFPDelegate setPropertySelectionChangeFP;
    //    internal static setPropertyStateChangeFPDelegate setPropertyStateChangeFP;
    //    internal static setPropertyTableModelChangeFPDelegate setPropertyTableModelChangeFP;
    //    internal static setPropertyTextChangeFPDelegate setPropertyTextChangeFP;
    //    internal static setPropertyValueChangeFPDelegate setPropertyValueChangeFP;
    //    internal static setPropertyVisibleDataChangeFPDelegate setPropertyVisibleDataChangeFP;
    //    internal static setTextContentsDelegate setTextContents;
    //    internal static Windows_runDelegate Windows_run;

    //    static JavaAccNativeMethods()
    //    {
    //        fieldsInfo = (from x in typeof(JavaAccNativeMethods).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
    //                      where (bool)((x.FieldType.BaseType == typeof(System.MulticastDelegate)) && x.IsAssembly)
    //                      select x).ToList<System.Reflection.FieldInfo>();
    //    }

    //    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    //    static extern bool FreeLibrary(System.IntPtr hModule);
    //    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    //    static extern System.IntPtr GetProcAddress(System.IntPtr hModule, string procedureName);
    //    public static void LoadJavaAccessBridge(string windowsAccessBridgeDll)
    //    {
    //        System.IntPtr pDll = LoadLibrary(windowsAccessBridgeDll);
    //        if (pDll == System.IntPtr.Zero)
    //        {
    //            System.Exception exceptionForHR = System.Runtime.InteropServices.Marshal.GetExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
    //            throw new Win32Exception(exceptionForHR.Message, exceptionForHR.InnerException);
    //        }
    //        loopExceptions = new System.Collections.Generic.List<System.Exception>();
    //        foreach (System.Reflection.FieldInfo info in fieldsInfo)
    //        {
    //            try
    //            {
    //                LoadJavaAccessBridgeMethod(info, pDll);
    //            }
    //            catch (System.Exception exception2)
    //            {
    //                loopExceptions.Add(exception2);
    //            }
    //        }
    //        if (loopExceptions.Count > 0)
    //        {
    //            System.Text.StringBuilder builder = new System.Text.StringBuilder();
    //            foreach (System.Exception exception3 in loopExceptions)
    //            {
    //                builder.AppendLine(exception3.Message);
    //            }
    //            throw new DataDrivenAdapterException(builder.ToString());
    //        }
    //    }

    //    static void LoadJavaAccessBridgeMethod(System.Reflection.FieldInfo delegateInstance, System.IntPtr pDll)
    //    {
    //        System.IntPtr procAddress = GetProcAddress(pDll, delegateInstance.Name);
    //        if (pDll == System.IntPtr.Zero)
    //        {
    //            throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
    //        }
    //        delegateInstance.SetValue(null, System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(procAddress, delegateInstance.FieldType));
    //    }

    //    [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
    //    static extern System.IntPtr LoadLibrary(string dllToLoad);
    //}

    internal static class JavaAccNativeMethods
    {
        internal static activateAccessibleHyperlinkDelegate activateAccessibleHyperlink;
        internal static addAccessibleSelectionFromContextDelegate addAccessibleSelectionFromContext;
        internal static clearAccessibleSelectionFromContextDelegate clearAccessibleSelectionFromContext;
        internal static doAccessibleActionsDelegate doAccessibleActions;
        internal static System.Collections.Generic.List<System.Reflection.FieldInfo> fieldsInfo = null;
        internal static getAccessibleActionsDelegate getAccessibleActions;
        internal static getAccessibleActionItemDelegate getAccessibleActionItem;
        internal static getAccessibleChildFromContextDelegate getAccessibleChildFromContext;
        internal static getAccessibleContextAtDelegate getAccessibleContextAt;
        internal static getAccessibleContextFromHWNDDelegate getAccessibleContextFromHWND;
        internal static getAccessibleContextInfoDelegate getAccessibleContextInfo;
        internal static getAccessibleContextWithFocusDelegate getAccessibleContextWithFocus;
        internal static getAccessibleHyperlinkDelegate getAccessibleHyperlink;
        internal static getAccessibleHyperlinkCountDelegate getAccessibleHyperlinkCount;
        internal static getAccessibleHypertextDelegate getAccessibleHypertext;
        internal static getAccessibleParentFromContextDelegate getAccessibleParentFromContext;
        internal static getAccessibleSelectionCountFromContextDelegate getAccessibleSelectionCountFromContext;
        internal static getAccessibleSelectionFromContextDelegate getAccessibleSelectionFromContext;
        internal static getAccessibleTableCellInfoDelegate getAccessibleTableCellInfo;
        internal static getAccessibleTableColumnDelegate getAccessibleTableColumn;
        internal static getAccessibleTableColumnSelectionCountDelegate getAccessibleTableColumnSelectionCount;
        internal static getAccessibleTableColumnSelectionsDelegate getAccessibleTableColumnSelections;
        internal static getAccessibleTableIndexDelegate getAccessibleTableIndex;
        internal static getAccessibleTableInfoDelegate getAccessibleTableInfo;
        internal static getAccessibleTableRowDelegate getAccessibleTableRow;
        internal static getAccessibleTableRowSelectionCountDelegate getAccessibleTableRowSelectionCount;
        internal static getAccessibleTableRowSelectionsDelegate getAccessibleTableRowSelections;
        internal static getAccessibleTextAttributesDelegate getAccessibleTextAttributes;
        internal static getAccessibleTextInfoDelegate getAccessibleTextInfo;
        internal static getAccessibleTextItemsDelegate getAccessibleTextItems;
        internal static getAccessibleTextRectDelegate getAccessibleTextRect;
        internal static getAccessibleTextSelectionInfoDelegate getAccessibleTextSelectionInfo;
        internal static getActiveDescendentDelegate getActiveDescendent;
        internal static getCaretLocationDelegate getCaretLocation;
        internal static getCurrentAccessibleValueFromContextDelegate getCurrentAccessibleValueFromContext;
        internal static getEventsWaitingDelegate getEventsWaiting;
        internal static getHWNDFromAccessibleContextDelegate getHWNDFromAccessibleContext;
        internal static getParentWithRoleDelegate getParentWithRole;
        internal static getTopLevelObjectDelegate getTopLevelObject;
        internal static getVersionInfoDelegate getVersionInfo;
        internal static getVirtualAccessibleNameDelegate getVirtualAccessibleName;
        internal static getVisibleChildrenDelegate getVisibleChildren;
        internal static getVisibleChildrenCountDelegate getVisibleChildrenCount;
        internal static isAccessibleTableColumnSelectedDelegate isAccessibleTableColumnSelected;
        internal static isAccessibleTableRowSelectedDelegate isAccessibleTableRowSelected;
        internal static isJavaWindowDelegate isJavaWindow;
        internal static isSameObjectDelegate isSameObject;
        internal static System.Collections.Generic.List<System.Exception> loopExceptions = null;
        internal static releaseJavaObjectDelegate releaseJavaObject;
        internal static requestFocusDelegate requestFocus;
        internal static setCaretPositionDelegate setCaretPosition;
        internal static setCaretUpdateFPDelegate setCaretUpdate;
        internal static setFocusGainedFPDelegate setFocusGained;
        internal static setFocusLostFPDelegate setFocusLost;
        internal static setJavaShutdownFPDelegate setJavaShutdown;
        internal static setMenuCanceledFPDelegate setMenuCanceled;
        internal static setMenuDeselectedFPDelegate setMenuDeselected;
        internal static setMenuSelectedFPDelegate setMenuSelected;
        internal static setMouseClickedFPDelegate setMouseClicked;
        internal static setMouseEnteredFPDelegate setMouseEntered;
        internal static setMouseExitedFPDelegate setMouseExited;
        internal static setMousePressedFPDelegate setMousePressed;
        internal static setMouseReleasedFPDelegate setMouseReleased;
        internal static setPopupMenuCanceledFPDelegate setPopupMenuCanceled;
        internal static setPopupMenuWillBecomeInvisibleFPDelegate setPopupMenuWillBecomeInvisible;
        internal static setPopupMenuWillBecomeVisibleFPDelegate setPopupMenuWillBecomeVisible;
        internal static setPropertyActiveDescendentChangeFPDelegate setPropertyActiveDescendentChange;
        internal static setPropertyCaretChangeFPDelegate setPropertyCaretChange;
        internal static setPropertyChildChangeFPDelegate setPropertyChildChange;
        internal static setPropertyDescriptionChangeFPDelegate setPropertyDescriptionChange;
        internal static setPropertyNameChangeFPDelegate setPropertyNameChange;
        internal static setPropertySelectionChangeFPDelegate setPropertySelectionChange;
        internal static setPropertyStateChangeFPDelegate setPropertyStateChange;
        internal static setPropertyTableModelChangeFPDelegate setPropertyTableModelChange;
        internal static setPropertyTextChangeFPDelegate setPropertyTextChange;
        internal static setPropertyValueChangeFPDelegate setPropertyValueChange;
        internal static setPropertyVisibleDataChangeFPDelegate setPropertyVisibleDataChange;
        internal static setTextContentsDelegate setTextContents;
        internal static initializeAccessBridgeDelegate initializeAccessBridge;
        internal static initializeAccessBridgeDelegate shutdownAccessBridge;

        public const int ACTIONNAMESIZE = 0x100;

        public static void Windows_run()
        {
            if (initializeAccessBridge() == true)
                _IsWindowsAccessBridgeAvailable = true;
        }

        static JavaAccNativeMethods()
        {
            fieldsInfo = (from x in typeof(JavaAccNativeMethods).GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
                          where (bool)((x.FieldType.BaseType == typeof(System.MulticastDelegate)) && x.IsAssembly)
                          select x).ToList<System.Reflection.FieldInfo>();
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeLibrary(System.IntPtr hModule);
        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern System.IntPtr GetProcAddress(System.IntPtr hModule, string procedureName);
        public static bool _IsWindowsAccessBridgeAvailable = false;

        public static void LoadJavaAccessBridge()
        {
            string windowsAccessBridgeDll = "jabproxy.dll"; //"windowsaccessbridge.dll"
            System.IntPtr pDll = LoadLibrary(windowsAccessBridgeDll);
            if (pDll == System.IntPtr.Zero)
            {
                System.Exception exceptionForHR = System.Runtime.InteropServices.Marshal.GetExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
                throw new Win32Exception(exceptionForHR.Message, exceptionForHR.InnerException);
            }
            loopExceptions = new System.Collections.Generic.List<System.Exception>();
            foreach (System.Reflection.FieldInfo info in fieldsInfo)
            {
                try
                {
                    LoadJavaAccessBridgeMethod(info, pDll);
                }
                catch (System.Exception exception2)
                {
                    loopExceptions.Add(exception2);
                }
            }
            if (loopExceptions.Count > 0)
            {
                System.Text.StringBuilder builder = new System.Text.StringBuilder();
                foreach (System.Exception exception3 in loopExceptions)
                {
                    builder.AppendLine(exception3.Message);
                }
                throw new DataDrivenAdapterException(builder.ToString());
            }
        }

        static void LoadJavaAccessBridgeMethod(System.Reflection.FieldInfo delegateInstance, System.IntPtr pDll)
        {
            System.IntPtr procAddress = GetProcAddress(pDll, delegateInstance.Name);
            if (procAddress == System.IntPtr.Zero)
            {
                throw System.Runtime.InteropServices.Marshal.GetExceptionForHR(System.Runtime.InteropServices.Marshal.GetLastWin32Error());
            }
            delegateInstance.SetValue(null, System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(procAddress, delegateInstance.FieldType));
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern System.IntPtr LoadLibrary(string dllToLoad);
    }
}
