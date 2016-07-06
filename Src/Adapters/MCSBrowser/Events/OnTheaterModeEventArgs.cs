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
    public sealed class OnTheaterModeEventArgs : EventArgs
    {
        // Fields
        internal bool _TheaterMode;

        // Properties
        public bool TheaterMode
        {
            get
            {
                return this._TheaterMode;
            }
        }
    }
}