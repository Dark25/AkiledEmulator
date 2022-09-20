using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Groups;using Akiled.HabboHotel.Rooms;using AkiledEmulator.HabboHotel.Rooms;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure{    class UpdateGroupSettingsEvent : IPacketEvent    {        public void Parse(GameClient Session, ClientPacket Packet)        {
            int GroupId = Packet.PopInt();

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (Group.CreatorId != Session.GetHabbo().Id)
                return;

            int Type = Packet.PopInt();
            int FurniOptions = Packet.PopInt();

            switch (Type)
            {
                default:
                case 0:
                    Group.GroupType = GroupType.OPEN;
                    break;
                case 1:
                    Group.GroupType = GroupType.LOCKED;
                    break;
                case 2:
                    Group.GroupType = GroupType.PRIVATE;
                    break;
            }

            if (Group.GroupType != GroupType.LOCKED)
            {
                if (Group.GetRequests.Count > 0)
                {
                    foreach (int UserId in Group.GetRequests.ToList())
                    {
                        Group.HandleRequest(UserId, false);
                    }

                    Group.ClearRequests();
                }
            }

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("UPDATE `groups` SET `state` = @GroupState, `admindeco` = @AdminDeco WHERE `id` = @groupId LIMIT 1");
                dbClient.AddParameter("GroupState", (Group.GroupType == GroupType.OPEN ? 0 : Group.GroupType == GroupType.LOCKED ? 1 : 2).ToString());
                dbClient.AddParameter("AdminDeco", (FurniOptions == 1 ? 1 : 0).ToString());
                dbClient.AddParameter("groupId", Group.Id);
                dbClient.RunQuery();
            }

            Group.AdminOnlyDeco = FurniOptions;

            Room Room;
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                return;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers().ToList())
            {
                if (Room.RoomData.OwnerId == User.UserId || Group.IsAdmin(User.UserId) || !Group.IsMember(User.UserId))
                    continue;

                if (FurniOptions == 1)
                {
                    User.SetStatus("flatctrl", "0");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.NONE));
                }
                else if (FurniOptions == 0 && !User.Statusses.ContainsKey("flatctrl"))
                {
                    User.SetStatus("flatctrl", "2");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.GUILD_RIGHTS));
                }
            }

            Session.SendPacket(new GroupInfoComposer(Group, Session));        }    }}