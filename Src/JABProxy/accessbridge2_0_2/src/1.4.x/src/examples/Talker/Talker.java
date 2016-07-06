/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Talker.java	1.7 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

import javax.speech.*;
import javax.speech.synthesis.*;

/**
 * <P>Talker is an example assistive technology that uses EventQueueMonitor,
 * AWTEventMonitor, and SwingEventMonitor.
 *
 * <P>It is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=Talker
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
 * @version     1.7 01/17/02
 * @author      Peter Korn
 */
public class Talker extends Frame
    implements MouseMotionListener, FocusListener, CaretListener, KeyListener,
	       ActionListener, MenuListener {

    public static int WIDTH = 680;
    public static int HEIGHT = 500;
    
    AccessibilityAPIPanel apiPanel;

    JMenu activeMenu = null; 
    JavaSpeechSpeaker speaker;

    TimeTracker tracker;
    Timer timer;

    CheckboxMenuItem trackMouseItem;
    CheckboxMenuItem trackFocusItem;
    CheckboxMenuItem trackCaretItem;
    CheckboxMenuItem updateF1Item;

    void createMenuBar() {
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

        // Speech Settings Menu
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
        trackMouseItem = 
	    (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Mouse"));
        trackMouseItem.addItemListener(il);
        trackFocusItem = 
	    (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Focus"));
        trackFocusItem.addItemListener(il);
        trackCaretItem = 
	    (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Track Caret"));
        trackCaretItem.addItemListener(il);
	updateF1Item = 
	    (CheckboxMenuItem) settings.add(new CheckboxMenuItem("Update with F1 key"));
        updateF1Item.addItemListener(il);

        setMenuBar(menuBar);
    }


    // Create the GUI
    //
    public Talker() {
	super("Talker");

        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        setLayout(new BorderLayout());
        addWindowListener(l);
        createMenuBar();
	apiPanel = new AccessibilityAPIPanel(20);
        add("Center", apiPanel);

	initializeSpeech();
	initializeSettings();

        setSize(WIDTH, HEIGHT);
	    pack();
	    show();
	}


    // initialize the speech subsystem
    //
    void initializeSpeech() {
	speaker = new JavaSpeechSpeaker();
    }


    // initialize settings
    //
    void initializeSettings() {
	// Create a 1/2 sec. delay timer for mouse motion response
	//
	timer = new Timer(500, this);
	timer.stop();   // just being sure...

	SwingEventMonitor.addMenuListener(this);  // may be in popped menu...

	SwingEventMonitor.addMouseMotionListener(this);	// respond to mouse motion
	SwingEventMonitor.addFocusListener(this); // respond to focus changes
	SwingEventMonitor.addCaretListener(this); // respond to caret movement
	SwingEventMonitor.addKeyListener(this);	  // respond to F1
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
        if (speaker != null && trackMouseItem.getState() == true) {
            speaker.shutup();
        }
    }

    public void mouseMoved(MouseEvent e) {
        timer.stop();
        timer.start();
        if (speaker != null && trackMouseItem.getState() == true) {
            speaker.shutup();
        }
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


    // Speak Accessible information about the object under the mouse
    //
    public void speakInfo(AccessibleContext ac, Point p) {
        String s;
	if (speaker != null) {
	    speaker.shutup();
	}
        AccessibleRole ar = ac.getAccessibleRole();
        if (ar != null) {
            speak(ar.toString());
        }
	s = ac.getAccessibleName();
	if (s != null) {
	    speak(ac.getAccessibleName());
	}
	s = ac.getAccessibleDescription();
	if (s != null) {
	    speak("Description: " + s);
	}
        AccessibleText textInfo = ac.getAccessibleText();
        if (textInfo != null) {
            int i = textInfo.getIndexAtPoint(p);
            Object o;
            o = textInfo.getAtIndex(AccessibleText.CHARACTER, i);
            if (o instanceof String) {
            	speak("Letter:   " + (String) o);
            }
            o = textInfo.getAtIndex(AccessibleText.WORD, i);
            if (o instanceof String) {
            	speak("Word:     " + (String) o);
            }
            o = textInfo.getAtIndex(AccessibleText.SENTENCE, i);
            if (o instanceof String) {
            	speak("Sentence: " + (String) o);
            }
        }
    }

    // Speak Accessible information about the object under the mouse
    //
    public void speakCaret(AccessibleContext ac) {
	if (speaker != null) {
	    speaker.shutup();
	}
	AccessibleText textInfo = ac.getAccessibleText();
        if (textInfo != null) {
            int i = textInfo.getCaretPosition();
            Object o;
            o = textInfo.getAtIndex(AccessibleText.CHARACTER, i);
            if (o instanceof String) {
            	speak("Letter:   " + (String) o);
            }
            o = textInfo.getAtIndex(AccessibleText.WORD, i);
            if (o instanceof String) {
            	speak("Word:     " + (String) o);
            }
        }
    }


    // Placeholder for JavaSpeech routine
    //
    public void speak(String s) {
        if (speaker != null) {
            speaker.speak(s);
        }
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
		speakInfo(ac, containerPoint);
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
		speakInfo(ac, null);
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
		speakCaret(ac);
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

        new Talker();
    }
}

/**
 *  Wrapper class around Timer to recieve ActionPerformed's
 */
class TimeTracker implements ActionListener {
    public Timer timer;
    Talker talker; 

    public TimeTracker(int delay, Talker t) {
        talker = t;
        timer = new Timer(delay, this);
    }

    public void actionPerformed(ActionEvent e) {
        talker.timerTimeout();
    }  
}

/**
 *  Wrapper class around speech
 */
class JavaSpeechSpeaker {
    static Synthesizer synthesizer;

    public JavaSpeechSpeaker() {
	try {
            // synthesizer
	    synthesizer = Central.createSynthesizer(null);
            synthesizer.allocate();
            synthesizer.speakPlainText("Talker starting up");
	} catch (Throwable t) {
	    t.printStackTrace();
	}
    }

    public void speak(String s) {
	try {
	    if (synthesizer != null) {
		synthesizer.speakPlainText(s);
	    }
	} catch (Exception e) {
	    System.out.println("synthesizer.speak threw exception: " + e);
	    e.printStackTrace();
	}
    }

    public void shutup() {
	if (synthesizer != null) {
	    synthesizer.cancelAll();
	}
    }
}

