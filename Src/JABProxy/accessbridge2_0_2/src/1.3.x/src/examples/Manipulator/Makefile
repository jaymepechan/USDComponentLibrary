#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)Makefile	1.8 02/01/17

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

CLASSPATH = .;..\AccessibilityPanels;..\..\..\build\classes;$(SWING_HOME);$(SWING_HOME)\swing.jar;$(JDK_HOME)\lib\classes.zip

JAVAC = $(JDK_HOME)\bin\javac -deprecation -g -classpath $(CLASSPATH)

FILES = Manipulator.class

all: $(FILES)

release: all

clean clobber:
	rm -f *.class *~

Manipulator.class: Manipulator.java
	$(JAVAC) $?

ManipulatorPanel.class: ManipulatorPanel.java
	$(JAVAC) $?
