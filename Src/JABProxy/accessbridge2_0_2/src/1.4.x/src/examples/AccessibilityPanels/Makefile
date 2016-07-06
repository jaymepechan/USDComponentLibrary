#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)Makefile	1.5 02/01/17

!ifndef JDK_HOME
JDK_HOME = c:\java
!endif
!ifndef SWING_HOME
SWING_HOME = c:\swing
!endif
!ifndef JACCESS_HOME
JACCESS_HOME = ..\..\..\build
!endif
CSEP = ;

CLASSPATH = .;..\..\..\build\classes;$(SWING_HOME);$(SWING_HOME)\swing.jar;$(JDK_HOME)\lib\classes.zip

JAVAC = $(JDK_HOME)\bin\javac -deprecation -g -classpath $(CLASSPATH)

FILES = AccessibilityPanel.class \
	AccessibilityAPIPanel.class \
	AccessibleActionPanel.class \
	AccessibleSelectionPanel.class \
	AccessibleHypertextPanel.class

all: $(FILES)

release: all

clean clobber:
	rm -f *.class *~

AccessibilityPanel.class: AccessibilityPanel.java
	$(JAVAC) $?

AccessibilityAPIPanel.class: AccessibilityAPIPanel.java
	$(JAVAC) $?

AccessibleActionPanel.class: AccessibleActionPanel.java
	$(JAVAC) $?

AccessibleSelectionPanel.class: AccessibleSelectionPanel.java
	$(JAVAC) $?

AccessibleHypertextPanel.class: AccessibleHypertextPanel.java
	$(JAVAC) $?
