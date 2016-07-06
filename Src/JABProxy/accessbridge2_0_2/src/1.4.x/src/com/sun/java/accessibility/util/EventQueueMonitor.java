/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)EventQueueMonitor.java	1.53 06/06/21
 */

package com.sun.java.accessibility.util;

import java.util.*;
import java.awt.*;
import java.awt.event.*;
import javax.accessibility.*;
import java.security.AccessController;
import java.security.PrivilegedAction;

/**
 * <P>The EventQueueMonitor is an EventQueue subclass that provides key
 * core functionality for Assistive Technologies (and other system-level
 * technologies that need some of the same things that Assistive Technology
 * needs).
 * <P>For JDK1.1, this class is meant to be automatically loaded by the 
 * Toolkit when it starts up.  In order for this to happen, this class 
 * needs to be in the CLASSPATH environment variable and the following 
 * line needs to be in the awt.properties file:
 * <pre>
 * AWT.EventQueueClass=com.sun.java.accessibility.util.EventQueueMonitor
 * </pre>
 * <P>For JDK1.1, this class also looks for the AWT.assistive_technologies 
 * property in the awt.properties file.  This is a comma-separated list of 
 * classes that represent assistive technologies that should run in the same 
 * Java Virtual Machine as the application.  If this property exists, this 
 * class will load each class in the list and create a single instance 
 * of it in the current thread.
 * <P>This class is commonly used with one or two other classes, 
 * AWTEventMonitor and SwingEventMonitor, that track all events generated
 * by AWT obects and Swing objects respectively.
 * <p>For JDK1.2, EventQueueMonitor merely needs to be in the CLASSPATH.
 * The best way for this to happen is to place the jaccess.jar file in
 * the standard extensions directory (e.g., $JDKHOME/jre/lib/ext).  In 
 * addition, the specification of the assistive technologies should no
 * longer be in awt.properties for JDK1.2.  Instead, the assistive technologies
 * should be specified using the 'assistive_technologies' property in
 * $JDKHOME/jre/lib/accessibility.properties.
 *
 * @see AWTEventMonitor
 * @see SwingEventMonitor
 *
 * @version	1.53 06/21/06
 * @author	Willie Walker
 * @author	Peter Korn
 * @author	Lynn Monsanto
 */
public class EventQueueMonitor 
	implements AWTEventListener {

    // NOTE:  All of the following properties are static.  The reason
    //        for this is that there may be multiple EventQueue instances
    //        in use in the same VM.  By making these properties static,
    //        we can guarantee we get the information from all of the 
    //        EventQueue instances.
 
    // The stuff that is cached.
    //
    static Vector topLevelWindows          = new Vector();
    static Window topLevelWindowWithFocus  = null;
    static Point currentMousePosition      = null;
    static Component currentMouseComponent = null;

    // Low-level listener interfaces
    //
    static GUIInitializedListener guiInitializedListener = null;
    static TopLevelWindowListener topLevelWindowListener = null;
    static MouseMotionListener    mouseMotionListener    = null;

    /** 
     * Class variable stating whether the assistive technologies have
     * been loaded yet or not.  The assistive technologies won't be
     * loaded until the first event is posted to the EventQueue.  This
     * gives the toolkit a chance to do all the necessary initialization
     * it needs to do.
     */

    /** 
     * Class variable stating whether the GUI subsystem has been initialized
     * or not.
     * @see #isGUIInitialized
     */
    static boolean guiInitialized = false;

    /**
     * Queue that holds events for later processing.
     */
    static EventQueueMonitorItem componentEventQueue = null;

    /**
     * Class that tells us what the component event dispatch thread is.
     */
    static private ComponentEvtDispatchThread cedt = null;

    /**
     * Handle the synchronization between the thing that populates the
     * component event dispatch thread (EventQueueMonitor.queueComponentEvent)
     * and the thing that processes the events (ComponentEvtDispatchThread).
     */
    static Object componentEventQueueLock = new Object();

    /**
     * Create a new EventQueueMonitor instance.  Normally, this will
     * be called only by the AWT Toolkit during initialization time.
     * Assistive technologies should not create instances of 
     * EventQueueMonitor by themselves.  Instead, they should either
     * refer to it directly via the static methods in this class
     * (e.g., EventQueueMonitor.getCurrentMousePosition) or obtain
     * the instance by asking the Toolkit (e.g., Toolkit.getSystemEventQueue).
     */
    public EventQueueMonitor() {
        super();
	    if (cedt == null) {
	        cedt = new ComponentEvtDispatchThread("EventQueueMonitor-ComponentEvtDispatch");
	        cedt.start();
	    }
    }

    /**
     * Queue up a ComponentEvent for later processing by the 
     * ComponentEvtDispatch thread.
     */
    static void queueComponentEvent(ComponentEvent e) {
	    synchronized(componentEventQueueLock) {
            EventQueueMonitorItem eqi = new EventQueueMonitorItem(e);
            if (componentEventQueue == null) {
                componentEventQueue = eqi;
            } else {
                EventQueueMonitorItem q = componentEventQueue;
                while (true) {
                    if (q.next != null) {
                        q = q.next;
                    } else {
                        break;
                    }
                }
                q.next = eqi;
            }
	        componentEventQueueLock.notifyAll();
	    }
    }

//////////////////////////////////////////////////
// Stuff that differs between JDK1.1 and JDK1.2 //
// vvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvvv //
//////////////////////////////////////////////////

    /**
     * Tell the EventQueueMonitor to start listening for events if
     * this is JDK1.2 or higher.  If the JDK is 1.1, then this 
     * class is the EventQueue and is already listening for events
     * in the postEvent method.
     */
 public static void maybeInitialize() {
     if (cedt == null) {
	 java.security.AccessController.doPrivileged(
	    new java.security.PrivilegedAction() {
		    public Object run() {
			try {
			    long eventMask = AWTEvent.WINDOW_EVENT_MASK | 
				AWTEvent.FOCUS_EVENT_MASK;
			    
			    Toolkit.getDefaultToolkit().addAWTEventListener(new EventQueueMonitor(), eventMask);
			} catch (Exception e) {
			}
			return null;
		    }
	 });
     }
 }

    /**
     * Post a 1.1-style event to the EventQueue.  This is only used
     * for JDK1.1.  If the JDK is 1.2 or higher, maybeInitialize will
     * have registered a listener on the EventQueue and events will
     * be delivered to the eventPosted method.
     * @param theEvent the AWTEvent
     */

    /**
     * Handle eventPosted events as a result of registering a listener
     * on the EventQueue in initialize().  This is for JDK1.2 support.
     * If this is JDK1.1, then this is the EventQueue and events will
     * be delivered to postEvent.
     */
 /* public void eventPosted(AWTEvent theEvent) {  */
  public void eventDispatched(AWTEvent theEvent) { 
     processEvent(theEvent);
 }

    /**
     * For JDK1.1, loads additional classes into the Java Virtual Machine using 
     * the AWT.assistive_technologies property in the awt.properties file.
     * The property specifies a comma-separated list of assistive technology
     * classes this class will load just before the first event is posted
     * to the event queue.  The classes will be loaded in the order specified
     * in the property, and a single instance of each will be created.
     * <p>This will only be done if the JDK version is less than 1.2 
     * because Toolkit.java executes this same code as of JDK1.2.  If the
     * JDK version is 1.2 or higher, assisitive technologies that have
     * registered a GUIInitializedListener will be notified.
     * @see #addGUIInitializedListener
     */
    static void maybeNotifyAssistiveTechnologies() {

        if (!guiInitialized) {
  	    guiInitialized = true;
    	    if (guiInitializedListener != null) {
	        guiInitializedListener.guiInitialized();
            }
	}

    }

//////////////////////////////////////////////////
// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ //
// Stuff that differs between JDK1.1 and JDK1.2 //
//////////////////////////////////////////////////

    /********************************************************************/
    /*                                                                  */
    /* Package Private Methods                                          */
    /*                                                                  */
    /********************************************************************/

    /**
     * Add a Container to the list of top-level containers
     * in the cache.  This follows the object's hierarchy up the
     * tree until it finds the top most parent.  If the parent is
     * not already in the list of Containers, it adds it to the list.
     * @param c the Container
     */
    static boolean addTopLevelWindow(Component c) {
        Container parent;

        if (c == null) {
	        return false;
        }

        if (!(c instanceof Window)) {
	        return addTopLevelWindow(c.getParent());
        }

        if ((c instanceof Dialog) || (c instanceof Window)) {
	        parent = (Container) c;
        } else {
	        parent = c.getParent();
            if (parent != null) {
		        return addTopLevelWindow(parent);
	        }
        }

        if (parent == null) {
            parent = (Container) c;
        }

        // Because this method is static, do not make it synchronized because
        // it can lock the whole class.  Instead, just lock what needs to be
        // locked.
        //
        synchronized (topLevelWindows) {
            if ((parent != null) && !topLevelWindows.contains(parent)) {
                topLevelWindows.addElement(parent);
                if (topLevelWindowListener != null) {
    		        topLevelWindowListener.topLevelWindowCreated((Window) parent);
		        }
                return true;
            } else {
                return false;
            }
        }
    }

    /**
     * Remove a container from the list of top level containers in the cache.
     * @param c the top level container to remove
     */
    static void removeTopLevelWindow(Window w) {

        // Because this method is static, do not make it synchronized because
        // it can lock the whole class.  Instead, just lock what needs to be
        // locked.
        //
        synchronized (topLevelWindows) {
            if (topLevelWindows.contains(w)) {
                topLevelWindows.removeElement(w);
                if (topLevelWindowListener != null) {
    		        topLevelWindowListener.topLevelWindowDestroyed(w);
		        }
            }
        }
    }

    /**
     * Update current mouse position.
     * @param mouseEvent the MouseEvent that holds the new mouse position.
     */
    static void updateCurrentMousePosition(MouseEvent mouseEvent) {
        Point oldMousePos = currentMousePosition;
	// Be careful here.  The component in the event might be
	// hidden by the time we process the event.
        try {
	        Point eventPoint      = mouseEvent.getPoint();
            currentMouseComponent = (Component) (mouseEvent.getSource());
            currentMousePosition  = currentMouseComponent.getLocationOnScreen();
            currentMousePosition.translate(eventPoint.x,eventPoint.y);
	    } catch (Exception e) {
	        currentMousePosition = oldMousePos;
        }
    }

    /**
     * Process the event.  This maintains the event cache in addition
     * to calling all the registered listeners.  NOTE: The events that
     * come through here are from peered Components.
     * @param theEvent the AWTEvent
     */
    static void processEvent(AWTEvent theEvent) {
        switch (theEvent.getID()) {
        case MouseEvent.MOUSE_MOVED:
        case MouseEvent.MOUSE_DRAGGED:
        case FocusEvent.FOCUS_GAINED:
        case WindowEvent.WINDOW_DEACTIVATED:
	    queueComponentEvent((ComponentEvent) theEvent);
            break;

        case WindowEvent.WINDOW_ACTIVATED:
            // Dialogs fire WINDOW_ACTIVATED and FOCUS_GAINED events
            // before WINDOW_OPENED so we need to add topLevelListeners
            // for the dialog when it is first activated to get a
            // focus gained event for the focus component in the dialog.
            if (theEvent instanceof ComponentEvent) {
                ComponentEvent ce = (ComponentEvent)theEvent;
                if (ce.getComponent() instanceof Window) {
                    EventQueueMonitor.addTopLevelWindow(ce.getComponent());
                    EventQueueMonitor.maybeNotifyAssistiveTechnologies();
                } else {
                    EventQueueMonitor.maybeNotifyAssistiveTechnologies();
                    EventQueueMonitor.addTopLevelWindow(ce.getComponent());
                }
            }
            queueComponentEvent((ComponentEvent) theEvent);
            break;

	    // handle WINDOW_OPENED and WINDOW_CLOSED events synchronously
	case WindowEvent.WINDOW_OPENED:
	    if (theEvent instanceof ComponentEvent) {
		ComponentEvent ce = (ComponentEvent)theEvent;
		if (ce.getComponent() instanceof Window) {
		    EventQueueMonitor.addTopLevelWindow(ce.getComponent());
		    EventQueueMonitor.maybeNotifyAssistiveTechnologies();
		} else {
		    EventQueueMonitor.maybeNotifyAssistiveTechnologies();
		    EventQueueMonitor.addTopLevelWindow(ce.getComponent());
		}
	    }
	    break;
	case WindowEvent.WINDOW_CLOSED:
	    if (theEvent instanceof ComponentEvent) {
		ComponentEvent ce = (ComponentEvent)theEvent;
		EventQueueMonitor.removeTopLevelWindow((Window) (ce.getComponent()));
	    }
	    break;

        default:
            break;
        }
    }

    /**
     * Internal test
     */
    static synchronized Component getShowingComponentAt(Container c, int x, int y) {
        if (!c.contains(x, y)) {
            return null;
        }
	    int ncomponents = c.getComponentCount();
	    for (int i = 0 ; i < ncomponents ; i++) {
	        Component comp = c.getComponent(i);
	        if (comp != null && comp.isShowing()) {
		        Point location = comp.getLocation();
		        if (comp.contains(x - location.x, y - location.y)) {
		            return comp;
		        }
	        }
	    }
        return c;
    }

    /** 
     * Return the Component at the given Point on the screen in the
     * given Container.
     * @param c the Container to search
     * @param p the Point in screen coordinates
     * @return the Component at the given Point on the screen in the
     * given Container -- can be null if no Component is at that Point
     */
    static synchronized Component getComponentAt(Container c, Point p) {
	    if (!c.isShowing()) {
	        return null;
        }

        Component comp;
        Point containerLoc = c.getLocationOnScreen();
        Point containerPoint = new Point(p.x - containerLoc.x,
                                         p.y - containerLoc.y);
        
        comp = getShowingComponentAt(c, containerPoint.x, containerPoint.y);

        if ((comp != c) && (comp instanceof Container)) {
            return getComponentAt((Container)comp,p);
        } else {
            return comp;
        }
    }
 
    /**
     * Obtain the Accessible object at the given point on the Screen.
     * The return value may be null if an Accessible object cannot be
     * found at the particular point.
     */
    static public Accessible getAccessibleAt(Point p) {
        Window w = getTopLevelWindowWithFocus();
        Window[] wins = getTopLevelWindows();
        Component c = null;

        // See if the point we're being asked about is the 
        // currentMousePosition.  If so, start with the component
        // that we know the currentMousePostion is over
        //
	if (currentMousePosition == null) {
            return null;
	}
        if (currentMousePosition.equals(p)) {
	        if (currentMouseComponent instanceof Container) {
	            c = getComponentAt((Container) currentMouseComponent, p);
	        }
        }

        // Try the window with focus next
        //
        if (c == null && w != null) {
            c = getComponentAt(w,p);
        }

        // Try the other windows next.  [[[WDW: Stacking order???]]]
        if (c == null) {
            for (int i = 0; i < wins.length; i++) {
                c = getComponentAt(wins[i],p);
                if (c != null) {
                    break;
                }
            }
        }


        if (c instanceof Accessible) {
	        AccessibleContext ac = ((Accessible) c).getAccessibleContext();
	        if (ac != null) {
		        AccessibleComponent acmp = ac.getAccessibleComponent();
		        if ((acmp != null) && (ac.getAccessibleChildrenCount() != 0)) {
		            Point location = acmp.getLocationOnScreen();
		            location.move(p.x - location.x, p.y - location.y);
		            return acmp.getAccessibleAt(location);
	            }
            }
            return (Accessible) c; 
        } else {
            return Translator.getAccessible(c);
//            Component parent = c.getParent();
//            while (parent != null) {
//                if (parent instanceof Accessible) {
//		    System.out.println("getAccessibleAt returning " + c);
//		    return (Accessible) parent;
//                } else {
//                    parent = parent.getParent();
//                }
//            }
//            return null;
        }
    }

    /********************************************************************/
    /*                                                                  */
    /* Public Methods                                                   */
    /*                                                                  */
    /********************************************************************/

    /**
     * Says whether the GUI subsystem has been initialized or not.
     * If this returns true, the assistive technology can freely
     * create GUI component instances.  If the return value is false,
     * the assistive technology should register a GUIInitializedListener
     * and wait to create GUI component instances until the listener is
     * called.
     * @see #addGUIInitializedListener
     */
    static public boolean isGUIInitialized() {
 maybeInitialize();
        return guiInitialized;
    }

    /**
     * Adds the specified listener to be notified when the GUI subsystem
     * is initialized.  Assistive technologies should get the results of
     * isGUIInitialized before calling this method.
     * is created or destroyed.
     * @see #isGUIInitialized
     * @see #removeTopLevelWindowListener
     * @param l the listener to add
     */
    static public void addGUIInitializedListener(GUIInitializedListener l) {
 maybeInitialize();
        guiInitializedListener = 
	    GUIInitializedMulticaster.add(guiInitializedListener,l);
    }

    /**
     * Removed the specified listener to be notified when the GUI subsystem
     * is initialized.
     * @see #addGUIInitializedListener
     * @param l the listener to remove
     */
    static public void removeGUIInitializedListener(GUIInitializedListener l) {
        guiInitializedListener = 
	    GUIInitializedMulticaster.remove(guiInitializedListener,l);
    }

    /**
     * Adds the specified listener to be notified when a top level window
     * is created or destroyed.
     * @see #removeTopLevelWindowListener
     * @param l the listener to add
     */
    static public void addTopLevelWindowListener(TopLevelWindowListener l) {
        topLevelWindowListener = 
	    TopLevelWindowMulticaster.add(topLevelWindowListener,l);
    }

    /**
     * Removed the specified listener to be notified when a top level window
     * is created or destroyed.
     * @see #addTopLevelWindowListener
     * @param l the listener to remove
     */
    static public void removeTopLevelWindowListener(TopLevelWindowListener l) {
        topLevelWindowListener = 
	    TopLevelWindowMulticaster.remove(topLevelWindowListener,l);
    }

    /**
     * Return the last recorded position of the mouse in screen coordinates.
     * @return the last recorded position of the mouse in screen coordinates
     */
    static public Point getCurrentMousePosition() {
	    return currentMousePosition;
    }

    /**
     * Return the list of top level Windows in use in the Java Virtual Machine.
     * @return an array of top level Windows in use in the Java Virtual Machine
     */
    static public Window[] getTopLevelWindows() {

        // Because this method is static, do not make it synchronized because
        // it can lock the whole class.  Instead, just lock what needs to be
        // locked.
        //
        synchronized (topLevelWindows) {
            int count = topLevelWindows.size();
            if (count > 0) {
                Window[] w = new Window[count];
                for (int i = 0; i < count; i++) {
                    w[i] = (Window)topLevelWindows.elementAt(i);
                }
                return w;
            } else {
                return new Window[0];
            }
        }
    }

    /**
     * Return the top level Window that currently has keyboard focus.
     * @return the top level Window that currently has keyboard focus
     */
    static public Window getTopLevelWindowWithFocus() {
        return topLevelWindowWithFocus;
    }
}

/**
 * Handle all Component events in a separate thread.  The reason for 
 * this is that WindowEvent<s tend to be used to do lots of processing
 * on the Window hierarchy.  As a result, it can frequently result
 * in deadlock situations.
 */
class ComponentEvtDispatchThread extends Thread {
    public ComponentEvtDispatchThread(String name) {
        super(name);
    }
    public void run() {
        ComponentEvent ce = null;
        while (true) {
	        synchronized(EventQueueMonitor.componentEventQueueLock) {
                while (EventQueueMonitor.componentEventQueue == null) {
	                try {
		                EventQueueMonitor.componentEventQueueLock.wait();
		            } catch (InterruptedException e) {
		        }
		    }
    	    ce = (ComponentEvent)
    	    EventQueueMonitor.componentEventQueue.event;
            EventQueueMonitor.componentEventQueue =
	        EventQueueMonitor.componentEventQueue.next;
        }
	    switch (ce.getID()) {
            case MouseEvent.MOUSE_MOVED:
            case MouseEvent.MOUSE_DRAGGED:
		        EventQueueMonitor.updateCurrentMousePosition((MouseEvent) ce);
		        break;
            case WindowEvent.WINDOW_ACTIVATED:
	            EventQueueMonitor.maybeNotifyAssistiveTechnologies();
    	        EventQueueMonitor.topLevelWindowWithFocus = ((WindowEvent) ce).getWindow();

	    default:
		break;
            }
        }
    }
}

/**
 * EventQueueMonitorItem is the basic type that handles the
 * queue for queueComponentEvent and the ComponentEvtDispatchThread.
 */
class EventQueueMonitorItem {
    AWTEvent event;
    EventQueueMonitorItem next;

    EventQueueMonitorItem(AWTEvent evt) {
        event = evt;
	    next = null;
    }
}




