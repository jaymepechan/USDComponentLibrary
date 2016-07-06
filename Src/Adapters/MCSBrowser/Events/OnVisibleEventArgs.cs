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
    public sealed class OnVisibleEventArgs : EventArgs
    {
        // Fields
        internal bool _Visible;

        // Properties
        public bool Visible
        {
            get
            {
                return this._Visible;
            }
        }
    }
}