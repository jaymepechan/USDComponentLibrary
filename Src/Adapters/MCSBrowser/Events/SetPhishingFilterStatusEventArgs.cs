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
    public sealed class SetPhishingFilterStatusEventArgs : EventArgs
    {
        // Fields
        internal int _PhishingFilterStatus;

        // Properties
        public int PhishingFilterStatus
        {
            get
            {
                return this._PhishingFilterStatus;
            }
        }
    }
}