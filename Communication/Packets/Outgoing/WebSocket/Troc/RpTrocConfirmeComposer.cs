namespace Akiled.Communication.Packets.Outgoing.WebSocket.Troc
{
    class RpTrocConfirmeComposer : ServerPacket
    {
        public RpTrocConfirmeComposer(int UserId)
          : base(16)
        {
            WriteInteger(UserId);
        }
    }
}
