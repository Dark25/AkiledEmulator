namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FloorHeightMapMessageComposer : ServerPacket
    {
        public FloorHeightMapMessageComposer()
            : base(ServerPacketHeader.FloorHeightMapMessageComposer)
        {

        }
    }
}
