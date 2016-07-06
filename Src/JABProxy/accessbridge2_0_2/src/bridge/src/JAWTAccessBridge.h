/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)JAWTAccessBridge.h	1.9 05/03/21
 */

/* 
 * A DLL which is loaded by Java applications to handle communication
 * between Java VMs purposes of Accessbility.
 */

#include <windows.h>
#include <jni.h>

#include "AccessBridgePackages.h"

#ifndef __JAWTAccessBridge_H__
#define __JAWTAccessBridge_H__


extern "C" {
	BOOL WINAPI DllMain(HINSTANCE hinstDll, DWORD fdwReason, 
						LPVOID lpvReserved);
}

#endif
