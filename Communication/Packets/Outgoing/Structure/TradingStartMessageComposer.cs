namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class TradingStartMessageComposer : ServerPacket
    {
        public TradingStartMessageComposer()
            : base(ServerPacketHeader.TradingStartMessageComposer)
        {

        }
    }
}
