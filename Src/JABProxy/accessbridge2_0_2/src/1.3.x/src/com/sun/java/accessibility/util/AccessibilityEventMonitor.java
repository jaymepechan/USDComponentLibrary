/*
 * Copyright 2004 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibilityEventMonitor.java	1.18 04/11/02
 */

package com.sun.java.accessibility.util;

import java.util.*;
import java.beans.*;
import java.awt.*;
import java.awt.event.*;
import javax.accessibility.*;

/**
 * <P>The AccessibilityEventMonitor implements a PropertyChange listener on 
 * every UI object that implements interface Accessibility in the Java 
 * Virtual Machine.  The events captured by these listeners are made available 
 * through a listeners supported by AccessibilityEventMonitor.
 * With this, all the individual events on each of the ui object
 * instances are funnelled into one set of PropertyChange listeners.
 * <p>This class depends upon EventQueueMonitor, which provides the base
 * level support for capturing the top-level containers as they are created.
 * <p>For JDK1.1 only, the EventQueueMonitor class needs to be in 
 * the CLASSPATH environment variable and the following line needs to be in 
 * the awt.properties file:
 * <pre>
 * AWT.EventQueueClass=com.sun.java.accessibility.util.EventQueueMonitor
 * </pre>
 * <p>For JDK1.2, EventQueueMonitor merely needs to be in the CLASSPATH.
 * The best way for this to happen is to place the jaccess.jar file in
 * the standard extensions directory (e.g., $JDKHOME/jre/lib/ext).
 *
 * @version     1.18 11/02/04 10:16:45
 * @author      Peter Korn
 */

public class AccessibilityEventMonitor {

    // listeners
    /** 
     * The current list of registered PropertyChangeListener classes. 
     * @see #addPropertyChangeListener
     * @see #removePropertyChangeListener
     */
    static protected AccessibilityListenerList listenerList = 
	new AccessibilityListenerList();


    /**
     * The actual listener that is installed on the component instances.
     * This listener calls the other registered listeners when an event 
     * occurs.  By doing things this way, the actual number of listeners
     * installed on a component instance is drastically reduced.
     */
    static protected AccessibilityEventListener accessibilityListener = 
	new AccessibilityEventListener();

    /**
     * Adds the specified listener to receive all PropertyChange events on 
     * each UI object instance in the Java Virtual Machine as they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to UI object instances that support this listener type.
     * @see #removePropertyChangeListener
     * @param l the listener to add
     */
    static public void addPropertyChangeListener(PropertyChangeListener l) {
        if (listenerList.getListenerCount(PropertyChangeListener.class) == 0) {
            accessibilityListener.installListeners();
        }
        listenerList.add(PropertyChangeListener.class, l);
    }

    /**
     * Removes the specified listener so it no longer receives PropertyChange 
     * events when they occur.
     * @see #addPropertyChangeListener
     * @param l the listener to remove
     */
    static public void removePropertyChangeListener(PropertyChangeListener l) {
        listenerList.remove(PropertyChangeListener.class, l);
        if (listenerList.getListenerCount(PropertyChangeListener.class) == 0) {
            accessibilityListener.removeListeners();
        }
    }


    /**
     * AccessibilityEventListener is the class that does all the work for 
     * AccessibilityEventMonitor.  It is not intended for use by any other 
     * class except AccessibilityEventMonitor.
     *
     */

    static class AccessibilityEventListener implements TopLevelWindowListener,
		PropertyChangeListener {

	/**
	 * Create a new instance of this class and install it on each component
	 * instance in the virtual machine that supports any of the currently
	 * registered listeners in AccessibilityEventMonitor.  Also registers 
	 * itself as a TopLevelWindowListener with EventQueueMonitor so it can
	 * automatically add new listeners to new components.
	 * @see EventQueueMonitor
	 * @see AccessibilityEventMonitor
	 */
	public AccessibilityEventListener() {
	    EventQueueMonitor.addTopLevelWindowListener(this);
	}

	/**
	 * Installs PropertyChange listeners on all Accessible objects based 
	 * upon the current topLevelWindows cached by EventQueueMonitor.
	 * @see EventQueueMonitor
	 * @see AWTEventMonitor
	 */
	protected void installListeners() {
	    Window topLevelWindows[] = EventQueueMonitor.getTopLevelWindows();
	    if (topLevelWindows != null) {
		for (int i = 0; i < topLevelWindows.length; i++) {
		    if (topLevelWindows[i] instanceof Accessible) {
			installListeners((Accessible) topLevelWindows[i]);
		    }
		}
	    }
	}

	/**
	 * Installs PropertyChange listeners to the Accessible object, and it's
	 * children (so long as the object isn't of TRANSIENT state).
	 * @param a the Accessible object to add listeners to
	 */
	protected void installListeners(Accessible a) {    
	    installListeners(a.getAccessibleContext());
	}

	/**
	 * Installs PropertyChange listeners to the AccessibleContext object, 
	 * and it's * children (so long as the object isn't of TRANSIENT state).
	 * @param a the Accessible object to add listeners to
	 */
	private void installListeners(AccessibleContext ac) {    
	    
	    if (ac != null) {
		AccessibleStateSet states = ac.getAccessibleStateSet();
		if (!states.contains(AccessibleState.TRANSIENT)) {
		    ac.addPropertyChangeListener(this);

		    Accessible child;
		    int count = ac.getAccessibleChildrenCount();

                    // don't install listeners on transient children
                    AccessibleRole role = ac.getAccessibleRole();
                    if (role == AccessibleRole.LIST ||
                        role == AccessibleRole.TREE) {
                        return;
                    }
                    if (role == AccessibleRole.TABLE) {
			// handle Oracle tables containing tables
			child = ac.getAccessibleChild(0);
			if (child != null) {
			    AccessibleContext ac2 = child.getAccessibleContext();
			    if (ac2 != null) {
				role = ac2.getAccessibleRole();
				if (role != AccessibleRole.TABLE &&
                                    role != null) {
				    return;
				}
			    }
			}
                    }
		    
		    for (int i = 0; i < count; i++) {
			child = ac.getAccessibleChild(i);
			if (child != null) {
			    installListeners(child);
			}
		    }
		} 
	    }
	}
		
	/**
	 * Removes PropertyChange listeners on all Accessible objects based
	 * upon the topLevelWindows cached by EventQueueMonitor.
	 * @param eventID the event ID
	 * @see EventID
	 */
	protected void removeListeners() {
	    Window topLevelWindows[] = EventQueueMonitor.getTopLevelWindows();
	    if (topLevelWindows != null) {
		for (int i = 0; i < topLevelWindows.length; i++) {
		    if (topLevelWindows[i] instanceof Accessible) {
			removeListeners((Accessible) topLevelWindows[i]);
		    }
		}
	    }
	}

	/**
	 * Removes PropertyChange listeners for the given Accessible object,
	 * it's children (so long as the object isn't of TRANSIENT state).
	 * @param a the Accessible object to remove listeners from
	 */
	protected void removeListeners(Accessible a) {
	    removeListeners(a.getAccessibleContext());
	}

	/**
	 * Removes PropertyChange listeners for the given AccessibleContext
	 * object, it's children (so long as the object isn't of TRANSIENT 
	 * state).
	 * @param a the Accessible object to remove listeners from
	 */
	private void removeListeners(AccessibleContext ac) {
	    
	    if (ac != null) {
		AccessibleStateSet states = ac.getAccessibleStateSet();
		if (!states.contains(AccessibleState.TRANSIENT)) {
		    ac.removePropertyChangeListener(this);
		    
		    int count = ac.getAccessibleChildrenCount();
		    for (int i = 0; i < count; i++) {
			Accessible child = ac.getAccessibleChild(i);
			if (child != null) {
			    removeListeners(child);
			}
		    }
		} 
	    }
	}

	/********************************************************************/
	/*                                                                  */
	/* Listener Interface Methods                                       */
	/*                                                                  */
	/********************************************************************/
     
	/* TopLevelWindow Methods ***************************************/
     
	/**
	 * Called when top level window is created.
	 * @see EventQueueMonitor
	 * @see EventQueueMonitor#addTopLevelWindowListener
	 */
	public void topLevelWindowCreated(Window w) {
	    if (w instanceof Accessible) {
		installListeners((Accessible) w);
	    }
	}

	/**
	 * Called when top level window is destroyed.
	 * @see EventQueueMonitor
	 * @see EventQueueMonitor#addTopLevelWindowListener
	 */
	public void topLevelWindowDestroyed(Window w) {
	    if (w instanceof Accessible) {
		removeListeners((Accessible) w);
	    }
	}


	/* PropertyChangeListener Methods **************************************/

	public void propertyChange(PropertyChangeEvent e) {
	    // propogate the event
	    Object[] listeners = 
		    AccessibilityEventMonitor.listenerList.getListenerList();
	    for (int i = listeners.length-2; i>=0; i-=2) {
		if (listeners[i]==PropertyChangeListener.class) {
		    ((PropertyChangeListener)listeners[i+1]).propertyChange(e);
		}
	    }

	    // handle childbirth/death
	    String name = e.getPropertyName();
	    if (name.compareTo(AccessibleContext.ACCESSIBLE_CHILD_PROPERTY) == 0) {
		Object oldValue = e.getOldValue();
		Object newValue = e.getNewValue();

		if ((oldValue == null) ^ (newValue == null)) { // one null, not both
		    if (oldValue != null) {
			// this Accessible is a child that's going away
			if (oldValue instanceof Accessible) {
			    Accessible a = (Accessible) oldValue;
			    removeListeners(a.getAccessibleContext());
			} else if (oldValue instanceof AccessibleContext) {
			    removeListeners((AccessibleContext) oldValue);
			}
		    } else if (newValue != null) {
			// this Accessible is a child was just born
			if (newValue instanceof Accessible) {
			    Accessible a = (Accessible) newValue;
			    installListeners(a.getAccessibleContext());
			} else if (newValue instanceof AccessibleContext) {
			    installListeners((AccessibleContext) newValue);
			}
		    }
		} else {
		    System.out.println("ERROR in usage of PropertyChangeEvents for: " + e.toString());
		}
	    }
	}
    }
}
