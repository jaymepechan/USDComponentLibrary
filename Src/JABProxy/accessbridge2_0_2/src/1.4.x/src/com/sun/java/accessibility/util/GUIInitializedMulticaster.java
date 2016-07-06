/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)GUIInitializedMulticaster.java	1.5 02/01/17
 */
package com.sun.java.accessibility.util;

import java.awt.*;
import java.util.EventListener;
import javax.accessibility.*;


/**
 * The GUIInitializedMulticaster class is used to maintain a list of
 * GUIInitializedListener classes.  It is intended to be used primarily
 * for internal support in the EventQueueMonitor class, and is not intended 
 * to be used by classes outside the Java Accessibility Utility package.
 * 
 * @see EventQueueMonitor
 * @see EventQueueMonitor#addGUIInitializedListener
 * @see EventQueueMonitor#removeGUIInitializedListener
 *
 * @version 1.5 01/17/02 16:10:58
 * @author Willie Walker
 */
public class GUIInitializedMulticaster 
    extends AWTEventMulticaster implements GUIInitializedListener
{
    protected GUIInitializedMulticaster(EventListener a, EventListener b) {
	super(a, b);
    }

    public void guiInitialized() {
	((GUIInitializedListener)a).guiInitialized();
	((GUIInitializedListener)b).guiInitialized();
    }

    public static GUIInitializedListener add(GUIInitializedListener a, GUIInitializedListener b) {
	return (GUIInitializedListener)addInternal(a, b);
    }

    public static GUIInitializedListener remove(GUIInitializedListener l, GUIInitializedListener oldl) {
	return (GUIInitializedListener)removeInternal(l, oldl);
    }

    protected static EventListener addInternal(EventListener a, EventListener b) {
	if (a == null)  return b;
	if (b == null)  return a;
	return new GUIInitializedMulticaster(a, b);
    }

    protected static EventListener removeInternal(EventListener l, EventListener oldl) {
	if (l == oldl || l == null) {
	    return null;
	} else if (l instanceof GUIInitializedMulticaster) {
	    return ((GUIInitializedMulticaster)l).remove(oldl);
	} else {
	    return l;		// it's not here
	}
    }

}
