namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ReceiveBadgeComposer : ServerPacket
    {
        public ReceiveBadgeComposer(string BadgeCode)
            : base(ServerPacketHeader.AddUserBadgeComposer)
        {
            WriteInteger(1);
            WriteString(BadgeCode);
        }
    }
}
