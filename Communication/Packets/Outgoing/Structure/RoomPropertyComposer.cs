namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomPropertyComposer : ServerPacket
    {
        public RoomPropertyComposer(string name, string val)
            : base(ServerPacketHeader.RoomPropertyMessageComposer)
        {
            WriteString(name);
            WriteString(val);
        }
    }
}
