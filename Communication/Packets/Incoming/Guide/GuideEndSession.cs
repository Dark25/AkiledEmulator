using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GuideEndSession : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionEnded);
            message.WriteInteger(1);
            Session.SendPacket(message);

            Session.GetHabbo().GuideOtherUserId = 0;
            if (Session.GetHabbo().OnDuty)
            {
                AkiledEnvironment.GetGame().GetGuideManager().EndService(Session.GetHabbo().Id);
            }

            if (requester != null)
            {
                requester.SendPacket(message);
                requester.GetHabbo().GuideOtherUserId = 0;

                if (requester.GetHabbo().OnDuty)
                {
                    AkiledEnvironment.GetGame().GetGuideManager().EndService(requester.GetHabbo().Id);
                }
            }
        }
    }
}