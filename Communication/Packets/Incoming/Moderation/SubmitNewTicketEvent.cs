using Akiled.HabboHotel.GameClients;
using Akiled.Utilities;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class SubmitNewTicketEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (AkiledEnvironment.GetGame().GetModerationManager().UsersHasPendingTicket(Session.GetHabbo().Id))
                return;
            string Message = StringCharFilter.Escape(Packet.PopString());
            int TicketType = Packet.PopInt();
            int ReporterId = Packet.PopInt();
            int RoomId = Packet.PopInt();
            int RepporteurId = Packet.PopInt();

            AkiledEnvironment.GetGame().GetModerationManager().SendNewTicket(Session, TicketType, ReporterId, Message);
            AkiledEnvironment.GetGame().GetModerationManager().ApplySanction(Session, ReporterId);
        }
    }
}
