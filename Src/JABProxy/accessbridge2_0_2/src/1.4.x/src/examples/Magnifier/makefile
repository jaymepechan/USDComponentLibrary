#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

#
# @(#)makefile	1.5 02/01/17
#

# @(#)makefile	1.0 97/07/23

JDKHOME = c:\jdk1.1.3
SWINGHOME = c:\swing-0.3
JACCESSHOME = c:\jaccess-0.3
CSEP = ;

SWING_JARS=$(SWINGHOME)\swing.jar
JACCESS_JARS=$(JACCESSHOME)\jaccess.jar

CLASSPATH = .;$(JACCESS_JARS);$(SWING_JARS);$(JDKHOME)\lib\classes.zip

JAVAC = $(JDKHOME)\bin\javac -deprecation -g -classpath $(CLASSPATH)

FILES = Magnifier.class

all: $(FILES)

release: all

clean:
	rm -f *.class *~

Magnifier.class: Magniifer.java
	$(JAVAC) $?
