using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class ParseQueryString
    {
        /// <summary>
        /// Compiles a querystring
        /// Returns string representation of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Encode(ConcurrentDictionary<string, string> obj)
        {
            var sb = new StringBuilder();
            foreach (var key in obj.Keys.OrderBy(x => x))
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(GlobalHelper.EncodeURIComponent(key));
                sb.Append("=");
                sb.Append(GlobalHelper.EncodeURIComponent(obj[key]));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Compiles a querystring
        /// Returns string representation of the object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static string Encode(System.Collections.Generic.Dictionary<string, string> obj)
        {
            var sb = new StringBuilder();
            foreach (var key in obj.Keys)
            {
                if (sb.Length > 0)
                {
                    sb.Append("&");
                }
                sb.Append(GlobalHelper.EncodeURIComponent(key));
                sb.Append("=");
                sb.Append(GlobalHelper.EncodeURIComponent(obj[key]));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Parses a simple querystring into an object
        /// </summary>
        /// <param name="qs"></param>
        /// <returns></returns>
        public static Dictionary<string, string> Decode(string qs)
        {
            var qry = new Dictionary<string, string>();
            var pairs = qs.Split('&');
            for (int i = 0; i < pairs.Length; i++)
            {
                var pair = pairs[i].Split('=');

                qry.Add(GlobalHelper.DecodeURIComponent(pair[0]), GlobalHelper.DecodeURIComponent(pair[1]));
            }
            return qry;
        }


    }
}
