namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class GuestRoomSearchResultMessageComposer : ServerPacket
    {
        public GuestRoomSearchResultMessageComposer()
            : base(ServerPacketHeader.GuestRoomSearchResultMessageComposer)
        {

        }
    }
}
