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
    public sealed class FileDownloadEventArgs : EventArgs
    {
        // Fields
        internal bool _ActiveDocument;
        private bool _Cancel;

        // Properties
        public bool ActiveDocument
        {
            get
            {
                return this._ActiveDocument;
            }
        }

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
    }
}