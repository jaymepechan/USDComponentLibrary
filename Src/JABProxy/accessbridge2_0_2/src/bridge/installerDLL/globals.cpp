/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)globals.cpp	1.4 05/11/11
 */

/* 
 * Java Access Bridge installer
 *
 * @author Lynn Monsanto
 */

#include "../installerDLL/installerDLL.h"

Installer *theInstaller;
char theInstallerClassName[] = "InstallerWindow";

HINSTANCE theInstance;
char szAppName [] = "IDD_PROPPAGE_MEDIUM";

HWND theInstallerWindow;
HWND theDialogWindow;
BOOL exitSuccess = FALSE;

JVM *jvmListHead = NULL;
