using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class VisitRoomGuides : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            GameClient requester = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
                return;
            int roomid = requester.GetHabbo().CurrentRoomId;

            ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionRequesterRoom);
            message.WriteInteger(roomid);
            Session.SendPacket(message);


        }
    }
}
