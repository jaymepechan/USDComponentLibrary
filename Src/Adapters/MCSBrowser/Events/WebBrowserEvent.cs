/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.USD.ComponentLibrary
{
    public enum WebBrowserEvent
    {
        BeforeNavigate,
        FrameBeforeNavigate,
        FrameNavigateComplete,
        FrameNewWindow,
        NavigateComplete,
        NewWindow,
        OnQuit,
        WindowActivate,
        WindowMove,
        WindowResize,
        BeforeNavigate2,
        ClientToHostWindow,
        CommandStateChange,
        DocumentComplete,
        DownloadBegin,
        DownloadComplete,
        FileDownload,
        NavigateComplete2,
        NavigateError,
        NewWindow2,
        NewWindow3,
        OnVisible,
        OnToolBar,
        OnMenuBar,
        OnStatusBar,
        OnFullScreen,
        OnTheaterMode,
        PrintTemplateInstantiation,
        PrintTemplateTeardown,
        PrivacyImpactedStateChange,
        ProgressChange,
        PropertyChange,
        Quit,
        SetPhishingFilterStatus,
        SetSecureLockIcon,
        StatusTextChange,
        TitleChange,
        UpdatePageStatus,
        WindowSetResizable,
        WindowSetLeft,
        WindowSetTop,
        WindowSetWidth,
        WindowSetHeight,
        WindowClosing,
        WindowStateChanged,
        LegacyBrowserExtendedBeforeNavigate,
        LegacyBrowserExtendedBeforeNewWindow,
        LegacyBrowserExtendedDownloadStarted,
        LegacyBrowserExtendedDownloadCompleted,
        LegacyBrowserExtendedWebBrowserNavigated,
        LegacyBrowserExtendedWebBrowserDocumentCompleted,
        BrowserProcessExited,
        BrowserObjectAppeared,
        PageLoadComplete
    }
}
