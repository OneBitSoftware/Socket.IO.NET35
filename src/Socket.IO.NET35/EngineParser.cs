using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public class EngineParser
    {

        public static readonly int Protocol = 3;


        private EngineParser()
        {
        }

        public static void EncodePacket(EnginePacket packet, IEncodeCallback callback)
        {
            packet.Encode(callback);
        }

        public static EnginePacket DecodePacket(string data, bool utf8decode = false)
        {
            return EnginePacket.DecodePacket(data, utf8decode);
        }

        public static EnginePacket DecodePacket(byte[] data)
        {
            return EnginePacket.DecodePacket(data);
        }

        public static void EncodePayload(EnginePacket[] packets, IEncodeCallback callback)
        {
            EnginePacket.EncodePayload(packets, callback);
        }


        public static void DecodePayload(string data, IDecodePayloadCallback callback)
        {
            EnginePacket.DecodePayload(data, callback);
        }

        public static void DecodePayload(byte[] data, IDecodePayloadCallback callback)
        {
            EnginePacket.DecodePayload(data, callback);
        }

    }
}
