using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Socket.IO.NET35
{
    public class ServerCertificate
    {
        public static bool Ignore { get; set; }

        static ServerCertificate()
        {
            Ignore = false;
        }

        public static void IgnoreServerCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            Ignore = true;
        }
    }
}
