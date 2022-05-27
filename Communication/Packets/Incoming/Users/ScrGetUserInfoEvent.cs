using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ScrGetUserInfoMessageEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {

            ServerPacket Message = new ServerPacket(ServerPacketHeader.ScrSendUserInfoMessageComposer);
            Message.WriteString("habbo_club");
            Double TimeLeft = 30000000;
            int TotalDaysLeft = (int)Math.Ceiling(TimeLeft / 86400);
            int MonthsLeft = TotalDaysLeft / 31;

            if (MonthsLeft >= 1)
            {
                MonthsLeft--;
            }
            Message.WriteInteger(TotalDaysLeft - (MonthsLeft * 31));
            Message.WriteInteger(2); // ??
            Message.WriteInteger(MonthsLeft);
            Message.WriteInteger(1); // type
            Message.WriteBoolean(true);
            Message.WriteBoolean(true);
            Message.WriteInteger(0);
            Message.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on hc
            Message.WriteInteger(Convert.ToInt32(TimeLeft)); // days i have on vip
            Session.SendPacket(Message);

        }
    }
}
