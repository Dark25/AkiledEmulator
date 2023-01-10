namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BadgesMessageComposer : ServerPacket
    {
        public BadgesMessageComposer()
            : base(ServerPacketHeader.BadgesMessageComposer)
        {

        }
    }
}
