namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketplaceMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceMakeOfferResultComposer(int Success)
            : base(ServerPacketHeader.MarketplaceMakeOfferResultMessageComposer)
        {
            WriteInteger(Success);
        }
    }
}
