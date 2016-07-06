# Microsoft Developer Studio Project File - Name="WindowsAccessBridgeDLL" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Dynamic-Link Library" 0x0102

CFG=WindowsAccessBridgeDLL - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "WindowsAccessBridgeDLL.mak".
!MESSAGE 
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

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "WindowsA"
# PROP BASE Intermediate_Dir "WindowsA"
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
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /machine:I386 /out:".\Release\WindowsAccessBridge.dll"

!ELSEIF  "$(CFG)" == "WindowsAccessBridgeDLL - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Windows0"
# PROP BASE Intermediate_Dir "Windows0"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir ".\Debug\"
# PROP Intermediate_Dir ".\tempFiles\"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /MTd /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /I "c:\j2sdk1.4.2\include" /I "c:\j2sdk1.4.2\include\win32" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# SUBTRACT CPP /Fr
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib /nologo /subsystem:windows /dll /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib odbc32.lib odbccp32.lib jawt.lib /nologo /subsystem:windows /dll /incremental:no /map /debug /machine:I386 /out:".\Debug\WindowsAccessBridge.dll" /pdbtype:sept /libpath:"c:\j2sdk1.4.2\lib"

!ENDIF 

# Begin Target

# Name "WindowsAccessBridgeDLL - Win32 Release"
# Name "WindowsAccessBridgeDLL - Win32 Debug"
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

SOURCE=..\src\AccessBridgeEventHandler.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeEventHandler.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeJavaEntryPoints.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeJavaVMInstance.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeJavaVMInstance.h
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeMessageQueue.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeMessageQueue.h
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

SOURCE=..\src\AccessBridgeWindowsEntryPoints.cpp
# End Source File
# Begin Source File

SOURCE=..\src\AccessBridgeWindowsEntryPoints.h
# End Source File
# Begin Source File

SOURCE=..\src\WinAccessBridge.cpp
# End Source File
# Begin Source File

SOURCE=..\src\WinAccessBridge.DEF
# End Source File
# Begin Source File

SOURCE=..\src\WinAccessBridge.h
# End Source File
# End Target
# End Project
