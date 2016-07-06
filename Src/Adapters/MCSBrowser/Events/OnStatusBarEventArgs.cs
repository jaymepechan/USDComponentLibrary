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
    public sealed class OnStatusBarEventArgs : EventArgs
    {
        // Fields
        internal bool _StatusBar;

        // Properties
        public bool StatusBar
        {
            get
            {
                return this._StatusBar;
            }
        }
    }
}