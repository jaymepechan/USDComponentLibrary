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
    public class BrowserExtendedNewWindowEventArgs : CancelEventArgs
    {
        // Fields
        private object _Flags;
        private object _pDisp;
        private string _Url;
        internal string _UrlContext;

        // Properties
        public object Flags
        {
            get
            {
                return this._Flags;
            }
            set
            {
                this._Flags = value;
            }
        }

        public object pDisp
        {
            get
            {
                return this._pDisp;
            }
            set
            {
                this._pDisp = value;
            }
        }

        public string Url
        {
            get
            {
                return this._Url;
            }
            set
            {
                this._Url = value;
            }
        }

        public string UrlContext
        {
            get
            {
                return this._UrlContext;
            }
        }
    }
}