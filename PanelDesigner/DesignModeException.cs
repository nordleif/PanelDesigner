using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace PanelDesigner
{
    public class DesignModeException : Exception
    {
        public DesignModeException()
        {

        }

        public DesignModeException(string message)
            : base(message)
        {

        }

        public DesignModeException(string message, Exception innerException)
            : base(message, innerException)
        {


        }

        public DesignModeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
 
        }
    }
}
