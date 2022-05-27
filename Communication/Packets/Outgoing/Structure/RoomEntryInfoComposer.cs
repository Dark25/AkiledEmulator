namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomEntryInfoComposer : ServerPacket
    {
        public RoomEntryInfoComposer(int roomID, bool isOwner)
            : base(ServerPacketHeader.RoomEntryInfoMessageComposer)
        {
            WriteInteger(roomID);
            WriteBoolean(isOwner);
        }
    }
}
