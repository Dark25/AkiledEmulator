namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class HabboActivityPointNotificationComposer : ServerPacket
    {
        public HabboActivityPointNotificationComposer(int Balance, int Notif, int currencyType = 0)
            : base(ServerPacketHeader.HabboActivityPointNotificationMessageComposer)
        {
            WriteInteger(Balance);
            WriteInteger(Notif);
            WriteInteger(currencyType);//Type
        }
    }
}
