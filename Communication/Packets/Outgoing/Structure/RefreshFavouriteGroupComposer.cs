namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RefreshFavouriteGroupComposer : ServerPacket
    {
        public RefreshFavouriteGroupComposer(int Id)
            : base(ServerPacketHeader.RefreshFavouriteGroupMessageComposer)
        {
            WriteInteger(Id);
        }
    }
}
