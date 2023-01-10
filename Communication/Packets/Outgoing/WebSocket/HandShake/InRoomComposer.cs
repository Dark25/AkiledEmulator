namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class InRoomComposer : ServerPacket
    {
        public InRoomComposer(bool InRoom)
            : base(5)
        {
            WriteBoolean(InRoom);
        }
    }
}
