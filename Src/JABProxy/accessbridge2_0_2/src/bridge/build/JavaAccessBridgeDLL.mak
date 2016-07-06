# Microsoft Developer Studio Generated NMAKE File, Based on JavaAccessBridgeDLL.dsp
!IF "$(CFG)" == ""
CFG=JavaAccessBridgeDLL - Win32 Release
!MESSAGE No configuration specified. Defaulting to JavaAccessBridgeDLL - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "JavaAccessBridgeDLL - Win32 Release" && "$(CFG)" != "JavaAccessBridgeDLL - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "JavaAccessBridgeDLL.mak" CFG="JavaAccessBridgeDLL - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "JavaAccessBridgeDLL - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "JavaAccessBridgeDLL - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
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

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\JavaAccessBridge.dll"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeATInstance.obj"
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessBridgeJavaEntryPoints.obj"
	-@erase "$(INTDIR)\AccessBridgeMessages.obj"
	-@erase "$(INTDIR)\AccessBridgeStatusWindow.res"
	-@erase "$(INTDIR)\JavaAccessBridge.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\JavaAccessBridge.dll"
	-@erase "$(OUTDIR)\JavaAccessBridge.exp"
	-@erase "$(OUTDIR)\JavaAccessBridge.lib"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /Fp"$(INTDIR)\JavaAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c /I "..\src" /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32"
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "NDEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JavaAccessBridgeDLL.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\JavaAccessBridge.pdb" /machine:I386 /def:"..\src\JavaAccessBridge.DEF" /out:"$(OUTDIR)\JavaAccessBridge.dll" /implib:"$(OUTDIR)\JavaAccessBridge.lib" /libpath:"$(JDK_HOME)\lib"
DEF_FILE= \
	"..\src\JavaAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeATInstance.obj" \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessBridgeJavaEntryPoints.obj" \
	"$(INTDIR)\AccessBridgeMessages.obj" \
	"$(INTDIR)\JavaAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\JavaAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Debug\
# End Custom Macros

ALL : "$(OUTDIR)\JavaAccessBridge.dll" "$(OUTDIR)\JavaAccessBridgeDLL.bsc"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeATInstance.obj"
	-@erase "$(INTDIR)\AccessBridgeATInstance.sbr"
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessBridgeDebug.sbr"
	-@erase "$(INTDIR)\AccessBridgeJavaEntryPoints.obj"
	-@erase "$(INTDIR)\AccessBridgeJavaEntryPoints.sbr"
	-@erase "$(INTDIR)\AccessBridgeMessages.obj"
	-@erase "$(INTDIR)\AccessBridgeMessages.sbr"
	-@erase "$(INTDIR)\AccessBridgeStatusWindow.res"
	-@erase "$(INTDIR)\JavaAccessBridge.obj"
	-@erase "$(INTDIR)\JavaAccessBridge.sbr"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\JavaAccessBridge.dll"
	-@erase "$(OUTDIR)\JavaAccessBridge.exp"
	-@erase "$(OUTDIR)\JavaAccessBridge.lib"
	-@erase "$(OUTDIR)\JavaAccessBridge.pdb"
	-@erase "$(OUTDIR)\JavaAccessBridgeDLL.bsc"
	-@erase ".\tempFiles\JavaAccessBridge.map"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MTd /W3 /Gm /GR /GX /ZI /Od /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32" /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /FAs /Fa"$(INTDIR)\\" /FR"$(INTDIR)\\" /Fp"$(INTDIR)\JavaAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "_DEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JavaAccessBridgeDLL.bsc" 
BSC32_SBRS= \
	"$(INTDIR)\AccessBridgeATInstance.sbr" \
	"$(INTDIR)\AccessBridgeDebug.sbr" \
	"$(INTDIR)\AccessBridgeJavaEntryPoints.sbr" \
	"$(INTDIR)\AccessBridgeMessages.sbr" \
	"$(INTDIR)\JavaAccessBridge.sbr"

"$(OUTDIR)\JavaAccessBridgeDLL.bsc" : "$(OUTDIR)" $(BSC32_SBRS)
    $(BSC32) @<<
  $(BSC32_FLAGS) $(BSC32_SBRS)
<<

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\JavaAccessBridge.pdb" /map:"$(INTDIR)\JavaAccessBridge.map" /debug /machine:I386 /def:"..\src\JavaAccessBridge.DEF" /out:"$(OUTDIR)\JavaAccessBridge.dll" /implib:"$(OUTDIR)\JavaAccessBridge.lib" /libpath:"$(JDK_HOME)\lib" 
DEF_FILE= \
	"..\src\JavaAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeATInstance.obj" \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessBridgeJavaEntryPoints.obj" \
	"$(INTDIR)\AccessBridgeMessages.obj" \
	"$(INTDIR)\JavaAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\JavaAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
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
!IF EXISTS("JavaAccessBridgeDLL.dep")
!INCLUDE "JavaAccessBridgeDLL.dep"
!ELSE 
!MESSAGE Warning: cannot find "JavaAccessBridgeDLL.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release" || "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"
SOURCE=..\src\AccessBridgeATInstance.cpp

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeATInstance.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeATInstance.obj"	"$(INTDIR)\AccessBridgeATInstance.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeDebug.cpp

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeDebug.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeDebug.obj"	"$(INTDIR)\AccessBridgeDebug.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeJavaEntryPoints.cpp

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeJavaEntryPoints.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeJavaEntryPoints.obj"	"$(INTDIR)\AccessBridgeJavaEntryPoints.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeMessages.cpp

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeMessages.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeMessages.obj"	"$(INTDIR)\AccessBridgeMessages.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeStatusWindow.RC

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeStatusWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /i "..\src" /d "NDEBUG" $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeStatusWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /i "..\src" /d "_DEBUG" $(SOURCE)


!ENDIF 

SOURCE=..\src\JavaAccessBridge.cpp

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"


"$(INTDIR)\JavaAccessBridge.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\JavaAccessBridge.obj"	"$(INTDIR)\JavaAccessBridge.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 


!ENDIF 

