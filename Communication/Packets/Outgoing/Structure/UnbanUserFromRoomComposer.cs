namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UnbanUserFromRoomComposer : ServerPacket
    {
        public UnbanUserFromRoomComposer(int RoomId, int UserId)
            : base(ServerPacketHeader.UnbanUserFromRoomMessageComposer)
        {
            WriteInteger(RoomId);
            WriteInteger(UserId);
        }
    }
}
