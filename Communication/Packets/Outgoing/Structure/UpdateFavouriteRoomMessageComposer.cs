namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class UpdateFavouriteRoomMessageComposer : ServerPacket
    {
        public UpdateFavouriteRoomMessageComposer()
            : base(ServerPacketHeader.UpdateFavouriteRoomMessageComposer)
        {

        }
    }
}
