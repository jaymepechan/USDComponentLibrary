# Microsoft Developer Studio Generated NMAKE File, Based on JAWTAccessBridgeDLL.dsp
!IF "$(CFG)" == ""
CFG=JAWTAccessBridgeDLL - Win32 Release
!MESSAGE No configuration specified. Defaulting to JAWTAccessBridgeDLL - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "JAWTAccessBridgeDLL - Win32 Release" && "$(CFG)" != "JAWTAccessBridgeDLL - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "JAWTAccessBridgeDLL.mak" CFG="JAWTAccessBridgeDLL - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "JAWTAccessBridgeDLL - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "JAWTAccessBridgeDLL - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
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

!IF  "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Release"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\JAWTAccessBridge.dll"


CLEAN :
	-@erase "$(INTDIR)\JAWTAccessBridge.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(OUTDIR)\JAWTAccessBridge.dll"
	-@erase "$(OUTDIR)\JAWTAccessBridge.exp"
	-@erase "$(OUTDIR)\JAWTAccessBridge.lib"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /Fp"$(INTDIR)\JAWTAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c /I "..\src" /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32"
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "NDEBUG" 
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JAWTAccessBridgeDLL.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\JAWTAccessBridge.pdb" /machine:I386 /def:"..\src\JAWTAccessBridge.DEF" /out:"$(OUTDIR)\JAWTAccessBridge.dll" /implib:"$(OUTDIR)\JAWTAccessBridge.lib" /libpath:"$(JDK_HOME)\lib"
DEF_FILE= \
	"..\src\JAWTAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\JAWTAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\JAWTAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Debug\
# End Custom Macros

ALL : "$(OUTDIR)\JAWTAccessBridge.dll" "$(OUTDIR)\JAWTAccessBridgeDLL.bsc"


CLEAN :
	-@erase "$(INTDIR)\JAWTAccessBridge.obj"
	-@erase "$(INTDIR)\JAWTAccessBridge.sbr"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(OUTDIR)\JAWTAccessBridge.dll"
	-@erase "$(OUTDIR)\JAWTAccessBridge.exp"
	-@erase "$(OUTDIR)\JAWTAccessBridge.lib"
	-@erase "$(OUTDIR)\JAWTAccessBridge.pdb"
	-@erase "$(OUTDIR)\JAWTAccessBridgeDLL.bsc"
	-@erase "$(INTDIR)\AccessBridgeStatusWindow.res"
	-@erase ".\tempFiles\JAWTAccessBridge.map"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MTd /W3 /Gm /GR /GX /ZI /Od /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32" /I "$(JDK_HOME)\include" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /FAs /Fa"$(INTDIR)\\" /FR"$(INTDIR)\\" /Fp"$(INTDIR)\JAWTAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "_DEBUG" 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\JAWTAccessBridgeDLL.bsc" 
BSC32_SBRS= \
	"$(INTDIR)\JAWTAccessBridge.sbr"

"$(OUTDIR)\JAWTAccessBridgeDLL.bsc" : "$(OUTDIR)" $(BSC32_SBRS)
    $(BSC32) @<<
  $(BSC32_FLAGS) $(BSC32_SBRS)
<<

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\JAWTAccessBridge.pdb" /map:"$(INTDIR)\JAWTAccessBridge.map" /debug /machine:I386 /def:"..\src\JAWTAccessBridge.DEF" /out:"$(OUTDIR)\JAWTAccessBridge.dll" /implib:"$(OUTDIR)\JAWTAccessBridge.lib" /libpath:"$(JDK_HOME)\lib" 
DEF_FILE= \
	"..\src\JAWTAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\JAWTAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\JAWTAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
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
!IF EXISTS("JAWTAccessBridgeDLL.dep")
!INCLUDE "JAWTAccessBridgeDLL.dep"
!ELSE 
!MESSAGE Warning: cannot find "JAWTAccessBridgeDLL.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Release" || "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Debug"
SOURCE=..\src\JAWTAccessBridge.cpp

!IF  "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Release"


"$(INTDIR)\JAWTAccessBridge.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "JAWTAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\JAWTAccessBridge.obj"	"$(INTDIR)\JAWTAccessBridge.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeStatusWindow.RC

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeStatusWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /i "..\src" /d "NDEBUG" $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeStatusWindow.res" : $(SOURCE) "$(INTDIR)"
	$(RSC) /l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /i "..\src" /d "_DEBUG" $(SOURCE)


!ENDIF 
!ENDIF 

