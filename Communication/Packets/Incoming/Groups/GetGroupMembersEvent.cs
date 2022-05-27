using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetGroupMembersEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            int GroupId = Packet.PopInt();
            int Page = Packet.PopInt();
            string SearchVal = Packet.PopString();
            int RequestType = Packet.PopInt();

            Group Group = null;
            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group))
                return;

            int StartIndex = (Page - 1) * 14 + 14;
            List<Habbo> Members = new List<Habbo>();
            int MemberCount = 0;

            switch (RequestType)
            {
                case 0:
                    MemberCount = Group.GetAllMembers.Count();
                    List<int> MemberIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        MemberIds = this.GetSearchMembres(Group.Id, SearchVal);
                    }
                    else
                    {
                        MemberIds = Group.GetAllMembers.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in MemberIds.ToList())
                    {
                        Habbo GroupMember = AkiledEnvironment.GetHabboById(Id);
                        if (GroupMember == null)
                            continue;

                        if (!Members.Contains(GroupMember))
                            Members.Add(GroupMember);
                    }
                    break;
                case 1:
                    MemberCount = Group.GetAdministrators.Count();
                    List<int> AdminIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        AdminIds = this.GetSearchAdmins(Group.Id, SearchVal);
                    }
                    else
                    {
                        AdminIds = Group.GetAdministrators.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int User in AdminIds.ToList())
                    {
                        Habbo GroupMember = AkiledEnvironment.GetHabboById(User);
                        if (GroupMember == null)
                            continue;

                        if (!Members.Contains(GroupMember))
                            Members.Add(GroupMember);
                    }
                    break;
                case 2:
                    MemberCount = Group.GetRequests.Count();
                    List<int> RequestIds = null;
                    if (!string.IsNullOrEmpty(SearchVal))
                    {
                        RequestIds = this.GetSearchRequests(Group.Id, SearchVal);
                    }
                    else
                    {
                        RequestIds = Group.GetRequests.Skip(StartIndex).Take(14).ToList();
                    }

                    foreach (int Id in RequestIds.ToList())
                    {
                        Habbo GroupMember = AkiledEnvironment.GetHabboById(Id);
                        if (GroupMember == null)
                            continue;

                        if (!Members.Contains(GroupMember))
                            Members.Add(GroupMember);
                    }
                    break;
            }

            //if (!string.IsNullOrEmpty(SearchVal))
            //{
            //Members = Members.Where(x => x.Username.StartsWith(SearchVal)).ToList();
            //}

            Session.SendPacket(new GroupMembersComposer(Group, Members.ToList(), MemberCount, Page, (Group.CreatorId == Session.GetHabbo().Id || Group.IsAdmin(Session.GetHabbo().Id)), RequestType, SearchVal));

        }

        private List<int> GetSearchRequests(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id FROM group_requests INNER JOIN users ON group_requests.user_id = users.id WHERE group_requests.group_id = @gid AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                    MembersId.Add(Convert.ToInt32(row["id"]));
            }

            return MembersId;
        }

        private List<int> GetSearchAdmins(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND group_memberships.rank > '0' AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                    MembersId.Add(Convert.ToInt32(row["id"]));
            }

            return MembersId;
        }

        private List<int> GetSearchMembres(int GroupeId, string SearchVal)
        {
            List<int> MembersId = new List<int>();

            DataTable MembresTable = null;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT users.id AS id FROM group_memberships INNER JOIN users ON group_memberships.user_id = users.id WHERE group_memberships.group_id = @gid AND users.username LIKE @username LIMIT 14;");
                dbClient.AddParameter("gid", GroupeId);
                dbClient.AddParameter("username", SearchVal.Replace("%", "\\%").Replace("_", "\\_") + "%");
                MembresTable = dbClient.GetTable();
            }

            foreach (DataRow row in MembresTable.Rows)
            {
                if (!MembersId.Contains(Convert.ToInt32(row["id"])))
                    MembersId.Add(Convert.ToInt32(row["id"]));
            }

            return MembersId;
        }
    }
}