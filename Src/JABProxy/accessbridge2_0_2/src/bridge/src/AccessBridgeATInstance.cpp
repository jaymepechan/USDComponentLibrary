/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessBridgeATInstance.cpp	1.18 05/03/21
 */

/*
 * A class to track key AT instance info from the JavaAccessBridge
 */

#ifdef _DEBUG
#define DEBUGGING_ON
#endif

#include "AccessBridgeDebug.h"
#include "AccessBridgeATInstance.h"
#include "AccessBridgeMessages.h"

#include <windows.h> 
#include <winbase.h>


/**
 *  AccessBridgeATInstance constructor
 */
AccessBridgeATInstance::AccessBridgeATInstance(HWND ourABWindow, HWND winABWindow,
                                               char *memoryFilename,
                                               AccessBridgeATInstance *next) {
    ourAccessBridgeWindow = ourABWindow;
    winAccessBridgeWindow = winABWindow;
    nextATInstance = next;
    javaEventMask = 0;
    accessibilityEventMask = 0;
    strncpy(memoryMappedFileName, memoryFilename, cMemoryMappedNameSize);
}

/**
 * AccessBridgeATInstance descructor
 */
AccessBridgeATInstance::~AccessBridgeATInstance() {
    PrintDebugString("\r\nin AccessBridgeATInstance::~AccessBridgeATInstance"); 

    // if IPC memory mapped file view is valid, unmap it
    if (memoryMappedView != (char *) 0) {
        PrintDebugString("  unmapping memoryMappedView; view = %X", memoryMappedView); 
        UnmapViewOfFile(memoryMappedView);
        memoryMappedView = (char *) 0;
    }
    // if IPC memory mapped file handle map is open, close it
    if (memoryMappedFileMapHandle != (HANDLE) 0) {
        PrintDebugString("  closing memoryMappedFileMapHandle; handle = %X", memoryMappedFileMapHandle); 
        CloseHandle(memoryMappedFileMapHandle);
        memoryMappedFileMapHandle = (HANDLE) 0;
    }
}

/**
 * Sets up the memory-mapped file to do IPC messaging 
 * 1 files is created: to handle requests for information
 * initiated from Windows AT.  The package is placed into
 * the memory-mapped file (char *memoryMappedView),
 * and then a special SendMessage() is sent.  When the
 * JavaDLL returns from SendMessage() processing, the
 * data will be in memoryMappedView.  The SendMessage()
 * return value tells us if all is right with the world.
 *
 * The set-up proces involves creating the memory-mapped
 * file, and writing a special string to it so that the
 * WindowsDLL so it knows about it as well.
 */
LRESULT
AccessBridgeATInstance::initiateIPC() {
    DWORD errorCode;

    PrintDebugString("\r\nin AccessBridgeATInstance::initiateIPC()"); 
	
    // open Windows-initiated IPC filemap & map it to a ptr

    memoryMappedFileMapHandle = OpenFileMapping(FILE_MAP_READ | FILE_MAP_WRITE,
                                                FALSE, memoryMappedFileName);
    if (memoryMappedFileMapHandle == NULL) {
        errorCode = GetLastError();
        PrintDebugString("  Failed to CreateFileMapping for %s, error: %X", memoryMappedFileName, errorCode); 
        return errorCode;
    } else {
        PrintDebugString("  CreateFileMapping worked - filename: %s", memoryMappedFileName); 
    }

    memoryMappedView = (char *) MapViewOfFile(memoryMappedFileMapHandle,
                                              FILE_MAP_READ | FILE_MAP_WRITE,
                                              0, 0, 0);
    if (memoryMappedView == NULL) { 
        errorCode = GetLastError();
        PrintDebugString("  Failed to MapViewOfFile for %s, error: %X", memoryMappedFileName, errorCode); 
        return errorCode;
    } else {
        PrintDebugString("  MapViewOfFile worked - view: %X", memoryMappedView); 
    }


    // look for the JavaDLL's answer to see if it could read the file
    if (strcmp(memoryMappedView, AB_MEMORY_MAPPED_FILE_OK_QUERY) != 0) {
        PrintDebugString("  JavaVM failed to write to memory mapped file %s",
                         memoryMappedFileName); 
        return -1;
    } else {
        PrintDebugString("  JavaVM successfully wrote to file!"); 
    }


    // write some data to the memory mapped file for WindowsDLL to verify
    strcpy(memoryMappedView, AB_MEMORY_MAPPED_FILE_OK_ANSWER);


    return 0;
}


typedef struct EVENT_STRUCT
{
    char *buffer;
    int bufsize;
    HWND winAccessBridgeWindow;
    HWND ourAccessBridgeWindow;
}EVENT_STRUCT;


#include <process.h>
#define THREAD_PROC unsigned int __stdcall
typedef unsigned int (__stdcall *THREAD_ROUTINE)(LPVOID lpThreadParameter);

static HANDLE BeginThread(THREAD_ROUTINE thread_func,DWORD *id,DWORD param)
{
    HANDLE ret;
    ret = (HANDLE) _beginthreadex(NULL,0,thread_func,(void *)param,0,(unsigned int *)id);
    if(ret == INVALID_HANDLE_VALUE)
        ret = NULL;
    return(ret);
}

DWORD JavaBridgeThreadId = 0;

static THREAD_PROC JavaBridgeThread(LPVOID param1)
{
    MSG msg;
    DWORD rc = 0;
    while (GetMessage(&msg,        // message structure
                      NULL,                  // handle of window receiving the message
                      0,                  // lowest message to examine
                      0))                 // highest message to examine
        {
            if(msg.message == WM_USER)
                {
                    EVENT_STRUCT *event_struct = (EVENT_STRUCT *)msg.wParam;
                    COPYDATASTRUCT toCopy;
                    toCopy.dwData = 0;		// 32-bits we could use for something...
                    toCopy.cbData = event_struct->bufsize;
                    toCopy.lpData = event_struct->buffer;

                    LRESULT ret = SendMessage(event_struct->winAccessBridgeWindow, WM_COPYDATA,
                                              (WPARAM)event_struct->ourAccessBridgeWindow, (LPARAM) &toCopy);
                    delete event_struct->buffer;
                    delete event_struct;
                }
            if(msg.message == (WM_USER+1))
                PostQuitMessage(0);
        }
    JavaBridgeThreadId = 0;
    return(0);
}

/*
 * Handles one event
 */
static void do_event(char *buffer, int bufsize,HWND ourAccessBridgeWindow,HWND winAccessBridgeWindow)
{
    EVENT_STRUCT *event_struct = new EVENT_STRUCT;
    event_struct->bufsize = bufsize;
    event_struct->buffer = new char[bufsize];
    memcpy(event_struct->buffer,buffer,bufsize);
    event_struct->ourAccessBridgeWindow = ourAccessBridgeWindow;
    event_struct->winAccessBridgeWindow = winAccessBridgeWindow;
    if(!JavaBridgeThreadId)
        {
            HANDLE JavaBridgeThreadHandle = BeginThread(JavaBridgeThread,&JavaBridgeThreadId,(DWORD)event_struct);
            CloseHandle(JavaBridgeThreadHandle);
        }
    PostThreadMessage(JavaBridgeThreadId,WM_USER,(WPARAM)event_struct,0);
}


/**
 * sendJavaEventPackage - uses SendMessage(WM_COPYDATA) to do 
 *					      IPC messaging with the Java AccessBridge DLL
 *					      to propogate events to those ATs that want 'em
 *
 */
LRESULT
AccessBridgeATInstance::sendJavaEventPackage(char *buffer, int bufsize, long eventID) {

    PrintDebugString("AccessBridgeATInstance::sendJavaEventPackage() eventID = %X", eventID);
    PrintDebugString("AccessBridgeATInstance::sendJavaEventPackage() (using PostMessage) eventID = %X", eventID);

    if (eventID & javaEventMask) {
        do_event(buffer,bufsize,ourAccessBridgeWindow,winAccessBridgeWindow);
        return(0);
    } else {
        return -1;
    }
}


/**
 * uses SendMessage(WM_COPYDATA) to do 
 * IPC messaging with the Java AccessBridge DLL
 * to propogate events to those ATs that want 'em
 *
 */
LRESULT
AccessBridgeATInstance::sendAccessibilityEventPackage(char *buffer, int bufsize, long eventID) {

    PrintDebugString("AccessBridgeATInstance::sendAccessibilityEventPackage() eventID = %X", eventID);

    if (eventID & accessibilityEventMask) {
        do_event(buffer,bufsize,ourAccessBridgeWindow,winAccessBridgeWindow);
        return(0);
    } else {
        return -1;
    }
}


/**
 * findABATInstanceFromATHWND - walk through linked list from 
 *								where we are.  Return the  
 *								AccessBridgeATInstance 
 *								of the ABATInstance that 
 *								matches the passed in vmID;  
 *								no match: return 0
 */
AccessBridgeATInstance *
AccessBridgeATInstance::findABATInstanceFromATHWND(HWND window) {
    // no need to recurse really
    if (winAccessBridgeWindow == window) {
        return this;
    } else {
        AccessBridgeATInstance *current = nextATInstance;
        while (current != (AccessBridgeATInstance *) 0) {
            if (current->winAccessBridgeWindow == window) {
                return current;
            }
            current = current->nextATInstance;
        }
    }
    return (AccessBridgeATInstance *) 0;
}


