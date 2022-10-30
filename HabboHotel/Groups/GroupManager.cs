using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Users;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Groups
{
    public class GroupManager
    {
        private readonly ConcurrentDictionary<int, Group> _groups;

        private readonly List<GroupBadgeParts> _bases;
        private readonly List<GroupBadgeParts> _symbols;
        private readonly List<GroupColours> _baseColours;
        private readonly Dictionary<int, GroupColours> _symbolColours;
        private readonly Dictionary<int, GroupColours> _backgroundColours;

        public GroupManager()
        {
            this._groups = new ConcurrentDictionary<int, Group>();

            this._bases = new List<GroupBadgeParts>();
            this._symbols = new List<GroupBadgeParts>();
            this._baseColours = new List<GroupColours>();
            this._symbolColours = new Dictionary<int, GroupColours>();
            this._backgroundColours = new Dictionary<int, GroupColours>();
        }

        public void Init()
        {
            this._bases.Clear();
            this._symbols.Clear();
            this._baseColours.Clear();
            this._symbolColours.Clear();
            this._backgroundColours.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `id`,`type`,`firstvalue`,`secondvalue` FROM `groups_items` WHERE `enabled` = '1'");
                DataTable dItems = dbClient.GetTable();

                foreach (DataRow dRow in dItems.Rows)
                {
                    switch (dRow["type"].ToString())
                    {
                        case "base":
                            this._bases.Add(new GroupBadgeParts(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "symbol":
                            this._symbols.Add(new GroupBadgeParts(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString(), dRow["secondvalue"].ToString()));
                            break;

                        case "color":
                            this._baseColours.Add(new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color2":
                            this._symbolColours.Add(Convert.ToInt32(dRow["id"]), new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;

                        case "color3":
                            this._backgroundColours.Add(Convert.ToInt32(dRow["id"]), new GroupColours(Convert.ToInt32(dRow["id"]), dRow["firstvalue"].ToString()));
                            break;
                    }
                }
            }
            Console.WriteLine("Grupos del Hotel -> Listo!");
        }

        public bool TryGetGroup(int id, out Group Group)
        {
            Group = null;

            if (this._groups.ContainsKey(id)) return this._groups.TryGetValue(id, out Group);

            if (this._groups.ContainsKey(id)) return this._groups.TryGetValue(id, out Group);

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `groups` WHERE `id` = @id LIMIT 1");
                dbClient.AddParameter("id", id);
                DataRow Row = dbClient.GetRow();

                if (Row == null) return false;

                Group = new Group(
                        Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["desc"]), Convert.ToString(Row["badge"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["owner_id"]),
                        Convert.ToInt32(Row["created"]), Convert.ToInt32(Row["state"]), Convert.ToInt32(Row["colour1"]), Convert.ToInt32(Row["colour2"]), Convert.ToInt32(Row["admindeco"]), Convert.ToInt32(Row["has_forum"]) == 1);
                this._groups.TryAdd(Group.Id, Group);
            }

            return true;
        }

        public bool TryCreateGroup(Habbo Player, string Name, string Description, int RoomId, string Badge, int Colour1, int Colour2, out Group Group)
        {
            Group = new Group(0, Name, Description, Badge, RoomId, Player.Id, (int)AkiledEnvironment.GetUnixTimestamp(), 0, Colour1, Colour2, 0, false);
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Badge)) return false;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO `groups` (`name`, `desc`, `badge`, `owner_id`, `created`, `room_id`, `state`, `colour1`, `colour2`, `admindeco`) VALUES (@name, @desc, @badge, @owner, UNIX_TIMESTAMP(), @room, '0', @colour1, @colour2, '1')");
                dbClient.AddParameter("name", Group.Name);
                dbClient.AddParameter("desc", Group.Description);
                dbClient.AddParameter("owner", Group.CreatorId);
                dbClient.AddParameter("badge", Group.Badge);
                dbClient.AddParameter("room", Group.RoomId);
                dbClient.AddParameter("colour1", Group.Colour1);
                dbClient.AddParameter("colour2", Group.Colour2);
                Group.Id = Convert.ToInt32(dbClient.InsertQuery());

                Group.AddMember(Player.Id);
                Group.MakeAdmin(Player.Id);

                Player.MyGroups.Add(Group.Id);

                if (!this._groups.TryAdd(Group.Id, Group)) return false;

                else
                {
                    dbClient.SetQuery("UPDATE `rooms` SET `groupId` = @gid WHERE `id` = @rid LIMIT 1");
                    dbClient.AddParameter("gid", Group.Id);
                    dbClient.AddParameter("rid", Group.RoomId);
                    dbClient.RunQuery();

                    dbClient.RunQuery("DELETE FROM `room_rights` WHERE `room_id` = '" + RoomId + "'");
                }
            }
            return true;
        }

        public string GetColourCode(int id, bool colourOne)
        {
            if (colourOne)
            {
                if (this._symbolColours.ContainsKey(id))
                {
                    return this._symbolColours[id].Colour;
                }
            }
            else
            {
                if (this._backgroundColours.ContainsKey(id))
                {
                    return this._backgroundColours[id].Colour;
                }
            }

            return "";
        }

        public void DeleteGroup(int id)
        {
            Group Group = null;
            if (this._groups.ContainsKey(id))
                this._groups.TryRemove(id, out Group);

            if (Group != null)
            {
                Group.Dispose();
            }
        }

        public List<Group> GetGroupsForUser(List<int> GroupIds)
        {
            List<Group> Groups = new List<Group>();

            foreach (int Id in GroupIds)
            {
                Group Group = null;
                if (this.TryGetGroup(Id, out Group))
                    Groups.Add(Group);
            }
            return Groups;
        }


        public ICollection<GroupBadgeParts> BadgeBases => this._bases;

        public ICollection<GroupBadgeParts> BadgeSymbols
        {
            get { return this._symbols; }
        }

        public ICollection<GroupColours> BadgeBaseColours => this._baseColours;

        public ICollection<GroupColours> BadgeSymbolColours => this._symbolColours.Values;

        public ICollection<GroupColours> BadgeBackColours => this._backgroundColours.Values;
    }
}
