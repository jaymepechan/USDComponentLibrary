/*
 * Copyright 2005 Sun Microsystems, Inc. All rights reserved.
 * SUN PROPRIETARY/CONFIDENTIAL. Use is subject to license terms.
 */

/*
 * @(#)main.cpp	1.3 05/11/11
 */

/* 
 * Java Access Bridge installer
 *
 * @author Lynn Monsanto
 */

#include "../installerDLL/installerDLL.h"

/**
 * WinMain
 */
int WINAPI WinMain(HINSTANCE hInst, HINSTANCE hPrevInst, 
                   LPSTR lpCmdLine, int nShowCmd) {
    MSG msg;
    HWND hwnd;
    
    theInstance = hInst;
    strncpy((char *)installerDir, "c:\\Program Files\\Java Access Bridge",
            sizeof(installerDir));
    PrintDebugString("WinMain: running from %s", installerDir);
    PrintDebugString("WinMain: lpCmdLine = %s", (char *)lpCmdLine);
    
    theInstaller = new Installer(hInst);
    
    // install the bridge
    hwnd = theInstaller->createMainWindow(nShowCmd);
    PostMessage(hwnd, INSTALLER_START_INSTALLATION, (WPARAM) 0, (LPARAM) 0);

    while (GetMessage(&msg, NULL, 0, 0)) {
        if (theDialogWindow == 0 || !IsDialogMessage(theDialogWindow, &msg)) {
            TranslateMessage(&msg);
            DispatchMessage(&msg);
        }
    }
    return msg.wParam;
}

/**
 * creates the installer GUI
 */
HWND
Installer::createMainWindow(int showCommand) {
    WNDCLASSEX wcl;
    HWND hwnd;
    char windowName[] = "Installer Window";

    PrintDebugString("createMainWindow");

    wcl.cbSize = sizeof(WNDCLASSEX);
    wcl.hInstance = theInstance;
    wcl.lpszClassName = windowName;
    wcl.lpfnWndProc = mainWindowProc;
    wcl.style = 0;
    wcl.hIcon = LoadIcon(NULL, IDI_APPLICATION);
    wcl.hIconSm = LoadIcon(NULL, IDI_WINLOGO);
    wcl.hCursor = LoadCursor(NULL, IDC_ARROW);
    wcl.lpszMenuName = 0;   // no menu
    wcl.cbClsExtra = 0;
    wcl.cbWndExtra = 0;

    wcl.hbrBackground = (HBRUSH) GetStockObject(WHITE_BRUSH);

    if(!RegisterClassEx(&wcl)) {
        return NULL;
    } 
    
    hwnd = CreateWindow(windowName,
                        "Java Access Bridge for Microsoft Windows installer",
                        WS_OVERLAPPEDWINDOW,
                        CW_USEDEFAULT,
                        CW_USEDEFAULT,
                        CW_USEDEFAULT,
                        CW_USEDEFAULT,
                        HWND_DESKTOP,
                        NULL,
                        theInstance,
                        NULL);

    // don't display main window
    // ShowWindow(hwnd, SW_SHOW);
    // UpdateWindow(hwnd);
    return hwnd;
}

/*
 * installer WindowProc
 */
LRESULT CALLBACK mainWindowProc(HWND hWnd, UINT message, UINT wParam, LONG lParam) {

    // PrintDebugString("mainWindowProc = %d", message);

    switch (message) {

    case WM_QUIT:
        PrintDebugString("mainWindowProc: WM_QUIT");
        PrintDebugString("mainWindowProc: quitting...");
        PostQuitMessage(0);

    case WM_DESTROY:
        PrintDebugString("mainWindowProc: WM_DESTROY");
        PostQuitMessage(0);
        return 0;

    case INSTALLER_START_INSTALLATION:
        PrintDebugString("mainWindowProc: INSTALLER_START_INSTALLATION");
        theInstaller->mainInstallRoutine();
        return 0;
    }
    return DefWindowProc(hWnd, message, wParam, lParam);
}









