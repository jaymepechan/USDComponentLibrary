# @(#)AccessBridgeJava.mak	1.16 05/07/25

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 

!ifndef OUTDIR
OUTDIR=.\Release
!endif

!ifndef INTDIR
INTDIR=.\tempFiles
!endif

PKG=com\sun\java\accessibility
SRC=..\src
CLS=classes\$(PKG)

JAR   = $(JDK_HOME)\bin\jar
JAVAC = $(JDK_HOME)\bin\javac -deprecation -g
JAVAH = $(JDK_HOME)\bin\javah -jni

FILES = "$(OUTDIR)\access-bridge.jar" \
	"$(INTDIR)\$(CLS)\AccessBridge.class" \
	"$(SRC)\AccessBridge.h"

all: "$(OUTDIR)" "$(INTDIR)\$(CLS)" $(FILES)

release: all

clean:
	rm -rf "$(INTDIR)"
	rm -f "$(SRC)\AccessBridge.h"

"$(OUTDIR)\access-bridge.jar" : $(INTDIR) $(INTDIR)\$(CLS)\AccessBridge.class
    cd "$(INTDIR)\classes"
	$(JAR) cf "..\..\$(OUTDIR)\access-bridge.jar" com
    cd ..\..

"$(INTDIR)\$(CLS)\AccessBridge.class" : $(SRC)\$(PKG)\AccessBridge.java
	$(JAVAC) -d $(INTDIR)\classes $?

"$(SRC)\AccessBridge.h" : "$(INTDIR)\$(CLS)\AccessBridge.class"
    cd "$(INTDIR)\classes"
	$(JAVAH) -o ..\..\..\src\AccessBridge.h $(PKG).AccessBridge 
    cd ..\..

"$(OUTDIR)" :
    if not exist "$(OUTDIR)\$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)\$(CLS)" :
    if not exist "$(INTDIR)\$(NULL)" mkdir "$(INTDIR)"
    if not exist "$(INTDIR)\classes\$(NULL)" mkdir "$(INTDIR)\classes"
    if not exist "$(INTDIR)\classes\com\$(NULL)" mkdir "$(INTDIR)\classes\com"
    if not exist "$(INTDIR)\classes\com\sun\$(NULL)" mkdir "$(INTDIR)\classes\com\sun"
    if not exist "$(INTDIR)\classes\com\sun\java\$(NULL)" mkdir "$(INTDIR)\classes\com\sun\java"
    if not exist "$(INTDIR)\classes\com\sun\java\accessibility\$(NULL)" mkdir "$(INTDIR)\classes\com\sun\java\accessibility"


