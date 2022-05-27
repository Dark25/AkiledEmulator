namespace Akiled.Communication.Packets.Outgoing.WebSocket.Troc
{
    class RpTrocStartComposer : ServerPacket
    {
        public RpTrocStartComposer(int UserId, string Username)
          : base(13)
        {
            WriteInteger(UserId);
            WriteString(Username);
        }
    }
}
