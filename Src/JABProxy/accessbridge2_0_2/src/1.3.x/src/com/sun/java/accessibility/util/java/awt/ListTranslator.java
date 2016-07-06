/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)ListTranslator.java	1.10 02/01/17
 */

package com.sun.java.accessibility.util.java.awt;

import java.lang.*;
import java.util.*;
import java.awt.*;
import java.awt.image.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <p>The Translator class provides a translation to interface Accessible 
 * for objects that do not implement interface Accessible.  Assistive
 * technologies can use the 'getAccessible' class method of Translator to
 * obtain an object that implements interface Accessible.  If the object
 * passed in already implements interface Accessible, getAccessible merely
 * returns the object. 
 *
 * <p>An example of how an assistive technology might use the Translator 
 * class is as follows: 
 *
 * <PRE>
 *    Accessible accessible = Translator.getAccessible(someObj);
 *    // obtain information from the 'accessible' object.
 * </PRE>
 *
 * <P>This class extends the Translator class to provide specific support
 * for the List class.  Translator.getAccessible() will automatically
 * load this class when an assistive technology asks for an accessible
 * translator for List.
 *
 * @version	1.10 01/17/02 16:09:10
 * @author	Willie Walker
 */
public class ListTranslator extends Translator {

    /**
     * Get the state of this object.
     * @return an instance of AccessibleState containing the current state of the object
     * @see AccessibleState
     */
    public AccessibleStateSet getAccessibleStateSet() {
        AccessibleStateSet states = super.getAccessibleStateSet();
        if (((java.awt.List) source).isMultipleMode()) {
            states.add(AccessibleState.MULTISELECTABLE);
        }
        if (((java.awt.List) source).getSelectedItems().length > 0) {
            states.add(AccessibleState.SELECTED);
        }
        return states;
    }

    public AccessibleRole getAccessibleRole() {
        return AccessibleRole.LIST;
    }
}
