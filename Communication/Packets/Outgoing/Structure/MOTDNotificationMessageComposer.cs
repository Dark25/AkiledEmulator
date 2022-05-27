namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MOTDNotificationMessageComposer : ServerPacket
    {
        public MOTDNotificationMessageComposer(string Message)
            : base(ServerPacketHeader.MOTDNotificationMessageComposer)
        {
            base.WriteInteger(1);
            base.WriteString(Message);

        }
    }
}
