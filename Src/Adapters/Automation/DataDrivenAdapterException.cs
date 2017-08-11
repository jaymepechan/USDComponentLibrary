using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Adapters.Automation
{
    [Serializable]
    public class DataDrivenAdapterException : Exception
    {
        // Methods
        public DataDrivenAdapterException()
        {
        }

        public DataDrivenAdapterException(string message) : base(message)
        {
        }

        protected DataDrivenAdapterException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public DataDrivenAdapterException(string message, Exception inner) : base(message, inner)
        {
        }

        public DataDrivenAdapterException(OperationType op, string controlName, Exception inner) : base(BuildExceptionMsg(op, controlName, inner), inner)
        {
        }

        public DataDrivenAdapterException(OperationType op, string controlName, string message) : base(BuildExceptionMsg(op, controlName, message))
        {
        }

        private static string BuildExceptionMsg(OperationType op, string controlName, Exception inner) =>
            BuildExceptionMsg(op, controlName, inner.Message);

        private static string BuildExceptionMsg(OperationType op, string controlName, string message)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("DataDrivenAdapterException (op=");
            builder.Append(Enum.GetName(typeof(OperationType), op));
            if ((controlName != null) && (controlName.Length > 0))
            {
                builder.AppendFormat(",controlName={0}", controlName);
            }
            builder.Append(")");
            if (message != null)
            {
                builder.AppendFormat(": {0}", message);
            }
            return builder.ToString();
        }
    }

}
