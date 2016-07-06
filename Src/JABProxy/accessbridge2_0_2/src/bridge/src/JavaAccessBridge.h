/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

/* 
 * A DLL which is loaded by Java applications to handle communication
 * between Java VMs purposes of Accessbility.
 */

#include <windows.h>
#include <jni.h>

#include "AccessBridgePackages.h"
#include "AccessBridgeATInstance.h"
#include "AccessBridgeJavaEntryPoints.h"

#ifndef __JavaAccessBridge_H__
#define __JavaAccessBridge_H__


extern "C" {
	BOOL WINAPI DllMain(HINSTANCE hinstDll, DWORD fdwReason, 
						LPVOID lpvReserved);
	void AppendToCallOutput(char *s);
	BOOL APIENTRY AccessBridgeDialogProc(HWND hDlg, UINT message, 
										 UINT wParam, LONG lParam);
}

/**
 * The JavaAccessBridge class.  The core of the Windows AT AccessBridge dll
 */
class JavaAccessBridge {
// for debugging
public:
// for debugging
	HINSTANCE windowsInstance;
	HWND dialogWindow;
	AccessBridgeATInstance *ATs;
	JavaVM *javaVM;
	JNIEnv *windowsThreadJNIEnv;	// for calls initiated from Windows
	AccessBridgeJavaEntryPoints *javaThreadEntryPoints;
	AccessBridgeJavaEntryPoints *windowsThreadEntryPoints;
	jobject javaThreadABObject;		// for calls initiated from Java
	jobject windowsThreadABObject;	// for calls initiated from Windows

public:
	JavaAccessBridge(HINSTANCE hInstance);
	~JavaAccessBridge();
	void javaRun(JNIEnv *env, jobject obj);
	BOOL initWindow();

	// IPC with the Java AccessBridge DLL
	void postHelloToWindowsDLLMsg(HWND destHwnd);
	LRESULT MemoryMappedFileCreated(HWND srcHwnd, char *filename);

	void sendPackage(char *buffer, int bufsize, HWND destHwnd);
	void sendJavaEventPackage(char *buffer, int bufsize, long type);
	void sendAccessibilityEventPackage(char *buffer, int bufsize, long type);
	BOOL sendMemoryPackage(char *buffer, long bufsize, HWND destWindow);
	LRESULT processPackage(char *buffer, int bufsize);
	BOOL receiveMemoryPackage(HWND srcWindow, long bufsize);
	void WindowsATDestroyed(HWND ATBridgeDLLWindow);

	// Java VM object memory management
	void releaseJavaObject(jobject object);

	// Event handling methods
	void addJavaEventNotification(jlong type, HWND DLLwindow);
	void removeJavaEventNotification(jlong type, HWND DLLwindow);
	void addAccessibilityEventNotification(jlong type, HWND DLLwindow);
	void removeAccessibilityEventNotification(jlong type, HWND DLLwindow);

	// Event firing methods
/*
    void firePropertyChange(JNIEnv *env, jobject callingObj, 
							jobject propertyChangeEvent, 
							jobject source, jstring propertyName,
							jstring oldValue, jstring newValue);
*/

    void javaShutdown(JNIEnv *env, jobject callingObj); 

    void fireFocusGained(JNIEnv *env, jobject callingObj, 
						 jobject focusEvent, jobject source);
	void fireFocusLost(JNIEnv *env, jobject callingObj,
					   jobject focusEvent,jobject source);
	void fireCaretUpdate(JNIEnv *env, jobject callingObj, 
						 jobject caretEvent, jobject source);
	void fireMouseClicked(JNIEnv *env, jobject callingObj, 
						  jobject mouseEvent, jobject source);
	void fireMouseEntered(JNIEnv *env, jobject callingObj, 
						  jobject mouseEvent, jobject source);
	void fireMouseExited(JNIEnv *env, jobject callingObj, 
						 jobject mouseEvent, jobject source);
	void fireMousePressed(JNIEnv *env, jobject callingObj, 
						  jobject mouseEvent, jobject source);
	void fireMouseReleased(JNIEnv *env, jobject callingObj, 
						   jobject mouseEvent, jobject source);
	void fireMenuCanceled(JNIEnv *env, jobject callingObj, 
						  jobject menuEvent, jobject source);
	void fireMenuDeselected(JNIEnv *env, jobject callingObj, 
							jobject menuEvent, jobject source);
	void fireMenuSelected(JNIEnv *env, jobject callingObj, 
						  jobject menuEvent, jobject source);
	void firePopupMenuCanceled(JNIEnv *env, jobject callingObj, 
							   jobject popupMenuEvent, jobject source);
	void firePopupMenuWillBecomeInvisible(JNIEnv *env, jobject callingObj, 
										  jobject popupMenuEvent, jobject source);
	void firePopupMenuWillBecomeVisible(JNIEnv *env, jobject callingObj, 
										jobject popupMenuEvent, jobject source);


	void firePropertyCaretChange(JNIEnv *env, jobject callingObj, 
							     jobject event, jobject source,
                                 jint oldValue, jint newValue);
	void firePropertyDescriptionChange(JNIEnv *env, jobject callingObj, 
							           jobject event, jobject source,
                                       jstring oldValue, jstring newValue);
	void firePropertyNameChange(JNIEnv *env, jobject callingObj, 
			                    jobject event, jobject source,
                                jstring oldValue, jstring newValue);
	void firePropertySelectionChange(JNIEnv *env, jobject callingObj, 
			                         jobject event, jobject source);
	void firePropertyStateChange(JNIEnv *env, jobject callingObj, 
			                     jobject event, jobject source,
                                 jstring oldValue, jstring newValue);
	void firePropertyTextChange(JNIEnv *env, jobject callingObj, 
			                    jobject event, jobject source);
	void firePropertyValueChange(JNIEnv *env, jobject callingObj, 
			                     jobject event, jobject source,
                                 jstring oldValue, jstring newValue);
	void firePropertyVisibleDataChange(JNIEnv *env, jobject callingObj, 
			                           jobject event, jobject source);
	void firePropertyChildChange(JNIEnv *env, jobject callingObj, 
			                     jobject event, jobject source,
                                 jobject oldValue, jobject newValue);
	void firePropertyActiveDescendentChange(JNIEnv *env, jobject callingObj, 
			                                jobject event, jobject source,
                                            jobject oldValue, jobject newValue);

	void firePropertyTableModelChange(JNIEnv *env, jobject callingObj, 
			                          jobject event, jobject source,
                                      jstring oldValue, jstring newValue);
};


#endif
