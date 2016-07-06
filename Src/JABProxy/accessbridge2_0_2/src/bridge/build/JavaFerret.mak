# Microsoft Developer Studio Generated NMAKE File, Based on JavaFerret.dsp
!IF "$(CFG)" == ""
CFG=JavaFerret - Win32 Release
!MESSAGE No configuration specified. Defaulting to JavaFerret - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "JavaFerret - Win32 Release" && "$(CFG)" != "JavaFerret - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "JavaFerret.mak" CFG="JavaFerret - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "JavaFerret - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "JavaFerret - Win32 Debug" (based on "Win32 (x86) Application")
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

!IF  "$(CFG)" == "JavaFerret - Win32 Release"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\JavaFerret.exe"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeCalls.obj"
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessInfo.obj"
	-@erase "$(INTDIR)\ferretWindow.res"
	-@erase "$(INTDIR)\JavaFerret.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\JavaFerret.exe"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /ML /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /Fp"$(INTDIR)\JavaFerret.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c /I "..\src" /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32"
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\ferretWindow.res" /d "NDEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JavaFerret.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /incremental:no /pdb:"$(OUTDIR)\JavaFerret.pdb" /machine:I386 /out:"$(OUTDIR)\JavaFerret.exe" /libpath:"$(JDK_HOME)\lib"
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeCalls.obj" \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessInfo.obj" \
	"$(INTDIR)\JavaFerret.obj" \
	"$(INTDIR)\ferretWindow.res"

"$(OUTDIR)\JavaFerret.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Debug
# End Custom Macros

ALL : "$(OUTDIR)\JavaFerret.exe" "$(OUTDIR)\JavaFerret.bsc"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeCalls.obj"
	-@erase "$(INTDIR)\AccessBridgeCalls.sbr"
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessBridgeDebug.sbr"
	-@erase "$(INTDIR)\AccessInfo.obj"
	-@erase "$(INTDIR)\AccessInfo.sbr"
	-@erase "$(INTDIR)\ferretWindow.res"
	-@erase "$(INTDIR)\JavaFerret.obj"
	-@erase "$(INTDIR)\JavaFerret.sbr"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\JavaFerret.bsc"
	-@erase "$(OUTDIR)\JavaFerret.exe"
	-@erase "$(OUTDIR)\JavaFerret.ilk"
	-@erase "$(OUTDIR)\JavaFerret.pdb"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MLd /W3 /Gm /GX /ZI /Od /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /FR"$(INTDIR)\\" /Fp"$(INTDIR)\JavaFerret.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\ferretWindow.res" /d "_DEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JavaFerret.bsc" 
BSC32_SBRS= \

"$(OUTDIR)\JavaFerret.bsc" : "$(OUTDIR)" $(BSC32_SBRS)
    $(BSC32) @<<
  $(BSC32_FLAGS) $(BSC32_SBRS)
<<

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /incremental:yes /pdb:"$(OUTDIR)\JavaFerret.pdb" /debug /machine:I386 /out:"$(OUTDIR)\JavaFerret.exe" /pdbtype:sept /libpath:"$(JDK_HOME)\lib" 
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeCalls.obj" \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessInfo.obj" \
	"$(INTDIR)\JavaFerret.obj" \
	"$(INTDIR)\ferretWindow.res"

"$(OUTDIR)\JavaFerret.exe" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
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
!IF EXISTS("JavaFerret.dep")
!INCLUDE "JavaFerret.dep"
!ELSE 
!MESSAGE Warning: cannot find "JavaFerret.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "JavaFerret - Win32 Release" || "$(CFG)" == "JavaFerret - Win32 Debug"
SOURCE=..\src\AccessBridgeCalls.c

!IF  "$(CFG)" == "JavaFerret - Win32 Release"


"$(INTDIR)\AccessBridgeCalls.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"


"$(INTDIR)\AccessBridgeCalls.obj"	"$(INTDIR)\AccessBridgeCalls.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeDebug.cpp

!IF  "$(CFG)" == "JavaFerret - Win32 Release"


"$(INTDIR)\AccessBridgeDebug.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"


"$(INTDIR)\AccessBridgeDebug.obj"	"$(INTDIR)\AccessBridgeDebug.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessInfo.cpp

!IF  "$(CFG)" == "JavaFerret - Win32 Release"


"$(INTDIR)\AccessInfo.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"


"$(INTDIR)\AccessInfo.obj"	"$(INTDIR)\AccessInfo.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\ferretWindow.RC

!IF  "$(CFG)" == "JavaFerret - Win32 Release"


"$(INTDIR)\ferretWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\ferretWindow.res" /i "..\src" /d "NDEBUG" $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"


"$(INTDIR)\ferretWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\ferretWindow.res" /i "..\src" /d "_DEBUG" $(SOURCE)


!ENDIF 

SOURCE=..\src\JavaFerret.cpp

!IF  "$(CFG)" == "JavaFerret - Win32 Release"


"$(INTDIR)\JavaFerret.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaFerret - Win32 Debug"


"$(INTDIR)\JavaFerret.obj"	"$(INTDIR)\JavaFerret.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 


!ENDIF 

