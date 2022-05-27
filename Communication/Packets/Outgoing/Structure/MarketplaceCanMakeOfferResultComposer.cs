namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketplaceCanMakeOfferResultComposer : ServerPacket
    {
        public MarketplaceCanMakeOfferResultComposer(int Result)
            : base(ServerPacketHeader.MarketplaceCanMakeOfferResultMessageComposer)
        {
            WriteInteger(Result);
            WriteInteger(0);
        }
    }
}
