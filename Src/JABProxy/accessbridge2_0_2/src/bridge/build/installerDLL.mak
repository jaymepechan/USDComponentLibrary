# Microsoft Developer Studio Generated NMAKE File, Based on installerDLL.dsp
!IF "$(CFG)" == ""
CFG=installerDLL - Win32 Release
!MESSAGE No configuration specified. Defaulting to installerDLL - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "installerDLL - Win32 Release" && "$(CFG)" != "installerDLL - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "installerDLL.mak" CFG="installerDLL - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "installerDLL - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "installerDLL - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
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

!IF  "$(CFG)" == "installerDLL - Win32 Release"

OUTDIR=.\Release\installer
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release\installer
# End Custom Macros

ALL : "$(OUTDIR)\installerDLL.dll" "$(OUTDIR)\eval-license.rtf" "$(OUTDIR)\AccessBridgeTester.class"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\installerDLL.res"
	-@erase "$(INTDIR)\globals.obj"
	-@erase "$(INTDIR)\deinstall.obj"
	-@erase "$(INTDIR)\installerDLL.obj"
	-@erase "$(INTDIR)\JVM.obj"
	-@erase "$(INTDIR)\search.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\installerDLL.dll"
	-@erase "$(OUTDIR)\installerDLL.exp"
	-@erase "$(OUTDIR)\installerDLL.lib"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"


CPP_PROJ=/nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "INSTALLERDLL_EXPORTS" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /win32 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\installerDLL.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /incremental:no /pdb:"$(OUTDIR)\installerDLL.pdb" /machine:I386 /out:"$(OUTDIR)\installerDLL.dll" /def:"..\installerDLL\installerDLL.DEF" /implib:"$(OUTDIR)\installerDLL.lib" 
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\installerDLL.res" \
	"$(INTDIR)\globals.obj" \
	"$(INTDIR)\deinstall.obj" \
	"$(INTDIR)\installerDLL.obj" \
	"$(INTDIR)\JVM.obj" \
	"$(INTDIR)\search.obj" \
	"$(DEV_HOME)\lib\LIBCMT.LIB"

"$(OUTDIR)\installerDLL.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "installerDLL - Win32 Debug"

OUTDIR=.\release\installer
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\release\installer
# End Custom Macros

ALL : "$(OUTDIR)\installerDLL.dll"  "$(OUTDIR)\eval-license.rtf" "$(OUTDIR)\AccessBridgeTester.class"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\installerDLL.res"
	-@erase "$(INTDIR)\globals.obj"
	-@erase "$(INTDIR)\deinstall.obj"
	-@erase "$(INTDIR)\installerDLL.obj"
	-@erase "$(INTDIR)\JVM.obj"
	-@erase "$(INTDIR)\search.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\installerDLL.dll"
	-@erase "$(OUTDIR)\installerDLL.exp"
	-@erase "$(OUTDIR)\installerDLL.ilk"
	-@erase "$(OUTDIR)\installerDLL.lib"
	-@erase "$(OUTDIR)\installerDLL.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MTd /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /D "_MBCS" /D "_USRDLL" /D "INSTALLERDLL_EXPORTS" /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /GZ /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /win32 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\installerDLL.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /dll /incremental:yes /pdb:"$(OUTDIR)\installerDLL.pdb" /debug /machine:I386 /out:"$(OUTDIR)\installerDLL.dll" /def:"..\installerDLL\installerDLL.DEF" /implib:"$(OUTDIR)\installerDLL.lib" /pdbtype:sept 
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\installerDLL.res" \
	"$(INTDIR)\globals.obj" \
	"$(INTDIR)\deinstall.obj" \
	"$(INTDIR)\installerDLL.obj" \
	"$(INTDIR)\JVM.obj" \
	"$(INTDIR)\search.obj" \
	"$(DEV_HOME)\lib\LIBCMT.LIB"

"$(OUTDIR)\installerDLL.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
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
!IF EXISTS("installerDLL.dep")
!INCLUDE "installerDLL.dep"
!ELSE 
!MESSAGE Warning: cannot find "installerDLL.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "installerDLL - Win32 Release" || "$(CFG)" == "installerDLL - Win32 Debug"
SOURCE=..\src\AccessBridgeDebug.cpp

"$(INTDIR)\AccessBridgeDebug.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)

SOURCE=..\installerDLL\globals.cpp

"$(INTDIR)\globals.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)

SOURCE=..\installerDLL\deinstall.cpp

"$(INTDIR)\deinstall.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)

SOURCE=..\installerDLL\installerDLL.cpp

"$(INTDIR)\installerDLL.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)

SOURCE=..\installerDLL\JVM.cpp

"$(INTDIR)\JVM.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)

SOURCE=..\installerDLL\search.cpp

"$(INTDIR)\search.obj" : $(SOURCE) "$(INTDIR)" ..\installerDLL\installerDLL.h
	$(CPP) $(CPP_PROJ) $(SOURCE)

!ENDIF 

#
# license file
#
SOURCE=..\installerDLL\eval-license.rtf

"$(OUTDIR)\eval-license.rtf":
	COPY $(SOURCE) "$(OUTDIR)"

#
# AccessBridgeTester.h
#

JAVAC=$(JDK_HOME)\bin\javac
JAVAH=$(JDK_HOME)\bin\javah -jni
SRC=..\installer
SOURCE_DIR=..\src

"$(SOURCE_DIR)\AccessBridgeTester.h" : "$(OUTDIR)\AccessBridgeTester.class"
    cd "$(OUTDIR)"
        $(JAVAH) -o $(SOURCE_DIR)\AccessBridgeTester.h AccessBridgeTester
    cd ..\..

#
# AccessBridgeTester.class
#
"$(OUTDIR)\AccessBridgeTester.class" : "$(SOURCE_DIR)\AccessBridgeTester.java"
        $(JAVAC) -d $(OUTDIR) $(SOURCE_DIR)\AccessBridgeTester.java


# resources

SOURCE=..\installerDLL\installerDLL.RC

!IF  "$(CFG)" == "installerDLL - Win32 Release"


"$(INTDIR)\installerDLL.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\installerDLL.res" /i "..\src" /d "NDEBUG" $(SOURCE)


!ELSEIF  "$(CFG)" == "installerDLL - Win32 Debug"


"$(INTDIR)\installerDLL.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\installerDLL.res" /i "..\src" /d "_DEBUG" $(SOURCE)


!ENDIF 
