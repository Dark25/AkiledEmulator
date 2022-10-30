namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class LatencyResponseMessageComposer : ServerPacket
    {
        public LatencyResponseMessageComposer()
            : base(ServerPacketHeader.LatencyResponseMessageComposer)
        {

        }
    }
}
