/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Listener.java	1.5 02/01/17
 */

import java.awt.*;
import java.awt.event.*;
import java.util.*;
import javax.swing.*;
import javax.swing.event.*;
import javax.swing.text.*;
import javax.accessibility.*;
import com.sun.java.accessibility.util.*;

import javax.speech.*;
import javax.speech.synthesis.*;
import javax.speech.recognition.*;

/**
 * <P>Listener is an example assistive technology that uses EventQueueMonitor,
 * AWTEventMonitor, and SwingEventMonitor.
 *
 * <P>It is meant to run in the same Java Virtual Machine as
 * the application it is accessing.  To do this, you need to make sure
 * there is an "AWT.AutoLoadClasses=" line in the file,
 * $JDKHOME/lib/awt.properties.  For example, you would add the following
 * line in awt.properties to make this thread run with the application:
 * <PRE>
 * AWT.AutoLoadClasses=Listener
 * </PRE>
 * <P>Note that AWT.AutoLoadClasses can take a comma-separated list of
 * assistive technology classes.  Note also that you need to make sure
 * the classes for the assistive technologies are in your CLASSPATH
 * environment variable.
 *
 * <P>Furthermore, this class is dynamically loaded and a single instance
 * is created by the event queue class (see EventQueueMonitor) in a separate
 * thread.  For this to work properly, the constructor method needs to
 * do all the work.
 *
 * <P>For more information on the event delegation model, see the event
 * delegation model specification that comes with the JDK 1.1.1 documentation.
 *
 * @see EventQueueMonitor
 * @see AWTEventMonitor
 * @see SwingEventMonitor
 *
 * @version     1.0 08/02/97 17:08:39
 * @author      Peter Korn
 */


public class Listener extends Frame implements MenuListener {

    // for Accessibility tracking
    static JMenu activeMenu = null;

    // List of strings we're scanning for
    static SpeechInfoPanel infoPanel;

    // for Text to Speech choice
    EngineList ttsEngines;
    Menu voiceMenu;
    CheckboxMenuItem lastSelectedVoiceItem;
    CheckboxMenuItem lastSelectedEngineItem;

    // arrays of strings for action/selection/highlight
    static Vector actionItems;
    static Vector selectionItems;
    static Hashtable highlightItems;

    static Hashtable visibleItems;
    static StringVector allActions;

    // various greetings
    static String startupStr = "Starting up...";
    static String readyStr = "Ready.";
    static String goodbyeStr = "Goodbye.  <emp>Thank you</emp> for talking to me.";

    // the current AccessibleContext we're talking about
    static AccessibleContext currentAC;

    // recognition
    static RuleGrammar ruleGrammar;
    static Recognizer recognizer;
    static Synthesizer synthesizer;

    class SynthMenuItem extends MenuItem {
	EngineModeDesc engine;
	Synthesizer synth;

	public SynthMenuItem(EngineModeDesc e) {
	    super(e.getEngineName());
	    engine = e;
	    synth = null;
	}

	public Synthesizer getSynth() {
	    return synth;
	}

	public boolean activateSynth() {
	    if (synth == null) {
		try {
		    synth = Central.createSynthesizer(engine);
		    synth.allocate();
		    synth.speak("Loaded synthesizer: " + engine.getEngineName(), null);
		    return true;
		} catch (Throwable t) {
		    t.printStackTrace();
		}
	    }
	    return false;
	}

    }

    class VoiceCheckboxMenuItem extends CheckboxMenuItem {
	SynthMenuItem synthmi;
	EngineModeDesc engine;
	Synthesizer synth;
	Voice voice;

	public VoiceCheckboxMenuItem(SynthMenuItem smi, EngineModeDesc e, Voice v) {
	    super(v.getName());
	    engine = e;
	    synth = null;
	    voice = v;
	    synthmi = smi;
	}

	Synthesizer activateVoice() {
	    if (synth == null) {
		synthmi.activateSynth();
	    }
	    synth = synthmi.getSynth();
	    if (voice != null && synth != null) {
		SynthesizerProperties sp = 
		    (SynthesizerProperties) synth.getEngineProperties();
		sp.setVoice(voice);
		synth.speak("Switching to voice: " + voice.getName(), null);
		return synth;
	    }
	    return null;
	}
    }

    // Create the Listener object class
    //
    public Listener() {
	super("Listener");

        WindowListener l = new WindowAdapter() {
            public void windowClosing(WindowEvent e) { System.exit(0); }
        };

        SwingEventMonitor.addMenuListener(this); // mouse may be in popped menu

        setLayout(new BorderLayout());
        addWindowListener(l);

	initializeJavaSpeech();

        createMenuBar();

	infoPanel = new SpeechInfoPanel(20);
	add("Center", infoPanel);

        setSize(WIDTH, HEIGHT);
	pack();
	show();
    }


    // create the menu bar
    //
    void createMenuBar() {
        MenuBar menuBar = new MenuBar();
        MenuItem mi;
	CheckboxMenuItem cmi;
	SynthMenuItem smi;
	VoiceCheckboxMenuItem vcmi;
	ItemListener il;

        // File Menu
        Menu file = (Menu) menuBar.add(new Menu("File"));
        mi = (MenuItem) file.add(new MenuItem("Exit"));
        mi.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
				System.exit(0);
            }
        });


        // Voice Menu
	if (ttsEngines.size() > 0) {
	    Menu voiceMenu = (Menu) menuBar.add(new Menu("Voice"));
	    il = new ItemListener() {
		public void itemStateChanged(ItemEvent e) {
		    if (lastSelectedVoiceItem != null) {
			lastSelectedVoiceItem.setState(false);
		    }
		    VoiceCheckboxMenuItem vmi = (VoiceCheckboxMenuItem) e.getItemSelectable();
		    if (e.getStateChange() == ItemEvent.SELECTED) {
			synthesizer = vmi.activateVoice();
			lastSelectedVoiceItem = vmi;
		    }
		}
	    };
	    SynthesizerModeDesc smd;
	    Voice voices[];
	    for (short i = 0; i < ttsEngines.size(); i++) {
		smd = (SynthesizerModeDesc) ttsEngines.elementAt(i);
		smi = (SynthMenuItem) voiceMenu.add(new SynthMenuItem(smd));
		smi.setEnabled(false);
		voices = smd.getVoice();
		for (short j = 0; j < voices.length; j++) {
		    vcmi = (VoiceCheckboxMenuItem) voiceMenu.add(new VoiceCheckboxMenuItem(smi, smd, voices[j]));
		    vcmi.addItemListener(il);
		}
	    }
        }

	// Engine menu
	Menu engineMenu = (Menu) menuBar.add(new Menu("Engine"));
	il  = new ItemListener() {
	    public void itemStateChanged(ItemEvent e) {
		if (lastSelectedEngineItem != null) {
		    lastSelectedEngineItem.setState(false);
		}
		CheckboxMenuItem checkmi = (CheckboxMenuItem) e.getItemSelectable();
		if (e.getStateChange() == ItemEvent.SELECTED) {
		    lastSelectedEngineItem = checkmi;
		} else if (e.getStateChange() == ItemEvent.DESELECTED) {
		    lastSelectedEngineItem = null;
		}
                
                if (lastSelectedEngineItem.getLabel().equals("start")) {
                    initializeJavaSpeechRecognition();
                    lastSelectedEngineItem.setState(false);
                    lastSelectedEngineItem.setEnabled(false);
                } else if (lastSelectedEngineItem.getLabel().equals("pause")) {
                    recognizer.pause();
                } else {
                    recognizer.resume();
                }
            }
        };
	cmi = (CheckboxMenuItem) engineMenu.add(new CheckboxMenuItem("start"));
	cmi.addItemListener(il);
	cmi = (CheckboxMenuItem) engineMenu.add(new CheckboxMenuItem("pause"));
	cmi.addItemListener(il);
        lastSelectedEngineItem = cmi;
	cmi = (CheckboxMenuItem) engineMenu.add(new CheckboxMenuItem("resume"));
	cmi.addItemListener(il);

        setMenuBar(menuBar);
    }



    // set up TTS and recognition
    //
    public void initializeCoreRules(RuleGrammar rg) 
        throws NullPointerException, IllegalArgumentException, Exception
    {
        RuleSequence rs;
        String emptyStringArray[] = {""};

            // initialize the set a list of actions
        Rule actions = new RuleAlternatives(emptyStringArray);
        rg.setRule("actions", actions, true);

            // initialize the set a list of accessibles
        Rule accessibles = new RuleAlternatives(emptyStringArray);
        rg.setRule("accessibles", accessibles, true);

            // initialize the set a list of selections
//        selectionItems = new Vector();
//        Rule selectables = new RuleAlternatives(emptyStringArray);
//        rg.setRule("selectables", selectables, true);

            // initialize the set a list of visible UI objects 
	    // implementing Accessible
        refreshAccessibleGrammars(rg);

            // public <bye> = Good bye {bye};
        rg.setRule("bye", new RuleTag(new RuleToken("Good bye"), "bye"), true);

            // public <select> = Select {select} <selectables>;
        rs = new RuleSequence(new RuleTag(new RuleToken("Select"), "select"));
//        rs.append(new RuleName("selectables"));
        rs.append(new RuleName("accessibles"));
        rg.setRule("select", rs, true);

            // public <deselect> = Deselect {deselect} <selectables>;
//        rs = new RuleSequence(new RuleTag(new RuleToken("Deselect"), "deselect"));
//        rs.append(new RuleName("selectables"));
//        rs.append(new RuleName("accessibles"));
//        rg.setRule("deselect", rs, true);

            // public <selectAll> = Select all {selectAll};
//        rg.setRule("selectAll", new RuleTag(new RuleToken("Select all"), "selectAll"), true);

            // public <deselectAll> = Clear selection {deselectAll};
//        rg.setRule("deselectAll", new RuleTag(new RuleToken("Clear selection"), "deselectAll"), true);

            // public <doAction> = Do {doAction} <actions>;
//        rs = new RuleSequence(new RuleTag(new RuleToken("Do"), "doAction"));
//        rs.append(new RuleName("actions"));
//        rg.setRule("doAction", rs, true);

            // public <highlight> = Highlight {highlight} <accessibles>;
        rs = new RuleSequence(new RuleTag(new RuleToken("Highlight"), "highlight"));
        rs.append(new RuleName("accessibles"));
        rg.setRule("highlight", rs, true);

            // public <perform> = <actions> {perform} <accessibles>;
        rs = new RuleSequence(new RuleTag(new RuleName("actions"), "Perform"));
        rs.append(new RuleName("accessibles"));
        rg.setRule("perform", rs, true);

            // public <refresh> = Refresh {refresh};
        rg.setRule("refresh", new RuleTag(new RuleToken("Refresh"), "refresh"), true);
    }

    public static void refreshAccessibleGrammars(RuleGrammar rg)
        throws NullPointerException, IllegalArgumentException, Exception
    {
        Window[] wins = EventQueueMonitor.getTopLevelWindows();
        Accessible a = Translator.getAccessible(wins[0]);
        if (a != null) {
            AccessibleContext ac = a.getAccessibleContext();
            if (ac != null) {
//                setAccessibles(rg, ac);
                buildOurGrammars(rg, ac);
            }
        }
    }

    // build the set of actions from the Accessible passed in
    //
    static public void setActions(RuleGrammar rg, AccessibleContext ac) 
        throws NullPointerException, IllegalArgumentException, Exception
    {
        if (ac != null) {
            AccessibleAction aa = ac.getAccessibleAction();
            if (aa != null) {
                actionItems.removeAllElements();
                for (short i = 0; i < aa.getAccessibleActionCount(); i++) {
                    actionItems.addElement(aa.getAccessibleActionDescription(i));
                }
                if (actionItems.size() >= 1) {
                    String actionsStr[] = new String[actionItems.size()];
                    actionItems.copyInto(actionsStr);
                    Rule actions = new RuleAlternatives(actionsStr);
                    rg.setRule("actions", actions, true);
                }
            }
        }
    }


    // build the set of selectionables from the Accessible passed in
    //
    static public void setSelectables(RuleGrammar rg, AccessibleContext ac) 
        throws NullPointerException, IllegalArgumentException, Exception
    {
        if (ac != null) {
            AccessibleSelection as = ac.getAccessibleSelection();
            if (as != null) {
	        Accessible child;
	        AccessibleContext childContext; 
        	String name;
                selectionItems.removeAllElements();
                for (short i = 0; i < ac.getAccessibleChildrenCount(); i++) {
	            child = ac.getAccessibleChild(i);
	            if (child != null) {
	                childContext = child.getAccessibleContext();
			if (childContext != null) {
			    name = childContext.getAccessibleName();
			    if (name != null) {
				selectionItems.addElement(name);
                            }
                        }
                    }
                }
                if (selectionItems.size() >= 1) {
                    String selectablesStr[] = new String[selectionItems.size()];
                    selectionItems.copyInto(selectablesStr);
                    Rule selectables = new RuleAlternatives(selectablesStr);
                    rg.setRule("selectables", selectables, true);
                }
            }
        }
    }


    // build the set of visible Accessibles from the Accessible (parent) 
    // passed in
    //
    static public void setAccessibles(RuleGrammar rg, AccessibleContext ac) 
        throws NullPointerException, IllegalArgumentException, Exception
    {
        highlightItems = buildNamesTable(ac, allActions);
        System.out.println("Built Accessibles table: " + highlightItems.toString());
        String highlightablesStr[] = new String[highlightItems.size()];
        int i = 0;
        String s;
        for (Enumeration e = highlightItems.keys(); e.hasMoreElements() ;) {
            s = (String) e.nextElement();
            highlightablesStr[i] = s;
            System.out.println("  -> highlightables[" + i + "] = " + s);
            i++;
        }
        Rule accessibles = new RuleAlternatives(highlightablesStr);
        rg.setRule("accessibles", accessibles, true);
        System.out.println("  -> highlight rules string: " + highlightablesStr);
    }


    // build the selection & action grammars with all visible components
    //
    static public void buildOurGrammars(RuleGrammar rg, AccessibleContext ac) 
        throws NullPointerException, IllegalArgumentException, Exception
    {
            // build Hashtable & vector
        visibleItems = buildNamesTable(ac, allActions);

            // set grammar for accessibles (Hashtable)
        String visiblesStr[] = new String[visibleItems.size()];
        int i = 0;
        String s;
        for (Enumeration e = visibleItems.keys(); e.hasMoreElements() ;) {
            s = (String) e.nextElement();
            visiblesStr[i] = s;
            i++;
        }
        Rule accessibles = new RuleAlternatives(visiblesStr);
        rg.setRule("accessibles", accessibles, true);

            //set grammar for actions (Vector)
        String actionsStr[] = new String[allActions.size()];
        allActions.copyInto(actionsStr);
        Rule actions = new RuleAlternatives(actionsStr);
        rg.setRule("actions", actions, true);
    }


    // return the index into the array that this string is (if it is...)
    //
    static public int getStringIndex(Vector v, String s) {
        int size = v.size();
        for (int i = 0; i < size; i++) {
            if (s.equals((String) v.elementAt(i))) {
                return i;
            }
        }
        return -1;
    }

    // return the AccessibleContext object matching the string key (if it is...)
    //
    static public AccessibleContext getAccessibleContextFromTable(Hashtable t, String s) {
        String current;
        if (t.size() > 0) {
            for (Enumeration e = t.keys() ; e.hasMoreElements() ;) {
                current = (String) e.nextElement();
                if (s.equals(current)) {
                    return (AccessibleContext) t.get(current);
                }
            }
        }
        return null;
    }


    // set up TTS and recognition
    //
    public void initializeJavaSpeech() {
	try {
	    // synthesizer
	    ttsEngines = Central.availableSynthesizers(null);
	    if (ttsEngines.size() > 0) {
		EngineModeDesc e = (EngineModeDesc) ttsEngines.firstElement();
		synthesizer = Central.createSynthesizer(e);
	    } else {
		synthesizer = Central.createSynthesizer(null);
	    }
	    synthesizer.allocate();
	    synthesizer.speak(startupStr, null);
        } catch (Throwable t) {
	    t.printStackTrace();
	}
    }

    // set up TTS and recognition
    //
    public void initializeJavaSpeechRecognition() {
	try {
	    // create a recognizer
	    recognizer = Central.createRecognizer(null);
	    recognizer.allocate();

	    // allocate our actions stringVector
	    allActions = new StringVector();
	
	    // rule grammar
	    ruleGrammar = recognizer.newRuleGrammar("Listener");
	    initializeCoreRules(ruleGrammar);
	    ruleGrammar.addGrammarListener(ruleListener);
	    ruleGrammar.setActive(true);

	    infoPanel.updateInfo(ruleGrammar);
		
	    // start
	    recognizer.commitChanges();
	    recognizer.resume();

	    synthesizer.speak(readyStr, null);
        } catch (Throwable t) {
	    t.printStackTrace();
	}
    }


    // wait for TTS "goodbye" is finished, then exit
    //
    static SpeakableListener exitWhenDone = new SpeakableAdapter() {
	public void endSpeakable(SpeakableEvent e) {
	    System.exit(0);
	}
    };


    // expand string vector
    //
    static String expandStringVector(Vector v) {
        int size = v.size();
        if (size >= 1) {
            String s = new String((String) v.elementAt(0));
            for (int i = 1; i < size; i++) {
                s = s.concat(", ");
                s = s.concat((String) v.elementAt(i));
            }
            return s;
        } else {
            return null;
        }
    }


    // parse user speech
    //
    static GrammarListener ruleListener = new GrammarResultAdapter() {

	// accepted result
	public void acceptedResult(ResultEvent e) {
	    FinalRuleResult result = (FinalRuleResult) e.getSource();
	    String tags[] = result.getTags();
            if (tags != null && tags.length >= 1) {
	        if (tags[0].equals("select")) {
                    String s = result.getBestToken(1).getSpokenText();
	            synthesizer.speak("Selecting: " + s, null);
                    AccessibleContext ac = (AccessibleContext) visibleItems.get(s);
                    if (ac != null) {
                        Accessible parent = ac.getAccessibleParent();
                        if (parent != null) {
                            AccessibleContext apc = parent.getAccessibleContext();
                            if (apc != null) {
                                AccessibleSelection as = apc.getAccessibleSelection();
                                if (as != null) {
                                    as.addAccessibleSelection(ac.getAccessibleIndexInParent());
                                }
                            }
                        }
                    }

	        } else if (tags[0].equals("deselect")) {

	            String s = result.getBestToken(1).getSpokenText();
	            synthesizer.speak("Deselecting: " + s, null);

                    int i = getStringIndex(selectionItems, s);
                    AccessibleSelection as = currentAC.getAccessibleSelection();
                    if (as != null) {
                        as.removeAccessibleSelection(i);
                    }

	        } else if (tags[0].equals("selectAll")) {

	            synthesizer.speak("Selecting everything", null);

                    AccessibleSelection as = currentAC.getAccessibleSelection();
                    if (as != null) {
                        as.selectAllAccessibleSelection();
                    }

	        } else if (tags[0].equals("deselectAll")) {

	            synthesizer.speak("Clearing selection", null);

                    AccessibleSelection as = currentAC.getAccessibleSelection();
                    if (as != null) {
                        as.clearAccessibleSelection();
                    }

	        } else if (tags[0].equals("doAction")) {

	            String s = result.getBestToken(1).getSpokenText();
	            synthesizer.speak("Doing: " + s, null);

                    int i = getStringIndex(actionItems, s);

                    AccessibleAction aa = currentAC.getAccessibleAction();
                    if (aa != null) {
                        aa.doAccessibleAction(i);
                    }

	        } else if (tags[0].equals("highlight")) {

	            String s = result.getBestToken(1).getSpokenText();
	            synthesizer.speak("Highlighting: " + s, null);

                    AccessibleContext ac = getAccessibleContextFromTable(highlightItems, s);

                    highlightAccessibleContextBounds(ac);

	        } else if (tags[0].equals("refresh")) {

	            synthesizer.speak("Refreshing grammar...", null);
                    try {
                        refreshAccessibleGrammars(ruleGrammar);
                        infoPanel.updateInfo(ruleGrammar);

 	                // update the recognizer with the new info
	                recognizer.commitChanges();
	                recognizer.resume();
                   } catch (Throwable t) {
             	        t.printStackTrace();
                   }

	        } else if (tags[0].equals("bye")) {

	            synthesizer.speak(goodbyeStr, exitWhenDone);

	        } else if (tags[0].equals("Perform") || tags[0].equals("perform")) {

                    System.out.println("Recognized 'peform'");
                    String token1 = result.getBestToken(0).getSpokenText();
                    String token2 = result.getBestToken(1).getSpokenText();
                    if (allActions.contains(token1)) {
                        AccessibleContext ac = (AccessibleContext) visibleItems.get(token2);
                        if (ac != null) {
                            AccessibleAction aa = ac.getAccessibleAction();
                            if (aa != null) {
                                for (int i = 0; i < aa.getAccessibleActionCount(); i++) {
                                    if (token1.equals(aa.getAccessibleActionDescription(i))) {
                                        synthesizer.speak("Doing " + token1 + " on " + token2, null);
                                        aa.doAccessibleAction(i);
                                    }
                                }
                            }
                        }
                    }
                }
            }
	}

	// rejected result
	int i = 0;
	public void rejectedResult(ResultEvent e) {
	    String eh[] = {"Huh?", "Eh?", "Pardon?", "What?"};
	    synthesizer.speak(eh[(i++)%eh.length], null);
	}
    };


    // walk Accessible heirarchy; 
    // build a table of AccessibleNames of the visible Accessible objects
    //
    static public AccessibleContext getAccessibleContextAtPoint() {
	Point currentMousePos = EventQueueMonitor.getCurrentMousePosition();
        Accessible a = null;
	if (activeMenu != null) {
	    JPopupMenu pm = activeMenu.getPopupMenu();
	    Point menuLoc = pm.getLocationOnScreen();
	    Point menuPoint = new Point(currentMousePos.x - menuLoc.x,
	    currentMousePos.y - menuLoc.y);
	    Component c = pm.getComponentAt(menuPoint);
	    if (c instanceof Accessible) {
		a = (Accessible) c;
	    }
	}
	if (a == null) {
	    a = EventQueueMonitor.getAccessibleAt(currentMousePos);
	}
        if (a != null) {
	AccessibleContext ac = a.getAccessibleContext();
	Point containerPoint = null;
	    if (ac != null) {
		AccessibleComponent acmp = ac.getAccessibleComponent();
		if (acmp != null) {
		    Point containerLoc = acmp.getLocationOnScreen();
		    if (containerLoc != null) {
			containerPoint = 
			    new Point(currentMousePos.x - containerLoc.x,
			    currentMousePos.y - containerLoc.y);
		    } 
		}
		return ac;
	    }	
	}
        return null;
    }

    // walk Accessible heirarchy; 
    // build a table of AccessibleNames of the visible Accessible objects
    //
    static Hashtable buildNamesTable(AccessibleContext root, StringVector actions) {
        Hashtable table = new Hashtable(50);
        if (root != null) {
            String name = root.getAccessibleName();
            if (name == null) {
                name = new String("root");
            }
            table.put(name, root);
            int childCount = root.getAccessibleChildrenCount();
            if (childCount != 0) {
                for (int i = 0; i < childCount; i++) {
                    addAccessibleNode(root.getAccessibleChild(i), table, actions);
                }
            }
        }
        return table;
    }

    // add a node (and it's children) to Accessible hashtable
    //
    static public void addAccessibleNode(Accessible node, Hashtable table, StringVector actions) {
        AccessibleContext ac = node.getAccessibleContext();
        Accessible child;
        AccessibleContext childAC;
        AccessibleComponent acmp;
        AccessibleAction aa;
        if (ac != null) {
            String name = ac.getAccessibleName();
            if (name != null) {
                table.put(name, ac);
            }
            aa = ac.getAccessibleAction();
            if (aa != null) {
//                System.out.println("Adding actions for: " + name + ", has " + aa.getAccessibleActionCount() + " actions");
                for (int i = 0; i < aa.getAccessibleActionCount(); i++) {
//                    System.out.println("  -> adding action name: " + aa.getAccessibleActionDescription(i));
                    actions.addElement(aa.getAccessibleActionDescription(i));
                }
            }
            int childCount = ac.getAccessibleChildrenCount();
            if (childCount != 0) {
                for (int i = 0; i < childCount; i++) {
                    child = ac.getAccessibleChild(i);
                    if (child != null) {
                        childAC = child.getAccessibleContext();
                        if (childAC != null) {
                            acmp = childAC.getAccessibleComponent();
                            if (acmp != null && acmp.isShowing()) {
                                addAccessibleNode(ac.getAccessibleChild(i), table, actions);
                            }
                        }
                    }
                }
            }
        }
    }


    // visually highlight an Accessible
    //
    public static void highlightAccessibleContextBounds(AccessibleContext ac) {
	Accessible parent;
        Component c;
        Graphics g;
        Rectangle b;

	if (ac == null) {
	    return;
	}

	AccessibleComponent acmp = ac.getAccessibleComponent();
	if (acmp == null) {
	    return;
	}

        b = acmp.getBounds();
        parent = ac.getAccessibleParent();
        while (parent != null && !(parent instanceof Component)) {
            ac = parent.getAccessibleContext();
            if (ac != null) {
                parent = ac.getAccessibleParent();
                if (ac == null) {
                    return;
                }
            }
        }
	if (parent instanceof Component) {
	    g = ((Component) parent).getGraphics();
	    if (g != null) {
		g.setXORMode(Color.pink);
		g.setColor(Color.black);
		g.fillRect(b.x, b.y, b.width, b.height);
	    }
        }
    }


    // Handle Menu events (track the activeMenu)
    //
    public void menuCanceled(MenuEvent theEvent) {
        activeMenu = null;
    }

    public void menuDeselected(MenuEvent theEvent) {
        activeMenu = null;
    }

    public void menuSelected(MenuEvent theEvent) {
        activeMenu = (JMenu) theEvent.getSource();
    }


    // start your engines!
    //
    public static void main(String args[]) {
        String vers = System.getProperty("java.version");
        if (vers.equals("1.1.2")) {
            System.out.println("!!!WARNING: Listener must be run with a " +
                               "1.1.2 or higher version VM!!!");
        }

	    new Listener();
    }


    // inner class for string-compare Vector
    //
    public class StringVector extends Vector {
        public boolean contains(String s) {
            for (int i = 0; i < elementCount; i++) {
                if (s.equals((String) elementAt(i))) {
                    return true;
                }
            }
            return false;
        }

        public void addElement(String s) {
            if (!contains(s)) {
                super.addElement(s);
            }
        } 
    }

    // inner class to display Speech Info
    //
    public class SpeechInfoPanel extends Panel {
    
        java.awt.List info;

        // Create the GUI
        //
        public SpeechInfoPanel() {
	    this(20);
        }
    
        public SpeechInfoPanel(int lines) {
            super();
	    setLayout(new BorderLayout());

            // Add Info List
            //
	    info = new java.awt.List(lines);
            add("Center", info);
        }
    
        public void updateInfo(RuleGrammar rg) {
            info.setVisible(false);
	    info.removeAll();
            info.addItem("Speech rules:");

            String ruleNames[] = rg.listRuleNames();
            for (short i = 0;  i < ruleNames.length;  i++) {
                info.addItem("    " + ruleNames[i] + ": " + rg.getRule(ruleNames[i]).toString());
            }
	    
            info.setVisible(true);
        }
    }
}

