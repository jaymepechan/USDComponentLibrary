# Microsoft Developer Studio Project File - Name="Installer" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Application" 0x0101

CFG=Installer - Win32 Debug
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "Installer.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "Installer.mak" CFG="Installer - Win32 Debug"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "Installer - Win32 Release" (based on "Win32 (x86) Application")
!MESSAGE "Installer - Win32 Debug" (based on "Win32 (x86) Application")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName ""
# PROP Scc_LocalPath ""
CPP=cl.exe
MTL=midl.exe
RSC=rc.exe

!IF  "$(CFG)" == "Installer - Win32 Release"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "Installe"
# PROP BASE Intermediate_Dir "Installe"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "Installe"
# PROP Intermediate_Dir "Installe"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /MT /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_WINDOWS" /YX /FD /c
# ADD BASE MTL /nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32
# ADD MTL /nologo /D "NDEBUG" /mktyplib203 /o "NUL" /win32
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib /nologo /subsystem:windows /machine:I386
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib libcimt.lib /nologo /subsystem:windows /machine:I386 /nodefaultlib /out:"Installe/Install.exe"

!ELSEIF  "$(CFG)" == "Installer - Win32 Debug"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "Install0"
# PROP BASE Intermediate_Dir "Install0"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "Install0"
# PROP Intermediate_Dir "Install0"
# PROP Ignore_Export_Lib 0
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /Zi /Od /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD CPP /nologo /MTd /W3 /Gm /GX /ZI /Od /I "c:\j2sdk1.4.2\include" /I "c:\j2sdk1.4.2\include\win32" /I "..\src" /D "WIN32" /D "_DEBUG" /D "_WINDOWS" /YX /FD /c
# ADD BASE MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD MTL /nologo /D "_DEBUG" /mktyplib203 /o "NUL" /win32
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LINK32=link.exe
# ADD BASE LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib /nologo /subsystem:windows /debug /machine:I386 /pdbtype:sept
# ADD LINK32 kernel32.lib user32.lib gdi32.lib winspool.lib comdlg32.lib advapi32.lib shell32.lib ole32.lib oleaut32.lib uuid.lib libcimt.lib /nologo /subsystem:windows /debug /machine:I386 /nodefaultlib /out:"Install0/Install.exe" /pdbtype:sept

!ENDIF 

# Begin Target

# Name "Installer - Win32 Release"
# Name "Installer - Win32 Debug"
# Begin Source File

SOURCE=..\src\AccessBridgeDebug.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeDrives.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeInstaller.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeInstaller.h
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeInstallerResource.h
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeInstallerResources.rc
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeJVM.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeRegistry.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\AccessBridgeTester.h
# End Source File
# Begin Source File

SOURCE=..\installer\JVM.cpp
# End Source File
# Begin Source File

SOURCE=..\installer\PathList.cpp
# End Source File
# Begin Source File

SOURCE="C:\Program Files\Microsoft Visual Studio\VC98\lib\LIBCMT.LIB"
# End Source File
# End Target
# End Project
