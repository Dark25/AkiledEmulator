namespace Akiled.Communication.Packets.Outgoing.WebSocket.Troc
{
    class RpTrocAccepteComposer : ServerPacket
    {
        public RpTrocAccepteComposer(int UserId, bool Etat)
          : base(15)
        {
            WriteInteger(UserId);
            WriteBoolean(Etat);
        }
    }
}
