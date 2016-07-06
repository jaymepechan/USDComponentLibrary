/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibilityAPIPanel.java	1.30 02/01/17
 */

import java.awt.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.swing.text.html.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

public class AccessibilityAPIPanel extends AccessibilityPanel {
    
    java.awt.List mouseInformation;

    // Create the GUI
    //
    public AccessibilityAPIPanel() {
	this(20);
    }
    
    public AccessibilityAPIPanel(int lines) {
        super();
	setLayout(new BorderLayout());

        // Add Mouse Info List
        //
	mouseInformation = new java.awt.List(lines);
        add("Center", mouseInformation);
    }
    
    public void updateInfo(AccessibleContext ac, Point p) {
        mouseInformation.setVisible(false);
	mouseInformation.removeAll();
	if (ac == null) {
	    mouseInformation.add("(null)");
        } else {
	    mouseInformation.add("interface AccessibleContext:");
            mouseInformation.add("    Name:  " + ac.getAccessibleName());
            mouseInformation.add("    Desc:  " + ac.getAccessibleDescription());
            mouseInformation.add("    Role:  " + ac.getAccessibleRole());
            mouseInformation.add("    State(s): " + ac.getAccessibleStateSet());
	    try {
	        mouseInformation.add("    Locale: " + ac.getLocale());
	    } catch (IllegalComponentStateException e) {
		mouseInformation.add("    Locale: (null)");
	    }

	    //
	    // Parent/Child/Sibling stuff
	    //
            Accessible parent = ac.getAccessibleParent();
            if (parent != null) {
		AccessibleContext ap = parent.getAccessibleContext();
		if (ap != null) {
		    mouseInformation.add("    Parent's Name: " + ap.getAccessibleName());
		}
            }

            mouseInformation.add("    # Children: " + ac.getAccessibleChildrenCount());

	    if (parent != null) {
		AccessibleContext ap = parent.getAccessibleContext();
		Accessible sibling = ap.getAccessibleChild(ac.getAccessibleIndexInParent()+1);
		if (sibling != null) {
		    AccessibleContext as = sibling.getAccessibleContext();
		    mouseInformation.add("    Next Sibling's Name: " + as.getAccessibleName());
		}
		sibling = ap.getAccessibleChild(ac.getAccessibleIndexInParent()-1);
		if (sibling != null) {
		    AccessibleContext as = sibling.getAccessibleContext();
		    mouseInformation.add("    Previous Sibling's Name: " + as.getAccessibleName());
		}
	    }

	    //
	    // AccessibleComponent
	    //
	    AccessibleComponent acp = ac.getAccessibleComponent();
	    if (acp != null) {
	    	mouseInformation.add("interface AccessibleComponent:");
		mouseInformation.add("    Background Color: " + acp.getBackground());
		mouseInformation.add("    Foreground Color: " + acp.getForeground());
		mouseInformation.add("    Cursor: " + acp.getCursor());
		Font font = acp.getFont();
		mouseInformation.add("    Font: " + font);
		if (font != null) {
		    mouseInformation.add("    Font Metrics: " + acp.getFontMetrics(font));
		}
		mouseInformation.add("    Enabled: " + acp.isEnabled());
		mouseInformation.add("    Visible: " + acp.isVisible());
		mouseInformation.add("    Showing: " + acp.isShowing());
		if (acp.isShowing()) {
		    mouseInformation.add("    Location on Screen: " + acp.getLocationOnScreen());
		}
		mouseInformation.add("    Bounds: " + acp.getBounds());
		mouseInformation.add("    Size: " + acp.getSize());
		mouseInformation.add("    Focus Traversable: " + acp.isFocusTraversable());
	    }

	    //
	    // AccessibleValue
	    //
	    AccessibleValue av = ac.getAccessibleValue();
	    if (av != null) {
	    	mouseInformation.add("interface AccessibleValue:");
		mouseInformation.add("    Value: " + av.getCurrentAccessibleValue());
		mouseInformation.add("    Min Value: " + av.getMinimumAccessibleValue());
		mouseInformation.add("    Max Value: " + av.getMaximumAccessibleValue());
	    }

	    //
	    // AccessibleAction
	    //
	    AccessibleAction aa = ac.getAccessibleAction();
	    if (aa != null) {
	    	mouseInformation.add("interface AccessibleAction:");
		mouseInformation.add("    # Actions: " + aa.getAccessibleActionCount());
		int count;
		if ((count = aa.getAccessibleActionCount()) > 0) {
		    String s = new String("    Actions: [");
		    for (int i = 0;  i < count;  i++) {
			s = s + aa.getAccessibleActionDescription(i);
			if (i < count - 1) {
			    s = s + ", ";
			}
		    }
		    s = s + "]";
		    mouseInformation.add(s);
		}
	    }

	    //
	    // AccessibleSelection
	    //
	    AccessibleSelection as = ac.getAccessibleSelection();
	    if (as != null) {
	    	mouseInformation.add("interface AccessibleSelection:");
		mouseInformation.add("    # Things Selected: " + as.getAccessibleSelectionCount());
            }

	    //
	    // AccessibleText
	    //
	    AccessibleText textInfo = ac.getAccessibleText();
	    if (textInfo != null) {
	    	mouseInformation.add("interface AccessibleText:");
		if (p != null) {
		    int i = textInfo.getIndexAtPoint(p);
		    mouseInformation.add("    Index under mouse: " + i);
		    String s;
		    s = textInfo.getAtIndex(AccessibleText.CHARACTER, i);
		    mouseInformation.add("    Character under mouse: " + s);
		    s = textInfo.getAtIndex(AccessibleText.WORD, i);
		    mouseInformation.add("    Word under mouse:" + s);
		    s = textInfo.getAtIndex(AccessibleText.SENTENCE, i);
		    mouseInformation.add("    Sentence under mouse: " + s);

		    s = textInfo.getBeforeIndex(AccessibleText.CHARACTER, i);
		    mouseInformation.add("    Character before mouse: " + s);
		    s = textInfo.getBeforeIndex(AccessibleText.WORD, i);
		    mouseInformation.add("    Word before mouse:" + s);
		    s = textInfo.getBeforeIndex(AccessibleText.SENTENCE, i);
		    mouseInformation.add("    Sentence before mouse: " + s);

		    s = textInfo.getAfterIndex(AccessibleText.CHARACTER, i);
		    mouseInformation.add("    Character after mouse: " + s);
		    s = textInfo.getAfterIndex(AccessibleText.WORD, i);
		    mouseInformation.add("    Word after mouse:" + s);
		    s = textInfo.getAfterIndex(AccessibleText.SENTENCE, i);
		    mouseInformation.add("    Sentence after mouse: " + s);

		    mouseInformation.add("    Char/Object rect under mouse: " 
			+ textInfo.getCharacterBounds(i));

		    Point lineBounds = getLineBounds(textInfo, i);
		    if (lineBounds != null) {
			s = getTextRange(textInfo, lineBounds.x, lineBounds.y);
			mouseInformation.add("    Line under mouse, range [" 
			    + lineBounds.x + ", " + lineBounds.y + "] " + s);
		    }

		    String attrText;
		    AttributeSet attributes = textInfo.getCharacterAttribute(i);
		    if (attributes != null) {
			attrText = expandAttributes(attributes);
		    } else {
			attrText = new String("");
		    }
		    mouseInformation.add("    Char attributes under mouse: " 
			+ attrText);
		    if (attributes != null) {
			attrText = expandStyleConstants(attributes);
		    } else {
			attrText = new String("");
		    }
		    mouseInformation.add("    Style Constants defined under mouse: " + attrText);

		    //
		    // special check for specific attributes at the mouse loc.
		    //
		    if (attributes != null) {
			Object o;

			// Icon
			o = attributes.getAttribute(StyleConstants.IconAttribute);
			if (o != null && o instanceof ImageIcon) {
			    s = ((ImageIcon) o).getDescription();
			    mouseInformation.add("        Icon under mouse: " + s);
			}

			// Component
			o = attributes.getAttribute(StyleConstants.ComponentAttribute);
			if (o != null && o instanceof Accessible) {
			    AccessibleContext tac = ((Accessible) o).getAccessibleContext();
			    if (tac != null) {
				s = tac.getAccessibleName();
			        mouseInformation.add("        Component under mouse: " + s);
			    }
			}

			// HTML IMG
			o = attributes.getAttribute(StyleConstants.NameAttribute);
			if (o != null && o == HTML.Tag.IMG) {
			    o = attributes.getAttribute(HTML.Attribute.SRC);
			    if (o != null) {
				mouseInformation.add("        HTML image source under mouse: " + o);
			    }
			    o = attributes.getAttribute(HTML.Attribute.ALT);
			    if (o != null) {
				mouseInformation.add("        HTML image ALT text under mouse: " + o);
			    }
			    o = attributes.getAttribute(HTML.Attribute.WIDTH);
			    if (o != null) {
				mouseInformation.add("        HTML image width under mouse: " + o);
			    }
			    o = attributes.getAttribute(HTML.Attribute.HEIGHT);
			    if (o != null) {
				mouseInformation.add("        HTML image height under mouse: " + o);
			    }
			}
		    }
		} else {
		    mouseInformation.add("    <<no point available>>");
		}
		mouseInformation.add("    Char count of object: " + textInfo.getCharCount());
		int i = textInfo.getCaretPosition();
		mouseInformation.add("    Caret position: " + i);
                String s;
                s = textInfo.getAtIndex(AccessibleText.CHARACTER, i);
                mouseInformation.add("    Character at caret: " + s);
                s = textInfo.getAtIndex(AccessibleText.WORD, i);
                mouseInformation.add("    Word at caret:" + s);
		mouseInformation.add("    Char/Object rect at caret: " + textInfo.getCharacterBounds(i));

		Point lineBounds = getLineBounds(textInfo, i);
		if (lineBounds != null) {
		    s = getTextRange(textInfo, lineBounds.x, lineBounds.y);
		    mouseInformation.add("    Line at caret, range [" 
			+ lineBounds.x + ", " + lineBounds.y + "] " + s);
		}

		int start = textInfo.getSelectionStart();
		int end = textInfo.getSelectionEnd();
		if (start != end) {
		    mouseInformation.add("    Selection range: [" +
		    start + ", " + end + "]");
		    Object selection = textInfo.getSelectedText();
		    if (selection instanceof String) {
		        mouseInformation.add("    Selected text: " + (String) selection);
		    }
		}

		//
		// AccessibleHypertext
		//

		if (textInfo instanceof AccessibleHypertext) {
		    AccessibleHypertext hyper = (AccessibleHypertext) textInfo;
		    mouseInformation.add("class AccessibleHypertext:");
		    int linkCount = hyper.getLinkCount();
		    mouseInformation.add("    # Links: " + linkCount);
		    if (linkCount > 0) {
			int index = textInfo.getIndexAtPoint(p);
			int linkIndex = hyper.getLinkIndex(index);
			mouseInformation.add("    Link # at mouse: " 
			    + linkIndex);
			AccessibleHyperlink link = hyper.getLink(linkIndex);
			if (link != null) {
			    start = link.getStartIndex();
			    end = link.getEndIndex();
			    mouseInformation.add("    Range at mouse: [" 
				+ start + ", " + end + "]");
			    mouseInformation.add("    Actions at mouse: "
				+ link.getAccessibleActionCount());
			    mouseInformation.add("    Action #1: " + 
				link.getAccessibleActionDescription(0));
			    mouseInformation.add("    Anchor #1: " + 
				link.getAccessibleActionAnchor(0));
			    mouseInformation.add("    Link object #1: " + 
				link.getAccessibleActionObject(0));
			}
		    }
		}
	    }

        } // if (ac != null)
	
        mouseInformation.setVisible(true);
    }

    /**
     * Enumerate all StyleConstants in the AttributeSet
     *
     * We need to check explicitly, 'cause of the HTML package conversion
     * mechanism (they may not be stored as StyleConstants, just translated
     * to them when asked).
     *
     * (Use convenience methods where they are defined...)
     *
     * Not checking the following (which the IBM SNS guidelines says 
     * should be defined):
     *    - ComponentElementName
     *    - IconElementName
     *    - NameAttribute
     *    - ResolveAttribute
     */
    private String expandStyleConstants(AttributeSet as) {
	Color c;
	Object o;
	String attrString = new String("");

	// ---------- check for various Character Constants

	attrString += "BidiLevel = " + StyleConstants.getBidiLevel(as);

	Component comp = StyleConstants.getComponent(as);
	if (comp != null) {
	    if (comp instanceof Accessible) {
		AccessibleContext ac = ((Accessible) comp).getAccessibleContext();
		if (ac != null) {
		    attrString += "; Accessible Component = " + ac.getAccessibleName();
		} else {
		    attrString += "; Innaccessible Component = " + comp;
		}
	    } else {
		attrString += "; Innaccessible Component = " + comp;
	    }
	}

	Icon i = StyleConstants.getIcon(as);
	if (i != null) {
	    if (i instanceof ImageIcon) {
		attrString += "; ImageIcon = " + ((ImageIcon) i).getDescription();
	    } else {
		attrString += "; Icon = " + i;
	    }
	}

	attrString += "; FontFamily = " + StyleConstants.getFontFamily(as);

	attrString += "; FontSize = " + StyleConstants.getFontSize(as);

	if (StyleConstants.isBold(as)) {
	    attrString += "; bold";
	}

	if (StyleConstants.isItalic(as)) {
	    attrString += "; italic";
	}

	if (StyleConstants.isUnderline(as)) {
	    attrString += "; underline";
	}

	if (StyleConstants.isStrikeThrough(as)) {
	    attrString += "; strikethrough";
	}

	if (StyleConstants.isSuperscript(as)) {
	    attrString += "; superscript";
	}

	if (StyleConstants.isSubscript(as)) {
	    attrString += "; subscript";
	}

	c = StyleConstants.getForeground(as);
	if (c != null) {
	    attrString += "; Foreground = " + c;
	}

	c = StyleConstants.getBackground(as);
	if (c != null) {
	    attrString += "; Background = " + c;
	}

	attrString += "; FirstLineIndent = " + StyleConstants.getFirstLineIndent(as);

	attrString += "; RightIndent = " + StyleConstants.getRightIndent(as);

	attrString += "; LeftIndent = " + StyleConstants.getLeftIndent(as);

	attrString += "; LineSpacing = " + StyleConstants.getLineSpacing(as);

	attrString += "; SpaceAbove = " + StyleConstants.getSpaceAbove(as);

	attrString += "; SpaceBelow = " + StyleConstants.getSpaceBelow(as);

	attrString += "; Alignment = " + StyleConstants.getAlignment(as);

	TabSet ts = StyleConstants.getTabSet(as);
	if (ts != null) {
	    attrString += "; TabSet = " + ts; 
        }

	return attrString;
    }


    /**
     * Get line info: left & right indicies of line, and line string
     *
     * algorythm:  cast forward/back, doubling each time,
     *             'till find line boundaries
     *              
     * return null if we can't get the info (e.g. index or at passed in
     * is bogus; etc.) 
     */
    private Point getLineBounds(AccessibleText at, int index) {
        int lineStart;
        int lineEnd;
        int offset;
        Rectangle charRect;
        Rectangle indexRect = at.getCharacterBounds(index);
        int textLen = at.getCharCount();

	if (indexRect == null) {
	    return null;
	}

        // find the end of the line
        //
        offset = 1;
        lineEnd = index + offset > textLen - 1 
		? textLen - 1 : index  + offset;
        charRect = at.getCharacterBounds(lineEnd);

             // push past end of line
        while (charRect != null 
	       && charRect.y <= indexRect.y 
	       && lineEnd < textLen - 1) {
            offset = offset << 1;
            lineEnd = index + offset > textLen - 1 
		    ? textLen - 1 : index + offset;
            charRect = at.getCharacterBounds(lineEnd);
        }
        if (lineEnd == textLen - 1) {    // special case: we're on the last line!
            // we found it!
        } else {
            offset = offset >> 1;   // know boundary within last expansion

                // pull back to end of line
            while (offset > 0) {
                charRect = at.getCharacterBounds(lineEnd - offset);
                if (charRect.y > indexRect.y) { // still beyond line
                    lineEnd -= offset;
                } else {
                    // leave lineEnd alone, it's close!
                }
                offset = offset >> 1;
            }
                // subtract one 'cause we're already too far...
            lineEnd -= 1;
        }

        // find the start of the line
        //
        offset = 1;
        lineStart = index - offset < 0 ? 0 : index - offset;
        charRect = at.getCharacterBounds(lineStart);

             // slouch behind beginning of line
        while (charRect != null
	       && charRect.y >= indexRect.y 
	       && lineStart > 0) {
            offset = offset << 1;
            lineStart = index - offset < 0 ? 0 : index - offset;
            charRect = at.getCharacterBounds(lineStart);
        }
        if (lineStart == 0) {    // special case: on the first line!
            // we found it!
        } else {
            offset = offset >> 1;   // know boundary within last expansion

                // ground forward to beginning of line
            while (offset > 0) {
                charRect = at.getCharacterBounds(lineStart + offset);
                if (charRect.y < indexRect.y) { // still before line
                    lineStart += offset;
                } else {
                    // leave lineStart alone, it's close!
                }
                offset = offset >> 1;
            }
                // subtract one 'cause we're already too far...
            lineStart += 1;
        }
        return new Point(lineStart, lineEnd);
    }


    /**
     * Get a range of text; null if indicies are bogus
     */
    private String getTextRange(AccessibleText at, int start, int end) {
        if (start >= end) {
            return null;
        }
	if (end >= at.getCharCount()) {
	    return null;
	}
        StringBuffer lineBuf = new StringBuffer(end-start+1);
        for (int i = start; i <= end; i++) {
            lineBuf.append(at.getAtIndex(AccessibleText.CHARACTER, i));
        }
        return lineBuf.toString();
    }



// idea:
//   - get list of attribute keys in the heirarchy
//   - build a hashtable with all keys and names, going from root to leaf
//   - build string with keys and (leaf-biased) attributes, using toString()

    private class HashtableRef {
	Hashtable table;
    }

    private String expandAttributes(AttributeSet as) {
	HashtableRef hashtable = new HashtableRef();
	hashtable.table = new Hashtable();
	AttributeSet currentAS = as;
	while (currentAS != null) {
	    gatherAttributeKeys(currentAS, hashtable);
	    currentAS = currentAS.getResolveParent();
	}
	String attributeText = new String("");
	Enumeration attributeNames = hashtable.table.keys();
	Object attrName;
	while (attributeNames.hasMoreElements()) {
	    attrName = attributeNames.nextElement();
	    attributeText += attrName + " = " 
		+ hashtable.table.get(attrName); 
	    if (attributeNames.hasMoreElements()) {
		attributeText += ", ";
	    }
	}
	return attributeText;
    }

    private void gatherAttributeKeys(AttributeSet as, HashtableRef htr) {
	Enumeration names = as.getAttributeNames();
	Object name;
	Object attribute;
	while (names.hasMoreElements()) {
	    name = names.nextElement();
	    if (!htr.table.containsKey(name)) {
		attribute = as.getAttribute(name);
		if (attribute instanceof AttributeSet) {
		    htr.table.put(name, as.NameAttribute);
		    AttributeSet currentAS = (AttributeSet) attribute;
		    while (currentAS != null) {
			gatherAttributeKeys((AttributeSet) attribute, htr);
			currentAS = currentAS.getResolveParent();
		    }
		} else {
		    htr.table.put(name, as.getAttribute(name));
		}
	    }
	}
    }
}
