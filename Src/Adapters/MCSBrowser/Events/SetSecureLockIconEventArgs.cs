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
    public sealed class SetSecureLockIconEventArgs : EventArgs
    {
        // Fields
        internal int _SecureLockIcon;

        // Properties
        public int SecureLockIcon
        {
            get
            {
                return this._SecureLockIcon;
            }
        }
    }
}