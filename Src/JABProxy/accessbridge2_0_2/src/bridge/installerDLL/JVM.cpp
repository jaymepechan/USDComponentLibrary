/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)JVM.cpp	1.4 05/11/11
 */

#include "installerDLL.h"

/*
 * returns a copy of str
 */
char *strdup(const char *str) {
    if (str == NULL) {
        return NULL;
    }
    int len = strlen(str) + 1;
    char *dup = new char[len];
    strcpy(dup, str);
    return dup;
}

/*
 * returns a copy of str1 concatanated with str2
 */
char *strdup(const char *str1, const char *str2) {
    if (str1 == NULL || str2 == NULL) {
        return NULL;
    }
    char *dup = new char[strlen(str1) + strlen(str2) + 1];
    strcpy(dup, str1);
    strcat(dup, str2);
    return dup;
}

/*
 * returns the base directory for installing the bridge
 */
char *
getJvmDir(char *dir) {

    // strip trailing \bin if necessary
    char *javahome = strdup(dir);
    char *cp = javahome + strlen(javahome) - 1;
    if (strlen(javahome) >= 4 &&
        *(cp)     == 'n' && 
        *(cp - 1) == 'i' && 
        *(cp - 2) == 'b' && 
        *(cp - 3) == '\\') {

        *(cp - 3) = 0;
    }
    // PrintDebugString("getJvmDir: javahome: %s", javahome);

    /*
     * Three cases:
     *   dir is JDK_HOME & JDK_HOME\jre exists (J2SE)
     *   dir is JDK_HOME\jre (J2SE)
     *   dir is JDK_HOME & JDK_HOME\jre does not exist (J2RE)
     */
    struct _finddata_t filedata;
    char *path = strdup(javahome, "\\jre");
    long file = _findfirst(path, &filedata);
    if (file != -1L) { 
        return path;            // handle case #1
    }
    return javahome;
}

/*
 * JVM node constructors
 */
JVM::JVM(char *dir, int result) {

    // PrintDebugString("  JVM ctor: dir = %s, result = %d", dir, result);

    this->javahome = NULL; 
    this->binpath = NULL;
    this->libpath = NULL;
    this->jarpath = NULL;
    this->next = NULL;
    this->has1_3apis = FALSE;

    // set the JVM paths
    char *base = getJvmDir(dir);

    this->javahome = strdup(base);
    this->binpath = strdup(base, "\\bin");
    this->libpath = strdup(base, "\\lib");
    this->jarpath = strdup(base, "\\lib\\ext");

    if (result & AccessBridgeTester_HAS_1_3_APIS) {
        has1_3apis = TRUE;
    }
}

JVM::JVM(char *dir) {

    // PrintDebugString("  JVM ctor: dir = %s", dir);

    this->javahome = NULL; 
    this->binpath = NULL;
    this->libpath = NULL;
    this->jarpath = NULL;
    this->next = NULL;
    this->has1_3apis = FALSE;

    // set the javahome path
    char *base = getJvmDir(dir);
    this->javahome = strdup(base);
}

/*
 * "java.exe" may be found in two subdirectories: \bin and jre\bin.
 * Returns whether jre\bin\java.exe has already been found.
 */
BOOL
JVM::contains(char *javahome) {

    // PrintDebugString("contains?: %s", javahome);
    JVM *current = jvmListHead;
    BOOL contains = FALSE;
    while (current != NULL) {
        // strip trailing \jre if necessary
        char *next = strdup(current->javahome);
        char *cp = next + strlen(next) - 1;
        if (strlen(next) >= 4 &&
            *(cp)     == 'e' && 
            *(cp - 1) == 'r' && 
            *(cp - 2) == 'j' && 
            *(cp - 3) == '\\') {
            
            *(cp - 3) = 0;
        }
        // PrintDebugString("  next = %s; javahome: %s", next, javahome);

        if (strcmp(next, javahome) == 0) {
            PrintDebugString("    found a duplicate");
            return TRUE;
        }
        current = current->next;
    }
    return FALSE;
}

/*
 * Returns whether search is looping
 */
BOOL
JVM::isLooping(char *javahome) {

    // PrintDebugString("isLooping?: %s", javahome);
    JVM *current = jvmListHead;
    while (current != NULL) {
        // PrintDebugString("  next javahome = %s", current->javahome);
        if (strcmp(javahome, current->javahome) == 0) {
            PrintDebugString("  LOOPING!!!");
            return TRUE;
        }
        current = current->next;
    }
    // PrintDebugString("  not looping");
    return FALSE;
}

/*
 * Appends a new JVM node to the list
 */
void JVM::append(char *javahome, BOOL result) {
    // PrintDebugString("  JVM::append: javahome = %s, result = %d", javahome, result);
    if (javahome == NULL) {
        return;
    }
    JVM *newNode = new JVM(javahome, result);
    newNode->next = jvmListHead;
    jvmListHead = newNode;
}

void JVM::append(char *javahome) {
    // PrintDebugString("  JVM::append: javahome = %s", javahome);
    if (javahome == NULL) {
        return;
    }
    JVM *newNode = new JVM(javahome);
    newNode->next = jvmListHead;
    jvmListHead = newNode;
}

/*
 * For debugging: dumps the JVM list
 */
void JVM::dump() {

    PrintDebugString("\nDumping JVM list...");
    JVM *current = jvmListHead;
    while (current != NULL) {
        PrintDebugString("  javahome = %s", current->javahome);
        current = current->next;
    }
    PrintDebugString("  ... end dumping JVM list");
    PrintDebugString("\n");
}

