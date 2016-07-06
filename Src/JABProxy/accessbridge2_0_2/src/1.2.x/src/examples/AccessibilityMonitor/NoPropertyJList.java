/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)NoPropertyJList.java	1.4 02/01/17
 */

import java.beans.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

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
	    if (accessibleContext == null) {
		accessibleContext = new AccessibleNoPropertyJList();
	    }
	    return accessibleContext;
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
