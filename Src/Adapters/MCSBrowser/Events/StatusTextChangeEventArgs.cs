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
    public sealed class StatusTextChangeEventArgs : EventArgs
    {
        // Fields
        internal string _Text;

        // Properties
        public string Text
        {
            get
            {
                return this._Text;
            }
        }
    }
}