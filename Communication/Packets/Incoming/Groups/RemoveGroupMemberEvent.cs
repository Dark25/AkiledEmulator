using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;
using AkiledEmulator.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RemoveGroupMemberEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int GroupId = Packet.PopInt();
            int UserId = Packet.PopInt();

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            if (UserId == Session.GetHabbo().Id)
            {
                if (Group.IsMember(UserId))
                    Group.DeleteMember(UserId);

                Session.GetHabbo().MyGroups.Remove(Group.Id);

                if (Group.IsAdmin(UserId))
                {
                    if (Group.IsAdmin(UserId))
                        Group.TakeAdmin(UserId);

                    Room Room;

                    if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                        return;

                    RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (User != null)
                    {
                        User.SetStatus("flatctrl", "0");
                        User.UpdateNeeded = true;

                        if (User.GetClient() != null)
                            User.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.NONE));
                    }
                }

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("DELETE FROM `group_memberships` WHERE `group_id` = @GroupId AND `user_id` = @UserId");
                    dbClient.AddParameter("GroupId", GroupId);
                    dbClient.AddParameter("UserId", UserId);
                    dbClient.RunQuery();
                }

                Session.SendPacket(new GroupInfoComposer(Group, Session));
                if (Session.GetHabbo().FavouriteGroupId == GroupId)
                {
                    Session.GetHabbo().FavouriteGroupId = 0;
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `user_stats` SET `groupid` = '0' WHERE `id` = @userId LIMIT 1");
                        dbClient.AddParameter("userId", UserId);
                        dbClient.RunQuery();
                    }

                    if (Group.AdminOnlyDeco == 0)
                    {
                        Room Room;
                        if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Group.RoomId, out Room))
                            return;

                        RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (User != null)
                        {
                            User.SetStatus("flatctrl", "0");
                            User.UpdateNeeded = true;

                            if (User.GetClient() != null)
                                User.GetClient().SendPacket(new YouAreControllerComposer(RoomRightLevels.NONE));
                        }
                    }

                    if (Session.GetHabbo().InRoom && Session.GetHabbo().CurrentRoom != null)
                    {
                        RoomUser User = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                        if (User != null)
                            Session.GetHabbo().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                        Session.GetHabbo().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                    }
                    else
                        Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetHabbo().Id));
                }
                return;
            }
            else
            {
                if (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id))
                {
                    if (!Group.IsMember(UserId))
                        return;

                    if (Group.IsAdmin(UserId) && Group.CreatorId != Session.GetHabbo().Id)
                    {
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.groupremoveuser.error", Session.Langue));
                        return;
                    }

                    if (Group.IsAdmin(UserId))
                        Group.TakeAdmin(UserId);

                    if (Group.IsMember(UserId))
                        Group.DeleteMember(UserId);

                    Habbo Habbo = AkiledEnvironment.GetHabboById(UserId);
                    Habbo.MyGroups.Remove(Group.Id);

                    int StartIndex = (1 - 1) * 14 + 14;

                    List<Habbo> Members = new List<Habbo>();
                    List<int> MemberIds = Group.GetMembers.Skip(StartIndex).Take(14).ToList();
                    foreach (int Id in MemberIds.ToList())
                    {
                        Habbo GroupMember = AkiledEnvironment.GetHabboById(Id);
                        if (GroupMember == null)
                            continue;

                        if (!Members.Contains(GroupMember))
                            Members.Add(GroupMember);
                    }

                    int FinishIndex = 14 < Members.Count ? 14 : Members.Count;
                    int MembersCount = Group.GetMembers.Count;

                    Session.SendPacket(new GroupMembersComposer(Group, Members.Take(FinishIndex).ToList(), MembersCount, 1, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), 0, ""));
                }
            }
        }
    }
}