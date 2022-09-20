using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;
using AkiledEmulator.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GiveAdminRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Session.GetHabbo().Id != Group.CreatorId || !Group.IsMember(UserId))
                return;

            Habbo Habbo = AkiledEnvironment.GetHabboById(UserId);
            if (Habbo == null)
                return;

            Group.MakeAdmin(UserId);

            Room Room = null;
            if (AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (User != null)
                {
                    User.SetStatus("flatctrl", "3");

                    User.UpdateNeeded = true;

                    if (User.GetClient() != null)
                        User.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.GUILD_ADMIN));
                }
            }

            Session.SendPacket(new GroupMemberUpdatedComposer(GroupId, Habbo, 1));
        }
    }
}