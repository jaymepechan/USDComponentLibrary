#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)GNUmakefile	1.15 02/01/17
#
# Makefile for use on Solaris using gnumake.  It assumes the environment
# variables JDK_HOME (points to the JDK), SWING_HOME (points to Swing), and
# JACCESS_HOME (Points to JAccess) are defined.

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

CLASSPATH = ".$(CSEP)$(JACCESSCLASSES)$(CSEP)$(CLASSES)"

JAVAC = $(JDK_HOME)/bin/javac $(DEPRECATION) -g -classpath $(CLASSPATH)

FILES = AWTMonitor.class

all: $(FILES)

release: all

clean clobber:
	rm -f *.class *~

%.class: %.java
	$(JAVAC) $?
