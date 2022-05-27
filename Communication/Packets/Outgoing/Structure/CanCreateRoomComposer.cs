namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CanCreateRoomComposer : ServerPacket
    {
        public CanCreateRoomComposer(bool Error, int MaxRoomsPerUser)
            : base(ServerPacketHeader.CanCreateRoomMessageComposer)
        {
            WriteInteger(Error ? 1 : 0);
            WriteInteger(MaxRoomsPerUser);
        }
    }
}
