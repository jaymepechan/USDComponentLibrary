/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessBridgeDebug.h	1.20 05/05/03
 */

/* 
 * A class to manage AccessBridge debugging
 */

#ifndef __AccessBridgeDebug_H__
#define __AccessBridgeDebug_H__

#include <crtdbg.h>
#include <windows.h>

// #define DEBUGGING_ON
// #define JAVA_DEBUGGING_ON

#ifdef _DEBUG
#define DEBUGGING_ON
#endif

#ifdef DEBUGGING_ON
#define DEBUG(x) x
#else
#define DEBUG(x) /* */
#endif

#ifdef __cplusplus
extern "C" {
#endif

    char *printError(char *msg);
    void PrintDebugString(char *msg, ...);
    void PrintJavaDebugString(char *msg, ...);
    void wPrintJavaDebugString(wchar_t *msg, ...);
    void wPrintDebugString(wchar_t *msg, ...);

#ifdef __cplusplus
}
#endif


#endif
