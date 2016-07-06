/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
var SAVE_PARAMETERS = null;
var SAVE_SOURCECOMPONENT = null;
var SAVE_INSTANCE = null;
var SAVE_EVENTPROCESSING = null;

var XrmForm = null;
function RetrieveXrmForm() {
    if (XrmForm == null) {
        var formDoc = FindDocWithId(document, "crmForm");
        if (formDoc != null)
            XrmForm = formDoc.parentWindow;
    }
}

function FindDocWithId(doc, id) {
    if (doc.getElementById(id) == null) {
        for (var i = 0; i < doc.frames.length; i++) {
            var frameDoc = FindDocWithId(doc.frames[i].document, id);
            if (frameDoc != null)
                return frameDoc;
        }
    }
    else {
        return doc;
    }
}

function Hook2013Events() {
    var deferExecuteEventCancelCount = 0;
    deferExecuteEvent = function () {
        RetrieveXrmForm();
        if (XrmForm == null || typeof (XrmForm.Xrm) == 'undefined' || XrmForm.Xrm == null || typeof (XrmForm.Xrm.Page) == 'undefined' || XrmForm.Xrm.Page == null || typeof (XrmForm.Xrm.Page.data) == 'undefined' || XrmForm.Xrm.Page.data == null
			|| (typeof (isActionQueueActive) != 'undefined' && isActionQueueActive() && APPLICATION_VERSION < 6.0) || (typeof (XrmForm.window["FinishPostInitializationTimestamp"]) == 'undefined' && APPLICATION_VERSION >= 6.0)) {
            deferExecuteEventCancelCount++
            if (deferExecuteEventCancelCount < 30)
                setTimeout('deferExecuteEvent()', 500);
            return;
        }
        if (APPLICATION_VERSION < 6.0 && typeof (XrmForm.EDIT_PRELOAD) != 'undefined')
            XrmForm.EDIT_PRELOAD = false;
        
        if (APPLICATION_VERSION >= 6.0) // 2013 or higher
        {
            if (typeof (Mscrm.NavigationManager.prototype.oldHandleNavigateRequestCallback) == 'undefined') {
                Mscrm.NavigationManager.prototype.oldHandleNavigateRequestCallback = Mscrm.NavigationManager.prototype.handleNavigateRequestCallback;
            }
            Mscrm.NavigationManager.prototype.handleNavigateRequestCallback = function (parameters, sourceComponent) {
                if (SAVE_EVENTPROCESSING == true) {   // if this is a sitemap selection, then we already processed it so skip the extra processing
                    SAVE_EVENTPROCESSING = false;
                    this.oldHandleNavigateRequestCallback(parameters, sourceComponent);
                    return;
                }
                SAVE_PARAMETERS = parameters;
                SAVE_SOURCECOMPONENT = sourceComponent;
                SAVE_EVENTPROCESSING = false;
                SAVE_INSTANCE = this;
                window.open('http://event.usd/?eventname=handleNavigateRequestCallback&uri=' + encodeURIComponent(parameters['uri']));
            }

            if (Mscrm.NavBar == 'undefined') {
                SAVE_EVENTPROCESSING = true;
                SAVE_PARAMETERS = parameters;
                SAVE_INSTANCE = this;
                window.open('http://event.usd/?eventname=raiseNavigateRequest&uri=' + encodeURIComponent(parameters['uri']));
            }
            if (Mscrm.NavBar != null) {
                if (typeof (Mscrm.NavBar.prototype.oldRaiseNavigateRequest) == 'undefined') {
                    Mscrm.NavBar.prototype.oldRaiseNavigateRequest = Mscrm.NavBar.prototype.raiseNavigateRequest;
                }
                Mscrm.NavBar.prototype.raiseNavigateRequest = function (parameters) {
                    // called when sitemap menu is used
                    // parameters["uri"] has relative URL
                    SAVE_EVENTPROCESSING = true;
                    SAVE_PARAMETERS = parameters;
                    SAVE_INSTANCE = this;
                    window.open('http://event.usd/?eventname=raiseNavigateRequest&uri=' + encodeURIComponent(parameters['uri']));
                }
            }
        }

    }
    deferExecuteEvent();
}

Hook2013Events();