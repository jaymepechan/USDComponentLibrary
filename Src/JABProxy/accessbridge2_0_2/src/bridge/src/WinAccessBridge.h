/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)WinAccessBridge.h	1.23 05/08/22
 */

/* 
 * A DLL which is loaded by Windows executables to handle communication
 * between Java VMs purposes of Accessbility.
 */

#ifndef __WinAccessBridge_H__
#define __WinAccessBridge_H__

#include <windows.h>
#include "AccessBridgePackages.h"
#include "AccessBridgeEventHandler.h"
#include "AccessBridgeJavaVMInstance.h"
#include "AccessBridgeMessageQueue.h"


extern "C" {
    BOOL WINAPI DllMain(HINSTANCE hinstDll, DWORD fdwReason, 
                        LPVOID lpvReserved);
    void AppendToCallOutput(char *s);
    BOOL CALLBACK AccessBridgeDialogProc(HWND hDlg, UINT message, 
                                         UINT wParam, LONG lParam);
    HWND getTopLevelHWND(HWND descendent);
}

LRESULT CALLBACK WinAccessBridgeWindowProc(HWND hWnd, UINT message, 
                                           UINT wParam, LONG lParam);

BOOL CALLBACK DeleteItemProc(HWND hwndDlg, UINT message, WPARAM wParam, LPARAM lParam);

/**
 * The WinAccessBridge class.  The core of the Windows AT AccessBridge dll
 */
class WinAccessBridge {
    HINSTANCE windowsInstance;
    HWND dialogWindow;
    AccessBridgeJavaVMInstance *javaVMs;
    AccessBridgeEventHandler *eventHandler;
    AccessBridgeMessageQueue *messageQueue;

public:
    WinAccessBridge(HINSTANCE hInstance);
    ~WinAccessBridge();
    BOOL initWindow();

    HWND showWinAccessBridgeGUI(int showCommand);

    // IPC with the Java AccessBridge DLL
    LRESULT rendezvousWithNewJavaDLL(HWND JavaBridgeDLLwindow, long vmID);

    void sendPackage(char *buffer, long bufsize, HWND destWindow);
    BOOL sendMemoryPackage(char *buffer, long bufsize, HWND destWindow);
    BOOL queuePackage(char *buffer, long bufsize);
    BOOL receiveAQueuedPackage();
    void preProcessPackage(char *buffer, long bufsize);
    void processPackage(char *buffer, long bufsize);
    void JavaVMDestroyed(HWND VMBridgeDLLWindow);

    // Java VM object memory management
    void releaseJavaObject(long vmID, jobject object);

    // Version info
    BOOL getVersionInfo(long vmID, AccessBridgeVersionInfo *info);

    // HWND management methods
    HWND getNextJavaWindow(HWND previous);
    BOOL isJavaWindow(HWND window);
    BOOL getAccessibleContextFromHWND(HWND window, long *vmID, jobject *AccessibleContext);
    HWND getHWNDFromAccessibleContext(long vmID, jobject accessibleContext);

    /* Additional utility methods */
    BOOL isSameObject(long vmID, jobject obj1, jobject obj2);

    BOOL setTextContents (const long vmID, const AccessibleContext accessibleContext, const wchar_t *text);

    AccessibleContext getParentWithRole (const long vmID, const AccessibleContext accessibleContext, 
                                         const wchar_t *role);

    AccessibleContext getTopLevelObject (const long vmID, const AccessibleContext accessibleContext);

    AccessibleContext getParentWithRoleElseRoot (const long vmID, const AccessibleContext accessibleContext, 
                                                 const wchar_t *role);

    int getObjectDepth (const long vmID, const AccessibleContext accessibleContext);

    AccessibleContext getActiveDescendent (const long vmID, const AccessibleContext accessibleContext);


    // Accessible Context methods
    BOOL getAccessibleContextAt(long vmID, jobject AccessibleContextParent, 
                                jint x, jint y, jobject *AccessibleContext);
    BOOL getAccessibleContextWithFocus(HWND window, long *vmID, jobject *AccessibleContext);
    BOOL getAccessibleContextInfo(long vmID, jobject AccessibleContext, AccessibleContextInfo *info);
    jobject getAccessibleChildFromContext(long vmID, jobject AccessibleContext, jint childIndex);
    jobject getAccessibleParentFromContext(long vmID, jobject AccessibleContext);

    /* begin AccessibleTable methods */
    BOOL getAccessibleTableInfo(long vmID, jobject acParent, AccessibleTableInfo *tableInfo);
    BOOL getAccessibleTableCellInfo(long vmID, jobject accessibleTable, jint row, jint column, 
                                    AccessibleTableCellInfo *tableCellInfo);

    BOOL getAccessibleTableRowHeader(long vmID, jobject acParent, AccessibleTableInfo *tableInfo);
    BOOL getAccessibleTableColumnHeader(long vmID, jobject acParent, AccessibleTableInfo *tableInfo);

    jobject getAccessibleTableRowDescription(long vmID, jobject acParent, jint row);
    jobject getAccessibleTableColumnDescription(long vmID, jobject acParent, jint column);

    jint getAccessibleTableRowSelectionCount(long vmID, jobject accessibleTable);
    BOOL isAccessibleTableRowSelected(long vmID, jobject accessibleTable, jint row);
    BOOL getAccessibleTableRowSelections(long vmID, jobject accessibleTable, jint count, 
                                         jint *selections);

    jint getAccessibleTableColumnSelectionCount(long vmID, jobject accessibleTable);
    BOOL isAccessibleTableColumnSelected(long vmID, jobject accessibleTable, jint column);
    BOOL getAccessibleTableColumnSelections(long vmID, jobject accessibleTable, jint count, 
                                            jint *selections);

    jint getAccessibleTableRow(long vmID, jobject accessibleTable, jint index);
    jint getAccessibleTableColumn(long vmID, jobject accessibleTable, jint index);
    jint getAccessibleTableIndex(long vmID, jobject accessibleTable, jint row, jint column);

    /* end AccessibleTable methods */

    // --------- AccessibleRelationSet methods
    BOOL getAccessibleRelationSet(long vmID, jobject accessibleContext, AccessibleRelationSetInfo *relationSet);

    // --------- AccessibleHypertext methods
    BOOL getAccessibleHypertext(long vmID, jobject accessibleContext, AccessibleHypertextInfo *hypertextInfo);
    BOOL activateAccessibleHyperlink(long vmID, jobject accessibleContext, jobject accessibleHyperlink);
    
    jint getAccessibleHyperlinkCount(const long vmID, 
                                     const AccessibleContext accessibleContext);
    
    BOOL getAccessibleHypertextExt(const long vmID,
                                   const AccessibleContext accessibleContext,
                                   const jint nStartIndex,
                                   /* OUT */ AccessibleHypertextInfo *hypertextInfo);
    
    jint getAccessibleHypertextLinkIndex(const long vmID, 
                                         const AccessibleHypertext hypertext,
                                         const jint nIndex);
    
    BOOL getAccessibleHyperlink(const long vmID, 
                                const AccessibleHypertext hypertext,
                                const jint nIndex, 
                                /* OUT */ AccessibleHyperlinkInfo *hyperlinkInfo);


    /* Accessible KeyBindings, Icons and Actions */
    BOOL getAccessibleKeyBindings(long vmID, jobject accessibleContext,
                                  AccessibleKeyBindings *keyBindings);

    BOOL getAccessibleIcons(long vmID, jobject accessibleContext,
                            AccessibleIcons *icons);

    BOOL getAccessibleActions(long vmID, jobject accessibleContext,
                              AccessibleActions *actions);

    BOOL doAccessibleActions(long vmID, jobject accessibleContext,
                             AccessibleActionsToDo *actionsToDo, jint *failure);


    // Accessible Text methods
    BOOL getAccessibleTextInfo(long vmID, jobject AccessibleContext, AccessibleTextInfo *textInfo, jint x, jint y);
    BOOL getAccessibleTextItems(long vmID, jobject AccessibleContext, AccessibleTextItemsInfo *textItems, jint index);
    BOOL getAccessibleTextSelectionInfo(long vmID, jobject AccessibleContext, AccessibleTextSelectionInfo *selectionInfo);
    BOOL getAccessibleTextAttributes(long vmID, jobject AccessibleContext, jint index, AccessibleTextAttributesInfo *attributes);
    BOOL getAccessibleTextRect(long vmID, jobject AccessibleContext, AccessibleTextRectInfo *rectInfo, jint index);
    BOOL getAccessibleTextLineBounds(long vmID, jobject AccessibleContext, jint index, jint *startIndex, jint *endIndex);
    BOOL getAccessibleTextRange(long vmID, jobject AccessibleContext, jint start, jint end, wchar_t *text, short len);

    // Accessible Value methods
    BOOL getCurrentAccessibleValueFromContext(long vmID, jobject AccessibleContext, wchar_t *value, short len);
    BOOL getMaximumAccessibleValueFromContext(long vmID, jobject AccessibleContext, wchar_t *value, short len);
    BOOL getMinimumAccessibleValueFromContext(long vmID, jobject AccessibleContext, wchar_t *value, short len);

    // Accessible Selection methods
    void addAccessibleSelectionFromContext(long vmID, jobject AccessibleContext, int i);
    void clearAccessibleSelectionFromContext(long vmID, jobject AccessibleContext);
    jobject getAccessibleSelectionFromContext(long vmID, jobject AccessibleContext, int i);
    int getAccessibleSelectionCountFromContext(long vmID, jobject AccessibleContext);
    BOOL isAccessibleChildSelectedFromContext(long vmID, jobject AccessibleContext, int i);
    void removeAccessibleSelectionFromContext(long vmID, jobject AccessibleContext, int i);
    void selectAllAccessibleSelectionFromContext(long vmID, jobject AccessibleContext);

    // Event handling methods
    void addJavaEventNotification(jlong type);
    void removeJavaEventNotification(jlong type);
    void addAccessibilityEventNotification(jlong type);
    void removeAccessibilityEventNotification(jlong type);

    void setPropertyChangeFP(AccessBridge_PropertyChangeFP fp);
    void setJavaShutdownFP(AccessBridge_JavaShutdownFP fp);
    void setFocusGainedFP(AccessBridge_FocusGainedFP fp);
    void setFocusLostFP(AccessBridge_FocusLostFP fp);
    void setCaretUpdateFP(AccessBridge_CaretUpdateFP fp);
    void setMouseClickedFP(AccessBridge_MouseClickedFP fp);
    void setMouseEnteredFP(AccessBridge_MouseEnteredFP fp);
    void setMouseExitedFP(AccessBridge_MouseExitedFP fp);
    void setMousePressedFP(AccessBridge_MousePressedFP fp);
    void setMouseReleasedFP(AccessBridge_MouseReleasedFP fp);
    void setMenuCanceledFP(AccessBridge_MenuCanceledFP fp);
    void setMenuDeselectedFP(AccessBridge_MenuDeselectedFP fp);
    void setMenuSelectedFP(AccessBridge_MenuSelectedFP fp);
    void setPopupMenuCanceledFP(AccessBridge_PopupMenuCanceledFP fp);
    void setPopupMenuWillBecomeInvisibleFP(AccessBridge_PopupMenuWillBecomeInvisibleFP fp);
    void setPopupMenuWillBecomeVisibleFP(AccessBridge_PopupMenuWillBecomeVisibleFP fp);

    void setPropertyNameChangeFP(AccessBridge_PropertyNameChangeFP fp);
    void setPropertyDescriptionChangeFP(AccessBridge_PropertyDescriptionChangeFP fp);
    void setPropertyStateChangeFP(AccessBridge_PropertyStateChangeFP fp);
    void setPropertyValueChangeFP(AccessBridge_PropertyValueChangeFP fp);
    void setPropertySelectionChangeFP(AccessBridge_PropertySelectionChangeFP fp);
    void setPropertyTextChangeFP(AccessBridge_PropertyTextChangeFP fp);
    void setPropertyCaretChangeFP(AccessBridge_PropertyCaretChangeFP fp);
    void setPropertyVisibleDataChangeFP(AccessBridge_PropertyVisibleDataChangeFP fp);
    void setPropertyChildChangeFP(AccessBridge_PropertyChildChangeFP fp);
    void setPropertyActiveDescendentChangeFP(AccessBridge_PropertyActiveDescendentChangeFP fp);

    void setPropertyTableModelChangeFP(AccessBridge_PropertyTableModelChangeFP fp);

    /**
     * Additional methods for Teton
     */

    /**
     * Gets the AccessibleName for a component based upon the JAWS algorithm. Returns
     * whether successful.
     *
     * Bug ID 4916682 - Implement JAWS AccessibleName policy
     */
    BOOL getVirtualAccessibleName(long vmID, AccessibleContext accessibleContext, wchar_t *name, int len);

    /**
     * Request focus for a component. Returns whether successful;
     *
     * Bug ID 4944757 - requestFocus method needed
     */
    BOOL requestFocus(long vmID, AccessibleContext accessibleContext);

    /**
     * Selects text between two indices.  Selection includes the text at the start index
     * and the text at the end index. Returns whether successful;
     *
     * Bug ID 4944758 - selectTextRange method needed
     */
    BOOL selectTextRange(long vmID, AccessibleContext accessibleContext, int startIndex, int endIndex);

    /**
     * Get text attributes between two indices.  The attribute list includes the text at the 
     * start index and the text at the end index. Returns whether successful;
     *
     * Bug ID 4944761 - getTextAttributes between two indices method needed
     */
    BOOL getTextAttributesInRange(long vmID, AccessibleContext accessibleContext, int startIndex, int endIndex,
                                  AccessibleTextAttributesInfo *attributes, short *len);

    /**
     * Gets number of visible children of a component. Returns -1 on error.
     *
     * Bug ID 4944762- getVisibleChildren for list-like components needed
     */
    int getVisibleChildrenCount(long vmID, AccessibleContext accessibleContext);

    /**
     * Gets the visible children of an AccessibleContext. Returns whether successful;
     *
     * Bug ID 4944762- getVisibleChildren for list-like components needed
     */
    BOOL getVisibleChildren(long vmID, AccessibleContext accessibleContext, int startIndex,
                            VisibleChildrenInfo *visibleChildrenInfo);

    /**
     * Set the caret to a text position. Returns whether successful;
     *
     * Bug ID 4944770 - setCaretPosition method needed
     */
    BOOL setCaretPosition(long vmID, AccessibleContext accessibleContext, int position);


    /**
     * Gets the text caret bounding rectangle
     */
    BOOL getCaretLocation(long vmID, jobject AccessibleContext, AccessibleTextRectInfo *rectInfo, jint index);

    /**
     * Gets number of events waiting in the message queue
     */
    int getEventsWaiting();

};

#endif

