namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class IgnoreStatusMessageComposer : ServerPacket
    {
        public IgnoreStatusMessageComposer()
            : base(ServerPacketHeader.IgnoreStatusMessageComposer)
        {

        }
    }
}
