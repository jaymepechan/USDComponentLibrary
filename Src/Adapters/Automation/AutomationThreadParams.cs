using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public class AutomationThreadParams
    {
        // Methods
        public AutomationThreadParams()
        {
            this.throwExceptionIfNotFound = true;
        }

        // Properties
        public string ControlName { get; set; }

        public string ControlValue { get; set; }

        public string Data { get; set; }

        public AutomationElement Element { get; set; }

        public Exception ex { get; set; }

        public bool throwExceptionIfNotFound { get; set; }

        public ManualResetEvent waitHandle { get; set; }
    }
}
