/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)JavaMonitor.java	1.45 02/01/17
 */

import java.util.*;
import java.beans.*;

import java.awt.*;
import java.awt.event.*;
import java.applet.Applet;

import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.swing.table.*;
import javax.swing.undo.*;
import javax.swing.border.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>JavaMonitor is an example assistive technology that uses EventQueueMonitor,
 * AWTEventMonitor, and SwingEventMonitor.
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=JavaMonitor
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
 * @see SwingEventMonitor
 *
 * @version     1.45 01/17/02 16:11:49
 * @author      Willie Walker
 * @author      Peter Korn
 */
public class JavaMonitor implements ActionListener, GUIInitializedListener {
    public static int WIDTH = 680;
    public static int HEIGHT = 500;
    public JFrame frame;

    JavaMonitorEventListener jmel = null;

    protected JMenu activeMenu = null;

    DefaultListModel eventModel;
    JList eventList;
    JScrollPane eventPane;

    java.awt.List focusInformation;
    java.awt.List mouseInformation;

    AccessibilityAPIPanel focusPanel;
    AccessibilityAPIPanel mousePanel;

    boolean displayingEvent = false;

    JMenuBar createMenuBar() {
        JMenuBar menuBar = new JMenuBar();
        JMenuItem mi;

        // File Menu
        JMenu file = (JMenu) menuBar.add(new JMenu("File"));
	file.setMnemonic('F');
        mi = (JMenuItem) file.add(new JMenuItem("Exit"));
	mi.setMnemonic('x');
        mi.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                System.exit(0);
            }
        });

        // Panels Menu
        JMenu panels = (JMenu) menuBar.add(new JMenu("Panels"));
        panels.setMnemonic('P');
        mi = (JMenuItem) panels.add(new JMenuItem("Accessibility API panel"));
        mi.setMnemonic('I');
        mi.addActionListener(this);
        mi = (JMenuItem) panels.add(new JMenuItem("AccessibleAction panel"));
        mi.setMnemonic('A');
        mi.addActionListener(this);
        mi = (JMenuItem) panels.add(new JMenuItem("AccessibleSelection panel"));
        mi.setMnemonic('S');
        mi.addActionListener(this);
        mi = (JMenuItem) panels.add(new JMenuItem("AccessibleHypertext panel"));
        mi.setMnemonic('H');
        mi.addActionListener(this);

        return menuBar;
    }

    public JavaMonitor() {
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
        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        frame = new JFrame("Java Monitor");
        frame.getContentPane().setLayout(new BorderLayout());
        frame.addWindowListener(l);
        frame.getContentPane().add("North", createMenuBar());
        frame.getContentPane().add("Center", createEventPanel());

        frame.setSize(WIDTH, HEIGHT);
        frame.show();
    }

    private JPanel createEventPanel() {
	JPanel eventPanel = new JPanel();
	eventPanel.setLayout(new BorderLayout());
	eventPanel.add("North", createCheckboxesPanel());

	JSplitPane splitter = new JSplitPane(JSplitPane.VERTICAL_SPLIT, 
		createEventListPanel(), createExaminerPanel());
	eventPanel.add("Center", splitter);

	return eventPanel;
    }

    // Create the event listing panel
    //
    private JPanel createEventListPanel() {
	JPanel eventListPanel = new JPanel();
	eventListPanel.setLayout(new BorderLayout());

	eventModel = new DefaultListModel();
	eventList = new JList(eventModel);

	MouseListener mouseListener = new MouseAdapter() {
	    public void mouseClicked(MouseEvent e) {
		if (e.getClickCount() == 2) {
		    int index = eventList.locationToIndex(e.getPoint());
		    Object event = eventModel.getElementAt(index);
		    if (event instanceof EventObject) {
			Object o = ((EventObject) event).getSource();
			if (o instanceof Accessible) {
			    AccessibilityAPIPanel panel = 
				new AccessibilityAPIPanel();
			    InfoPanel infoPanel = 
			    	new InfoPanel((Accessible) o, panel);
			}
		    }
		}
	    }
            public void mousePressed(MouseEvent e) {
                if (e.getModifiers() == InputEvent.BUTTON3_MASK) {
                    int index = eventList.locationToIndex(e.getPoint());
                    Object event = eventModel.getElementAt(index);
                    if (event instanceof EventObject) {
                        Object o = ((EventObject) event).getSource();
                        if (o instanceof Accessible) {
                            selectAccessiblePaneMenu((Accessible) o, eventList, e.getX(), e.getY());
			}
		    }
                }
	    }
	};
	eventList.addMouseListener(mouseListener);
	eventPane = new JScrollPane(eventList);
	eventListPanel.add("Center", eventPane);

	return eventListPanel;
    }

    // Create the checkboxes panel
    //
    private JPanel createCheckboxesPanel() {
        jmel = new JavaMonitorEventListener(this);

	JPanel checkboxesPanel = new JPanel();
	checkboxesPanel.setLayout(new GridBagLayout());
	addComp(checkboxesPanel, createAWTcheckboxPanel(jmel), 
		0, 0, 4, 4, 0.6, 0.0, new Insets(4, 4, 2, 2));
	addComp(checkboxesPanel, createBeansCheckboxPanel(jmel), 
		0, 4, 4, 1, 0.6, 0.0, new Insets(2, 4, 4, 2));
	addComp(checkboxesPanel, createSwingCheckboxPanel(jmel), 
		5, 0, 3, 5, 0.4, 0.0, new Insets(4, 2, 4, 4));

	return checkboxesPanel;
    }

    // Create the checkboxes panel
    //
    private JPanel createAWTcheckboxPanel(JavaMonitorEventListener jmel) {
        JCheckBox cb;
	GridLayout gl;
        JPanel eventSelector = new JPanel();
        eventSelector.setLayout(gl = new GridLayout(3, 4));
	gl.setVgap(2);
	gl.setHgap(0);
	eventSelector.setBorder(new TitledBorder("AWT events"));

        eventSelector.add(cb = new JCheckBox("Action"));
        cb.addItemListener(new JavaMonitorCommand(EventID.ACTION,jmel));
	cb.requestFocus();

        eventSelector.add(cb = new JCheckBox("Adjustment"));
        cb.addItemListener(new JavaMonitorCommand(EventID.ADJUSTMENT,jmel));

        eventSelector.add(cb = new JCheckBox("Component",false));
        cb.addItemListener(new JavaMonitorCommand(EventID.COMPONENT,jmel));

        eventSelector.add(cb = new JCheckBox("Container",false));
        cb.addItemListener(new JavaMonitorCommand(EventID.CONTAINER,jmel));

        eventSelector.add(cb = new JCheckBox("Focus"));
        cb.addItemListener(new JavaMonitorCommand(EventID.FOCUS,jmel));

        eventSelector.add(cb = new JCheckBox("Item"));
        cb.addItemListener(new JavaMonitorCommand(EventID.ITEM,jmel));

        eventSelector.add(cb = new JCheckBox("Key"));
        cb.addItemListener(new JavaMonitorCommand(EventID.KEY,jmel));

        eventSelector.add(cb = new JCheckBox("Mouse"));
        cb.addItemListener(new JavaMonitorCommand(EventID.MOUSE,jmel));

        eventSelector.add(cb = new JCheckBox("Motion"));
        cb.addItemListener(new JavaMonitorCommand(EventID.MOTION,jmel));

        eventSelector.add(cb = new JCheckBox("Text"));
        cb.addItemListener(new JavaMonitorCommand(EventID.TEXT,jmel));

        eventSelector.add(cb = new JCheckBox("Window"));
	cb.setEnabled(false);
        cb.addItemListener(new JavaMonitorCommand(EventID.WINDOW,jmel));

	return eventSelector;
    }

    // Create the beansCheckboxes panel
    //
    private JPanel createBeansCheckboxPanel(JavaMonitorEventListener jmel) {
        JCheckBox cb;
        JPanel eventSelector = new JPanel();
        eventSelector.setLayout(new GridLayout(1, 2));
	eventSelector.setBorder(new TitledBorder("Beans events"));

        eventSelector.add(cb = new JCheckBox("PropertyChange"));
        cb.addItemListener(new JavaMonitorCommand(EventID.PROPERTYCHANGE,jmel));

        eventSelector.add(cb = new JCheckBox("VetoableChange"));
        cb.addItemListener(new JavaMonitorCommand(EventID.VETOABLECHANGE,jmel));

	return eventSelector;
    }

    // Create the swingCheckboxes panel
    //
    private JPanel createSwingCheckboxPanel(JavaMonitorEventListener jmel) {
        JCheckBox cb;
	GridLayout gl;
        JPanel eventSelector = new JPanel();
        eventSelector.setLayout(gl = new GridLayout(5, 3));
	gl.setVgap(2);
	gl.setHgap(0);
	eventSelector.setBorder(new TitledBorder("Swing events"));

        eventSelector.add(cb = new JCheckBox("Ancestor"));
        cb.addItemListener(new JavaMonitorCommand(EventID.ANCESTOR,jmel));

        eventSelector.add(cb = new JCheckBox("CellEditor"));
        cb.addItemListener(new JavaMonitorCommand(EventID.CELLEDITOR,jmel));

        eventSelector.add(cb = new JCheckBox("Change"));
        cb.addItemListener(new JavaMonitorCommand(EventID.CHANGE,jmel));

        eventSelector.add(cb = new JCheckBox("Document"));
        cb.addItemListener(new JavaMonitorCommand(EventID.DOCUMENT,jmel));

        eventSelector.add(cb = new JCheckBox("Internal Frame"));
        cb.addItemListener(new JavaMonitorCommand(EventID.INTERNALFRAME,jmel));

        eventSelector.add(cb = new JCheckBox("ListData"));
        cb.addItemListener(new JavaMonitorCommand(EventID.LISTDATA,jmel));

        eventSelector.add(cb = new JCheckBox("ListSelection")); 
        cb.addItemListener(new JavaMonitorCommand(EventID.LISTSELECTION,jmel));

        eventSelector.add(cb = new JCheckBox("Menu"));
        cb.addItemListener(new JavaMonitorCommand(EventID.MENU,jmel));

        eventSelector.add(cb = new JCheckBox("PopupMenu"));
        cb.addItemListener(new JavaMonitorCommand(EventID.POPUPMENU,jmel));

        eventSelector.add(cb = new JCheckBox("ColumnModel"));
        cb.addItemListener(new JavaMonitorCommand(EventID.COLUMNMODEL,jmel));

        eventSelector.add(cb = new JCheckBox("TableModel"));
        cb.addItemListener(new JavaMonitorCommand(EventID.TABLEMODEL,jmel));

        eventSelector.add(cb = new JCheckBox("TreeExpansion"));
        cb.addItemListener(new JavaMonitorCommand(EventID.TREEEXPANSION,jmel));

        eventSelector.add(cb = new JCheckBox("TreeModel"));
        cb.addItemListener(new JavaMonitorCommand(EventID.TREEMODEL,jmel));

        eventSelector.add(cb = new JCheckBox("TreeSelection")); 
        cb.addItemListener(new JavaMonitorCommand(EventID.TREESELECTION,jmel));

        eventSelector.add(cb = new JCheckBox("UndoableEdit"));
        cb.addItemListener(new JavaMonitorCommand(EventID.UNDOABLEEDIT,jmel));

	return eventSelector;
    }

    // create the examiner panel
    //
    private JPanel createExaminerPanel() {
	JPanel examinerPanel = new JPanel();
	examinerPanel.setLayout(new GridLayout(1, 2));

	JPanel p;

	focusPanel = new AccessibilityAPIPanel(5);
	p = new JPanel();
	p.setLayout(new BorderLayout());
	p.add("Center", focusPanel);
	p.setBorder(new TitledBorder("Focus detail"));
	examinerPanel.add(p);

	mousePanel = new AccessibilityAPIPanel(5);
	p = new JPanel();
	p.setLayout(new BorderLayout());
	p.add("Center", mousePanel);
	p.setBorder(new TitledBorder("Mouse detail"));
	examinerPanel.add(p);

	return examinerPanel;
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

    public void displayFocusInformation() {
	Accessible a;
	a = Translator.getAccessible(SwingEventMonitor.getComponentWithFocus());

	if (a != null) {
	    AccessibleContext ac = a.getAccessibleContext();
	    if (ac != null) {
	        focusPanel.updateInfo(ac, null);
	    }
        }
    }

    public void displayMouseInformation(Accessible information, Point p) {
	if (information != null) {
	    AccessibleContext ac = information.getAccessibleContext();
	    if (ac != null) {
	        mousePanel.updateInfo(ac, p);
	    }
        }
    }

    public synchronized void displayEvent(EventObject event) {

	// Prevent recursion caused by DataListEvents being
	// fired when items are added to the JavaMonitor's
	// JScrollPane or JList.
	if (displayingEvent) {
	    return;
	} else {
	    displayingEvent = true;
	}

	// add the new element
	eventModel.addElement(event);

	// scroll the list so the new element is visible
	Point p = eventList.indexToLocation(eventModel.getSize());
	Rectangle r = eventPane.getViewport().getViewRect();
        if ((p != null) && (r != null)) {
	    if (p.y > r.height) {
	        p.y -= r.height;
	    }
	    eventPane.getViewport().setViewPosition(p);
        }
	displayingEvent = false;
    }

    public synchronized void displayEvent(String eventString) {

	// Prevent recursion caused by DataListEvents being
	// fired when items are added to the JavaMonitor's
	// JScrollPane or JList.
	if (displayingEvent) {
	    return;
	} else {
	    displayingEvent = true;
	}

	// add the new element
	eventModel.addElement(eventString);

	// scroll the list so the new element is visible
	Point p = eventList.indexToLocation(eventModel.getSize());
	Rectangle r = eventPane.getViewport().getViewRect();
        if (p != null && r != null) {
	    if (p.y > r.height) {
	        p.y -= r.height;
	    }
	    eventPane.getViewport().setViewPosition(p);
        }
	displayingEvent = false;
    }

    static public void main(String args[]) {

        String vers = System.getProperty("java.version");
        if (vers.compareTo("1.1.2") < 0) {
            System.out.println("!!!WARNING: Java Monitor must be run with a " +
                               "1.1.2 or higher version VM!!!");
        }

        new JavaMonitor();
    }

    public void actionPerformed(ActionEvent e) {
        String action = e.getActionCommand();
        Object source = e.getSource();
        if (source instanceof AccessibilityItem) {	// from popup

            Accessible a = ((AccessibilityItem) source).getAccessible();
            if (action == "Accessibility API panel") {
                AccessibilityAPIPanel panel = new AccessibilityAPIPanel();
                InfoPanel infoPanel = new InfoPanel(a, panel);
            } else if (action == "AccessibleAction panel") {
                AccessibleActionPanel panel = new AccessibleActionPanel();
                InfoPanel infoPanel = new InfoPanel(a, panel);
            } else if (action == "AccessibleSelection panel") {
                AccessibleSelectionPanel panel = new AccessibleSelectionPanel();
                InfoPanel infoPanel = new InfoPanel(a, panel);
            } else if (action == "AccessibleHypertext panel") {
                AccessibleHypertextPanel panel = new AccessibleHypertextPanel();
                InfoPanel infoPanel = new InfoPanel(a, panel);
            }

        } else if (source instanceof JMenuItem)  {      // from menu bar

	    int index = eventList.getSelectedIndex();
            if (index < 0) {
                return;
            }
            Object event = eventModel.getElementAt(index);
            if (event instanceof EventObject) {
                Object o = ((EventObject) event).getSource();
                if (o != null && o instanceof Accessible) {
		    Accessible a = (Accessible) o;
		    if (action == "Accessibility API panel") {
			AccessibilityAPIPanel panel =
			    new AccessibilityAPIPanel();
			InfoPanel infoPanel = new InfoPanel(a, panel);
		    } else if (action == "AccessibleAction panel") {
			AccessibleActionPanel panel =
			    new AccessibleActionPanel();
			InfoPanel infoPanel = new InfoPanel(a, panel);
		    } else if (action == "AccessibleSelection panel") {
			AccessibleSelectionPanel panel =
			    new AccessibleSelectionPanel();
			InfoPanel infoPanel = new InfoPanel(a, panel);
		    } else if (action == "AccessibleHypertext panel") {
			AccessibleHypertextPanel panel =
			    new AccessibleHypertextPanel();
			InfoPanel infoPanel = new InfoPanel(a, panel);
		    }
                }
            }
        }
    }

    class AccessibilityItem extends JMenuItem {
        Accessible accessible;
        AccessibilityItem(String s, Accessible a) {
            super(s);
            accessible = a;
        }

        public Accessible getAccessible() {
            return accessible;
        }
    }

    public void selectAccessiblePaneMenu(Accessible a, JList invoker, int x, int y) {
        JPopupMenu menu = new JPopupMenu();
        menu.setInvoker(invoker);
        AccessibilityItem item = new AccessibilityItem("Accessibility API panel", a);
        item.addActionListener(this);
        menu.add(item);

        item = new AccessibilityItem("AccessibleAction panel", a);
        item.addActionListener(this);
        menu.add(item);

        item = new AccessibilityItem("AccessibleSelection panel", a);
        item.addActionListener(this);
        menu.add(item);

        menu.pack();
        menu.show(invoker, x, y);
    }


    public class InfoPanel extends Frame {
        InfoPanel(Accessible a, AccessibilityPanel p) {
            super();
            WindowListener l = new WindowAdapter() {
                public void windowClosing(WindowEvent e) { dispose(); }
            };
            setLayout(new BorderLayout());
            addWindowListener(l);

            // Add AccessiblityPanel
            //
            add("Center", p);
            if (a != null) {
                AccessibleContext ac = a.getAccessibleContext();
                if (ac != null) {
                    p.updateInfo(ac, null);
                    if (p instanceof AccessibilityAPIPanel) {
                        setTitle("API info for: " + ac.getAccessibleName());
                    } else if (p instanceof AccessibleSelectionPanel) {
                        setTitle("Selection info for: " + ac.getAccessibleName()
);
                    } else if (p instanceof AccessibleActionPanel) {
                        setTitle("Action info for: " + ac.getAccessibleName());
                    }
                } else {
                    dispose();
                }
            } else {
                 dispose();
            }
            pack();
            show();
        }
    }

}

class JavaMonitorCommand implements ItemListener {
    int    id;
    JavaMonitorEventListener jmel;

    public JavaMonitorCommand(int id, JavaMonitorEventListener jmel) {
        this.id   = id;
        this.jmel = jmel;
    }

    public void itemStateChanged(ItemEvent e) {
        if (e.getStateChange() == ItemEvent.SELECTED) {
            switch (id) {

            case EventID.COMPONENT:
                SwingEventMonitor.addComponentListener((ComponentListener)jmel);
                break;

            case EventID.CONTAINER:
                SwingEventMonitor.addContainerListener((ContainerListener)jmel);
                break;

            case EventID.FOCUS:
                SwingEventMonitor.addFocusListener((FocusListener)jmel);
                break;

            case EventID.KEY:
                SwingEventMonitor.addKeyListener((KeyListener)jmel);
                break;

            case EventID.MOUSE:
                SwingEventMonitor.addMouseListener((MouseListener)jmel);
                break;

            case EventID.MOTION:
                SwingEventMonitor.addMouseMotionListener((MouseMotionListener)jmel);
                break;
                
            case EventID.WINDOW:
                SwingEventMonitor.addWindowListener((WindowListener)jmel);
                break;
               
            case EventID.ACTION:
                SwingEventMonitor.addActionListener((ActionListener)jmel);
                break;
                
            case EventID.ADJUSTMENT:
                SwingEventMonitor.addAdjustmentListener((AdjustmentListener)jmel);
                break;
                
            case EventID.ITEM:
                SwingEventMonitor.addItemListener((ItemListener)jmel);
                break;

            case EventID.TEXT:
                SwingEventMonitor.addTextListener((TextListener)jmel);
                break;

            case EventID.ANCESTOR:
                SwingEventMonitor.addAncestorListener((AncestorListener)jmel);
                break;

            case EventID.CELLEDITOR:
                SwingEventMonitor.addCellEditorListener((CellEditorListener)jmel);
                break;

            case EventID.CHANGE:
                SwingEventMonitor.addChangeListener((ChangeListener)jmel);
                break;

            case EventID.DOCUMENT:
                SwingEventMonitor.addDocumentListener((DocumentListener)jmel);
                break;

            case EventID.LISTDATA:
                SwingEventMonitor.addListDataListener((ListDataListener)jmel);
                break;

            case EventID.LISTSELECTION:
                SwingEventMonitor.addListSelectionListener((ListSelectionListener)jmel);
                break;

            case EventID.MENU:
                SwingEventMonitor.addMenuListener((MenuListener)jmel);
                break;

            case EventID.COLUMNMODEL:
                SwingEventMonitor.addColumnModelListener((TableColumnModelListener)jmel);
                break;

            case EventID.POPUPMENU:
                SwingEventMonitor.addPopupMenuListener((PopupMenuListener)jmel);
                break;

            case EventID.TABLEMODEL:
                SwingEventMonitor.addTableModelListener((TableModelListener)jmel);
                break;

            case EventID.TREEEXPANSION:
                SwingEventMonitor.addTreeExpansionListener((TreeExpansionListener)jmel);
                break;

            case EventID.TREEMODEL:
                SwingEventMonitor.addTreeModelListener((TreeModelListener)jmel);
                break;

            case EventID.TREESELECTION:
                SwingEventMonitor.addTreeSelectionListener((TreeSelectionListener)jmel);
                break;

            case EventID.UNDOABLEEDIT:
                SwingEventMonitor.addUndoableEditListener((UndoableEditListener)jmel);
                break;

            case EventID.INTERNALFRAME:
                SwingEventMonitor.addInternalFrameListener((InternalFrameListener)jmel);
                break;

            case EventID.PROPERTYCHANGE:
                SwingEventMonitor.addPropertyChangeListener((PropertyChangeListener)jmel);
                break;

            case EventID.VETOABLECHANGE:
                SwingEventMonitor.addVetoableChangeListener((VetoableChangeListener)jmel);
                break;

            default:
                break;
            }
        } else {
            switch (id) {

            case EventID.COMPONENT:
                SwingEventMonitor.removeComponentListener((ComponentListener)jmel);
                break;

            case EventID.CONTAINER:
                SwingEventMonitor.removeContainerListener((ContainerListener)jmel);
                break;

            case EventID.FOCUS:
                SwingEventMonitor.removeFocusListener((FocusListener)jmel);
                break;

            case EventID.KEY:
                SwingEventMonitor.removeKeyListener((KeyListener)jmel);
                break;

            case EventID.MOUSE:
                SwingEventMonitor.removeMouseListener((MouseListener)jmel);
                break;

            case EventID.MOTION:
                SwingEventMonitor.removeMouseMotionListener((MouseMotionListener)jmel);
                break;

            case EventID.WINDOW:
                SwingEventMonitor.removeWindowListener((WindowListener)jmel);
                break;

            case EventID.ACTION:
                SwingEventMonitor.removeActionListener((ActionListener)jmel);
                break;

            case EventID.ADJUSTMENT:
                SwingEventMonitor.removeAdjustmentListener((AdjustmentListener)jmel);
                break;

            case EventID.ITEM:
                SwingEventMonitor.removeItemListener((ItemListener)jmel);
                break;

            case EventID.TEXT:
                SwingEventMonitor.removeTextListener((TextListener)jmel);
                break;

            case EventID.ANCESTOR:
                SwingEventMonitor.removeAncestorListener((AncestorListener)jmel);
                break;

            case EventID.CELLEDITOR:
                SwingEventMonitor.removeCellEditorListener((CellEditorListener)jmel);
                break;

            case EventID.CHANGE:
                SwingEventMonitor.removeChangeListener((ChangeListener)jmel);
                break;

            case EventID.DOCUMENT:
                SwingEventMonitor.removeDocumentListener((DocumentListener)jmel);
                break;

            case EventID.LISTDATA:
                SwingEventMonitor.removeListDataListener((ListDataListener)jmel);
                break;

            case EventID.LISTSELECTION:
                SwingEventMonitor.removeListSelectionListener((ListSelectionListener)jmel);
                break;

            case EventID.MENU:
                SwingEventMonitor.removeMenuListener((MenuListener)jmel);
                break;

            case EventID.COLUMNMODEL:
                SwingEventMonitor.removeColumnModelListener((TableColumnModelListener)jmel);
                break;

            case EventID.TREEEXPANSION:
                SwingEventMonitor.removeTreeExpansionListener((TreeExpansionListener)jmel);
                break;

            case EventID.TREEMODEL:
                SwingEventMonitor.removeTreeModelListener((TreeModelListener)jmel);
                break;

            case EventID.TREESELECTION:
                SwingEventMonitor.removeTreeSelectionListener((TreeSelectionListener)jmel);
                break;

            case EventID.UNDOABLEEDIT:
                SwingEventMonitor.removeUndoableEditListener((UndoableEditListener)jmel);
                break;

            case EventID.INTERNALFRAME:
                SwingEventMonitor.removeInternalFrameListener((InternalFrameListener)jmel);
                break;

            case EventID.PROPERTYCHANGE:
                SwingEventMonitor.removePropertyChangeListener((PropertyChangeListener)jmel);
                break;

            case EventID.VETOABLECHANGE:
                SwingEventMonitor.removeVetoableChangeListener((VetoableChangeListener)jmel);
                break;

            default:
                break;
            }
        }
    }
}

class JavaMonitorEventListener implements ComponentListener, ContainerListener,
    FocusListener, KeyListener, MouseListener, MouseMotionListener,
    WindowListener, ActionListener,AdjustmentListener,ItemListener,
    TextListener, AncestorListener, CellEditorListener, ChangeListener, 
    DocumentListener, ListDataListener, ListSelectionListener, 
    MenuListener, PopupMenuListener, TableColumnModelListener, 
    TableModelListener, TreeExpansionListener, TreeModelListener, 
    TreeSelectionListener, UndoableEditListener, 
    PropertyChangeListener, VetoableChangeListener, InternalFrameListener  {

    JavaMonitor app;

    public JavaMonitorEventListener(JavaMonitor app) {
        this.app = app;
    }

        /* ActionListener Methods ***************************************/
  
    public void actionPerformed(ActionEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* AdjustmentListener Methods ***********************************/

    public void adjustmentValueChanged(AdjustmentEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* ItemListener Methods *****************************************/

    public void itemStateChanged(ItemEvent theEvent) {
        app.displayEvent(theEvent);
    }

       /* ComponentListener Methods ************************************/
  
    public void componentMoved(ComponentEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void componentResized(ComponentEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void componentShown(ComponentEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void componentHidden(ComponentEvent theEvent) {
        app.displayEvent(theEvent);
    }

       /* ContainerListener Methods ************************************/

    public void componentAdded(ContainerEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void componentRemoved(ContainerEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* FocusListener Methods ****************************************/
  
    public void focusGained(FocusEvent theEvent) {
        app.displayFocusInformation();
        app.displayEvent(theEvent);
    }

    public void focusLost(FocusEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* KeyListener Methods ******************************************/
  
    public void keyTyped(KeyEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void keyPressed(KeyEvent theEvent) {
        String statusString = null;
	Point containerPoint = null;
        if (theEvent.getKeyCode() == KeyEvent.VK_F1) {
            Point currentMousePos = EventQueueMonitor.getCurrentMousePosition();
	    Accessible a = null;
  	    if (app.activeMenu != null) {
		JPopupMenu pm = app.activeMenu.getPopupMenu();
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
		if (ac != null) {
		    AccessibleComponent acmp = ac.getAccessibleComponent();
		    if (acmp != null) {
		        Point containerLoc = acmp.getLocationOnScreen();
		        containerPoint = 
			        new Point(currentMousePos.x - containerLoc.x,
				          currentMousePos.y - containerLoc.y);
		    }
		}
		app.displayMouseInformation(a, containerPoint);
	    }
        }
        app.displayEvent(theEvent);
    }

    public void keyReleased(KeyEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* MouseListener Methods ****************************************/
  
    public void mouseClicked(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void mousePressed(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void mouseReleased(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void mouseEntered(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void mouseExited(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* MouseMotionListener Methods **********************************/
  
    public void mouseDragged(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void mouseMoved(MouseEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowOpened(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* TextListener Methods *****************************************/
  
    public void textValueChanged(TextEvent theEvent) {
        app.displayEvent(theEvent);
    }
  
        /* WindowListener Methods ***************************************/
  
    public void windowClosing(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowClosed(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowIconified(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowDeiconified(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowActivated(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void windowDeactivated(WindowEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* AncestorListener Methods ******************************************/

    public void ancestorAdded(AncestorEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void ancestorRemoved(AncestorEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void ancestorMoved(AncestorEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* CellEditorListener Methods *****************************************/

    public void editingStarted(ChangeEvent theEvent) {
	app.displayEvent(theEvent);
    }

    public void editingStopped(ChangeEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void editingCanceled(ChangeEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* ChangeListener Methods *****************************************/

    public void stateChanged(ChangeEvent theEvent) {
        app.displayEvent(theEvent);
    }
    

        /* DocumentListener Methods **************************************/
  
    public void changedUpdate(DocumentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void insertUpdate(DocumentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }

    public void removeUpdate(DocumentEvent theEvent) {
        app.displayEvent(theEvent.toString());
    }
    

        /* ListSelectionListener Methods ***********************************/
  
    public void valueChanged(ListSelectionEvent theEvent) {
        app.displayEvent(theEvent);
    }
    

        /* ListDataListener Methods *****************************************/
  
    public void contentsChanged(ListDataEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void intervalAdded(ListDataEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void intervalRemoved(ListDataEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* MenuListener Methods *****************************************/
 
    public void menuCanceled(MenuEvent theEvent) {
        app.activeMenu = null;
        app.displayEvent(theEvent);
    }

    public void menuDeselected(MenuEvent theEvent) {
        app.activeMenu = null;
        app.displayEvent(theEvent);
    }

    public void menuSelected(MenuEvent theEvent) {
        app.activeMenu = (JMenu) theEvent.getSource();
        app.displayEvent(theEvent);
    }


        /* TableColumnModelListener Methods *******************************/
 
    public void columnAdded(TableColumnModelEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void columnMarginChanged(ChangeEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void columnMoved(TableColumnModelEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void columnRemoved(TableColumnModelEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void columnSelectionChanged(ListSelectionEvent theEvent) {
        app.displayEvent(theEvent);
    }

 
        /* PopupMenuListener Methods **************************************/

    public void popupMenuWillBecomeVisible(PopupMenuEvent theEvent) {
	app.displayEvent(theEvent);
    }

    public void popupMenuWillBecomeInvisible(PopupMenuEvent theEvent) {
	app.displayEvent(theEvent);
    }

    public void popupMenuCanceled(PopupMenuEvent theEvent) {
	app.displayEvent(theEvent);
    }

 
        /* TableModelListener Methods **************************************/

    public void tableChanged(TableModelEvent theEvent) {
	app.displayEvent(theEvent);
    }

    public void tableRowsInserted(TableModelEvent theEvent) {
	app.displayEvent(theEvent);
    }

    public void tableRowsRemoved(TableModelEvent theEvent) {
	app.displayEvent(theEvent);
    }

        /* TreeExpansionListener Methods **********************************/
  
    public void treeCollapsed(TreeExpansionEvent theEvent) {
        app.displayEvent(theEvent);
    }
  
    public void treeExpanded(TreeExpansionEvent theEvent) {
        app.displayEvent(theEvent);
    }

  
        /* TreeModelListener Methods **********************************/
 
    public void treeNodesChanged(TreeModelEvent theEvent) {
        app.displayEvent(theEvent);
    }
  
    public void treeNodesInserted(TreeModelEvent theEvent) {
        app.displayEvent(theEvent);
    }
  
    public void treeNodesRemoved(TreeModelEvent theEvent) {
        app.displayEvent(theEvent);
    }

    public void treeStructureChanged(TreeModelEvent theEvent) {
        app.displayEvent(theEvent);
    }  

        /* TreeSelectionListener Methods ***********************************/
  
    public void valueChanged(TreeSelectionEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* UndoableEditListener Methods **************************************/

    public void undoableEditHappened(UndoableEditEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* InternalFrame Methods **********************************/

    public void internalFrameOpened(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameActivated(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameDeactivated(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameIconified(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameDeiconified(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameClosing(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }
	
    public void internalFrameClosed(InternalFrameEvent theEvent) {
        app.displayEvent(theEvent);
    }

        /* PropertyChangeListener Methods **********************************/
  
    public void propertyChange(PropertyChangeEvent theEvent) {
        app.displayEvent(theEvent);
    }


        /* VetoableChangeListener Methods **********************************/

    public void vetoableChange(PropertyChangeEvent theEvent) 
	    throws PropertyVetoException {
        app.displayEvent(theEvent);
    }
}

