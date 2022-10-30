namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class TradingClosedMessageComposer : ServerPacket
    {
        public TradingClosedMessageComposer()
            : base(ServerPacketHeader.TradingClosedMessageComposer)
        {

        }
    }
}
