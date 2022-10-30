namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserTypingMessageComposer : ServerPacket
    {
        public UserTypingMessageComposer()
            : base(ServerPacketHeader.UserTypingMessageComposer)
        {

        }
    }
}
