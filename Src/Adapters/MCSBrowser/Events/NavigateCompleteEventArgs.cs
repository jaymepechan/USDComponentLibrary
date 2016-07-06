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
    public sealed class NavigateCompleteEventArgs : EventArgs
    {
        // Fields
        internal object _URL;

        // Properties
        public object URL
        {
            get
            {
                return this._URL;
            }
        }
    }
}