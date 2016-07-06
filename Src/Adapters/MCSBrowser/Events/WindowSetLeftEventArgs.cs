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
    public sealed class WindowSetLeftEventArgs : EventArgs
    {
        // Fields
        internal int _Left;

        // Properties
        public int Left
        {
            get
            {
                return this._Left;
            }
        }
    }
}