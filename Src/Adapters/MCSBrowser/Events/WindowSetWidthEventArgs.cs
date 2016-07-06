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
    public sealed class WindowSetWidthEventArgs : EventArgs
    {
        // Fields
        internal int _Width;

        // Properties
        public int Width
        {
            get
            {
                return this._Width;
            }
        }
    }
}