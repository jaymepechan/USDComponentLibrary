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
    public sealed class CommandStateChangeEventArgs : EventArgs
    {
        // Fields
        internal int _Command;
        internal bool _Enable;

        // Properties
        public int Command
        {
            get
            {
                return this._Command;
            }
        }

        public bool Enable
        {
            get
            {
                return this._Enable;
            }
        }
    }
}