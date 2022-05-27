namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketplaceCancelOfferResultComposer : ServerPacket
    {
        public MarketplaceCancelOfferResultComposer(int OfferId, bool Success)
            : base(ServerPacketHeader.MarketplaceCancelOfferResultMessageComposer)
        {
            WriteInteger(OfferId);
            WriteBoolean(Success);
        }
    }
}
