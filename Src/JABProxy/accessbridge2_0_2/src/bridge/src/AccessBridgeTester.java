/*
 * @(#)AccessBridgeTester.java	1.15 05/09/20
 * 
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * 1.15 @(#)AccessBridgeTester.java	1.15
 */

import java.awt.*;
import java.awt.event.*;
import java.lang.reflect.*;

/**
 * @version     1.15 09/20/05
 * @author      Peter Korn
 */
public class AccessBridgeTester {

    public static final int ACCESS_BRIDGE_COMPATIBILITY = 2;
    public static final int AWT_EVENT_LISTENER_COMPATIBILITY = 4;
    public static final int HAS_1_3_APIS = 8;

    /**
     * main
     */
    static public void main(String args[]) {
        new AccessBridgeTester();
    }

    /**
     * 
     */
    public AccessBridgeTester() {
        int exitVal = 0;

	// don't install on JDK 1.1.x
        String version = System.getProperty("java.version");
	if ((version == null) || (version.startsWith("1.1"))) {
	    System.exit(exitVal);
	}

        if (testAccessBridgeCompatibility()) {
            exitVal |= ACCESS_BRIDGE_COMPATIBILITY;
        }
        if (testAWTEventListenerCompatibility()) {
            exitVal |= AWT_EVENT_LISTENER_COMPATIBILITY;
        }
        if (test1_3APICompatibility()) {
            exitVal |= HAS_1_3_APIS;
        }

        System.exit(exitVal);
    }
    
    /**
     * Determine if Toolkit.getComponentFromNativeWindowHandle() exists,
     * without which the Java Access Bride won't work
     *
     */
    public boolean testAccessBridgeCompatibility() {
	Method getComponentFromNativeWindowHandleMethod;
	Class integerParemter[] = new Class[1];
        integerParemter[0] = Integer.TYPE;
        Object[] args = new Object[1];
        args[0] = new Integer(1);
        Component c;

	// JDK 1.4.1 and above use JAWT.DLL to map
	// components to native window handles.
	String s = System.getProperty("java.version");
	if (s != null && s.compareTo("1.4.1") >= 0) {
	    return true;
        } 

	Toolkit tk = Toolkit.getDefaultToolkit();
        try {
            getComponentFromNativeWindowHandleMethod = tk.getClass().getMethod("getComponentFromNativeWindowHandle", integerParemter);
            if (getComponentFromNativeWindowHandleMethod != null) {
                try {
                    c = (Component) getComponentFromNativeWindowHandleMethod.invoke(tk, args);
                    return true;
                } catch (InvocationTargetException e) {
		    // System.out.println("Exception: " + e.toString());
                } catch (IllegalAccessException e) {
		    // System.out.println("Exception: " + e.toString());
                }
            }
	    
        } catch (NoSuchMethodException e) {
	    // System.out.println("Exception: " + e.toString());
	} catch (SecurityException e) {
	    // System.out.println("Exception: " + e.toString());
        }
	
        return false;
    }
    
    /**
     * Determine if Toolkit.addAWTEventListener() exists,
     * to decide which version of JAccess to install...
     *
     */
    public boolean testAWTEventListenerCompatibility() {
    	Method addAWTEventListenerMethod;
	Class paremeters[] = new Class[2];
        try {
            paremeters[0] = Class.forName("java.awt.event.AWTEventListener");
	} catch (ClassNotFoundException e) {
            return false;
        }
        paremeters[1] = Long.TYPE;
	Toolkit tk = Toolkit.getDefaultToolkit();
        try {
            addAWTEventListenerMethod = tk.getClass().getMethod("addAWTEventListener", paremeters);
            if (addAWTEventListenerMethod != null) {
                return true;
            }

        } catch (NoSuchMethodException e) {
	    // System.out.println("Exception: " + e.toString());
	} catch (SecurityException e) {
	    // System.out.println("Exception: " + e.toString());
        }

        return false;
    }


    /**
     * Determine if the new 1.3 Accessibility APIs are ,
     * to decide which version of JAccess to install...
     *
     */
    public boolean test1_3APICompatibility() {
        try {
	    Class AccessibleRelationClass = Class.forName("javax.accessibility.AccessibleRelation");
	    Class AccessibleIconClass = Class.forName("javax.accessibility.AccessibleIcon");
	    Class AccessibleTableClass = Class.forName("javax.accessibility.AccessibleTable");
            return true;
	} catch (ClassNotFoundException e) {
        }

        return false;
    }

}
