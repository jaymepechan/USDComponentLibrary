using Microsoft.Uii.Desktop.Cti.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public interface IUSDAutomation : IDisposable
    {
        void Initialize();
        IUSDAutomationObject GetAutomationObject(XmlNode control);
        LookupRequestItem GetValue(IUSDAutomationObject automationObject);
        void SetValue(IUSDAutomationObject automationObject, string value);
        void Execute(IUSDAutomationObject automationObject, string action);
        void DisplayVisualTree();
    }

    public interface IUSDAutomationObject : IDisposable
    {

    }
}
