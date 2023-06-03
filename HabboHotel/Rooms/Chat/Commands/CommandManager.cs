using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Chat.Commands.Cmd;
using Akiled.HabboHotel.Rooms.Chat.Commands.Moderator;
using Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        private readonly Dictionary<string, ChatCommand> commandRegisterInvokeable;
        private readonly Dictionary<int, string> ListCommande;

        private readonly Dictionary<int, IChatCommand> _commands;

        public CommandManager()
        {
            this._commands = new Dictionary<int, IChatCommand>();
            this.commandRegisterInvokeable = new Dictionary<string, ChatCommand>();
            this.ListCommande = new Dictionary<int, string>();
        }

        public void Init()
        {
            this.InitInvokeableRegister();
            this.RegisterCommand();

            Console.WriteLine("Comandos -> Listo!");
        }


        public bool Parse(GameClient Session, RoomUser User, Room Room, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(":"))
                return false;

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            if (!commandRegisterInvokeable.TryGetValue(Split[0].ToLower(), out ChatCommand CmdInfo))
                return false;

            if (!_commands.TryGetValue(CmdInfo.commandID, out IChatCommand Cmd))
                return false;

            int AutorisationType = CmdInfo.UserGotAuthorization2(Session, Room.RoomData.Langue);
            switch (AutorisationType)
            {
                case 2:
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.premium", Session.Langue));
                    return true;
                case 3:
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.accred", Session.Langue));
                    return true;
                case 4:
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.owner", Session.Langue));
                    return true;
                case 5:
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.authorized.langue", Session.Langue));
                    return true;
            }
            if (!CmdInfo.UserGotAuthorization(Session))
                return false;

            if (CmdInfo.UserGotAuthorizationStaffLog())
                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().CurrentRoomId, string.Empty, Split[0].ToLower(), string.Format("Uso de Comando: {0}", string.Join(" ", Split)));

            if (CmdInfo.UserGotAuthorizationUsersLog(Session))
                AkiledEnvironment.GetGame().GetModerationManager().LogUserEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, Session.GetHabbo().CurrentRoomId, string.Empty, Split[0].ToLower(), string.Format("Uso de Comando: {0}", string.Join(" ", Split)));



            Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, User, Split);
            return true;

        }

        private void InitInvokeableRegister()
        {
            this.commandRegisterInvokeable.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM system_commands");
                table = dbClient.GetTable();
            }
            if (table == null)
                return;

            foreach (DataRow dataRow in table.Rows)
            {
                int key = (int)dataRow["id"];
                int pRank = (int)dataRow["minrank"];
                string pDescriptionFr = (string)dataRow["description_fr"];
                string pDescriptionEn = (string)dataRow["description_en"];
                string pDescriptionBr = (string)dataRow["description_br"];
                string pDescriptionEs = (string)dataRow["description_es"];
                string input = (string)dataRow["input"];
                string[] strArray = input.ToLower().Split(new char[1] { ',' });

                foreach (string command in strArray)
                {
                    if (this.commandRegisterInvokeable.ContainsKey(command))
                        continue;

                    this.commandRegisterInvokeable.Add(command, new ChatCommand(key, strArray[0], pRank, pDescriptionFr, pDescriptionEn, pDescriptionBr, pDescriptionEs));
                }
            }
        }

        public string GetCommandList(GameClient client)
        {
            int rank = client.GetHabbo().Rank;
            if (this.ListCommande.ContainsKey(rank))
                return this.ListCommande[rank];

            List<string> NotDoublons = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();

            foreach (ChatCommand chatCommand in this.commandRegisterInvokeable.Values)
            {
                if (chatCommand.UserGotAuthorization(client) && !NotDoublons.Contains(chatCommand.input))
                {
                    if (client.Langue == Language.ANGLAIS)
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionEn + "\r\r");
                    else if (client.Langue == Language.PORTUGAIS)
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionBr + "\r\r");
                    else if (client.Langue == Language.SPANISH)
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionEs + "\r\r");
                    else
                        stringBuilder.Append(":" + chatCommand.input + " - " + chatCommand.descriptionFr + "\r\r");

                    NotDoublons.Add(chatCommand.input);
                }
            }
            NotDoublons.Clear();

            ListCommande.Add(rank, (stringBuilder).ToString());
            return (stringBuilder).ToString();
        }

        public void Register(int CommandId, IChatCommand Command)
        {
            this._commands.Add(CommandId, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < Params.Length; ++index)
            {
                if (index >= Start)
                {
                    if (index > Start)
                        stringBuilder.Append(" ");
                    stringBuilder.Append(Params[index]);
                }
            }
            return (stringBuilder).ToString();
        }

        public void RegisterCommand()
        {
            this._commands.Clear();

            this.Register(1, new pickall());
            this.Register(2, new setspeed());
            this.Register(3, new unload());
            this.Register(4, new disablediagonal());
            this.Register(5, new Setmax());
            this.Register(6, new overridee());
            this.Register(7, new teleport());
            this.Register(8, new StaffAlert());
            this.Register(10, new roomalert());
            this.Register(11, new coords());
            this.Register(12, new Coins());
            this.Register(14, new Handitem());
            this.Register(15, new hotelalert());
            this.Register(16, new freeze());
            this.Register(18, new enable());
            this.Register(19, new roommute());
            this.Register(23, new RoomBadgeCommand());
            this.Register(24, new Massbadge());
            this.Register(26, new UserInfo());
            this.Register(28, new RestartCommand());
            this.Register(30, new GiveBadge());
            this.Register(31, new invisible());
            this.Register(32, new ban());
            this.Register(33, new Disconnect());
            this.Register(34, new Superban());
            this.Register(36, new roomkick());
            this.Register(37, new mute());
            this.Register(38, new UnMute());
            this.Register(39, new alert());
            this.Register(40, new Kick());
            this.Register(41, new Cmd.Commands());
            this.Register(42, new About());
            this.Register(43, new info());
            this.Register(52, new forcerot());
            this.Register(53, new SetEffect());
            this.Register(54, new Emptyitems());
            this.Register(60, new warp());
            this.Register(61, new deleteMission());
            this.Register(62, new follow());
            this.Register(63, new come());
            this.Register(64, new moonwalk());
            this.Register(65, new push());
            this.Register(66, new pull());
            this.Register(67, new Copylook());
            this.Register(69, new sit());
            this.Register(70, new lay());
            this.Register(84, new transf());
            this.Register(85, new transfstop());
            this.Register(86, new kickall());
            this.Register(87, new troc());
            this.Register(88, new textamigo());
            this.Register(89, new Ipban());
            this.Register(90, new Giveitem());
            this.Register(91, new roommutepet());
            this.Register(92, new facewalk());
            this.Register(94, new AddFilter());
            this.Register(95, new noface());
            this.Register(96, new emptypets());
            this.Register(97, new construit());
            this.Register(98, new construitstop());
            this.Register(100, new spull());
            this.Register(101, new TeleportStaff());
            this.Register(102, new trigger());
            this.Register(105, new roomfreeze());
            this.Register(106, new RemoveBadge());
            this.Register(107, new roomenable());
            this.Register(108, new VipProtect());
            this.Register(109, new MachineBan());
            this.Register(111, new UnloadRoom());
            this.Register(112, new WarpStaff());
            this.Register(115, new eventalert());
            this.Register(116, new Control());
            this.Register(117, new Say());
            this.Register(118, new SetCopyLook());
            this.Register(119, new SetTransf());
            this.Register(120, new SetTransfStop());
            this.Register(121, new SetEnable());
            this.Register(122, new givelot());
            this.Register(123, new extrabox());
            this.Register(124, new SayBot());
            this.Register(126, new SetRotateCommand());
            this.Register(127, new SetStateCommand());
            this.Register(128, new murmur());
            this.Register(130, new Emptybots());
            this.Register(132, new Vip());
            this.Register(133, new followme());
            this.Register(134, new disableoblique());
            this.Register(135, new AddPhoto());
            this.Register(138, new infosuperwired());
            this.Register(140, new TransfBot());
            this.Register(141, new SetTransfBot());
            this.Register(143, new ShowGuide());
            this.Register(144, new Cmd.Janken());
            this.Register(145, new RandomLook());
            this.Register(146, new mazo());
            this.Register(148, new loadvideo());
            this.Register(149, new hidewireds());
            this.Register(150, new warpall());
            this.Register(151, new use());
            this.Register(152, new usestop());
            this.Register(153, new Youtube());
            this.Register(154, new RoomSell());
            this.Register(155, new RoomBuy());
            this.Register(156, new RoomRemoveSell());
            this.Register(157, new DupliRoom());
            this.Register(158, new Tir());
            this.Register(159, new SuperBot());
            this.Register(160, new OldFoot());
            this.Register(161, new Pyramide());
            this.Register(162, new Cac());
            this.Register(163, new Pan());
            this.Register(165, new Prison());
            this.Register(166, new Refresh());
            this.Register(168, new MaxFloor());
            this.Register(169, new AutoFloor());
            this.Register(170, new emblem());
            this.Register(171, new Givemoney());
            this.Register(172, new ConfigBot());
            this.Register(173, new SpeedWalk());
            this.Register(174, new ChutAll());
            this.Register(175, new Flagme());
            this.Register(176, new IgnoreAll());
            this.Register(177, new PushNotif());
            this.Register(178, new Big());
            this.Register(179, new Little());
            this.Register(180, new RoomYoutube());
            this.Register(181, new EdiftFurniture());
            this.Register(182, new ConvertDucketsCommand());
            this.Register(183, new ConvertGemasCommand());
            this.Register(184, new ConvertCreditsCommand());
            this.Register(185, new ConvertdDiamondsCommand());
            this.Register(186, new SmokeWeedCommand());
            this.Register(187, new WhosON());
            this.Register(188, new RewardCommand());
            this.Register(189, new VerClones());
            this.Register(190, new Subastaalert());
            this.Register(191, new Fiestaalert());
            this.Register(192, new LTDmensual());
            this.Register(193, new LTDSemanal());
            this.Register(194, new FacebookCommand());
            this.Register(195, new CDAALERT());
            this.Register(196, new InstagramAlert());
            this.Register(197, new roomunmute());
            this.Register(198, new SpamCommand());
            this.Register(199, new DadosAlerts());
            this.Register(200, new DjAlert());
            this.Register(201, new LoadRoomItems());
            this.Register(202, new neweha());
            this.Register(203, new Oleada());
            this.Register(204, new paycommand());
            this.Register(205, new GiveCommand());
            this.Register(206, new NewLTD());
            //this.Register(207, new mute());
            this.Register(208, new hotelbubble());
            this.Register(209, new userinfoforid());
            this.Register(210, new StaffAlertCommand());
            this.Register(211, new spush());
            this.Register(212, new KissCOMMAND());
            this.Register(213, new RoomCommand());
            this.Register(214, new BurnCommand());
            this.Register(215, new LTDSorpresa());
            this.Register(216, new CloseDiceCommand());
            this.Register(217, new Fiestamensual());
            this.Register(218, new RPALERT());
            this.Register(219, new KillCommand());
            this.Register(220, new GolpeCommand());
            this.Register(221, new GivebadgeOff());
            this.Register(222, new sexcommand());
            this.Register(223, new CasinoCommand());
            this.Register(224, new Massgivecommand());
            this.Register(225, new userinfo2());
            this.Register(226, new ViewInventory());
            this.Register(227, new BanList());
            this.Register(228, new BubbleCommand());
            this.Register(229, new ReloadInventory());
            this.Register(230, new roomdance());
            this.Register(231, new massdiamonds());
            this.Register(232, new massduckets());
            this.Register(233, new LastConsoleMessagesCommand());
            this.Register(234, new LastMessagesCommand());
            this.Register(235, new AllAroundMeCommand());
            this.Register(236, new AllEyesOnMeCommand());
            this.Register(237, new WelcomeCommand());
            this.Register(238, new RoomBadgeCommand());
            this.Register(239, new SMSCommand());
            this.Register(240, new LastCMDCommand());
            this.Register(241, new CheckItemCommand());
            this.Register(242, new HALCommand());
            this.Register(243, new MuteCommand());
            this.Register(244, new UnmuteCommand());
            this.Register(245, new Verinv());
            this.Register(246, new SetBadge());
            this.Register(247, new LastCMDSTAFFCommand());
            this.Register(248, new RadioStatus());
            this.Register(249, new Oleadabig());
            this.Register(250, new RegenLTD());
            this.Register(251, new ForceOpenGift());
            this.Register(252, new AllFriends());
            this.Register(253, new GameTime()); 
            this.Register(254, new StartQuestion());
            this.Register(255, new StopQuestion());
            this.Register(256, new PlaySoundRoom());
            this.Register(257, new StopSoundRoom());
            this.Register(259, new YoutubeRoom());
            this.Register(261, new Staffons());
            this.Register(262, new StaffAlertBubble());
            this.Register(263, new PremioUser());
            this.Register(264, new vomitar());
            this.Register(265, new cachetada());
            this.Register(266, new abrazar());
            this.Register(267, new FishCommand());
            this.Register(268, new TeleportUser());
            this.Register(269, new AddTagCommand());
            this.Register(270, new NameSizeCommand());
            this.Register(271, new OverrideUser());
            this.Register(272, new PrefixSizeCommand());
            this.Register(273, new SayBotUser());
            this.Register(274, new EnablePvP());
            this.Register(275, new MinaCommand());
            this.Register(276, new SuschatstaffCommand());
            this.Register(277, new StackHeightCommand());
            this.Register(278, new GiveawayCommand());
            this.Register(279, new ClearBlocksGiveawayCommand());
            this.Register(280, new CollectorParkCommand());
        }
    }
}