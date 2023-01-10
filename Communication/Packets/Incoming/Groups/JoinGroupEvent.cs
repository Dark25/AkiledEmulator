using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class JoinGroupEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(Packet.PopInt(), out Group))
                return;

            if (Group.IsMember(Session.GetHabbo().Id) || Group.IsAdmin(Session.GetHabbo().Id) || (Group.HasRequest(Session.GetHabbo().Id) && Group.GroupType == GroupType.LOCKED) || Group.GroupType == GroupType.PRIVATE)
                return;

            if (Session.GetHabbo().MyGroups.Count >= 50)
            {
                Session.SendNotification("¡Vaya, parece que has alcanzado el límite de membresía de grupo!Solo puedes unirte hasta 50 grupos.");
                return;
            }

            Group.AddMember(Session.GetHabbo().Id);

            if (Group.GroupType == GroupType.LOCKED)
            {
                /*List<GameClient> GroupAdmins = (from Client in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList() where Client != null && Client.GetHabbo() != null && Group.IsAdmin(Client.GetHabbo().Id) select Client).ToList();
                foreach (GameClient Client in GroupAdmins)
                {
                    Client.SendPacket(new GroupMembershipRequestedComposer(Group.Id, Session.GetHabbo(), 3));
                }*/

                Session.SendPacket(new GroupInfoComposer(Group, Session));
            }
            else
            {
                Session.GetHabbo().MyGroups.Add(Group.Id);
                Session.SendPacket(new GroupFurniConfigComposer(AkiledEnvironment.GetGame().GetGroupManager().GetGroupsForUser(Session.GetHabbo().MyGroups)));
                Session.SendPacket(new GroupInfoComposer(Group, Session));

                if (Session.GetHabbo().CurrentRoom != null)
                    Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                else
                    Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
            }

        }
    }
}
