namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class HeightMapComposer : ServerPacket
    {
        public HeightMapComposer()
            : base(ServerPacketHeader.HeightMapMessageComposer)
        {

        }
    }
}
