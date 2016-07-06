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
    public sealed class PrivacyImpactedStateChangeEventArgs : EventArgs
    {
        // Fields
        internal bool _bImpacted;

        // Properties
        public bool bImpacted
        {
            get
            {
                return this._bImpacted;
            }
        }
    }
}