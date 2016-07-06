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
    public sealed class WindowClosingEventArgs : EventArgs
    {
        // Fields
        private bool _Cancel;
        internal bool _IsChildWindow;

        // Properties
        public bool Cancel
        {
            get
            {
                return this._Cancel;
            }
            set
            {
                this._Cancel = value;
            }
        }

        public bool IsChildWindow
        {
            get
            {
                return this._IsChildWindow;
            }
        }
    }
}