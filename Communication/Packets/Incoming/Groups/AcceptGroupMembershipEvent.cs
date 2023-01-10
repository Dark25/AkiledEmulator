using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AcceptGroupMembershipEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if ((Session.GetHabbo().Id != Group.CreatorId && !Group.IsAdmin(Session.GetHabbo().Id)) && !Session.GetHabbo().HasFuse("fuse_group_accept_any"))
                return;

            if (!Group.HasRequest(UserId))
                return;

            Habbo Habbo = AkiledEnvironment.GetHabboById(UserId);
            if (Habbo == null)
                return;

            Group.HandleRequest(UserId, true);

            Habbo.MyGroups.Add(Group.Id);

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 4));
        }
    }
}