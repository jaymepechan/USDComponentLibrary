/*
 * @(#)AccessibleExtendedRelation.java	1.1 02/10/09
 *
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package com.sun.java.accessibility.extensions;

import javax.accessibility.*;

/**
 * <P>Class AccessibleExtendedRelation contains extensions to the class
 * AccessibleRelation that are currently not in a public API.
 * 
 * <P>Class AccessibleRelation describes a relation between the
 * object that implements the AccessibleRelation and one or more other
 * objects.  The actual relations that an object has with other
 * objects are defined as an AccessibleRelationSet, which is a composed
 * set of AccessibleRelations.
 * <p>The toDisplayString method allows you to obtain the localized string 
 * for a locale independent key from a predefined ResourceBundle for the 
 * keys defined in this class.
 * <p>The constants in this class present a strongly typed enumeration
 * of common object roles. If the constants in this class are not sufficient
 * to describe the role of an object, a subclass should be generated
 * from this class and it should provide constants in a similar manner.
 *
 * @version     1.1 10/09/02
 * @author	Lynn Monsanto
 */

public class AccessibleExtendedRelation 
    extends AccessibleExtendedRelationConstants {

    public AccessibleExtendedRelation(String s) {
	super(s);
    }

    public AccessibleExtendedRelation(String key, Object target) {
	super(key, target);
    }
}
