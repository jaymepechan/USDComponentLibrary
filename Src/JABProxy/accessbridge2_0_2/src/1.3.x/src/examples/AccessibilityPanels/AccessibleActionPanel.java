/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibleActionPanel.java	1.8 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

public class AccessibleActionPanel 
  extends AccessibilityPanel implements ActionListener {
    java.awt.List actionList;
    Label accessibleLabel;
    Button doAction;
    AccessibleContext currentContext;

    // Create the GUI
    //
    public AccessibleActionPanel() {
        super();
	setLayout(new BorderLayout(0, 5)); // 0 pixel horiz gap; 5 pixel vert gap

        // Add Panel label
        //
	accessibleLabel = new Label();
        add("North", accessibleLabel);

        // Add AccessibleAction List
        //
	actionList = new java.awt.List(5);
        add("Center", actionList);

        // Add DoIt Button
        //
	doAction = new Button("Do Action");
        add("South", doAction);
	doAction.addActionListener(this);
    }
    
    public void updateInfo(AccessibleContext ac, Point p) {
	currentContext = ac;
	actionList.setVisible(false);
	actionList.removeAll();
	String desc;
	if (ac == null) {
	    accessibleLabel.setText("(null)");
        } else {
            accessibleLabel.setText("Name:  " + ac.getAccessibleName());

	    //
	    // AccessibleAction
	    //
	    AccessibleAction aa = ac.getAccessibleAction();
	    if (aa == null) {
		actionList.add("(no accessible actions)");
	    } else {
		int count = aa.getAccessibleActionCount();
		for (int i = 0; i < count; i++) {
		    desc = aa.getAccessibleActionDescription(i);
		    if (desc != null) {
		        actionList.add(desc);
		    } else {
			actionList.add("(null description)");
		    }
		}
	    }
        }
        actionList.setVisible(true);
    }

    public void actionPerformed(ActionEvent e) {
	if (e.getActionCommand() == "Do Action") {
	    int index = actionList.getSelectedIndex();
	    if (index != -1) {
		AccessibleAction aa = currentContext.getAccessibleAction();
		if (aa != null) {
		    aa.doAccessibleAction(index);
		}
	    }
	}
    }
}
