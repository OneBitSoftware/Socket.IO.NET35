using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class SocketException : Exception
    {
        public string Transport;
        public object code;

        public SocketException(string message)
            : base(message)
        {
        }


        public SocketException(Exception cause)
            : base("", cause)
        {
        }

        public SocketException(string message, Exception cause)
            : base(message, cause)
        {
        }
    }
}
