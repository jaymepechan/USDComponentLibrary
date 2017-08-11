using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public class AutomationEventArgsMapping
    {
        // Methods
        public AutomationEventArgsMapping(AutomationEvent aEvent, Type argsType)
        {
            this.Event = aEvent;
            this.EventArgsType = argsType;
        }

        // Properties
        public AutomationEvent Event { get; set; }

        public Type EventArgsType { get; set; }
    }
}
