using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Socket.IO.NET35
{
    public static class GlobalHelper
    {
        public static string EncodeURIComponent(string str)
        {
            //http://stackoverflow.com/a/4550600/1109316
            return Uri.EscapeDataString(str);
        }

        public static string DecodeURIComponent(string str)
        {
            return Uri.UnescapeDataString(str);
        }

        public static string CallerName(string caller = "", int number = 0, string path = "")
        {
            var fileName = "";
            return string.Format("{0}-{1}:{2}#{3}", path, fileName, caller, number);
        }

        //from http://stackoverflow.com/questions/8767103/how-to-remove-invalid-code-points-from-a-string
        public static string StripInvalidUnicodeCharacters(string str)
        {
            var invalidCharactersRegex = new Regex("([\ud800-\udbff](?![\udc00-\udfff]))|((?<![\ud800-\udbff])[\udc00-\udfff])");
            return invalidCharactersRegex.Replace(str, "");
        }
    }
}
