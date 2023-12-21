using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.ChatMessageStorage;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Janken;
using Akiled.HabboHotel.Rooms.Projectile;
using Akiled.HabboHotel.Rooms.RoomBots;
using Akiled.HabboHotel.Rooms.TraxMachine;
using Akiled.HabboHotel.Rooms.Wired;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms
{


    public delegate void RoomEventDelegate(object sender, EventArgs e);

    public delegate void RoomUserSaysDelegate(object sender, UserSaysArgs e, ref bool messageHandled);

    public delegate void TriggerUserDelegate(RoomUser user, string ActionType);

    public delegate void BotCollisionDelegate(RoomUser user, string BotName);

    public class Room : IDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        private readonly TimeSpan _saveFurnitureTimer = TimeSpan.FromMinutes(2);
        private DateTime _saveFurnitureTimerLast = DateTime.Now;
        public RoomRoleplay RoomRoleplay { get; }
        public bool RoomMuted
        {
            get; set;
        }
        public bool isCycling
        {
            get; set;
        }
        public int IsLagging
        {
            get; set;
        }
        public bool mCycleEnded
        {
            get; set;
        }
        public int IdleTime
        {
            get; set;
        }

        public bool IsRoleplay
        {
            get; set;
        }
        public bool Pvp
        {
            get; set;
        }
        public int RpHour
        {
            get; set;
        }
        public int RpMinute
        {
            get; set;
        }
        private int RpIntensity;
        public bool RpCycleHourEffect
        {
            get; set;
        }
        public bool RpTimeSpeed
        {
            get; set;
        }
        public Task ProcessTask { get; set; }
        private TeamManager teammanager;
        private RoomTraxManager _traxManager;
        public List<int> UsersWithRights
        {
            get; set;
        }
        public bool EveryoneGotRights
        {
            get; set;
        }
        private readonly Dictionary<int, double> Bans;
        private readonly Dictionary<int, double> Mutes;
        public bool HeightMapLoaded
        {
            get; set;
        }
        public DateTime lastTimerReset
        {
            get; set;
        }
        public GameManager game { get; set; }
        private readonly Gamemap gamemap;
        private RoomItemHandling RoomItemHandling { get; }
        private readonly RoomUserManager roomUserManager;
        private Soccer soccer;
        private BattleBanzai banzai;
        private Freeze freeze;
        private JankenManager jankan;
        private GameItemHandler gameItemHandler;
        private WiredHandler wiredHandler;
        public MoodlightData MoodlightData { get; set; }
        public List<Trade> ActiveTrades { get; set; }
        public bool mIsIdle
        {
            get; set;
        }
        private readonly ChatMessageManager chatMessageManager;
        public RoomData RoomData { get; set; }

        public bool Disposed
        {
            get; set;
        }
        public bool RoomMutePets
        {
            get; set;
        }
        public bool FreezeRoom
        {
            get; set;
        }
        public bool PushPullAllowed
        {
            get; set;
        }
        public bool CloseFullRoom
        {
            get; set;
        }
        public bool OldFoot = false;
        public bool RoomIngameChat
        {
            get; set;
        }
        public bool BurnEnabled
        {
            get; set;
        }
        public bool MatarEnabled
        {
            get; set;
        }
        public bool RobarEnabled
        {
            get; set;
        }
        public bool CrispyEnabled
        {
            get; set;
        }
        public bool GolpeEnabled
        {
            get; set;
        }
        public bool SPushEnabled
        {
            get; set;
        }
        public bool SPullEnabled
        {
            get; set;
        }
        public bool PetMorphsAllowed
        {
            get; set;
        }

        private ProjectileManager projectileManager;
        private int SaveTimer;

        private CancellationTokenSource _mainProcessSource;


        public event Room.FurnitureLoad OnFurnisLoad;

        //Question
        public int VotedYesCount
        {
            get; set;
        }
        public int VotedNoCount
        {
            get; set;
        }
        private bool _hideWired;
        private bool _processingWireds;
        private bool _processingItem;

        public int UserCount
        {
            get { return this.roomUserManager.GetRoomUserCount(); }
        }


        public int Id
        {
            get { return this.RoomData.Id; }
        }

        public event TriggerUserDelegate TriggerUser;
        public event RoomUserSaysDelegate OnUserSays;
        public event RoomEventDelegate OnUserCmd;
        public event RoomEventDelegate OnUserCls;

        public Room(RoomData Data)
        {
            _mainProcessSource = new CancellationTokenSource();

            RolePlayerManager RPManager = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(Data.OwnerId);
            if (RPManager != null)
            {
                this.IsRoleplay = true;
                this.Pvp = true;
                this.RpCycleHourEffect = true;
                this.RpTimeSpeed = false;
                this.RpHour = -1;
            }

            this.SaveTimer = 0;
            this.Disposed = false;
            this.Bans = new Dictionary<int, double>();
            this.Mutes = new Dictionary<int, double>();
            this.ActiveTrades = new List<Trade>();
            this.mCycleEnded = false;
            this.HeightMapLoaded = false;
            this.RoomData = Data;
            this.EveryoneGotRights = Data.AllowRightsOverride;
            this.IdleTime = 0;
            this.RoomMuted = false;
            this.PushPullAllowed = true;
            this.SPullEnabled = true;
            this.SPushEnabled = true;
            this.GolpeEnabled = true;
            this.BurnEnabled = true;
            this.RobarEnabled = true;
            this.MatarEnabled = true;
            this.CrispyEnabled = true;
            this.PetMorphsAllowed = true;
            this.PetMorphsAllowed = true;
            this.RoomIngameChat = false;
            this.gamemap = new Gamemap(this);
            this.RoomItemHandling = new RoomItemHandling(this);
            this.roomUserManager = new RoomUserManager(this);
            this.wiredHandler = new WiredHandler(this);
            this.projectileManager = new ProjectileManager(this);
            this.chatMessageManager = new ChatMessageManager();
            this.chatMessageManager.LoadRoomChatlogs(this.Id);
            this.LoadRights();
            this.GetRoomItemHandler().LoadFurniture();
            if (this.RoomData.OwnerName == "AkiledGames")
                this.GetRoomItemHandler().LoadFurniture(5400713);

            this.GetGameMap().GenerateMaps(true);
            this.LoadBots();
            this.InitPets();
            this.lastTimerReset = DateTime.Now;
            this._hideWired = Data.HideWired;
            StartRoomProcessing();
            StartBallProcess();
            StartWiredsProcess();


        }

        /// <summary>
        /// Starts the room processing.
        /// </summary>


        private bool _processingBall;
        internal void StartRoomProcessing()
        {
            if (_mainProcessSource == null)
            {
                return;
            }

            try
            {
                new Task(async () =>
                {
                    while (!_mainProcessSource.IsCancellationRequested)
                    {
                        try
                        {
                            var start = AkiledEnvironment.GetIUnixTimestamp();
                            await ProcessRoom();
                            var end = AkiledEnvironment.GetIUnixTimestamp();
                            var wait = 500 - (end - start);

                            if (wait <= 0)
                                continue;
                            await Task.Delay(wait);
                        }
                        catch (Exception e)
                        {
                            Logging.HandleException(e, "RoomProcessing");
                        }
                    }
                }, _mainProcessSource.Token, TaskCreationOptions.LongRunning).Start();
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "StartRoomProcess");
            }
        }

        internal void StartBallProcess()
        {
            if (_processingBall || _mainProcessSource == null) return;

            _processingBall = true;

            try
            {
                new Task(async () =>
                {
                    while ((GotSoccer() && !Disposed) && !_mainProcessSource.IsCancellationRequested)
                    {
                        var start = AkiledEnvironment.GetUnixTimestamp();
                        try
                        {
                            if (!GetSoccer().OnCycle())
                            {
                                ;
                                await Task.Delay(250);
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            Logging.LogCriticalException(e.ToString());
                        }

                        var end = AkiledEnvironment.GetUnixTimestamp() - start;
                        await Task.Delay(100);
                    }
                }, TaskCreationOptions.LongRunning).Start();
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Start ball");
            }
        }


        public Gamemap GetGameMap() => this.gamemap;

        public RoomItemHandling GetRoomItemHandler() => this.RoomItemHandling;

        public RoomUserManager GetRoomUserManager() => this.roomUserManager;

        public Soccer GetSoccer()
        {
            if (soccer == null)
            {
                soccer = new Soccer(this);
                StartBallProcess();
            }

            return this.soccer;
        }



        public TeamManager GetTeamManager()
        {
            if (this.teammanager == null)
                this.teammanager = new TeamManager();
            return this.teammanager;
        }

        public BattleBanzai GetBanzai()
        {
            if (this.banzai == null)
                this.banzai = new BattleBanzai(this);
            return this.banzai;
        }

        public Freeze GetFreeze()
        {
            if (this.freeze == null)
                this.freeze = new Freeze(this);
            return this.freeze;
        }

        public JankenManager GetJanken()
        {
            if (this.jankan == null)
                this.jankan = new JankenManager(this);
            return this.jankan;
        }

        public GameManager GetGameManager()
        {
            if (this.game == null)
                this.game = new GameManager(this);
            return this.game;
        }

        public GameItemHandler GetGameItemHandler()
        {
            if (this.gameItemHandler == null)
                this.gameItemHandler = new GameItemHandler(this);
            return this.gameItemHandler;
        }

        public WiredHandler GetWiredHandler()
        {
            if (wiredHandler != null)
                return wiredHandler;

            wiredHandler = new WiredHandler(this);
            StartWiredsProcess();

            return wiredHandler;

        }

        internal void StartWiredsProcess()
        {
            if (_processingWireds || _mainProcessSource == null) return;

            try
            {
                _processingWireds = true;

                new Task(async () =>
                {
                    while (wiredHandler != null && !_mainProcessSource.IsCancellationRequested)
                    {
                        try
                        {
                            wiredHandler.OnCycle();
                        }
                        catch (Exception e)
                        {
                            Logging.HandleException(e, "WiredProcess");
                        }

                        await Task.Delay(250);
                    }
                }, _mainProcessSource.Token, TaskCreationOptions.LongRunning).Start();
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "StartWiredProcess");
            }
        }

        public ProjectileManager GetProjectileManager()
        {
            if (this.projectileManager == null)
                this.projectileManager = new ProjectileManager(this);
            return this.projectileManager;
        }

        public bool GotSoccer() => this.soccer != null;

        public bool GotBanzai() => banzai != null;

        public bool GotFreeze() => this.freeze != null;

        public bool GotJanken() => this.jankan != null;

        public bool GotWired() => this.wiredHandler != null;

        public ChatMessageManager GetChatMessageManager()
            => this.chatMessageManager;

        public bool AllowsShous(RoomUser user, string message)
        {
            bool messageHandled = false;
            if (this.OnUserSays != null)
                this.OnUserSays(null, new UserSaysArgs(user, message), ref messageHandled);

            return messageHandled;
        }

        public void CollisionUser(RoomUser User)
        {
            if (this.OnUserCls == null) return;

            int Lenght = 1;
            int GoalX = User.X;
            int GoalY = User.Y;

            switch (User.RotBody)
            {
                case 0:
                    GoalX = User.X;
                    GoalY = User.Y - Lenght;
                    break;
                case 1:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y - Lenght;
                    break;
                case 2:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y;
                    break;
                case 3:
                    GoalX = User.X + Lenght;
                    GoalY = User.Y + Lenght;
                    break;
                case 4:
                    GoalX = User.X;
                    GoalY = User.Y + Lenght;
                    break;
                case 5:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y + Lenght;
                    break;
                case 6:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y;
                    break;
                case 7:
                    GoalX = User.X - Lenght;
                    GoalY = User.Y - Lenght;
                    break;
            }

            RoomUser UserGoal = this.GetRoomUserManager().GetUserForSquare(GoalX, GoalY);
            if (UserGoal == null)
                return;

            if (UserGoal.Team == User.Team && User.Team != Team.none)
                return;

            this.OnUserCls(UserGoal, null);
        }

        public void UserCmd(RoomUser roomUser)
        {
            if (this.OnUserCmd != null)
                this.OnUserCmd(roomUser, null);
        }

        public void ClearTags() => this.RoomData.Tags.Clear();

        public void AddTagRange(List<string> tags) => this.RoomData.Tags.AddRange(tags);

        public List<ServerPacket> HideWiredMessages(bool hideWired)
        {
            List<ServerPacket> list = new();

            var items = this.GetRoomItemHandler().GetFloor.ToList();

            foreach (var item in items)
            {
                if (!item.IsWired)
                    continue;

                if (hideWired)
                    list.Add(new ObjectRemoveMessageComposer(item.Id, 0));
                else
                    list.Add(new ObjectAddComposer(item, item.Username, item.OwnerId));
            }


            return list;
        }

        public bool HideWired
        {
            get { return this._hideWired; }
            set { this._hideWired = value; }
        }

        private void LoadBots()
        {
            DataTable table;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM bots WHERE room_id = " + this.Id);
                table = queryreactor.GetTable();
                if (table == null)
                    return;
                foreach (DataRow Row in table.Rows)
                {
                    RoomBot roomBot = new RoomBot(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]),
                        Convert.ToInt32(Row["room_id"]), (this.IsRoleplay) ? AIType.RolePlayBot : AIType.Generic,
                        (string)Row["walk_enabled"] == "1", (string)Row["name"], (string)Row["motto"],
                        (string)Row["gender"], (string)Row["look"], (int)Row["x"], (int)Row["y"], (int)Row["z"],
                        (int)Row["rotation"], (string)Row["chat_enabled"] == "1", (string)Row["chat_text"],
                        (int)Row["chat_seconds"], (string)Row["is_dancing"] == "1", (int)Row["enable"],
                        (int)Row["handitem"], Convert.ToInt32((string)Row["status"]));
                    RoomUser roomUser = this.GetRoomUserManager().DeployBot(roomBot, (Pet)null);
                    if (roomBot.IsDancing)
                        roomUser.DanceId = 3;
                }
            }
        }

        public void InitPets()
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery(
                    "SELECT id, user_id, room_id, name, type, race, color, expirience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM user_pets WHERE room_id = " +
                    this.Id);
                DataTable table = queryreactor.GetTable();
                if (table == null)
                    return;
                foreach (DataRow Row in table.Rows)
                {
                    Pet PetData = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]),
                        Convert.ToInt32(Row["room_id"]), (string)Row["name"], Convert.ToInt32(Row["type"]),
                        (string)Row["race"], (string)Row["color"], (int)Row["expirience"], (int)Row["energy"],
                        (int)Row["nutrition"], (int)Row["respect"], (double)Row["createstamp"], (int)Row["x"],
                        (int)Row["y"], (double)Row["z"], (int)Row["have_saddle"], (int)Row["hairdye"],
                        (int)Row["pethair"], (string)(Row["anyone_ride"]) == "1");
                    List<string> list = new List<string>();
                    this.roomUserManager.DeployBot(
                        new RoomBot(PetData.PetId, PetData.OwnerId, this.Id, AIType.Pet, true, PetData.Name, "", "",
                            PetData.Look, PetData.X, PetData.Y, PetData.Z, 0, false, "", 0, false, 0, 0, 0), PetData);
                }
            }
        }

        public void onRoomKick()
        {
            List<RoomUser> list = new List<RoomUser>();
            foreach (RoomUser roomUser in this.roomUserManager.GetUserList().ToList())
            {
                if (!roomUser.IsBot && !roomUser.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                {
                    this.GetRoomUserManager().RemoveUserFromRoom(roomUser.GetClient(), true, true);
                }
            }
        }

        public void OnUserSay(RoomUser User, string Message, bool Shout)
        {
            foreach (RoomUser roomUser in this.roomUserManager.GetPets().ToList())
            {
                if (Shout)
                    roomUser.BotAI.OnUserShout(User, Message);
                else
                    roomUser.BotAI.OnUserSay(User, Message);
            }
        }

        public void LoadRights()
        {
            this.UsersWithRights = new List<int>();
            DataTable dataTable = new DataTable();
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT user_id FROM room_rights WHERE room_id = " + this.RoomData.Id);
                dataTable = queryreactor.GetTable();
            }

            if (dataTable == null)
                return;
            foreach (DataRow dataRow in dataTable.Rows)
                this.UsersWithRights.Add(Convert.ToInt32(dataRow["user_id"]));
        }

        public int GetRightsLevel(GameClient Session)
        {
            if (Session == null || Session.GetHabbo() == null)
                return 0;
            if (Session.GetHabbo().Username == this.RoomData.OwnerName ||
                Session.GetHabbo().HasFuse("fuse_any_room_controller"))
                return 4;
            if (Session.GetHabbo().HasFuse("fuse_any_room_rights"))
                return 3;
            if (this.UsersWithRights.Contains(Session.GetHabbo().Id))
                return 1;
            if (this.EveryoneGotRights)
                return 1;

            return 0;
        }

        public bool CheckRights(GameClient Session, bool RequireOwnership = false)
        {
            if (Session == null || Session.GetHabbo() == null)
                return false;

            if (Session.GetHabbo().Username == this.RoomData.OwnerName ||
                Session.GetHabbo().HasFuse("fuse_any_room_controller"))
            {
                //Session.SendNotification("session u: " + Session.GetHabbo().Username + " | roomdata ownername: " + this.RoomData.OwnerName);
                //Session.SendNotification("01, fuse? " + Session.GetHabbo().HasFuse("fuse_any_room_controller") + ", owner room? " + Session.GetHabbo().Username.Equals(this.RoomData.OwnerName));
                return true;
            }

            if (!RequireOwnership)
            {
                if (Session.GetHabbo().HasFuse("fuse_any_room_rights") ||
                    this.UsersWithRights.Contains(Session.GetHabbo().Id))
                {
                    //Session.SendNotification("02");
                    return true;
                }

                //if (this.EveryoneGotRights)
                //    return true;

                if (this.RoomData.Group == null)
                    return false;

                if (this.RoomData.Group.IsAdmin(Session.GetHabbo().Id))
                {
                    //Session.SendNotification("room group admin");
                    return true;
                }

                if (this.RoomData.Group.AdminOnlyDeco == 0)
                {
                    if (this.RoomData.Group.IsAdmin(Session.GetHabbo().Id))
                    {
                        //Session.SendNotification("room group admin can deco");
                        return true;
                    }
                }
            }

            return false;
        }

        public void SendObjects(GameClient Session)
        {
            Room Room = this;

            Session.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());
            Session.SendPacket(Room.GetGameMap().Model.GetHeightmap());

            foreach (RoomUser RoomUser in roomUserManager.GetUserList().ToList())
            {
                if (RoomUser == null)
                    continue;

                if (RoomUser.IsSpectator)
                    continue;

                if (!RoomUser.IsBot && RoomUser.GetClient() == null)
                    continue;

                if (!RoomUser.IsBot && RoomUser.GetClient().GetHabbo() == null)
                    continue;

                Session.SendPacket(new UsersComposer(RoomUser));

                if (RoomUser.IsDancing)
                    Session.SendPacket(new DanceComposer(RoomUser, RoomUser.DanceId));

                if (RoomUser.IsAsleep)
                    Session.SendPacket(new SleepComposer(RoomUser, true));

                if (RoomUser.CarryItemID > 0 && RoomUser.CarryTimer > 0)
                    Session.SendPacket(new CarryObjectComposer(RoomUser.VirtualId, RoomUser.CarryItemID));

                if (RoomUser.CurrentEffect > 0)
                    Session.SendPacket(new AvatarEffectComposer(RoomUser.VirtualId, RoomUser.CurrentEffect));
            }

            Session.SendPacket(new UserUpdateComposer(roomUserManager.GetUserList().ToList()));
            Session.SendPacket(new ObjectsComposer(Room.GetRoomItemHandler().GetFloor.ToArray(), this));
            Session.SendPacket(new ObjectsComposer(Room.GetRoomItemHandler().GetTempItems.ToArray(), this));
            Session.SendPacket(new ItemsComposer(Room.GetRoomItemHandler().GetWall.ToArray(), this));
        }

        public Task ProcessRoom()
        {

            if (this.Disposed)
                return Task.CompletedTask;

            try
            {
                var timeStarted = DateTime.Now;
                this.GetRoomUserManager().OnCycle();

                this.GetRoomItemHandler().OnCycle();

                this.RoomRoleplay?.OnCycle();

                this.GetGameItemHandler().OnCycle();

                this.GetProjectileManager().OnCycle();

                if (GetRoomUserManager().GetRoomUsers().Count == 0)
                    IdleTime++;
                else if (IdleTime > 0)
                    IdleTime = 0;

                if (!this.mCycleEnded)
                {
                    if (this.IdleTime >= 60)
                    {
                        AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(this);
                        this.mIsIdle = false;
                        return Task.CompletedTask;
                    }
                    else
                    {
                        this.GetRoomUserManager().SerializeStatusUpdates();
                    }
                }



                if (timeStarted > this._saveFurnitureTimerLast + this._saveFurnitureTimer)
                {
                    this._saveFurnitureTimerLast = timeStarted;

                    this.RoomItemHandling.SaveFurniture();
                }

                var timeEnded = DateTime.Now;

                var timeExecution = timeEnded - timeStarted;
            }
            catch (Exception ex)
            {
                this.OnRoomCrash(ex);
            }
            return Task.CompletedTask;
        }





        private void RpCycleHour()
        {
            if (!this.IsRoleplay)
                return;

            DateTime Now = DateTime.Now;

            int RpHourNow =
                (int)Math.Floor((double)(((Now.Minute * 60) + Now.Second) / 150)); //150sec = 2m30s = 1heure dans le rp

            int RpMinuteNow = (int)Math.Floor((((Now.Minute * 60) + Now.Second) - (RpHourNow * 150)) / 2.5);

            if (RpHourNow >= 16)
                RpHourNow = (RpHourNow + 8) - 24;
            else
                RpHourNow = RpHourNow + 8;

            if (this.RpTimeSpeed)
                RpHourNow = (int)Math.Floor((double)(Now.Second / 2.5));

            if (this.RpMinute != RpMinuteNow)
                this.RpMinute = RpMinuteNow;

            if (this.RpHour == RpHourNow)
                return;

            this.RpHour = RpHourNow;

            if (!this.RpCycleHourEffect)
                return;

            int Intensity = 255;

            if (RpHour >= 8 && RpHour < 20) //Journée
            {
                Intensity = 255;
            }
            else if (RpHour >= 20 && RpHour < 21) //Crépuscule
            {
                Intensity = 200;
            }
            else if (RpHour >= 21 && RpHour < 22) //Crépuscule
            {
                Intensity = 150;
            }
            else if (RpHour >= 22 && RpHour < 23) //Crépuscule
            {
                Intensity = 100;
            }
            else if (RpHour >= 23 && RpHour < 24) //Crépuscule
            {
                Intensity = 75;
            }
            else if (RpHour >= 0 && RpHour < 4) //Nuit
            {
                Intensity = 50;
            }
            else if (RpHour >= 4 && RpHour < 5) //Aube
            {
                Intensity = 75;
            }
            else if (RpHour >= 5 && RpHour < 6) //Aube
            {
                Intensity = 100;
            }
            else if (RpHour >= 6 && RpHour < 7) //Aube
            {
                Intensity = 150;
            }
            else if (RpHour >= 7 && RpHour < 8) //Aube
            {
                Intensity = 200;
            }

            if (this.RpIntensity == Intensity)
                return;

            this.RpIntensity = Intensity;

            this.UpdateRpMoodLight();
            this.UpdateRpToner();
            this.UpdateRpBlock();
        }

        private void UpdateRpBlock()
        {
            List<Item> roomItems =
                this.GetRoomItemHandler().GetFloor.Where(i => i.GetBaseItem().Id == 99138022).ToList();
            if (roomItems == null)
                return;

            int UseNum = 0;
            if (this.RpIntensity == 50)
                UseNum = 0;
            else if (this.RpIntensity == 75)
                UseNum = 1;
            else if (this.RpIntensity == 100)
                UseNum = 2;
            else if (this.RpIntensity == 150)
                UseNum = 3;
            else if (this.RpIntensity == 200)
                UseNum = 4;
            else if (this.RpIntensity == 255)
                UseNum = 5;

            foreach (Item RoomItem in roomItems)
            {
                RoomItem.ExtraData = UseNum.ToString();
                RoomItem.UpdateState();
            }
        }

        private void UpdateRpMoodLight()
        {
            if (this.MoodlightData == null)
                return;

            Item roomItem = this.GetRoomItemHandler().GetItem(this.MoodlightData.ItemId);
            if (roomItem == null || roomItem.GetBaseItem().InteractionType != InteractionType.MOODLIGHT)
                return;

            this.MoodlightData.Enabled = true;
            this.MoodlightData.CurrentPreset = 1;
            this.MoodlightData.UpdatePreset(1, "#000000", this.RpIntensity, false);
            roomItem.ExtraData = this.MoodlightData.GenerateExtraData();
            roomItem.UpdateState();
        }

        private void UpdateRpToner()
        {
            Item roomItem = Enumerable.FirstOrDefault<Item>((IEnumerable<Item>)this.GetRoomItemHandler().GetFloor
                .Where(i => i.GetBaseItem().InteractionType == InteractionType.TONER));
            if (roomItem == null)
                return;

            int Teinte = 135;
            int Saturation = 180;
            int Luminosite = (int)Math.Floor((double)this.RpIntensity / 2);
            roomItem.ExtraData = "on," + Teinte + "," + Saturation + "," + Luminosite;
            roomItem.UpdateState(true, true);
        }

        public void OnRoomCrash(Exception e) =>
            Logging.LogThreadException((e).ToString(),
                "Room cycle task for room " + this.Id); //AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(this);

        public void SendPacketOnChat(IServerPacket Message, RoomUser ThisUser = null, bool UserMutedOnly = false,
            bool UserNotIngameOnly = false)
        {
            try
            {
                if (Message == null)
                    return;

                if (this == null || this.roomUserManager == null)
                    return;

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                    return;

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null ||
                        User.GetClient().GetHabbo() == null)
                        continue;

                    if (UserMutedOnly && ThisUser != null &&
                        User.GetClient().GetHabbo().MutedUsers.Contains(ThisUser.UserId))
                        continue;

                    if (ThisUser != null && ThisUser.GetClient() != null && ThisUser.GetClient().GetHabbo() != null &&
                        ThisUser.GetClient().GetHabbo().IgnoreAll && ThisUser != User)
                        continue;

                    if (!UserMutedOnly && ThisUser == User)
                        continue;

                    if (this.RoomIngameChat && (UserNotIngameOnly && User.Team != Team.none))
                        continue;

                    if (this.RoomData.ChatMaxDistance > 0 &&
                        (Math.Abs(ThisUser.X - User.X) > this.RoomData.ChatMaxDistance ||
                         Math.Abs(ThisUser.Y - User.Y) > this.RoomData.ChatMaxDistance))
                        continue;

                    User.GetClient().SendPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
            }
        }

        public void SendPacketWeb(IServerPacket Message)
        {
            try
            {
                if (Message == null)
                    return;

                if (this == null || this.roomUserManager == null)
                    return;

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                    return;

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                        continue;

                    User.GetClient().GetHabbo().SendWebPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessageWeb (" + this.Id + ")");
            }
        }

        public void SendPacket(IServerPacket Message, bool UsersWithRightsOnly = false)
        {
            try
            {
                if (Message == null)
                    return;

                if (this == null || this.roomUserManager == null)
                    return;

                List<RoomUser> Users = this.roomUserManager.GetUserList().ToList();
                if (Users == null)
                    return;

                foreach (RoomUser User in Users)
                {
                    if (User == null || User.IsBot)
                        continue;

                    if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                        continue;

                    if (UsersWithRightsOnly && !this.CheckRights(User.GetClient()))
                        continue;

                    User.GetClient().SendPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessage (" + this.Id + ")");
            }
        }

        public void SendMessage(List<ServerPacket> Messages)
        {
            if (Messages.Count == 0)
                return;

            try
            {
                byte[] TotalBytes = new byte[0];
                int Current = 0;

                foreach (ServerPacket Packet in Messages.ToList())
                {
                    byte[] ToAdd = Packet.GetBytes();
                    int NewLen = TotalBytes.Length + ToAdd.Length;

                    Array.Resize(ref TotalBytes, NewLen);

                    for (int i = 0; i < ToAdd.Length; i++)
                    {
                        TotalBytes[Current] = ToAdd[i];
                        Current++;
                    }
                }

                this.BroadcastPacket(TotalBytes);
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Room.SendMessage List<ServerPacket>");
            }
        }

        public void BroadcastPacket(byte[] Packet)
        {
            foreach (RoomUser User in this.roomUserManager.GetUserList().ToList())
            {
                if (User == null || User.IsBot)
                    continue;

                if (User.GetClient() == null || User.GetClient().GetConnection() == null)
                    continue;

                User.GetClient().GetConnection().SendData(Packet);
            }
        }

        public void Dispose()
        {

            if (this.Disposed)
                return;

            this.SendPacket(new CloseConnectionComposer());

            _mainProcessSource?.Cancel();

            this.Disposed = true;
            this.mCycleEnded = true;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.GetRoomItemHandler().SaveFurniture();
            }

            this.RoomData.Tags.Clear();

            this.UsersWithRights.Clear();
            this.Bans.Clear();
            foreach (Item roomItem in this.GetRoomItemHandler().GetWallAndFloor)
                roomItem.Destroy();

            this.GetRoomItemHandler().Destroy();

            this.ActiveTrades.Clear();

            this.GetRoomUserManager().UpdateUserCount(0);
            this.GetRoomUserManager().Destroy();

            this.gamemap.Destroy();

            new Task(async () =>
            {
                await Task.Delay(2500);
                _mainProcessSource?.Dispose();
                _mainProcessSource = null;
            }).Start();
        }

        public Dictionary<int, double> getBans() => this.Bans;

        public bool UserIsBanned(int pId) => this.Bans.ContainsKey(pId);

        public void RemoveBan(int pId) => this.Bans.Remove(pId);

        public void AddBan(int pId, int Time)
        {
            if (this.Bans.ContainsKey(pId))
                return;
            this.Bans.Add(pId, (double)(AkiledEnvironment.GetUnixTimestamp() + Time));
        }

        public bool HasBanExpired(int pId) => !this.UserIsBanned(pId) ||
                                              this.Bans[pId] - (double)AkiledEnvironment.GetUnixTimestamp() <= 0.0;

        public Dictionary<int, double> getMute() => this.Mutes;

        public bool UserIsMuted(int pId) => this.Mutes.ContainsKey(pId);

        public void RemoveMute(int pId) => this.Mutes.Remove(pId);

        public void AddMute(int pId, int Time)
        {
            if (this.Mutes.ContainsKey(pId))
                return;
            this.Mutes.Add(pId, (double)(AkiledEnvironment.GetUnixTimestamp() + Time));
        }

        public bool HasMuteExpired(int pId) => !this.UserIsMuted(pId) ||
                                               this.Mutes[pId] - (double)AkiledEnvironment.GetUnixTimestamp() <= 0.0;

        public RoomTraxManager GetTraxManager() => this._traxManager;

        public bool HasActiveTrade(RoomUser User)
        {
            if (User.IsBot)
                return false;
            else
                return this.HasActiveTrade(User.GetClient().GetHabbo().Id);
        }

        public bool HasActiveTrade(int UserId)
        {
            foreach (Trade trade in this.ActiveTrades)
            {
                if (trade.ContainsUser(UserId))
                    return true;
            }

            return false;
        }

        public Trade GetUserTrade(int UserId)
        {
            foreach (Trade trade in this.ActiveTrades)
            {
                if (trade.ContainsUser(UserId))
                    return trade;
            }

            return (Trade)null;
        }

        public void TryStartTrade(RoomUser UserOne, RoomUser UserTwo)
        {
            if (UserOne == null || UserTwo == null)
                return;
            if ((UserOne.IsBot || UserTwo.IsBot) || (UserOne.IsTrading || UserTwo.IsTrading ||
                                                     (this.HasActiveTrade(UserOne) || this.HasActiveTrade(UserTwo))))
                return;

            this.ActiveTrades.Add(new Trade(UserOne.GetClient().GetHabbo().Id, UserTwo.GetClient().GetHabbo().Id,
                this.Id));
        }

        public void TryStopTrade(int UserId)
        {
            Trade userTrade = this.GetUserTrade(UserId);
            if (userTrade == null)
                return;
            userTrade.CloseTrade(UserId);
            this.ActiveTrades.Remove(userTrade);
        }

        public Task RunTask(Func<Task> callBack)
        {
            var task = Task.Run(async () =>
            {
                if (this.Disposed)
                {
                    return;
                }

                await callBack();

            }, this._cancellationTokenSource.Token);

            return task;
        }

        public void SetMaxUsers(int MaxUsers)
        {
            this.RoomData.UsersMax = MaxUsers;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4]
                {
                    "UPDATE rooms SET users_max = ",
                    MaxUsers,
                    " WHERE id = ",
                    this.Id
                }));
        }

        public delegate void FurnitureLoad();
    }

}