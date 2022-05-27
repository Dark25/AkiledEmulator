namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UserTagsComposer : ServerPacket
    {
        public UserTagsComposer(int UserId)
            : base(ServerPacketHeader.UserTagsMessageComposer)
        {
            WriteInteger(UserId);
            WriteInteger(2);//Count of the tags.
            {
                WriteString("Akiled.com");
                WriteString("test");
                //Append a string.
            }
        }
    }
}
