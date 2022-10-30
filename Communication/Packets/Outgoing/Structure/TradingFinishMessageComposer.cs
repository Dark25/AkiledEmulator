namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class TradingFinishMessageComposer : ServerPacket
    {
        public TradingFinishMessageComposer()
            : base(ServerPacketHeader.TradingFinishMessageComposer)
        {

        }
    }
}
