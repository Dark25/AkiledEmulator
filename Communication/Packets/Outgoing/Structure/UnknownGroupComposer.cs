namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UnknownGroupComposer : ServerPacket
    {
        public UnknownGroupComposer(int GroupId, int HabboId)
            : base(ServerPacketHeader.UnknownGroupMessageComposer)
        {
            WriteInteger(GroupId);
            WriteInteger(HabboId);
        }
    }
}
