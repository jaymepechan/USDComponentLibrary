/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibleNode.java	1.9 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.applet.Applet;
import javax.swing.*;
import javax.swing.tree.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>AccessibleTree extends TreeNode to add support for storing the
 * Accessible object represented by the node.
 *
 * @version     1.9 01/17/02 16:12:17
 * @author      Willie Walker
 */
public class AccessibleNode extends DefaultMutableTreeNode
{
    public Accessible accessible = null;
    private String name;

    public AccessibleNode(String name) {
        super();
        this.setUserObject(this);
        this.name = name;
    }

    public AccessibleNode(Accessible accessible) {
        super();
        this.setUserObject(this);
        this.accessible = accessible;
	AccessibleContext ac = accessible.getAccessibleContext();
        if (ac != null) {
	    if (ac.getAccessibleRole() == AccessibleRole.UNKNOWN) {
                name = '!' + ac.getAccessibleName();
	    } else {
                name = '*' + ac.getAccessibleName();
	    }
	} else {
	    name = "fweep";
	    System.out.println("OOPS.  This class declares itself as" 
		+ " Accessible, but getAccessibleContext() returns null:  "
		+ accessible.getClass().toString());
	}
        name = name + " (" + accessible.getClass().toString() + ")";
    }

    public String toString() {
        return name;
    }
}
