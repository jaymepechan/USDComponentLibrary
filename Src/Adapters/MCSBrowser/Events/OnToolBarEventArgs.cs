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
    public sealed class OnToolBarEventArgs : EventArgs
    {
        // Fields
        internal bool _ToolBar;

        // Properties
        public bool ToolBar
        {
            get
            {
                return this._ToolBar;
            }
        }
    }
}