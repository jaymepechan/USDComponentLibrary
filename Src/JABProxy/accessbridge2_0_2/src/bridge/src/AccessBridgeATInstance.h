/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

/* 
 * A class to track key AT instance info from the JavaAccessBridge
 */

#include <windows.h>
#include "AccessBridgePackages.h"

#ifndef __AccessBridgeATInstance_H__
#define __AccessBridgeATInstance_H__


/**
 * The AccessBridgeATInstance class.
 */
class AccessBridgeATInstance {
	friend class JavaAccessBridge;

	AccessBridgeATInstance *nextATInstance;
	HWND ourAccessBridgeWindow;
	HWND winAccessBridgeWindow;
	long javaEventMask;
	long accessibilityEventMask;

	// IPC variables
	HANDLE memoryMappedFileMapHandle;	// handle to file map
	char *memoryMappedView;				// ptr to shared memory
	char memoryMappedFileName[cMemoryMappedNameSize];

public:
	AccessBridgeATInstance(HWND ourABWindow, HWND winABWindow,
						   char *memoryFilename,
						   AccessBridgeATInstance *next);
	~AccessBridgeATInstance();
	LRESULT initiateIPC();
	LRESULT sendJavaEventPackage(char *buffer, int bufsize, long eventID);
	LRESULT sendAccessibilityEventPackage(char *buffer, int bufsize, long eventID);
	AccessBridgeATInstance *findABATInstanceFromATHWND(HWND window);
};

#endif
