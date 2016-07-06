#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

#
# @(#)makefile	1.3 02/01/17
#

# @(#)makefile	1.0 97/08/02

JDKHOME = c:\jdk1.1.4
SWINGHOME = c:\swing
JACCESSHOME = c:\jaccess
JSAPIHOME = c:\speech
JSAPIIBMHOME = d:\JavaSpeech_for_IBM\lib
CSEP = ;

SWINGJARS=$(SWINGHOME)\swing.jar
JACCESSJARS=$(JACCESSHOME)\build\jaccess.jar
#JSAPIJARS=$(JSAPIHOME)\jsapi.jar;$(JSAPIIMBHOME)\speechx.jar;$(JSAPIIBMHOME)\ibmjs.jar
JSAPIJARS=$(JSAPIIBMHOME)\speechx.jar;$(JSAPIIBMHOME)\ibmjs.jar

CLASSPATH = .;$(JACCESSJARS);$(SWINGJARS);$(JSAPIJARS);$(JDKHOME)\lib\classes.zip

JAVAC = $(JDKHOME)\bin\javac -deprecation -g -classpath $(CLASSPATH)

FILES = Listener.class

all: $(FILES)

release: all

clean:
	rm -f *.class *~

Listener.class: Listener.java
	$(JAVAC) $?
