/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)GUIInitializedListener.java	1.5 02/01/17
 */

package com.sun.java.accessibility.util;

import java.awt.*;
import java.util.*;
import javax.accessibility.*;

/**
 * The GUIInitializedListener interface is used by the EventQueueMonitor
 * class to notify an interested party when the GUI subsystem has been
 * initialized.  This is necessary because assistive technologies can
 * be loaded before the GUI subsystem is initialized.  As a result, 
 * assistive technologies should check the isGUIInitialized() method 
 * of EventQueueMonitor before creating any GUI components.  If the
 * return value is true, assistive technologies can create GUI components
 * following the same thread restrictions as any other application.  If
 * the return value is false, the assistive technology should register
 * a GUIInitializedListener with the EventQueueMonitor to be notified
 * when the GUI subsystem is initialized.
 *
 * @see EventQueueMonitor
 * @see EventQueueMonitor#isGUIInitialized
 * @see EventQueueMonitor#addGUIInitializedListener
 * @see EventQueueMonitor#removeGUIInitializedListener
 *
 * @version	1.5 01/17/02 16:08:54
 * @author	Willie Walker
 */
public interface GUIInitializedListener extends EventListener {

    /**
     * Invoked when the GUI subsystem is initialized and it's OK for
     * the assisitive technology to create instances of GUI objects.
     */
    public void guiInitialized();

}
