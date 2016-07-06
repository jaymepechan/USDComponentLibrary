using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C1.WPF.Docking;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    public class C1USDDockTabItem : C1DockTabItem
    {
        public Guid SessionId { get; set; }
        public string DisplayGroup { get; set; }
        public string ApplicationName { get; set; }
    }
}
