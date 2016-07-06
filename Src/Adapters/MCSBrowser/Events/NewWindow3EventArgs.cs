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
    public sealed class NewWindow3EventArgs : EventArgs
    {
        // Fields
        internal string _bstrUrl;
        internal string _bstrUrlContext;
        private bool _Cancel;
        internal uint _dwFlags;
        private object _ppDisp;

        // Properties
        public string bstrUrl
        {
            get
            {
                return this._bstrUrl;
            }
        }

        public string bstrUrlContext
        {
            get
            {
                return this._bstrUrlContext;
            }
        }

        public bool Cancel
        {
            get
            {
                return this._Cancel;
            }
            set
            {
                this._Cancel = value;
            }
        }

        public uint dwFlags
        {
            get
            {
                return this._dwFlags;
            }
        }

        public object ppDisp
        {
            get
            {
                return this._ppDisp;
            }
            set
            {
                this._ppDisp = value;
            }
        }
    }
}