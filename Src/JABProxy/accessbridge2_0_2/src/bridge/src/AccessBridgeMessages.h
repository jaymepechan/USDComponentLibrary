/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

/* 
 * Common AccessBridge IPC message definitions
 */

#include <windows.h>
#include <winuser.h>

#ifndef __AccessBridgeMessages_H__
#define __AccessBridgeMessages_H__


// used for messages between AccessBridge dlls to manage IPC
// In the SendMessage call, the third param (WPARAM) is
// the source HWND (ourAccessBridgeWindow in this case), 
// and the fourth param (LPARAM) is the size in bytes of
// the package put into shared memory.
#define AB_MEMORY_MAPPED_FILE_SETUP (WM_USER+0x1000)

// used for messages between AccessBridge dlls to manage IPC
// In the SendMessage call, the third param (WPARAM) is
// the source HWND (ourAccessBridgeWindow in this case), 
// and the fourth param (LPARAM) is the size in bytes of
// the package put into shared memory.
#define AB_MESSAGE_WAITING (WM_USER+0x1001)

// used for messages from JavaDLL to itself (or perhaps later also
// for messages from WindowsDLL to itself).  Used with PostMessage,
// it is called for deferred processing of messages to send across
// to another DLL (or DLLs)
#define AB_MESSAGE_QUEUED (WM_USER+0x1002)

// used to let other AccessBridge DLLs know that one of the DLLs
// they are communicating with is going away (not reversable)
#define AB_DLL_GOING_AWAY (WM_USER+0x1003)


// used as part of the Memory-Mapped file IPC setup.  The first
// constant is the query, the second the response, that are put
// into the memory mapped file for reading by the opposite DLL
// to verify that communication is working
#define AB_MEMORY_MAPPED_FILE_OK_QUERY "OK?"
#define AB_MEMORY_MAPPED_FILE_OK_ANSWER "OK!"


BOOL initBroadcastMessageIDs();


#endif
