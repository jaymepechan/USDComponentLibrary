/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)ComponentNode.java	1.9 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.applet.Applet;
import javax.swing.*;
import javax.swing.tree.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>ComponentTree extends TreeNode to add support for storing the component
 * represented by the node.
 *
 * @version     1.9 01/17/02 16:08:17
 * @author      Willie Walker
 * @author	Peter Korn
 */
public class ComponentNode extends DefaultMutableTreeNode
{
    public Component component = null;
    private String name;

    public ComponentNode(String name) {
        super();
        this.setUserObject(this);
        this.name = name;
    }

    public ComponentNode(Component component) {
        super();
        this.setUserObject(this);
        this.component = component;
        if (component instanceof Accessible) {
	    AccessibleContext ac = ((Accessible) component).getAccessibleContext();
	    if ((ac != null) 
		&& (ac.getAccessibleRole() == AccessibleRole.UNKNOWN)) {
                name = '!' + ac.getAccessibleName();
	    } else {
		if (ac != null) {
		    name = '*' + ac.getAccessibleName();
		} else {
		    name = component.getName();
		}
	    }
        } else {
	    name = component.getName();
        }
        name = name + " (" + component.getClass().toString() + ")";
    }

    public String toString() {
        return name;
    }
}
