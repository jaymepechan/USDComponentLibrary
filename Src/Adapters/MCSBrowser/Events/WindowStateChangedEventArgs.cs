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
    public sealed class WindowStateChangedEventArgs : EventArgs
    {
        // Fields
        internal uint _dwValidFlagsMask;
        internal uint _dwWindowStateFlags;

        // Properties
        public uint dwValidFlagsMask
        {
            get
            {
                return this._dwValidFlagsMask;
            }
        }

        public uint dwWindowStateFlags
        {
            get
            {
                return this._dwWindowStateFlags;
            }
        }
    }
}