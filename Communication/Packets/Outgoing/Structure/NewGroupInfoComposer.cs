namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NewGroupInfoComposer : ServerPacket
    {
        public NewGroupInfoComposer(int RoomId, int GroupId)
            : base(ServerPacketHeader.NewGroupInfoMessageComposer)
        {
            WriteInteger(RoomId);
            WriteInteger(GroupId);
        }
    }
}
