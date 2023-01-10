namespace Akiled.Communication.Packets.Outgoing.Rooms.Camera
{
    public class SetCameraPriceMessageComposer : ServerPacket
    {
        public SetCameraPriceMessageComposer(int purchasePriceCoins, int purchasePriceDuckets, int publishPriceDuckets)
            : base(ServerPacketHeader.CameraPriceComposer)
        {
            base.WriteInteger(purchasePriceCoins);
            base.WriteInteger(purchasePriceDuckets);
            base.WriteInteger(publishPriceDuckets);
        }
    }
}