/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)LabelTranslator.java	1.8 02/01/17
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
 * for the Label class.  Translator.getAccessible() will automatically
 * load this class when an assistive technology asks for an accessible
 * translator for Label.
 *
 * @version	1.8 01/17/02 16:09:08
 * @author	Willie Walker
 */
public class LabelTranslator extends Translator {

    public String getAccessibleName() {
        return ((Label) source).getText();
    }

    /**
     * Set the name of this object.
     */
    public void setAccessibleName(String s) {
        ((Label) source).setText(s);
    }

    public AccessibleRole getAccessibleRole() {
        return AccessibleRole.LABEL;
    }
}
