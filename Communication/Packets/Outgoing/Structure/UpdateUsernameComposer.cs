namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UpdateUsernameComposer : ServerPacket
    {
        public UpdateUsernameComposer(string User)
            : base(ServerPacketHeader.UpdateUsernameMessageComposer)
        {
            WriteInteger(0);
            WriteString(User);
            WriteInteger(0);
        }

        public UpdateUsernameComposer(string User, int VirtualId)
            : base(ServerPacketHeader.UpdateUsernameMessageComposer)
        {
            WriteInteger(VirtualId);
            WriteString(User);
            WriteInteger(1);
            WriteString(User);
        }
    }
}
