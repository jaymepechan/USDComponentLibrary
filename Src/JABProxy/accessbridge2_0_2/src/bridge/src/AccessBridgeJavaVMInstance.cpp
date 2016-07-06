/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessBridgeJavaVMInstance.cpp	1.21 05/03/21
 */

/* 
 * A class to track key JVM instance info from the AT WinAccessBridge
 */

#ifdef _DEBUG
#define DEBUGGING_ON
#endif

#include "AccessBridgeDebug.h"
#include "AccessBridgeJavaVMInstance.h"
#include "AccessBridgeMessages.h"
#include "AccessBridgePackages.h"
#include "accessBridgeResource.h"	// for debugging messages

#include <winbase.h>
#include <jni.h>

// send memory lock
CRITICAL_SECTION sendMemoryIPCLock;

DEBUG(extern HWND theDialogWindow);
extern "C" {
    DEBUG(void AppendToCallInfo(char *s));
}


/**
 *
 *
 */
AccessBridgeJavaVMInstance::AccessBridgeJavaVMInstance(HWND ourABWindow, 
                                                       HWND javaABWindow,
                                                       long javaVMID, 
                                                       AccessBridgeJavaVMInstance *next) {
    goingAway = FALSE;
    InitializeCriticalSection(&sendMemoryIPCLock);
    ourAccessBridgeWindow = ourABWindow;
    javaAccessBridgeWindow = javaABWindow;
    vmID = javaVMID;
    nextJVMInstance = next;
    memoryMappedFileMapHandle = (HANDLE) 0;
    memoryMappedView = (char *) 0;
    sprintf(memoryMappedFileName, "AccessBridge-%X-%X.mmf",
            ourAccessBridgeWindow, javaAccessBridgeWindow);
}

/**
 *
 *
 */
AccessBridgeJavaVMInstance::~AccessBridgeJavaVMInstance() {
    DEBUG(char buffer[256]);

    DEBUG(AppendToCallInfo("\r\n***** in AccessBridgeJavaVMInstance::~AccessBridgeJavaVMInstance\r\n"));
    EnterCriticalSection(&sendMemoryIPCLock);

    // if IPC memory mapped file view is valid, unmap it
    goingAway = TRUE;
    if (memoryMappedView != (char *) 0) {
        DEBUG(sprintf(buffer, "  unmapping memoryMappedView; view = %X\r\n", memoryMappedView)); 
        DEBUG(AppendToCallInfo(buffer));
        UnmapViewOfFile(memoryMappedView);
        memoryMappedView = (char *) 0;
    }
    // if IPC memory mapped file handle map is open, close it
    if (memoryMappedFileMapHandle != (HANDLE) 0) {
        DEBUG(sprintf(buffer, "  closing memoryMappedFileMapHandle; handle = %X\r\n", memoryMappedFileMapHandle)); 
        DEBUG(AppendToCallInfo(buffer));
        CloseHandle(memoryMappedFileMapHandle);
        memoryMappedFileMapHandle = (HANDLE) 0;
    }
    LeaveCriticalSection(&sendMemoryIPCLock);

    // Do not delta lock since it is shared.
    // DeleteCriticalSection(&sendMemoryIPCLock);
}

/**
 * initiateIPC - sets up the memory-mapped file to do IPC messaging 
 *				 1 files is created: to handle requests for information
 *				 initiated from Windows AT.  The package is placed into
 *				 the memory-mapped file (char *memoryMappedView),
 *				 and then a special SendMessage() is sent.  When the
 *				 JavaDLL returns from SendMessage() processing, the
 *				 data will be in memoryMappedView.  The SendMessage()
 *				 return value tells us if all is right with the world.
 *
 *				 The set-up proces involves creating the memory-mapped
 *				 file, and handshaking with the JavaDLL so it knows
 *				 about it as well.
 *
 */
LRESULT
AccessBridgeJavaVMInstance::initiateIPC() {
    DEBUG(char debugBuf[256]);
    DWORD errorCode;

    DEBUG(AppendToCallInfo(" in AccessBridgeJavaVMInstance::initiateIPC()\r\n"));

    // create Windows-initiated IPC file & map it to a ptr
    memoryMappedFileMapHandle = CreateFileMapping((HANDLE)0xFFFFFFFF, NULL,
                                                  PAGE_READWRITE, 0,
                                                  // 8 bytes for return code
                                                  sizeof(WindowsInitiatedPackages) + 8,
                                                  memoryMappedFileName);
    if (memoryMappedFileMapHandle == NULL) {
        errorCode = GetLastError();
        DEBUG(sprintf(debugBuf, "  Failed to CreateFileMapping for %s, error: %X", memoryMappedFileName, errorCode)); 
        DEBUG(AppendToCallInfo(debugBuf));
        return errorCode;
    } else {
        DEBUG(sprintf(debugBuf, "  CreateFileMapping worked - filename: %s\r\n", memoryMappedFileName)); 
        DEBUG(AppendToCallInfo(debugBuf));
    }

    memoryMappedView = (char *) MapViewOfFile(memoryMappedFileMapHandle,
                                              FILE_MAP_READ | FILE_MAP_WRITE,
                                              0, 0, 0);
    if (memoryMappedView == NULL) {
        errorCode = GetLastError();
        DEBUG(sprintf(debugBuf, "  Failed to MapViewOfFile for %s, error: %X", memoryMappedFileName, errorCode)); 
        DEBUG(AppendToCallInfo(debugBuf));
        return errorCode;
    } else {
        DEBUG(sprintf(debugBuf, "  MapViewOfFile worked - view: %X\r\n", memoryMappedView)); 
        DEBUG(AppendToCallInfo(debugBuf));
    }


    // write some data to the memory mapped file
    strcpy(memoryMappedView, AB_MEMORY_MAPPED_FILE_OK_QUERY);


    // inform the JavaDLL that we've a memory mapped file ready for it
    char buffer[sizeof(PackageType) + sizeof(MemoryMappedFileCreatedPackage)];
    PackageType *type = (PackageType *) buffer;
    MemoryMappedFileCreatedPackage *pkg = (MemoryMappedFileCreatedPackage *) (buffer + sizeof(PackageType));
    *type = cMemoryMappedFileCreatedPackage;
    pkg->bridgeWindow = ourAccessBridgeWindow;
    strncpy(pkg->filename, memoryMappedFileName, cMemoryMappedNameSize);
    sendPackage(buffer, sizeof(buffer));


    // look for the JavaDLL's answer to see if it could read the file
    if (strcmp(memoryMappedView, AB_MEMORY_MAPPED_FILE_OK_ANSWER) != 0) {
        DEBUG(sprintf(debugBuf, "  JavaVM failed to deal with memory mapped file %s\r\n",
                      memoryMappedFileName)); 
        DEBUG(AppendToCallInfo(debugBuf));
        return -1;
    } else {
        DEBUG(sprintf(debugBuf, "  Success!  JavaVM accpeted our file\r\n")); 
        DEBUG(AppendToCallInfo(debugBuf));
    }

    return 0;
}

// -----------------------

/**
 * sendPackage - uses SendMessage(WM_COPYDATA) to do IPC messaging 
 *				 with the Java AccessBridge DLL
 *
 *				 NOTE: WM_COPYDATA is only for one-way IPC; there
 *				 is now way to return parameters (especially big ones)
 *				 Use sendMemoryPackage() to do that!
 */
LRESULT
AccessBridgeJavaVMInstance::sendPackage(char *buffer, long bufsize) {
    COPYDATASTRUCT toCopy;
    toCopy.dwData = 0;		// 32-bits we could use for something...
    toCopy.cbData = bufsize;
    toCopy.lpData = buffer;

    return SendMessage(javaAccessBridgeWindow, WM_COPYDATA,
                       (WPARAM) ourAccessBridgeWindow, (LPARAM) &toCopy);
}


/**
 * sendMemoryPackage - uses Memory-Mapped files to do IPC messaging 
 *					   with the Java AccessBridge DLL, informing the
 *					   Java AccessBridge DLL via SendMessage that something
 *					   is waiting for it in the shared file...
 *
 *					   In the SendMessage call, the third param (WPARAM) is
 *					   the source HWND (ourAccessBridgeWindow in this case), 
 *					   and the fourth param (LPARAM) is the size in bytes of
 *					   the package put into shared memory.
 *
 */
BOOL
AccessBridgeJavaVMInstance::sendMemoryPackage(char *buffer, long bufsize) {

    // Protect against race condition where the memory mapped file is
    // deallocated before the memory package is being sent
    if (goingAway) {
        return FALSE;
    }
    BOOL retval = FALSE;

    DEBUG(char outputBuf[256]);
    DEBUG(sprintf(outputBuf, "\r\nAccessBridgeJavaVMInstance::sendMemoryPackage(, %d)", bufsize)); 
    DEBUG(AppendToCallInfo(outputBuf));

    DEBUG(AppendToCallInfo("\r\n  'buffer' contains:"));
    DEBUG(PackageType *type = (PackageType *) buffer); 
    DEBUG(if (*type == cGetAccessibleTextRangePackage) {)
          DEBUG(GetAccessibleTextRangePackage *pkg = (GetAccessibleTextRangePackage *) (buffer + sizeof(PackageType))); 
          DEBUG(sprintf(outputBuf, "\r\n    PackageType = %X", *type)); 
          DEBUG(AppendToCallInfo(outputBuf));
          DEBUG(wsprintf(outputBuf, "\r\n    GetAccessibleTextRange: start = %d, end = %d, rText = %ls", 
                         pkg->start, pkg->end, pkg->rText));
          DEBUG(})
	DEBUG(AppendToCallInfo(outputBuf));

    EnterCriticalSection(&sendMemoryIPCLock);
    {
        // copy the package into shared memory
        if (!goingAway) {
            memcpy(memoryMappedView, buffer, bufsize);

            DEBUG(AppendToCallInfo("\r\n  'memoryMappedView' now contains:"));
            DEBUG(PackageType *type = (PackageType *) memoryMappedView); 
            DEBUG(if (*type == cGetAccessibleTextItemsPackage) {)
                  DEBUG(GetAccessibleTextItemsPackage *pkg = (GetAccessibleTextItemsPackage *) (buffer + sizeof(PackageType))); 
                  DEBUG(sprintf(outputBuf, "\r\n    PackageType = %X", *type)); 
                  DEBUG(AppendToCallInfo(outputBuf));
                  DEBUG(})
                DEBUG(AppendToCallInfo(outputBuf));
        }

        if (!goingAway) {
            // Let the recipient know there is a package waiting for them. The unset byte 
            // at end of buffer which will only be set if message is properly received 
            char *done = &memoryMappedView[bufsize];
            *done = 0;

            SendMessage(javaAccessBridgeWindow, AB_MESSAGE_WAITING,
                        (WPARAM) ourAccessBridgeWindow, (LPARAM) bufsize);

            // only succeed if message has been properly received
            if(!goingAway) retval = (*done == 1);
        }

        // copy the package back from shared memory
        if (!goingAway) {
            memcpy(buffer, memoryMappedView, bufsize);
        }
    }
    LeaveCriticalSection(&sendMemoryIPCLock);
    return retval;
}


/**
 * findAccessBridgeWindow - walk through linked list from where we are,
 *							return the HWND of the ABJavaVMInstance that 
 *							matches the passed in vmID; no match: return 0
 *
 */
HWND
AccessBridgeJavaVMInstance::findAccessBridgeWindow(long javaVMID) {
    // no need to recurse really
    if (vmID == javaVMID) {
        return javaAccessBridgeWindow;
    } else {
        AccessBridgeJavaVMInstance *current = nextJVMInstance;
        while (current != (AccessBridgeJavaVMInstance *) 0) {
            if (current->vmID == javaVMID) {
                return current->javaAccessBridgeWindow;
            }
            current = current->nextJVMInstance;
        }
    }
    return 0;
}

/**
 * findABJavaVMInstanceFromJavaHWND - walk through linked list from 
 *									  where we are.  Return the  
 *									  AccessBridgeJavaVMInstance 
 *									  of the ABJavaVMInstance that 
 *									  matches the passed in vmID;  
 *									  no match: return 0
 */
AccessBridgeJavaVMInstance *
AccessBridgeJavaVMInstance::findABJavaVMInstanceFromJavaHWND(HWND window) {
    // no need to recurse really
    if (javaAccessBridgeWindow == window) {
        return this;
    } else {
        AccessBridgeJavaVMInstance *current = nextJVMInstance;
        while (current != (AccessBridgeJavaVMInstance *) 0) {
            if (current->javaAccessBridgeWindow == window) {
                return current;
            }
            current = current->nextJVMInstance;
        }
    }
    return (AccessBridgeJavaVMInstance *) 0;
}

