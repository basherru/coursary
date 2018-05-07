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
            public static int ICMP_ECHO_REQUEST = 1;
            public static int ICMP_ECHO_REPLY = 2;
        }

        public static class ARP {
            public static int ARP_REQUEST = 3;
            public static int ARP_REPLY = 4;
        }
    }
}
