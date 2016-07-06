#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

#
# @(#)GNUmakefile	1.7 02/01/17
#

# @(#)GNUmakefile	1.7 97/12/19
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

CLASSPATH = ".$(CSEP)../AccessibilityPanels$(CSEP)../ibmjs/lib/ibmjs.jar$(CSEP)$(JACCESSCLASSES)$(CSEP):$(CLASSES)"

JAVAC = $(JDK_HOME)/bin/javac -deprecation -g -classpath '$(subst :,$(CSEP),$(CLASSPATH))'
JAVAC = $(JDK_HOME)/bin/javac -deprecation -g -classpath $(CLASSPATH)

FILES = Talker.class

all: $(FILES)

release: all

clean clobber:
	rm -f *.class *~

%.class: %.java
	$(JAVAC) $?
