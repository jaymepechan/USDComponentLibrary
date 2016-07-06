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
    public sealed class OnMenuBarEventArgs : EventArgs
    {
        // Fields
        internal bool _MenuBar;

        // Properties
        public bool MenuBar
        {
            get
            {
                return this._MenuBar;
            }
        }
    }
}