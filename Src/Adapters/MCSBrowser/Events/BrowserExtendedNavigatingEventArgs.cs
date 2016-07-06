/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Uii.Csr.Browser.Web;

namespace Microsoft.USD.ComponentLibrary
{
    public class BrowserExtendedNavigatingEventArgs : CancelEventArgs
    {
        // Fields
        private object _Flags;
        internal string _Frame;
        private object _Headers;
        internal UrlContext _NavigationContext;
        private object _pDisp;
        private object _Postdata;
        private string _Url;

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

        public string Frame
        {
            get
            {
                return this._Frame;
            }
        }

        public object Headers
        {
            get
            {
                return this._Headers;
            }
            set
            {
                this._Headers = value;
            }
        }

        public UrlContext NavigationContext
        {
            get
            {
                return this._NavigationContext;
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

        public object Postdata
        {
            get
            {
                return this._Postdata;
            }
            set
            {
                this._Postdata = value;
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
    }
}
