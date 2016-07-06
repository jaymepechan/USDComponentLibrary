/*
 * Copyright 2002 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)Hello.java	1.3 02/01/17
 */

package hello;

import java.io.*;
import java.util.Locale;
import java.util.ResourceBundle;
import java.util.StringTokenizer;
import javax.speech.*;
import javax.speech.recognition.*;
import javax.speech.synthesis.*;

public class Hello {

    static RuleGrammar ruleGrammar;
    static DictationGrammar dictationGrammar;
    static Recognizer recognizer;
    static Synthesizer synthesizer;
    static ResourceBundle resources;

    //
    // This is the listener for rule grammar results.  The
    // acceptedResult method is called when the user issues a command.
    // We then request the tags that we associated with the grammar in
    // hello.gram, and take an action based on the tag.  Using tags
    // rather than looking directly at what the user said means we can
    // change the grammar without having to change our code.
    //
    static GrammarListener ruleListener = new GrammarResultAdapter() {

        // accepted result
        public void acceptedResult(ResultEvent e) {
            try {

		// following two lines were changed for version 0.7
                FinalRuleResult result = (FinalRuleResult) e.getSource();
                String tags[] = result.getTags();

                // The user has said "my name is ..."
                if (tags[0].equals("name")) {
                    String s = resources.getString("hello");
                    for (int i=1; i<tags.length; i++)
                        s += " " + tags[i];
                    speak(s, false);

                // the user has said "repeat after me"
                } else if (tags[0].equals("begin")) {
                    speak(resources.getString("listening"), false);
                    ruleGrammar.setActive(false);
                    ruleGrammar.setActive("<stop>", true);
                    dictationGrammar.setActive(true);
                    recognizer.commitChanges();
                
                // the user has said "that's all"
                } else if (tags[0].equals("stop")) {
                    dictationGrammar.setActive(false);
                    ruleGrammar.setActive(true);
                    recognizer.commitChanges();

                // the user has said "good bye"
                } else if (tags[0].equals("bye")) {
                    speak(resources.getString("bye"), true);
                }

            } catch (GrammarException ex) {
                ex.printStackTrace();
            }
        }

        // rejected result - say "eh?" etc.
        int i = 0;
	String eh[] = null;
        public void rejectedResult(ResultEvent e) {
	    if (eh==null) {
		String s = resources.getString("eh");
		StringTokenizer t = new StringTokenizer(s);
		int n = t.countTokens();
		eh = new String[n];
		for (int i=0; i<n; i++)
		    eh[i] = t.nextToken();
	    }
            speak(eh[(i++)%eh.length], false);
        }
    };


    //
    // This is the listener for dictation results.  The
    // recognitionUpdate method is called for every recognized token.
    // The acceptedResult method is called when the dictation result
    // completes, which in this application occurs when the user says
    // "that's all".
    //
    static GrammarListener dictationListener = new GrammarResultAdapter() {

	int n = 0; // number of tokens seen so far

	public void recognitionUpdate(ResultEvent e) {
            Result result = (Result) e.getSource();
	    for (int i=n; i<result.numTokens(); i++)
		System.out.println(result.getBestToken(i).getSpokenText());
	    n = result.numTokens();
	}

        public void acceptedResult(ResultEvent e) {
            Result result = (Result) e.getSource();
            int n = result.numTokens();
            String s = "";
            for (int i=0; i<n; i++)
                s += result.getBestToken(i).getSpokenText() + " ";
            speak(s, false);
        }
    };


    //
    // Audio listener prints out audio levels to help diagnose problems.
    //
    static RecognizerAudioListener audioListener =new RecognizerAudioAdapter(){
	public void audioLevel(AudioLevelEvent e) {
	    System.out.println("volume " + e.getAudioLevel());
	}
    };


    //
    // This is a listener that exits the application when the
    // speak method it is used with finishes speaking.
    //
    static SpeakableListener exitWhenDone = new SpeakableAdapter() {
        public void endSpeakable(SpeakableEvent e) {
            System.exit(0);
        }
    };


    //
    // Here's a method to say something, and maybe exit when done
    // speaking.  If the synthesizer isn't available, we just print
    // the message.
    //
    static void speak(String s, boolean exit) {
	if (synthesizer!=null) {
	    synthesizer.speak(s, exit? exitWhenDone : null);
	} else {
	    System.out.println(s);
	    if (exit) System.exit(0);
	}
    }


    //
    // main
    //
    public static void main(String args[]) {

        try {

	    // locale, resources
	    if (args.length>0) Locale.setDefault(new Locale(args[0], ""));
	    if (args.length>1) Locale.setDefault(new Locale(args[0], args[1]));
	    System.out.println("locale is " + Locale.getDefault());
	    resources = ResourceBundle.getBundle("res");

            // create a recognizer matching default locale, add audio listener
            recognizer = Central.createRecognizer(null);
            recognizer.allocate();
	    recognizer.getAudioManager().addAudioListener(audioListener);

            // create dictation grammar
            dictationGrammar = recognizer.getDictationGrammar();
            dictationGrammar.addGrammarListener(dictationListener);
            
            // create a rule grammar, activate it
            Reader reader = new FileReader(resources.getString("grammar"));
            ruleGrammar = (RuleGrammar) recognizer.loadJSGF(reader);
            ruleGrammar.addGrammarListener(ruleListener);
            ruleGrammar.setActive(true);
        
            // commit new grammars, start recognizer
            recognizer.commitChanges();
            recognizer.resume();

            // create a synthesizer, speak a greeting
            synthesizer = Central.createSynthesizer(null);
	    if (synthesizer!=null) synthesizer.allocate();
            speak(resources.getString("greeting"), false);

        } catch (Exception e) {

            e.printStackTrace();
	    System.exit(-1);

        }
    }

}

