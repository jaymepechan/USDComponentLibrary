#
# Copyright 2002 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

# @(#)Makefile	1.14 02/01/17
# NMAKE makefile

SUBDIRS = AccessibilityPanels AWTMonitor JavaMonitor AccessibilityMonitor Ferret Monkey Manipulator Selector Mouser

default: $(SUBDIRS)

all clean clobber:
	nmake -nologo action=$@

$(SUBDIRS) : FORCE
	cd $@
	nmake -nologo $(action)
	cd ..

FORCE :
