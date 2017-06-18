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
    public class ParserEncodingTests : BaseParserTests
    {
        [Fact]
        public void Decode()
        {
            var decoder = new Parser.Decoder();
            var called = false;
            decoder.On(Parser.Decoder.EVENT_DECODED, new ListenerImpl((data1) =>
            {
                called = true;

            }));
            decoder.Add("0/woot");
            Assert.True(called);
        }

        [Fact]
        public void EncodeConnection()
        {
            var packet = new Packet(Parser.CONNECT) { Nsp = "/woot" };
            Test(packet);
        }

        [Fact]
        public void EncodeDisconnection()
        {
            var packet = new Packet(Parser.DISCONNECT) { Nsp = "/woot" };
            Test(packet);
        }

        [Fact]
        public void EncodeEvent()
        {
            var packet = new Packet(Parser.EVENT) { Nsp = "/", Data = JArray.Parse("[\"a\", 1, {}]") };
            Test(packet);

            var packet2 = new Packet(Parser.EVENT) { Nsp = "/test", Data = JArray.Parse("[\"a\", 1, {}]") };
            Test(packet2);
        }

        [Fact]
        public void EncodeAck()
        {
            var packet = new Packet(Parser.ACK) { Id = 123, Nsp = "/", Data = JArray.Parse("[\"a\", 1, {}]") };
            Test(packet);
        }
    }
}
