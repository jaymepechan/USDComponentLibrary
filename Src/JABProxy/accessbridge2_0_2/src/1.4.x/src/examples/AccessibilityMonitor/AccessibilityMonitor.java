/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibilityMonitor.java	1.10 02/01/17
 */

import java.util.*;
import java.beans.*;
import java.awt.*;
import java.awt.event.*;

import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.swing.table.*;
import javax.swing.undo.*;
import javax.swing.border.*;
import javax.swing.table.*;
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
 * @version     1.10 01/17/02 16:11:22
 * @author      Peter Korn
 */
public class AccessibilityMonitor 
    implements ActionListener, PropertyChangeListener, GUIInitializedListener {

    public static int WIDTH = 680;
    public static int HEIGHT = 500;
    public JFrame frame;

    protected JMenu activeMenu = null;

    UpdatingAbstractTableModel eventModel;
    JTable eventTable;
    Vector events;
    NoPropertyJList eventList;
    NoPropertyJScrollPane eventPane;


    // main
    //
    static public void main(String args[]) {
        String vers = System.getProperty("java.version");
        if (vers.compareTo("1.1.2") < 0) {
            System.out.println("!!!WARNING: AccessibilityMonitor must be run" +
                               "with a 1.1.2 or higher version VM!!!");
        }
        new AccessibilityMonitor();
    }

    // Constructor
    //
    public AccessibilityMonitor() {
        if (EventQueueMonitor.isGUIInitialized()) {
	    createGUI();
	} else {
	    EventQueueMonitor.addGUIInitializedListener(this);
        }
    }

    // GUIInitializedListener method 
    public void guiInitialized() {
        createGUI();
    }

    // Create the GUI
    //
    private void createGUI() {
        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        frame = new JFrame("Accessibility Monitor");
        frame.getContentPane().setLayout(new BorderLayout());
        frame.addWindowListener(l);
        frame.getContentPane().add("North", createMenuBar());
	JPanel p = createEventPanel();
        frame.getContentPane().add("Center", p);

        frame.setSize(WIDTH, HEIGHT);
        frame.show();
	p.requestFocus();
    }

    // Create the Menu Bar
    //
    private JMenuBar createMenuBar() {
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

    // Create the GUI
    //
    private JPanel createEventPanel() {
	JPanel eventPanel = new JPanel();
	eventPanel.setLayout(new BorderLayout());
	eventPanel.add("North", createCheckboxesPanel());
	eventPanel.add("Center", createEventListPanel());

	return eventPanel;
    }

    // Create the checkboxes panel
    //
    private JPanel createCheckboxesPanel() {
	JCheckBox cb;
	JPanel checkboxesPanel = new JPanel();
	checkboxesPanel.setLayout(new BorderLayout());
	checkboxesPanel.add("Center", cb = new JCheckBox("Accessibility PropertyChange Events"));
	cb.setMnemonic('C');
	cb.addItemListener(new ItemListener() {
	    public void itemStateChanged(ItemEvent e) {
		if (e.getStateChange() == ItemEvent.SELECTED) {
		    AccessibilityEventMonitor.addPropertyChangeListener(AccessibilityMonitor.this);
		} else {
		    AccessibilityEventMonitor.removePropertyChangeListener(AccessibilityMonitor.this);
		}
	    }
	});
	return checkboxesPanel;
    }
    
    // AccessibleValue string format
    //
    class AccessibleValueString {
        Number min;
        Number current;
        Number max;

        AccessibleValueString(Number min, Number current, Number max) {
	    this.min = min;
	    this.current = current;
	    this.max = max;
        }

        public String toString() {
	    return "min="+min+", current="+current+", max="+max;
        }
    }
    
    // Create the event listing panel
    //
    private JScrollPane createEventListPanel() {
	// Create the event table headers
	final String[] headers = {"Name", "Type", "Old value", "New value"};

	// Create vector of events
	events = new Vector();

        // Create a model of the data.
        eventModel = new UpdatingAbstractTableModel() {
            public int getColumnCount() { return headers.length; }
            public int getRowCount() { return events.size();}
            public Object getValueAt(int row, int col) {
		Object o = events.elementAt(row);
		if (o instanceof PropertyChangeEvent) {
		    PropertyChangeEvent e = (PropertyChangeEvent) o;
		    switch (col) {
		    case 0:
			Object src = e.getSource();
			if (src instanceof AccessibleContext) {
			    return ((AccessibleContext) src).getAccessibleName();
			} else {
			    return null;
			}
		    case 1:
			return e.getPropertyName();
		    case 2:
		        Object val = e.getOldValue();
			if (val instanceof AccessibleValue) {
			    AccessibleValue accessVal = (AccessibleValue)val;
			    return new AccessibleValueString(accessVal.getMinimumAccessibleValue(),
							     accessVal.getCurrentAccessibleValue(),
							     accessVal.getMaximumAccessibleValue());
			} else {
			    return val;
			}
		    case 3:
		        val = e.getNewValue();
			if (val instanceof AccessibleValue) {
			    AccessibleValue accessVal = (AccessibleValue)val;
			    return new AccessibleValueString(accessVal.getMinimumAccessibleValue(),
							     accessVal.getCurrentAccessibleValue(),
							     accessVal.getMaximumAccessibleValue());
			} else {
			    return val;
			}
		    default:
			return null;
		    }
		} else {
		    return null;
		}
	    }
            public String getColumnName(int column) {return headers[column];}
            public boolean isCellEditable(int row, int col) {return false;}
        };


        // Create the table
        eventTable = new JTable(eventModel);

        MouseListener mouseListener = new MouseAdapter() {
            public void mouseClicked(MouseEvent e) {
                if (e.getClickCount() == 2) {
		    int column = eventTable.columnAtPoint(e.getPoint());
                    int row = eventTable.rowAtPoint(e.getPoint());
		    switch(column) {  
		    case 0:	// click over "Name"
			Object event = events.elementAt(row);
			if (event instanceof EventObject) {
			    Object o = ((EventObject) event).getSource();
			    if (o instanceof AccessibleContext) {
				AccessibilityAPIPanel panel =
				    new AccessibilityAPIPanel();
				InfoPanel infoPanel =
				    new InfoPanel((AccessibleContext) o, panel);
			    }
			}
			break;
		    case 2:
		    case 3:
			Object value = eventModel.getValueAt(row, column);
			if (value instanceof AccessibleContext) {
			    AccessibilityAPIPanel panel =
				new AccessibilityAPIPanel();
			    InfoPanel infoPanel =
				new InfoPanel((AccessibleContext) value, panel);
			}
		    case 1:
		    default:
			return;
		    }
                }
            }
            public void mousePressed(MouseEvent e) {
                if (e.getModifiers() == InputEvent.BUTTON3_MASK) {
		    int column = eventTable.columnAtPoint(e.getPoint());
                    int row = eventTable.rowAtPoint(e.getPoint());
		    switch(column) {  
		    case 0:	// click over "Name"
			Object event = events.elementAt(row);
			if (event instanceof EventObject) {
			    Object o = ((EventObject) event).getSource();
			    if (o instanceof AccessibleContext) {
				selectAccessiblePaneMenu((AccessibleContext) o,
				    eventTable, e.getX(), e.getY());
			    }
			}
			break;
		    case 2:	// click over "Old value"
		    case 3:	// click over "New value"
			Object value = eventModel.getValueAt(row, column);
			if (value instanceof AccessibleContext) {
			    selectAccessiblePaneMenu((AccessibleContext) value,
				eventTable, e.getX(), e.getY());
			}
			break;
		    case 1:
		    default:
			return;
                    }
                }
            }
        };
        eventTable.addMouseListener(mouseListener);

	eventPane = new NoPropertyJScrollPane(eventTable);
	return eventPane;
    }

    // Display the event object in the eventList
    //
    public void displayEvent(EventObject event) {
	// add the new element
	events.addElement(event);
	int lastRow = events.size()-1;
	eventModel.tableRowsInserted(lastRow, lastRow);

	// scroll the list so the new element is visible
	Rectangle cellRect = eventTable.getCellRect(lastRow, 1, true);
	eventPane.getViewport().setViewPosition(cellRect.getLocation());
    }

    // Handle selection from Accessibility Panel menu
    //
    public void actionPerformed(ActionEvent e) {
        String action = e.getActionCommand();
        Object source = e.getSource();
        if (source instanceof AccessibilityItem) {
            AccessibleContext ac = ((AccessibilityItem) source).getAccessibleContext();
            if (action == "Accessibility API panel") {
                AccessibilityAPIPanel panel = new AccessibilityAPIPanel();
                InfoPanel infoPanel = new InfoPanel(ac, panel);
            } else if (action == "AccessibleAction panel") {
                AccessibleActionPanel panel = new AccessibleActionPanel();
                InfoPanel infoPanel = new InfoPanel(ac, panel);
            } else if (action == "AccessibleSelection panel") {
                AccessibleSelectionPanel panel = new AccessibleSelectionPanel();
                InfoPanel infoPanel = new InfoPanel(ac, panel);
            } else if (action == "AccessibleHypertext panel") {
                AccessibleHypertextPanel panel = new AccessibleHypertextPanel();
                InfoPanel infoPanel = new InfoPanel(ac, panel);
            }

        } else if (source instanceof JMenuItem)  {      // from menu bar

	    AccessibleContext ac = null;
            action = ((JMenuItem) source).getText();
	    int row = eventTable.getSelectedRow();
	    int column = eventTable.getSelectedColumn();
	    switch(column) {  
	    case 0:	// click over "Name"
		Object event = events.elementAt(row);
		if (event instanceof EventObject) {
		    Object o = ((EventObject) event).getSource();
		    if (o instanceof AccessibleContext) {
			ac = (AccessibleContext) o;
		    }
		}
		break;
	    case 2:	// click over "Old value"
	    case 3:	// click over "New value"
		Object value = eventModel.getValueAt(row, column);
		if (value instanceof AccessibleContext) {
		    ac = (AccessibleContext) value;
		}
		break;
	    case 1:
	    default:
		return;
	    }
	    
	    if (ac != null) {
		if (action == "Accessibility API panel") {
		    AccessibilityAPIPanel panel =
			new AccessibilityAPIPanel();
		    InfoPanel infoPanel = new InfoPanel(ac, panel);
		} else if (action == "AccessibleAction panel") {
		    AccessibleActionPanel panel =
			new AccessibleActionPanel();
                    InfoPanel infoPanel = new InfoPanel(ac, panel);
                } else if (action == "AccessibleSelection panel") {
                    AccessibleSelectionPanel panel =
                        new AccessibleSelectionPanel();
                    InfoPanel infoPanel = new InfoPanel(ac, panel);
                } else if (action == "AccessibleHypertext panel") {
                    AccessibleHypertextPanel panel =
                        new AccessibleHypertextPanel();
                    InfoPanel infoPanel = new InfoPanel(ac, panel);
                }
            }
        }
    }

    // Menu Item containing an Accessibility InfoPanel
    //
    class AccessibilityItem extends JMenuItem {
        AccessibleContext accessibleContext;
        AccessibilityItem(String s, AccessibleContext ac) {
            super(s);
            accessibleContext = ac;
        }

        public AccessibleContext getAccessibleContext() {
            return accessibleContext;
        }
    }

    // Menu to choose an Accessibility InfoPanel from
    //
    public void selectAccessiblePaneMenu(AccessibleContext ac, JTable invoker, 
					 int x, int y) {
        JPopupMenu menu = new JPopupMenu();
        menu.setInvoker(invoker);
        AccessibilityItem item = new AccessibilityItem("Accessibility API panel", ac);
        item.addActionListener(this);
        menu.add(item);

        item = new AccessibilityItem("AccessibleAction panel", ac);
        item.addActionListener(this);
        menu.add(item);

        item = new AccessibilityItem("AccessibleSelection panel", ac);
        item.addActionListener(this);
        menu.add(item);

        item = new AccessibilityItem("AccessibleHypertext panel", ac);
        item.addActionListener(this);
        menu.add(item);

        menu.pack();
        menu.show(invoker, x, y);
    }


    // Create the window containing the detailed Accessibility info
    //
    public class InfoPanel extends Frame {
        InfoPanel(AccessibleContext ac, AccessibilityPanel p) {
            super();
            WindowListener l = new WindowAdapter() {
                public void windowClosing(WindowEvent e) { dispose(); }
            };
            setLayout(new BorderLayout());
            addWindowListener(l);

            // Add AccessiblityPanel
            //
            add("Center", p);
            if (ac != null) {
                p.updateInfo(ac, null);
                if (p instanceof AccessibilityAPIPanel) {
                    setTitle("API info for: " + ac.getAccessibleName());
                } else if (p instanceof AccessibleSelectionPanel) {
                    setTitle("Selection info for: " + ac.getAccessibleName());
                } else if (p instanceof AccessibleActionPanel) {
                    setTitle("Action info for: " + ac.getAccessibleName());
                }
            } else {
                dispose();
            }
            pack();
            show();
        }
    }

        /* PropertyChangeListener Methods *******************************/
  
    public void propertyChange(PropertyChangeEvent theEvent) {
	displayEvent(theEvent);
    }

    // A special JList which doesn't generate Accessible PropertyChange
    // events - else we'd stack overflow when whenever this JList
    // was updated - 'cause it'd cause a new PropertyChange event for
    // the AccessbleJList inner class, requiring another JList update,
    // generating a new PropertyChange event, requiring another...  well,
    // you get the idea.
    //
    public class NoPropertyJList extends JList {
	public NoPropertyJList(DefaultListModel dlm) {
	    super(dlm);
	}

	public AccessibleContext getAccessibleContext() {
	    return null;
//	    System.out.println(" [In NoPropertyJList.getAccessibleContext()]");
//
//	    if (accessibleContext == null) {
//
//		System.out.println("  -> creating a new AccessibleNoPropertyJList()");
//
//		accessibleContext = new AccessibleNoPropertyJList();
////		accessibleContext = new AccessibleJList();
////		accessibleContext = new AccessibleJList() {
////		    public void addPropertyChangeListener(PropertyChangeListener l) {
////		    } 
////		    public void removePropertyChangeListener(PropertyChangeListener l) {
////		    }
////		};
//
//		System.out.println("  -> created a new AccessibleNoPropertyJList()");
//
//	    }
//	    return accessibleContext;
	}

	// Override add/removePropertyChangeListener to do nothing.
	// Otherwise, leave the rest of the JList accessibility support
	// intact.
	//
	protected class AccessibleNoPropertyJList extends AccessibleJList {
	    public AccessibleNoPropertyJList() {
		super();
	    }
	    public void addPropertyChangeListener(PropertyChangeListener l) {
	    } 
	    public void removePropertyChangeListener(PropertyChangeListener l) {
	    }
	}
    }


    // A special JScrollPane which isn't accessible - else we'd hang 
    // whenever this JScrollPane was updated - 'cause it'd cause a new 
    // PropertyChange event for the AccessbleJScrollPane inner class, 
    // requiring another JScrollPane update, generating a new 
    // PropertyChange event, requiring another...  well, you get the idea.
    //
    public class NoPropertyJScrollPane extends JScrollPane {
	public NoPropertyJScrollPane(Component c) {
	    super(c);
	}
	public AccessibleContext getAccessibleContext() {
	    return null;
	}
    }

    // A special AbstractTableModel that has a public method for notifying
    // listeners that rows were inserted
    //
    abstract public class UpdatingAbstractTableModel extends AbstractTableModel {
	public void tableRowsInserted(int start, int end) {
	    fireTableRowsInserted(start, end);
	}
    }
}

