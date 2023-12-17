using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Achievements;
using Akiled.HabboHotel.ChatMessageStorage;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users.Badges;
using Akiled.HabboHotel.Users.Clothing;
using Akiled.HabboHotel.Users.Ignores;
using Akiled.HabboHotel.Users.Inventory;
using Akiled.HabboHotel.Users.Messenger;
using Akiled.HabboHotel.WebClients;
using AkiledEmulator.HabboHotel.Camera;
using JNogueira.Discord.Webhook.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Users
{
    public class Habbo
    {
        public bool forceOpenGift;
        public int forceUse = -1;
        public int forceRot = -1;
        public int ChatColor = -1;
        public int Id;
        public string Username;
        public string Prefix;
        public string Prefixnamecolor;
        public string PrefixSize;
        public int Rank;
        public string Motto;
        public string Look;
        public string BackupLook;
        public string Gender;
        public string BackupGender;
        public bool LastMovFGate;
        public int Credits;
        public int AkiledPoints;
        public int AccountCreated;
        public int AchievementPoints;
        public int Duckets;
        public double LastActivityPointsUpdate;
        public int Respect;
        public int DailyRespectPoints;
        public int DailyPetRespectPoints;
        public int CurrentRoomId;
        public int LoadingRoomId;
        public int HomeRoom;
        public int LastOnline;
        public bool IsTeleporting;
        public int TeleportingRoomID;
        public int TeleporterId;
        public int FloodCount;
        public DateTime FloodTime;
        public List<int> ClientVolume;
        public string MachineId;
        public Language Langue;
        public bool PickupItemsStatus = false;
        public int GamePointsMonth { get; set; }
        //Collector Park
        public bool collecting = false;
        public double nextReward = 0;
        public int timeWaitReward = 0;
        public double nextMovementCollector = 0;

        public List<RoomData> RoomRightsList;
        public List<RoomData> FavoriteRooms;
        public List<RoomData> UsersRooms;
        private RoomUserManager roomUserManager;
        public List<int> MutedUsers;
        public Dictionary<string, UserAchievement> Achievements;
        public List<int> RatedRooms;
        private HabboMessenger Messenger;
        private BadgeComponent BadgeComponent;
        private InventoryComponent InventoryComponent;
        private ChatMessageManager chatMessageManager;
        private GameClient mClient;
        private LowMoney _process;
        public bool SpectatorMode;
        public bool Disconnected;
        public bool HasFriendRequestsDisabled;
        public int FavouriteGroupId;
        public List<int> MyGroups;
        internal static MessengerBuddy MessengerStaff;
        public bool spamEnable;
        public int spamProtectionTime;
        public DateTime spamFloodTime;

        public Dictionary<int, int> quests;
        public int CurrentQuestId;
        public int LastCompleted;
        public int LastQuestId;
        private bool HabboinfoSaved;
        public bool AcceptTrading;
        public bool HideInRoom;
        public int PubDectectCount = 0;
        public DateTime OnlineTime;
        public bool PremiumProtect;
        public int ControlUserId;
        public string IP;
        public bool ViewMurmur = true;
        public bool HideOnline;
        public string LastPhotoId;

        public int GuideOtherUserId;
        public bool OnDuty;

        public int Mazo;
        public int MazoHighScore;

        public bool NewUser;
        public bool Nuxenable;
        public bool Nuxenable2;
        public bool Ismod;
        public bool Isinter;
        public bool Ispub;
        public bool Isgm;
        public bool Isguia;
        public bool IsEMB;
        public int PassedNuxCount;

        public bool AllowDoorBell;
        public bool CanChangeName;
        public int GiftPurchasingWarnings;
        public bool SessionGiftBlocked;

        public int RolePlayId;

        public bool IgnoreAll;
        private DateTime _timeCached;
        public string _guardar;
        public string _sexWith;
        public int _guardar2;
        public bool casinoEnabled;
        public int casinoCount;
        private int _lastMarkedFriend;
        public int last_sms;
        public int last_kill;
        public int last_kiss;
        public int last_fumar;
        public int last_sex;
        public int last_burn;
        public int last_dadosalert;
        public int last_golpe;
        public int last_pay;
        public int last_oleada;
        public int last_rp;
        public int last_eha;
        public int last_dj;
        public int last_hal;
        public int last_fish;
        public int last_mina;
        public int last_pickupitem = 0;
        private DateTime _lastClothingUpdateTime;
        private int _creditsTickUpdate;
        private int _calendarCounter;
        private int _customBubbleId;
        public DateTime LastGiftPurchaseTime;
        private double _timeMuted;
        private string _lastdailycredits;
        private bool _allowPetSpeech;
        private bool _allowBotSpeech;
        public bool isLoggedIn = false;
        private int _petId;
        public JSONCamera lastPhotoPreview;
        public string _lastPhotoPreview;
        private int _clothingUpdateWarnings;
        private bool _sessionClothingBlocked;
        private ClothingComponent _clothing;
        private bool _ViewInventory;
        public bool onfarm = false;
        public bool hasschaufel = false;
        public int angelpass;
        public int miningpass;
        public bool is_angeln = false;
        public bool is_mining = false;
        public int wurm = -1;
        public int explosivo = -1;
        public int porbora = -1;
        public int angelstatus;
        public bool hasangel = false;
        public bool hasdetector = false;
        public bool haspico = false;
        public int async_angel_id = 0;
        public bool luckmachine = false;
        public string last_premiumbonus;
        public bool[] calendarGift;
        internal double DiamondsCycleUpdate;
        internal double MoedasCycleUpdate;
        private IgnoresComponent _ignores;
 

        public bool InRoom
        {
            get
            {
                return this.CurrentRoomId >= 1;
            }
        }

        public Room CurrentRoom
        {
            get
            {
                if (this.CurrentRoomId <= 0)
                    return (Room)null;
                else
                    return AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.CurrentRoomId);
            }
        }

        public bool SendWebPacket(IServerPacket Message)
        {
            WebClient ClientWeb = AkiledEnvironment.GetGame().GetClientWebManager().GetClientByUserID(this.Id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
                return true;
            }
            return false;
        }
        public bool CacheExpired() => (DateTime.Now - this._timeCached).TotalMinutes >= 30.0;
        public bool InitProcess()
        {
            this._process = new LowMoney();
            return this._process.Init(this);
        }



        public string GetQueryString
        {
            get
            {
                TimeSpan TimeOnline = DateTime.Now - this.OnlineTime;
                int TimeOnlineSec = (int)TimeOnline.TotalSeconds;
                this.HabboinfoSaved = true;

                return "UPDATE users SET users.online = '0', users.last_online = '" + AkiledEnvironment.GetUnixTimestamp() + "', activity_points = '" + this.Duckets + "', activity_points_lastupdate = '" + this.LastActivityPointsUpdate + "', credits = '" + this.Credits + "' WHERE id = '" + this.Id + "'; " +
                    "UPDATE user_stats SET groupid = " + this.FavouriteGroupId + ", OnlineTime = OnlineTime + " + TimeOnlineSec + ", quest_id = '" + this.CurrentQuestId + "', Respect = '" + this.Respect + "', DailyRespectPoints = '" + this.DailyRespectPoints + "', DailyPetRespectPoints = '" + this.DailyPetRespectPoints + "' WHERE id = " + this.Id + ";";
            }
        }

        public Habbo(int Id, string Username, string Prefix, string Prefixnamecolor,
      string PrefixSize, int Rank, string Motto, string Look, string Gender, int Credits,
            int WPoint, int ActivityPoints, double LastActivityPointsUpdate, int HomeRoom, int Respect, int DailyRespectPoints,
            int DailyPetRespectPoints, bool HasFriendRequestsDisabled, int currentQuestID, int achievementPoints,
            int LastOnline, int FavoriteGroup, int accountCreated, bool accepttrading, string ip, bool HideInroom,
            bool HideOnline, int MazoHighScore, int Mazo, string clientVolume, bool nuxenable, string MachineId, bool ChangeName, Language Langue, bool IgnoreAll, bool PetsMuted, bool BotsMuted, int last_marked_friend, double TimeMuted, string lastdailycredits, bool nuxenable2, bool ismod, bool ispub, bool isinter, bool isgm, bool isguia, bool isemb, int angelpass,
      int angelstatus,
      int miningpass, int gamePointsMonth)
        {
            this.Id = Id;
            this.Username = Username;
            this.Prefix = Prefix;
            this.Prefixnamecolor = Prefixnamecolor;
            this.PrefixSize = PrefixSize;
            this.Motto = Motto;
            this.MachineId = MachineId;
            this.Look = Look;
            this.Gender = Gender.ToLower();
            this.Rank = Rank;
            this.Credits = Credits;
            this.AkiledPoints = WPoint;
            this.Duckets = ActivityPoints;
            this.GamePointsMonth = gamePointsMonth;
            this.AchievementPoints = achievementPoints;
            this.LastActivityPointsUpdate = LastActivityPointsUpdate;
            this.CurrentRoomId = 0;
            this.LoadingRoomId = 0;
            this.HomeRoom = (HomeRoom == 0) ? 0 : HomeRoom;
            this.FavoriteRooms = new List<RoomData>();
            this.MutedUsers = new List<int>();
            this.Achievements = new Dictionary<string, UserAchievement>();
            this.RatedRooms = new List<int>();
            this.Respect = Respect;
            this.DailyRespectPoints = DailyRespectPoints;
            this.DailyPetRespectPoints = DailyPetRespectPoints;
            this.IsTeleporting = false;
            this.TeleporterId = 0;
            this.UsersRooms = new List<RoomData>();
            this.HasFriendRequestsDisabled = HasFriendRequestsDisabled;
            this.ClientVolume = new List<int>(3);
            this.CanChangeName = ChangeName;
            this.Langue = Langue;
            this.IgnoreAll = IgnoreAll;
            this._guardar = "";
            this._sexWith = "";
            this._guardar2 = 0;
            this._allowPetSpeech = PetsMuted;
            this._allowBotSpeech = BotsMuted;
            this._lastClothingUpdateTime = DateTime.Now;
            this._clothingUpdateWarnings = 0;
            this._sessionClothingBlocked = false;

            if (clientVolume.Contains(','))
            {
                foreach (string Str in clientVolume.Split(','))
                {
                    int Val = 0;
                    if (int.TryParse(Str, out Val))
                        this.ClientVolume.Add(int.Parse(Str));
                    else
                        this.ClientVolume.Add(100);
                }
            }
            else
            {
                this.ClientVolume.Add(100);
                this.ClientVolume.Add(100);
                this.ClientVolume.Add(100);
            }

            this.LastOnline = LastOnline;
            this.MyGroups = new List<int>();
            this.FavouriteGroupId = FavoriteGroup;

            this.AccountCreated = accountCreated;

            this.CurrentQuestId = currentQuestID;
            this.AcceptTrading = accepttrading;

            this.OnlineTime = DateTime.Now;
            this.PremiumProtect = (this.Rank > 1);

            this.ControlUserId = 0;
            this.IP = ip;
            this.SpectatorMode = false;
            this.Disconnected = false;
            this.HideInRoom = HideInroom;
            this.HideOnline = HideOnline;
            this.MazoHighScore = MazoHighScore;
            this.Mazo = Mazo;

            this.LastGiftPurchaseTime = DateTime.Now;

            this.Nuxenable = nuxenable;
            this.Nuxenable2 = nuxenable2;
            this.NewUser = nuxenable;

            this.Isguia = isguia;
            this.Isinter = isinter;
            this.Ismod = ismod;
            this.Isgm = isgm;
            this.Ispub = ispub;
            this.IsEMB = isemb;
            this.angelstatus = angelstatus;
            this.angelpass = angelpass;
            this.miningpass = miningpass;
            this._lastMarkedFriend = last_marked_friend;
            this._timeMuted = TimeMuted;
            this._lastdailycredits = lastdailycredits;
            this._creditsTickUpdate = 30;
            this._calendarCounter = 0;
            this.DiamondsCycleUpdate = AkiledEnvironment.GetUnixTimestamp();
            this.MoedasCycleUpdate = AkiledEnvironment.GetUnixTimestamp();


        }


        public int CustomBubbleId
        {
            get { return this._customBubbleId; }
            set { this._customBubbleId = value; }
        }

        public int PetId
        {
            get { return this._petId; }
            set { this._petId = value; }
        }

        public string guardar
        {
            get { return this._guardar; }
            set { this._guardar = value; }
        }

        public int guardar2
        {
            get { return this._guardar2; }
            set { this._guardar2 = value; }
        }

        public string sexWith
        {
            get { return this._sexWith; }
            set { this._sexWith = value; }
        }

        public bool ViewInventory
        {
            get { return this._ViewInventory; }
            set { this._ViewInventory = value; }
        }

        public bool AllowPetSpeech
        {
            get { return this._allowPetSpeech; }
            set { this._allowPetSpeech = value; }
        }

        public bool AllowBotSpeech
        {
            get { return this._allowBotSpeech; }
            set { this._allowBotSpeech = value; }
        }

        public int LastMarkedFriend
        {
            get
            {
                return _lastMarkedFriend;
            }
            set
            {
                _lastMarkedFriend = value;
            }
        }
        public string lastdailycredits
        {
            get
            {
                return _lastdailycredits;
            }
            set
            {
                _lastdailycredits = value;
            }
        }
        public DateTime LastClothingUpdateTime
        {
            get { return _lastClothingUpdateTime; }
            set { _lastClothingUpdateTime = value; }
        }
        public int ClothingUpdateWarnings
        {
            get { return _clothingUpdateWarnings; }
            set { _clothingUpdateWarnings = value; }
        }
        public double TimeMuted
        {
            get
            {
                return _timeMuted;
            }
            set
            {
                _timeMuted = value;
            }
        }

        public int CreditsUpdateTick
        {
            get
            {
                return _creditsTickUpdate;
            }
            set
            {
                _creditsTickUpdate = value;
            }
        }

        public int CalendarCounter
        {
            get
            {
                return _calendarCounter;
            }
            set
            {
                _calendarCounter = value;
            }
        }

        public void Init(GameClient client, UserData.UserData data)
        {
            this.mClient = client;
            this.BadgeComponent = new BadgeComponent(this.Id, data.badges);
            this.InventoryComponent = new InventoryComponent(this.Id, client);
            this.InventoryComponent.SetActiveState(client);
            this.quests = data.quests;
            this.chatMessageManager = new ChatMessageManager();
            this.chatMessageManager.LoadUserChatlogs(this.Id);
            this.Messenger = new HabboMessenger(this.Id);
            this.Messenger.AppearOffline = this.HideOnline;
            this.MyGroups = data.MyGroups;
            this.Messenger.Init(data.friends, data.requests, data.Relationships);
            this.UpdateRooms();
            this.InitClothing();
        }
        public IgnoresComponent GetIgnores()
        {
            return _ignores;
        }


        public void UpdateRooms()
        {
            try
            {
                this.UsersRooms.Clear();

                DataTable table;
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("SELECT * FROM rooms WHERE owner = @name ORDER BY id ASC");
                    queryreactor.AddParameter("name", this.Username);
                    table = queryreactor.GetTable();
                }

                foreach (DataRow dRow in table.Rows)
                    this.UsersRooms.Add(AkiledEnvironment.GetGame().GetRoomManager().FetchRoomData(Convert.ToInt32(dRow["id"]), dRow));
            }

            catch (Exception ex)
            {
                Logging.LogCriticalException("Bug while updating own rooms: " + (ex).ToString());
            }
        }

        public void PrepareRoom(int Id, string Password = "", bool override_doorbell = false)
        {
            if (this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return;

            if (this.GetClient().GetHabbo().InRoom)
            {
                Room OldRoom = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.GetClient().GetHabbo().CurrentRoomId);

                if (OldRoom != null)
                {
                    OldRoom.GetRoomUserManager().RemoveUserFromRoom(this.GetClient(), false, false);
                }
            }

            Room room = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(Id);
            if (room == null)
            {
                this.GetClient().SendPacket(new CloseConnectionComposer());
                return;
            }
            if (room.mIsIdle)
                return;

            if (this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID != Id)
            {
                this.GetClient().GetHabbo().TeleportingRoomID = 0;
                this.GetClient().GetHabbo().IsTeleporting = false;
                this.GetClient().GetHabbo().TeleporterId = 0;
                this.GetClient().SendPacket(new CloseConnectionComposer());

                return;
            }

            if (!this.GetClient().GetHabbo().HasFuse("fuse_mod") && room.UserIsBanned(this.GetClient().GetHabbo().Id))
            {
                if (room.HasBanExpired(this.GetClient().GetHabbo().Id))
                {
                    room.RemoveBan(this.GetClient().GetHabbo().Id);
                }
                else
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            if (room.RoomData.UsersNow >= room.RoomData.UsersMax && !this.GetClient().GetHabbo().HasFuse("fuse_enter_full_rooms") && !AkiledEnvironment.GetGame().GetRoleManager().RankHasRight(this.GetClient().GetHabbo().Rank, "fuse_enter_full_rooms"))
            {
                if (room.CloseFullRoom)
                {
                    room.RoomData.State = 1;
                    room.CloseFullRoom = false;
                }

                if (this.GetClient().GetHabbo().Id != room.RoomData.OwnerId)
                {
                    this.GetClient().SendPacket(new CantConnectComposer(1));

                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            string[] OwnerEnterNotAllowed = { "AkiledGames", "SalasPublicas", "WorldRunOff1", "SeasonRunOff1" };

            if (!this.GetClient().GetHabbo().HasFuse("fuse_mod"))
            {
                if (!(this.GetClient().GetHabbo().HasFuse("fuse_enter_any_room") && OwnerEnterNotAllowed.All(x => x != room.RoomData.OwnerName)) && !room.CheckRights(this.GetClient(), true) && !(this.GetClient().GetHabbo().IsTeleporting && this.GetClient().GetHabbo().TeleportingRoomID == room.Id))
                {
                    if (room.RoomData.State == 1 && (!override_doorbell && !room.CheckRights(this.GetClient())))
                    {
                        if (room.UserCount == 0)
                        {
                            ServerPacket message = new ServerPacket(ServerPacketHeader.FlatAccessDeniedMessageComposer);
                            this.GetClient().SendPacket(message);
                        }
                        else
                        {
                            this.GetClient().SendPacket(new DoorbellComposer(""));
                            room.SendPacket(new DoorbellComposer(this.GetClient().GetHabbo().Username), true);
                            this.GetClient().GetHabbo().LoadingRoomId = Id;
                            this.GetClient().GetHabbo().AllowDoorBell = false;
                        }

                        return;
                    }
                    else if (room.RoomData.State == 2 && Password.ToLower() != room.RoomData.Password.ToLower())
                    {
                        this.GetClient().SendPacket(new GenericErrorComposer(-100002));
                        this.GetClient().SendPacket(new CloseConnectionComposer());
                        return;
                    }
                }
            }

            if (room.RoomData.OwnerName == "AkiledGames")
            {
                if (room.GetRoomUserManager().GetUserByTracker(this.IP, this.GetClient().GetConnection().getIp(), this.GetClient().MachineId) != null)
                {
                    this.GetClient().SendPacket(new CloseConnectionComposer());
                    return;
                }
            }

            if (!EnterRoom(room))
                this.GetClient().SendPacket(new CloseConnectionComposer());
            else
            {
                this.GetClient().GetHabbo().LoadingRoomId = Id;
                this.GetClient().GetHabbo().AllowDoorBell = true;
            }

        }
        public bool InitClothing()
        {
            this._clothing = new ClothingComponent();

            return _clothing.Init(this);
        }

        public bool InitIgnores()
        {
            _ignores = new IgnoresComponent();

            return _ignores.Init(this);
        }


        public bool EnterRoom(Room Room)
        {
            GameClient Session = this.GetClient();
            if (Session == null)
                return false;
            if (Room == null)
                return false;

            Session.SendPacket(new RoomReadyComposer(Room.Id, Room.RoomData.ModelName));

            if (Room.RoomData.Wallpaper != "0.0")
                Session.SendPacket(new RoomPropertyComposer("wallpaper", Room.RoomData.Wallpaper));
            if (Room.RoomData.Floor != "0.0")
                Session.SendPacket(new RoomPropertyComposer("floor", Room.RoomData.Floor));

            Session.SendPacket(new RoomPropertyComposer("landscape", Room.RoomData.Landscape));
            Session.SendPacket(new RoomRatingComposer(Room.RoomData.Score, !(Session.GetHabbo().RatedRooms.Contains(Room.Id) || Room.RoomData.OwnerId == Session.GetHabbo().Id)));


            return true;
        }

        public void LoadData(UserData.UserData data)
        {
            this.LoadAchievements(data.achievements);
            this.LoadFavorites(data.favouritedRooms);
            this.LoadRoomRights(data.RoomRightsList);
        }

        public bool HasFuse(string Fuse)
        {
            if (AkiledEnvironment.GetGame().GetRoleManager().RankHasRight(this.Rank, Fuse))
                return true;

            return false;
        }

        public void LoadRoomRights(List<int> roomID)
        {
            this.RoomRightsList = new List<RoomData>();
            foreach (int num in roomID)
            {
                RoomData roomdata = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
                this.RoomRightsList.Add(roomdata);
            }
        }

        public void LoadFavorites(List<int> roomID)
        {
            this.FavoriteRooms = new List<RoomData>();
            foreach (int num in roomID)
            {
                RoomData roomdata = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
                this.FavoriteRooms.Add(roomdata);
            }
        }

        public void LoadAchievements(Dictionary<string, UserAchievement> achievements) => this.Achievements = achievements;

        public async Task OnDisconnectAsync()
        {
            if (this.Disconnected)
                return;

            if (_process != null)
            {
                _process.Dispose();
            }

            this.Disconnected = true;

            AkiledEnvironment.GetGame().GetClientManager().UnregisterClient(this.Id, this.Username);

            if (this.Langue == Language.FRANCAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersFr--;
            else if (this.Langue == Language.ANGLAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersEn--;
            else if (this.Langue == Language.PORTUGAIS) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersBr--;
            else if (this.Langue == Language.SPANISH) AkiledEnvironment.GetGame().GetClientManager().OnlineUsersEs--;

            if (this.HasFuse("fuse_mod"))

                AkiledEnvironment.GetGame().GetClientManager().RemoveUserStaff(this.Id);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("El Usuario : " + this.Username + " Se ha Desconectado del Hotel.",
            ConsoleColor.DarkGreen);

            string Webhook = AkiledEnvironment.GetConfig().data["Webhook"];
            string Webhook_login_logout_ProfilePicture = AkiledEnvironment.GetConfig().data["Webhook_on_off_Image"];
            string Webhook_login_logout_UserNameD = AkiledEnvironment.GetConfig().data["Webhook_on_off_Username"];
            string Webhook_login_logout_WebHookurl = AkiledEnvironment.GetConfig().data["Webhook_on_off_URL"];
            string Webhooka_avatar = AkiledEnvironment.GetConfig().data["Webhook_avatar"];

            if (Webhook == "true")
            {
                var client = new DiscordWebhookClient(Webhook_login_logout_WebHookurl);

                var message = new DiscordMessage(
                 "La Seguridad es importante para nosotros! " + DiscordEmoji.Grinning,
                    username: Webhook_login_logout_UserNameD,
                    avatarUrl: Webhook_login_logout_ProfilePicture,
                    tts: false,
                    embeds: new[]
        {
                                new DiscordMessageEmbed(
                                "Notificacion de Logout" + DiscordEmoji.Thumbsup,
                                 color: 14687003,
                                author: new DiscordMessageEmbedAuthor(this.Username),
                                description: "Se ha Desconectado del Hotel",
                                thumbnail: new DiscordMessageEmbedThumbnail(Webhooka_avatar + this.Look),
                                footer: new DiscordMessageEmbedFooter("Creado por "+Webhook_login_logout_UserNameD, Webhook_login_logout_ProfilePicture)
        )
        }
        );
                await client.SendToDiscord(message);
                Console.WriteLine("logout enviado a Discord ", ConsoleColor.DarkYellow);
                //discord stats enviar a discord


            }


            if (!this.HabboinfoSaved)
            {
                this.HabboinfoSaved = true;
                TimeSpan TimeOnline = DateTime.Now - this.OnlineTime;
                int TimeOnlineSec = (int)TimeOnline.TotalSeconds;
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.RunQuery("UPDATE users SET online = '0', last_online = '" + AkiledEnvironment.GetUnixTimestamp() + "', activity_points = " + this.Duckets + ", activity_points_lastupdate = '" + this.LastActivityPointsUpdate + "', credits = " + this.Credits + " WHERE id = " + this.Id + " ;");
                    queryreactor.RunQuery("UPDATE user_stats SET groupid = " + this.FavouriteGroupId + ",  OnlineTime = OnlineTime + " + TimeOnlineSec + ", quest_id = '" + this.CurrentQuestId + "', Respect = '" + this.Respect + "', DailyRespectPoints = '" + this.DailyRespectPoints + "', DailyPetRespectPoints = '" + this.DailyPetRespectPoints + "' WHERE id = " + this.Id + " ;");
                }

            }


            if (this.InRoom && this.CurrentRoom != null)
            {
                this.CurrentRoom.GetRoomUserManager().RemoveUserFromRoom(this.mClient, false, false);
            }

            if (this.RolePlayId > 0)
            {
                RolePlayerManager RPManager = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.RolePlayId);
                if (RPManager != null)
                {
                    RolePlayer Rp = RPManager.GetPlayer(this.Id);
                    if (Rp != null)
                        RPManager.RemovePlayer(this.Id);
                }
                this.RolePlayId = 0;
            }

            if (this.GuideOtherUserId != 0)
            {
                GameClient requester = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(this.GuideOtherUserId);
                if (requester != null)
                {
                    ServerPacket message = new ServerPacket(ServerPacketHeader.OnGuideSessionEnded);
                    message.WriteInteger(1);
                    requester.SendPacket(message);

                    requester.GetHabbo().GuideOtherUserId = 0;
                }
            }
            if (this.OnDuty)
                AkiledEnvironment.GetGame().GetGuideManager().RemoveGuide(this.Id);

            if (this.Messenger != null)
            {
                this.Messenger.AppearOffline = true;
                this.Messenger.Destroy();
            }

            if (this.InventoryComponent != null)
            {
                this.InventoryComponent.Destroy();
                this.InventoryComponent = null;
            }

            if (this.BadgeComponent != null)
            {
                this.BadgeComponent.Destroy();
                this.BadgeComponent = null;
            }

            if (this.UsersRooms != null)
                this.UsersRooms.Clear();

            if (this.RoomRightsList != null)
                this.RoomRightsList.Clear();

            if (this.FavoriteRooms != null)
                this.FavoriteRooms.Clear();

            this.mClient = (GameClient)null;
        }

        public void Dispose()
        {

            if (Messenger != null)
            {
                Messenger.AppearOffline = true;
                Messenger.Destroy();
            }



            if (_clothing != null)
                _clothing.Dispose();

        }

        public void UpdateCreditsBalance()
        {
            GameClient client = this.GetClient();
            if (client == null)
                return;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.CreditBalanceMessageComposer);
            Message.WriteString(this.Credits + ".0");
            client.SendPacket(Message);
        }

        public void UpdateDiamondsBalance()
        {
            GameClient client = this.GetClient();
            if (client == null) return;

            ServerPacket Message = new ServerPacket(ServerPacketHeader.ActivityPointsMessageComposer);
            Message.WriteInteger(1);
            Message.WriteInteger(105);
            Message.WriteInteger(this.AkiledPoints);
            client.SendPacket(Message);
        }

        public void UpdateActivityPointsBalance()
        {
            GameClient client = this.GetClient();
            if (client == null) return;

            client.SendPacket(new HabboActivityPointNotificationComposer(this.Duckets, 1));
        }

        private GameClient GetClient() => AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(this.Id);

        public GameClient GetCliente()
        {
            if (mClient != null)
            {
                return mClient;
            }
            return AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
        }


        public HabboMessenger GetMessenger() => this.Messenger;

        public BadgeComponent GetBadgeComponent()
        => this.BadgeComponent;

        public InventoryComponent GetInventoryComponent()
        => this.InventoryComponent;

        public ChatMessageManager GetChatMessageManager()
        => this.chatMessageManager;

        public int GetQuestProgress(int p)
        {
            this.quests.TryGetValue(p, out int num);
            return num;
        }
        public ClothingComponent GetClothing()
        => _clothing;

        public UserAchievement GetAchievementData(string p)
        {
            UserAchievement userAchievement = (UserAchievement)null;
            this.Achievements.TryGetValue(p, out userAchievement);
            return userAchievement;
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


        public void CheckCreditsTimer()
        {
            try
            {
                --this._creditsTickUpdate;
                if (this.CalendarCounter < 30)
                    ++this.CalendarCounter;
                if (this._creditsTickUpdate > 0)
                    return;
                Random random = new Random();
                int Notif1 = 0;
                int num = 0;
                int Notif2 = 0;
                string str1 = "";
                string str2 = "";
                string str3 = "";
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT * FROM `game_happyhour` LIMIT 1");
                    foreach (DataRow row in (InternalDataCollectionBase)queryReactor.GetTable().Rows)
                    {
                        num = Convert.ToInt32(row["credits"]);
                        Notif1 = Convert.ToInt32(row["duckets"]);
                        Notif2 = Convert.ToInt32(row["diamantes"]);
                        str1 = Convert.ToString(row["moneda1"]);
                        str2 = Convert.ToString(row["moneda2"]);
                        str3 = Convert.ToString(row["moneda3"]);
                    }
                }
                this.mClient.GetHabbo().Credits += num;
                this.mClient.SendPacket((IServerPacket)new CreditBalanceComposer(this.mClient.GetHabbo().Credits));
                this.mClient.GetHabbo().Duckets += Notif2;
                this.mClient.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(this.mClient.GetHabbo().Duckets, Notif2));
                this.mClient.GetHabbo().AkiledPoints += Notif1;
                this.mClient.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(this.mClient.GetHabbo().AkiledPoints, Notif1, 105));
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryReactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Notif1.ToString() + " WHERE id = " + this.mClient.GetHabbo().Id.ToString() + " LIMIT 1");
                this.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("happyhour", "Has recibido " + num.ToString() + " " + str1 + ", " + Notif2.ToString() + " " + str2 + ", " + Notif1.ToString() + " " + str3 + " Por estar conectado 30 minutos en el hotel."));
                this.GetClient().SendWhisper("Has recibido " + num.ToString() + " " + str1 + ", " + Notif2.ToString() + " " + str2 + ", " + Notif1.ToString() + " " + str3 + " Por estar conectado 30 minutos en el hotel.", 34);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("El Usuario: " + this.mClient.GetHabbo().Username + " ha recibido su premio por tiempo online. ", (object)".", (object)ConsoleColor.DarkGreen);
                this.CreditsUpdateTick = 30;
            }
            catch
            {
            }
        }
        public void CheckCreditsTimerRP()
        {
            try
            {
                --this._creditsTickUpdate;
                if (this.CalendarCounter < 30)
                    ++this.CalendarCounter;
                if (this._creditsTickUpdate > 0)
                    return;
                Random random = new Random();
                int Notif1 = 0;
                int num = 0;
                int num2 = 0;
                int Notif2 = 0;
                string str1 = "";
                string str2 = "";
                string str3 = "";
                string str4 = "";
                RoomUser roomUserByHabboId = this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id);
                RolePlayer roleplayer = roomUserByHabboId.Roleplayer;
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT * FROM `game_happyhour` LIMIT 1");
                    foreach (DataRow row in (InternalDataCollectionBase)queryReactor.GetTable().Rows)
                    {
                        num = Convert.ToInt32(row["credits"]);
                        Notif1 = Convert.ToInt32(row["duckets"]);
                        Notif2 = Convert.ToInt32(row["diamantes"]);
                        num2 = Convert.ToInt32(row["dolares"]);
                        str1 = Convert.ToString(row["moneda1"]);
                        str2 = Convert.ToString(row["moneda2"]);
                        str3 = Convert.ToString(row["moneda3"]);
                        str4 = Convert.ToString(row["moneda4"]);
                    }
                }
                this.mClient.GetHabbo().Credits += num;
                this.mClient.SendPacket((IServerPacket)new CreditBalanceComposer(this.mClient.GetHabbo().Credits));
                this.mClient.GetHabbo().Duckets += Notif2;
                this.mClient.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(this.mClient.GetHabbo().Duckets, Notif2));
                this.mClient.GetHabbo().AkiledPoints += Notif1;
                roleplayer.Money += num2;
                roleplayer.SendUpdate();
                this.mClient.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(this.mClient.GetHabbo().AkiledPoints, Notif1, 105));
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryReactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Notif1.ToString() + " WHERE id = " + this.mClient.GetHabbo().Id.ToString() + " LIMIT 1");
                this.GetClient().SendMessage((IServerPacket)RoomNotificationComposer.SendBubble("notibonusrp", "Has recibido " + num.ToString() + " " + str1 + ", " + Notif2.ToString() + " " + str2 + ", " + Notif1.ToString() + " " + str3 + ", " + num2.ToString() + " " + str4 + "Por estar conectado 30 minutos en en la zona rp."));
                this.GetClient().SendWhisper("Has recibido " + num.ToString() + " " + str1 + ", " + Notif2.ToString() + " " + str2 + ", " + Notif1.ToString() + " " + str3 + " Por estar conectado 30 minutos en el hotel.", 34);
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("El Usuario: " + this.mClient.GetHabbo().Username + " ha recibido su premio por tiempo online. ", (object)".", (object)ConsoleColor.DarkGreen);
                this.CreditsUpdateTick = 30;
            }
            catch
            {
            }
        }

        public void DestroyAngel(int hp)
        {
            int num = 0;
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(166);
            this.angelstatus = num;
        }

        public void DestroyDetector(int hp)
        {
            int num = 0;
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(372);
            this.angelstatus = num;
        }

        public void DestroyPico(int hp)
        {
            int num = 0;
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(374);
            this.angelstatus = num;
        }

        public void EatWurm()
        {
            string str = AkiledEnvironment.GetConfig().data["item_idgusano"];
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(167);
            --this.wurm;
        }

        public void Leftpolvora()
        {
            string str = AkiledEnvironment.GetConfig().data["item_idporbora"];
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(375);
            --this.porbora;
        }

        public void Leftexplosivo()
        {
            string str = AkiledEnvironment.GetConfig().data["item_idexplosivo"];
            this.GetClient().GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(this.GetClient().GetHabbo().Id).Roleplayer.RemoveInventoryItem(373);
            --this.explosivo;
        }

        public void AddFish(int fishid)
        {
            try
            {
                string str = "";
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT `itemname` FROM `user_rpfish` WHERE `id` = @id LIMIT 1");
                    queryReactor.AddParameter("id", fishid);
                    str = queryReactor.GetString();
                }
                if (str == "")
                    return;
                DataTable dataTable = (DataTable)null;
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryReactor.SetQuery("SELECT count FROM user_rpitems WHERE user_id = '" + this.Id.ToString() + "' AND fish_name = @fishname LIMIT 1");
                    queryReactor.AddParameter("fishname", str);
                    dataTable = queryReactor.GetTable();
                }
                if (dataTable != null && (uint)dataTable.Rows.Count > 0U)
                {
                    foreach (DataRow row in (InternalDataCollectionBase)dataTable.Rows)
                    {
                        using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryReactor.RunQuery("UPDATE user_rpitems SET count = count + 1 WHERE  user_id = '" + this.Id.ToString() + "' AND fish_name = '" + str + "' LIMIT 1 ");
                    }
                }
                else
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        queryReactor.RunQuery("INSERT INTO user_rpitems (user_id,count,fish_name) VALUES ('" + this.Id.ToString() + "','1','" + str + "');");
                }
            }
            catch (Exception ex)
            {
            }
        }


        public void casinoEvent(string diceRoll)
        {
            if (this.casinoEnabled)
            {
                this.casinoCount = this.casinoCount + Int32.Parse(diceRoll);
                if (this.casinoCount > 21)
                {
                    this.CurrentRoom.SendPacket(RoomNotificationComposer.SendBubble("volada", "El usuario " + this.Username + " tira los dados y lleva " + this.casinoCount + ", ha volado.", ""));
                    this.GetClient().SendWhisper("El usuario " + this.Username + " tira los dados y lleva " + this.casinoCount + ", ha volado.", 27);
                    this.casinoCount = 0;
                    this.casinoEnabled = false;
                    this.GetClient().SendWhisper("Modo casino desactivado", 34);
                }
                else if (this.casinoCount == 21)
                {
                    this.CurrentRoom.SendPacket(RoomNotificationComposer.SendBubble("ganadordado", "El usuario " + this.Username + " ha sacado " + this.casinoCount + " en los dados (Ganador ó ganando por el momento)", ""));
                    this.GetClient().SendWhisper("El usuario " + this.Username + " ha sacado " + this.casinoCount + " en los dados (Ganador ó ganando por el momento)", 27);
                    //this.Effects().ApplyEffect(165);
                    this.casinoCount = 0;
                    this.casinoEnabled = false;
                    this.GetClient().SendWhisper("Modo casino desactivado", 34);
                }
                if ((this.casinoCount == 19) || (this.casinoCount == 20))
                {
                    this.CurrentRoom.SendPacket(RoomNotificationComposer.SendBubble("dadospl", "El usuario " + this.Username + " ha sacado " + this.casinoCount + " en los dados (puede quedar en PL ó Arriesgarse)", ""));
                    this.GetClient().SendWhisper("El usuario " + this.Username + " ha sacado " + this.casinoCount + " en los dados (puede quedar en PL ó Arriesgarse)", 27);
                    //this.Effects().ApplyEffect(165);
                    this.casinoCount = 0;
                    this.casinoEnabled = false;
                    this.GetClient().SendWhisper("Modo casino desactivado", 34);
                }
                else
                {
                    this.CurrentRoom.SendPacket(RoomNotificationComposer.SendBubble("sumandodados", "El usuario " + this.Username + " tira los dados y lleva " + this.casinoCount + ".", ""));
                    this.GetClient().SendWhisper("El usuario " + this.Username + " tira los dados y lleva " + this.casinoCount + ".", 27);

                }

            }
        }
    }
}
