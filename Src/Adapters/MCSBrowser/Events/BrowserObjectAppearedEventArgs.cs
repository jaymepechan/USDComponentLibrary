/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SHDocVw;
using Microsoft.Uii.Csr.Browser.Web;

namespace Microsoft.USD.ComponentLibrary
{
    public sealed class BrowserObjectAppearedEventArgs : EventArgs
    {
        // Fields
        internal IWebBrowser2 _IE;
        internal bool _IsPopup;
        internal int _pId;

        // Properties
        public IWebBrowser2 IE
        {
            get
            {
                return this._IE;
            }
        }

        public bool IsPopup
        {
            get
            {
                return this._IsPopup;
            }
        }

        public int pId
        {
            get
            {
                return this._pId;
            }
        }
    }
}