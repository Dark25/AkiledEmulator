namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomForwardComposer : ServerPacket
    {
        public RoomForwardComposer(int RoomId)
            : base(ServerPacketHeader.RoomForwardMessageComposer)
        {
            WriteInteger(RoomId);
        }
    }
}
