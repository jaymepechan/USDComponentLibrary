                                README

Welcome to this release of the Java(TM) Access Bridge for the
Microsoft Windows(TM) Operating System  This release includes one
Java class, three Java Access Bridge DLLs, two example applications
and documentation for the Java Access Bridge API.

NOTE: This release includes what is needed to use the Java Access
Bridge on JDK 1.2 or later. If you are using JDK 1.1.x, you need 
to install and run Java Access Bridge for Windows 1.0.2.

You must have JDK 1.2 or later installed on your Windows system.
To obtain the Java 2 SDK, connect to the following URL:

        http://java.sun.com/j2se/

This file has three sections:

   - WHAT'S IN THIS RELEASE
   - REDISTRIBUTABLES
   - INSTALLER SOURCE FILES
   - QUICK START


WHAT'S IN THIS RELEASE:
======================

This release contains the following:

Program Files\Java Access Bridge\README.txt      This file.
Program Files\Java Access Bridge\binary-license.txt  Software binary license.
Program Files\Java Access Bridge\binary-license.html Software binary license in HTML.
Program Files\Java Access Bridge\CHANGES.txt     Changes since the last 
                                                 release.
Program Files\Java Access Bridge\NOTES.txt       Notes for reviewers.
Program Files\Java Access Bridge\installer.exe   The installer program to install
                                                 the Java Access Bridge.
Program Files\Java Access Bridge\installerDLL.dll 
                                                 A .dll file used by the insatller
                                                 program.
Program Files\Java Access Bridge\access-bridge.jar
                                                 The Java language classfile
                                                 for the AccessBridge:
                                           	 DO NOT UNARCHIVE THIS FILE!
Program Files\Java Access Bridge\JavaAccessBridge.dll
                                                 The AccessBridge DLL invoked 
                                                 by the AccessBridge Java 
                                                 language classfile loaded 
                                                 into the Java Virtual Machine*.
Program Files\Java Access Bridge\WindowsAccessBridge.dll
                                                 The AccessBridge DLL invoked 
                                                 by the AccessBridge Java 
                                                 language classfile loaded into
                                                 the Java VM.
Program Files\Java Access Bridge\JAWTAccessBridge.dll
                                                 The AccessBridge DLL invoked 
                                                 by the AccessBridge Java 
                                                 language classfile loaded into
                                                 the Java VM.
Program Files\Java Access Bridge\src.zip         Source to the examples
                                           	 NOTE: You must unzip this file
                                                 before attempting to follow a
                                                 link from the documentation to
                                                 a source file.
Program Files\Java Access Bridge\JavaFerret.exe  An example application that uses 
                                                 the Java Accessibility Utilities 
                                                 to examine accessible information 
                                                 about the objects in the Java VM.
Program Files\Java Access Bridge\JavaMonkey.exe  An example application that traverses
                                                 the component trees in a particular 
                                                 Java VM and presents the hierarchy
                                                 in a tree view.
Program Files\Java Access Bridge\installerFiles  A directory containing a
                                           	 redistributable files for
                                           	 the Java Access Bridge and the
                                           	 Java Accessibility Utilities.
Program Files\Java Access Bridge\doc             A directory containing documentation
                                           	 in HTML.  The main page is
                                                 Program Files\Java Access Bridge\doc\index.html



REDISTRIBUTABLES
================

DEVELOPING INSTALLERS.  Subject to the terms and conditions of the Software License Agreement 
and the obligations, restrictions, and exceptions set forth below, You may use and modify 
those source files identified below as installer source files ("the Installer Source Files") 
to develop installers for use with Your Programs ("Binary Installers").  Binary Installers 
are Redistributables. 

DISTRIBUTION BY DEVELOPERS.  Subject to the terms and conditions of the Software License 
Agreement and the obligations, restrictions, and exceptions set forth below, You may 
reproduce and distribute the portions of Software identified below ("Redistributable"), 
provided that:

(a) you distribute the Redistributable complete and unmodified and only bundled as part 
of Your applets and applications ("Programs"), 

(b) your Programs add significant and primary functionality to the Software 

(c) you distribute Redistributable for the sole purpose of running your Programs,

(d) you do not distribute additional software intended to replace any
component(s) of the Redistributable,

(e) you do not remove or alter any proprietary legends or notices contained in or on 
the Redistributable.
 
(f) you only distribute the Redistributable subject to a license agreement that protects 
Sun's interests consistent with the terms contained in the Software License Agreement, and

(g) you agree to defend and indemnify Sun and its licensors from and against any damages, 
costs, liabilities, settlement amounts and/or expenses  (including attorneys' fees) 
incurred in connection with any claim, lawsuit or action by any third party that arises 
or results from the use or distribution of any and all Programs and/or Redistributable.  

The following files are Redistributables:

  jaccess-1_2.jar         - in Program Files\Java Access Bridge\installerFiles\
  jaccess-1_3.jar         - in Program Files\Java Access Bridge\installerFiles\
  jaccess-1_4.jar         - in Program Files\Java Access Bridge\installerFiles\
  access-bridge.jar       - in Program Files\Java Access Bridge\installerFiles\
  JavaAccessBridge.dll    - in Program Files\Java Access Bridge\installerFiles\
  WindowsAccessBridge.dll - in Program Files\Java Access Bridge\installerFiles\
  JAWTAccessBridge.dll    - in Program Files\Java Access Bridge\installerFiles\

In addition, the following example properties file may be redistributed:

 accessibility.properties - in Program Files\Java Access Bridge\installerFiles\


INSTALLER SOURCE FILES:
======================
Finally, for purposes of making a custom installer or simply in order
to localize the installer to different languages and/or locales, the source 
files listed below, contained in the Program Files\Java Access Bridge\src.zip 
file, which together create the Installer.exe and AccessBridgeTester.class 
files, may be modified and custom binary installers may be built and 
redistributed from those files:

  AccessBridgeTester.h    - \bridge\installer
  installer.dsp           - \bridge\installer
  install.cpp             - \bridge\installer
  installer.dsw           - \bridge\installer
  main.cpp                - \bridge\installer

  AccessBridgeTester.h    - \bridge\installerDLL
  JVM.cpp                 - \bridge\installerDLL
  deinstall.cpp           - \bridge\installerDLL
  globals.cpp             - \bridge\installerDLL
  installerDLL.DEF        - \bridge\installerDLL
  installerDLL.cpp        - \bridge\installerDLL
  installerDLL.dsp        - \bridge\installerDLL
  installerDLL.dsw        - \bridge\installerDLL
  installerDLL.h          - \bridge\installerDLL
  installerDLL.rc         - \bridge\installerDLL
  resource.h              - \bridge\installerDLL
  search.cpp              - \bridge\installerDLL



QUICK START:
===========

This section contains quick start directions on setting up the
Java Access Bridge.

The Java Access Bridge includes a self-extracting installer file. 
The fastest way to get up and running is double click on the file called
accessbridge-2_0.exe.

This scans your system for installed Java virtual machines, tests them
to see whether they support the Java Access Bridge, and then installs 
the Java Access Bridge into each of those Java virtual machines.

Alternatively, you can modify each Java virtual machine by hand.  Detailed
instructions are in

  Program Files\Java Access Bridge\doc\setup.html

To read the Java Access Bridge documentation, view the following 
file from the installed Java Access Bridge in a Web browser:

  Program Files\Java Access Bridge\doc\index.html


Please send feedback to access@sun.com.  For more general 
information on Java Accessibility, please refer to the following URL:

        http://www.sun.com/access

--The Java Accessibility Team


Copyright (c) 2006 Sun Microsystems, Inc. All rights reserved.

  Use is subject to license terms.  Third-party software, including font 
  technology, is copyrighted and licensed from Sun suppliers.  Sun,  
  Sun Microsystems,  the Sun logo and  Java are trademarks or registered 
  trademarks of Sun Microsystems, Inc. in the U.S. and other countries.  
  Federal Acquisitions: Commercial Software - Government Users Subject to 
  Standard License Terms and Conditions.  

  Tous droits reserves.  Distribue par des licences qui en restreignent 
  l'utilisation.  Le logiciel detenu par des tiers, et qui comprend la 
  technologie relative aux polices de caracteres, est protege par un copyright 
  et licencie par des fournisseurs de Sun.  Sun, Sun Microsystems, le logo Sun 
  et Java sont des marques de fabrique ou des marques deposees de Sun 
  Microsystems, Inc. aux Etats-Unis et dans d'autres pays.  


*As used in this document, the terms "Java virtual machine" or "JVM" mean 
a virtual machine for the Java platform. 
