/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibilityMonitor-AWT.java	1.4 02/01/17
 */

import java.beans.*;
import java.awt.*;
import java.awt.event.*;

import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>AccessibilityMonitor is an example assistive technology that uses 
 * EventQueueMonitor and AccessibilityEventMonitor.
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=AccessibilityMonitor
 * </PRE>
 * <P>Note that AWT.AutoLoadClasses can take a comma-separated list of 
 * assistive technology classes.  Note also that you need to make sure 
 * the classes for the assistive technologies are in your CLASSPATH
 * environment variable.
 *
 * <P>Furthermore, this class is dynamically loaded and a single instance
 * is created by the event queue class (see EventQueueMonitor) in a separate 
 * thread.  For this to work properly, the constructor method needs to 
 * do all the work.
 *
 * <P>For more information on the event delegation model, see the event
 * delegation model specification that comes with the JDK 1.1.1 documentation.
 *
 * <p>NOTE:  This is a preliminary draft.  The methods and name may change 
 * in future beta releases.
 *
 * @see EventQueueMonitor
 * @see AccessibilityEventMonitor
 *
 * @version     1.4 01/17/02 16:11:20
 * @author      Peter Korn
 */
public class AccessibilityMonitor extends Frame 
    implements PropertyChangeListener {

    GridBagLayout gbl = new GridBagLayout();

    java.awt.List eventStatusList;
    int  eventStatusListSize = 0;

    // Create the GUI
    //
    public AccessibilityMonitor() {
        super("Accessibility Monitor");
        setLayout(new BorderLayout());

        // Make the menu bar
        //
        MenuBar mb = new MenuBar();
        Menu m     = new Menu("File");
        m.add("Exit");
        mb.add(m);
        setMenuBar(mb);

        // Make the event selector checkbox area
        //
        Checkbox cb;
        Panel eventSelector = new Panel();
        eventSelector.setLayout(new BorderLayout());
        eventSelector.add(cb = new Checkbox("Accessibility PropertyChange Events"));
        cb.addItemListener(new ItemListener() {
	    public void itemStateChanged(ItemEvent e) {
		if (e.getStateChange() == ItemEvent.SELECTED) {
		    AccessibilityEventMonitor.addPropertyChangeListener(AccessibilityMonitor.this);
		} else {
		    AccessibilityEventMonitor.removePropertyChangeListener(AccessibilityMonitor.this);
		}
	    }
	});

	add("North", eventSelector);

        // Make the event status list
        //
        add("Center", eventStatusList = new java.awt.List(10,true));

        pack();
        show();
    }

    public void displayEvent(String eventString) {
        if (eventStatusListSize > 100) {
            eventStatusList.delItem(0);
        } else {
            eventStatusListSize++;
        }

        eventStatusList.addItem(eventString);
        eventStatusList.makeVisible(eventStatusList.getItemCount() - 1);
    }

        /* PropertyChangeListener Methods *******************************/
  
    public void propertyChange(PropertyChangeEvent theEvent) {
        displayEvent(theEvent.toString());
    }

    static public void main(String args[]) {
        new AccessibilityMonitor();
    }
}
