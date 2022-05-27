namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FloodControlComposer : ServerPacket
    {
        public FloodControlComposer(int floodTime)
            : base(ServerPacketHeader.FloodControlMessageComposer)
        {
            WriteInteger(floodTime);
        }
    }
}
