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
    public sealed class PropertyChangeEventArgs : EventArgs
    {
        // Fields
        internal string _szProperty;

        // Properties
        public string szProperty
        {
            get
            {
                return this._szProperty;
            }
        }
    }
}