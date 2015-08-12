using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YuriNET.Common.CoreServer;
using YuriNET.Common.Utils;

namespace YuriNET.Central.CoreServer {

    public class ClientPacket : Packet {

        private String fromUser;
        private String toUser;
        private PacketType packetType;
        private String message;


        public ClientPacket(byte[] byteData)
            : base(byteData) {
            if (null == byteData) {
                packetType = PacketType.Null;
                return;
            }
            if (byteData.Length == 0) {
                packetType = PacketType.Null;
                return;
            }
            String[] strData = Encoding.UTF8.GetString(byteData).Split('\0');
            if (strData.Length < 4) {
                packetType = PacketType.Null;
                return;
            }

            this.fromUser = strData[0];
            this.toUser = strData[1];
            this.packetType = (PacketType) ConvertUtil.ToInt(strData[2], 0);
            this.message = strData[3];
        }

        public static implicit operator bool(ClientPacket clientPacket) {
            return clientPacket.packetType != PacketType.Null;
        }

        public override string ToString() {
            return base.ToString();
        }

    }
}
