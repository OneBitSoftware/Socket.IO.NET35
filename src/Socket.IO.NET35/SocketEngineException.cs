using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class SocketEngineException : Exception
    {
        public string Transport;
        public object code;

        public SocketEngineException(string message)
            : base(message)
        {
        }


        public SocketEngineException(Exception cause)
            : base("", cause)
        {
        }

        public SocketEngineException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
