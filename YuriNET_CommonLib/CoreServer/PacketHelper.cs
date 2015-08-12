namespace YuriNET.Common.CoreServer {

    public static class PacketHelper {

        public static int ToInt(this Packet.PacketType pt) {
            return (int) pt;
        }
    }
}