/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)search.cpp	1.8 05/11/14
 */

#include "installerDLL.h"
#include <string>

/**
 * Search directory tree for JVMs where the bridge can be installed
 */
void 
Installer::searchDrive(const char *parent) {
    
    struct _finddata_t fileinfo; // file information
    char filespec[_MAX_DIR];     // search pattern

    // build the target file specification: "parent\*.*"
    strncpy((char *)filespec, parent, sizeof(filespec));
    if (strcmp(parent, "\\") != 0) {
        strncat((char *)filespec, "\\", sizeof(filespec));
    }
    strncat((char *)filespec, "*.*", sizeof(filespec));
    // PrintDebugString("searchDrive parent: %s; filespec: %s", parent, filespec);
    
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
                    searchDrive((char *)filespec);

                } else {        // regular file
                    // did we find a JVM?
                    if (strcmp(fileinfo.name, "java.exe") == 0) {

                        char *javahome = normalize((char *)parent);
                        PrintDebugString("\n  found java.exe in: %s", javahome);
                        
                        // does the JVM list already contain this JVM?
                        if (jvmListHead->contains(javahome)) {
                            PrintDebugString("  %s already in jvmList", javahome);
                            return;
                        }
                        
                        // can the bridge be installed in this JVM?
                        int result = isInstallable(javahome);

                        if (result >= AccessBridgeTester_ACCESS_BRIDGE_COMPATIBILITY) {
                            // found a JVM for installing the bridge
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
 * Search all drives for JVMs where the bridge can be installed
 */
void 
Installer::searchAllDrives() {

    char drives[27][3] = { "", "a:", "b:", "c:", "d:", "e:", "f:", "g:", "h:", "i:", "j:", "k:", "l:", "m:", "n:", "o:", "p:", "q:", "r:", "s:", "t:", "u:", "v:", "w:", "x:", "y:", "z:" };
 
    PrintDebugString("searchAllDrives");
    
    // save current drive and directory 
    int curDrive = _getdrive();
    char curPath[_MAX_PATH];
    _getcwd(curPath, _MAX_PATH);
    
    // iterate through all drives
    jvmListHead = NULL;
    for(int drive = 1; drive <= 26; drive++ ) {
        PrintDebugString("next drive is %s", drives[drive]);

        // skip non-fixed drives (e.g., network drives)
        int type = GetDriveType(drives[drive]);
        switch(type) {
        case DRIVE_NO_ROOT_DIR:
            PrintDebugString("  skipping unmounted drive");
            continue;

        case DRIVE_REMOVABLE:
            PrintDebugString("  skipping removable drive");
            continue;

        case DRIVE_FIXED:       // search only fixed drives
            PrintDebugString("  fixed drive: search here");
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
                searchDrive(rootDir);    // find JVMs on this drive
            } catch (char *str) {
                PrintDebugString("  searchDrive error for drive: %s %s", 
                                 drives[drive], str);
            }
        }
    }

    // restore original drive and directory
   _chdrive(curDrive);
   _chdir(curPath);
}

/*
 * returns whether the bridge can be installed on a JVM
 */
int
Installer::isInstallable(char *dir) {

    PrintDebugString("  isInstallable: %s", dir);

    if (dir == NULL) {
        return -1;
    }

    // remember the cwd
    char currentDir[_MAX_PATH];
    _getcwd(currentDir, _MAX_PATH);

    // find java.exe
    char *bindir = strdup(dir, "\\bin");
    _chdir(bindir);

    struct _finddata_t firstFile;
    long file = _findfirst("java.exe", &firstFile);
    BOOL foundJava = (file != -1L);

    _findclose(file);
    _chdir(currentDir);
    if (! foundJava) {
        PrintDebugString("  no java.exe in %s", (char *)bindir);
        free((void *)bindir);
        return -1;
    }
    
    // temporarily rename the accessibilities file so the bridge won't be loaded
    char *libdir = deactivateAccessibilityDotProperties(dir);
    
    // run the AccessBridgeTester on the JVM
    _chdir(installerDir);

    char *command = strdup(bindir, "\\javaw.exe");
    PrintDebugString("  testing: %s", command);
    int result = _spawnl(_P_WAIT, command,
                         (const char *) "javaw.exe",
                         (const char *) "-cp .",
                         (const char *) "AccessBridgeTester", NULL );
    
    PrintDebugString("  test result = %d", result);
    free((void *)bindir);
    free((void *)command);
    _chdir(currentDir);

    // restore the accessibility.properties file name
    if (libdir != NULL) {
        reactivateAccessibilityDotProperties(libdir);
    }
    return result;
}

/*
 * deactivates accessibility.properties if it exists
 * and returns the directory containing the file
 */
char *
deactivateAccessibilityDotProperties(char *dir) {

    // PrintDebugString("deactivateAccessibilityDotProperties %s", dir);

    // remember the cwd
    char currentDir[_MAX_PATH];
    _getcwd(currentDir, _MAX_PATH);

    char *jvmDir = getJvmDir(dir);
    char *libdir = strdup(jvmDir, "\\lib");
    
    if (_chdir(libdir) != 0) {
        // PrintDebugString("  %s does not exist", libdir);
        return NULL;
    }

    struct _finddata_t firstFile;
    long file = _findfirst("accessibility.properties", &firstFile);
    if (file == -1L) {
        // PrintDebugString("  accessibility.properties does not exist");
        return NULL;
    }
    // PrintDebugString("  renaming accessibility.properties");
    remove(",accessibility.properties");
    if (rename("accessibility.properties", ",accessibility.properties") != 0) {
        // printError("  cannot rename accessibility.properties");
        _findclose(file);
        _chdir(currentDir);
        return NULL;
    }
    _findclose(file);
    _chdir(currentDir);
    
    // PrintDebugString("  success, returning %s", libdir);
    return libdir;
}

/*
 * reactivates accessibility.properties
 */
void
reactivateAccessibilityDotProperties(char *libdir) {

    // PrintDebugString("reactivateAccessibilityDotProperties %s", libdir);

    // remember the cwd
    char currentDir[_MAX_PATH];
    _getcwd(currentDir, _MAX_PATH);
    _chdir(libdir);

    struct _finddata_t firstFile;
    long file = _findfirst(",accessibility.properties", &firstFile);
    if (file == -1L) {
        // PrintDebugString("  %s\\,accessibility.properties does not exist", libdir);
        return;
    }
    // PrintDebugString("  renaming %s\\,accessibility.properties", libdir);
    remove("accessibility.properties");
    if (rename(",accessibility.properties", "accessibility.properties") != 0) {
        printError("  cannot rename ,accessibility.properties");
    } 
    _findclose(file);
    _chdir(currentDir);
}

/* 
 * normalizes the path string
 */
char *
Installer::normalize(char *str) {

    // PrintDebugString("normalize %s", str);
    if (str == NULL) {
        return NULL;
    }

    // removing any trailing "\bin"
    char *cp = str;
    while (*cp != NULL) {
        cp++;
    }
    if (strlen(str) >= 4 &&
        *(cp - 4) == '\\' &&
        *(cp - 3) == 'b' &&
        *(cp - 2) == 'i' &&
        *(cp - 1) == 'n') {
        
        *(cp - 4) = NULL;
    }
    
    // remove any trailing "\jre"
    cp = str;
    while (*cp != NULL) {
        cp++;
    }
    if (strlen(str) >= 4 &&
        *(cp - 4) == '\\' &&
        *(cp - 3) == 'j' &&
        *(cp - 2) == 'r' &&
        *(cp - 1) == 'e') {
        
        *(cp - 4) = NULL;
    }
    cp = str;
    while (*cp != NULL) {
        cp++;
    }

    // remove any surrounding parenthesis
    if (strlen(str) >= 2 && *str == '"' && *(cp - 1) == '"') {
        *(cp - 1) = NULL;
        // PrintDebugString("  returning %s", str + 1);
        return str + 1;
    } else {
        // PrintDebugString("  returning %s", str);
        return str;
    }
}
