/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

/* 
 * Glue routines called by Windows AT into the WindowsAccessBridge dll
 */

#ifndef __AccessBridgeWindowsEntryPoints_H__
#define __AccessBridgeWindowsEntryPoints_H__

#include <windows.h>
#include <jni.h>

#include "AccessBridgePackages.h"
#include "AccessBridgeCallbacks.h"

#ifdef __cplusplus
extern "C" {
#endif

    void Windows_run();

    void releaseJavaObject(long vmID, jobject object);
    void getVersionInfo(long vmID, AccessBridgeVersionInfo *info);

    // Window related functions
    HWND getTopLevelHWND(HWND descendent);
    BOOL isJavaWindow(HWND window);
    BOOL getAccessibleContextFromHWND(HWND window, long *vmID, jobject *AccessibleContext);
    HWND getHWNDFromAccessibleContext(long vmID, jobject accessibleContext);

    // returns whether two objects are the same
    BOOL isSameObject(long vmID, jobject obj1, jobject obj2);

    // Accessible Context functions
    BOOL getAccessibleContextAt(long vmID, jobject AccessibleContextParent, 
                                jint x, jint y, jobject *AccessibleContext);
    BOOL getAccessibleContextWithFocus(HWND window, long *vmID, jobject *AccessibleContext);
    BOOL getAccessibleContextInfo(long vmID, jobject AccessibleContext, AccessibleContextInfo *info);
    jobject getAccessibleChildFromContext(long vmID, jobject AccessibleContext, jint childIndex);
    jobject getAccessibleParentFromContext(long vmID, jobject AccessibleContext);

    /* begin AccessibleTable */
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

    /* end AccessibleTable */

    BOOL getAccessibleRelationSet(long vmID, jobject accessibleContext, 
                                  AccessibleRelationSetInfo *relationSetInfo);
    
    // AccessibleHypertext methods
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

    /* ----- Additional AccessibleHypertext methods for Teton */


    jint getAccessibleHypertextLinkCount(const long vmID, 
                                         const AccessibleContext accessibleContext);

    BOOL getAccessibleHypertextExt(const long vmID,
                                   const AccessibleContext accessibleContext,
                                   const jint nStartIndex,
                                   /* OUT */ AccessibleHypertextInfo *hypertextInfo);

    jint getAccessibleHypertextLinkIndex(const long vmID, 
                                         const AccessibleContext accessibleContext,
                                         const jint nIndex);

    BOOL getAccessibleHyperlink(const long vmID, 
                                const AccessibleContext accessibleContext,
                                const jint nIndex, 
                                /* OUT */ AccessibleHyperlinkInfo *hyperlinkInfo);


    /* Additional utility methods */
    BOOL setTextContents (const long vmID, const AccessibleContext accessibleContext, const wchar_t *text);

    AccessibleContext getParentWithRole (const long vmID, const AccessibleContext accessibleContext, const wchar_t *role);

    AccessibleContext getTopLevelObject (const long vmID, const AccessibleContext accessibleContext);

    AccessibleContext getParentWithRoleElseRoot (const long vmID, const AccessibleContext accessibleContext, const wchar_t *role);

    int getObjectDepth (const long vmID, const AccessibleContext accessibleContext);

    AccessibleContext getActiveDescendent (const long vmID, const AccessibleContext accessibleContext);

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
     * Returns the number of visible children of a component. Returns -1 on error.
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

    // Accessible Text functions
    BOOL getAccessibleTextInfo(long vmID, jobject AccessibleContext, AccessibleTextInfo *textInfo, jint x, jint y);
    BOOL getAccessibleTextItems(long vmID, jobject AccessibleContext, AccessibleTextItemsInfo *textItems, jint index);
    BOOL getAccessibleTextSelectionInfo(long vmID, jobject AccessibleContext, AccessibleTextSelectionInfo *selectionInfo);
    BOOL getAccessibleTextAttributes(long vmID, jobject AccessibleContext, jint index, AccessibleTextAttributesInfo *attributes);
    BOOL getAccessibleTextRect(long vmID, jobject AccessibleContext, AccessibleTextRectInfo *rectInfo, jint index);
    BOOL getAccessibleTextLineBounds(long vmID, jobject AccessibleContext, jint index, jint *startIndex, jint *endIndex);
    BOOL getAccessibleTextRange(long vmID, jobject AccessibleContext, jint start, jint end, wchar_t *text, short len);

    // Accessible Value methods
    BOOL getCurrentAccessibleValueFromContext(long vmID,jobject AccessibleContext, wchar_t *value, short len);
    BOOL getMaximumAccessibleValueFromContext(long vmID,jobject AccessibleContext, wchar_t *value, short len);
    BOOL getMinimumAccessibleValueFromContext(long vmID,jobject AccessibleContext, wchar_t *value, short len);

    // Accessible Selection methods
    void addAccessibleSelectionFromContext(long vmID,jobject AccessibleContext, int i);
    void clearAccessibleSelectionFromContext(long vmID,jobject AccessibleContext);
    jobject getAccessibleSelectionFromContext(long vmID,jobject AccessibleContext, int i);
    int getAccessibleSelectionCountFromContext(long vmID,jobject AccessibleContext);
    BOOL isAccessibleChildSelectedFromContext(long vmID,jobject AccessibleContext, int i);
    void removeAccessibleSelectionFromContext(long vmID,jobject AccessibleContext, int i);
    void selectAllAccessibleSelectionFromContext(long vmID,jobject AccessibleContext);


    // PropertyChange Event registry routines
    void setPropertyChangeFP(AccessBridge_PropertyChangeFP fp);

    // Java application shutdown
    void setJavaShutdownFP(AccessBridge_JavaShutdownFP fp);

    // Focus Event registry routines
    void setFocusGainedFP(AccessBridge_FocusGainedFP fp);
    void setFocusLostFP(AccessBridge_FocusLostFP fp);

    // Caret Event registry routines
    void setCaretUpdateFP(AccessBridge_CaretUpdateFP fp);

    // Mouse Event registry routines
    void setMouseClickedFP(AccessBridge_MouseClickedFP fp);
    void setMouseEnteredFP(AccessBridge_MouseEnteredFP fp);
    void setMouseExitedFP(AccessBridge_MouseExitedFP fp);
    void setMousePressedFP(AccessBridge_MousePressedFP fp);
    void setMouseReleasedFP(AccessBridge_MouseReleasedFP fp);

    // Menu/PopupMenu Event registry routines
    void setMenuCanceledFP(AccessBridge_MenuCanceledFP fp);
    void setMenuDeselectedFP(AccessBridge_MenuDeselectedFP fp);
    void setMenuSelectedFP(AccessBridge_MenuSelectedFP fp);
    void setPopupMenuCanceledFP(AccessBridge_PopupMenuCanceledFP fp);
    void setPopupMenuWillBecomeInvisibleFP(AccessBridge_PopupMenuWillBecomeInvisibleFP fp);
    void setPopupMenuWillBecomeVisibleFP(AccessBridge_PopupMenuWillBecomeVisibleFP fp);

    // Accessibility PropertyChange Event registry routines
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



#ifdef __cplusplus
}
#endif

#endif

