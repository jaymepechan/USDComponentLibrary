using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C1.WPF.Docking;

namespace Microsoft.USD.ComponentLibrary.ComponentOne
{
    public class C1UsdDockControl:C1DockControl
    {
        protected override void AddChild(object value)
        {
            base.AddChild(value);
        }

        protected override C1DockTabControl CreateDockTabControlOverride()
        {
            return new C1USDTabBasePanel();
        } 
    }
}
