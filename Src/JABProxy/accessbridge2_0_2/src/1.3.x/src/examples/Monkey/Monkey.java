/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Monkey.java	1.29 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.applet.Applet;
import javax.swing.*;
import javax.swing.tree.*;
import javax.swing.event.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>Monkey is an example assistive technology that uses the EventQueueMonitor
 * utility class from com.sun.java.accessibility.util.  Like a Monkey, it runs 
 * around in trees.  In particular, it runs around the Component tree in an 
 * application running in the same VM as the Monkey.  On the left hand side 
 * of Monkey is a tree that represents the Component hierarchy.  On the right 
 * hand side of Monkey is the same tree, but viewed as though the components 
 * are Accessible objects.  In order to tell Monkey to learn about an updated 
 * tree, you need to tell it to "Refresh Trees," which is located under 
 * the "File" menu. 
 *
 * <P>In the display, a '*' at the beginning of each object's name means it
 * implements interface Accessible.
 *
 * <P>For now, it is meant to run in the same Java Virtual Machine as 
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following 
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=Monkey
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
 *
 * @version     1.29 01/17/02 16:10:21
 * @author      Willie Walker
 */
public class Monkey implements TreeSelectionListener, TreeExpansionListener,
    ActionListener, KeyListener, GUIInitializedListener {

    public static int WIDTH = 600;
    public static int HEIGHT = 400;
    public JFrame frame;
    public JTree componentTree; 
    public JScrollPane componentScrollPane;
    public JTree accessibleTree;
    public JScrollPane accessibleScrollPane;
    Component lastComponent = null;
    Accessible lastAccessible = null;

    public void refreshTrees() {
	componentScrollPane.getViewport().remove(componentTree);
	accessibleScrollPane.getViewport().remove(accessibleTree);
        componentTree = createComponentTree();
        accessibleTree = createAccessibleTree();
	componentScrollPane.getViewport().add(componentTree);
	accessibleScrollPane.getViewport().add(accessibleTree);
	// allow JSplitPane to always work
	Dimension minSize = new Dimension(5, 5);
	componentScrollPane.setMinimumSize(minSize);
	accessibleScrollPane.setMinimumSize(minSize);
	// force redrawing of trees
	componentTree.revalidate();
	accessibleTree.revalidate();
    }

    JMenuBar createMenuBar() {
	JMenuBar menuBar = new JMenuBar();
	JMenuItem mi;

	// File Menu
	JMenu file = (JMenu) menuBar.add(new JMenu("File"));
	file.setMnemonic('F');
        mi = (JMenuItem) file.add(new JMenuItem("Refresh Trees"));
	mi.setMnemonic('R');
	mi.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e) {
		refreshTrees();
	    }
	});

        file.add(new JSeparator());
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

    public void addComponentNodes(Component c, ComponentNode root) {
        String name;
        ComponentNode me;
        me = new ComponentNode(c);
        root.add(me);
        if (c instanceof Container) {
            int count = ((Container) c).getComponentCount();
            for (int i = 0; i < count; i++) {
                addComponentNodes(((Container) c).getComponent(i), me);
            }
        }
    }

    static private int depth = 0;
    static private final int MAX_DEPTH = 12;

    public void addAccessibleNodes(Accessible a, AccessibleNode root) {
        // avoid recursing too deep
	if (depth >= MAX_DEPTH) {
	    return;
	}
	depth++;

        AccessibleNode me;
        me = new AccessibleNode(a);
        root.add(me);
	AccessibleContext ac = me.accessible.getAccessibleContext();
	if (ac != null) {
	    Accessible ap = ac.getAccessibleParent();
	    if (ap != root.accessible && root.accessible != null) {
	        System.out.println("OOPS.  Accessible parent of " + a
		        + " should be " + root.accessible + " but it is " +
		        ap);
	    }
            int count = ac.getAccessibleChildrenCount();
            for (int i = 0; i < count; i++) {
                addAccessibleNodes(ac.getAccessibleChild(i), me);
            }
	}
        depth--;
    }

    public JTree createComponentTree() {
        JTree t;
        Window[] wins = EventQueueMonitor.getTopLevelWindows();
	ComponentNode root = new ComponentNode("Component Tree");
        for (int i = 0; i < wins.length; i++) {
            addComponentNodes(wins[i], root);
        }        
	t = new JTree(root);
	t.addTreeSelectionListener(this);
	t.addTreeExpansionListener(this);
	return t;
    }

    public JTree createAccessibleTree() {
	final JTree t;
        Window[] wins = EventQueueMonitor.getTopLevelWindows();

	AccessibleNode root = new AccessibleNode("Accessible Tree");
	Accessible accessible;
        for (int i = 0; i < wins.length; i++) {
	    accessible = Translator.getAccessible(wins[i]);
            if (accessible != null) {
                addAccessibleNodes(accessible, root);
	    }
        }        

	t = new JTree(root);
	t.addTreeSelectionListener(this);
	t.addTreeExpansionListener(this);

	MouseListener ml = new MouseAdapter() {
	    public void mousePressed(MouseEvent e) {
//		if (e.isPopupTrigger()) {  // broken in JDK 1.1.5 on NT
		if (e.getModifiers() == InputEvent.BUTTON3_MASK) {
		    TreePath selPath = t.getPathForLocation(e.getX(), e.getY());
		    if(selPath != null) {
			Object o = selPath.getLastPathComponent();
			if (o instanceof AccessibleNode) {
			    Accessible a = ((AccessibleNode) o).accessible;
			    if (a != null) {
				selectAccessiblePaneMenu(a, t, e.getX(), e.getY());
			    }
			} else {
			    System.out.println("*** Whops!  not an AccessibleNode here");
			}
		    } 
		}
	    }
	};

	t.addMouseListener(ml);

	return t;
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
			setTitle("Selection info for: " + ac.getAccessibleName());
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

    public void actionPerformed(ActionEvent e) {
	String action = e.getActionCommand();
	Object source = e.getSource();
	if (source instanceof AccessibilityItem) {	// from popup

	    action = ((AccessibilityItem) source).getText();
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

	    action = ((JMenuItem) source).getText();
	    Object o = accessibleTree.getLastSelectedPathComponent();
	    if (o != null && o instanceof AccessibleNode) {
		Accessible a = ((AccessibleNode) o).accessible;
		if (a != null) {
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

    public void selectAccessiblePaneMenu(Accessible a, JTree invoker, int x, int y) {
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

	item = new AccessibilityItem("AccessibleHypertext panel", a);
	item.addActionListener(this);
	menu.add(item);

	menu.pack();
	menu.show(invoker, x, y);
    }

    public Monkey() {
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

        frame = new JFrame("Monkey");
	frame.getContentPane().setLayout(new BorderLayout());
	frame.addWindowListener(l);
	frame.getContentPane().add("North", createMenuBar());

        JSplitPane pane = new JSplitPane();
        //pane.setLayout(new GridLayout(1,2));
        frame.getContentPane().add("Center", pane);

        componentTree = createComponentTree();
	componentScrollPane = new JScrollPane();
	componentScrollPane.getViewport().add(componentTree);
	pane.setLeftComponent(componentScrollPane);

        accessibleTree = createAccessibleTree();
	accessibleScrollPane = new JScrollPane();
	accessibleScrollPane.getViewport().add(accessibleTree);
	pane.setRightComponent(accessibleScrollPane);

	SwingEventMonitor.addKeyListener(this); // Respond to F2

	frame.setSize(WIDTH, HEIGHT);
	frame.show();
	//frame.validate();
	//frame.repaint();
	accessibleTree.requestFocus();
    }

    public void showComponentBounds(Component c) {
        Graphics g;
        Rectangle b;

	if (c != null) {
            b = c.getBounds();
	    Container parent = c.getParent();
	    if (parent != null) {
                g = parent.getGraphics();
        	if (g != null) {
                    g.setXORMode(Color.pink);
                    g.setColor(Color.black);
                    g.fillRect(b.x, b.y, b.width, b.height);
	        }
            } else {
                g = c.getGraphics();
        	if (g != null) {
                    g.setXORMode(Color.red);
                    g.setColor(Color.black);
                    g.fillRect(0, 0, b.width, b.height);
	        }
            }
        }
    }

    public void showAccessibleBounds(Accessible a) {
	Object parent;
        Component c;
        Graphics g;
        Rectangle b;

	if (a != null) {
	    AccessibleContext ac = a.getAccessibleContext();
	    if (ac == null) {
		return;
	    }

	    AccessibleComponent acmp = ac.getAccessibleComponent();
	    if (acmp == null) {
		return;
	    }

            b = acmp.getBounds();
	
            // This little hack is to get the graphics context
            // of the actual parent rather than the accessible
            // parent if we can.
	    if (a instanceof Component) {
		parent = ((Component) a).getParent();
	    } else {
   	        parent = ac.getAccessibleParent();
	    }
	    if (parent instanceof Component) {
                g = ((Component) parent).getGraphics();
        	if (g != null) {
                    g.setXORMode(Color.pink);
                    g.setColor(Color.black);
                    g.fillRect(b.x, b.y, b.width, b.height);
	        }
            } else if (a instanceof Component) {
                g = ((Component) a).getGraphics();
        	if (g != null) {
                    g.setXORMode(Color.red);
                    g.setColor(Color.black);
                    g.fillRect(0, 0, b.width, b.height);
	        }
            }

        }
    }

    public void valueChanged(TreeSelectionEvent e) {
	Object[] path = e.getPath().getPath();

        // Erase
	showComponentBounds(lastComponent);
	lastComponent = null;
	showAccessibleBounds(lastAccessible);
	lastAccessible = null;

        // Draw
        if (e.getSource() == componentTree) {
            lastComponent = ((ComponentNode) path[path.length - 1]).component;
	    showComponentBounds(lastComponent);
	} else if (e.getSource() == accessibleTree) {
	    lastAccessible = ((AccessibleNode) path[path.length - 1]).accessible;
	    showAccessibleBounds(lastAccessible);
	}
    }

    public void treeCollapsed(TreeExpansionEvent e) {
    }

    public void treeExpanded(TreeExpansionEvent e) {
	Object[] path = e.getPath().getPath();
    }

    // Handle Function Key
    //
    public void keyTyped(KeyEvent theEvent) {
    }

    public void keyReleased(KeyEvent theEvent) {
    }

    public void keyPressed(KeyEvent theEvent) {
	if (theEvent.getKeyCode() == KeyEvent.VK_F2) {
	    refreshTrees();
	}
    }

    public static void main(String s[]) {

        String vers = System.getProperty("java.version");
        if (vers.compareTo("1.1.2") < 0) {
            System.out.println("!!!WARNING: Monkey must be run with a " +
                               "1.1.2 or higher version VM!!!");
        }

	new Monkey();
    }
}

