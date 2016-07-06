/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary
{
    public sealed class BeforeNavigate2EventArgs : EventArgs
    {
        private bool _Cancel;
        private object _Flags;
        private object _Headers;
        private object _pDisp;
        private object _PostData;
        private object _TargetFrameName;
        private object _URL;

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

        public object Flags
        {
            get
            {
                return this._Flags;
            }
            set
            {
                this._Flags = value;
            }
        }

        public object Headers
        {
            get
            {
                return this._Headers;
            }
            set
            {
                this._Headers = value;
            }
        }

        public object pDisp
        {
            get
            {
                return this._pDisp;
            }
            set
            {
                this._pDisp = value;
            }
        }

        public object PostData
        {
            get
            {
                return this._PostData;
            }
            set
            {
                this._PostData = value;
            }
        }

        public object TargetFrameName
        {
            get
            {
                return this._TargetFrameName;
            }
            set
            {
                this._TargetFrameName = value;
            }
        }

        public object URL
        {
            get
            {
                return this._URL;
            }
            set
            {
                this._URL = value;
            }
        }
    }
}
