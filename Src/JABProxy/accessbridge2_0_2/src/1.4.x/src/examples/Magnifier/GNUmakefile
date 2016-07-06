#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)GNUmakefile	1.7 02/01/17
#
# Makefile for use on Solaris using gnumake.  It assumes an environment
# variable called JDKHOME is defined.

TOPLEVEL = $(shell echo `pwd`)

ifeq ($(JDKHOME),)
JDKHOME = /usr/local/java/jdk1.1.3/solaris
endif
ifeq ($(SWING_HOME),)
SWING_HOME = $(shell if [ -d ../../../build ] ; then (cd ../../../.. ; echo `pwd`/swing-0.4) else echo `pwd`/../../../swing-0.4 ; fi)
endif
ifeq ($(JACCESS_HOME),)
JACCESS_HOME = $(shell if [ -d ../../../build ] ; then (cd ../../../build ; echo `pwd`) else echo `pwd`/../.. ; fi)
endif
ifeq ($(CSEP),)
CSEP = :
endif

SWING_JARS=$(SWING_HOME)/swing.jar
JACCESS_JARS=$(JACCESS_HOME)/jaccess.jar

CLASSPATH = ".$(CSEP)$(JACCESS_JARS)$(CSEP)$(SWING_JARS)$(CSEP)$(JDKHOME)/lib/classes.zip"

JAVAC = $(JDKHOME)/bin/javac -deprecation -g -classpath $(CLASSPATH)

FILES = Magnifier.class

all: $(FILES)

release: all

clean:
	rm -f *.class *~

%.class: %.java
	$(JAVAC) $?
