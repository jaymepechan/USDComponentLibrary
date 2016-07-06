/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AWTMonitor.java	1.16 02/01/17
 */

import java.util.Properties;
import java.beans.*;

import java.awt.*;
import java.awt.event.*;
import java.applet.Applet;

import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>AWTMonitor is an example assistive technology that uses EventQueueMonitor
 * and AWTEventMonitor.
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=AWTMonitor
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
 * @see AWTEventMonitor
 *
 * @version     1.16 01/17/02 16:09:13
 * @author      Willie Walker
 * @author      Peter Korn
 */
public class AWTMonitor implements GUIInitializedListener {

    Frame frame;
    AWTMonitorEventListener amel = null;
    GridBagLayout gbl = new GridBagLayout();

    java.awt.List eventStatusList;
    int  eventStatusListSize = 0;

    java.awt.List focusInformation;
    java.awt.List mouseInformation;

    public AWTMonitor() {
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
        frame = new Frame("AWT Monitor");
        GridBagConstraints c = new GridBagConstraints();
        frame.setLayout(gbl);

        // Make the menu bar
        //
        MenuBar mb = new MenuBar();
        Menu m     = new Menu("File");
        m.add("Exit");
        mb.add(m);
        frame.setMenuBar(mb);

        // Make the event selector checkbox area
        //
        amel = new AWTMonitorEventListener(this);

        Checkbox cb;
        Panel eventSelector = new Panel();
        eventSelector.setLayout(new GridLayout(4,6));
        eventSelector.add(cb = new Checkbox("Action"));
        cb.addItemListener(new AWTMonitorCommand(EventID.ACTION,amel));

        eventSelector.add(cb = new Checkbox("Adjustment"));
        cb.addItemListener(new AWTMonitorCommand(EventID.ADJUSTMENT,amel));

        eventSelector.add(cb = new Checkbox("Component",false));
        cb.addItemListener(new AWTMonitorCommand(EventID.COMPONENT,amel));

        eventSelector.add(cb = new Checkbox("Container",false));
        cb.addItemListener(new AWTMonitorCommand(EventID.CONTAINER,amel));

        eventSelector.add(cb = new Checkbox("Focus"));
        cb.addItemListener(new AWTMonitorCommand(EventID.FOCUS,amel));

        eventSelector.add(cb = new Checkbox("Item"));
        cb.addItemListener(new AWTMonitorCommand(EventID.ITEM,amel));

        eventSelector.add(cb = new Checkbox("Key"));
        cb.addItemListener(new AWTMonitorCommand(EventID.KEY,amel));

        eventSelector.add(cb = new Checkbox("Mouse"));
        cb.addItemListener(new AWTMonitorCommand(EventID.MOUSE,amel));

        eventSelector.add(cb = new Checkbox("Mouse Motion"));
        cb.addItemListener(new AWTMonitorCommand(EventID.MOTION,amel));

        eventSelector.add(cb = new Checkbox("Text"));
        cb.addItemListener(new AWTMonitorCommand(EventID.TEXT,amel));

        eventSelector.add(cb = new Checkbox("Window",false));
        cb.addItemListener(new AWTMonitorCommand(EventID.WINDOW,amel));

        addComp(frame,eventSelector,0,0,2,1,0.0,0.0,
                new Insets(4,4,0,4));

        // Make the event status list
        //
        addComp(frame,eventStatusList = new java.awt.List(10,true),0,1,2,1,1.0,1.0,
                new Insets(4,4,0,4));

        // Add the focus information
        //
        addComp(frame,new Label("Last Focus Detail:"),0,2,1,1,0.5,0.0,
                new Insets(4,4,0,0));
        addComp(frame,focusInformation = new java.awt.List(5),0,3,1,1,0.5,0.0,
                new Insets(0,4,4,4));
                

        // Add the mouse information
        //
        addComp(frame,new Label("Object under Mouse (F1):"),1,2,1,1,0.5,0.0,
                new Insets(4,0,0,0));
        addComp(frame,mouseInformation = new java.awt.List(5),1,3,1,1,0.5,0.0,
                new Insets(0,0,4,4));

        frame.pack();
        frame.show();
    }

    static void addComp(Container cont, Component comp,
                        int x, int y,
                        int w, int h,
                        double weightx, double weighty,
                        Insets insets) {

        cont.add(comp);

        GridBagLayout gbl = (GridBagLayout)cont.getLayout();
        GridBagConstraints c = new GridBagConstraints();
        c.fill = GridBagConstraints.BOTH;
        c.gridx = x;
        c.gridy = y;
        c.gridwidth = w;
        c.gridheight = h;
        c.weightx = weightx;
        c.weighty = weighty;
        c.insets = insets;
        gbl.setConstraints(comp,c);
    }

    public void displayInformationInfo(java.awt.List list, Accessible information) {
        list.removeAll();
        if (information == null) {
            list.add("(null)");
	}
	AccessibleContext ac = information.getAccessibleContext();
	if (ac == null) {
            list.add("(null)");
        } else {
             list.add("Name:  " + ac.getAccessibleName());
             list.add("Desc:  " + ac.getAccessibleDescription());
             list.add("Role:  " + ac.getAccessibleRole());
             list.add("State: " + ac.getAccessibleStateSet().toString());
	     AccessibleValue av = ac.getAccessibleValue();
	     if (av != null) {
                 list.add("Value: " + av.getCurrentAccessibleValue());
	    }
        }
    }

    public void displayFocusInformation() {
        Component c = AWTEventMonitor.getComponentWithFocus();
        Accessible a = null;
        if (c instanceof Accessible) {
            a = (Accessible) c; 
        } else {
            a = Translator.getAccessible(c);
        }
        displayInformationInfo(focusInformation,a);
    }

    public void displayMouseInformation(Accessible information) {
        displayInformationInfo(mouseInformation,information);
    }

    public void displayEvent(String eventString) {
        if (eventStatusListSize > 100) {
            eventStatusList.remove(0);
        } else {
            eventStatusListSize++;
        }

        eventStatusList.add(eventString);
        eventStatusList.makeVisible(eventStatusList.getItemCount() - 1);
    }

    static public void main(String args[]) {
        new AWTMonitor();
    }
}

class AWTMonitorCommand implements ItemListener {
    int    id;
    AWTMonitorEventListener amel;

    public AWTMonitorCommand(int id, AWTMonitorEventListener amel) {
        this.id   = id;
        this.amel = amel;
    }

    public void itemStateChanged(ItemEvent e) {
        if (e.getStateChange() == ItemEvent.SELECTED) {
            switch (id) {

            case EventID.COMPONENT:
                AWTEventMonitor.addComponentListener((ComponentListener)amel);
                break;

            case EventID.CONTAINER:
                AWTEventMonitor.addContainerListener((ContainerListener)amel);
                break;

            case EventID.FOCUS:
                AWTEventMonitor.addFocusListener((FocusListener)amel);
                break;

            case EventID.KEY:
                AWTEventMonitor.addKeyListener((KeyListener)amel);
                break;

            case EventID.MOUSE:
                AWTEventMonitor.addMouseListener((MouseListener)amel);
                break;

            case EventID.MOTION:
                AWTEventMonitor.addMouseMotionListener((MouseMotionListener)amel);
                break;
                
            case EventID.WINDOW:
                AWTEventMonitor.addWindowListener((WindowListener)amel);
                break;
               
            case EventID.ACTION:
                AWTEventMonitor.addActionListener((ActionListener)amel);
                break;
                
            case EventID.ITEM:
                AWTEventMonitor.addItemListener((ItemListener)amel);
                break;

            case EventID.TEXT:
                AWTEventMonitor.addTextListener((TextListener)amel);
                break;

            default:
                break;
            }
        } else {
            switch (id) {

            case EventID.COMPONENT:
                AWTEventMonitor.removeComponentListener((ComponentListener)amel);
                break;

            case EventID.CONTAINER:
                AWTEventMonitor.removeContainerListener((ContainerListener)amel);
                break;

            case EventID.FOCUS:
                AWTEventMonitor.removeFocusListener((FocusListener)amel);
                break;

            case EventID.KEY:
                AWTEventMonitor.removeKeyListener((KeyListener)amel);
                break;

            case EventID.MOUSE:
                AWTEventMonitor.removeMouseListener((MouseListener)amel);
                break;

            case EventID.MOTION:
                AWTEventMonitor.removeMouseMotionListener((MouseMotionListener)amel);
                break;
                
            case EventID.WINDOW:
                AWTEventMonitor.removeWindowListener((WindowListener)amel);
                break;
               
            case EventID.ACTION:
                AWTEventMonitor.removeActionListener((ActionListener)amel);
                break;
                
            case EventID.ITEM:
                AWTEventMonitor.removeItemListener((ItemListener)amel);
                break;

            case EventID.TEXT:
                AWTEventMonitor.removeTextListener((TextListener)amel);
                break;

            default:
                break;
            }
	}
    }
}

class AWTMonitorEventListener implements ComponentListener, ContainerListener,
    FocusListener, KeyListener, MouseListener, MouseMotionListener,
    WindowListener, ActionListener,AdjustmentListener,ItemListener,
    TextListener {

    AWTMonitor app;

    public AWTMonitorEventListener(AWTMonitor app) {
        this.app = app;
    }

        /* ActionListener Methods ***************************************/
  
    public void actionPerformed(ActionEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* AdjustmentListener Methods ***********************************/

    public void adjustmentValueChanged(AdjustmentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* ItemListener Methods *****************************************/

    public void itemStateChanged(ItemEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

       /* ComponentListener Methods ************************************/
  
    public void componentMoved(ComponentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void componentResized(ComponentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void componentShown(ComponentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void componentHidden(ComponentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

       /* ContainerListener Methods ************************************/

    public void componentAdded(ContainerEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void componentRemoved(ContainerEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* FocusListener Methods ****************************************/
  
    public void focusGained(FocusEvent theEvent) {
        app.displayFocusInformation();
        app.displayEvent(theEvent.toString());
    }

    public void focusLost(FocusEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* KeyListener Methods ******************************************/
  
    public void keyTyped(KeyEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void keyPressed(KeyEvent theEvent) {
        String statusString = null;
        if (theEvent.getKeyCode() == KeyEvent.VK_F1) {
            Point currentMousePos = EventQueueMonitor.getCurrentMousePosition();
            Accessible a = EventQueueMonitor.getAccessibleAt(currentMousePos);
            app.displayMouseInformation(a);
        }
        app.displayEvent(theEvent.toString());
    }

    public void keyReleased(KeyEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* MouseListener Methods ****************************************/
  
    public void mouseClicked(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void mousePressed(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void mouseReleased(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void mouseEntered(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void mouseExited(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* MouseMotionListener Methods **********************************/
  
    public void mouseDragged(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void mouseMoved(MouseEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowOpened(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

        /* TextListener Methods *****************************************/
  
    public void textValueChanged(TextEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }
  
        /* WindowListener Methods ***************************************/
  
    public void windowClosing(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowClosed(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowIconified(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowDeiconified(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowActivated(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void windowDeactivated(WindowEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }
}

