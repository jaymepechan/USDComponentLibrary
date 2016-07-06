/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Translator.java	1.29 02/01/17
 */

package com.sun.java.accessibility.util;

import java.lang.*;
import java.beans.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;
import java.awt.image.*;
// Do not import Swing classes.  This module is intended to work
// with both Swing and AWT.
// import javax.swing.*;
import javax.accessibility.*;

/**
 * <p>The Translator class provides a translation to interface Accessible 
 * for objects that do not implement interface Accessible.  Assistive
 * technologies can use the 'getAccessible' class method of Translator to
 * obtain an object that implements interface Accessible.  If the object
 * passed in already implements interface Accessible, getAccessible merely
 * returns the object. 
 *
 * <p>An example of how an assistive technology might use the Translator 
 * class is as follows: 
 *
 * <PRE>
 *    Accessible accessible = Translator.getAccessible(someObj);
 *    // obtain information from the 'accessible' object.
 * </PRE>
 *
 * <P>NOTE:  This is proof-of-concept code and is missing many things.  It
 * is also an undesirable way to implement accessibility features for a
 * Toolkit.  Instead of relying upon this code, the Toolkit's Components
 * should implement interface Accessible directly.  This is also a preliminary 
 * draft.  The methods and name may change in future beta releases.
 *
 * @version     1.29 01/17/02 16:07:02
 * @author      Willie Walker
 */
public class Translator extends AccessibleContext 
	implements Accessible, AccessibleComponent {

    /** The source object needing translating. */
    protected Object source;

    /**
     * Find a translator for this class.  If one doesn't exist for this
     * class explicitly, try its superclass and so on.
     */
    protected static Class getTranslatorClass(Class c) {
        Class t = null;
        if (c == null) {
            return null;
        }
        try {
            t = Class.forName("com.sun.java.accessibility.util."
                              + c.getName() 
                              + "Translator");
            return t;
        } catch (Exception e) {
            return getTranslatorClass(c.getSuperclass());
        }
    }

    /** 
     * Obtain an object that implements interface Accessible.  If the object
     * passed in already implements interface Accessible, getAccessible merely
     * returns the object.
     */
    public static Accessible getAccessible(Object o) {
        Accessible a = null;

        if (o == null) {
            return null;
        }
        if (o instanceof Accessible) {
            a = (Accessible)o;
        } else {
            Class translatorClass = getTranslatorClass(o.getClass());
            if (translatorClass != null) {
                try {
                    Translator t = (Translator)translatorClass.newInstance();
                    t.setSource(o);
                    a = t;
                } catch (Exception e) {
                }
            }
        }
        if (a == null) {
            a = new Translator(o);
        }
        return a;
    }
 
    /**
     * Create a new Translator.  You must call the setSource method to
     * set the object to be translated after calling this constructor.
     */
    public Translator() {
    }
 
    /**
     * Create a new Translator with the source object o.
     * @param o the Component that does not implement interface Accessible
     */
    public Translator(Object o) {
        source = o;
    }
 
    /**
     * Get the source object of the Translator. 
     * @return the source object of the Translator
     */
    public Object getSource() {
        return source;
    }
 
    /**
     * Set the source object of the Translator. 
     * @param o the Component that does not implement interface Accessible
     */
    public void setSource(Object o) {
        source = o;
    }
 
    /**
     * Returns true if this object is the same as the one passed in.
     * @param c the Component to check against.
     * @return true if this is the same object.
     */
    public boolean equals(Object o) {
        return source.equals(o);
    }


// Accessible methods

    /**
     * Returns this object.
     */
    public AccessibleContext getAccessibleContext() {
	return this;
    }

// AccessibleContext methods

    /**
     * Get the accessible name of this object.
     * @return the localized name of the object -- can be null if this object
     * does not have a name
     */
    public String getAccessibleName() {
        if (source instanceof MenuItem) {
            return ((MenuItem) source).getLabel();
        } else if (source instanceof Component) {
            return ((Component) source).getName();
        } else {
            return null;
        }
    }

    /**
     * Set the name of this object.
     */
    public void setAccessibleName(String s) {
        if (source instanceof MenuItem) {
            ((MenuItem) source).setLabel(s);
        } else if (source instanceof Component) {
            ((Component) source).setName(s);
        }
    }

    /**
     * Get the accessible description of this object.
     * @return the description of the object -- can be null if this object does
     * not have a description
     */
    public String getAccessibleDescription() {
        return null;
    }

    /**
     * Set the accessible description of this object.
     * @param s the new localized description of the object
     */
    public void setAccessibleDescription(String s) {
    }

    /**
     * Get the role of this object.
     * @return an instance of AccessibleRole describing the role of the object
     */
    public AccessibleRole getAccessibleRole() {
	return AccessibleRole.UNKNOWN;
    }


    /**
     * Get the state of this object, given an already populated state.
     * This method is intended for use by subclasses so they don't have
     * to check for everything.
     * @return an instance of AccessibleStateSet containing the current 
     * state of the object
     */
    public AccessibleStateSet getAccessibleStateSet() {
	AccessibleStateSet states = new AccessibleStateSet();
	if (source instanceof Component) {
	    Component c = (Component) source;
            for (Container p = c.getParent(); p != null; p = p.getParent()) {
                if (p instanceof Window) {
		    if (((Window)p).getFocusOwner() == c) {
	                states.add(AccessibleState.FOCUSED);
		    }
	        }
	    }
        }
        if (isEnabled()) {
            states.add(AccessibleState.ENABLED);
        }
        if (isFocusTraversable()) {
            states.add(AccessibleState.FOCUSABLE);
        }
        if (source instanceof MenuItem) {
            states.add(AccessibleState.FOCUSABLE);
        }
        return states;
    }

    /**
     * Get the Accessible parent of this object.
     * @return the Accessible parent of this object -- can be null if this
     * object does not have an Accessible parent
     */
    public Accessible getAccessibleParent() {
	if (accessibleParent != null) {
	    return accessibleParent;
	} else if (source instanceof Component) {
            return getAccessible(((Component) source).getParent());
        } else {
            return null;
        }
    }

    /**
     * Get the index of this object in its accessible parent.
     *
     * @return -1 of this object does not have an accessible parent.
     * Otherwise, the index of the child in its accessible parent.
     */
    public int getAccessibleIndexInParent() {
	if (source instanceof Component) {
            Container parent = ((Component) source).getParent();
            if (parent != null) {
                Component ca[] = parent.getComponents();
                for (int i = 0; i < ca.length; i++) {
                    if (source.equals(ca[i])) {
		        return i;
		    }
		}
            }
        }
	return -1;
    }

    /**
     * Returns the number of accessible children in the object.
     * @return the number of accessible children in the object.
     */
    public int getAccessibleChildrenCount() { 
        if (source instanceof Container) {
	    Component[] children = ((Container) source).getComponents();
	    int count = 0;
	    for (int i = 0; i < children.length; i++) {
		Accessible a = getAccessible(children[i]);
		if (a != null) {
		    count++;
	        }
            }
  	    return count;
        } else {
            return 0;
        }
    }

    /**
     * Return the nth Accessible child of the object.
     * @param i zero-based index of child
     * @return the nth Accessible child of the object
     */
    public Accessible getAccessibleChild(int i) { 
        if (source instanceof Container) {
            Component[] children = ((Container) source).getComponents();
            int count = 0;

            for (int j = 0; j < children.length; j++) {
		Accessible a = getAccessible(children[j]);
                if (a != null) {
                    if (count == i) {
			AccessibleContext ac = a.getAccessibleContext();
			if (ac != null) {
			    ac.setAccessibleParent(this);
		        }
		        return a;
	 	    } else {
		        count++;
		    }
		}
            }
        }
        return null;
    }

    /**
     * Gets the locale of the component. If the component does not have a
     * locale, the locale of its parent is returned.
     * @return the Locale of the object.
     */
    public Locale getLocale() throws IllegalComponentStateException {
	if (source instanceof Component) {
	    return ((Component) source).getLocale();
        } else {
	    return null;
        }
    }

    /**
     * Add a PropertyChangeListener to the listener list.  The listener is
     * registered for all properties.
     */
    public void addPropertyChangeListener(PropertyChangeListener l) {
    }

    /**
     * Remove the PropertyChangeListener from the listener list.
     */
    public void removePropertyChangeListener(PropertyChangeListener l) {
    }

// AccessibleComponent methods

    /**
     * Get the background color of this object.
     * @return if supported, the background color of the object; 
     * otherwise, null
     *
     */
    public Color getBackground() {
        if (source instanceof Component) { // MenuComponent doesn't do background
            return ((Component) source).getBackground();
        } else {
            return null;
        }
    }

    /**
     * Set the background color of this object.
     * @param c the new Color for the background
     */
    public void setBackground(Color c) {
        if (source instanceof Component) { // MenuComponent doesn't do background
            ((Component) source).setBackground(c);
        }
    }

    /**
     * Get the foreground color of this object.
     * @return if supported, the foreground color of the object; otherwise, null
     */
    public Color getForeground() {
        if (source instanceof Component) { // MenuComponent doesn't do foreground
            return ((Component) source).getForeground();
        } else {
            return null;
        }
    }

    /**
     * Set the foreground color of this object.
     * @param c the new Color for the foreground
     */
    public void setForeground(Color c) {
        if (source instanceof Component) { // MenuComponent doesn't do foreground
            ((Component) source).setForeground(c);
        }
    }

    /**
     * Get the Cursor of this object.
     * @return if supported, the Cursor of the object; otherwise, null
     */
    public Cursor getCursor() {
        if (source instanceof Component) { // MenuComponent doesn't do cursor
            return ((Component) source).getCursor();
        } else {
            return null;
        }
    }

    /**
     * Set the Cursor of this object.
     * @param c the new Cursor for the object
     */
    public void setCursor(Cursor c) {
        if (source instanceof Component) { // MenuComponent doesn't do cursor
            ((Component) source).setCursor(c);
        }
    }

    /**
     * Get the Font of this object.
     * @return if supported, the Font for the object; otherwise, null
     */
    public Font getFont() {
        if (source instanceof Component) {
            return ((Component) source).getFont();
        } else if (source instanceof MenuComponent) {
            return ((MenuComponent) source).getFont();
        } else {
            return null;
        }
    }

    /**
     * Set the Font of this object.
     * @param f the new Font for the object
     */
    public void setFont(Font f) {
        if (source instanceof Component) {
            ((Component) source).setFont(f);
        } else if (source instanceof MenuComponent) {
            ((MenuComponent) source).setFont(f);
        }
    }

    /**
     * Get the FontMetrics of this object.
     * @param f the Font
     * @return if supported, the FontMetrics the object; otherwise, null
     * @see #getFont
     */
    public FontMetrics getFontMetrics(Font f) {
        if (source instanceof Component) {
            return ((Component) source).getFontMetrics(f);
        } else {
            return null;
        }
    }

    /**
     * Determine if the object is enabled.
     * @return true if object is enabled; otherwise, false
     */
    public boolean isEnabled() {
        if (source instanceof Component) {
            return ((Component) source).isEnabled();
        } else if (source instanceof MenuItem) {
            return ((MenuItem) source).isEnabled();
        } else {
            return true;
        }
    }

    /**
     * Set the enabled state of the object.
     * @param b if true, enables this object; otherwise, disables it 
     */
    public void setEnabled(boolean b) {
        if (source instanceof Component) {
            ((Component) source).setEnabled(b);
        } else if (source instanceof MenuItem) {
            ((MenuItem) source).setEnabled(b);
        }
    }

    /**
     * Determine if the object is visible.
     * @return true if object is visible; otherwise, false
     */
    public boolean isVisible() {
        if (source instanceof Component) {
            return ((Component) source).isVisible();
        } else {
            return false;
        }
    }

    /**
     * Set the visible state of the object.
     * @param b if true, shows this object; otherwise, hides it 
     */
    public void setVisible(boolean b) {
        if (source instanceof Component) {
            ((Component) source).setVisible(b);
        }
    }

    /**
     * Determine if the object is showing.  This is determined by checking
     * the visibility of the object and ancestors of the object.
     * @return true if object is showing; otherwise, false
     */
    public boolean isShowing() {
        if (source instanceof Component) {
            return ((Component) source).isShowing();
        } else {
            return false;
        }
    }

    /** 
     * Checks whether the specified Point is within this object's bounds, 
     * where the Point is relative to the coordinate system of the object.
     * @param p the Point relative to the coordinate system of the object
     * @return true if object contains Point; otherwise false
     */
    public boolean contains(Point p) {
        if (source instanceof Component) {
            return ((Component) source).contains(p);
        } else {
            return false;
        }
    }

    /** 
     * Returns the location of the object on the screen.
     * @return location of object on screen -- can be null if this object
     * is not on the screen
     */
    public Point getLocationOnScreen() {
        if (source instanceof Component) {
            return ((Component) source).getLocationOnScreen();
        } else {
            return null;
        }
    }

    /** 
     * Returns the location of the object relative to parent.
     * @return location of object relative to parent -- can be null if
     * this object or its parent are not on the screen
     */
    public Point getLocation() {
        if (source instanceof Component) {
            return ((Component) source).getLocation();
        } else {
            return null;
        }
    }

    /** 
     * Sets the location of the object relative to parent.
     */
    public void setLocation(Point p) {
        if (source instanceof Component) {
            ((Component) source).setLocation(p);
        }
    }

    /** 
     * Returns the current bounds of this object
     * @return current bounds of object -- can be null if this object
     * is not on the screen
     */
    public Rectangle getBounds() {
        if (source instanceof Component) {
            return ((Component) source).getBounds();
        } else {
            return null;
        }
    }

    /** 
     * Sets the current bounds of this object
     */
    public void setBounds(Rectangle r) {
        if (source instanceof Component) {
            ((Component) source).setBounds(r);
        }
    }

    /** 
     * Returns the current size of this object
     * @return current size of object -- can be null if this object is
     * not on the screen
     */
    public Dimension getSize() {
        if (source instanceof Component) {
            return ((Component) source).getSize();
        } else {
            return null;
        }
    }

    /** 
     * Sets the current size of this object
     */
    public void setSize(Dimension d) {
        if (source instanceof Component) {
            ((Component) source).setSize(d);
        }
    }

    /**
     * Returns the Accessible child contained at the local coordinate
     * Point, if one exists.
     * @return the Accessible at the specified location, if it exists
     */
    public Accessible getAccessibleAt(Point p) {
	if (source instanceof Component) {
	    Component c = ((Component) source).getComponentAt(p);
	    if (c != null) {
	    	return (getAccessible(c));
	    }
	}
	return null;
    }

    /**
     * Returns whether this object can accept focus or not.
     * @return true if object can accept focus; otherwise false
     */
    public boolean isFocusTraversable() {
        if (source instanceof Component) {
            return ((Component) source).isFocusTraversable();
        } else {
            return false;
        }
    }

    /**
     * Requests focus for this object.
     */
    public void requestFocus() {
        if (source instanceof Component) {
            ((Component) source).requestFocus();
        }
    }

    /**
     * Adds the specified focus listener to receive focus events from this 
     * component.
     * @param l the focus listener
     */
    public synchronized void addFocusListener(FocusListener l) {
        if (source instanceof Component) {
	    ((Component) source).addFocusListener(l);
	}
    }

    /**
     * Removes the specified focus listener so it no longer receives focus 
     * events from this component.
     * @param l the focus listener
     */
    public synchronized void removeFocusListener(FocusListener l) {
        if (source instanceof Component) {
	    ((Component) source).removeFocusListener(l);
	}
    }
}

