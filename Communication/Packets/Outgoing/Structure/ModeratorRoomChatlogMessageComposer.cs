namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ModeratorRoomChatlogMessageComposer : ServerPacket
    {
        public ModeratorRoomChatlogMessageComposer()
            : base(ServerPacketHeader.ModeratorRoomChatlogMessageComposer)
        {

        }
    }
}
