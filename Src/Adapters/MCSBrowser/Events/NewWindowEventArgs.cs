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
    public sealed class NewWindowEventArgs : EventArgs
    {
        // Fields
        internal object _Flags;
        internal object _Headers;
        private object _PostData;
        private bool _Processed;
        internal object _TargetFrameName;
        internal object _URL;

        // Properties
        public object Flags
        {
            get
            {
                return this._Flags;
            }
        }

        public object Headers
        {
            get
            {
                return this._Headers;
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

        public bool Processed
        {
            get
            {
                return this._Processed;
            }
            set
            {
                this._Processed = value;
            }
        }

        public object TargetFrameName
        {
            get
            {
                return this._TargetFrameName;
            }
        }

        public object URL
        {
            get
            {
                return this._URL;
            }
        }
    }
}