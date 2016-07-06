/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Runtime;

namespace Microsoft.USD.ComponentLibrary
{
    [HostProtection(SecurityAction.LinkDemand, SharedState=true)]
    public class CancelEventArgs : EventArgs
    {
        // Fields
        private bool cancel;

        // Methods
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CancelEventArgs() : this(false)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public CancelEventArgs(bool cancel)
        {
            this.cancel = cancel;
        }

        // Properties
        public bool Cancel
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.cancel;
            }
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            set
            {
                this.cancel = value;
            }
        }
    }
}
