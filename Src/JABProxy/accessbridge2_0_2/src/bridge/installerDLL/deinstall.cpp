/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)deinstall.cpp	1.8 05/11/14
 */

#include "installerDLL.h"

char *accessibilityDotProperties = "\\lib\\accessibility.properties";
char *jaccessDotJar = "\\lib\\ext\\jaccess.jar";
char *jaccessExamplesDotJar = "\\lib\\ext\\jaccess-examples.jar";
char *accessBridgeDotJar = "\\lib\\ext\\access-bridge.jar";

/*
 * Deinstall the bridge from all drives
 */
UINT
Installer::deinstallAllDrives() {
 
    PrintDebugString("deinstallAllDrives");

    // search all drives for installed files

    char drives[27][3] = { "", "a:", "b:", "c:", "d:", "e:", "f:", "g:", "h:", "i:", "j:", "k:", "l:", "m:", "n:", "o:", "p:", "q:", "r:", "s:", "t:", "u:", "v:", "w:", "x:", "y:", "z:" };
 
    // save current drive and directory 
    int curDrive = _getdrive();
    char curPath[_MAX_PATH];
    _getcwd(curPath, _MAX_PATH);
    
    // iterate through all drives
    jvmListHead = NULL;
    for(int drive = 1; drive <= 26; drive++ ) {
        PrintDebugString("  next drive is %s", drives[drive]);

        // skip non-fixed drives (e.g., network drives)
        int type = GetDriveType(drives[drive]);
        switch(type) {
        case DRIVE_NO_ROOT_DIR:
            PrintDebugString("  skipping unmounted drive");
            continue;

        case DRIVE_REMOVABLE:
            PrintDebugString("  skipping removable drive");
            continue;

        case DRIVE_FIXED:       // only search fixed drives
            PrintDebugString("  fixed drive");
            break;

        case DRIVE_REMOTE:
            PrintDebugString("  skipping network drive");
            continue;

        case DRIVE_CDROM:
            PrintDebugString("  skipping cd-rom drive");
            continue;

        case DRIVE_UNKNOWN: 
        default:
            PrintDebugString("  skipping unknown drive type");
            continue;
        }

        // if we can switch to the drive, it exists
        if( !_chdrive(drive) ) {
            // start in root
            PrintDebugString("  searching drive %s", drives[drive]);
            _chdir("\\");
            try {
                char *rootDir = strdup(drives[drive], "\\");
                deinstallDrive(rootDir);    // find JVMs on this drive
            } catch (char *str) {
                PrintDebugString("  deleteDrive error for drive: %s %s", 
                                 drives[drive], str);
            }
        }
    }

    // restore original drive and directory
   _chdrive(curDrive);
   _chdir(curPath);
    
    // delete the DLLs
    deleteDLLs();
    
    // deinstall .bak files
    deleteBakFiles();

    // delete the JAR and properties files we just found
    deleteJvmFiles();
    
    /* restore original drive and directory */
    _chdrive(curDrive);
    _chdir(curPath);
    
    return 0;
}

/**
 * finds the JVMs on the current drive for deinstalling
 */
void 
Installer::deinstallDrive(const char *parent) {
    
    struct _finddata_t fileinfo; // file information
    char filespec[_MAX_DIR];     // search pattern

    // build the target file specification: "parent\*.*"
    strncpy((char *)filespec, parent, sizeof(filespec));
    if (strcmp(parent, "\\") != 0) {
        strncat((char *)filespec, "\\", sizeof(filespec));
    }
    strncat((char *)filespec, "*.*", sizeof(filespec));
    // PrintDebugString("deinstallDrive parent: %s; filespec: %s", parent, filespec);
    
    // initialize the search functions
    long handle = _findfirst((char *)filespec, &fileinfo);
    
    if (handle >= 0) {
        do {
            // ignore . and .. directories.
            if (strcmp(fileinfo.name, ".") != 0 && 
                strcmp(fileinfo.name, "..") != 0) {

                if (fileinfo.attrib & _A_SUBDIR) {
                    // found a directory so recurse into the directory
                    strncpy((char *)filespec, parent, sizeof(filespec));
                    if (strcmp(parent, "\\") != 0) {
                        strncat((char *)filespec, "\\", sizeof(filespec));
                    }
                    strncat((char *)filespec, fileinfo.name, sizeof(filespec));
                    deinstallDrive((char *)filespec);

                } else {        // regular file
                    // did we find a JVM?
                    if (strcmp(fileinfo.name, "java.exe") == 0) {

                        char *javahome = normalize((char *)parent);
                        PrintDebugString("\n  => found java.exe in: %s", javahome);
                        
                        // does the JVM list already contain this JVM?
                        if (jvmListHead->contains(javahome)) {
                            PrintDebugString("  %s already in jvmList", javahome);
                            return;
                        }
                        
                        // should the bridge be deinstalled from this JVM?
                        int result = isInstallable(javahome);

                        if (result >= AccessBridgeTester_ACCESS_BRIDGE_COMPATIBILITY) {
                            // found a JVM for deinstalling the bridge
                            jvmListHead->append(javahome, result);
                        }
                    }
                }
            }
        } while (_findnext(handle, &fileinfo) == 0); // get the next file
        
        // finished searching this directory
        _findclose(handle);
    }
}

/*
 * delete the specified file
 */
void
Installer::deleteTheFile(char *src) {
    char dest[_MAX_DIR];

    PrintDebugString("  deleting %s", src);
    BOOL result = DeleteFile(src);
    if (result == 0) {
        printError("    DeleteFile failed");

        // copy the file to the %TEMP% file after reboot
        TCHAR tchBuffer[_MAX_PATH];
        LPTSTR lpszSystemInfo = tchBuffer;
        DWORD dwResult = ExpandEnvironmentStrings("%TEMP%",
                                                  lpszSystemInfo, 
                                                  _MAX_PATH);
        if (dwResult <= _MAX_PATH) {
            strncpy(dest, lpszSystemInfo, _MAX_DIR);
            strncat(dest, "\\", _MAX_DIR);
            strncat(dest, src, _MAX_DIR);
            PrintDebugString("  moving %s to %s", src, dest);
            result = MoveFileEx(src, dest, 
                                MOVEFILE_DELAY_UNTIL_REBOOT | 
                                MOVEFILE_REPLACE_EXISTING);
            if (result == 0) {
                printError("    MoveFileEx failed");
            }
        }
    }
}

/*
 * deinstall .bak files
 */
void
Installer::deleteBakFiles() {

    PrintDebugString("deleteBakFiles");

    char src[_MAX_DIR];

    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\JavaAccessBridge.dll.BAK", _MAX_DIR);
    deleteTheFile((char *)src);

    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\JAWTAccessBridge.dll.BAK", _MAX_DIR);
    deleteTheFile((char *)src);

    strncpy(src, installerDir, _MAX_DIR);
    strncat(src, "\\installerFiles", _MAX_DIR);
    strncat(src, "\\WindowsAccessBridge.dll.BAK", _MAX_DIR);
    deleteTheFile((char *)src);
}

/*
 * delete the DLLs
 */
void 
Installer::deleteDLLs() {

    PrintDebugString("deleteDLLs");

    char system32[_MAX_PATH];
    char src[_MAX_DIR];
    char dest[_MAX_DIR];

    GetSystemDirectory(system32, _MAX_PATH);

    strncpy(src, system32, _MAX_DIR);
    strncat(src, "\\WindowsAccessBridge.dll", _MAX_DIR);
    deleteTheFile(src);

    strncpy(src, system32, _MAX_DIR);
    strncat(src, "\\JavaAccessBridge.dll", _MAX_DIR);
    deleteTheFile(src);

    strncpy(src, system32, _MAX_DIR);
    strncat(src, "\\JAWTAccessBridge.dll", _MAX_DIR);
    deleteTheFile(src);
}

/*
 * Remove the JAR and properties files from each JVM
 */
void
Installer::deleteJvmFiles() {

    PrintDebugString("deleteJvmFiles");
    jvmListHead->dump();

    char dest[_MAX_DIR];

    JVM *current = jvmListHead;
    while (current != NULL) {

        PrintDebugString("deleting bridge in %s", current->javahome);

        strncpy((char *)dest, current->javahome, _MAX_DIR);
        strncat((char *)dest, accessibilityDotProperties, _MAX_DIR);
        deleteTheFile((char *)dest);

        strncpy((char *)dest, current->javahome, _MAX_DIR);
        strncat((char *)dest, jaccessDotJar, _MAX_DIR);
        deleteTheFile((char *)dest);

        strncpy((char *)dest, current->javahome, _MAX_DIR);
        strncat((char *)dest, jaccessExamplesDotJar, _MAX_DIR);
        deleteTheFile((char *)dest);

        strncpy((char *)dest, current->javahome, _MAX_DIR);
        strncat((char *)dest, accessBridgeDotJar, _MAX_DIR);
        deleteTheFile((char *)dest);

        current = current->next;
    }
}






