namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserRightsComposer : ServerPacket
    {
        public UserRightsComposer(int Rank)
            : base(ServerPacketHeader.UserRightsMessageComposer)
        {
            WriteInteger(2);//Club level
            WriteInteger(Rank);
            WriteBoolean(false);//Is an ambassador
        }
    }
}
