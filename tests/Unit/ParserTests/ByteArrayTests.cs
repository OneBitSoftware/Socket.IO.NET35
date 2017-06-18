using Newtonsoft.Json.Linq;
using Socket.IO.NET35;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Unit.ParserTests
{
    public class ByteArrayTests : BaseParserTests
    {
        [Fact]
        public void EncodeByteArray()
        {
            var packet = new Packet(Parser.BINARY_EVENT)
            {
                Id = 23,
                Nsp = "/woot",
                Data = System.Text.Encoding.UTF8.GetBytes("abc")
            };
            TestBin(packet);
        }

        [Fact]
        public void EncodeByteArray2()
        {
            var packet = new Packet(Parser.BINARY_EVENT)
            {
                Id = 0,
                Nsp = "/",
                Data = new byte[2]
            };
            TestBin(packet);
        }

        [Fact]
        public void EncodeByteArrayInJson()
        {
            var exptected = System.Text.Encoding.UTF8.GetBytes("asdfasdf");
            var _args = new List<object> { "buffa" };
            _args.Add(exptected);

            var data = Packet.Args2JArray(_args);

            var packet = new Packet()
            {
                Type = Parser.BINARY_EVENT,
                Id = 999,
                Nsp = "/deep",
                Data = data
            };
            TestBin(packet);
        }

        [Fact]
        public void EncodeByteArrayDeepInJson()
        {
            var buf = System.Text.Encoding.UTF8.GetBytes("howdy");
            var jobj = new JObject();
            jobj.Add("hello", "lol");
            jobj.Add("message", buf);
            jobj.Add("goodbye", "gotcha");

            var _args = new List<object> { "jsonbuff" };
            _args.Add(jobj);

            var data = Packet.Args2JArray(_args);

            var packet = new Packet()
            {
                Type = Parser.BINARY_EVENT,
                Id = 999,
                Nsp = "/deep",
                Data = data
            };
            TestBin(packet);
        }
    }
}
