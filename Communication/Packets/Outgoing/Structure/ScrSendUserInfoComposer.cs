namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ScrSendUserInfoComposer : ServerPacket
    {
        public ScrSendUserInfoComposer()
            : base(ServerPacketHeader.ScrSendUserInfoMessageComposer)
        {
            int DisplayMonths = 0;
            int DisplayDays = 0;

            WriteString("habbo_club");
            WriteInteger(DisplayDays);
            WriteInteger(2);
            WriteInteger(DisplayMonths);
            WriteInteger(1);
            WriteBoolean(true); // hc
            WriteBoolean(true); // vip
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(495);
        }
    }
}
