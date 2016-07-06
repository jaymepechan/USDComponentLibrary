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
    public sealed class UpdatePageStatusEventArgs : EventArgs
    {
        // Fields
        private object _fDone;
        private object _nPage;
        internal object _pDisp;

        // Properties
        public object fDone
        {
            get
            {
                return this._fDone;
            }
            set
            {
                this._fDone = value;
            }
        }

        public object nPage
        {
            get
            {
                return this._nPage;
            }
            set
            {
                this._nPage = value;
            }
        }

        public object pDisp
        {
            get
            {
                return this._pDisp;
            }
        }
    }
}