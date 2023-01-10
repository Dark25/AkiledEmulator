namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class TradingUpdateMessageComposer : ServerPacket
    {
        public TradingUpdateMessageComposer()
            : base(ServerPacketHeader.TradingUpdateMessageComposer)
        {

        }
    }
}
