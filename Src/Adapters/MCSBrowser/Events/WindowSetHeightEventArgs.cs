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
    public sealed class WindowSetHeightEventArgs : EventArgs
    {
        // Fields
        internal int _Height;

        // Properties
        public int Height
        {
            get
            {
                return this._Height;
            }
        }
    }
}