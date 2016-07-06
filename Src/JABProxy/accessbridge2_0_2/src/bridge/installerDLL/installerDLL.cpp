/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)installerDLL.cpp	1.4 05/11/11
 */

#include "installerDLL.h"

BOOL APIENTRY DllMain( HANDLE hModule, 
                       DWORD  ul_reason_for_call, 
                       LPVOID lpReserved
					 )
{
    PrintDebugString("BOOL APIENTRY DllMain");
    
    switch (ul_reason_for_call)
	{
        case DLL_PROCESS_ATTACH:
            PrintDebugString("DLL_PROCESS_ATTACH");
            theInstaller = new Installer((HINSTANCE)hModule);
            break;
            
        case DLL_THREAD_ATTACH:
            PrintDebugString("DLL_THREAD_ATTACH");
            break;
            
        case DLL_THREAD_DETACH:
            PrintDebugString("DLL_THREAD_DETACH");
            break;
            
        case DLL_PROCESS_DETACH:
            PrintDebugString("DLL_PROCESS_DETACH");
            break;
    }
    return TRUE;
}

const char appName[] = "c:\\Program Files\\Java Access Bridge\\installer.exe";
LPCTSTR lpAppName = appName;

// exported function to run installer
UINT __stdcall runInstaller(MSIHANDLE handle) {
    
    PrintDebugString("UINT __stdcall runInstaller(MSIHANDLE)");
    
    // for debugging
    struct _finddata_t nextFile;
    long searchHandle = _findfirst(lpAppName, &nextFile);
    if (searchHandle != -1L) {
        PrintDebugString("  found %s", lpAppName);
    } else {
        printError("  _findfirst");
    }

    // launch install.exe and return exit code.
    return runInstallerApp();
}

// exported function to run deinstaller
UINT __stdcall runDeinstaller(MSIHANDLE handle) {

    PrintDebugString("UINT __stdcall runDeinstaller()");
    return theInstaller->deinstallAllDrives();
}

/*
 * returns the exit code from running installer.exe
 */
DWORD runInstallerApp() {

    PrintDebugString("runInstallerApp: %s", lpAppName);

    STARTUPINFO startupInfo;
    PROCESS_INFORMATION processInfo;

    // prepare the startup structure
    ::ZeroMemory(&startupInfo, sizeof(STARTUPINFO));
    startupInfo.cb = sizeof(STARTUPINFO);

    // execute installer.exe
    BOOL bOK = ::CreateProcess("c:\\Program Files\\Java Access Bridge\\installer.exe", 
                               NULL, NULL, NULL, FALSE, NULL, NULL, NULL, 
                               &startupInfo, &processInfo);
    if (! bOK) {
        printError("CreateProcess");
        return -1;
    }
    PrintDebugString("  CreateProcess succeeded");
    
    // wait until child process exits.
    PrintDebugString("  waiting for process to terminate...");
    DWORD retval = WaitForSingleObject( processInfo.hProcess, INFINITE );
    switch(retval) {
    case WAIT_FAILED:
        printError("WaitForSingleObject");
        break;
    case WAIT_ABANDONED:
        PrintDebugString("  WAIT_ABONDONED");
        break;
    case WAIT_OBJECT_0:
        PrintDebugString("  WAIT_OBJECT_0");
        break;
    case WAIT_TIMEOUT:
        PrintDebugString("  WAIT_TIMEOUT");
        break;
    }
    
    // get the exit code
    DWORD dwExitCode = -1;
    retval = GetExitCodeProcess(processInfo.hProcess, &dwExitCode);
    ::CloseHandle(processInfo.hProcess);
    if (retval == 0) {
        printError("GetExitCodeProcess");
        return ERROR_INSTALL_FAILURE;
    }

    PrintDebugString("  returning %d", ERROR_SUCCESS);
    return ERROR_SUCCESS;
}
