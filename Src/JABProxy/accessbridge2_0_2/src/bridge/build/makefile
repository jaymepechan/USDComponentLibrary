#
# Copyright 2005 Sun Microsystems, Inc. All rights reserved.
# SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
#

#
# @(#)makefile	1.75 06/06/22
#

#
# Makefile for AccessBridge 2.0.1
#

RELEASE_NAME=AccessBridge_2_0_1
MANUAL_NAME=AccessBridge_2_0_1_manual_install
RELEASE_DIR=..\$(RELEASE_NAME)	

CP=cp
RM=rm -rf

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 

!ifndef DEV_HOME
DEV_HOME = "c:\Program Files\Microsoft Visual Studio\VC98"
!endif

!ifndef JDK_HOME
JDK_HOME = c:\j2sdk1.4.2
!endif

!ifndef SRC
SRC=..\..
!endif

!ifndef JACCESS_1_2_SRC
JACCESS_1_2_SRC=1.2.x
!endif

!ifndef JACCESS_1_3_SRC
JACCESS_1_3_SRC=1.3.x
!endif

!ifndef JACCESS_1_4_SRC
JACCESS_1_4_SRC=1.4.x
!endif

!ifndef OUTDIR
OUTDIR=.\Release
!endif

!ifndef MANUAL_DIR
MANUAL_DIR=.\manual
!endif

!ifndef INTDIR
INTDIR=.\tempFiles
!endif

!ifndef JACCESS_1_2
JACCESS_1_2=..\..\1.2.x\build\jaccess.jar
!endif

!ifndef JACCESS_1_3
JACCESS_1_3=..\..\1.3.x\build\jaccess.jar
!endif

!ifndef JACCESS_1_4
JACCESS_1_4=..\..\1.4.x\build\jaccess.jar
JACCESS_EXAMPLES_1_4=..\..\1.4.x\build\jaccess-examples.jar
!endif

!ifndef PKZIP
PKZIP = c:\utils\pkzipc.exe
!endif

!ifndef SRC_ZIPFILE
SRC_ZIPFILE = $(OUTDIR)\src.zip
!endif


#
# Build targets
#

MAKEFILES = \
	AccessBridgeJava.mak \
	JavaAccessBridgeDLL.mak \
	WindowsAccessBridgeDLL.mak \
	installerDLL.mak \
	installer.mak \
	JavaFerret.mak \
	JavaMonkey.mak \
	JAWTAccessBridgeDLL.mak

default: "$(INTDIR)" $(MAKEFILES)

all : \
	"$(INTDIR)" "$(OUTDIR)" \
	"$(OUTDIR)\installer" \
	"$(OUTDIR)\installer\installerFiles" \
	"$(OUTDIR)\$(RELEASE_DIR)" \
	"$(OUTDIR)\$(RELEASE_DIR)\installer" \
	"$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles"
    nmake action=$@ JAWT_HOME=$(JAWT_HOME)

clean clobber:
    nmake action=$@
    $(RM) -rf "$(OUTDIR)"
    $(RM) -rf "$(INTDIR)"
    $(RM) -rf ""$(RELEASE_DIR).zip"

$(MAKEFILES) : FORCE
    nmake -f $@ $(action) JAWT_HOME=$(JAWT_HOME)

FORCE:  

# for GA (General Availability)
# release: src install
release: install manual

# for BETA
# release: install

manual: release-files
    if not exist "$(MANUAL_DIR)" mkdir "$(MANUAL_DIR)"

# installer files
    if not exist "$(MANUAL_DIR)\installerFiles\" mkdir "$(MANUAL_DIR)\installerFiles"

    $(CP) "release\installer\installerFiles\access-bridge.jar" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\accessibility.properties" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\jaccess-1_2.jar" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\jaccess-1_3.jar" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\jaccess-1_4.jar" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\jaccess-examples.jar" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\JavaAccessBridge.DLL" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\JAWTAccessBridge.DLL" "$(MANUAL_DIR)\installerFiles"
    $(CP) "release\installer\installerFiles\WindowsAccessBridge.DLL" "$(MANUAL_DIR)\installerFiles"

# utilities
    $(CP) "Release\JavaFerret.exe" "$(MANUAL_DIR)"
    $(CP) "Release\JavaMonkey.exe" "$(MANUAL_DIR)"

# source
    $(CP) "Release\src.zip" "$(MANUAL_DIR)"

# docs
    if not exist "$(MANUAL_DIR)\doc" mkdir "$(MANUAL_DIR)\doc"
    if not exist "$(MANUAL_DIR)\doc\images" mkdir "$(MANUAL_DIR)\doc\images"
    $(CP) "Release\binary-license.txt" "$(MANUAL_DIR)"
    $(CP) "Release\binary-license.html" "$(MANUAL_DIR)"
    $(CP) Release\doc\*.* "$(MANUAL_DIR)\doc"
    $(CP) Release\doc\images\*.* "$(MANUAL_DIR)\doc\images"
    cd $(MANUAL_DIR)
    "$(PKZIP)" -dir=current -add "..\$(MANUAL_NAME).zip" -recurse -include "*.*"
    cd ..

install: release-files
    $(CP) $(OUTDIR)\installer\*.* "$(OUTDIR)\$(RELEASE_DIR)\installer\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\JavaAccessBridge.dll "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\JAWTAccessBridge.dll "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\WindowsAccessBridge.dll "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\access-bridge.jar "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\."
    $(CP) ..\src\accessibility.properties "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\."
    $(CP) $(JACCESS_1_2) "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\jaccess-1_2.jar"
    $(CP) $(JACCESS_1_3) "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\jaccess-1_3.jar"
    $(CP) $(JACCESS_1_4) "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles\jaccess-1_4.jar"
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\JavaAccessBridge.dll "$(OUTDIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\JAWTAccessBridge.dll "$(OUTDIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\WindowsAccessBridge.dll "$(OUTDIR)\installer\installerFiles\."
    $(CP) $(OUTDIR)\$(RELEASE_DIR)\access-bridge.jar "$(OUTDIR)\installer\installerFiles\."
    $(CP) ..\src\accessibility.properties "$(OUTDIR)\installer\installerFiles\."
    $(CP) $(JACCESS_1_2) "$(OUTDIR)\installer\installerFiles\jaccess-1_2.jar"
    $(CP) $(JACCESS_1_3) "$(OUTDIR)\installer\installerFiles\jaccess-1_3.jar"
    $(CP) $(JACCESS_1_4) "$(OUTDIR)\installer\installerFiles\jaccess-1_4.jar"
    $(CP) $(JACCESS_EXAMPLES_1_4) "$(OUTDIR)\installer\installerFiles\jaccess-examples.jar"
    pwd
    "$(PKZIP)" -dir=current -add "$(RELEASE_NAME).zip" -recurse -include "$(RELEASE_NAME)\*"

release-files: $(MAKEFILES) all doc src
    if not exist "$(OUTDIR)" mkdir "$(OUTDIR)"
    if not exist "$(OUTDIR)" mkdir "$(RELEASE_DIR)"
    if not exist "$(OUTDIR)\$(RELEASE_DIR)" mkdir "$(OUTDIR)\$(RELEASE_DIR)"
    if not exist "$(OUTDIR)\$(RELEASE_DIR)\doc" mkdir "$(OUTDIR)\$(RELEASE_DIR)\doc"
    if not exist "$(OUTDIR)\$(RELEASE_DIR)\doc\images" mkdir "$(OUTDIR)\$(RELEASE_DIR)\doc\images"
    $(CP) $(OUTDIR)\*.* "$(OUTDIR)\$(RELEASE_DIR)\."
    $(RM) $(OUTDIR)\$(RELEASE_DIR)\*.lib
    $(RM) $(OUTDIR)\$(RELEASE_DIR)\*.exp
    $(CP) ..\doc\*.* "$(OUTDIR)\$(RELEASE_DIR)\doc\."
    $(CP) ..\doc\images\*.* "$(OUTDIR)\$(RELEASE_DIR)\doc\images\."

BRIDGE_INCLUDES = \
	-include="$(SRC)\buildall.bat" \
	-include="$(SRC)\*.c" \
	-include="$(SRC)\*.cpp" \
	-include="$(SRC)\*.h" \
	-include="$(SRC)\*.def" \
	-include="$(SRC)\*.ds*" \
	-include="$(SRC)\*.properties" \
	-include="$(SRC)\*.rc" \
	-include="$(SRC)\*.gif" \
	-include="$(SRC)\*.txt" \
	-include="$(SRC)\*.htm*" \
	-include="$(SRC)\*.java" \
	-include="$(SRC)\config" \
	-include="$(SRC)\*akefile" \
	-include="$(SRC)\*.mak"

EXCLUDES = \
	-exclude="Release" \
	-exclude="tempFiles" \
	-exclude="Codemgr_wsdata" \
	-exclude="$(RELEASE_NAME)" \
	-exclude="*~" \
	-exclude="s.*" \
	-exclude="p.*"

src:
	pwd
	$(RM) $(SRC_ZIPFILE)
	$(PKZIP) -add $(SRC_ZIPFILE) -dir=relative $(BRIDGE_INCLUDES) $(EXCLUDES)

doc: "$(INTDIR)" "$(OUTDIR)"
    if not exist "$(OUTDIR)\doc" mkdir "$(OUTDIR)\doc"
    if not exist "$(OUTDIR)\doc\images" mkdir "$(OUTDIR)\doc\images"
    $(CP) ..\doc\*.* "$(OUTDIR)\doc\."
    $(CP) ..\doc\images\*.* "$(OUTDIR)\doc\images\."
# beta  $(CP) ..\eval-license.txt "$(OUTDIR)\." 
# beta  $(CP) ..\eval-license.html "$(OUTDIR)\." 
# beta  $(CP) ..\eval-license.rtf "$(OUTDIR)\." 
# GA    $(CP) ..\binary-license.txt "$(OUTDIR)\." 
# GA    $(CP) ..\binary-license.html "$(OUTDIR)\." 
    $(CP) ..\binary-license.txt "$(OUTDIR)\." 
    $(CP) ..\binary-license.html "$(OUTDIR)\." 
    $(CP) ..\binary-license.rtf "$(OUTDIR)\." 
    $(CP) ..\README.txt "$(OUTDIR)\."
    $(CP) ..\NOTES.txt "$(OUTDIR)\."
    $(CP) ..\CHANGES.txt "$(OUTDIR)\."

# directories
"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(OUTDIR)\installer" :
    if not exist "$(OUTDIR)\installer/$(NULL)" mkdir "$(OUTDIR)\installer"

"$(OUTDIR)\installer\installerFiles" :
    if not exist "$(OUTDIR)\installer\installerFiles/$(NULL)" mkdir "$(OUTDIR)\installer\installerFiles"

"$(OUTDIR)\$(RELEASE_DIR)" :
    if not exist "$(OUTDIR)\$(RELEASE_DIR)/$(NULL)" mkdir "$(OUTDIR)\$(RELEASE_DIR)"

"$(OUTDIR)\$(RELEASE_DIR)\installer" :
    if not exist "$(OUTDIR)\$(RELEASE_DIR)\installer/$(NULL)" mkdir "$(OUTDIR)\$(RELEASE_DIR)\installer"

"$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles" :
    if not exist "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles/$(NULL)" mkdir "$(OUTDIR)\$(RELEASE_DIR)\installer\installerFiles"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"


