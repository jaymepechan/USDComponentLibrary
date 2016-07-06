/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
var USDFORMDATA = "";

//[START]: Code taken from CRM
// Defines the type of attribute. The value matches Microsoft.Crm.Metadata.AttributeMetadata.Type.Name
// Copied from src\Core\Application\web\ScriptSharp\_Controls\InlineEditControls\Constants.cs
var AttributeType =
{
    Bit: "bit",
    Picklist: "picklist",
    Owner: "owner",
    Date: "date",
    DateTime: "datetime",
    NVarchar: "nvarchar",
    NText: "ntext",
    Customer: "customer",
    Lookup: "lookup",
    Money: "money",
    Integer: "int",
    FloatAttribute: "float",
    Decimal: "decimal",
    StateCode: "state",
    Status: "status",
    PartyList: "partylist",
    Guid: "uniqueidentifier",
    PrimaryKey: "primarykey"
}

// Attribute types exposed through client API
var ExposedAttributeType =
{
    Boolean: "boolean",
    DateTime: "datetime",
    Decimal: "decimal",
    Double: "double",
    Integer: "integer",
    Lookup: "lookup",
    Memo: "memo",
    Money: "money",
    OptionSet: "optionset",
    String: "string"
}

var AttributeFormat =
{
    Date: "date",
    Datetime: "datetime",
    Duration: "duration",
    Email: "email",
    EmailBody: "emailbody",
    Language: "language",
    None: "none",
    Text: "text",
    Tickersymbol: "tickersymbol",
    Timezone: "timezone",
    Url: "url",
    Phone: "phone",
    Textarea: "textarea"
}
//[END]: Code taken from CRM

function GetMappedAttributeType(attributeType, attributeFormat) {
    // The following logic is in similar to the one present in file
    // \src\Core\Application\web\ScriptSharp\_Controls\InlineEditControls\InlineControlFactory.cs
    switch (attributeType) {
        case AttributeType.Bit:
            return ExposedAttributeType.Boolean;
        case AttributeType.DateTime:
            return ExposedAttributeType.DateTime;
        case AttributeType.FloatAttribute:
            return ExposedAttributeType.Double;
        case AttributeType.Integer:
            switch (attributeFormat) {
                case AttributeFormat.Language:
                case AttributeFormat.TimeZone:
                    return ExposedAttributeType.OptionSet;
                default:
                    return ExposedAttributeType.Integer;
            }
        case AttributeType.Decimal:
            return ExposedAttributeType.Decimal;
        case AttributeType.Lookup:
        case AttributeType.Owner:
        case AttributeType.Customer:
            return ExposedAttributeType.Lookup;
        case AttributeType.Picklist:
        case AttributeType.StateCode:
        case AttributeType.Status:
            return ExposedAttributeType.OptionSet;
        case AttributeType.Guid:
        case AttributeType.NVarchar:
        case AttributeType.PrimaryKey:
            switch (attributeFormat) {
                case AttributeFormat.Textarea:
                    return ExposedAttributeType.Memo;
                default:
                    return ExposedAttributeType.String;
            }
        case AttributeType.NText:
            return ExposedAttributeType.Memo;
        case AttributeType.Money:
            return ExposedAttributeType.Money;
    }

    // No match. Return whatever was passed in.
    return attributeType;
}

if (typeof (deferCancelPopupBlockMessage) == 'undefined') {
    var cancelCount = 0;
    deferCancelPopupBlockMessage = function () {
        if (typeof (handlePopupBlockerError) == 'undefined' && cancelCount < 100) {
            cancelCount++;
            setTimeout('deferCancelPopupBlockMessage()', 100);
            return;
        }
        handlePopupBlockerError = function (url) { };
        // The following line was added to deal with a new caching mechanism in CRM online that causes
        // the popup window to always be /_static/loading.htm.   This breaks the routing rules engine
        // in USD.   The following line replaces the new function and always uses the standard window
        // opening procedure, which is needed for the popup URL to be consistant with the way it's 
        // been in the past.
        openStdWinWithUrlPreload = function (url, name, width, height, customWinFeatures) { return openStdWin(url, name, width, height, customWinFeatures); }
        if (cancelCount < 100)
            setTimeout('deferCancelPopupBlockMessage()', 100);
        cancelCount++;
    }
    deferCancelPopupBlockMessage();
}

function isNullOrEmpty(str) {
    return typeof (str) === 'undefined' || typeof (str) === 'unknown' || str == null || typeof str === "string" && !str.length
}

// Method to construct the context data from entityData.
function getFlowData(frameName) {
    var contentWindow = getContentWindow(frameName);

    try {
        var result = '';
        var entityData = JSON.parse(contentWindow._entityData);
        var entityMetadata = contentWindow.FormLayout['entityMetadata'];// contains metadata corresponding to each form on the page.
        var activeFormId = entityData['_formId'];

        // Get all the attributes on the active form.
        var activeFormEntityMetadata = null;
        for (i = 0; i < entityMetadata.length; ++i) {
            if (entityMetadata[i].FormId == activeFormId) {
                activeFormEntityMetadata = entityMetadata[i];
                break;
            }
        }

        if (activeFormEntityMetadata == null) {
            return result;
        }

        var attributes = activeFormEntityMetadata.Attributes;

        // Iterate over each attribute in the active form and add its type, LogicalName, value to the result.
        for (var i in attributes) {
            var attribute = attributes[i];
            var name = attribute.LogicalName;
            var type = '';
            var attributeFormat = '';
            // Adding try/catch for the attributeType and format just to ensure that we don't fail if they aren't
            // present in entityMetadata(highly unlikely but just in case).
            try { type = attribute.AttributeType; } catch (ex) { }
            try { attributeFormat = attribute.Format; } catch (ex) { }

            var value = null; // Formatted value eg. 10,000
            var rawValue = null; // Raw value eg 10000
            var attributeData = entityData[name];
            try {
                // Get the value of the attribute from entityData
                if (typeof attributeData != 'undefined') {
                    value = attributeData.value;
                    rawValue = attributeData.raw;
                }
            } catch (ex) { }

            // Get the correct type exposed by clientAPI. Eg.: Bit -> Boolean, int -> integer.
            type = GetMappedAttributeType(type, attributeFormat);
            if (value == null) {
                // If the value is null set the default value of the field. Null for every type of field except for Boolean which defaults to false.
                result += encodeURI(name) + '|' + encodeURI(type) + (type != ExposedAttributeType.Boolean ? '|null' : '|false') + '\n';
                continue;
            }
            else {
                // The value is not null. The value sent by CRM server is double Html encoded. We need to Html decode it twice to get
                // correct value. Eg.: City Power &amp;amp; Light (sample)
                value = UsdUtil.UsdEncodeDecode.HtmlDecode(value); // City Power &amp; Light (sample)
                value = UsdUtil.UsdEncodeDecode.HtmlDecode(value); // City Power & Light (sample)
                value = UsdUtil.GetDecodedValue(value); // Replace <br> with \r
            }

            switch (type) {
                case ExposedAttributeType.Boolean:
                    result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(rawValue == '1' ? true : false) + '\n';
                    break;
                case ExposedAttributeType.Decimal:
                case ExposedAttributeType.Double:
                case ExposedAttributeType.Integer:
                case ExposedAttributeType.Money:
                    result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(rawValue) + '\n';
                    break;
                case ExposedAttributeType.Memo:
                case ExposedAttributeType.String:
                    result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value) + '\n';
                    break;
                case ExposedAttributeType.OptionSet:
                    result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(rawValue + ',' + value) + '\n';
                    break;
                case ExposedAttributeType.DateTime:
                    if (!isNullOrEmpty(value)) {
                        result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value.toString()) + '\n';
                        result += encodeURI(name + "_GMT") + '|' + encodeURI(type) + '|' + encodeURI(getGMTDateString(new Date(value))) + '\n';
                    }
                    break;
                case ExposedAttributeType.Lookup:
                    var valueId = attributeData.oid;
                    // Remove leading and trailing braces from the Guid(If any).
                    if (valueId.length > 2 && valueId[0] == '{') {
                        valueId = valueId.substr(1, valueId.length - 2);
                    }
                    var data = encodeURIComponent(attributeData.otype) + ',' + encodeURIComponent(valueId) + ',' + encodeURIComponent(value) + '&';
                    result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(data) + '\n';
                    break;
            }
        }

        result += 'Id|string|' + contentWindow._id + '\n';
        result += 'LogicalName|string|' + contentWindow._etn + '\n';
        return result;
    }
    catch (ex) {
        // Something went wrong. Ignore and return ''. The calling code will fallback
        // to use ClientAPI in this case.
    }

    return '';
}

// Method to construct context data using the ClientAPI.
function getClassicData() {
    try {
        var result = '';
        if (typeof (XrmForm.Xrm) == 'undefined' || XrmForm.Xrm == null) return '';
        if (XrmForm.Xrm.Page.data != null) {
            result += "Id|string|" + XrmForm.Xrm.Page.data.entity.getId() + '\n';
            result += "LogicalName|string|" + encodeURI(XrmForm.Xrm.Page.data.entity.getEntityName()) + '\n';
            var attributes = XrmForm.Xrm.Page.data.entity.attributes.get();
            for (var i in attributes) {
                var attribute = attributes[i];
                var name = attribute.getName();
                var type = ''; try { if (typeof (attribute.getAttributeType) != 'undefined') type = attribute.getAttributeType(); } catch (ex) { }
                var value = ''; try { if (typeof (attribute.getValue) != 'undefined') value = attribute.getValue(); } catch (ex) { }
                switch (type) {
                    case ExposedAttributeType.Boolean:
                    case ExposedAttributeType.Decimal:
                    case ExposedAttributeType.Double:
                    case ExposedAttributeType.Integer:
                    case ExposedAttributeType.Money:
                        result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value) + '\n';
                        break;
                    case ExposedAttributeType.OptionSet:
                        var optionsetText = attribute.getText();
                        result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value + ',' + optionsetText) + '\n';
                        break;
                    case ExposedAttributeType.Memo:
                    case ExposedAttributeType.String:
                        if (value != null)
                            result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value) + '\n';
                        else
                            result += encodeURI(name) + '|' + encodeURI(type) + '|null' + '\n';
                        break;
                    case ExposedAttributeType.DateTime:
                        if (value != null) {
                            result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(value.toString()) + '\n';
                            result += encodeURI(name + "_GMT") + '|' + encodeURI(type) + '|' + encodeURI(getGMTDateString(value)) + '\n';
                        }
                        else
                            result += encodeURI(name) + '|' + encodeURI(type) + '|null' + '\n';
                        break;
                    case ExposedAttributeType.Lookup:
                        var data = "";
                        if (value != null) {
                            for (var j = 0; j < value.length; j++) {
                                if (value[j].id != null) {
                                    var valueid = value[j].id.toString();
                                    if (valueid.length > 2) {
                                        valueid = valueid.substr(1, valueid.length - 2);
                                        data += encodeURIComponent(value[j].entityType) + ',' + encodeURIComponent(valueid) + ',' + encodeURIComponent(value[j].name) + '&';
                                    }
                                }
                            }
                            result += encodeURI(name) + '|' + encodeURI(type) + '|' + encodeURI(data) + '\n';
                        }
                        else
                            result += encodeURI(name) + '|' + encodeURI(type) + '|null' + '\n';
                        break;
                }
            }
        }
        return result;
    }
    catch (ex)
    { }
    return '';
}

function getGMTDateString(date) {
    var formattedDate = (date.getUTCMonth() + 1) + "/";
    formattedDate += date.getUTCDate() + "/";
    formattedDate += date.getUTCFullYear();
    formattedDate += " ";
    formattedDate += date.getUTCHours() + ":";
    formattedDate += date.getUTCMinutes() + ":";
    formattedDate += date.getUTCSeconds() + " GMT";
    return formattedDate;
}

function getEntityData(frameName) {
    var contentWindow = getContentWindow(frameName);
    if (typeof (contentWindow._entityData) != 'undefined') {
        return contentWindow._entityData;
    }
    return '';
}

function getContentWindow(frameName) {
    if (frameName && frameName != '') {
        var contentIFrame = document.getElementById(frameName);
        if (contentIFrame && contentIFrame.contentWindow) {
            return contentIFrame.contentWindow;
        }
    }
    return window;
}


function ClassicUI() {
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
        PopulateData(getClassicData());
    }
    deferExecuteEvent();
}

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

function PopulateData(data, eventToFire) {
    if (USDFORMDATA == data) {
        return; // only load new data if it's actually new
    }

    USDFORMDATA = data;
    // Do extra stuff
    Hook2013Events();

    var event = 'usddataload';
    if (eventToFire) {
        event = eventToFire;
    }

    // send data to USD
    window.open('http://event.usd/?eventname=' + event);
}

var SAVE_PARAMETERS = null;
var SAVE_SOURCECOMPONENT = null;
var SAVE_INSTANCE = null;
var SAVE_EVENTPROCESSING = null;
function Hook2013Events() {
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

function GetUSDFormData() {
    return USDFORMDATA;
}

function GetDataFromClientAPI() {
    ClassicUI();
}

function GetDataFromEntityData(frameName) {
    if (getEntityData(frameName) != "") {
        var formData = getFlowData(frameName);
        if (formData) {
            PopulateData(formData, 'usdrawdataload');
            return;
        }
    }

    // If we reached over here then something went wrong while processing _entityData.
    // Use ClientAPI approach as a fallback in such cases.
    GetDataFromClientAPI();
}

function ScanForData(frameName, useEntityData) {
    if (useEntityData == true) {
        GetDataFromEntityData(frameName);
    }
    else {
        GetDataFromClientAPI();
    }
}

//[START]: Code taken from CRM(Ignore object UsdUtil)
if (typeof (UsdUtil) == 'undefined') {
    UsdUtil = {};

    // Code similar to GetDecodedValue in \src\Core\Application\web\ScriptSharp\_Common\Scripts\InlineEdit\InlineEditUtilities.cs
    UsdUtil.GetDecodedValue = function (s) {
        if (IsNull(s)) { return s; } else { if (typeof (s) != "string") { s = s.toString(); } }
        return s.split("<br>").join('\r');
    },

    // The below code is taken from \src\Core\Application\web\ScriptSharp\_Common\Scripts\Global\ScriptTemplate\EncodeDecode.js
	UsdUtil.UsdEncodeDecode =
		{
		    // Decodes a complete string or only the specified character
		    // Params: s - string to decode
		    // charToDecode - character that needs to be decoded, if null or not present all chars are decoded.
		    HtmlDecode: function (s, charToDecode) {
		        if (IsNull(s)) { return s; } else { if (typeof (s) != "string") { s = s.toString(); } }

		        if (typeof (charToDecode) != "undefined" && charToDecode != null) {
		            //only one char needs to be decoded
		            if (charToDecode.length > 1) charToDecode = charToDecode.charAt(0);
		            var sEncodedChar = HtmlEncode(charToDecode);
		            var rex = new RegExp(sEncodedChar, "g");
		            s = s.replace(rex, charToDecode);

		            //additionaly decode the forms &lt;&gt;&amp;&quot;&apos;
		            switch (charToDecode) {
		                case "<":
		                    s = s.replace(/&lt;/g, "<");
		                    break;
		                case ">":
		                    s = s.replace(/&gt;/g, ">");
		                    break;
		                case "&":
		                    s = s.replace(/&amp;/g, "&");
		                    break;
		                case "\"":
		                    s = s.replace(/&quot;/g, "\"");
		                    break;
		                case "'":
		                    s = s.replace(/&apos;/g, "'");
		                    break;
		            }
		            return s;
		        }

		        // Decode all encoded chars
		        s = s.replace(/&[^;]+;/g, function ($1) {
		            // Decode special encoding sequences
		            switch ($1) {
		                case "&lt;":
		                    return "<";
		                case "&gt;":
		                    return ">";
		                case "&amp;":
		                    return "&";
		                case "&quot;":
		                    return "\"";
		                case "&apos;":
		                    return "'";
		            }

		            // decode numeric encoded sequences
		            // decimal encoded
		            if ($1.match(/&#[0-9]+;/g)) {
		                return _crmCharCodeToChar($1.substr(2, $1.length - 3));
		            }

		            // hex encoded
		            if ($1.match(/&#x[a-fA-F0-9]+;/g)) {
		                return _crmCharCodeToChar(parseInt($1.substr(3, $1.length - 4), 16));
		            }

		            return $1;
		        });

		        return s;
		    },

		    HtmlEncode: function (strInput) {
		        var c;
		        var _HtmlEncode = '';
		        var buffer = '';
		        var bufferLength = 500;
		        var count = 0;

		        if (strInput == null) {
		            return null;
		        }
		        if (strInput == '') {
		            return '';
		        }
		        // 4330 - buffer the concatenated string in chunks of 500 and then add it to the larger string.
		        for (var cnt = 0; cnt < strInput.length; cnt++) {
		            c = strInput.charCodeAt(cnt);

		            if (((c > 96) && (c < 123)) ||
						((c > 64) && (c < 91)) ||
						(c == 32) ||
						((c > 47) && (c < 58)) ||
						(c == 46) ||
						(c == 44) ||
						(c == 45) ||
						(c == 95)) {
		                buffer += String.fromCharCode(c);
		            }
		            else {
		                buffer += '&#' + c + ';';
		            }

		            if (++count == bufferLength) {
		                _HtmlEncode += buffer;
		                buffer = '';
		                count = 0;
		            }
		        }

		        if (buffer.length) {
		            _HtmlEncode += buffer;
		        }

		        return _HtmlEncode;
		    },

		    // Converts char code into char.
		    _crmCharCodeToChar: function (charCode) {
		        if (charCode > 0xFFFF && charCode < 0x110000) {
		            charCode -= 0x10000;
		            return String.fromCharCode(0xD800 + (charCode >> 10), 0xDC00 + (charCode & 0x3FF));
		        }
		        else {
		            return String.fromCharCode(charCode);
		        }
		    }
		};
}
//[END]: Code taken from CRM