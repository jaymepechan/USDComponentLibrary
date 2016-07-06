/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Ferret.java	1.32 02/01/17
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
 * <P>Ferret is an example assistive technology that uses EventQueueMonitor,
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=Ferret
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
 * @see EventQueueMonitor
 * @see AWTEventMonitor
 * @see SwingEventMonitor
 *
 * @version     1.32 01/17/02 16:11:40
 * @author      Peter Korn
 */
public class Ferret implements MouseMotionListener, FocusListener, 
        CaretListener, KeyListener, ActionListener, MenuListener,
        GUIInitializedListener {

    public static int WIDTH = 680;
    public static int HEIGHT = 500;
    
    AccessibilityAPIPanel apiPanel;

    JMenu activeMenu = null; 

    TimeTracker tracker;
    javax.swing.Timer timer;

    CheckboxMenuItem trackMouseItem;
    CheckboxMenuItem trackFocusItem;
    CheckboxMenuItem trackCaretItem;
    CheckboxMenuItem updateF1Item;

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

        // Settings Menu
        Menu settings = (Menu) menuBar.add(new Menu("Settings"));
	il = new ItemListener() {
            public void itemStateChanged(ItemEvent e) {
		CheckboxMenuItem checkmi = (CheckboxMenuItem) e.getItemSelectable();
		if (e.getStateChange() == ItemEvent.SELECTED) {
		    checkmi.setState(true);
		} else if (e.getStateChange() == ItemEvent.DESELECTED) {
		    checkmi.setState(false);
		}
            }
        };
        trackMouseItem = (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Mouse"));
        trackMouseItem.addItemListener(il);
        trackFocusItem = (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Focus"));
        trackFocusItem.addItemListener(il);
        trackCaretItem = (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Caret"));
        trackCaretItem.addItemListener(il);
	updateF1Item = (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Update with F1 key"));
        updateF1Item.addItemListener(il);

        frame.setMenuBar(menuBar);
    }


    // Create the GUI
    //
    public Ferret() {
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
	Frame frame = new Frame("Ferret");

        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        frame.setLayout(new BorderLayout());
        frame.addWindowListener(l);
        createMenuBar(frame);
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
	// Create a 1/2 sec. delay timer for mouse motion response
	//
	timer = new javax.swing.Timer(500, this);
	timer.stop();   // just being sure...

	SwingEventMonitor.addMenuListener(this);		// mouse may be in popped menu...

	SwingEventMonitor.addMouseMotionListener(this);	// respond to mouse motion
	SwingEventMonitor.addFocusListener(this);		// respond to focus changes
	SwingEventMonitor.addCaretListener(this);		// respond to caret movement
	SwingEventMonitor.addKeyListener(this);			// respond to F1
    }


    public void actionPerformed(ActionEvent e) {
	timerTimeout();
    }  

    public void timerTimeout() {
        timer.stop();
	if (trackMouseItem.getState() == true) {
	    updateAccessibleAtMouse();
	}
    }

    // Handle mouse movements
    //
    public void mouseDragged(MouseEvent e) {
        timer.stop();
        timer.start();
    }

    public void mouseMoved(MouseEvent e) {
        timer.stop();
        timer.start();
    }

    // Handle Function Key
    //
    public void keyTyped(KeyEvent theEvent) {
    }

    public void keyReleased(KeyEvent theEvent) {
    }

    public void keyPressed(KeyEvent theEvent) {
	if (updateF1Item.getState() == true) {
	    String statusString = null;
	    if (theEvent.getKeyCode() == KeyEvent.VK_F1) {
		updateAccessibleAtMouse();
	    }
	}
    }

    // Handle Focus events
    //
    public void focusGained(FocusEvent theEvent) {
	if (trackFocusItem.getState() == true) {
	    updateAccessibilityAtFocus();
	}
    }

    public void focusLost(FocusEvent theEvent) {
    }

    // Handle Caret events
    //
    public void caretUpdate(CaretEvent theEvent) {
	if (trackCaretItem.getState() == true) {
	    updateAccessibilityAtCaret(theEvent);
	}
    }


    // Handle Menu events (track the activeMenu)
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


    // Update (and speak) what's under the mouse
    //   
    public void updateAccessibleAtMouse() {
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
		apiPanel.updateInfo(ac, containerPoint);

/*
System.out.println("*** in Ferret.updateAccessibleAtMouse()");
if (a instanceof JTextComponent) {
    JTextComponent t = (JTextComponent) a;
    Document model = t.getDocument();
    int i = t.viewToModel(containerPoint);
    Element e = null;
    AttributeSet as;
    String attributeStr;
    int treeCount = 0;
    for (e = model.getDefaultRootElement(); ! e.isLeaf(); treeCount++) {
	as = e.getAttributes();
	attributeStr = expandAttributes(as);
	System.out.println("  -> Attributes for element: " + e.toString() + ", at level: " + treeCount + ": " + attributeStr);
        int index = e.getElementIndex(i);
        e = e.getElement(index);
    }
}
*/
	    } else {
		apiPanel.updateInfo(null,null);
	    }	
	} else {
	    apiPanel.updateInfo(null, null);
	}
    }


    // Update (and speak) the Accessible with focus
    //   
    public void updateAccessibilityAtFocus() {
	Accessible a;
	a = Translator.getAccessible(SwingEventMonitor.getComponentWithFocus());

	if (a != null) {
	    AccessibleContext ac = a.getAccessibleContext();
	    if (ac != null) {
		apiPanel.updateInfo(ac, null);
	    }
        }
    }


    // Update (and speak) the Accessible at Caret Event
    //   
    public void updateAccessibilityAtCaret(CaretEvent e) {
	Accessible a;
	a = Translator.getAccessible(e.getSource());

	if (a != null) {
	    AccessibleContext ac = a.getAccessibleContext();
	    if (ac != null) {
		apiPanel.updateInfo(ac, null);
	    }
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

        new Ferret();
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

/**
 *  Wrapper class around Timer to recieve ActionPerformed's
 */
class TimeTracker implements ActionListener {
    public javax.swing.Timer timer;
    public Ferret ferret;

    public TimeTracker(int delay, Ferret e) {
        timer = new javax.swing.Timer(delay, this);
	ferret = e;
    }

    public void actionPerformed(ActionEvent e) {
        ferret.timerTimeout();
    }  
}


