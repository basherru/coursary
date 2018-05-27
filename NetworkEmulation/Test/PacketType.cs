namespace NetworkPackets
{
    public class PacketType
    {

        public int Type;

        PacketType(int Type) {
            this.Type = Type;
        }

        public static class ICMP
        {
            public static string ICMP_ECHO_REQUEST = "ICMP_ECHO_REQUEST";
            public static string ICMP_ECHO_REPLY = "ICMP_ECHO_REPLY";
        }

        public static class HANDSHAKE {
            public static string SYN = "SYN";
            public static string ACK = "ACK";
        }

    }
}
