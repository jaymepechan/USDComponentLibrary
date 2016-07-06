/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessBridgeEventHandler.h	1.17 05/03/21
 */

/* 
 * A class to manage firing Accessibility events to Windows AT
 */

#ifndef __AccessBridgeEventHandler_H__
#define __AccessBridgeEventHandler_H__

#include "AccessBridgeCallbacks.h"
#include "AccessBridgePackages.h"

class WinAccessBridge;

class AccessBridgeEventHandler {
	long javaEventMask;
	long accessibilityEventMask;

	AccessBridge_PropertyChangeFP propertyChangeFP;
        AccessBridge_JavaShutdownFP javaShutdownFP;
	AccessBridge_FocusGainedFP focusGainedFP;
	AccessBridge_FocusLostFP focusLostFP;
	AccessBridge_CaretUpdateFP caretUpdateFP;
	AccessBridge_MouseClickedFP mouseClickedFP;
	AccessBridge_MouseEnteredFP mouseEnteredFP;
	AccessBridge_MouseExitedFP mouseExitedFP;
	AccessBridge_MousePressedFP mousePressedFP;
	AccessBridge_MouseReleasedFP mouseReleasedFP;
	AccessBridge_MenuCanceledFP menuCanceledFP;
	AccessBridge_MenuDeselectedFP menuDeselectedFP;
	AccessBridge_MenuSelectedFP menuSelectedFP;
	AccessBridge_PopupMenuCanceledFP popupMenuCanceledFP;
	AccessBridge_PopupMenuWillBecomeInvisibleFP popupMenuWillBecomeInvisibleFP;
	AccessBridge_PopupMenuWillBecomeVisibleFP popupMenuWillBecomeVisibleFP;

    AccessBridge_PropertyNameChangeFP propertyNameChangeFP;
    AccessBridge_PropertyDescriptionChangeFP propertyDescriptionChangeFP;
    AccessBridge_PropertyStateChangeFP propertyStateChangeFP;
    AccessBridge_PropertyValueChangeFP propertyValueChangeFP;
    AccessBridge_PropertySelectionChangeFP propertySelectionChangeFP;
    AccessBridge_PropertyTextChangeFP propertyTextChangeFP;
    AccessBridge_PropertyCaretChangeFP propertyCaretChangeFP;
    AccessBridge_PropertyVisibleDataChangeFP propertyVisibleDataChangeFP;
    AccessBridge_PropertyChildChangeFP propertyChildChangeFP;
    AccessBridge_PropertyActiveDescendentChangeFP propertyActiveDescendentChangeFP;

	AccessBridge_PropertyTableModelChangeFP propertyTableModelChangeFP;



public:
	AccessBridgeEventHandler();
	~AccessBridgeEventHandler();
	long getJavaEventMask() {return javaEventMask;};
	long getAccessibilityEventMask() {return accessibilityEventMask;};

	// ------- Registry methods
	void setPropertyChangeFP(AccessBridge_PropertyChangeFP fp, WinAccessBridge *wab);
        void setJavaShutdownFP(AccessBridge_JavaShutdownFP fp, WinAccessBridge *wab);
	void setFocusGainedFP(AccessBridge_FocusGainedFP fp, WinAccessBridge *wab);
	void setFocusLostFP(AccessBridge_FocusLostFP fp, WinAccessBridge *wab);
	void setCaretUpdateFP(AccessBridge_CaretUpdateFP fp, WinAccessBridge *wab);
	void setMouseClickedFP(AccessBridge_MouseClickedFP fp, WinAccessBridge *wab);
	void setMouseEnteredFP(AccessBridge_MouseEnteredFP fp, WinAccessBridge *wab);
	void setMouseExitedFP(AccessBridge_MouseExitedFP fp, WinAccessBridge *wab);
	void setMousePressedFP(AccessBridge_MousePressedFP fp, WinAccessBridge *wab);
	void setMouseReleasedFP(AccessBridge_MouseReleasedFP fp, WinAccessBridge *wab);
	void setMenuCanceledFP(AccessBridge_MenuCanceledFP fp, WinAccessBridge *wab);
	void setMenuDeselectedFP(AccessBridge_MenuDeselectedFP fp, WinAccessBridge *wab);
	void setMenuSelectedFP(AccessBridge_MenuSelectedFP fp, WinAccessBridge *wab);
	void setPopupMenuCanceledFP(AccessBridge_PopupMenuCanceledFP fp, WinAccessBridge *wab);
	void setPopupMenuWillBecomeInvisibleFP(AccessBridge_PopupMenuWillBecomeInvisibleFP fp, 
                                               WinAccessBridge *wab);
	void setPopupMenuWillBecomeVisibleFP(AccessBridge_PopupMenuWillBecomeVisibleFP fp, 
                                             WinAccessBridge *wab);

	void setPropertyNameChangeFP(AccessBridge_PropertyNameChangeFP fp, WinAccessBridge *wab);
	void setPropertyDescriptionChangeFP(AccessBridge_PropertyDescriptionChangeFP fp, 
                                            WinAccessBridge *wab);
	void setPropertyStateChangeFP(AccessBridge_PropertyStateChangeFP fp, WinAccessBridge *wab);
	void setPropertyValueChangeFP(AccessBridge_PropertyValueChangeFP fp, WinAccessBridge *wab);
	void setPropertySelectionChangeFP(AccessBridge_PropertySelectionChangeFP fp, 
                                          WinAccessBridge *wab);
	void setPropertyTextChangeFP(AccessBridge_PropertyTextChangeFP fp, WinAccessBridge *wab);
	void setPropertyCaretChangeFP(AccessBridge_PropertyCaretChangeFP fp, WinAccessBridge *wab);
	void setPropertyVisibleDataChangeFP(AccessBridge_PropertyVisibleDataChangeFP fp, 
                                            WinAccessBridge *wab);
	void setPropertyChildChangeFP(AccessBridge_PropertyChildChangeFP fp, WinAccessBridge *wab);
	void setPropertyActiveDescendentChangeFP(AccessBridge_PropertyActiveDescendentChangeFP fp, 
                                                 WinAccessBridge *wab);

	void setPropertyTableModelChangeFP(AccessBridge_PropertyTableModelChangeFP fp, 
                                           WinAccessBridge *wab);

	// ------- Event notification methods
	void firePropertyChange(long vmID, jobject event, jobject source, 
                                wchar_t *property, wchar_t *oldName, wchar_t *newName);
        void fireJavaShutdown(long vmID);
	void fireFocusGained(long vmID, jobject event, jobject source);
	void fireFocusLost(long vmID, jobject event, jobject source);
	void fireCaretUpdate(long vmID, jobject event, jobject source);
	void fireMouseClicked(long vmID, jobject event, jobject source);
	void fireMouseEntered(long vmID, jobject event, jobject source);
	void fireMouseExited(long vmID, jobject event, jobject source);
	void fireMousePressed(long vmID, jobject event, jobject source);
	void fireMouseReleased(long vmID, jobject event, jobject source);
	void fireMenuCanceled(long vmID, jobject event, jobject source);
	void fireMenuDeselected(long vmID, jobject event, jobject source);
	void fireMenuSelected(long vmID, jobject event, jobject source);
	void firePopupMenuCanceled(long vmID, jobject event, jobject source);
	void firePopupMenuWillBecomeInvisible(long vmID, jobject event, jobject source);
	void firePopupMenuWillBecomeVisible(long vmID, jobject event, jobject source);

	void firePropertyNameChange(long vmID, jobject event, jobject source, 
						        wchar_t *oldName, wchar_t *newName);
	void firePropertyDescriptionChange(long vmID, jobject event, jobject source, 
						               wchar_t *oldDescription, wchar_t *newDescription);
	void firePropertyStateChange(long vmID, jobject event, jobject source, 
						         wchar_t *oldState, wchar_t *newState);
	void firePropertyValueChange(long vmID, jobject event, jobject source, 
						         wchar_t *oldValue, wchar_t *newValue);
	void firePropertySelectionChange(long vmID, jobject event, jobject source);
	void firePropertyTextChange(long vmID, jobject event, jobject source);
	void firePropertyCaretChange(long vmID, jobject event, jobject source, 
						         int oldPosition, int newPosition);
	void firePropertyVisibleDataChange(long vmID, jobject event, jobject source);
	void firePropertyChildChange(long vmID, jobject event, jobject source, 
						         jobject oldChild, jobject newChild);
	void firePropertyActiveDescendentChange(long vmID, jobject event, jobject source, 
						                    jobject oldActiveDescendent, jobject newActiveDescendent);

	void firePropertyTableModelChange(long vmID, jobject event, jobject source, 
						              wchar_t *oldValue, wchar_t *newValue);

};


#endif
