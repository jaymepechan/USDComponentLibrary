/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessBridgeDebug.cpp	1.16 05/05/03
 */

/* 
 * A class to manage AccessBridge debugging
 */

#include "AccessBridgeDebug.h"
#include <stdarg.h>
#include <stdio.h>
#include <windows.h>

#ifdef __cplusplus
extern "C" {
#endif

/**
 * print a GetLastError message
 */
char *printError(char *msg) {
    LPVOID lpMsgBuf = NULL;
    static char retbuf[256];
    
    if (msg != NULL) {
        strncpy((char *)retbuf, msg, sizeof(retbuf));
    }
    if (!FormatMessage( 
                       FORMAT_MESSAGE_ALLOCATE_BUFFER | 
                       FORMAT_MESSAGE_FROM_SYSTEM | 
                       FORMAT_MESSAGE_IGNORE_INSERTS,
                       NULL,
                       GetLastError(),
                       MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
                       (LPTSTR) &lpMsgBuf,
                       0,
                       NULL ))
        {
            PrintDebugString("  %s: FormatMessage failed", msg);
        } else {
            PrintDebugString("  %s: %s", msg, (char *)lpMsgBuf);
        }
    if (lpMsgBuf != NULL) {
        strncat((char *)retbuf, ": ", sizeof(retbuf) - strlen(retbuf) - 1);
        strncat((char *)retbuf, (char *)lpMsgBuf, sizeof(retbuf) - strlen(retbuf) - 1);
    }
    return (char *)retbuf;
}


    /**
     * Send debugging info to the appropriate place
     */
    void PrintDebugString(char *msg, ...) {
#ifdef DEBUGGING_ON
        char buf[1024];
        va_list argprt;

        va_start(argprt, msg);     // set up argptr
        vsprintf(buf, msg, argprt);
#ifdef SEND_TO_OUTPUT_DEBUG_STRING
        OutputDebugString(buf);
#endif
#ifdef SEND_TO_CONSOLE
        printf(buf);
        printf("\r\n");
#endif
#endif
    }

    /**
     * Send Java debugging info to the appropriate place
     */
    void PrintJavaDebugString2(char *msg, ...) {
#ifdef JAVA_DEBUGGING_ON
        char buf[1024];
        va_list argprt;

        va_start(argprt, msg);     // set up argptr
        vsprintf(buf, msg, argprt);
#ifdef SEND_TO_OUTPUT_DEBUG_STRING
        OutputDebugString(buf);
#endif
#ifdef SEND_TO_CONSOLE
        printf(buf);
        printf("\r\n");
#endif
#endif
    }
    /**
     * Wide version of the method to send debugging info to the appropriate place
     */
    void wPrintDebugString(wchar_t *msg, ...) {
#ifdef DEBUGGING_ON
        char buf[1024];
        char charmsg[256];
        va_list argprt;

        va_start(argprt, msg);          // set up argptr
        wsprintf(charmsg, "%ls", msg);  // convert format string to multi-byte
        wvsprintf(buf, charmsg, argprt);
#ifdef SEND_TO_OUTPUT_DEBUG_STRING
        OutputDebugString(buf);
#endif
#ifdef SEND_TO_CONSOLE
        printf(buf);
        printf("\r\n");
#endif
#endif
    }

    /**
     * Wide version of the method to send Java debugging info to the appropriate place
     */
    void wPrintJavaDebugString(wchar_t *msg, ...) {
#ifdef JAVA_DEBUGGING_ON
        char buf[1024];
        char charmsg[256];
        va_list argprt;

        va_start(argprt, msg);          // set up argptr
        wsprintf(charmsg, "%ls", msg);  // convert format string to multi-byte
        wvsprintf(buf, charmsg, argprt);
#ifdef SEND_TO_OUTPUT_DEBUG_STRING
        OutputDebugString(buf);
#endif
#ifdef SEND_TO_CONSOLE
        printf(buf);
        printf("\r\n");
#endif
#endif
    }
#ifdef __cplusplus
}
#endif

