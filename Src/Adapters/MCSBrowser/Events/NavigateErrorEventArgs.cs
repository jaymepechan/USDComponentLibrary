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
    public sealed class NavigateErrorEventArgs : EventArgs
    {
        // Fields
        private bool _Cancel;
        private object _Frame;
        internal object _pDisp;
        private object _StatusCode;
        private object _URL;

        // Properties
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

        public object Frame
        {
            get
            {
                return this._Frame;
            }
            set
            {
                this._Frame = value;
            }
        }

        public object pDisp
        {
            get
            {
                return this._pDisp;
            }
        }

        public object StatusCode
        {
            get
            {
                return this._StatusCode;
            }
            set
            {
                this._StatusCode = value;
            }
        }

        public object URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }
    }
}