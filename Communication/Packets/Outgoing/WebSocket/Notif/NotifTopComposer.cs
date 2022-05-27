namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class NotifTopComposer : ServerPacket
    {
        public NotifTopComposer(string Message)
         : base(18)
        {
            WriteString(Message);
        }
    }
}
