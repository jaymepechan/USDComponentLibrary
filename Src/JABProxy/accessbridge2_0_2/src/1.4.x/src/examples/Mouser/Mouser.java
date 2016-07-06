/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Mouser.java	1.5 02/01/17
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
 * <P>Mouser is an example assistive technology that uses EventQueueMonitor,
 *
 * <P>It is meant to run in the same Java Virtual Machine as
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * assistive_technologies=Mouser
 * </PRE>
 *
 * @see EventQueueMonitor
 * @see AWTEventMonitor
 * @see SwingEventMonitor
 *
 * @version     1.27 10/06/98 16:00:01
 * @author      Peter Korn
 */
public class Mouser implements MouseMotionListener, KeyListener,
        GUIInitializedListener {

    public static int WIDTH = 680;
    public static int HEIGHT = 500;
    
    AccessibilityAPIPanel apiPanel;
    TextField capturedText;
    TextField coordText;
    Point capturedPoint;
    Point currentMousePoint;

    void createMenuBar(Frame frame) {
        MenuBar menuBar = new MenuBar();
        MenuItem mi;
	    CheckboxMenuItem cmi;
	    ItemListener il;

        // File Menu
        Menu file = (Menu) menuBar.add(new Menu("File"));
        mi = (MenuItem) file.add(new MenuItem("Exit"));
        mi.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
		        System.exit(0);
            }
        });

        frame.setMenuBar(menuBar);
    }


    // Create the GUI
    //
    public Mouser() {
        currentMousePoint = new Point(0,0);
        capturedPoint = new Point(0,0);
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
	    Frame frame = new Frame("Mouser");

        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        frame.setLayout(new BorderLayout());
        frame.addWindowListener(l);
        createMenuBar(frame);
        Panel p = new Panel();
        p.setLayout(new BorderLayout());
        capturedText = new TextField();
        p.add("North", capturedText);
        coordText = new TextField();
        p.add("Center", coordText);
        frame.add("North", p);
        apiPanel = new AccessibilityAPIPanel();
        frame.add("Center", apiPanel);
        initializeSettings();

        frame.setSize(WIDTH, HEIGHT);
        frame.pack();
        frame.show();
    }


    // initialize the tracking settings
    //
    void initializeSettings() {
        AWTEventMonitor.addMouseMotionListener(this);	// respond to mouse motion
        AWTEventMonitor.addKeyListener(this);			// respond to F1, F2
    }


    // Handle mouse movements
    //
    public void mouseDragged(MouseEvent e) {
        Component c = e.getComponent();
        Point sourcePoint = c.getLocationOnScreen();
        Point eventPoint = e.getPoint();
        currentMousePoint.setLocation(sourcePoint.x + eventPoint.x,
                                      sourcePoint.y + eventPoint.y);
	    String s = new String("Current mouse = (" + currentMousePoint.x + ", " +
			          currentMousePoint.y + ")");
	    capturedText.setText(s);
    }

    public void mouseMoved(MouseEvent e) {
        Component c = e.getComponent();
        Point sourcePoint = c.getLocationOnScreen();
        Point eventPoint = e.getPoint();
        currentMousePoint.setLocation(sourcePoint.x + eventPoint.x,
                                      sourcePoint.y + eventPoint.y);
	    String s = new String("Current mouse = (" + currentMousePoint.x + ", " +
			          currentMousePoint.y + ")");
	    capturedText.setText(s);
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
		    captureMousePoint();
	    } else if (theEvent.getKeyCode() == KeyEvent.VK_F2) {
		    updateAccessibleAtPoint(capturedPoint);
	    }
    }


    // Handle Function Key
    //
    void captureMousePoint() {
        capturedPoint.setLocation(currentMousePoint);
	    String s = new String("Captured mouse = (" + capturedPoint.x + ", " +
			          capturedPoint.y + ")");
	    coordText.setText(s);
    }


    // Update (and speak) what's at the Point
    //   
    public void updateAccessibleAtPoint(Point p) {
        Accessible a;
	    a = EventQueueMonitor.getAccessibleAt(p);
        if (a != null) {
	        AccessibleContext ac = a.getAccessibleContext();
	        Point containerPoint = null;
	        if (ac != null) {
		        AccessibleComponent acmp = ac.getAccessibleComponent();
		        if (acmp != null) {
		            Point containerLoc = acmp.getLocationOnScreen();
		            if (containerLoc != null) {
			            containerPoint = 
				        new Point(p.x - containerLoc.x, p.y - containerLoc.y);
		            } 
		        }
		        apiPanel.updateInfo(ac, containerPoint);

	        } else {
		        apiPanel.updateInfo(null,null);
	        }	
	    } else {
	        apiPanel.updateInfo(null, null);
	    }
    }


    // Start the whole shebang going 
    //   
    static public void main(String args[]) {
        String vers = System.getProperty("java.version");
        if (vers.compareTo("1.1.2") < 0) {
            System.out.println("!!!WARNING: Talker must be run with a " +
                               "1.1.2 or higher version VM!!!");
        }

        new Mouser();
    }


// idea:
//   - get list of attribute keys in the heirarchy
//   - build a hashtable with all keys and names, going from root to leaf
//   - build string with keys and (leaf-biased) attributes, using toString()

    private class HashtableRef {
        Hashtable table;
    }

    private String expandAttributes(AttributeSet as) {
        HashtableRef hashtable = new HashtableRef();
        hashtable.table = new Hashtable();
        AttributeSet currentAS = as;
        while (currentAS != null) {
            gatherAttributeKeys(currentAS, hashtable);
            currentAS = currentAS.getResolveParent();
        }

        String attributeText = new String("");
        Enumeration attributeNames = hashtable.table.keys();
        Object attrName;
        while (attributeNames.hasMoreElements()) {
            attrName = attributeNames.nextElement();
            attributeText += attrName.toString() + " = "
                + hashtable.table.get(attrName).toString();
            if (attributeNames.hasMoreElements()) {
                attributeText += ", ";
            }
        }
        return attributeText;
    }

    private void gatherAttributeKeys(AttributeSet as, HashtableRef htr) {
        Enumeration names = as.getAttributeNames();
        Object name;
        Object attribute;
        while (names.hasMoreElements()) {
            name = names.nextElement();
            if (!htr.table.containsKey(name)) {
                attribute = as.getAttribute(name);
                if (attribute instanceof AttributeSet) {
                    htr.table.put(name, as.NameAttribute);
                    AttributeSet currentAS = (AttributeSet) attribute;
                    while (currentAS != null) {
                        gatherAttributeKeys((AttributeSet) attribute, htr);
                        currentAS = currentAS.getResolveParent();
                    }
                } else {
                    htr.table.put(name, as.getAttribute(name));
                }
            }
        }
    }



}

