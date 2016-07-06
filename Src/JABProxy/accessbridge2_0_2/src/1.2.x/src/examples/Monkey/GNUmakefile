#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)GNUmakefile	1.16 02/01/17
#
# Makefile for use on Solaris using gnumake.  It assumes an environment
# variable called JDK_HOME is defined.

TOPLEVEL = $(shell echo `pwd`)

include ../../../build/config

ifeq ($(JACCESS_HOME),)
JACCESSCLASSES=../../../build/classes
else
JACCESSCLASSES=$(JACCESS_HOME)/jaccess.jar
endif
ifeq ($(CSEP),)
CSEP = :
endif

CLASSPATH = ".$(CSEP)../AccessibilityPanels$(CSEP)$(JACCESSCLASSES)$(CSEP)$(CLASSES)"

JAVAC = $(JDK_HOME)/bin/javac $(DEPRECATION) -g -classpath $(CLASSPATH)

FILES = Monkey.class \
	AccessibleNode.class \
	ComponentNode.class

all: $(FILES)

release: all

clean clobber:
	rm -f *.class *~

%.class: %.java
	$(JAVAC) $?
