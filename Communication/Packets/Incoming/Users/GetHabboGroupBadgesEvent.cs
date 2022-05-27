using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetHabboGroupBadgesEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().LoadingRoomId == 0)
                return;

            Room Room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().LoadingRoomId);
            if (Room == null)
                return;

            Dictionary<int, string> Badges = new Dictionary<int, string>();
            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                    continue;

                if (User.GetClient().GetHabbo().FavouriteGroupId == 0 || Badges.ContainsKey(User.GetClient().GetHabbo().FavouriteGroupId))
                    continue;

                Group Group = null;
                if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(User.GetClient().GetHabbo().FavouriteGroupId, out Group))
                    continue;

                if (!Badges.ContainsKey(Group.Id))
                    Badges.Add(Group.Id, Group.Badge);
            }

            if (Session.GetHabbo().FavouriteGroupId > 0)
            {
                Group Group = null;
                if (AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(Session.GetHabbo().FavouriteGroupId, out Group))
                {
                    if (!Badges.ContainsKey(Group.Id))
                        Badges.Add(Group.Id, Group.Badge);
                }
            }

            Room.SendPacket(new HabboGroupBadgesComposer(Badges));
            Session.SendPacket(new HabboGroupBadgesComposer(Badges));
        }
    }
}
