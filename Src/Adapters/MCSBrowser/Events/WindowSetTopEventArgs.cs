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
    public sealed class WindowSetTopEventArgs : EventArgs
    {
        // Fields
        internal int _Top;

        // Properties
        public int Top
        {
            get
            {
                return this._Top;
            }
        }
    }
}