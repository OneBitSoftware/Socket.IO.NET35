using Newtonsoft.Json;
using Socket.IO.NET35;
using Socket.IO.NET35.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Integration
{
    public abstract class BaseIntegrationTest
    {
        internal readonly static string ServerUrl = "http://localhost:3000/";

        public IO.Options CreateOptions()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());


            var config = ConfigBase.Load();
            var options = new IO.Options();
            //options.Port = config.server.port;
            //options.Hostname = config.server.hostname;
            options.ForceNew = true;
            log.Info("Please add to your hosts file: 127.0.0.1 " + options.Hostname);

            return options;
        }

        public string CreateUri()
        {
            //var options = CreateOptions();
            //var uri = string.Format("{0}://{1}:{2}", options.Secure ? "https" : "http", options.Hostname, options.Port);
            //return uri;

            return ServerUrl;
        }


        public IO.Options CreateOptionsSecure()
        {
            var log = LogManager.GetLogger(GlobalHelper.CallerName());

            var config = ConfigBase.Load();
            var options = new IO.Options();
            //options.Port = config.server.ssl_port;
            //options.Hostname = config.server.hostname;
            log.Info("Please add to your hosts file: 127.0.0.1 " + options.Hostname);
            options.Secure = true;
            options.IgnoreServerCertificateValidation = true;
            return options;
        }
    }

    public class ConfigBase
    {
        public string version { get; set; }
        public ConfigServer server { get; set; }

        public static ConfigBase Load()
        {
            string configString = "{'version':'1.0'}";
            try
            {
                if(File.Exists("config.json"))
                    configString = File.ReadAllText("config.json");
            }
            catch (FileNotFoundException)
            {
            }
            var config = JsonConvert.DeserializeObject<ConfigBase>(configString);
            return config;
        }
    }

    public class ConfigServer
    {
        public string hostname { get; set; }
        public int port { get; set; }
        public int ssl_port { get; set; }
    }
}
