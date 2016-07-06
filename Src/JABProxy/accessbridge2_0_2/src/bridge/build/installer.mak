# Microsoft Developer Studio Generated NMAKE File, Based on installer.dsp
!IF "$(CFG)" == ""
CFG=installer - Win32 Release
!MESSAGE No configuration specified. Defaulting to installer - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "installer - Win32 Release" && "$(CFG)" != "installer - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "installer.mak" CFG="installer - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "installer - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "installer - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE 
!ERROR An invalid configuration is specified.
!ENDIF 

!IF "$(OS)" == "Windows_NT"
NULL=
!ELSE 
NULL=nul
!ENDIF 

CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "installer - Win32 Release"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\installer.exe" "$(OUTDIR)\AccessBridgeTester.class"


CLEAN :
	-@erase "$(INTDIR)\install.obj"
	-@erase "$(INTDIR)\main.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\installer.exe"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/MT /nologo /ML /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /Fp"$(INTDIR)\installer.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\installerDLL.res" /d "NDEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\installer.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /incremental:no /pdb:"$(OUTDIR)\installer.pdb" /machine:I386 /out:"$(OUTDIR)\installer.exe" 
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\globals.obj" \
	"$(INTDIR)\JVM.obj" \
	"$(INTDIR)\main.obj" \
	"$(INTDIR)\search.obj" \
	"$(INTDIR)\installerDLL.res" \
	"$(INTDIR)\install.obj" \
	"$(DEV_HOME)\lib\LIBCMT.LIB"

"$(OUTDIR)\installer.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "installer - Win32 Debug"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\installer.exe" "$(OUTDIR)\AccessBridgeTester.class"


CLEAN :
	-@erase "$(INTDIR)\install.obj"
	-@erase "$(INTDIR)\main.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\installer.exe"
	-@erase "$(OUTDIR)\installer.ilk"
	-@erase "$(OUTDIR)\installer.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"


CPP_PROJ=/MT /nologo /MDd /W3 /Gm /GX /ZI /Od /I "$(JDK_HOME)\include $(JDK_HOME)\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /Fp"$(INTDIR)\installer.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /GZ /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\installerDLL.res" /d "_DEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\installer.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /incremental:yes /pdb:"$(OUTDIR)\installer.pdb" /debug /machine:I386 /out:"$(OUTDIR)\installer.exe" /pdbtype:sept 
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\globals.obj" \
	"$(INTDIR)\JVM.obj" \
	"$(INTDIR)\main.obj" \
	"$(INTDIR)\search.obj" \
	"$(INTDIR)\installerDLL.res" \
	"$(INTDIR)\install.obj" \
	"$(DEV_HOME)\lib\LIBCMT.LIB"


"$(OUTDIR)\installer.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ENDIF 

.c{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.obj::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.c{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cpp{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<

.cxx{$(INTDIR)}.sbr::
   $(CPP) @<<
   $(CPP_PROJ) $< 
<<


!IF "$(NO_EXTERNAL_DEPS)" != "1"
!IF EXISTS("installer.dep")
!INCLUDE "installer.dep"
!ELSE 
!MESSAGE Warning: cannot find "installer.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "installer - Win32 Release" || "$(CFG)" == "installer - Win32 Debug"
SOURCE=..\src\AccessBridgeDebug.cpp

"$(INTDIR)\AccessBridgeDebug.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


SOURCE=..\installerDLL\globals.cpp

"$(INTDIR)\globals.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)


SOURCE=..\installer\install.cpp

"$(INTDIR)\install.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)


SOURCE=..\installerDLL\JVM.cpp

"$(INTDIR)\JVM.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\InstallerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)


SOURCE=..\installer\main.cpp

"$(INTDIR)\main.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)


SOURCE=..\installerDLL\search.cpp

"$(INTDIR)\search.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)


JAVAC=$(JDK_HOME)\bin\javac
JAVAH=$(JDK_HOME)\bin\javah -jni
SRC=..\installer

#
# AccessBridgeTester.h
#
"$(SRC)\..\src\AccessBridgeTester.h" : "$(OUTDIR)\AccessBridgeTester.class"
    cd "$(OUTDIR)"
	$(JAVAH) -o $(SRC)\..\src\AccessBridgeTester.h AccessBridgeTester 
    cd ..\..

#
# AccessBridgeTester.class
#
"$(OUTDIR)\AccessBridgeTester.class" : "$(SRC)\..\src\AccessBridgeTester.java"
	$(JAVAC) -d $(OUTDIR) $(SRC)\..\src\AccessBridgeTester.java

!ENDIF 

# resources

SOURCE=..\installerDLL\installerDLL.RC

!IF  "$(CFG)" == "installerDLL - Win32 Release"


"$(INTDIR)\installerDLL.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\installerDLL.res" /i "..\src" /d "NDEBUG" $(SOURCE)


!ELSEIF  "$(CFG)" == "installerDLL - Win32 Debug"


"$(INTDIR)\installerDLL.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\installerDLL.res" /i "..\src" /d "_DEBUG" $(SOURCE)


!ENDIF 


