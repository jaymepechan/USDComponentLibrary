/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)AccessibleHypertextPanel.java	1.5 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.border.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.swing.table.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

public class AccessibleHypertextPanel extends AccessibilityPanel {
    AccessibleContext currentContext;
    AccessibleHypertext hypertext;
    JTextField nameField;
    JTextField descriptionField;
    JTextField linkCountField;
    JTable linkTable;
    UpdatingAbstractTableModel linkModel;

    // Create the GUI
    //
    public AccessibleHypertextPanel() {
        super();
	setLayout(new BorderLayout(0, 5)); // 0 pixel horiz gap; 5 pixel vert gap
        add("North", createTitlePanel());
        add("Center", createLinkPanel());
	add("South", createButtonPanel());
    }
    
    private JPanel createTitlePanel() {
	JPanel titlePanel = new JPanel();

	nameField = new JTextField(10);
	nameField.setEditable(false);
	JLabel nameLabel = new JLabel("Name: ");
	nameLabel.setLabelFor(nameField);
	titlePanel.add(nameLabel);
	titlePanel.add(nameField);

	descriptionField = new JTextField(20);
	descriptionField.setEditable(false);
	JLabel descriptionLabel = new JLabel("Description: ");
	descriptionLabel.setLabelFor(descriptionField);
	titlePanel.add(descriptionLabel);
	titlePanel.add(descriptionField);

	linkCountField = new JTextField(5);
	linkCountField.setEditable(false);
	JLabel linkLabel = new JLabel("Link Count: ");
	linkLabel.setLabelFor(linkCountField);
	titlePanel.add(linkLabel);
	titlePanel.add(linkCountField);

	return titlePanel;
    }

    private JScrollPane createLinkPanel() {
	// Create the link table headers
	final String[] headers = {"Link #", "Text Range", "Action Count", "Acion #0", "Anchor #0", "Link Object #0"};

        // Create a model of the data.
        linkModel = new UpdatingAbstractTableModel() {
            public int getColumnCount() { return headers.length; }
            public int getRowCount() { 
		if (hypertext != null) {
		    return hypertext.getLinkCount();
		} else {
		    return 0;
		}
	    }
            public Object getValueAt(int row, int col) {
		if (hypertext != null) {
		    AccessibleHyperlink link = hypertext.getLink(row);
		    switch (col) {
		    case 0: // link #
			return new Integer(row);
		    case 1: // text range of link
			return new String("[" + link.getStartIndex() + ", " 
					  + link.getEndIndex() + "]");
		    case 2: // action count at link
			return new Integer(link.getAccessibleActionCount());
		    case 3: // action #0
			return link.getAccessibleActionDescription(0);
		    case 4:
			return link.getAccessibleActionAnchor(0).toString();
		    case 5:
			return link.getAccessibleActionObject(0).toString();
		    default:
			return null;
		    }
		} else {
		    return null;
		}
	    }
            public String getColumnName(int column) {return headers[column];}
            public boolean isCellEditable(int row, int col) {return false;}
        };

        // Create the table
        linkTable = new JTable(linkModel);

	JScrollPane linkPane = new JScrollPane(linkTable);
	return linkPane;
    }

    private JPanel createButtonPanel() {
	JPanel buttonPanel = new JPanel();
	JButton activate = new JButton("Activate Selected Link");
	activate.addActionListener(new ActionListener() {
	    public void actionPerformed(ActionEvent e) {
		if (hypertext != null) {
		    int row = linkTable.getSelectedRow();
		    AccessibleHyperlink link = hypertext.getLink(row);
		    link.doAccessibleAction(0);
		}
	    }
	});
	buttonPanel.add(activate);

	return buttonPanel;
    }

    public void updateInfo(AccessibleContext ac, Point p) {
	currentContext = ac;
	String desc;
	if (ac == null) {
	    linkModel.tableRowsInserted(0, 0);
        } else {
	    AccessibleText at = ac.getAccessibleText();
	    if (at != null && at instanceof AccessibleHypertext) {
		hypertext = (AccessibleHypertext) at;
		linkModel.tableRowsInserted(0, hypertext.getLinkCount()-1);
		String s = ac.getAccessibleName();
		if (s == null) {
		    nameField.setText("[null]");
		} else {
		    nameField.setText(s);
		}
		s =  ac.getAccessibleDescription();
		if (s == null) {
		    descriptionField.setText("[null]");
		} else {
		    descriptionField.setText(s);
		}
		s = (new Integer(hypertext.getLinkCount())).toString();
		if (s == null) {
		    linkCountField.setText("?");
		} else {
		    linkCountField.setText(s);
		}
	    } else {
		// tell user this isn't an AccessibleHypertext thingie
		hypertext = null;
		linkModel.tableRowsInserted(0, 0);
		nameField.setText("[null]");
		descriptionField.setText("[null]");
		linkCountField.setText("?");
	    }
        }
    }

    // A special AbstractTableModel that has a public method for notifying
    // listeners that rows were inserted
    //
    abstract public class UpdatingAbstractTableModel 
    extends AbstractTableModel {
        public void tableRowsInserted(int start, int end) {
            fireTableRowsInserted(start, end);
        }
    }
}
