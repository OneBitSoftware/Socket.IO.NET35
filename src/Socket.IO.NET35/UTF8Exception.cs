using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class UTF8Exception : Exception
    {
        public UTF8Exception(string message) : base(message)
        { }
    }
}
