namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AvailabilityStatusComposer : ServerPacket
    {
        public AvailabilityStatusComposer()
            : base(ServerPacketHeader.AvailabilityStatusMessageComposer)
        {
            WriteBoolean(true);
            WriteBoolean(false);
            WriteBoolean(true);
        }
    }
}
