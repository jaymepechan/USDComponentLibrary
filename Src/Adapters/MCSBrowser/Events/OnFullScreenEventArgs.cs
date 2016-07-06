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
    public sealed class OnFullScreenEventArgs : EventArgs
    {
        // Fields
        internal bool _FullScreen;

        // Properties
        public bool FullScreen
        {
            get
            {
                return this._FullScreen;
            }
        }
    }
}