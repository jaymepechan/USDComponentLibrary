/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary
{
    public abstract class MicrosoftBase : DynamicsBaseHostedControl
    {
        public delegate void ActionHandler (Uii.Csr.RequestActionEventArgs args);
        Dictionary<string, ActionHandler> registeredActions = new Dictionary<string, ActionHandler>();

        public MicrosoftBase()
        {

        }

        public MicrosoftBase(Guid appID, string appName, string initString)
            : base(appID, appName, initString)
        {

        }

        protected void RegisterAction(string name, ActionHandler handler)
        {
            string lowername = name.ToLower();
            lock (registeredActions)
            {
                if (registeredActions.ContainsKey(lowername))
                    registeredActions.Remove(lowername);
                registeredActions.Add(lowername, handler);
            }
        }

        protected override void DoAction(Uii.Csr.RequestActionEventArgs args)
        {
            string action = args.Action.ToLower();
            lock (registeredActions)
            { 
                if (registeredActions.ContainsKey(action))
                {
                    registeredActions[action].Invoke(args);
                    return;
                }
            }

            base.DoAction(args);
        }
    }
}
