namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomChatOptionsComposer : ServerPacket
    {
        public RoomChatOptionsComposer()
            : base(ServerPacketHeader.RoomChatSettingsComposer)
        {

        }
    }
}
