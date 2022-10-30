namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BuddyListMessageComposer : ServerPacket
    {
        public BuddyListMessageComposer()
            : base(ServerPacketHeader.BuddyListMessageComposer)
        {

        }
    }
}
