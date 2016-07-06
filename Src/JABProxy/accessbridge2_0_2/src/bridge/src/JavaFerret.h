/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)JavaFerret.h	1.3 05/03/21
 */

#define JAVA_FERRET_LOG "JavaFerret.log"

void CALLBACK TimerProc(HWND hWnd, UINT uTimerMsg, UINT uTimerID, DWORD dwTime);
LRESULT CALLBACK KeyboardProc (int code, WPARAM wParam, LPARAM lParam);
LRESULT CALLBACK MouseProc (int code, WPARAM wParam, LPARAM lParam);

BOOL InitWindow (HANDLE hInstance);
BOOL APIENTRY FerretDialogProc (HWND hDlg, UINT message, UINT wParam, LONG lParam);

void HandleJavaPropertyChange(long vmID, PropertyChangeEvent event,
			      AccessibleContext ac,
			      wchar_t *name, wchar_t *oldValue, wchar_t *newValue);

void HandleJavaShutdown(long vmID);
void HandleJavaFocusGained(long vmID, FocusEvent event, AccessibleContext ac);
void HandleJavaFocusLost(long vmID, FocusEvent event, AccessibleContext ac);

void HandleJavaCaretUpdate(long vmID, CaretEvent event, AccessibleContext ac);

void HandleMouseClicked(long vmID, MouseEvent event, AccessibleContext ac);
void HandleMouseEntered(long vmID, MouseEvent event, AccessibleContext ac);
void HandleMouseExited(long vmID, MouseEvent event, AccessibleContext ac);
void HandleMousePressed(long vmID, MouseEvent event, AccessibleContext ac);
void HandleMouseReleased(long vmID, MouseEvent event, AccessibleContext ac);

void HandleMenuCanceled(long vmID, MenuEvent event, AccessibleContext ac);
void HandleMenuDeselected(long vmID, MenuEvent event, AccessibleContext ac);
void HandleMenuSelected(long vmID, MenuEvent event, AccessibleContext ac);
void HandlePopupMenuCanceled(long vmID, MenuEvent event, AccessibleContext ac);
void HandlePopupMenuWillBecomeInvisible(long vmID, MenuEvent event, AccessibleContext ac);
void HandlePopupMenuWillBecomeVisible(long vmID, MenuEvent event, AccessibleContext ac);

void HandlePropertyNameChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                              wchar_t *oldName, wchar_t *newName);
void HandlePropertyDescriptionChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                                     wchar_t *oldDescription, wchar_t *newDescription);
void HandlePropertyStateChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                               wchar_t *oldState, wchar_t *newState);
void HandlePropertyValueChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                               wchar_t *oldValue, wchar_t *newValue);
void HandlePropertySelectionChange(long vmID, PropertyChangeEvent event, AccessibleContext ac);
void HandlePropertyTextChange(long vmID, PropertyChangeEvent event, AccessibleContext ac);
void HandlePropertyCaretChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                               int oldPosition, int newPosition);
void HandlePropertyVisibleDataChange(long vmID, PropertyChangeEvent event, AccessibleContext ac);
void HandlePropertyChildChange(long vmID, PropertyChangeEvent event, AccessibleContext ac, 
                               jobject oldChild, jobject newChild);
void HandlePropertyActiveDescendentChange(long vmID, PropertyChangeEvent event, 
                                          AccessibleContext ac, 
                                          jobject oldActiveDescendent,
                                          jobject newActiveDescendent);

void HandlePropertyTableModelChange(long vmID, PropertyChangeEvent event, 
				    AccessibleContext ac, 
				    wchar_t *oldValue, wchar_t *newValue);

char *getAccessibleInfo(long vmID, AccessibleContext ac,
			int x, int y, char *buffer, int bufsize);

char *getTimeAndDate();
void displayAndLogText(char *buffer, ...);

