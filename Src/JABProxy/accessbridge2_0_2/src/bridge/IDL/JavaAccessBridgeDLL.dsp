# Microsoft Developer Studio Project File - Name="JavaAccessBridgeDLL" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=JavaAccessBridgeDLL - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "JavaAccessBridgeDLL.mak".
!MESSAGE 
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

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Release"
# PROP BASE Intermediate_Dir "Release"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir ".\Release"
# PROP Intermediate_Dir ".\tempFiles"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /machine:I386 /out:".\Release\JavaAccessBridge.dll"

!ELSEIF  "$(CFG)" == "JavaAccessBridgeDLL - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Debug"
# PROP BASE Intermediate_Dir "Debug"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir ".\Debug\"
# PROP Intermediate_Dir ".\tempFiles\"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MTd /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /MTd /W3 /Gm /GR /GX /ZI /Od /I "c:\j2sdk1.4.2\include" /I "c:\j2sdk1.4.2\include\win32" /I "c:\j2sdk1.4.2\include" /I "c:\j2sdk1.4.2\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /FAs /FR /YX /FD /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /map /debug /machine:I386 /out:".\Debug\JavaAccessBridge.dll" /libpath:"c:\j2sdk1.4.2\lib"

!ENDIF 

# Begin Target

# Name "JavaAccessBridgeDLL - Win32 Release"
# Name "JavaAccessBridgeDLL - Win32 Debug"
# Begin Source File

SOURCE=..\src\AccessBridge.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeATInstance.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeATInstance.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeCallbacks.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeDebug.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeDebug.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeJavaEntryPoints.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeJavaEntryPoints.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeMessages.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeMessages.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgePackages.h
# End Source File
# Begin Source File

SOURCE=..\src\accessBridgeResource.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeStatusWindow.RC
# End Source File
# Begin Source File

SOURCE=..\src\bitmap1.bmp
# End Source File
# Begin Source File

SOURCE=..\src\JavaAccessBridge.cpp
# End Source File
# Begin Source File

SOURCE=..\src\JavaAccessBridge.DEF
# End Source File
# Begin Source File

SOURCE=..\src\JavaAccessBridge.h
# End Source File
# End Target
# End Project
