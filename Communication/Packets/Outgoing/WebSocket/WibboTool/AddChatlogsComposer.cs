namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class AddChatlogsComposer : ServerPacket
    {
        public AddChatlogsComposer(int UserId, string Pseudo, string Message)
          : base(7)
        {
            WriteInteger(UserId);
            WriteString(Pseudo);
            WriteString(Message);
        }
    }
}