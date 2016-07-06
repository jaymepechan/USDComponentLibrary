/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Magnifier.java	1.7 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

/**
 * <P>Magnifier is an example app magnifies the pixels centered
 * around the object that has the focus, or the text I-bar.
 *
 * @version     1.0 07/24/97 18:12:20
 * @author      Peter Korn
 */
public class Magnifier extends Frame 
    implements FocusListener, MouseMotionListener, ChangeListener, 
    ActionListener {
    
    private boolean magnifyMouse = false;
    private boolean magnifyFocus = false;
    private boolean magnifyIbar = false;
    private int magnification = 1;

    // Create the GUI
    //
    public Magnifier() {
        super("Magnifier");
	    
        // Make the menu bar
        //
        MenuBar mb = new MenuBar();
        Menu m     = new Menu("File");
        MenuItem mi = new MenuItem("Exit");
        mi.addActionListener(this);
        m.add(mi);
        mb.add(m);

        m = new Menu("Settings");
        mi = new MenuItem("Track Mouse");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("Track Focus");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("Track I-bar");
        mi.addActionListener(this);
        m.add(mi);
        mb.add(m);

        m = new Menu("Magnification");
        mi = new MenuItem("1x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("2x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("3x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("4x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("5x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("6x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("7x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("8x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("9x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("10x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("11x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("12x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("13x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("14x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("15x");
        mi.addActionListener(this);
        m.add(mi);
        mi = new MenuItem("16x");
        mi.addActionListener(this);
        m.add(mi);
        mb.add(m);
        setMenuBar(mb);

        // Add Magnification Canvas
        //
	    Canvas magnifier = new Canvas();
        magnifier.setBounds(0, 0, 100, 100);
        add(magnifier);

        pack();
        show();
        
    }
    
    public void actionPerformed(ActionEvent e) {
        String s = e.getActionCommand();
        if (s == "Exit") {
            System.exit(0);
        } else if (s == "Track Mouse") {
            magnifyMouse = !magnifyMouse;
        } else if (s == "Track Focus") {
            magnifyFocus = !magnifyFocus;
        } else if (s == "Track I-bar") {
            magnifyIbar = !magnifyFocus;
        } else if (s == "1x") {
            magnification = 1;
        } else if (s == "2x") {
            magnification = 2;
        } else if (s == "3x") {
            magnification = 3;
        } else if (s == "4x") {
            magnification = 4;
        } else if (s == "5x") {
            magnification = 5;
        } else if (s == "6x") {
            magnification = 6;
        } else if (s == "7x") {
            magnification = 7;
        } else if (s == "8x") {
            magnification = 8;
        } else if (s == "9x") {
            magnification = 9;
        } else if (s == "10x") {
            magnification = 10;
        } else if (s == "11x") {
            magnification = 11;
        } else if (s == "12x") {
            magnification = 12;
        } else if (s == "13x") {
            magnification = 13;
        } else if (s == "14x") {
            magnification = 14;
        } else if (s == "15x") {
            magnification = 15;
        } else if (s == "16x") {
            magnification = 16;
        }
    }  

    // Do the magnification
    //
    public void magnify(Rectangle r) {
        
        // get the top-level window containing this rect
        // call getPeer() on it to get the ComponentPeer
        // on the ComponentPeer, call createImage() ?? 
        // create a java.awt.image.PixelGrabber and sic
        // it onto the create image to get a subset
        // of the pixels around the specified rectangle
        // put that subset into a new image, and draw
        // that new image, magnified, into the magnification
        // panel

    }

    public void magnify(Point p) {
        // do basically the same thing as magnify(Rect) above,
        // only centering around the Point passed in       
    }

    // Handle focus changes
    //
    public void focusGained(FocusEvent e) {
        if (e.isTemporary() == false && magnifyFocus == true) {
            magnify(e.getComponent().getBounds());  // translate to global coords!!!
        }
    }

    public void focusLost(FocusEvent e) {
        // nothing to do here...
    }

    // Handle mouse movements
    //
    public void mouseDragged(MouseEvent e) {
        if (magnifyMouse == true) {
            magnify(e.getPoint());  // translate to global coords!!!
        }
    }

    public void mouseMoved(MouseEvent e) {
        if (magnifyMouse == true) {
            magnify(e.getPoint());  // translate to global coords!!!
        }
    }


    // Handle I-bar movements
    //
    public void stateChanged(ChangeEvent e) {
	Accessible a;
	a = Translator.getAccessible(e.getSource());
	if (a instanceof SpatialTextInformation) {
	    SelectionRange = a.getSelectionRange();
	    magnify(SelectionRange.start);	// translate to global coords!!!
	}
    }


    static public void main(String args[]) {
        new Magnifier();
    }
}
