using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Socket.IO.NET35
{
    public interface IDecodePayloadCallback
    {
        bool Call(EnginePacket packet, int index, int total);
    }
}
