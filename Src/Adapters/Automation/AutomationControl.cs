using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Uii.Desktop.Cti.Core;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    public class AutomationControl : IUSDAutomation
    {
        public static int GetAttributeValue(XmlNode node, string attributeName, int defaultValue)
        {
            int num = defaultValue;
            if ((node != null) && (node.Attributes != null))
            {
                int num2;
                XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
                if ((namedItem != null) && int.TryParse(namedItem.Value, out num2))
                {
                    num = num2;
                }
            }
            return num;
        }

        public static string GetAttributeValue(XmlNode node, string attributeName, string defaultValue)
        {
            string str = defaultValue;
            if ((node != null) && (node.Attributes != null))
            {
                XmlNode namedItem = node.Attributes.GetNamedItem(attributeName);
                if (namedItem != null)
                {
                    str = namedItem.Value;
                }
            }
            return str;
        }

        public virtual void Initialize()
        {
            throw new NotImplementedException();
        }

        public virtual void Execute(IUSDAutomationObject automationObject, string action)
        {
            throw new NotImplementedException();
        }

        public virtual IUSDAutomationObject GetAutomationObject(XmlNode control)
        {
            throw new NotImplementedException();
        }

        public virtual LookupRequestItem GetValue(IUSDAutomationObject automationObject)
        {
            throw new NotImplementedException();
        }

        public virtual void SetValue(IUSDAutomationObject automationObject, string value)
        {
            throw new NotImplementedException();
        }

        public virtual void DisplayVisualTree()
        {
            throw new NotImplementedException();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~AutomationControl() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
