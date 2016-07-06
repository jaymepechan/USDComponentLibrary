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
    public sealed class PrintTemplateTeardownEventArgs : EventArgs
    {
        // Fields
        internal object _pDisp;

        // Properties
        public object pDisp
        {
            get
            {
                return this._pDisp;
            }
        }
    }
}