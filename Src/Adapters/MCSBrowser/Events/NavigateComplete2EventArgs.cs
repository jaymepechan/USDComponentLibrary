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
    public sealed class NavigateComplete2EventArgs : EventArgs
    {
        // Fields
        internal object _pDisp;
        private object _URL;

        // Properties
        public object pDisp
        {
            get
            {
                return this._pDisp;
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