namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NewBuddyRequestMessageComposer : ServerPacket
    {
        public NewBuddyRequestMessageComposer()
            : base(ServerPacketHeader.NewBuddyRequestMessageComposer)
        {

        }
    }
}
