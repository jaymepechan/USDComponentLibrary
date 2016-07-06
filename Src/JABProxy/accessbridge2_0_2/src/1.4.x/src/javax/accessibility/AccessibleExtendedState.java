/*
 * @(#)AccessibleExtendedState.java	1.1 02/10/01
 *
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package javax.accessibility;

import javax.accessibility.*;

/**
 * <P>Class AccessibleState describes a component's particular state.  The actual
 * state of the component is defined as an AccessibleStateSet, which is a
 * composed set of AccessibleStates.
 * <p>The toDisplayString method allows you to obtain the localized string 
 * for a locale independent key from a predefined ResourceBundle for the 
 * keys defined in this class.
 * <p>The constants in this class present a strongly typed enumeration
 * of common object roles.  A public constructor for this class has been
 * purposely omitted and applications should use one of the constants
 * from this class.  If the constants in this class are not sufficient
 * to describe the role of an object, a subclass should be generated
 * from this class and it should provide constants in a similar manner.
 *
 * @version     1.1 10/01/02
 * @author	Lynn Monsanto
 */

public class AccessibleExtendedState extends AccessibleState {

    /**
     * Indicates a component is responsible for managing
     * its subcomponents.
     */
    public static final AccessibleExtendedState MANAGES_DESCENDENTS
            = new AccessibleExtendedState("managesDescendents");

    protected AccessibleExtendedState(String s) {
	super(s);
    }
}
