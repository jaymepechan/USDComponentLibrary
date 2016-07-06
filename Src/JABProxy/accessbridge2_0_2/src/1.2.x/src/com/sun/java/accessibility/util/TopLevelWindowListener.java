/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

package com.sun.java.accessibility.util;

import java.awt.*;
import java.util.*;
import javax.accessibility.*;

/**
 * The TopLevelWindowListener interface is used by the EventQueueMonitor
 * class to notify an interested party when a top level window is created
 * or destroyed in the Java Virtual Machine.  Classes wishing to express
 * an interest in top level window events should implement this interface
 * and register themselves with the EventQueueMonitor by calling the 
 * EventQueueMonitor.addTopLevelWindowListener class method.
 *
 * @see EventQueueMonitor
 * @see EventQueueMonitor#addTopLevelWindowListener
 * @see EventQueueMonitor#removeTopLevelWindowListener
 *
 * @version	%I% %G% %U%
 * @author	Willie Walker
 */
public interface TopLevelWindowListener extends EventListener {

    /**
     * Invoked when a new top level window has been created.
     * @param w the Window that was created
     */
    public void topLevelWindowCreated(Window w);

    /**
     * Invoked when a top level window has been destroyed.
     * @param w the Window that was destroyed
     */
    public void topLevelWindowDestroyed(Window w);
}
