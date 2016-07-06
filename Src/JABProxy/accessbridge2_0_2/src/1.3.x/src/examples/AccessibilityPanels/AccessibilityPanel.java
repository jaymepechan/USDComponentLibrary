/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibilityPanel.java	1.6 02/01/17
 */

import java.awt.*;
import javax.accessibility.*;

abstract public class AccessibilityPanel extends Panel {
    
    public AccessibilityPanel() {
        super();
    }
    
    abstract public void updateInfo(AccessibleContext ac, Point p);
}
