namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class NotifAlertComposer : ServerPacket
    {
        public NotifAlertComposer(string Image, string Title, string Message, string TextButton, int RoomId, string Url)
         : base(12)
        {
            WriteString(Image);
            WriteString(Title);
            WriteString(Message);
            WriteString(TextButton);
            WriteInteger(RoomId);
            WriteString(Url);
        }
    }
}
