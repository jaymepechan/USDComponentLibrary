# Microsoft Developer Studio Generated NMAKE File, Based on WindowsAccessBridgeDLL.dsp
!IF "$(CFG)" == ""
CFG=WindowsAccessBridgeDLL - Win32 Release
!MESSAGE No configuration specified. Defaulting to WindowsAccessBridgeDLL - Win32 Release.
!ENDIF 

!IF "$(CFG)" != "WindowsAccessBridgeDLL - Win32 Release" && "$(CFG)" != "WindowsAccessBridgeDLL - Win32 Debug"
!MESSAGE Invalid configuration "$(CFG)" specified.
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "WindowsAccessBridgeDLL.mak" CFG="WindowsAccessBridgeDLL - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "WindowsAccessBridgeDLL - Win32 Release" (based on "Win32 (x86) Dynamic-Link Library")
!MESSAGE "WindowsAccessBridgeDLL - Win32 Debug" (based on "Win32 (x86) Dynamic-Link Library")
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

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"

OUTDIR=.\Release
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Release
# End Custom Macros

ALL : "$(OUTDIR)\WindowsAccessBridge.dll"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessBridgeEventHandler.obj"
	-@erase "$(INTDIR)\AccessBridgeJavaVMInstance.obj"
	-@erase "$(INTDIR)\AccessBridgeMessageQueue.obj"
	-@erase "$(INTDIR)\AccessBridgeMessages.obj"
	-@erase "$(INTDIR)\AccessBridgeStatusWindow.res"
	-@erase "$(INTDIR)\AccessBridgeWindowsEntryPoints.obj"
	-@erase "$(INTDIR)\AccessBridgeInfo.obj"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\WinAccessBridge.obj"
	-@erase "$(OUTDIR)\WindowsAccessBridge.dll"
	-@erase "$(OUTDIR)\WindowsAccessBridge.exp"
	-@erase "$(OUTDIR)\WindowsAccessBridge.lib"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /Fp"$(INTDIR)\WindowsAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c /I "..\src" /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32"
MTL_PROJ=/nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "NDEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\WindowsAccessBridgeDLL.bsc" 
BSC32_SBRS= \

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\WindowsAccessBridge.pdb" /machine:I386 /def:"..\src\WinAccessBridge.DEF" /out:"$(OUTDIR)\WindowsAccessBridge.dll" /implib:"$(OUTDIR)\WindowsAccessBridge.lib" /libpath:"$(JDK_HOME)\lib"
DEF_FILE= \
	"..\src\WinAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessBridgeEventHandler.obj" \
	"$(INTDIR)\AccessBridgeJavaVMInstance.obj" \
	"$(INTDIR)\AccessBridgeMessageQueue.obj" \
	"$(INTDIR)\AccessBridgeMessages.obj" \
	"$(INTDIR)\AccessBridgeWindowsEntryPoints.obj" \
	"$(INTDIR)\WinAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\WindowsAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
    $(LINK32) @<<
  $(LINK32_FLAGS) $(LINK32_OBJS)
<<

!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"

OUTDIR=.\Debug
INTDIR=.\tempFiles
# Begin Custom Macros
OutDir=.\Debug\
# End Custom Macros

ALL : "$(OUTDIR)\WindowsAccessBridge.dll" "$(OUTDIR)\WindowsAccessBridgeDLL.bsc"


CLEAN :
	-@erase "$(INTDIR)\AccessBridgeDebug.obj"
	-@erase "$(INTDIR)\AccessBridgeDebug.sbr"
	-@erase "$(INTDIR)\AccessBridgeEventHandler.obj"
	-@erase "$(INTDIR)\AccessBridgeEventHandler.sbr"
	-@erase "$(INTDIR)\AccessBridgeJavaVMInstance.obj"
	-@erase "$(INTDIR)\AccessBridgeJavaVMInstance.sbr"
	-@erase "$(INTDIR)\AccessBridgeMessageQueue.obj"
	-@erase "$(INTDIR)\AccessBridgeMessageQueue.sbr"
	-@erase "$(INTDIR)\AccessBridgeMessages.obj"
	-@erase "$(INTDIR)\AccessBridgeMessages.sbr"
	-@erase "$(INTDIR)\AccessBridgeStatusWindow.res"
	-@erase "$(INTDIR)\AccessBridgeWindowsEntryPoints.obj"
	-@erase "$(INTDIR)\AccessBridgeWindowsEntryPoints.sbr"
	-@erase "$(INTDIR)\vc60.idb"
	-@erase "$(INTDIR)\vc60.pdb"
	-@erase "$(INTDIR)\WinAccessBridge.obj"
	-@erase "$(INTDIR)\WinAccessBridge.sbr"
	-@erase "$(OUTDIR)\WindowsAccessBridge.dll"
	-@erase "$(OUTDIR)\WindowsAccessBridge.exp"
	-@erase "$(OUTDIR)\WindowsAccessBridge.lib"
	-@erase "$(OUTDIR)\WindowsAccessBridge.pdb"
	-@erase "$(OUTDIR)\WindowsAccessBridgeDLL.bsc"
	-@erase ".\tempFiles\WindowsAccessBridge.map"

"$(OUTDIR)" :
    if not exist "$(OUTDIR)/$(NULL)" mkdir "$(OUTDIR)"

"$(INTDIR)" :
    if not exist "$(INTDIR)/$(NULL)" mkdir "$(INTDIR)"

CPP_PROJ=/nologo /MTd /W3 /Gm /GX /ZI /Od /I "$(JDK_HOME)\include" /I "$(JDK_HOME)\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /Fr"$(INTDIR)\\" /Fp"$(INTDIR)\WindowsAccessBridgeDLL.pch" /YX /Fo"$(INTDIR)\\" /Fd"$(INTDIR)\\" /FD /c 
MTL_PROJ=/nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32 
RSC_PROJ=/l 0x409 /fo"$(INTDIR)\AccessBridgeStatusWindow.res" /d "_DEBUG" 
BSC32=bscmake.exe
BSC32_FLAGS=/nologo /o"$(OUTDIR)\WindowsAccessBridgeDLL.bsc" 
BSC32_SBRS= \
	"$(INTDIR)\AccessBridgeDebug.sbr" \
	"$(INTDIR)\AccessBridgeEventHandler.sbr" \
	"$(INTDIR)\AccessBridgeJavaVMInstance.sbr" \
	"$(INTDIR)\AccessBridgeMessageQueue.sbr" \
	"$(INTDIR)\AccessBridgeMessages.sbr" \
	"$(INTDIR)\AccessBridgeWindowsEntryPoints.sbr" \
	"$(INTDIR)\WinAccessBridge.sbr"

"$(OUTDIR)\WindowsAccessBridgeDLL.bsc" : "$(OUTDIR)" $(BSC32_SBRS)
    $(BSC32) @<<
  $(BSC32_FLAGS) $(BSC32_SBRS)
<<

LINK32=link.exe
LINK32_FLAGS=kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /pdb:"$(OUTDIR)\WindowsAccessBridge.pdb" /map:"$(INTDIR)\WindowsAccessBridge.map" /debug /machine:I386 /def:"..\src\WinAccessBridge.DEF" /out:"$(OUTDIR)\WindowsAccessBridge.dll" /implib:"$(OUTDIR)\WindowsAccessBridge.lib" /pdbtype:sept /libpath:"$(JDK_HOME)\lib" 
DEF_FILE= \
	"..\src\WinAccessBridge.DEF"
LINK32_OBJS= \
	"$(INTDIR)\AccessBridgeDebug.obj" \
	"$(INTDIR)\AccessBridgeEventHandler.obj" \
	"$(INTDIR)\AccessBridgeJavaVMInstance.obj" \
	"$(INTDIR)\AccessBridgeMessageQueue.obj" \
	"$(INTDIR)\AccessBridgeMessages.obj" \
	"$(INTDIR)\AccessBridgeWindowsEntryPoints.obj" \
	"$(INTDIR)\WinAccessBridge.obj" \
	"$(INTDIR)\AccessBridgeStatusWindow.res"

"$(OUTDIR)\WindowsAccessBridge.dll" : "$(OUTDIR)" $(DEF_FILE) $(LINK32_OBJS)
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
!IF EXISTS("WindowsAccessBridgeDLL.dep")
!INCLUDE "WindowsAccessBridgeDLL.dep"
!ELSE 
!MESSAGE Warning: cannot find "WindowsAccessBridgeDLL.dep"
!ENDIF 
!ENDIF 


!IF "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release" || "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"
SOURCE=..\src\AccessBridgeDebug.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeDebug.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeDebug.obj"	"$(INTDIR)\AccessBridgeDebug.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeEventHandler.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeEventHandler.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeEventHandler.obj"	"$(INTDIR)\AccessBridgeEventHandler.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeJavaVMInstance.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeJavaVMInstance.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeJavaVMInstance.obj"	"$(INTDIR)\AccessBridgeJavaVMInstance.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeMessageQueue.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeMessageQueue.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeMessageQueue.obj"	"$(INTDIR)\AccessBridgeMessageQueue.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\AccessBridgeMessages.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeMessages.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeMessages.obj"	"$(INTDIR)\AccessBridgeMessages.sbr" : $(SOURCE) "$(INTDIR)"
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

SOURCE=..\src\AccessBridgeWindowsEntryPoints.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\AccessBridgeWindowsEntryPoints.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\AccessBridgeWindowsEntryPoints.obj"	"$(INTDIR)\AccessBridgeWindowsEntryPoints.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 

SOURCE=..\src\WinAccessBridge.cpp

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"


"$(INTDIR)\WinAccessBridge.obj" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"


"$(INTDIR)\WinAccessBridge.obj"	"$(INTDIR)\WinAccessBridge.sbr" : $(SOURCE) "$(INTDIR)"
	$(CPP) $(CPP_PROJ) $(SOURCE)


!ENDIF 


!ENDIF 

