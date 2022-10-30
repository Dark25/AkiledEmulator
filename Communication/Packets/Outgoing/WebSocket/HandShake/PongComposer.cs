namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class PongComposer : ServerPacket
    {
        public PongComposer()
            : base(4)
        {
        }
    }
}
