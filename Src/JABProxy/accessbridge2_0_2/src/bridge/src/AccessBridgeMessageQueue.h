/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

/* 
 * A class to manage queueing of messages for IPC
 */

#include <windows.h> 

#ifndef __AccessBridgeMessageQueue_H__
#define __AccessBridgeMessageQueue_H__


enum QueueReturns {
    cQueueEmpty = 0,
    cMoreMessages = 1,
    cQueueInUse,
    cElementPushedOK,
    cQueueFull,
    cQueueOK,
    cQueueBroken		// shouldn't ever happen!
};

class AccessBridgeQueueElement {
    friend class AccessBridgeMessageQueue;
    friend class WinAccessBridge;
    char *buffer;
    int bufsize;
    AccessBridgeQueueElement *next;
    AccessBridgeQueueElement *previous;

public:
    AccessBridgeQueueElement(char *buf, int size);
    ~AccessBridgeQueueElement();
};

class AccessBridgeMessageQueue {
    BOOL queueLocked;
    BOOL queueRemoveLocked;
    AccessBridgeQueueElement *start;
    AccessBridgeQueueElement *end;
    int size;

public:
    AccessBridgeMessageQueue();
    ~AccessBridgeMessageQueue();

    int getEventsWaiting();

    QueueReturns add(AccessBridgeQueueElement *element);
    QueueReturns remove(AccessBridgeQueueElement **element);
    QueueReturns setRemoveLock(BOOL removeLockSetting);
    BOOL getRemoveLockSetting();
};


#endif
