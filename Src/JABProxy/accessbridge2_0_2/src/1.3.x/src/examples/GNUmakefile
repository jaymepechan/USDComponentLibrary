#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

#
# @(#)GNUmakefile	1.27 02/01/17
#

#
# for alternate configurations, such as Win32
#
include ../../build/config

SUBDIRS = AccessibilityPanels AWTMonitor JavaMonitor AccessibilityMonitor Ferret Monkey Manipulator Selector Linker Mouser
RELEASE_HOME = $(shell cd ../../build; echo `pwd`/release_doc/examples)

all clean clobber ::
	rm -f *.class *~
	@for i in $(SUBDIRS) ; do \
		cd $$i; $(MAKE) "JDK_HOME=$(JDK_HOME)" "CSEP=$(CSEP)" $@ || exit 1; cd ..; \
	done

release:
	for i in $(SUBDIRS) ; do \
		mkdir -p $(RELEASE_HOME)/$$i ; \
		cd $$i; find . -name SCCS -prune -o -type f -print | \
		cpio -pdm $(RELEASE_HOME)/$$i ; cd ..; \
	done

