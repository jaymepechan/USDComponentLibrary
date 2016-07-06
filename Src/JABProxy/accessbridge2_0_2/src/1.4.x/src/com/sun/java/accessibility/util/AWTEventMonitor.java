/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * %W% %E%
 */

package com.sun.java.accessibility.util;

import java.util.*;
import java.awt.*;
import java.awt.event.*;
import javax.accessibility.*;
import javax.swing.*;
import javax.swing.event.*;

/**
 * <P>The AWTEventMonitor implements a suite of listeners that are
 * conditionally installed on every AWT component instance in the Java 
 * Virtual Machine.  The events captured by these listeners are made 
 * available through a unified set of listeners supported by AWTEventMonitor.
 * With this, all the individual events on each of the AWT component
 * instances are funnelled into one set of listeners broken down by category 
 * (see <a href="com.sun.java.accessibility.util.EventID.html">EventID</a>
 * for the categories).
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
 * @version     %I% %G% %U%
 * @author      Willie Walker
 * @author      Peter Korn
 */

public class AWTEventMonitor {

    static private boolean runningOnJDK1_4 = false;

    /**
     * The current component with keyboard focus.
     * @see #getComponentWithFocus
     */
    static protected Component componentWithFocus = null;

    // Low-level listeners
    /** 
     * The current list of registered ComponentListener classes. 
     * @see #addComponentListener
     * @see #removeComponentListener
     */
    static protected ComponentListener     componentListener     = null;

    /** 
     * The current list of registered ContainerListener classes. 
     * @see #addContainerListener
     * @see #removeContainerListener
     */
    static protected ContainerListener     containerListener     = null;

    /** 
     * The current list of registered FocusListener classes. 
     * @see #addFocusListener
     * @see #removeFocusListener
     */
    static protected FocusListener         focusListener         = null;

    /** 
     * The current list of registered KeyListener classes. 
     * @see #addKeyListener
     * @see #removeKeyListener
     */
    static protected KeyListener           keyListener           = null;

    /** 
     * The current list of registered MouseListener classes. 
     * @see #addMouseListener
     * @see #removeMouseListener
     */
    static protected MouseListener         mouseListener         = null;

    /** 
     * The current list of registered MouseMotionListener classes. 
     * @see #addMouseMotionListener
     * @see #removeMouseMotionListener
     */
    static protected MouseMotionListener   mouseMotionListener   = null;

    /** 
     * The current list of registered WindowListener classes. 
     * @see #addWindowListener
     * @see #removeWindowListener
     */
    static protected WindowListener        windowListener        = null;


    // Semantic listeners
    /** 
     * The current list of registered ActionListener classes. 
     * @see #addActionListener
     * @see #removeActionListener
     */
    static protected ActionListener        actionListener        = null;

    /** 
     * The current list of registered AdjustmentListener classes. 
     * @see #addAdjustmentListener
     * @see #removeAdjustmentListener
     */
    static protected AdjustmentListener    adjustmentListener    = null;

    /** 
     * The current list of registered ItemListener classes. 
     * @see #addItemListener
     * @see #removeItemListener
     */
    static protected ItemListener          itemListener          = null;

    /** 
     * The current list of registered TextListener classes. 
     * @see #addTextListener
     * @see #removeTextListener
     */
    static protected TextListener          textListener          = null;


    /**
     * The actual listener that is installed on the component instances.
     * This listener calls the other registered listeners when an event 
     * occurs.  By doing things this way, the actual number of listeners
     * installed on a component instance is drastically reduced.
     * @see #componentListener
     * @see #containerListener
     * @see #focusListener
     * @see #keyListener
     * @see #mouseListener
     * @see #mouseMotionListener
     * @see #windowListener
     * @see #actionListener
     * @see #adjustmentListener
     * @see #itemListener
     * @see #textListener
     */
    static protected AWTEventsListener awtListener = new AWTEventsListener();

    /**
     * Returns the component that currently has keyboard focus.  The return 
     * value can be null.
     */
    static public Component getComponentWithFocus() {
        return componentWithFocus;
    }

    /**
     * Adds the specified listener to receive all Component events on each
     * component instance in the Java Virtual Machine as they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeComponentListener
     * @param l the listener to add
     */
    static public void addComponentListener(ComponentListener l) {
        if (componentListener == null) {
            awtListener.installListeners(EventID.COMPONENT);
        }
        componentListener = AWTEventMulticaster.add(componentListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Component 
     * events when they occur.
     * @see #addComponentListener
     * @param l the listener to remove
     */
    static public void removeComponentListener(ComponentListener l) {
        componentListener = AWTEventMulticaster.remove(componentListener,l);
        if (componentListener == null) {
            awtListener.removeListeners(EventID.COMPONENT);
        }
    }

    /**
     * Adds the specified listener to receive all Container events on each
     * component instance in the Java Virtual Machine as they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeContainerListener
     * @param l the listener to add
     */
    static public void addContainerListener(ContainerListener l) {
        containerListener = AWTEventMulticaster.add(containerListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Container 
     * events when they occur.
     * @see #addContainerListener
     * @param l the listener to remove
     */
    static public void removeContainerListener(ContainerListener l) {
        containerListener = AWTEventMulticaster.remove(containerListener,l);
    }

    /**
     * Adds the specified listener to receive all Focus events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeFocusListener
     * @param l the listener to add
     */
    static public void addFocusListener(FocusListener l) {
        focusListener = AWTEventMulticaster.add(focusListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Focus events
     * when they occur.
     * @see #addFocusListener
     * @param l the listener to remove
     */
    static public void removeFocusListener(FocusListener l) {
        focusListener = AWTEventMulticaster.remove(focusListener,l);
    }

    /**
     * Adds the specified listener to receive all Key events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeKeyListener
     * @param l the listener to add
     */
    static public void addKeyListener(KeyListener l) {
        if (keyListener == null) {
            awtListener.installListeners(EventID.KEY);
        }
        keyListener = AWTEventMulticaster.add(keyListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Key events
     * when they occur.
     * @see #addKeyListener
     * @param l the listener to remove
     */
    static public void removeKeyListener(KeyListener l) {
        keyListener = AWTEventMulticaster.remove(keyListener,l);
        if (keyListener == null)  {
            awtListener.removeListeners(EventID.KEY);
        }
    }

    /**
     * Adds the specified listener to receive all Mouse events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeMouseListener
     * @param l the listener to add
     */
    static public void addMouseListener(MouseListener l) {
        if (mouseListener == null) {
            awtListener.installListeners(EventID.MOUSE);
        }
        mouseListener = AWTEventMulticaster.add(mouseListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Mouse events
     * when they occur.
     * @see #addMouseListener
     * @param l the listener to remove
     */
    static public void removeMouseListener(MouseListener l) {
        mouseListener = AWTEventMulticaster.remove(mouseListener,l);
        if (mouseListener == null) {
            awtListener.removeListeners(EventID.MOUSE);
        }
    }

    /**
     * Adds the specified listener to receive all MouseMotion events on
     * each component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeMouseMotionListener
     * @param l the listener to add
     */
    static public void addMouseMotionListener(MouseMotionListener l) {
        if (mouseMotionListener == null) {
            awtListener.installListeners(EventID.MOTION);
        }
        mouseMotionListener = AWTEventMulticaster.add(mouseMotionListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Motion events
     * when they occur.
     * @see #addMouseMotionListener
     * @param l the listener to remove
     */
    static public void removeMouseMotionListener(MouseMotionListener l) {
        mouseMotionListener = 
                AWTEventMulticaster.remove(mouseMotionListener,l);
        if (mouseMotionListener == null) {
            awtListener.removeListeners(EventID.MOTION);
        }
    }

    /**
     * Adds the specified listener to receive all Window events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeWindowListener
     * @param l the listener to add
     */
    static public void addWindowListener(WindowListener l) {
        if (windowListener == null) {
            awtListener.installListeners(EventID.WINDOW);
        }
        windowListener = AWTEventMulticaster.add(windowListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Window events
     * when they occur.
     * @see #addWindowListener
     * @param l the listener to remove
     */
    static public void removeWindowListener(WindowListener l) {
        windowListener = AWTEventMulticaster.remove(windowListener,l);
        if (windowListener == null) {
            awtListener.removeListeners(EventID.WINDOW);
        }
    }

    /**
     * Adds the specified listener to receive all Action events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeActionListener
     * @param l the listener to add
     */
    static public void addActionListener(ActionListener l) {
        if (actionListener == null) {
            awtListener.installListeners(EventID.ACTION);
        }
        actionListener = AWTEventMulticaster.add(actionListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Action events
     * when they occur.
     * @see #addActionListener
     * @param l the listener to remove
     */
    static public void removeActionListener(ActionListener l) {
        actionListener = AWTEventMulticaster.remove(actionListener,l);
        if (actionListener == null) {
            awtListener.removeListeners(EventID.ACTION);
        }
    }

    /**
     * Adds the specified listener to receive all Adjustment events on
     * each component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeAdjustmentListener
     * @param l the listener to add
     */
    static public void addAdjustmentListener(AdjustmentListener l) {
        if (adjustmentListener == null) {
            awtListener.installListeners(EventID.ADJUSTMENT);
        }
        adjustmentListener = AWTEventMulticaster.add(adjustmentListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Adjustment 
     * events when they occur.
     * @see #addAdjustmentListener
     * @param l the listener to remove
     */
    static public void removeAdjustmentListener(AdjustmentListener l) {
        adjustmentListener = AWTEventMulticaster.remove(adjustmentListener,l);
        if (adjustmentListener == null) {
            awtListener.removeListeners(EventID.ADJUSTMENT);
        }
    }

    /**
     * Adds the specified listener to receive all Item events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeItemListener
     * @param l the listener to add
     */
    static public void addItemListener(ItemListener l) {
        if (itemListener == null) {
            awtListener.installListeners(EventID.ITEM);
        }
        itemListener = AWTEventMulticaster.add(itemListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Item events
     * when they occur.
     * @see #addItemListener
     * @param l the listener to remove
     */
    static public void removeItemListener(ItemListener l) {
        itemListener = AWTEventMulticaster.remove(itemListener,l);
        if (itemListener == null) {
            awtListener.removeListeners(EventID.ITEM);
        }
    }

    /**
     * Adds the specified listener to receive all Text events on each 
     * component instance in the Java Virtual Machine when they occur.
     * <P>Note: this listener is automatically added to all component 
     * instances created after this method is called.  In addition, it
     * is only added to component instances that support this listener type.
     * @see #removeTextListener
     * @param l the listener to add
     */
    static public void addTextListener(TextListener l) {
        if (textListener == null) {
            awtListener.installListeners(EventID.TEXT);
        }
        textListener = AWTEventMulticaster.add(textListener,l);
    }

    /**
     * Removes the specified listener so it no longer receives Text events
     * when they occur.
     * @see #addTextListener
     * @param l the listener to remove
     */
    static public void removeTextListener(TextListener l) {
        textListener = AWTEventMulticaster.remove(textListener,l);
        if (textListener == null) {
            awtListener.removeListeners(EventID.TEXT);
        }
    }


    /**
     * AWTEventsListener is the class that does all the work for AWTEventMonitor.
     * It is not intended for use by any other class except AWTEventMonitor.
     *
     */

    static class AWTEventsListener implements TopLevelWindowListener,
	ActionListener, AdjustmentListener, ComponentListener, 
	ContainerListener, FocusListener, ItemListener, KeyListener,
	MouseListener, MouseMotionListener, TextListener, WindowListener,
        ChangeListener {

	/**
	 * internal variables for Action introspection
	 */
	private java.lang.Class actionListeners[];
	private java.lang.reflect.Method removeActionMethod;
	private java.lang.reflect.Method addActionMethod;
	private java.lang.Object actionArgs[];

	/**
	 * internal variables for Item introspection
	 */
	private java.lang.Class itemListeners[];
	private java.lang.reflect.Method removeItemMethod;
	private java.lang.reflect.Method addItemMethod;
	private java.lang.Object itemArgs[];

	/**
	 * internal variables for Text introspection
	 */
	private java.lang.Class textListeners[];
	private java.lang.reflect.Method removeTextMethod;
	private java.lang.reflect.Method addTextMethod;
	private java.lang.Object textArgs[];

	/**
	 * internal variables for Window introspection
	 */
	private java.lang.Class windowListeners[];
	private java.lang.reflect.Method removeWindowMethod;
	private java.lang.reflect.Method addWindowMethod;
	private java.lang.Object windowArgs[];

	/**
	 * Create a new instance of this class and install it on each component
	 * instance in the virtual machine that supports any of the currently
	 * registered listeners in AWTEventMonitor.  Also registers itself
	 * as a TopLevelWindowListener with EventQueueMonitor so it can
	 * automatically add new listeners to new components.
	 * @see EventQueueMonitor
	 * @see AWTEventMonitor
	 */
	public AWTEventsListener() {
	    String version = System.getProperty("java.version");
	    if (version != null) {
		runningOnJDK1_4 = (version.compareTo("1.4") >= 0);
	    }
	    initializeIntrospection();
	    installListeners();
	    if (runningOnJDK1_4) {
		MenuSelectionManager.defaultManager().addChangeListener(this);
	    }
	    EventQueueMonitor.addTopLevelWindowListener(this);
	}

	/**
	 * Set up all of the variables needed for introspection
	 */
	private boolean initializeIntrospection() {
	    try {
		actionListeners = new java.lang.Class[1];
		actionArgs = new java.lang.Object[1];
		actionListeners[0] = Class.forName("java.awt.event.ActionListener");
		actionArgs[0] = this;
		
		itemListeners = new java.lang.Class[1];
		itemArgs = new java.lang.Object[1];
		itemListeners[0] = Class.forName("java.awt.event.ItemListener");
		itemArgs[0] = this;
		
		textListeners = new java.lang.Class[1];
		textArgs = new java.lang.Object[1];
		textListeners[0] = Class.forName("java.awt.event.TextListener");
		textArgs[0] = this;
		
		windowListeners = new java.lang.Class[1];
		windowArgs = new java.lang.Object[1];
		windowListeners[0] = Class.forName("java.awt.event.WindowListener");
		windowArgs[0] = this;
		
		return true;
	    } catch (ClassNotFoundException e) {
		System.out.println("EXCEPTION - Class 'java.awt.event.*' not in CLASSPATH");
		return false;
	    }
	}

	/**
	 * Installs all currently registered listeners on all components based 
	 * upon the current topLevelWindows cached by EventQueueMonitor.
	 * @see EventQueueMonitor
	 * @see AWTEventMonitor
	 */
	protected void installListeners() {
	    Window topLevelWindows[] = EventQueueMonitor.getTopLevelWindows();
	    if (topLevelWindows != null) {
		for (int i = 0; i < topLevelWindows.length; i++) {
		    installListeners(topLevelWindows[i]);
		}
	    }
	}

	/**
	 * Installs listeners for the given event ID on all components based 
	 * upon the current topLevelWindows cached by EventQueueMonitor.
	 * @see EventID
	 * @param eventID the event ID
	 */
	protected void installListeners(int eventID) {
	    Window topLevelWindows[] = EventQueueMonitor.getTopLevelWindows();
	    if (topLevelWindows != null) {
		for (int i = 0; i < topLevelWindows.length; i++) {
		    installListeners(topLevelWindows[i], eventID);
		}
	    }
	}

	/**
	 * Installs all currently registered listeners to just the component.
	 * @param c the component to add listeners to
	 */
	protected void installListeners(Component c) {    

	    // Container and focus listeners are always installed for our own use.
	    //
	    installListeners(c,EventID.CONTAINER);
	    installListeners(c,EventID.FOCUS);

	    // conditionally install low-level listeners
	    //
	    if (AWTEventMonitor.componentListener != null) {
		installListeners(c,EventID.COMPONENT);
	    }
	    if (AWTEventMonitor.keyListener != null) {
		installListeners(c,EventID.KEY);
	    }
	    if (AWTEventMonitor.mouseListener != null) {
		installListeners(c,EventID.MOUSE);
	    }
	    if (AWTEventMonitor.mouseMotionListener != null) {
		installListeners(c,EventID.MOTION);
	    }
	    if (AWTEventMonitor.windowListener != null) {
		installListeners(c,EventID.WINDOW);
	    }

	    // conditionally install Semantic listeners
	    //
	    if (AWTEventMonitor.actionListener != null) {
		installListeners(c,EventID.ACTION);
	    }
	    if (AWTEventMonitor.adjustmentListener != null) {
		installListeners(c,EventID.ADJUSTMENT);
	    }
	    if (AWTEventMonitor.itemListener != null) {
		installListeners(c,EventID.ITEM);
	    }
	    if (AWTEventMonitor.textListener != null) {
		installListeners(c,EventID.TEXT);
	    }
	}

	public void stateChanged(ChangeEvent e) {
	    processFocusGained();
	}

	private void processFocusGained() {
	    Component focusOwner = KeyboardFocusManager.getCurrentKeyboardFocusManager().getFocusOwner();
	    if (focusOwner == null) {
		return;
	    }
	    MenuSelectionManager.defaultManager().removeChangeListener(this);
	    MenuSelectionManager.defaultManager().addChangeListener(this);

	    // Only menus and popup selections are handled by the JRootPane.
	    if (focusOwner instanceof JRootPane) {
		MenuElement [] path = 
		    MenuSelectionManager.defaultManager().getSelectedPath();
		if (path.length > 1) {
		    Component penult = path[path.length-2].getComponent();
		    Component last = path[path.length-1].getComponent();
		    
		    if (last instanceof JPopupMenu ||
			last instanceof JMenu) {
			// This is a popup with nothing in the popup
			// selected. The menu itself is selected.
			componentWithFocus = last;
		    } else if (penult instanceof JPopupMenu) {
			// This is a popup with an item selected
			componentWithFocus = penult;
		    }
		}
	    } else {
		// The focus owner has the selection.
		componentWithFocus = focusOwner;
	    }
	}

	/**
	 * Installs the given listener on the component and any of its children.
	 * As a precaution, it always attempts to remove itself as a listener
	 * first so it's always guaranteed to have installed itself just once.
	 * @param c the component to add listeners to
	 * @param eventID the eventID to add listeners for
	 * @see EventID
	 */
	protected void installListeners(Component c, int eventID) {
	    
	    // install the appropriate listener hook into this component
	    //
	    switch (eventID) {

	    case EventID.ACTION:        
		try {
		    removeActionMethod = c.getClass().getMethod(
			"removeActionListener", actionListeners);
		    addActionMethod = c.getClass().getMethod(
			"addActionListener", actionListeners);
		    try {
			removeActionMethod.invoke(c, actionArgs);
			addActionMethod.invoke(c, actionArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    case EventID.ADJUSTMENT:    
		if (c instanceof Adjustable) {
		    ((Adjustable) c).removeAdjustmentListener(this);
		    ((Adjustable) c).addAdjustmentListener(this);
		}
		break;

	    case EventID.COMPONENT:        
		c.removeComponentListener(this);
		c.addComponentListener(this);
		break;

	    case EventID.CONTAINER:
		if (c instanceof Container) {
		    ((Container) c).removeContainerListener(this);
		    ((Container) c).addContainerListener(this);
		}
		break;

	    case EventID.FOCUS:
		c.removeFocusListener(this);
		c.addFocusListener(this);
		
		if (runningOnJDK1_4) {
		    processFocusGained();
		    
		} else {	// not runningOnJDK1_4
		    if ((c != componentWithFocus) && c.hasFocus()) {
			componentWithFocus = c;
		    }
		}
		break;

	    case EventID.ITEM:
		try {
		    removeItemMethod = c.getClass().getMethod(
			"removeItemListener", itemListeners);
		    addItemMethod = c.getClass().getMethod(
			"addItemListener", itemListeners);
		    try {
			removeItemMethod.invoke(c, itemArgs);
			addItemMethod.invoke(c, itemArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		// [PK] CheckboxMenuItem isn't a component but it does 
		// implement Interface ItemSelectable!!
		// if (c instanceof CheckboxMenuItem) {
		//     ((CheckboxMenuItem) c).removeItemListener(this);
		//     ((CheckboxMenuItem) c).addItemListener(this);
		break;

	    case EventID.KEY:
		c.removeKeyListener(this);
		c.addKeyListener(this);
		break;

	    case EventID.MOUSE:
		c.removeMouseListener(this);
		c.addMouseListener(this);
		break;

	    case EventID.MOTION:
		c.removeMouseMotionListener(this);
		c.addMouseMotionListener(this);
		break;

	    case EventID.TEXT:
		try {
		    removeTextMethod = c.getClass().getMethod(
			"removeTextListener", textListeners);
		    addTextMethod = c.getClass().getMethod(
			"addTextListener", textListeners);
		    try {
			removeTextMethod.invoke(c, textArgs);
			addTextMethod.invoke(c, textArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    case EventID.WINDOW:
		try {
		    removeWindowMethod = c.getClass().getMethod(
			"removeWindowListener", windowListeners);
		    addWindowMethod = c.getClass().getMethod(
			"addWindowListener", windowListeners);
		    try {
			removeWindowMethod.invoke(c, windowArgs);
			addWindowMethod.invoke(c, windowArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    // Don't bother recursing the children if this isn't going to
	    // accomplish anything.
	    //        
	    default:
		return;
	    }
	     
	    // if this component is a container, recurse through children
	    //
	    if (c instanceof Container) {
		int count = ((Container) c).getComponentCount();
		for (int i = 0; i < count; i++) {
		    installListeners(((Container) c).getComponent(i), eventID);
		}
	    }
	}
		
	/**
	 * Removes all listeners for the given event ID on all components based
	 * upon the topLevelWindows cached by EventQueueMonitor.
	 * @param eventID the event ID
	 * @see EventID
	 */
	protected void removeListeners(int eventID) {
	    Window topLevelWindows[] = EventQueueMonitor.getTopLevelWindows();
	    if (topLevelWindows != null) {
		for (int i = 0; i < topLevelWindows.length; i++) {
		    removeListeners((Window)topLevelWindows[i], eventID);
		}
	    }
	}

	/**
	 * Removes all listeners for the given component and all its children.
	 * @param c the component
	 */
	protected void removeListeners(Component c) {

	    // conditionally remove low-level listeners
	    //
	    if (AWTEventMonitor.componentListener != null) {
		removeListeners(c,EventID.COMPONENT);
	    }
	    if (AWTEventMonitor.keyListener != null) {
		removeListeners(c,EventID.KEY);
	    }
	    if (AWTEventMonitor.mouseListener != null) {
		removeListeners(c,EventID.MOUSE);
	    }
	    if (AWTEventMonitor.mouseMotionListener != null) {
		removeListeners(c,EventID.MOTION);
	    }
	    if (AWTEventMonitor.windowListener != null) {
		removeListeners(c,EventID.WINDOW);
	    }

	    // Remove low-level listeners
	    //
	    if (AWTEventMonitor.actionListener != null) {
		removeListeners(c,EventID.ACTION);
	    }
	    if (AWTEventMonitor.adjustmentListener != null) {
		removeListeners(c,EventID.ADJUSTMENT);
	    }
	    if (AWTEventMonitor.itemListener != null) {
		removeListeners(c,EventID.ITEM);
	    }
	    if (AWTEventMonitor.textListener != null) {
		removeListeners(c,EventID.TEXT);
	    }
	}

	/**
	 * Removes all listeners for the event ID from the component and all
	 * of its children.
	 * @param c the component to remove listeners from
	 * @see EventID
	 */
	protected void removeListeners(Component c, int eventID) {
	    
	    // remove the appropriate listener hook into this component
	    //
	    switch (eventID) {

	    case EventID.ACTION:        
		try {
		    removeActionMethod = c.getClass().getMethod(
			"removeActionListener",
			actionListeners);
		    try {
			removeActionMethod.invoke(c, actionArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    case EventID.ADJUSTMENT:    
		if (c instanceof Adjustable) {
		    ((Adjustable) c).removeAdjustmentListener(this);
		}
		break;

	    case EventID.COMPONENT:        
		c.removeComponentListener(this);
		break;

	    // Never remove these because we're always interested in them
	    // for our own use.
	    //case EventID.CONTAINER:
	    //    if (c instanceof Container) {
	    //        ((Container) c).removeContainerListener(this);
	    //    }
	    //    break;
	    //
	    //case EventID.FOCUS:
	    //    c.removeFocusListener(this);
	    //    break;

	    case EventID.ITEM:
		try {
		    removeItemMethod = c.getClass().getMethod(
			"removeItemListener", itemListeners);
		    try {
			removeItemMethod.invoke(c, itemArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		// [PK] CheckboxMenuItem isn't a component but it does 
		// implement Interface ItemSelectable!!
		// if (c instanceof CheckboxMenuItem) {
		//     ((CheckboxMenuItem) c).removeItemListener(this);
		break;

	    case EventID.KEY:
		c.removeKeyListener(this);
		break;

	    case EventID.MOUSE:
		c.removeMouseListener(this);
		break;

	    case EventID.MOTION:
		c.removeMouseMotionListener(this);
		break;

	    case EventID.TEXT:
		try {
		    removeTextMethod = c.getClass().getMethod(
			"removeTextListener", textListeners);
		    try {
			removeTextMethod.invoke(c, textArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    case EventID.WINDOW:
		try {
		    removeWindowMethod = c.getClass().getMethod(
			"removeWindowListener", windowListeners);
		    try {
			removeWindowMethod.invoke(c, windowArgs);
		    } catch (java.lang.reflect.InvocationTargetException e) {
			System.out.println("Exception: " + e.toString());
		    } catch (IllegalAccessException e) {
			System.out.println("Exception: " + e.toString());
		    }
		} catch (NoSuchMethodException e) {
		    // System.out.println("Exception: " + e.toString());
		} catch (SecurityException e) {
		    System.out.println("Exception: " + e.toString());
		}
		break;

	    default:
		return;
	    }
	     
	    if (c instanceof Container) {
		int count = ((Container) c).getComponentCount();
		for (int i = 0; i < count; i++) {
		    removeListeners(((Container) c).getComponent(i), eventID);
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
	    installListeners(w);
	}

	/**
	 * Called when top level window is destroyed.
	 * @see EventQueueMonitor
	 * @see EventQueueMonitor#addTopLevelWindowListener
	 */
	public void topLevelWindowDestroyed(Window w) {
	}

	/* ActionListener Methods ***************************************/

	/**
	 * Called when an action is performed.
	 * @see AWTEventMonitor#addActionListener
	 */
	public void actionPerformed(ActionEvent e) {
	    if (AWTEventMonitor.actionListener != null) {
		AWTEventMonitor.actionListener.actionPerformed(e);
	    }
	}

	/* AdjustmentListener Methods ***********************************/

	/**
	 * Called when an adjustment is made.
	 * @see AWTEventMonitor#addAdjustmentListener
	 */
	public void adjustmentValueChanged(AdjustmentEvent e) {
	    if (AWTEventMonitor.adjustmentListener != null) {
		AWTEventMonitor.adjustmentListener.adjustmentValueChanged(e);
	    }
	}

	/* ComponentListener Methods ************************************/

	/**
	 * Called when a component is hidden.
	 * @see AWTEventMonitor#addComponentListener
	 */
	public void componentHidden(ComponentEvent e) {
	    if (AWTEventMonitor.componentListener != null) {
		AWTEventMonitor.componentListener.componentHidden(e);
	    }
	}

	/**
	 * Called when a component is moved.
	 * @see AWTEventMonitor#addComponentListener
	 */
	public void componentMoved(ComponentEvent e) {
	    if (AWTEventMonitor.componentListener != null) {
		AWTEventMonitor.componentListener.componentMoved(e);
	    }
	}

	/**
	 * Called when a component is resized.
	 * @see AWTEventMonitor#addComponentListener
	 */
	public void componentResized(ComponentEvent e) {
	    if (AWTEventMonitor.componentListener != null) {
		AWTEventMonitor.componentListener.componentResized(e);
	    }
	}

	/**
	 * Called when a component is shown.
	 * @see AWTEventMonitor#addComponentListener
	 */
	public void componentShown(ComponentEvent e) {
	    if (AWTEventMonitor.componentListener != null) {
		AWTEventMonitor.componentListener.componentShown(e);
	    }
	}

	/* ContainerListener Methods ************************************/

	/**
	 * Called when a component is added to a container.
	 * @see AWTEventMonitor#addContainerListener
	 */
	public void componentAdded(ContainerEvent e) {
	    installListeners((Component) (e.getChild()));
	    if (AWTEventMonitor.containerListener != null) {
		AWTEventMonitor.containerListener.componentAdded(e);
	    }
	}

	/**
	 * Called when a component is removed from a container.
	 * @see AWTEventMonitor#addContainerListener
	 */
	public void componentRemoved(ContainerEvent e) {
	    removeListeners((Component) (e.getChild()));
	    if (AWTEventMonitor.containerListener != null) {
		AWTEventMonitor.containerListener.componentRemoved(e);
	    }
	}

	/* FocusListener Methods ****************************************/

	/**
	 * Called when a component gains keyboard focus.
	 * @see AWTEventMonitor#addFocusListener
	 */
	public void focusGained(FocusEvent e) {
	    AWTEventMonitor.componentWithFocus = (Component) e.getSource();
	    if (AWTEventMonitor.focusListener != null) {
		AWTEventMonitor.focusListener.focusGained(e);
	    }
	}

	/**
	 * Called when a component loses keyboard focus.
	 * @see AWTEventMonitor#addFocusListener
	 */
	public void focusLost(FocusEvent e) {
	    AWTEventMonitor.componentWithFocus = null;
	    if (AWTEventMonitor.focusListener != null) {
		AWTEventMonitor.focusListener.focusLost(e);
	    }
	}

	/* ItemListener Methods *****************************************/

	/**
	 * Called when an item's state changes.
	 * @see AWTEventMonitor#addItemListener
	 */
	public void itemStateChanged(ItemEvent e) {
	    if (AWTEventMonitor.itemListener != null) {
		AWTEventMonitor.itemListener.itemStateChanged(e);
	    }
	}

	/* KeyListener Methods ******************************************/

	/**
	 * Called when a key is pressed.
	 * @see AWTEventMonitor#addKeyListener
	 */
	public void keyPressed(KeyEvent e) {
	    if (AWTEventMonitor.keyListener != null) {
		AWTEventMonitor.keyListener.keyPressed(e);
	    }
	}

	/**
	 * Called when a key is typed.
	 * @see AWTEventMonitor#addKeyListener
	 */
	public void keyReleased(KeyEvent e) {
	    if (AWTEventMonitor.keyListener != null) {
		AWTEventMonitor.keyListener.keyReleased(e); 
	    }
	}

	/**
	 * Called when a key is released.
	 * @see AWTEventMonitor#addKeyListener
	 */
	public void keyTyped(KeyEvent e) {
	    if (AWTEventMonitor.keyListener != null) {
		AWTEventMonitor.keyListener.keyTyped(e);
	    }
	}

	/* MouseListener Methods ****************************************/

	/**
	 * Called when the mouse is clicked.
	 * @see AWTEventMonitor#addMouseListener
	 */
	public void mouseClicked(MouseEvent e) {
	    if (AWTEventMonitor.mouseListener != null) {
		AWTEventMonitor.mouseListener.mouseClicked(e);
	    }
	}

	/**
	 * Called when the mouse enters a component.
	 * @see AWTEventMonitor#addMouseListener
	 */
	public void mouseEntered(MouseEvent e) {
	    if (AWTEventMonitor.mouseListener != null) {
		AWTEventMonitor.mouseListener.mouseEntered(e);
	    }
	}

	/**
	 * Called when the mouse leaves a component.
	 * @see AWTEventMonitor#addMouseListener
	 */
	public void mouseExited(MouseEvent e) {
	    if (AWTEventMonitor.mouseListener != null) {
		AWTEventMonitor.mouseListener.mouseExited(e);
	    }
	}

	/**
	 * Called when the mouse is pressed.
	 * @see AWTEventMonitor#addMouseListener
	 */
	public void mousePressed(MouseEvent e) {
	    if (AWTEventMonitor.mouseListener != null) {
		AWTEventMonitor.mouseListener.mousePressed(e);
	    }
	}

	/**
	 * Called when the mouse is released.
	 * @see AWTEventMonitor#addMouseListener
	 */
	public void mouseReleased(MouseEvent e) {
	    if (AWTEventMonitor.mouseListener != null) {
		AWTEventMonitor.mouseListener.mouseReleased(e);
	    }
	}

	/* MouseMotionListener Methods **********************************/

	/**
	 * Called when the mouse is dragged.
	 * @see AWTEventMonitor#addMouseMotionListener
	 */
	public void mouseDragged(MouseEvent e) {
	    if (AWTEventMonitor.mouseMotionListener != null) {
		AWTEventMonitor.mouseMotionListener.mouseDragged(e);
	    }
	}

	/**
	 * Called when the mouse is moved.
	 * @see AWTEventMonitor#addMouseMotionListener
	 */
	public void mouseMoved(MouseEvent e) {
	    if (AWTEventMonitor.mouseMotionListener != null) {
		AWTEventMonitor.mouseMotionListener.mouseMoved(e);
	    }
	}

	/* TextListener Methods *****************************************/

	/**
	 * Called when a component's text value changed.
	 * @see AWTEventMonitor#addTextListener
	 */
	public void textValueChanged(TextEvent e) {
	    if (AWTEventMonitor.textListener != null) {
		AWTEventMonitor.textListener.textValueChanged(e);
	    }
	}

	/* WindowListener Methods ***************************************/

	/**
	 * Called when a window is opened.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowOpened(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowOpened(e);
	    }
	}

	/**
	 * Called when a window is in the process of closing.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowClosing(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowClosing(e);
	    }
	}

	/**
	 * Called when a window is closed.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowClosed(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowClosed(e);
	    }
	}

	/**
	 * Called when a window is iconified.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowIconified(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowIconified(e);
	    }
	}

	/**
	 * Called when a window is deiconified.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowDeiconified(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowDeiconified(e);
	    }
	}

	/**
	 * Called when a window is activated.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowActivated(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowActivated(e);
	    }
	}

	/**
	 * Called when a window is deactivated.
	 * @see AWTEventMonitor#addWindowListener
	 */
	public void windowDeactivated(WindowEvent e) {
	    if (AWTEventMonitor.windowListener != null) {
		AWTEventMonitor.windowListener.windowDeactivated(e);
	    }
	}
    }
}
