
				README

Welcome to JDK 1.3 release of the Java Accessibility Utilities!
This release includes the Java Accessibility Utilties, some examples, and
documentation of the Java Accessibility design and API.

NOTE: The package names for Swing and the Java Accessibility API
are now javax.swing and javax.accessibility.  They used to be 
com.sun.java.swing and com.sun.java.accessibility.  The sources in 
the Java Accessibility Utilities have been updated to use the new 
javax package names.  These Utilities will not work with any previous
release of the Swing classes in any previous JFC release.


This file has two sections:

- WHAT'S IN THIS RELEASE
- QUICK START


    NOTE:  To run the examples in this release of Java Accessibility
    Utilities, you need to obtain AND INSTALL JDK 1.3.

    To obtain JDK 1.3, connect to the following URL:

        http://java.sun.com/jdk


This release contains the following:

jaccess-1.3/README.txt			This file.
jaccess-1.3/LICENSE.txt			Software license.
jaccess-1.3/CHANGES.txt			Changes since the last release.
jaccess-1.3/NOTES.txt			Notes for reviewers (you).
jaccess-1.3/jaccess.jar   		The Java Accessibility Utilities.
					        	DO NOT UNARCHIVE THIS FILE!
jaccess-1.3/doc/index.html		The main page for documentation.
jaccess-1.3/src.zip		    	The source code for this release.
jaccess-1.3/examples			Example uses of the Java Accessibility
					        	Utilities and the Java Accessibility 
								API.
jaccess-1.3/jaccess-examples.jar  	The Java Accessibility Utilities
					            examples.   DO NOT UNARCHIVE THIS FILE!


QUICK START:

This section contains quick start directions on setting up the Java 
Accessibility Utilities package, once you have downloaded and installed 
jaccess-1.3 onto your system.

Please read the License (LICENSE.txt) before using this release.

To read the Java Accessibility Utilities documentation, view the following 
file from the installed jaccess-1.3 package in a Web browser:

	jaccess-1.3/doc/index.html

To run a sample application, named JavaMonitor, that provides small 
sample set of Java Accessibility features, do the following:

1. Place the jaccess.jar and jaccess-examples.jar files in
   your CLASSPATH environment variable.  For example, on Solaris:

	setenv CLASSPATH $CLASSPATH:/home/me/jaccess-1.3/jaccess.jar:/home/me/jaccess-1.3/jaccess-examples.jar

   Or, on Win95:

	set CLASSPATH=%CLASSPATH%;C:\jaccess-1.3\jaccess.jar;C:\jaccess-1.3\jaccess-examples.jar

2. Modify the lib/awt.properties file under your JDKHOME directory (e.g., 
   /home/me/jdk1.3/jre/lib/accessibility.properties) to include the 
   following line:

        AWT.assistive_technologies=JavaMonitor

   The file, jaccess-1.3/doc/awt.properties.sample, provides an example 
   of what the final edited version might look like.  

3. Run any Java application using the 'java' command provided with
   the JDK.  For example, run the SwingSet example that ships with the
   Java Foundation Classes.  When you run the Java application, 
   the JavaMonitor will automatically start.  For more information on the 
   JavaMonitor, view the following file from the installed jaccess-1.3
   package in a Web browser:

       jaccess-1.3/doc/examples.html

Have fun trying out the Java Accessibility Utilities!  We look forward
to the feedback you will be sending to access@sun.com.  For more
general information on Java Accessibility, please refer to the
following URL:

        http://www.sun.com/access

--The Java Accessibility Team
