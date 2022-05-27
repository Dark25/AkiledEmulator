namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserRemoveComposer : ServerPacket
    {
        public UserRemoveComposer(int Id)
            : base(ServerPacketHeader.UserRemoveMessageComposer)
        {
            WriteString(Id.ToString());
        }
    }
}
