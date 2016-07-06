/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibleSelectionPanel.java	1.6 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;


public class AccessibleSelectionPanel extends Panel implements ActionListener {

    java.awt.List childrenList;
    java.awt.List selectedList;
    Label accessibleLabel;
    Label selectionCount;
    Button add;
    Button remove;
    Button clear;
    Button selectAll;

    AccessibleContext currentContext;

    // Create the GUI
    //
    public AccessibleSelectionPanel() {
        super();
	setLayout(new BorderLayout(0, 5)); // 0 pixel horiz gap; 5 pixel vert gap

        // Add Panel labels
        //
	Panel panel = new Panel();
	panel.setLayout(new BorderLayout());
	accessibleLabel = new Label("                              ");
        panel.add("West", accessibleLabel);

	selectionCount = new Label("                               ");
	panel.add("East", selectionCount);

	add("North", panel);

        // Add Lists
        //
	panel = new Panel();
	panel.setLayout(new BorderLayout());
	childrenList = new java.awt.List(5);
        panel.add("West", childrenList);

	selectedList = new java.awt.List(5);
	panel.add("East", selectedList);

	add("Center", panel);

        // Add Buttons
        //
	panel = new Panel();
	add = new Button("Add");
        panel.add(add);
	add.addActionListener(this);

	remove = new Button("Remove");
	panel.add(remove);
	remove.addActionListener(this);

	clear = new Button("Clear");
	panel.add(clear);
	clear.addActionListener(this);

	selectAll = new Button("Select All");
	panel.add(selectAll);
	selectAll.addActionListener(this);

	add("South", panel);
    }
    
    public void updateInfo(AccessibleContext ac, Point p) {
	currentContext = ac;
	childrenList.setVisible(false);
	childrenList.removeAll();
	if (ac == null) {
	    accessibleLabel.setText("(null)");
        } else {
            accessibleLabel.setText("Name:  " + ac.getAccessibleName());

	    //
	    // AccessibleSelection
	    //
	    AccessibleSelection as = ac.getAccessibleSelection();
	    if (as == null) {
		childrenList.addItem("(AccessibleSelection not supported)");
	    } else {
		Accessible child;
		AccessibleContext childContext; 
		String name;
		int children = ac.getAccessibleChildrenCount();
		for (int i = 0; i < children; i++) {
		    child = ac.getAccessibleChild(i);
		    if (child != null) {
		        childContext = child.getAccessibleContext();
			if (childContext != null) {
			    name = childContext.getAccessibleName();
			    if (name != null) {
		                childrenList.addItem(name);
			    } else {
				childrenList.addItem("(null)");
			    }
			}
		    }
		}
		updateSelectionInfo();
	    }
        }
        childrenList.setVisible(true);
    }

    public void updateSelectionInfo() {
	selectedList.setVisible(false);
	selectedList.removeAll();
	AccessibleSelection as = currentContext.getAccessibleSelection();
	if (as != null) {
	    Accessible selected;
	    AccessibleContext selectedContext; 
	    String name;
	    int selCount = as.getAccessibleSelectionCount();
   	    selectionCount.setText(selCount + " items selected");
	    for (int i = 0; i < selCount; i++) {
		selected = as.getAccessibleSelection(i);
		if (selected != null) {
		    selectedContext = selected.getAccessibleContext();
		    if (selectedContext != null) {
			name = selectedContext.getAccessibleName();
			if (name != null) {
			    selectedList.addItem(name);
			} else {
			    selectedList.addItem("(null)");
			}
		    }
		}
	    }
	}
	selectedList.setVisible(true);
    }

    public void actionPerformed(ActionEvent e) {
	String cmd = e.getActionCommand();
	AccessibleSelection as = currentContext.getAccessibleSelection();
	if (as != null) {
	    if (cmd == "Add") {
		int index = childrenList.getSelectedIndex();
		as.addAccessibleSelection(index);
	    } else if (cmd == "Remove") {
		int index = childrenList.getSelectedIndex();
		as.removeAccessibleSelection(index);
	    } else if (cmd == "Clear") {
		as.clearAccessibleSelection();
	    } else if (cmd == "Select All") {
		as.selectAllAccessibleSelection();
	    }
	    updateSelectionInfo();
	}
    }
}
