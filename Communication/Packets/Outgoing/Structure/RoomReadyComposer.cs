namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomReadyComposer : ServerPacket
    {
        public RoomReadyComposer(int RoomId, string Model)
            : base(ServerPacketHeader.RoomReadyMessageComposer)
        {
            WriteString(Model);
            WriteInteger(RoomId);
        }
    }
}
