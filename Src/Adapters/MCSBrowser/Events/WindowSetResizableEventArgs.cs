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
    public sealed class WindowSetResizableEventArgs : EventArgs
    {
        // Fields
        internal bool _Resizable;

        // Properties
        public bool Resizable
        {
            get
            {
                return this._Resizable;
            }
        }
    }
}