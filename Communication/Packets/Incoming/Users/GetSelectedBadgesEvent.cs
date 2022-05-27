using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users.Badges;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetSelectedBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Packet.PopInt());
            if (roomUserByHabbo == null || roomUserByHabbo.IsBot || roomUserByHabbo.GetClient() == null || roomUserByHabbo.GetClient().GetHabbo() == null || roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent() == null)
                return;
            ServerPacket Response = new ServerPacket(ServerPacketHeader.HabboUserBadgesMessageComposer);
            Response.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().Id);
            Response.WriteInteger(roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent().EquippedCount);

            int comptebadge = 0;
            foreach (Badge badge in roomUserByHabbo.GetClient().GetHabbo().GetBadgeComponent().BadgeList.Values)
            {
                if (badge.Slot > 0)
                {
                    comptebadge++;
                    if (comptebadge > 8)
                        break;
                    Response.WriteInteger(badge.Slot);
                    Response.WriteString(badge.Code);
                }
            }
            Session.SendPacket(Response);
        }
    }
}