REM
REM Copyright 2005 Sun Microsystems, Inc. All rights reserved.
REM SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
REM

rem @(#)buildall.bat	1.22 06/06/22
set BUILD_VERSION=Java_Access_Bridge_2.0.1
rem Batch file to build %BUILD_VERSION%

echo
echo Start making %BUILD_VERSION%

set BASE=c:\AccessBridge-2.0.1

rem Set environment variables for building jaccess-1.2.jar
rem on JDK 1.2.2
rem
set JACCESS_HOME=%BASE%\1.2.x\build
SEt JDK_HOME=c:/jdk1.2.2
echo Building jaccess for JDK 1.2.1
cd 1.2.x\build
gnumake

rem Set environment variables for building jaccess-1.3.jar
rem on JDK 1.3.1
rem
echo Building jaccess for JDK 1.3.1
cd ..\..\1.3.x\build
set JDK_HOME=c:/jdk1.3.1
set JACCESS_HOME=%BASE%\1.3.x\build
gnumake

rem Set environment variables for building jaccess-1.4.jar
rem on JDK 1.4.2
rem
echo Building jaccess for JDK 1.4
cd ..\..\1.4.x\build
set JDK_HOME=c:/j2sdk1.4.2
set JACCESS_HOME=%BASE%\1.4.x\build
gnumake

rem Set environment variables for building the Java Access
rem Bridge on JDK 1.4.2
rem
echo "Building Access Bridge
rem
rem *** NOTE BACKSLASH INSTEAD OF FORWARD SLASH
set JACCESS_HOME=c:\1.4.x\build
rem
set JDK_HOME=c:\j2sdk1.4.2
rem
rem *** NOTE DEV_HOME IS THE LOCATION OF EITHER VC OR VC98
rem *** DEPENDING IF YOU ARE BUILDING ON VISUAL STUDIO
rem *** 5.0 (VC) or 6.0 (VC98)
rem
set DEV_HOME=c:\Program Files\Microsoft Visual Studio\VC98
rem
set ZIP=c:\utils\zip.exe
set PKZIP=c:\utils\pkzipc.exe
rem
cd ..\..\bridge\build
nmake
nmake release

cd ..\..

rem run InstallShield batch build
rem
set ISHIELD_CMD=c:\InstallShield\issabld
set ISHIELD_OPTS=-c COMP -p "JavaAccessBridge.ism"

%ISHIELD_CMD% %ISHIELD_OPTS%
mv "%BASE%\JavaAccessBridge\Product Configuration 1\Release 1\DiskImages\DISK1\setup.exe" "%BASE%\JavaAccessBridge\Product Configuration 1\Release 1\DiskImages\DISK1\%BUILD_VERSION%.exe"

echo Finished making %BUILD_VERSION%
