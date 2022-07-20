namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserHomeRoomComposer : ServerPacket
    {

        public UserHomeRoomComposer(int homeRoom, int roomToEnter)
            : base(ServerPacketHeader.UserHomeRoomComposer)
        {
            this.WriteInteger(homeRoom);
            this.WriteInteger(roomToEnter);
        }
    }
}
