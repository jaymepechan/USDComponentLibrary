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
    public sealed class ProgressChangeEventArgs : EventArgs
    {
        // Fields
        internal int _Progress;
        internal int _ProgressMax;

        // Properties
        public int Progress
        {
            get
            {
                return this._Progress;
            }
        }

        public int ProgressMax
        {
            get
            {
                return this._ProgressMax;
            }
        }
    }
}