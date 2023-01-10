namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CostumNotificationComposer : ServerPacket
    {
        public CostumNotificationComposer(string Message)
            : base(ServerPacketHeader.CostumNotificationComposer)

        {
            base.WriteInteger(1);
            base.WriteString(Message);
        }
    }
}