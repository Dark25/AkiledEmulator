using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Daos.User;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Achievements;
using Akiled.HabboHotel.Rooms.Chat.Commands.Cmd;
using Akiled.HabboHotel.Users.Badges;
using Akiled.HabboHotel.Users.Messenger;
using MySqlX.XDevAPI.Relational;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Users.UserData
{
    public class UserDataFactory
    {
        public static UserData GetUserData(string sessionTicket, string ip, string machineid)
        {
            try
            {
                int userId;
                DataRow dUserInfo;
                DataRow row2;
                DataTable Achievement;
                DataTable Favorites;
                DataTable RoomRights;
                DataTable Badges;
                DataTable FrienShips;
                DataTable Requests;
                DataTable Quests;
                DataTable GroupMemberships;

                bool ChangeName = false;
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("SELECT * FROM users WHERE auth_ticket = @sso LIMIT 1");
                    queryreactor.AddParameter("sso", sessionTicket);

                    dUserInfo = queryreactor.GetRow();
                    if (dUserInfo == null) return (UserData)null;

                    queryreactor.SetQuery("SELECT id FROM bans WHERE expire > @nowtime AND ((bantype = 'user' AND value = @username) OR (bantype = 'ip' AND value = @IP1) OR (bantype = 'ip' AND value = @IP2) OR (bantype = 'machine' AND value = @machineid)) LIMIT 1");
                    queryreactor.AddParameter("nowtime", AkiledEnvironment.GetUnixTimestamp());
                    queryreactor.AddParameter("username", (string)dUserInfo["username"]);
                    queryreactor.AddParameter("IP1", ip);
                    queryreactor.AddParameter("IP2", (string)dUserInfo["ip_last"]);
                    queryreactor.AddParameter("machineid", machineid);

                    DataRow IsBanned = queryreactor.GetRow();
                    if (IsBanned != null) return (UserData)null;

                    userId = Convert.ToInt32(dUserInfo["id"]);
                    string username = (string)dUserInfo["username"];
                    if (AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(userId) != null)
                    {
                        AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(userId).Disconnect();
                        return (UserData)null;
                    }
                    string LastDailyCredits = (string)dUserInfo["lastdailycredits"];
                    string DateAujourdhui = DateTime.Today.ToString("MM/dd");
                    string respect1 = (AkiledEnvironment.GetConfig().data["respect_user_daily"]);
                    string respect2 = (AkiledEnvironment.GetConfig().data["respect_userstaff_daily"]);
                    if (LastDailyCredits != DateAujourdhui)
                    {
                        if (Convert.ToInt32(dUserInfo["rank"]) == 1)
                            queryreactor.RunQuery("UPDATE user_stats SET dailyrespectpoints  = " + respect1 + ", DailyPetRespectPoints = " + respect1 + " WHERE id = '" + userId + "' LIMIT 1");
                        else
                            queryreactor.RunQuery("UPDATE user_stats SET dailyrespectpoints = " + respect2 + ", DailyPetRespectPoints = " + respect2 + " WHERE id = '" + userId + "' LIMIT 1");

                        //ChangeName = true;
                    }

                    queryreactor.RunQuery("UPDATE users SET online = '1' WHERE id = '" + userId + "'");


                    queryreactor.SetQuery("SELECT * FROM user_stats WHERE id = '" + userId + "';");
                    row2 = queryreactor.GetRow();

                    if (row2 == null)
                    {
                        queryreactor.RunQuery("INSERT INTO user_stats (id) VALUES ('" + userId + "')");
                        queryreactor.SetQuery("SELECT * FROM user_stats WHERE id =  '" + userId + "';");
                        row2 = queryreactor.GetRow();
                    }

                    queryreactor.SetQuery("SELECT * FROM user_achievement WHERE userid = '" + userId + "';");
                    Achievement = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT room_id FROM user_favorites WHERE user_id = '" + userId + "';");
                    Favorites = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT room_id FROM room_rights WHERE user_id = '" + userId + "';");
                    RoomRights = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT * FROM user_badges WHERE user_id = '" + userId + "';");
                    Badges = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT users.id,users.username,messenger_friendships.relation FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = '" + userId + "'");
                    FrienShips = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = '" + userId + "'");
                    Requests = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT * FROM user_quests WHERE user_id = '" + userId + "';");
                    Quests = queryreactor.GetTable();

                    queryreactor.SetQuery("SELECT group_id FROM group_memberships WHERE user_id = '" + userId + "';");
                    GroupMemberships = queryreactor.GetTable();

                    string name_hotel = (AkiledEnvironment.GetConfig().data["namehotel_text"]);
                    string icon_newuser = (AkiledEnvironment.GetConfig().data["icon_newuser"]);
                    string icon_newconectado = (AkiledEnvironment.GetConfig().data["icon_newconectado"]);
                    string Username = Convert.ToString(dUserInfo["username"]);
                    string Ip = Convert.ToString(dUserInfo["ip_last"]);
                    int Rank = Convert.ToInt32(dUserInfo["rank"]);
                    int nuevo = Convert.ToInt32(dUserInfo["nuevo"]);
                    if (nuevo == 1)
                    {
                        AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble(icon_newuser, "El usuario: " + Username + " - Ha entrado por primera vez en " + name_hotel + ", acercate y dale una bienvenida. "));
                        queryreactor.RunQuery("UPDATE users SET nuevo = '0' WHERE id = '" + userId + "';");
                    }
                    else if ((Rank >= 1) && (Rank != 19) && (Rank != 25) && (Rank != 888))
                    {
                        AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble(icon_newconectado, "El usuario: " + Username + " (ID:" + Convert.ToInt32(dUserInfo["id"]) + "), IP: " + Ip + " - Se ha conectado con el rango: " + Rank));
                    }
                }

                Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();
                foreach (DataRow dataRow in Achievement.Rows)
                {
                    string str = (string)dataRow["group"];
                    int level = (int)dataRow["level"];
                    int progress = (int)dataRow["progress"];
                    UserAchievement userAchievement = new UserAchievement(str, level, progress);
                    achievements.Add(str, userAchievement);
                }

                if (!achievements.ContainsKey("ACH_CameraPhotoCount"))
                {
                    UserAchievement userAchievement = new UserAchievement("ACH_CameraPhotoCount", 10, 0);
                    achievements.Add("ACH_CameraPhotoCount", userAchievement);
                }

                List<int> RoomRightsList = new List<int>();
                foreach (DataRow dataRow in RoomRights.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["room_id"]);
                    RoomRightsList.Add(num3);
                }

                List<int> favouritedRooms = new List<int>();
                foreach (DataRow dataRow in Favorites.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["room_id"]);
                    favouritedRooms.Add(num3);
                }

                List<Badge> badges = new List<Badge>();
                foreach (DataRow dataRow in Badges.Rows)
                {
                    string Code = (string)dataRow["badge_id"];
                    int Slot = (int)dataRow["badge_slot"];
                    badges.Add(new Badge(Code, Slot));
                }

                Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
                Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
                foreach (DataRow dataRow in FrienShips.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["id"]);
                    string pUsername = (string)dataRow["username"];
                    string pLook = "";//(string)dataRow["look"];
                    int Relation = Convert.ToInt32(dataRow["relation"]);
                    if (num3 != userId)
                    {
                        if (!friends.ContainsKey(num3))
                        {
                            friends.Add(num3, new MessengerBuddy(num3, pUsername, pLook, Relation));
                            if (Relation != 0)
                                Relationships.Add(num3, new Relationship(num3, Relation));
                        }
                    }
                }

                Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
                foreach (DataRow dataRow in Requests.Rows)
                {
                    int num3 = Convert.ToInt32(dataRow["from_id"]);
                    int num4 = Convert.ToInt32(dataRow["to_id"]);
                    string pUsername = (string)dataRow["username"];
                    if (num3 != userId)
                    {
                        if (!requests.ContainsKey(num3))
                            requests.Add(num3, new MessengerRequest(userId, num3, pUsername));
                    }
                    else if (!requests.ContainsKey(num4))
                        requests.Add(num4, new MessengerRequest(userId, num4, pUsername));
                }

                Dictionary<int, int> quests = new Dictionary<int, int>();
                foreach (DataRow dataRow in Quests.Rows)
                {
                    int key = Convert.ToInt32(dataRow["quest_id"]);
                    int num3 = (int)dataRow["progress"];
                    quests.Add(key, num3);
                }

                List<int> MyGroups = new List<int>();
                foreach (DataRow dRow in GroupMemberships.Rows)
                {
                    MyGroups.Add((int)dRow["group_id"]);
                }

                Habbo user = GenerateHabbo(dUserInfo, row2, ChangeName);

                string staffchat = (AkiledEnvironment.GetConfig().data["staffchat"]);
                string staffchatrank = (AkiledEnvironment.GetConfig().data["staffchatrank"]);
                string modschat = (AkiledEnvironment.GetConfig().data["modschat"]);
                string modschatrank = (AkiledEnvironment.GetConfig().data["modschatrank"]);
                string gamemasterchat = (AkiledEnvironment.GetConfig().data["gamemasterchat"]);
                string gamemasterchatrank = (AkiledEnvironment.GetConfig().data["gamemasterchatrank"]);
                string interschat = (AkiledEnvironment.GetConfig().data["interschat"]);
                string interschatrank = (AkiledEnvironment.GetConfig().data["interschatrank"]);
                string guiaschat = (AkiledEnvironment.GetConfig().data["guiaschat"]);
                string guiaschatrank = (AkiledEnvironment.GetConfig().data["guiaschatrank"]);
                string pubschat = (AkiledEnvironment.GetConfig().data["pubschat"]);
                string pubschatrank = (AkiledEnvironment.GetConfig().data["pubschatrank"]);
                string lookchatpub = (AkiledEnvironment.GetConfig().data["lookchatpub"]);
                string lookchatinter = (AkiledEnvironment.GetConfig().data["lookchatinter"]);
                string lookchatguia = (AkiledEnvironment.GetConfig().data["lookchatguia"]);
                string lookchatgm = (AkiledEnvironment.GetConfig().data["lookchatgm"]);
                string lookchatmod = (AkiledEnvironment.GetConfig().data["lookchatmod"]);
                string lookchatstaff = (AkiledEnvironment.GetConfig().data["lookchatstaff"]);
                if (staffchat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(staffchatrank)) //Change it to the minimum rank you want
                        friends.Add(0x7fffffff, new MessengerBuddy(0x7fffffff, "Staff Chat", lookchatstaff, 0));
                }
                if (modschat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(modschatrank) && user.Ismod || user.HasFuse("fuse_allchatsvisible")) //Change it to the minimum rank you want
                        friends.Add(0x6fffffff, new MessengerBuddy(0x6fffffff, "Mods Chat", lookchatmod, 0));
                }
                if (gamemasterchat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(gamemasterchatrank) && user.Isgm || user.HasFuse("fuse_allchatsvisible")) //Change it to the minimum rank you want
                        friends.Add(0x5fffffff, new MessengerBuddy(0x5fffffff, "GM's Chat", lookchatgm, 0));
                }
                if (interschat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(interschatrank) && user.Isinter || user.HasFuse("fuse_allchatsvisible")) //Change it to the minimum rank you want
                        friends.Add(0x4fffffff, new MessengerBuddy(0x4fffffff, "Inter's Chat", lookchatinter, 0));
                }
                if (guiaschat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(guiaschatrank) && user.Isguia || user.HasFuse("fuse_allchatsvisible")) //Change it to the minimum rank you want
                        friends.Add(0x3fffffff, new MessengerBuddy(0x3fffffff, "Embajadores", lookchatguia, 0));
                }
                if (pubschat == "true")
                {
                    if (user.Rank >= Convert.ToInt32(pubschatrank) && user.Ispub || user.HasFuse("fuse_allchatsvisible")) //Change it to the minimum rank you want
                        friends.Add(0x2fffffff, new MessengerBuddy(0x2fffffff, "Pub's Chat", lookchatpub, 0));
                }

                return new UserData(userId, achievements, favouritedRooms, badges, friends, requests, quests, MyGroups, user, Relationships, RoomRightsList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public static UserData GetUserData(int UserId)
        {
            DataRow row;
            DataRow row2;
            int userID;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM users WHERE id = @id LIMIT 1");
                queryreactor.AddParameter("id", UserId);
                row = queryreactor.GetRow();
                if (row == null)
                    return (UserData)null;

                userID = Convert.ToInt32(row["id"]);
                if (AkiledEnvironment.GetGame()?.GetClientManager()?.GetClientByUserID(userID) != null)
                    return (UserData)null;

                queryreactor.SetQuery("SELECT * FROM user_stats WHERE id = @id");
                queryreactor.AddParameter("id", UserId);
                row2 = queryreactor.GetRow();

                if (row2 == null)
                {
                    queryreactor.RunQuery("INSERT INTO user_stats (id) VALUES ('" + UserId + "')");
                    queryreactor.SetQuery("SELECT * FROM user_stats WHERE id = " + UserId);
                    row2 = queryreactor.GetRow();
                }
            }
            Dictionary<string, UserAchievement> achievements = new Dictionary<string, UserAchievement>();
            List<int> favouritedRooms = new List<int>();
            List<int> RoomRight = new List<int>();
            List<Badge> badges = new List<Badge>();
            Dictionary<int, MessengerBuddy> friends = new Dictionary<int, MessengerBuddy>();
            Dictionary<int, MessengerRequest> requests = new Dictionary<int, MessengerRequest>();
            Dictionary<int, int> quests = new Dictionary<int, int>();
            Dictionary<int, Relationship> Relationships = new Dictionary<int, Relationship>();
            List<int> MyGroups = new List<int>();

            Habbo user = GenerateHabbo(row, row2, false);
            return new UserData(userID, achievements, favouritedRooms, badges, friends, requests, quests, MyGroups, user, Relationships, RoomRight);
        }

      
        public static Habbo GenerateHabbo(DataRow dRow, DataRow dRow2, bool ChangeName)
        {
            int Id = Convert.ToInt32(dRow["id"]);
            string Username = (string)dRow["username"];
            string Prefix = Convert.ToString(dRow["prefix"]);
            string Prefixnamecolor = Convert.ToString(dRow["prefixnamecolor"]);
            string PrefixSize = Convert.ToString(dRow["prefixsize"]);
            int Rank = Convert.ToInt32(dRow["rank"]);
            string Motto = (string)dRow["motto"];
            string Look = (string)dRow["look"];
            string Gender = (string)dRow["gender"];
            int LastOnline = (int)dRow["last_online"];
            int Credits = (int)dRow["credits"];
            int Diamonds = (int)dRow["vip_points"];
            int ActivityPoints = (int)dRow["activity_points"];
            double LastActivityPointsUpdate = Convert.ToDouble(dRow["activity_points_lastupdate"]);
            int HomeRoom = Convert.ToInt32(dRow["home_room"]);
            int Respect = (int)dRow2["respect"];
            int DailyRespectPoints = (int)dRow2["DailyRespectPoints"];
            int DailyPetRespectPoints = (int)dRow2["DailyPetRespectPoints"];
            bool HasFriendRequestsDisabled = AkiledEnvironment.EnumToBool(dRow["block_newfriends"].ToString());
            int currentQuestID = Convert.ToInt32(dRow2["quest_id"]);
            int achievementPoints = (int)dRow2["AchievementScore"];
            int FavoriteGroup = (int)dRow2["groupid"];
            int accountCreated = (int)dRow["account_created"];
            bool AcceptTrading = AkiledEnvironment.EnumToBool(dRow["accept_trading"].ToString());
            string Ip = dRow["ip_last"].ToString();
            bool HideInroom = AkiledEnvironment.EnumToBool(dRow["hide_inroom"].ToString());
            bool HideOnline = AkiledEnvironment.EnumToBool(dRow["hide_online"].ToString());
            int MazoHighScore = Convert.ToInt32(dRow["mazoscore"]);
            int Mazo = Convert.ToInt32(dRow["mazo"]);
            string clientVolume = (string)dRow["volume"];
            bool NuxEnable = AkiledEnvironment.EnumToBool(dRow["nux_enable"].ToString());
            string MachineId = (string)dRow["machine_id"];
            Language Langue = LanguageManager.ParseLanguage((string)dRow["langue"]);
            bool IgnoreAll = AkiledEnvironment.EnumToBool(dRow["ignoreall"].ToString());
            int last_marked_friend = Convert.ToInt32(dRow["last_marked_friend"]);
            double time_muted = Convert.ToDouble(dRow["time_muted"]);
            string lastdailycredits = Convert.ToString(dRow["lastdailycredits"]);
            bool NuxEnable2 = AkiledEnvironment.EnumToBool(dRow["nux_enable2"].ToString());
            bool Ismod = AkiledEnvironment.EnumToBool(dRow["ismod"].ToString());
            bool ispub = AkiledEnvironment.EnumToBool(dRow["ispub"].ToString());
            bool Isinter = AkiledEnvironment.EnumToBool(dRow["isinter"].ToString());
            bool Isgm = AkiledEnvironment.EnumToBool(dRow["isgm"].ToString());
            bool Isguia = AkiledEnvironment.EnumToBool(dRow["isguia"].ToString());
            bool IsEMB = AkiledEnvironment.EnumToBool(dRow["isemb"].ToString());
            int int32_8 = Convert.ToInt32(dRow["angelpass"]);
            int int32_9 = Convert.ToInt32(dRow["angelstatus"]);
            int int32_10 = Convert.ToInt32(dRow["miningpass"]);
            var gamePointsMonth = Convert.ToInt32(dRow["game_points_month"]);
            return new Habbo(Id, Username, Prefix, Prefixnamecolor, PrefixSize, Rank, Motto, Look, Gender, Credits, Diamonds, ActivityPoints, LastActivityPointsUpdate, HomeRoom, Respect, DailyRespectPoints, DailyPetRespectPoints, HasFriendRequestsDisabled, currentQuestID, achievementPoints, LastOnline, FavoriteGroup, accountCreated, AcceptTrading, Ip, HideInroom, HideOnline, MazoHighScore, Mazo, clientVolume, NuxEnable, MachineId, ChangeName, Langue, IgnoreAll, PetsMuted: false, BotsMuted: false, last_marked_friend, time_muted, lastdailycredits, NuxEnable2, Ismod, ispub, Isinter, Isgm, Isguia, IsEMB, int32_8, int32_9, int32_10, gamePointsMonth);
        }
       
    }
}
