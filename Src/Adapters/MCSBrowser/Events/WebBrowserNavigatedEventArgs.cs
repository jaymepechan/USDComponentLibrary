/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime;

namespace Microsoft.USD.ComponentLibrary
{
    public class WebBrowserNavigatedEventArgs : EventArgs
    {
        // Fields
        private Uri url;

        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public WebBrowserNavigatedEventArgs(Uri url)
        {
            this.url = url;
        }

        // Properties
        public Uri Url
        {
            get
            {
                //WebBrowser.EnsureUrlConnectPermission(this.url);
                return this.url;
            }
        }
    }
}