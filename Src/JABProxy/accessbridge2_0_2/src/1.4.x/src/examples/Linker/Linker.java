/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Linker.java	1.7 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>Linker is an example assistive technology that uses EventQueueMonitor,
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.assistive_technologies=Linker
 * </PRE>
 * <P>Note that AWT.assistive_technologies can take a comma-separated list of 
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
 * @see EventQueueMonitor
 *
 * @version     1.0 08/02/97 17:08:39
 * @author      Peter Korn
 */
public class Linker implements KeyListener, ActionListener, MenuListener,
        GUIInitializedListener {
    
    JMenu activeMenu = null; 
    AccessibleHypertextPanel ahPanel;

    public Linker() {
        if (EventQueueMonitor.isGUIInitialized()) {
	    createGUI();
	} else {
	    EventQueueMonitor.addGUIInitializedListener(this);
        }
    }

    public void guiInitialized() {
        createGUI();
    }

    public void createGUI() {
        Frame frame = new Frame("Linker");
        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };
        frame.setLayout(new BorderLayout());
        frame.addWindowListener(l);
	    
        // Make the menu bar
        //
        MenuBar mb  = new MenuBar();
        Menu m      = new Menu("File");
        MenuItem mi = new MenuItem("Exit");
        mi.addActionListener(this);
        m.add(mi);
        mb.add(m);

        frame.setMenuBar(mb);

        // Add AccessibleHypertextPanel
        //
	ahPanel = new AccessibleHypertextPanel();
        frame.add("Center", ahPanel);

        frame.pack();
        frame.show();

	SwingEventMonitor.addKeyListener((KeyListener)this);
	SwingEventMonitor.addMenuListener((MenuListener)this);
    }

    public void actionPerformed(ActionEvent e) {
        String s = e.getActionCommand();
        if (s == "Exit") {
            System.exit(0);
        }
    }  

    // Handle menus
    //

    public void menuCanceled(MenuEvent theEvent) {
        activeMenu = null;
    }

    public void menuDeselected(MenuEvent theEvent) {
        activeMenu = null;
    }

    public void menuSelected(MenuEvent theEvent) {
        activeMenu = (JMenu) theEvent.getSource();
    }


    // Handle Function Key
    //

    public void keyTyped(KeyEvent theEvent) {
    }

    public void keyReleased(KeyEvent theEvent) {
    }

    public void keyPressed(KeyEvent theEvent) {
        String statusString = null;
        if (theEvent.getKeyCode() == KeyEvent.VK_F1) {
            updateAccessibleAtMouse();
        }
    }

    private void updateAccessibleAtMouse() {
        Point currentMousePos = EventQueueMonitor.getCurrentMousePosition();
        Accessible a = null;
  	if (activeMenu != null) {
	    JPopupMenu pm = activeMenu.getPopupMenu();
            Point menuLoc = pm.getLocationOnScreen();
            Point menuPoint = new Point(currentMousePos.x - menuLoc.x,
                                        currentMousePos.y - menuLoc.y);
            Component c = pm.getComponentAt(menuPoint);
	    if (c instanceof Accessible) {
		a = (Accessible) c;
	    }
	}
	if (a == null) {
	    a = EventQueueMonitor.getAccessibleAt(currentMousePos);
	}
        if (a != null) {
	    AccessibleContext ac = a.getAccessibleContext();
	    Point containerPoint = null;
	    if (ac != null) {
		AccessibleComponent acmp = ac.getAccessibleComponent();
		if (acmp != null) {
		    Point containerLoc = acmp.getLocationOnScreen();
		    if (containerLoc != null) {
			containerPoint = 
			    new Point(currentMousePos.x - containerLoc.x,
				      currentMousePos.y - containerLoc.y);
		    } 
		}
		ahPanel.updateInfo(ac, containerPoint);
	    } else {
		ahPanel.updateInfo(null,null);
	    }
        } else {
	    ahPanel.updateInfo(null, null);
	}
    }

   static public void main(String args[]) {
        new Linker();
    }
}
