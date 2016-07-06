/*
 * @(#)AccessibleExtendedRole.java	1.1 02/10/01
 *
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

package javax.accessibility;

import javax.accessibility.*;

/**
 * <P>Class AccessibleExtendedRole contains extensions to the class
 * AccessibleRole that are currently not in a public API.
 * 
 * <P>Class AccessibleRole determines the role of a component.  The role 
 * of a component describes its generic function. (E.G., 
 * "push button," "table," or "list.")
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

public class AccessibleExtendedRole extends AccessibleRole {

    /**
     * Indicates this component is a text header.
     */
    public static final AccessibleExtendedRole HEADER
            = new AccessibleExtendedRole("Header");

    /**
     * Indicates this component is a text footer.
     */
    public static final AccessibleExtendedRole FOOTER
            = new AccessibleExtendedRole("Footer");

    /**
     * Indicates this component is a text paragraph.
     */
    public static final AccessibleExtendedRole PARAGRAPH
            = new AccessibleExtendedRole("Paragraph");

    /**
     * Indicates this component is a ruler.
     */
    public static final AccessibleExtendedRole RULER
            = new AccessibleExtendedRole("RULER");

    protected AccessibleExtendedRole(String s) {
	super(s);
    }
}
