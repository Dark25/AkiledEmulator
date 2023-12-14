using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Enemy;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.RoomBots;
using Akiled.Utilities;
using AkiledEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Akiled.HabboHotel.Rooms
{
    public delegate void UserAndItemDelegate(RoomUser user, Item item);

    public class RoomUserManager
    {
        private Room _room;
        private ConcurrentDictionary<string, RoomUser> _usersByUsername;
        private ConcurrentDictionary<int, RoomUser> _usersByUserID;

        private ConcurrentDictionary<int, RoomUser> _users;
        private readonly ConcurrentDictionary<int, RoomUser> _pets;
        private readonly ConcurrentDictionary<int, RoomUser> _bots;

        private readonly List<int> _usersRank;

        private int _primaryPrivateUserID;
        public int BotCounter;

        public event RoomEventDelegate OnUserEnter;

        public RoomUserManager(Room room)
        {
            this._room = room;
            this._users = new ConcurrentDictionary<int, RoomUser>();
            this._pets = new ConcurrentDictionary<int, RoomUser>();
            this._bots = new ConcurrentDictionary<int, RoomUser>();
            this._usersByUsername = new ConcurrentDictionary<string, RoomUser>();
            this._usersByUserID = new ConcurrentDictionary<int, RoomUser>();
            this._usersRank = new List<int>();
            this._primaryPrivateUserID = 1;
            this.BotCounter = 0;
        }

        public void UserEnter(RoomUser thisUser)
        {
            if (this.OnUserEnter != null)
                this.OnUserEnter(thisUser, (EventArgs)null);
        }

        public int GetRoomUserCount() => this._room.RoomData.UsersNow;

        public RoomUser DeploySuperBot(RoomBot Bot)
        {
            int key = this._primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this._room.Id, key, this._room);

            Bot.Id = -key;

            this._users.TryAdd(key, roomUser);

            this.BotCounter++;

            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;
            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this._room.Id, roomUser, this._room);

            roomUser.SetStatus("flatctrl 4", "");
            this.UpdateUserStatus(roomUser, false);
            roomUser.UpdateNeeded = true;

            this._room.SendPacket(new UsersComposer(roomUser));

            roomUser.BotAI.OnSelfEnterRoom();

            if (this._bots.ContainsKey(roomUser.BotData.Id))
                this._bots[roomUser.BotData.Id] = roomUser;
            else
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);

            return roomUser;
        }

        public bool UpdateClientUsername(RoomUser User, string OldUsername, string NewUsername)
        {
            if (!this._usersByUsername.ContainsKey(OldUsername.ToLower()))
                return false;
            _usersByUsername.TryRemove(OldUsername.ToLower(), out User);
            _usersByUsername.TryAdd(NewUsername.ToLower(), User);
            return true;
        }

        public RoomUser DeployBot(RoomBot Bot, Pet PetData)
        {
            int key = this._primaryPrivateUserID++;
            RoomUser roomUser = new RoomUser(0, this._room.Id, key, this._room);

            this._users.TryAdd(key, roomUser);

            roomUser.SetPos(Bot.X, Bot.Y, Bot.Z);
            roomUser.SetRot(Bot.Rot, false);

            roomUser.BotData = Bot;

            if (this._room.IsRoleplay)
            {
                RPEnemy Enemy;
                if (Bot.IsPet)
                    Enemy = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(Bot.Id);
                else
                    Enemy = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(Bot.Id);

                if (Enemy != null)
                {
                    roomUser.BotData.RoleBot = new RoleBot(Enemy);
                    if (Bot.IsPet)
                        roomUser.BotData.AiType = AIType.RolePlayPet;
                    else
                        roomUser.BotData.AiType = AIType.RolePlayBot;
                }
            }

            roomUser.BotAI = Bot.GenerateBotAI(roomUser.VirtualId);

            if (roomUser.IsPet)
            {
                roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this._room.Id, roomUser, this._room);
                roomUser.PetData = PetData;
                roomUser.PetData.VirtualId = roomUser.VirtualId;
            }
            else
            {
                roomUser.BotAI.Init(Bot.Id, roomUser.VirtualId, this._room.Id, roomUser, this._room);
            }
            this.BotCounter++;
            roomUser.SetStatus("flatctrl 4", "");

            if (Bot.Status == 1)
            {
                roomUser.SetStatus("sit", "0.5");
                roomUser.IsSit = true;
            }

            if (Bot.Status == 2)
            {
                roomUser.SetStatus("lay", "0.7");
                roomUser.IsLay = true;
            }

            this.UpdateUserStatus(roomUser, false);
            roomUser.UpdateNeeded = true;

            if (Bot.IsDancing)
            {
                roomUser.DanceId = 3;
                ServerPacket Response = new ServerPacket(ServerPacketHeader.DanceMessageComposer);
                Response.WriteInteger(roomUser.VirtualId);
                Response.WriteInteger(3);
                this._room.SendPacket(Response);
            }

            if (Bot.Enable > 0)
            {
                roomUser.ApplyEffect(Bot.Enable);
            }

            if (Bot.Handitem > 0)
            {
                roomUser.CarryItem(Bot.Handitem, true);
            }

            this._room.SendPacket(new UsersComposer(roomUser));

            roomUser.BotAI.OnSelfEnterRoom();
            if (roomUser.IsPet)
            {
                if (this._pets.ContainsKey(roomUser.PetData.PetId))
                    this._pets[roomUser.PetData.PetId] = roomUser;
                else
                    this._pets.TryAdd(roomUser.PetData.PetId, roomUser);
            }
            else if (this._bots.ContainsKey(roomUser.BotData.Id))
                this._bots[roomUser.BotData.Id] = roomUser;
            else
                this._bots.TryAdd(roomUser.BotData.Id, roomUser);

            return roomUser;
        }

        public void RemoveBot(int VirtualId, bool Kicked)
        {
            RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(VirtualId);
            if (roomUserByVirtualId == null || !roomUserByVirtualId.IsBot)
                return;
            if (roomUserByVirtualId.IsPet)
            {
                RoomUser PetRemoval = null;
                this._pets.TryRemove(roomUserByVirtualId.PetData.PetId, out PetRemoval);
            }
            else
            {
                RoomUser BotRemoval = null;
                this._bots.TryRemove(roomUserByVirtualId.BotData.Id, out BotRemoval);
            }
            this.BotCounter--;
            roomUserByVirtualId.BotAI.OnSelfLeaveRoom(Kicked);

            this._room.SendPacket(new UserRemoveComposer(roomUserByVirtualId.VirtualId));

            this._room.GetGameMap().RemoveTakingSquare(roomUserByVirtualId.SetX, roomUserByVirtualId.SetY);
            this._room.GetGameMap().RemoveUserFromMap(roomUserByVirtualId, new Point(roomUserByVirtualId.X, roomUserByVirtualId.Y));

            RoomUser toRemove = null;
            this._users.TryRemove(roomUserByVirtualId.VirtualId, out toRemove);

        }

        private void UpdateUserEffect(RoomUser User, int x, int y)
        {
            try
            {
                if (User == null)
                    return;
                if (User.IsPet)
                    return;
                if (!this._room.GetGameMap().ValidTile(x, y))
                    return;
                byte pByte = this._room.GetGameMap().EffectMap[x, y];
                if (pByte > 0)
                {
                    ItemEffectType itemEffectType = ByteToItemEffectEnum.Parse(pByte);
                    if (itemEffectType == User.CurrentItemEffect)
                        return;
                    switch (itemEffectType)
                    {
                        case ItemEffectType.None:
                            User.ApplyEffect(0);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.Swim:
                            User.ApplyEffect(29);
                            if (User.GetClient() != null)
                                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1948);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.SwimLow:
                            User.ApplyEffect(30);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.SwimHalloween:
                            User.ApplyEffect(37);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.Iceskates:
                            if (User.GetClient() != null)
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                    User.ApplyEffect(38);
                                else
                                    User.ApplyEffect(39);
                            }
                            else
                                User.ApplyEffect(38);
                            User.CurrentItemEffect = ItemEffectType.Iceskates;
                            if (User.GetClient() != null)
                                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 1413);
                            break;
                        case ItemEffectType.Normalskates:
                            if (User.GetClient() != null)
                            {
                                if (User.GetClient().GetHabbo().Gender == "M")
                                    User.ApplyEffect(55);
                                else
                                    User.ApplyEffect(56);
                            }
                            else
                                User.ApplyEffect(55);

                            User.CurrentItemEffect = itemEffectType;
                            if (User.GetClient() != null)
                                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.EXPLORE_FIND_ITEM, 2199);
                            break;
                        case ItemEffectType.Trampoline:
                            User.ApplyEffect(193);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.TreadMill:
                            User.ApplyEffect(194);
                            User.CurrentItemEffect = itemEffectType;
                            break;
                        case ItemEffectType.CrossTrainer:
                            User.ApplyEffect(195);
                            User.CurrentItemEffect = itemEffectType;
                            break;

                    }
                }
                else
                {
                    if (User.CurrentItemEffect == ItemEffectType.None || pByte != 0)
                        return;
                    User.ApplyEffect(0);
                    User.CurrentItemEffect = ItemEffectType.None;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UpdateUserEffecterreur: " + ex);
            }
        }

        public RoomUser GetUserForSquare(int x, int y) => Enumerable.FirstOrDefault<RoomUser>((IEnumerable<RoomUser>)this._room.GetGameMap().GetRoomUsers(new Point(x, y)).OrderBy(u => u.IsBot == true));

        public RoomUser GetUserForSquareNotBot(int x, int y) => Enumerable.FirstOrDefault<RoomUser>((IEnumerable<RoomUser>)this._room.GetGameMap().GetRoomUsers(new Point(x, y)).Where(u => u.IsBot == false));

        public bool AddAvatarToRoom(GameClient Session)
        {
            if (this._room == null || Session == null || Session.GetHabbo() == null)
                return false;

            int PersonalID = this._primaryPrivateUserID++;

            RoomUser User = new RoomUser(Session.GetHabbo().Id, this._room.Id, PersonalID, this._room);
            if (User.GetClient() == null)
                return false;
            User.UserId = Session.GetHabbo().Id;
            User.IsSpectator = Session.GetHabbo().SpectatorMode;

            if (!this._users.TryAdd(PersonalID, User))
                return false;

            if (Session.GetHabbo().HasFuse("show_whispers") && !this._usersRank.Contains(User.UserId)) //quien puede ver los susurros
                this._usersRank.Add(User.UserId);

            Session.GetHabbo().CurrentRoomId = this._room.Id;
            Session.GetHabbo().LoadingRoomId = 0;

            string Username = Session.GetHabbo().Username;
            int UserId = Session.GetHabbo().Id;

            if (this._usersByUsername.ContainsKey(Username.ToLower()))
                this._usersByUsername.TryRemove(Username.ToLower(), out User);

            if (this._usersByUserID.ContainsKey(UserId))
                this._usersByUserID.TryRemove(UserId, out User);

            this._usersByUsername.TryAdd(Username.ToLower(), User);
            this._usersByUserID.TryAdd(UserId, User);

            DynamicRoomModel Model = this._room.GetGameMap().Model;
            if (Model == null)
                return false;

            User.SetPos(Model.DoorX, Model.DoorY, Model.DoorZ);
            User.SetRot(Model.DoorOrientation, false);

            if (Session.GetHabbo().IsTeleporting)
            {
                Item roomItem = this._room.GetRoomItemHandler().GetItem(User.GetClient().GetHabbo().TeleporterId);
                if (roomItem != null)
                {
                    roomItem.GetRoom().GetGameMap().TeleportToItem(User, roomItem);

                    roomItem.InteractingUser2 = Session.GetHabbo().Id;
                    roomItem.ReqUpdate(1);
                }
            }

            if (User.GetClient() != null && User.GetClient().GetHabbo() != null)
            {
                User.GetClient().GetHabbo().IsTeleporting = false;
                User.GetClient().GetHabbo().TeleporterId = 0;
                User.GetClient().GetHabbo().TeleportingRoomID = 0;
            }

            if (!User.IsSpectator)
                _room.SendPacket(new UsersComposer(User));

            if (User.IsSpectator)
            {
                List<RoomUser> roomUserByRank = _room.GetRoomUserManager().GetStaffRoomUser();
                if (roomUserByRank.Count > 0)
                {
                    foreach (RoomUser StaffUser in roomUserByRank)
                    {
                        if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetHabbo() != null && StaffUser.GetClient().GetHabbo().HasFuse("fuse_show_invisible")))
                            StaffUser.SendWhisperChat(User.GetUsername() + " quien estaba en modo invisible salió de la sala !", true);
                    }
                }
            }

            RoomRightLevels level = RoomRightLevels.NONE;

            if (_room.CheckRights(Session, true))
            {
                //Session.SendNotification("owner");
                level = RoomRightLevels.MODERATOR;
                Session.SendMessage(new YouAreOwnerComposer());
            }
            else if (_room.CheckRights(Session, false) && _room.RoomData.Group == null)
            {
                //Session.SendNotification("rights");
                level = RoomRightLevels.RIGHTS;
            }
            else if (_room.RoomData.Group != null && this._room.CheckRights(Session))
            {
                //Session.SendNotification("group");
                level = _room.RoomData.Group.getGroupRightLevel(Session.GetHabbo());
            }

            User.SetStatus("flatctrl", ((int)level).ToString());

            //Session.SendNotification(User.Statusses["flatctrl"]);

            Session.SendMessage(new YouAreControllerComposer(level));

            if (level == RoomRightLevels.NONE)
                Session.SendMessage(new YouAreNotControllerComposer());

            //if (!User.IsBot && Session.GetHabbo().Rank > 15)
            //{
            //    User.SetStatus("flatctrl", "5");
            //    Session.SendPacket((IServerPacket)new YouAreOwnerComposer());
            //    Session.SendPacket((IServerPacket)new YouAreControllerComposer(5));
            //}
            //else if (this._room.CheckRights(Session, true))
            //{
            //    User.SetStatus("flatctrl", "useradmin");
            //    Session.SendPacket((IServerPacket)new YouAreOwnerComposer());
            //    Session.SendPacket((IServerPacket)new YouAreControllerComposer(4));
            //    if (Session.GetHabbo().HasFuse("ads_background"))
            //        Session.SendPacket((IServerPacket)new UserRightsComposer(5));
            //}
            //else if (this._room.CheckRights(Session))
            //{
            //    User.SetStatus("flatctrl", "1");
            //    Session.SendPacket((IServerPacket)new YouAreControllerComposer(1));
            //}
            //else
            //{
            //    if (Session.GetHabbo().HasFuse("ads_background"))
            //        Session.SendPacket((IServerPacket)new UserRightsComposer(Session.GetHabbo().Rank));
            //    Session.SendPacket((IServerPacket)new YouAreNotControllerComposer());
            //}

            if (!User.IsBot && Session.GetHabbo().Rank > 2)
            {
                if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("ADM"))
                    User.CurrentEffect = 102;
                else if (Session.GetHabbo().Isguia || Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAGUIA"))
                    User.CurrentEffect = 187;
                else if (Session.GetHabbo().IsEMB || Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAGUIA"))
                    User.CurrentEffect = 178;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAINTER"))
                    User.CurrentEffect = 546;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAGM"))
                    User.CurrentEffect = 570;
                else if (Session.GetHabbo().GetBadgeComponent().HasBadgeSlot("PLACAMOD"))
                    User.CurrentEffect = 552;

                if (User.CurrentEffect > 0)
                    _room.SendPacket(new AvatarEffectComposer(User.VirtualId, User.CurrentEffect));
            }

            User.UpdateNeeded = true;

            foreach (RoomUser Bot in this._bots.Values.ToList())
            {
                if (Bot == null || Bot.BotAI == null)
                    continue;

                Bot.BotAI.OnUserEnterRoom(User);
            }

            if (!User.IsBot && this._room.RoomData.OwnerName != User.GetClient().GetHabbo().Username)
                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(User.GetClient(), QuestType.SOCIAL_VISIT, 0);

            if (!User.IsBot)
            {
                Session.GetHabbo().SendWebPacket(new InRoomComposer(true));

                if (Session.GetHabbo().RolePlayId > 0 && _room.RoomData.OwnerId != Session.GetHabbo().RolePlayId)
                {
                    RolePlayerManager RPManager = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Session.GetHabbo().RolePlayId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetHabbo().Id);
                        if (Rp != null)
                            RPManager.RemovePlayer(Session.GetHabbo().Id);

                    }
                    Session.GetHabbo().RolePlayId = 0;
                }

                if (_room.IsRoleplay && _room.RoomData.OwnerId != Session.GetHabbo().RolePlayId)
                {
                    RolePlayerManager RPManager = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(_room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        RolePlayer Rp = RPManager.GetPlayer(Session.GetHabbo().Id);
                        if (Rp == null)
                            RPManager.AddPlayer(Session.GetHabbo().Id);
                    }

                    Session.GetHabbo().RolePlayId = _room.RoomData.OwnerId;
                }
            }


            User.InGame = _room.IsRoleplay;

            return true;
        }



        public void RemoveUserFromRoom(GameClient Session, bool NotifyClient, bool NotifyKick)
        {
            try
            {
                if (Session == null)
                    return;

                if (Session.GetHabbo() == null)
                    return;

                if (NotifyClient)
                {
                    if (NotifyKick)
                    {
                        Session.SendPacket(new GenericErrorComposer(4008));
                    }
                    Session.SendPacket(new CloseConnectionComposer());
                }

                RoomUser User = this.GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (User == null)
                    return;

                if (this._usersRank.Contains(User.UserId))
                    this._usersRank.Remove(User.UserId);

                if (User.Team != Team.none)
                {
                    this._room.GetTeamManager().OnUserLeave(User);
                    this._room.GetGameManager().UpdateGatesTeamCounts();
                }

                if (this._room.GotJanken())
                {
                    this._room.GetJanken().RemovePlayer(User);
                }

                if (User.RidingHorse)
                {
                    User.RidingHorse = false;
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(User.HorseID);
                    if (roomUserByVirtualId != null)
                    {
                        roomUserByVirtualId.RidingHorse = false;
                        roomUserByVirtualId.HorseID = 0;
                    }
                }

                if (User.IsSit || User.IsLay)
                {
                    User.IsSit = false;
                    User.IsLay = false;
                }

                if (this._room.HasActiveTrade(Session.GetHabbo().Id))
                    this._room.TryStopTrade(Session.GetHabbo().Id);

                if (User.Roleplayer != null)
                    AkiledEnvironment.GetGame().GetRoleplayManager().GetTrocManager().RemoveTrade(User.Roleplayer.TradeId);

                if (User.IsSpectator)
                {
                    List<RoomUser> roomUserByRank = _room.GetRoomUserManager().GetStaffRoomUser();
                    if (roomUserByRank.Count > 0)
                    {
                        foreach (RoomUser StaffUser in roomUserByRank)
                        {
                            if (StaffUser != null && StaffUser.GetClient() != null && (StaffUser.GetClient().GetHabbo() != null && StaffUser.GetClient().GetHabbo().HasFuse("fuse_show_invisible")))
                                StaffUser.SendWhisperChat(User.GetUsername() + "  quien estaba en modo invisible salió de la sala", true);
                        }
                    }
                }

                Session.GetHabbo().SendWebPacket(new InRoomComposer(false));

                Session.GetHabbo().CurrentRoomId = 0;
                Session.GetHabbo().LoadingRoomId = 0;

                this._usersByUserID.TryRemove(User.UserId, out User);
                this._usersByUsername.TryRemove(Session.GetHabbo().Username.ToLower(), out User);

                this.RemoveRoomUser(User);

                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException("Error during removing user (" + Session.ConnectionID + ") from room:" + (ex).ToString());
            }
        }

        private void RemoveRoomUser(RoomUser user)
        {
            this._room.GetGameMap().RemoveTakingSquare(user.SetX, user.SetY);
            this._room.GetGameMap().RemoveUserFromMap(user, new Point(user.X, user.Y));

            this._room.SendPacket(new UserRemoveComposer(user.VirtualId));
            RoomUser toRemove;
            this._users.TryRemove(user.VirtualId, out toRemove);
        }

        public void UpdateUserCount(int count)
        {
            if (this._room.RoomData.UsersNow == count)
                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4]
                    {
                       "UPDATE rooms SET users_now = ",
                       count,
                       " WHERE id = ",
                       this._room.Id
             }));
            this._room.RoomData.UsersNow = count;
        }

        public RoomUser GetRoomUserByVirtualId(int VirtualId)
        {
            RoomUser User = null;
            if (!_users.TryGetValue(VirtualId, out User))
                return null;
            return User;
        }

        public RoomUser GetRoomUserByName(string pName)
        {
            if (this._usersByUsername.ContainsKey(pName.ToLower()))
                return (RoomUser)this._usersByUsername[pName.ToLower()];
            else
                return (RoomUser)null;
        }

        public RoomUser GetRoomUserByHabboId(int pId)
        {
            if (this._usersByUserID.ContainsKey(pId))
                return (RoomUser)this._usersByUserID[pId];
            else
                return (RoomUser)null;
        }

        public RoomUser GetRoomUserByHabbo(string pName)
        {
            if (this._usersByUsername.ContainsKey(pName.ToLower()))
                return (RoomUser)this._usersByUsername[pName.ToLower()];
            else
                return (RoomUser)null;
        }

        public RoomUser GetUserByTracker(string IPWeb, string IPClient, string MachineId)
        {
            foreach (RoomUser User in this.GetUserList())
            {
                if (User == null) continue;
                if (User.GetClient() == null) continue;
                if (User.GetClient().GetHabbo() == null) continue;
                if (User.GetClient().GetConnection() == null) continue;
                if (User.GetClient().MachineId != MachineId && User.GetClient().GetHabbo().IP != IPWeb && User.GetClient().GetConnection().getIp() != IPClient) continue;

                return User;
            }

            return null;
        }

        public List<RoomUser> GetRoomUsers()
        {
            List<RoomUser> List = new List<RoomUser>();

            List = this.GetUserList().Where(x => (!x.IsBot)).ToList();

            return List;
        }

        public List<RoomUser> GetRoomUserByRank(int minRank)
        {
            var returnList = new List<RoomUser>();
            foreach (RoomUser user in GetUserList().ToList())
            {
                if (user == null)
                    continue;

                if (!user.IsBot && user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().Rank >= minRank)
                    returnList.Add(user);
            }

            return returnList;
        }

        public ICollection<RoomUser> GetUserList() => this._users.Values;

        public RoomUser GetBotByName(string name) => this._bots.Values.FirstOrDefault(b => b.IsBot && b.BotData.Name == name);

        public RoomUser GetBotOrPetByName(string name) => _bots.Values.Concat(_pets.Values).FirstOrDefault(b => (b.IsBot && b.BotData.Name == name) || (b.IsPet && b.BotData.Name == name));


        public List<RoomUser> GetStaffRoomUser()
        {
            List<RoomUser> list = new List<RoomUser>();
            foreach (int UserId in this._usersRank)
            {
                RoomUser roomUser = this.GetRoomUserByHabboId(UserId);
                if (roomUser != null)
                    list.Add(roomUser);
            }
            return list;
        }

        public void SavePositionBots(IQueryAdapter dbClient)
        {
            List<RoomUser> Botlist = this.GetBots();
            if (Botlist.Count <= 0)
                return;

            QueryChunk queryChunk = new QueryChunk();

            foreach (RoomUser bot in Botlist)
            {
                RoomBot BotData = bot.BotData;
                if (BotData.AiType == AIType.RolePlayBot)
                    continue;
                if (bot.X != BotData.X || bot.Y != BotData.Y || bot.Z != BotData.Z || bot.RotBody != BotData.Rot)
                    queryChunk.AddQuery("UPDATE bots SET x = '" + bot.X + "', y = '" + bot.Y + "', z = '" + bot.Z + "', rotation = '" + bot.RotBody + "' WHERE id = " + bot.BotData.Id);
            }

            queryChunk.Execute(dbClient);
            queryChunk.Dispose();
        }

        public void AppendPetsUpdateString(IQueryAdapter dbClient)
        {
            List<RoomUser> Petlist = this.GetPets();
            if (Petlist.Count <= 0)
                return;
            QueryChunk queryChunk = new QueryChunk();
            QueryChunk queryChunk2 = new QueryChunk();

            foreach (RoomUser petData in Petlist)
            {
                Pet pet = petData.PetData;
                if (pet.DBState == DatabaseUpdateState.NeedsUpdate)
                {
                    queryChunk.AddParameter(pet.PetId + "name", pet.Name);
                    queryChunk.AddParameter(pet.PetId + "race", pet.Race);
                    queryChunk.AddParameter(pet.PetId + "color", pet.Color);
                    queryChunk.AddQuery("UPDATE user_pets SET room_id = " + pet.RoomId + ", name = @" + pet.PetId + "name, race = @" + pet.PetId + "race, color = @" + pet.PetId + "color, type = " + pet.Type + ", expirience = " + pet.Expirience + ", energy = " + pet.Energy + ", nutrition = " + pet.Nutrition + ", respect = " + pet.Respect + ", createstamp = '" + pet.CreationStamp + "', x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }
                else
                {
                    if (petData.BotData.AiType == AIType.RolePlayPet)
                        continue;
                    queryChunk2.AddQuery("UPDATE user_pets SET x = " + petData.X + ", Y = " + petData.Y + ", Z = " + petData.Z + " WHERE id = " + pet.PetId);
                }
                pet.DBState = DatabaseUpdateState.Updated;
            }
            queryChunk.Execute(dbClient);
            queryChunk2.Execute(dbClient);
            queryChunk.Dispose();
            queryChunk2.Dispose();
        }

        public List<RoomUser> GetBots()
        {
            List<RoomUser> Bots = new List<RoomUser>();
            foreach (RoomUser User in this._bots.Values.ToList())
            {
                if (User == null || !User.IsBot || User.IsPet)
                    continue;

                Bots.Add(User);
            }

            return Bots;
        }

        public List<RoomUser> GetPets()
        {
            List<RoomUser> Pets = new List<RoomUser>();
            foreach (RoomUser User in this._pets.Values.ToList())
            {
                if (User == null || !User.IsPet)
                    continue;

                Pets.Add(User);
            }

            return Pets;
        }

        public void SerializeStatusUpdates()
        {
            List<RoomUser> Users = new List<RoomUser>();
            ICollection<RoomUser> RoomUsers = GetUserList();

            if (RoomUsers == null)
                return;

            foreach (RoomUser User in RoomUsers.ToList())
            {
                if (User == null || !User.UpdateNeeded)
                    continue;

                User.UpdateNeeded = false;
                Users.Add(User);
            }

            if (Users.Count > 0)
                this._room.SendPacket(new UserUpdateComposer(Users));
        }

        public void UpdateUserStatusses() => this.onUserUpdateStatus();

        private void onUserUpdateStatus()
        {
            foreach (RoomUser User in this.GetUserList().ToList())
                this.UpdateUserStatus(User, false);
        }

        private bool isValid(RoomUser user) => user.IsBot || user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().CurrentRoomId == this._room.Id;

        public bool TryGetPet(int PetId, out RoomUser Pet) => this._pets.TryGetValue(PetId, out Pet);

        public bool TryGetBot(int BotId, out RoomUser Bot) => this._bots.TryGetValue(BotId, out Bot);

        public void UpdateUserStatus(RoomUser User, bool cyclegameitems)
        {
            if (User == null)
                return;

            if (User.Statusses.ContainsKey("lay") || User.Statusses.ContainsKey("sit") || User.Statusses.ContainsKey("sign"))
            {
                if (User.Statusses.ContainsKey("lay"))
                    User.RemoveStatus("lay");
                if (User.Statusses.ContainsKey("sit"))
                    User.RemoveStatus("sit");
                if (User.Statusses.ContainsKey("sign"))
                    User.RemoveStatus("sign");
                User.UpdateNeeded = true;
            }

            List<Item> roomItemForSquare = this._room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).OrderBy(p => p.GetZ).ToList();

            double newZ = !User.RidingHorse || User.IsPet ? this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) : this._room.GetGameMap().SqAbsoluteHeight(User.X, User.Y, roomItemForSquare) + 1.0;
            if (newZ != User.Z)
            {
                User.Z = newZ;
                User.UpdateNeeded = true;
            }

            foreach (Item roomItem in roomItemForSquare)
            {
                if (cyclegameitems)
                {
                    roomItem.UserWalksOnFurni(User, roomItem);

                    if (roomItem.Fx != 0 && !User.IsBot)
                        User.ApplyEffect(roomItem.Fx);
                }

                if (roomItem.GetBaseItem().IsSeat)
                {
                    if (!User.Statusses.ContainsKey("sit"))
                    {
                        User.SetStatus("sit", roomItem.Height.ToString());
                        User.IsSit = true;
                    }
                    User.Z = roomItem.GetZ;
                    User.RotHead = roomItem.Rotation;
                    User.RotBody = roomItem.Rotation;
                    User.UpdateNeeded = true;
                }

                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.bed:
                        if (!User.Statusses.ContainsKey("lay"))
                        {
                            User.SetStatus("lay", roomItem.Height.ToString() + " null");
                            User.IsLay = true;
                        }
                        User.Z = roomItem.GetZ;
                        User.RotHead = roomItem.Rotation;
                        User.RotBody = roomItem.Rotation;
                        User.UpdateNeeded = true;
                        break;
                    #region Plantas
                    case InteractionType.PLANT_SEED:
                        {
                            if (User.IsWalking && roomItem.ExtraData.Length > 0)
                            {
                                var givenHits = int.Parse(roomItem.ExtraData);
                                if (givenHits < 1 && User.CurrentEffect == 192)
                                {
                                    givenHits++;
                                    roomItem.ExtraData = givenHits.ToString();
                                    roomItem.UpdateState();

                                    if (givenHits > 5) AkiledEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(User, _room, roomItem);
                                    {
                                        givenHits = 0;
                                        roomItem.ExtraData = givenHits.ToString();
                                        roomItem.UpdateState();
                                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), "ACH_PinataWhacker", 1);
                                    }
                                }
                            }
                            break;
                        }
                    #endregion
                    case InteractionType.pressurepad:
                    case InteractionType.TRAMPOLINE:
                    case InteractionType.TREADMILL:
                    case InteractionType.CROSSTRAINER:
                        roomItem.ExtraData = "1";
                        roomItem.UpdateState(false, true);
                        break;
                    case InteractionType.GUILD_GATE:
                        roomItem.ExtraData = "1;" + roomItem.GroupId;
                        roomItem.UpdateState(false, true);
                        break;
                    case InteractionType.ARROW:
                        if (!cyclegameitems || User.IsBot)
                            break;
                        if (roomItem.InteractingUser != 0)
                            break;
                        User.CanWalk = true;
                        roomItem.InteractingUser = User.GetClient().GetHabbo().Id;
                        roomItem.ReqUpdate(2);
                        break;
                    case InteractionType.banzaigateblue:
                    case InteractionType.banzaigatered:
                    case InteractionType.banzaigateyellow:
                    case InteractionType.banzaigategreen:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.team + 32);
                            TeamManager managerForBanzai = this._room.GetTeamManager();
                            if (User.Team != roomItem.team)
                            {
                                if (User.Team != Team.none)
                                {
                                    managerForBanzai.OnUserLeave(User);
                                }
                                User.Team = roomItem.team;
                                managerForBanzai.AddUser(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect != EffectId)
                                    User.ApplyEffect(EffectId);
                            }
                            else
                            {
                                managerForBanzai.OnUserLeave(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                    User.ApplyEffect(0);
                                User.Team = Team.none;
                                continue;
                            }
                        }
                        break;
                    case InteractionType.banzaiblo:
                        if (cyclegameitems && User.Team != Team.none && !User.IsBot)
                        {
                            this._room.GetGameItemHandler().OnWalkableBanzaiBlo(User, roomItem);
                        }
                        break;
                    case InteractionType.banzaiblob:
                        if (cyclegameitems && User.Team != Team.none && !User.IsBot)
                        {
                            this._room.GetGameItemHandler().OnWalkableBanzaiBlob(User, roomItem);
                        }
                        break;
                    case InteractionType.banzaitele:
                        if (cyclegameitems)
                            this._room.GetGameItemHandler().onTeleportRoomUserEnter(User, roomItem);
                        break;
                    case InteractionType.freezeyellowgate:
                    case InteractionType.freezeredgate:
                    case InteractionType.freezegreengate:
                    case InteractionType.freezebluegate:
                        if (cyclegameitems && !User.IsBot)
                        {
                            int EffectId = ((int)roomItem.team + 39);
                            TeamManager managerForFreeze = this._room.GetTeamManager();
                            if (User.Team != roomItem.team)
                            {
                                if (User.Team != Team.none)
                                {
                                    managerForFreeze.OnUserLeave(User);
                                }
                                User.Team = roomItem.team;
                                managerForFreeze.AddUser(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect != EffectId)
                                    User.ApplyEffect(EffectId);
                            }
                            else
                            {
                                managerForFreeze.OnUserLeave(User);
                                this._room.GetGameManager().UpdateGatesTeamCounts();
                                if (User.CurrentEffect == EffectId)
                                    User.ApplyEffect(0);
                                User.Team = Team.none;
                            }
                        }
                        break;
                    case InteractionType.fbgate:
                        if (cyclegameitems || string.IsNullOrEmpty(roomItem.ExtraData) || !roomItem.ExtraData.Contains(',') || User == null || User.IsBot || User.transformation || User.IsSpectator)
                            break;

                        if (User.GetClient().GetHabbo().LastMovFGate && User.GetClient().GetHabbo().BackupGender == User.GetClient().GetHabbo().Gender)
                        {
                            User.GetClient().GetHabbo().LastMovFGate = false;
                            User.GetClient().GetHabbo().Look = User.GetClient().GetHabbo().BackupLook;
                        }
                        else
                        {
                            // mini Fix
                            string _gateLook = ((User.GetClient().GetHabbo().Gender.ToUpper() == "M") ? roomItem.ExtraData.Split(',')[0] : roomItem.ExtraData.Split(',')[1]);
                            if (_gateLook == "")
                                break;
                            string gateLook = _gateLook.Split('.').Where(part => !part.StartsWith("hd")).Aggregate("", (current, part) => current + (part + "."));
                            gateLook = gateLook.Substring(0, gateLook.Length - 1);

                            // Generating New Look.
                            string[] Parts = User.GetClient().GetHabbo().Look.Split('.');
                            string NewLook = "";
                            foreach (string Part in Parts)
                            {
                                if (/*Part.StartsWith("hd") || */Part.StartsWith("sh") || Part.StartsWith("cp") || Part.StartsWith("cc") || Part.StartsWith("ch") || Part.StartsWith("lg") || Part.StartsWith("ca") || Part.StartsWith("wa"))
                                    continue;
                                NewLook += Part + ".";
                            }
                            NewLook += gateLook;

                            User.GetClient().GetHabbo().BackupLook = User.GetClient().GetHabbo().Look;
                            User.GetClient().GetHabbo().BackupGender = User.GetClient().GetHabbo().Gender;
                            User.GetClient().GetHabbo().Look = NewLook;
                            User.GetClient().GetHabbo().LastMovFGate = true;
                        }

                        User.GetClient().SendPacket(new UserChangeComposer(User, true));

                        if (User.GetClient().GetHabbo().InRoom)
                        {
                            this._room.SendPacket(new UserChangeComposer(User, false));
                        }
                        break;
                    case InteractionType.freezetileblock:
                        if (!cyclegameitems)
                            break;
                        this._room.GetFreeze().OnWalkFreezeBlock(roomItem, User);
                        break;
                    default:
                        break;
                }
            }
            if (cyclegameitems)
            {
                this._room.GetBanzai().HandleBanzaiTiles(User.Coordinate, User.Team, User);
            }

            if (User.IsSit || User.IsLay)
            {
                if (User.IsSit)
                {
                    if (!User.Statusses.ContainsKey("sit"))
                    {
                        if (User.transformation)
                            User.SetStatus("sit", "");
                        else
                            User.SetStatus("sit", "0.5");

                        User.UpdateNeeded = true;
                    }

                }
                else if (User.IsLay)
                {

                    if (!User.Statusses.ContainsKey("lay"))
                    {
                        if (User.transformation)
                            User.SetStatus("lay", "");
                        else
                            User.SetStatus("lay", "0.7");

                        User.UpdateNeeded = true;
                    }

                }
            }
        }

        public void OnCycle()
        {
            int userCounter = 0;
            int idleCount = 0;

            List<RoomUser> ToRemove = new List<RoomUser>();

            foreach (RoomUser User in this.GetUserList().ToList())
            {
                if (!this.isValid(User))
                {
                    if (User.GetClient() != null && User.GetClient().GetHabbo() != null)
                    {
                        this.RemoveUserFromRoom(User.GetClient(), false, false);
                    }
                    else
                        this.RemoveRoomUser(User);
                }

                if (User.IsDispose)
                    continue;

                if (User.RidingHorse && User.IsPet)
                    continue;

                if (this._room.IsRoleplay)
                {
                    RolePlayerManager RPManager = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this._room.RoomData.OwnerId);
                    if (RPManager != null)
                    {
                        if (User.IsBot)
                        {
                            if (User.BotData.RoleBot != null)
                                User.BotData.RoleBot.OnCycle(User, this._room);
                        }
                        else
                        {
                            RolePlayer Rp = User.Roleplayer;
                            if (Rp != null)
                                Rp.OnCycle(User, RPManager);
                        }
                    }
                }

                User.IdleTime++;

                if (!User.IsAsleep && User.IdleTime >= 600 && !User.IsBot)
                {
                    User.IsAsleep = true;
                    this._room.SendPacket(new SleepComposer(User, true));
                }

                /*if (User.NeedsAutokick && !ToRemove.Contains(User))
                {
                    ToRemove.Add(User);
                    continue;
                }*/

                if (User.CarryItemID > 0 && User.CarryTimer > 0)
                {
                    User.CarryTimer--;
                    if (User.CarryTimer <= 0)
                        User.CarryItem(0);
                }

                if (User.UserTimer > 0)
                    User.UserTimer--;

                if (User.FreezeEndCounter > 0)
                {
                    User.FreezeEndCounter--;
                    if (User.FreezeEndCounter <= 0)
                        User.Freeze = false;
                }

                if (User.TimerResetEffect > 0)
                {
                    User.TimerResetEffect--;
                    if (User.TimerResetEffect <= 0)
                    {
                        User.ApplyEffect(User.CurrentEffect, true);
                    }
                }

                if (this._room.GotFreeze())
                    this._room.GetFreeze().CycleUser(User);

                if (User.SetStep)
                {
                    if (SetStepForUser(User))
                        continue;

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        if (SetStepForUser(roomUserByVirtualId))
                            continue;
                    }
                }
                else
                {
                    User.AllowMoveToRoller = true;
                    User.MoveWithBall = false;
                }

                if (User.IsWalking && !User.Freezed && !User.Freeze && !(this._room.FreezeRoom && (User.GetClient() != null && User.GetClient().GetHabbo().Rank < 6)))
                {
                    CalculatePath(User);

                    User.UpdateNeeded = true;
                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }
                else if (User.Statusses.ContainsKey("mv"))
                {
                    User.RemoveStatus("mv");
                    User.IsWalking = false;
                    User.UpdateNeeded = true;

                    if (User.RidingHorse && !User.IsPet)
                    {
                        RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                        roomUserByVirtualId.RemoveStatus("mv");
                        roomUserByVirtualId.IsWalking = false;
                        roomUserByVirtualId.UpdateNeeded = true;
                    }
                }

                if (User.IsBot && User.BotAI != null)
                    User.BotAI.OnTimerTick();
                else if (!User.IsSpectator)
                    userCounter++;
                //if (!updated)
                //this.UpdateUserEffect(User, User.X, User.Y);
            }
            if (userCounter == 0)
                idleCount++;

            foreach (RoomUser user in ToRemove)
            {
                GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(user.HabboId);
                if (clientByUserId != null)
                {
                    this.RemoveUserFromRoom(clientByUserId, true, false);
                }
                else
                    this.RemoveRoomUser(user);
            }
            ToRemove.Clear();

            this.UpdateUserCount(userCounter);
        }

        private void CalculatePath(RoomUser User)
        {
            Gamemap gameMap = this._room.GetGameMap();
            SquarePoint nextStep = DreamPathfinder.GetNextStep(User.X, User.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.mUserOnMap, gameMap.mSquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
            if (User.WalkSpeed)
                nextStep = DreamPathfinder.GetNextStep(nextStep.X, nextStep.Y, User.GoalX, User.GoalY, gameMap.GameMap, gameMap.ItemHeightMap, gameMap.mUserOnMap, gameMap.mSquareTaking, gameMap.Model.MapSizeX, gameMap.Model.MapSizeY, User.AllowOverride, gameMap.DiagonalEnabled, this._room.RoomData.AllowWalkthrough, gameMap.ObliqueDisable);
            if (User.GoalX == User.X && User.GoalY == User.Y)
                User.AllowBall = false;

            if (User.BreakWalkEnable && User.StopWalking)
            {
                User.StopWalking = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else if (nextStep.X == User.X && nextStep.Y == User.Y || this._room.GetGameItemHandler().CheckGroupGate(User, new Point(nextStep.X, nextStep.Y)))
            {
                User.IsWalking = false;
                User.AllowBall = false;
                this.UpdateUserStatus(User, false);
                User.RemoveStatus("mv");

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    roomUserByVirtualId.IsWalking = false;
                    this.UpdateUserStatus(roomUserByVirtualId, false);
                    roomUserByVirtualId.RemoveStatus("mv");
                }
            }
            else
            {
                HandleSetMovement(nextStep, User);

                if (User.BreakWalkEnable && !User.StopWalking)
                    User.StopWalking = true;

                if (User.RidingHorse && !User.IsPet)
                {
                    RoomUser roomUserByVirtualId = this.GetRoomUserByVirtualId(Convert.ToInt32(User.HorseID));
                    HandleSetMovement(nextStep, roomUserByVirtualId);
                    roomUserByVirtualId.UpdateNeeded = true;
                }

                if (User.IsSit)
                    User.IsSit = false;
                if (User.IsLay)
                    User.IsLay = false;

                this._room.GetSoccer().OnUserWalk(User);
                this._room.GetBanzai().OnUserWalk(User);
            }
        }

        private void HandleSetMovement(SquarePoint nextStep, RoomUser User)
        {
            int nextX = nextStep.X;
            int nextY = nextStep.Y;

            double nextZ = this._room.GetGameMap().SqAbsoluteHeight(nextX, nextY);
            if (User.RidingHorse && !User.IsPet)
                nextZ = nextZ + 1;

            User.RemoveStatus("mv");
            User.RemoveStatus("lay");
            User.RemoveStatus("sit");

            User.SetStatus("mv", nextX + "," + nextY + "," + nextZ);

            int newRot;
            if (User.FacewalkEnabled)
                newRot = User.RotBody;
            else
                newRot = Rotation.Calculate(User.X, User.Y, nextX, nextY, User.MoonwalkEnabled);

            User.RotBody = newRot;
            User.RotHead = newRot;

            User.SetStep = true;
            User.SetX = nextX;
            User.SetY = nextY;
            User.SetZ = nextZ;

            this._room.GetGameMap().AddTakingSquare(nextX, nextY);

            this.UpdateUserEffect(User, User.SetX, User.SetY);
        }

        private bool SetStepForUser(RoomUser User, bool NotUpdate = false)
        {
            this._room.GetGameMap().UpdateUserMovement(User.Coordinate, new Point(User.SetX, User.SetY), User);

            List<Item> coordinatedItems = this._room.GetGameMap().GetCoordinatedItems(new Point(User.X, User.Y)).ToList(); //Quitter la dalle


            if (User.IsBot)
            {
                RoomUser BotCollisionUser = this._room.GetGameMap().LookHasUserNearNotBot(User.X, User.Y);
                if (BotCollisionUser != null)
                    this._room.GetWiredHandler().TriggerBotCollision(BotCollisionUser, User.BotData.Name);
            }

            User.X = User.SetX;
            User.Y = User.SetY;
            User.Z = User.SetZ;

            this._room.CollisionUser(User);

            if (this._room.IsRoleplay)
            {
                RolePlayer Rp = User.Roleplayer;
                if (Rp != null && !Rp.Dead)
                {
                    ItemTemp ItemTmp = this._room.GetRoomItemHandler().GetFirstTempDrop(User.X, User.Y);
                    if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.MONEY)
                    {
                        Rp.Money += ItemTmp.Value;
                        Rp.SendUpdate();
                        if (User.GetClient() != null)
                            User.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.pickdollard", User.GetClient().Langue), ItemTmp.Value));

                        User.OnChat("*He recogido un objeto del suelo*");
                        this._room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                    else if (ItemTmp != null && ItemTmp.InteractionType == InteractionTypeTemp.RPITEM)
                    {
                        RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemTmp.Value);
                        if (RpItem != null)
                        {
                            if (!RpItem.AllowStack && Rp.GetInventoryItem(RpItem.Id) != null)
                            {
                                if (User.GetClient() != null)
                                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.itemown", User.GetClient().Langue));
                            }
                            else
                            {
                                Rp.AddInventoryItem(RpItem.Id);

                                if (User.GetClient() != null)
                                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.itempick", User.GetClient().Langue));
                            }
                        }
                        User.OnChat("*He recogido un objeto del suelo*");
                        this._room.GetRoomItemHandler().RemoveTempItem(ItemTmp.Id);
                    }
                }
            }

            foreach (Item roomItem in coordinatedItems)
            {
                roomItem.UserWalksOffFurni(User, roomItem);

                if (roomItem.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                {
                    roomItem.ExtraData = "0;" + roomItem.GroupId;
                    roomItem.UpdateState(false, true);
                }
                else if (roomItem.GetBaseItem().InteractionType == InteractionType.pressurepad
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TRAMPOLINE
                    || roomItem.GetBaseItem().InteractionType == InteractionType.TREADMILL
                    || roomItem.GetBaseItem().InteractionType == InteractionType.CROSSTRAINER)
                {
                    roomItem.ExtraData = "0";
                    roomItem.UpdateState(false, true);
                }

            }
            
            if (_room.GotSoccer())
                _room.GetSoccer().OnUserWalk(User);

            this.UpdateUserStatus(User, true);
            this._room.GetGameMap().RemoveTakingSquare(User.SetX, User.SetY);


            
            User.SetStep = false;
            User.AllowMoveToRoller = false;

            if (User.SetMoveWithBall)
            {
                User.SetMoveWithBall = false;
                User.MoveWithBall = false;
            }
            return false;
        }

        public void Destroy()
        {
            this._room = (Room)null;
            this._usersByUsername.Clear();
            this._usersByUserID.Clear();
            this.OnUserEnter = (RoomEventDelegate)null;
            this._pets.Clear();
            this._bots.Clear();
            this._users.Clear();
            this._usersRank.Clear();
        }
    }
}
