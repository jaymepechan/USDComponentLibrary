/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)install.cpp	1.4 05/11/11
 */

#include "../installerDLL/installerDLL.h"

/**
 * installer main routine
 */
int
Installer::mainInstallRoutine() {

    PrintDebugString("Installer::mainInstallRoutine");
    PrintDebugString(  "installerDir = %s", installerDir);

    // launch a non-modal dialog so the user can cancel the installation
    theDialogWindow = (struct HWND__ *)CreateDialog (theInstance,
                                                     MAKEINTRESOURCE(SEARCH_DIALOG),
                                                     theInstallerWindow, 
                                                     cancelDialogProc);
    
    PrintDebugString("CreateDialog = %d", theDialogWindow);

    // launch a thread to scan for Java installations and install the bridge
    _beginthread(installerThread, 0, NULL);
    return 0;
}

/**
 * dialog proc for cancel dialog
 */
int CALLBACK cancelDialogProc (HWND hWnd, UINT message, UINT wParam, LONG lParam) {

    int command;
    short width, height;
    HWND dlgItem;

    switch (message) {

    case WM_CLOSE:
        PrintDebugString("cancelDialogProc: WM_CLOSE");
        EndDialog(hWnd, TRUE);
        return 1;
	
    case WM_SIZE:
        width = LOWORD(lParam);
        height = HIWORD(lParam);
        dlgItem = GetDlgItem(theDialogWindow, CANCEL_BUTTON); 
        SetWindowPos(dlgItem, NULL, 0, 0, width, height, 0); 
        return 0;               // let windows finish handling this
	
    case WM_COMMAND:
        command = LOWORD(wParam);
        if (command == CANCEL_BUTTON) {
            PrintDebugString("cancelDialogProc: CANCEL_BUTTON");
            PrintDebugString("calling exit(-1)");
            exit(-1);            // the installation did not succeed
        }
    }
    return DefWindowProc(hWnd, message, wParam, lParam);
}

/*
 * the installer thread
 */
VOID
installerThread(PVOID pvoid) {
    PrintDebugString("starting installerThread");

    PrintDebugString("calling searchAllDrives");
    theInstaller->searchAllDrives();

    PrintDebugString("calling doInstall");
    theInstaller->doInstall();

    // terminate installer
    PrintDebugString("done: calling exit(0)"); // success!
    exit(0);
}


void
copyTheFile(char *src, char *dst) {

    PrintDebugString("  src: %s", src);
    PrintDebugString("  dst: %s", dst);
    int result = CopyFile(src, dst, FALSE);
    if (result == 0) {
        printError("CopyFile failed");
        ;
    }
}

/**
 * installs the DLLs, JAR and properties files
 */
int
Installer::doInstall() {

    PrintDebugString("Installer::doInstall");
    jvmListHead->dump();

    char system32[_MAX_PATH];
    char src[_MAX_DIR];         // source directory
    char dest[_MAX_DIR];        // destination directory

    // copy JavaAccessBridge.dll to Windows system directory
    GetSystemDirectory(system32, _MAX_PATH);
    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\JavaAccessBridge.dll", _MAX_DIR);
    strncpy(dest, system32, _MAX_DIR);
    strncat(dest, "\\JavaAccessBridge.dll", _MAX_DIR);
    PrintDebugString("  CopyFile: %s -> %s", src, dest);
    if (CopyFile(src, dest, FALSE) == 0) {
        printError("  Move src to dest next time system is rebooted");
        char savedFile[_MAX_PATH];
        strcpy((char *)savedFile, src);
        strcat((char *)savedFile, ".BAK");
        CopyFile(src, (char *)savedFile, FALSE);
        int result = MoveFileEx(src, dest, 
                                MOVEFILE_DELAY_UNTIL_REBOOT | 
                                MOVEFILE_REPLACE_EXISTING);
        if (result == 0) {
            printError("  MoveFileEx failed");
        }
        CopyFile((char *)savedFile, src, FALSE);
    }

    // copy WindowsAccessBridge.dll into \Windows\System directory
    GetSystemDirectory(system32, _MAX_PATH);
    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\WindowsAccessBridge.dll", _MAX_DIR);
    strncpy(dest, system32, _MAX_DIR);
    strncat(dest, "\\WindowsAccessBridge.dll", _MAX_DIR);

    PrintDebugString("  CopyFile: %s -> %s", src, dest);
    if (CopyFile(src, dest, FALSE) == 0) {
        printError("  Move src to dest next time system is rebooted");
        char savedFile[_MAX_PATH];
        strcpy((char *)savedFile, src);
        strcat((char *)savedFile, ".BAK");
        CopyFile(src, (char *)savedFile, FALSE);
        int result = MoveFileEx(src, dest, 
                                MOVEFILE_DELAY_UNTIL_REBOOT | 
                                MOVEFILE_REPLACE_EXISTING);
        if (result == 0) {
            printError("  MoveFileEx failed");
        }
        CopyFile((char *)savedFile, src, FALSE);
    }

    // copy JAWTAccessBridge.dll into \Windows\System directory
    GetSystemDirectory(system32, _MAX_PATH);
    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\JAWTAccessBridge.dll", _MAX_DIR);
    strncpy(dest, system32, _MAX_DIR);
    strncat(dest, "\\JAWTAccessBridge.dll", _MAX_DIR);

    PrintDebugString("  CopyFile: %s -> %s", src, dest);
    if (CopyFile(src, dest, FALSE) == 0) {
        printError("  Move src to dest next time system is rebooted");
        char savedFile[_MAX_PATH];
        strcpy((char *)savedFile, src);
        strcat((char *)savedFile, ".BAK");
        CopyFile(src, (char *)savedFile, FALSE);
        int result = MoveFileEx(src, dest, 
                                MOVEFILE_DELAY_UNTIL_REBOOT | 
                                MOVEFILE_REPLACE_EXISTING);
        if (result == 0) {
            printError("  MoveFileEx failed");
        }
        CopyFile((char *)savedFile, src, FALSE);
    }

    // copy jar and property files appropriate for the JVM
    JVM *current = jvmListHead;
    while (current != NULL) {
        // PrintDebugString("  next JVM = %s", current->javahome);
        if (current->has1_3apis) { // JVM is 1.3 or later
            // PrintDebugString("  jdk 1.3 or later");
            
            // copy accessibility.properties into libpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\accessibility.properties", _MAX_DIR);
            strncpy(dest, current->libpath, _MAX_DIR);
            strncat(dest, "\\accessibility.properties", _MAX_DIR);
            copyTheFile(src, dest);
            
            // copy jaccess-examples.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\jaccess-examples.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\jaccess-examples.jar", _MAX_DIR);
            copyTheFile(src, dest);
            
            // copy jaccess-1_4.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\jaccess-1_4.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\jaccess.jar", _MAX_DIR);
            copyTheFile(src, dest);

            // copy access-bridge.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\access-bridge.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\access-bridge.jar", _MAX_DIR);
            copyTheFile(src, dest);

        } else {
            // jdk 1.2.x
            // PrintDebugString("  jdk 1.2.x");
            
            // copy accessibility.properties into libpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\accessibility.properties", _MAX_DIR);
            strncpy(dest, current->libpath, _MAX_DIR);
            strncat(dest, "\\accessibility.properties", _MAX_DIR);
            copyTheFile(src, dest);

            // copy jaccess-examples.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\jaccess-examples.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\jaccess-examples.jar", _MAX_DIR);
            copyTheFile(src, dest);

            // copy jaccess-1_2.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\jaccess-1_2.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\jaccess.jar", _MAX_DIR);
            copyTheFile(src, dest);

            // copy access-bridge.jar into jarpath
            strncpy(src, installerDir, _MAX_DIR);
            strncat(src, "\\installerFiles", _MAX_DIR);
            strncat(src, "\\access-bridge.jar", _MAX_DIR);
            strncpy(dest, current->jarpath, _MAX_DIR);
            strncat(dest, "\\access-bridge.jar", _MAX_DIR);
            copyTheFile(src, dest);
        }
        current = current->next;
    }
    return 0;
}

