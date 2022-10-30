namespace Akiled.Communication.Packets.Outgoing.Rooms.Camera
{
    public class BuyPhoto : ServerPacket
    {
        public BuyPhoto()
            : base(ServerPacketHeader.CameraPriceComposer)
        {
        }
    }
}