namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class RoomCustomizedAlertComposer : ServerPacket
    {
        public RoomCustomizedAlertComposer(string Message)
            : base(ServerPacketHeader.RoomCustomizedAlertComposer)

        {
            base.WriteInteger(1);
            base.WriteString(Message);
        }
    }
}