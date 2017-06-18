using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class Options
    {
        public bool Agent = false;
        public bool ForceBase64 = false;
        public bool ForceJsonp = false;
        public string Hostname;
        public string Path;
        public string TimestampParam;
        public bool Secure = false;
        public bool TimestampRequests = true;
        public int Port;
        public int PolicyPort;
        public Dictionary<string, string> Query;
        public bool IgnoreServerCertificateValidation = false;
        //internal Socket Socket;
        public Dictionary<string, string> Cookies = new Dictionary<string, string>();

        public string GetCookiesAsString()
        {
            var result = new StringBuilder();
            var first = true;
            foreach (var item in Cookies)
            {
                if (!first)
                {
                    result.Append("; ");
                }
                result.Append(string.Format("{0}={1}", item.Key, item.Value));
                first = false;
            }
            return result.ToString();
        }
    }
}
