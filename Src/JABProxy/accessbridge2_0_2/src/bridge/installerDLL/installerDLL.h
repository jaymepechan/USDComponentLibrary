/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)installerDLL.h	1.4 05/11/11
 */

#ifndef _MT
#define _MT
#endif

#ifdef _DEBUG
#define DEBUGGING_ON
#endif

// link to MSI library
#pragma comment(lib, "msi.lib")

// include standard Windows and MSI headers
#include < windows.h >
#include < msi.h >
#include < msiquery.h > 

#include <shlobj.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <io.h>
#include <direct.h>
#include <process.h>
#include <fstream.h>

#include "resource.h"
#include "AccessBridgeTester.h"
#include "..\src\AccessBridgeDebug.h"

#define installerDir "c:\\Program Files\\Java Access Bridge"

/* 
 * the installer window proc 
 */
LRESULT CALLBACK mainWindowProc(HWND, UINT, UINT, LONG);

/*
 * the cancel dialog window proc
 */
int CALLBACK cancelDialogProc (HWND hWnd, UINT message, UINT wParam, LONG lParam);

/*
 * the installer thread
 */
VOID installerThread(PVOID pvoid);

/*
 * export install bridge files method to be called by InstallShield custom action
 */
UINT __stdcall runInstaller(MSIHANDLE);

/*
 * export method to start main installation routine
 */
UINT __stdcall mainInstallRoutine();

/*
 * export deinstall bridge files method to be called by InstallShield custom action
 */
UINT __stdcall runDeinstaller(MSIHANDLE);


// Used for messages between the dir searching thread
// and the Dialog UI.
// In the SendMessage call, the third and fourth parameters
// (WARAM and LPARAM) are null.
#define INSTALLER_START_INSTALLATION (WM_USER+0x0150)  

/*
 * List of all the JVMs on the Windows system
 */
class JVM;
extern JVM *jvmListHead;

class JVM {
public:
    JVM *next;

    char *javahome;     // full path to top of JVM installation
    char *binpath;      // full path to bin directory
    char *libpath;      // full path to jre\lib directory
    char *jarpath;      // full path to jre\lib\ext directory
    BOOL has1_3apis;    // does it support AccessibleRelation, etc. of 1.3?
	
public:

    JVM(char *javahome, int result);
    JVM(char *javahome);

    void append(char *javahome, BOOL result);
    void append(char *javahome);

    BOOL contains(char *dir);
    BOOL isLooping(char *dir);

    void dump();
};

/*
 * the installer class
 */
class Installer;

/* utility functions */
char *strdup(const char *str);
char *strdup(const char *str1, const char *str2);
char *getJvmDir(char *javahome);
char *deactivateAccessibilityDotProperties(char *dir);
void reactivateAccessibilityDotProperties(char *libdir);
DWORD runInstallerApp();

/* global data structures */
extern HINSTANCE theInstance;
extern Installer *theInstaller;
extern JVM *jvmListHead;
extern HWND theDialogWindow;
extern HWND theInstallerWindow;

/*
 * the cancel dialog window proc
 */
int CALLBACK cancelDialogProc (HWND hWnd, UINT message, UINT wParam, LONG lParam);

/**
 * Installer class.
 */
class Installer {

public:

    Installer(HINSTANCE hInstance) { theInstance = hInstance; }
    ~Installer() {};

    HWND createMainWindow(int showCommand);
    int mainInstallRoutine();
    void searchAllDrives();
    // void searchDrive();
    void searchDrive(const char *path);
    int isInstallable(char *dir);
    char *normalize(char *path);
    int doInstall();
    void showDone(BOOL successful);

    UINT deinstallAllDrives();
    void deinstallDrive(const char *path);
    void deleteDLLs();
    void deleteTheFile(char *file);
    void deleteBakFiles();    
    void deleteJvmFiles();    
};  

