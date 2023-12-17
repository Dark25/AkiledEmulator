using Akiled.Communication.Packets;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Achievements;
using Akiled.HabboHotel.Animations;
using Akiled.HabboHotel.Cache;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.EffectsInventory;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Guides;
using Akiled.HabboHotel.HotelView;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Items.Crafting;
using Akiled.HabboHotel.LandingView;
using Akiled.HabboHotel.Navigators;
using Akiled.HabboHotel.NotifTop;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roles;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Chat;
using Akiled.HabboHotel.Subscriptions;
using Akiled.HabboHotel.Support;
using Akiled.HabboHotel.Users.Messenger;
using Akiled.HabboHotel.WebClients;
using AkiledEmulator.HabboHotel.Hotel.CollectorPark;
using AkiledEmulator.HabboHotel.Hotel.Giveaway;
using System;
using System.Diagnostics;
using System.Threading;

namespace Akiled.HabboHotel
{
    public class Game
    {
        private readonly GameClientManager _clientManager;
        private readonly WebClientManager _clientWebManager;
        private readonly RoleManager _roleManager;
        private readonly CatalogManager _catalogManager;
        private readonly NavigatorManager _navigatorManager;
        private readonly ItemDataManager _itemDataManager;
        private readonly RoomManager _roomManager;
        private readonly AchievementManager _achievementManager;
        private readonly ModerationManager _moderationManager;
        private readonly QuestManager _questManager;
        private readonly GroupManager _groupManager;
        private readonly HotelViewManager _hotelViewManager;
        private readonly GuideManager _guideManager;
        private readonly PacketManager _packetManager;
        private readonly ChatManager _chatManager;
        private readonly EffectsInventoryManager _effectsInventory;
        private readonly RoleplayManager _roleplayManager;
        private readonly AnimationManager _animationManager;
        private readonly NotificationTopManager _notiftopManager;
        private readonly LowPriorityWorker _lowPriorityWorker;
        private readonly SubscriptionManager _subscriptionManager;
        private readonly CacheManager _cacheManager;
        private readonly CrackableManager _crackableManager;
        private readonly CraftingManager _craftingManager;
        private readonly GiveAwayBlocksManager _giveAwayBlocksM;
        private readonly HallOfFameManager _hallOfFameManager;

        private Thread gameLoop; //Task
        public static bool gameLoopEnabled = true;
        public bool gameLoopActive;
        public bool gameLoopEnded;
        private bool _cycleEnded;
        private readonly Stopwatch moduleWatch;

        public Game()
        {
            this._clientManager = new GameClientManager();
            this._clientWebManager = new WebClientManager();
            this._hallOfFameManager = new HallOfFameManager();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            
                this._hallOfFameManager.Init(dbClient);
            


            this._roleManager = new RoleManager();
            this._roleManager.Init();

            this._itemDataManager = new ItemDataManager();
            this._itemDataManager.Init();

            this._catalogManager = new CatalogManager();
            this._catalogManager.Init(this._itemDataManager);

            this._cacheManager = new CacheManager();

            this._navigatorManager = new NavigatorManager();
            this._navigatorManager.Init();

            this._roleplayManager = new RoleplayManager();
            this._roleplayManager.Init();

            this._roomManager = new RoomManager();
            this._roomManager.LoadModels();

            this._groupManager = new GroupManager();
            this._groupManager.Init();

            this._moderationManager = new ModerationManager();
            this._moderationManager.Init();

            this._questManager = new QuestManager();
            this._questManager.Init();

            this._hotelViewManager = new HotelViewManager();
            this._guideManager = new GuideManager();
            this._packetManager = new PacketManager();
            this._chatManager = new ChatManager();

            this._effectsInventory = new EffectsInventoryManager();
            this._effectsInventory.init();

            this._achievementManager = new AchievementManager();

            this._animationManager = new AnimationManager();
            this._animationManager.Init();
            this._subscriptionManager = new SubscriptionManager();
            this._subscriptionManager.Init();
            this._crackableManager = new CrackableManager();
            this._craftingManager = new CraftingManager();
            this._craftingManager.Init();
            this._notiftopManager = new NotificationTopManager();
            this._notiftopManager.Init();

            CollectorParkConfigs.loadConfigs();
            CollectorParkConfigs.check();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
           
                StaffChat.Initialize(dbClient);
            

            DatabaseCleanup();
            LowPriorityWorker.Init();

            this.moduleWatch = new Stopwatch();

            this._giveAwayBlocksM = new GiveAwayBlocksManager();
        }




        #region Return values

        public CraftingManager GetCraftingManager() => this._craftingManager;
        public NotificationTopManager GetNotifTopManager()
        {
            return this._notiftopManager;
        }

        public AnimationManager GetAnimationManager()
        {
            return this._animationManager;
        }


        public EffectsInventoryManager GetEffectsInventoryManager()
        {
            return this._effectsInventory;
        }

        public ChatManager GetChatManager()
        {
            return this._chatManager;
        }

        public PacketManager GetPacketManager()
        {
            return _packetManager;
        }

        public HallOfFameManager GetHallOFFame() => this._hallOfFameManager;

        public GuideManager GetGuideManager()
        {
            return this._guideManager;
        }

        public RoleplayManager GetRoleplayManager()
        {
            return this._roleplayManager;
        }

        public GameClientManager GetClientManager()
        {
            return this._clientManager;
        }

        public WebClientManager GetClientWebManager()
        {
            return this._clientWebManager;
        }

        public RoleManager GetRoleManager()
        {
            return this._roleManager;
        }

        public CatalogManager GetCatalog()
        {
            return this._catalogManager;
        }

        public NavigatorManager GetNavigator()
        {
            return this._navigatorManager;
        }

        public ItemDataManager GetItemManager()
        {
            return this._itemDataManager;
        }

        public RoomManager GetRoomManager()
        {
            return this._roomManager;
        }

        public AchievementManager GetAchievementManager()
        {
            return this._achievementManager;
        }

        public ModerationManager GetModerationManager()
        {
            return this._moderationManager;
        }
        public SubscriptionManager GetSubscriptionManager()
        {
            return this._subscriptionManager;
        }


        public QuestManager GetQuestManager()
        {
            return this._questManager;
        }

        public GroupManager GetGroupManager() => this._groupManager;

        public HotelViewManager GetHotelView() => this._hotelViewManager;

        public GiveAwayBlocksManager GetGiveAwayBlocks() => _giveAwayBlocksM;

        #endregion

        public void StartGameLoop()
        {
           this.gameLoopActive = true;

        var receiver = new ThreadStart(this.MainGameLoop);
        this.gameLoop = new Thread(receiver)
        {
            IsBackground = true
        };

        this.gameLoop.Start();
        }

        [Obsolete]
        public void StopGameLoop()
        {
            this.gameLoopActive = false;
            int i = 0;
            while (!this.gameLoopEnded && i < 100)
            {
                Thread.Sleep(250);

                i++;
            }

            //this.gameLoop.Dispose();
        }

        public CacheManager GetCacheManager() => this._cacheManager;
        internal CrackableManager GetPinataManager() => this._crackableManager;
        private void MainGameLoop()
        {
            while (this.gameLoopActive)
            {
                this._cycleEnded = false;
                try
                {
                    if (gameLoopEnabled)
                    {
                        moduleWatch.Restart();
                        LowPriorityWorker.Process();

                        if (moduleWatch.ElapsedMilliseconds > 500)
                            Console.WriteLine("High latency in LowPriorityWorker.Process ({0} ms)", moduleWatch.ElapsedMilliseconds);


                        this._hallOfFameManager.OnCycle();

                        if (this.moduleWatch.ElapsedMilliseconds > 500)
                        {
                            Console.WriteLine("High latency in HallOfFame ({0} ms)", this.moduleWatch.ElapsedMilliseconds);
                        }

                        this.moduleWatch.Restart();

                        this._animationManager.OnCycle(moduleWatch);


                        if (moduleWatch.ElapsedMilliseconds > 500)
                            Console.WriteLine("High latency in AnimationManager ({0} ms)", moduleWatch.ElapsedMilliseconds);

                    }
                }
                catch (OperationCanceledException e)
                {
                    Console.WriteLine("Canceled operation {0}", e);
                }
                this._cycleEnded = true;
                Thread.Sleep(500);
            }

            Console.WriteLine("MainGameLoop end");
            this.gameLoopEnded = true;
        }

        public static void DatabaseCleanup()
        {
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE users SET online = '0' WHERE online = '1'");
                dbClient.RunQuery("UPDATE users SET auth_ticket = '' WHERE auth_ticket != ''");
                dbClient.RunQuery("UPDATE user_websocket SET auth_ticket = '' WHERE auth_ticket != ''");
                dbClient.RunQuery("UPDATE rooms SET users_now = '0' WHERE users_now > '0'");
                dbClient.RunQuery("UPDATE server_status SET status = '1', users_online = '0', rooms_loaded = '0', stamp = '" + AkiledEnvironment.GetUnixTimestamp() + "'");
            }
        }
        public void Destroy()
        {
            DatabaseCleanup();
            this.GetClientManager();
            Console.WriteLine("Destroyed Habbo Hotel.");
        }
    }
}