namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ModeratorUserChatlogMessageComposer : ServerPacket
    {
        public ModeratorUserChatlogMessageComposer()
            : base(ServerPacketHeader.ModeratorUserChatlogMessageComposer)
        {

        }
    }
}
