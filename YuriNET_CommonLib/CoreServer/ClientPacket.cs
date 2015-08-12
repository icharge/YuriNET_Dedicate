using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuriNET.Common.CoreServer;

namespace YuriNET.Central.CoreServer {

    public class ClientPacket : Packet {

        private String fromUser;
        private String toUser;
        private PacketType packetType;
        private String message;


        public ClientPacket(byte[] byteData)
            : base(byteData) {

        }

    }
}
