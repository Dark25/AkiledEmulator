
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.RoomBots;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace Akiled.HabboHotel.Rooms
{
    public class RoomUser : IEquatable<RoomUser>
    {
        public int UserId;
        public int HabboId;
        public int VirtualId;
        public int RoomId;
        public int IdleTime;
        public int X;
        public int Y;
        public double Z;
        public int GoalX;
        public int GoalY;
        public bool SetStep;
        public int SetX;
        public int SetY;
        public double SetZ;
        public int CarryItemID;
        public int CarryTimer;
        public int RotHead;
        public int RotBody;
        public bool CanWalk;
        public bool AllowOverride;
        public bool TeleportEnabled;
        public bool AllowMoveToRoller;
        private RoomUserManager roomUserManager;
        private GameClient Client;
        public RoomBot BotData;
        public Pet PetData;
        public BotAI BotAI;
        public Room Room;
        public RoomData RoomData;
        public ItemEffectType CurrentItemEffect;
        public bool Freezed;
        public int FreezeCounter;
        public Team Team;
        public FreezePowerUp BanzaiPowerUp;
        public int FreezeLives;
        public bool ShieldActive;
        public int ShieldCounter;
        public int CountFreezeBall = 1;
        public bool MoonwalkEnabled;
        public bool FacewalkEnabled;
        public bool RidingHorse;
        public bool IsSit;
        public bool IsLay;
        public int HorseID;
        public bool IsWalking;
        public bool UpdateNeeded;
        public bool IsAsleep;
        public Dictionary<string, string> Statusses;
        public int DanceId;
        public int FloodCount;
        public bool IsSpectator;
        public bool ConstruitEnable = false;
        public bool ConstruitZMode = false;
        public double ConstruitHeigth = 1.0;
        public bool Freeze;
        public bool is_angeln;
        public int FreezeEndCounter;
        public bool transformation;
        public bool transfbot;
        public string transformationrace;
        public bool AllowBall;
        public bool MoveWithBall;
        public bool SetMoveWithBall;
        public bool AllowShoot;
        public string ChatTextColor;
        public string LastMessage;
        public int LastMessageCount;
        public int PartyId;
        public int TimerResetEffect;
        public string LoaderVideoId;
        public int WiredPoints;
        public bool InGame;
        public bool WiredGivelot;
        public bool BreakWalkEnable;
        public bool StopWalking;
        public bool ReverseWalk;
        public bool WalkSpeed;
        public bool AllowMoveTo;
        public int LLPartner = 0;
        public int CurrentEffect;
        public List<int> AllowBuyItems;
        public bool IsDispose;
        public int UserTimer;
        public int LastBubble = 0;
        public List<string> WhiperGroupUsers;
        public bool muted;

        public int setRotate;
        public int setState;
        public double forceHeight;

        public int Id => this.RoomData.Id;

        public Point Coordinate => new Point(this.X, this.Y);

        public bool IsPet => this.IsBot && this.BotData.IsPet;

        public bool IsDancing => this.DanceId >= 1;

        public bool NeedsAutokick
        {
            get
            {
                if (this.IsBot)
                    return false;
                if (this.GetClient() == null || this.GetClient().GetHabbo() == null)
                    return true;
                return this.GetClient().GetHabbo().Rank < 2 && this.IdleTime >= 1200;
            }
        }

        public bool IsTrading => !this.IsBot && this.Statusses.ContainsKey("/trd");

        public bool IsBot => this.BotData != null;

        public RolePlayer Roleplayer
        {
            get
            {
                RolePlayerManager rolePlay = AkiledEnvironment.GetGame().GetRoleplayManager().GetRolePlay(this.GetRoom().RoomData.OwnerId);
                return rolePlay == null ? (RolePlayer)null : rolePlay.GetPlayer(this.UserId) ?? (RolePlayer)null;
            }
        }

        public int HandelingBallStatus { get; set; }

        public RoomUser(int HabboId, int RoomId, int VirtualId, Room room)
        {
            this.Freezed = false;
            this.is_angeln = false;
            this.HabboId = HabboId;
            this.RoomId = RoomId;
            this.VirtualId = VirtualId;
            this.IdleTime = 0;
            this.X = 0;
            this.Y = 0;
            this.Z = 0.0;
            this.RotHead = 0;
            this.RotBody = 0;
            this.UpdateNeeded = true;
            this.Statusses = new Dictionary<string, string>();
            this.Room = room;
            this.AllowOverride = false;
            this.CanWalk = true;
            this.CurrentItemEffect = ItemEffectType.None;
            this.BreakWalkEnable = false;
            this.AllowShoot = false;
            this.AllowBuyItems = new List<int>();
            this.IsDispose = false;
            this.AllowMoveTo = true;
            this.WhiperGroupUsers = new List<string>();

            this.setRotate = -1;
            this.setState = -1;
            this.forceHeight = -1;
        }

        public bool Equals(RoomUser comparedUser) => comparedUser != null && comparedUser.VirtualId == this.VirtualId;

        public string GetUsername()
        {
            if (this.IsBot)
                return this.BotData.Name;
            return this.IsPet ? this.PetData.Name : (this.GetClient() == null || this.GetClient().GetHabbo() == null ? string.Empty : this.GetClient().GetHabbo().Username);
        }

        public bool IsOwner() => !this.IsBot && this.GetUsername() == this.GetRoom().RoomData.OwnerName;

        public void Unidle()
        {
            this.IdleTime = 0;
            if (!this.IsAsleep)
                return;
            this.IsAsleep = false;
            ServerPacket serverPacket = new ServerPacket(1797);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteBoolean(false);
            this.GetRoom().SendPacket((IServerPacket)serverPacket);
        }

        public void Dispose()
        {
            this.Statusses.Clear();
            this.IsDispose = true;
            this.Room = (Room)null;
            this.Client = (GameClient)null;
        }

        public void SendWhisperChat(string message, bool Info = true)
        {
            ServerPacket serverPacket = new ServerPacket(2704);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteString(message);
            serverPacket.WriteInteger(0);
            serverPacket.WriteInteger(Info ? 34 : 0);
            serverPacket.WriteInteger(0);
            serverPacket.WriteInteger(-1);
            this.GetClient().SendPacket((IServerPacket)serverPacket);
        }

        public void OnChatMe(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = 1446;
            if (Shout)
                Header = 1036;
            ServerPacket serverPacket = new ServerPacket(Header);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteString(MessageText);
            serverPacket.WriteInteger(AkiledEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            serverPacket.WriteInteger(Color);
            serverPacket.WriteInteger(0);
            serverPacket.WriteInteger(-1);
            this.GetClient().SendPacket((IServerPacket)serverPacket);
        }

        public void SendPrefixPacket()
        {
            if (this.IsBot || this.IsPet || this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return;
            string str1 = this.GetClient().GetHabbo().Prefix.Split(';')[0];
            string str2 = this.GetClient().GetHabbo().Prefixnamecolor.Split(';')[0];
            string str3 = this.GetClient().GetHabbo().PrefixSize.Split(';')[0];
            string str4 = this.GetClient().GetHabbo().PrefixSize.Split(';')[1];
            string str5 = this.GetClient().GetHabbo().Prefix.Split(';')[1];
            string str6 = (string)null;
            string prefixemoji = this.GetClient().GetHabbo().Prefixnamecolor.Split(';')[1];
            if (str5 == "RAINCOLOR")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[6]
                {
          "FC0808",
          "FFA501",
          "068409",
          "0609FF",
          "810683",
          "FF00FF"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "germany")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[3]
                {
          "000",
          "CA0303",
          "f39c12"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "bluyeor")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[3]
                {
          "2980b9",
          "f39c12",
          "e67e22"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "pinkrosa")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[2]
                {
          "FF1493",
          "FF00FF"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "blurapi")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[3]
                {
          "2980b9",
          "FF00FF",
          "e67e22"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "CHRISTMAS18")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[2]
                {
          "009432",
          "c0392b"
                };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }
            else if (str5 == "yeblack")
            {
                string str7 = "";
                Random random = new Random();
                string[] strArray = new string[2] { "c0392b", "000" };
                int index = 0;
                foreach (char ch in str1)
                {
                    if (index > strArray.Length - 1)
                        index = 0;
                    str7 = str7 + "<font color='#" + strArray[index] + "'>" + ch.ToString() + "</font>";
                    ++index;
                }
                str6 = str7;
            }

            if (string.IsNullOrWhiteSpace(str1) || !(str1 != "off"))
                return;
            string Username;
            if (str5 == "RAINCOLOR" || str5 == "germany" || (str5 == "yeblack" || str5 == "CHRISTMAS18") || (str5 == "blurapi" || str5 == "pinkrosa") || str5 == "bluyeor")
                Username = "<font size= '" + str3 + "'>[" + str6 + "] <font size= '" + str4 + "px'><font color='#" + str2 + "'>" + prefixemoji + this.GetClient().GetHabbo().Username + "</font></font></font></font>";
            else
                Username = "<font size= '" + str3 + "px'><font color='#" + str5 + "'>[" + str1 + "]</font></font> <font size= '" + str4 + "px'><font color='#" + str2 + "'>" + prefixemoji + this.GetClient().GetHabbo().Username + " </font></font></font></font>";
            if (this.GetRoom() == null)
                return;
            this.GetRoom().SendPacket((IServerPacket)new UserNameChangeMessageComposer(0, this.VirtualId, Username));
        }

        public void SendNamePacket()
        {
            if (this.IsBot || this.IsPet || this.GetClient() == null || this.GetClient().GetHabbo() == null)
                return;
            if (string.IsNullOrWhiteSpace(this.GetClient().GetHabbo().Prefix.Split(';')[0]))
                return;
            string username = this.GetClient().GetHabbo().Username;
            if (this.GetRoom() == null)
                return;
            this.GetRoom().SendPacket((IServerPacket)new UserNameChangeMessageComposer(0, this.VirtualId, username));
        }

        public void OnChat(string MessageText, int Color = 0, bool Shout = false)
        {
            int Header = 1446;
            if (Shout)
                Header = 1036;
            ServerPacket serverPacket = new ServerPacket(Header);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteString(MessageText);
            serverPacket.WriteInteger(AkiledEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(MessageText));
            string pattern = "<img\\b[^\\<\\>]+?\\bsrc\\s*=\\s*[\"'](?<L>.+?)[\"'][^\\<\\>]*?\\>";
            foreach (Match match in Regex.Matches(MessageText, pattern, RegexOptions.IgnoreCase))
            {
                if (!match.Groups["L"].Value.ToLower().StartsWith("http://localhost/"))
                    return;
            }
            serverPacket.WriteInteger(Color);
            serverPacket.WriteInteger(0);
            serverPacket.WriteInteger(-1);
            this.SendPrefixPacket();
            this.GetRoom().SendPacketOnChat((IServerPacket)serverPacket, this, true, this.Team == Team.none && !this.IsBot);
            this.SendNamePacket();
        }

        public static int GetSpeechEmotion(string Message)
        {
            Message = Message.ToLower();
            if (Message.Contains(":)") || Message.Contains(":d") || (Message.Contains("=]") || Message.Contains("=d")) || Message.Contains(":>"))
                return 1;
            return Message.Contains(">:(") || Message.Contains(":@") ? 2 : (Message.Contains(":o") ? 3 : (Message.Contains(":(") || Message.Contains("=[") || (Message.Contains(":'(") || Message.Contains("='[")) ? 4 : 0));
        }

        public void MoveTo(Point c, bool Override = false) => this.MoveTo(c.X, c.Y, Override);

        public void MoveTo(int pX, int pY, bool pOverride = false)
        {
            if (!this.GetRoom().GetGameMap().CanWalkState(pX, pY, pOverride) || this.Freeze || !this.AllowMoveTo || (this.is_angeln = false))
                return;
            this.Unidle();
            if (this.TeleportEnabled && (!this.InGame || this.IsOwner()))
            {
                this.GetRoom().SendPacket((IServerPacket)this.GetRoom().GetRoomItemHandler().TeleportUser(this, new Point(pX, pY), 0, this.GetRoom().GetGameMap().SqAbsoluteHeight(pX, pY)));
            }
            else
            {
                this.IsWalking = true;
                this.GoalX = pX;
                this.GoalY = pY;
            }
        }

        public void UnlockWalking()
        {
            this.AllowOverride = false;
            this.CanWalk = true;
        }

        public void SetPosRoller(int pX, int pY, double pZ)
        {
            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;
            this.SetStep = true;
            this.GetRoom().GetGameMap().AddTakingSquare(pX, pY);
            this.UpdateNeeded = false;
            this.IsWalking = false;
        }

        public void SetPos(int pX, int pY, double pZ)
        {
            this.GetRoom().GetGameMap().UpdateUserMovement(this.Coordinate, new Point(pX, pY), this);
            this.X = pX;
            this.Y = pY;
            this.Z = pZ;
            this.SetX = pX;
            this.SetY = pY;
            this.SetZ = pZ;
            this.GoalX = this.X;
            this.GoalY = this.Y;
            this.SetStep = false;
            this.IsWalking = false;
            this.UpdateNeeded = true;
        }

        public void CarryItem(int Item, bool notTimer = false)
        {
            this.CarryItemID = Item;
            this.CarryTimer = Item <= 0 | notTimer ? 0 : 240;
            ServerPacket serverPacket = new ServerPacket(1474);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteInteger(Item);
            this.GetRoom().SendPacket((IServerPacket)serverPacket);
        }

        public void SetRot(int Rotation, bool HeadOnly, bool IgnoreWalk = false)
        {
            if (this.Statusses.ContainsKey("lay") || this.IsWalking && !IgnoreWalk)
                return;
            int num = this.RotBody - Rotation;
            this.RotHead = this.RotBody;
            if (HeadOnly || this.Statusses.ContainsKey("sit"))
            {
                if (this.RotBody == 2 || this.RotBody == 4)
                {
                    if (num > 0)
                        this.RotHead = this.RotBody - 1;
                    else if (num < 0)
                        this.RotHead = this.RotBody + 1;
                }
                else if (this.RotBody == 0 || this.RotBody == 6)
                {
                    if (num > 0)
                        this.RotHead = this.RotBody - 1;
                    else if (num < 0)
                        this.RotHead = this.RotBody + 1;
                }
            }
            else if (num <= -2 || num >= 2)
            {
                this.RotHead = Rotation;
                this.RotBody = Rotation;
            }
            else
                this.RotHead = Rotation;
            this.UpdateNeeded = true;
        }

        public void SetStatus(string Key, string Value)
        {
            if (this.Statusses.ContainsKey(Key))
                this.Statusses[Key] = Value;
            else
                this.Statusses.Add(Key, Value);
        }

        public void RemoveStatus(string Key)
        {
            if (!this.Statusses.ContainsKey(Key))
                return;
            this.Statusses.Remove(Key);
        }

        public void ApplyEffect(int EffectId, bool DontSave = false)
        {
            if (this.Room == null || this.RidingHorse && EffectId != 77 && EffectId != 103 || this.CurrentEffect == EffectId && !DontSave)
                return;
            if (!DontSave)
                this.CurrentEffect = EffectId;
            ServerPacket serverPacket = new ServerPacket(1167);
            serverPacket.WriteInteger(this.VirtualId);
            serverPacket.WriteInteger(EffectId);
            serverPacket.WriteInteger(2);
            this.Room.SendPacket((IServerPacket)serverPacket);
        }

        public void Chat(string Message, bool Shout, int colour = 0)
        {
            if (this.GetRoom() == null || !this.IsBot)
                return;
            if (this.IsPet)
            {
                foreach (RoomUser roomUser in this.GetRoom().GetRoomUserManager().GetUserList().ToList<RoomUser>())
                {
                    if (roomUser != null && !roomUser.IsBot)
                    {
                        if (roomUser.GetClient() == null || roomUser.GetClient().GetHabbo() == null)
                            break;
                        if (!roomUser.GetClient().GetHabbo().AllowPetSpeech)
                            roomUser.GetClient().SendMessage((IServerPacket)new ChatComposer(this.VirtualId, Message, 0, 0));
                    }
                }
            }
            else
            {
                foreach (RoomUser roomUser in this.GetRoom().GetRoomUserManager().GetUserList().ToList<RoomUser>())
                {
                    if (roomUser != null && !roomUser.IsBot)
                    {
                        if (roomUser.GetClient() != null && roomUser.GetClient().GetHabbo() != null)
                        {
                            if (!roomUser.GetClient().GetHabbo().AllowBotSpeech)
                                roomUser.GetClient().SendMessage((IServerPacket)new ChatComposer(this.VirtualId, Message, 0, colour == 0 ? 2 : colour));
                        }
                        else
                            break;
                    }
                }
            }
        }

        public bool SetPetTransformation(string NamePet, int RaceId)
        {
            switch (NamePet.ToLower())
            {
                case "arraigne":
                    this.transformationrace = "8 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bebe":
                    this.transformationrace = "27 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bebeelephant":
                    this.transformationrace = "30 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bebepingouin":
                    this.transformationrace = "31 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bebeterrier":
                    this.transformationrace = "26 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bigbelchonok":
                    this.transformationrace = "72 0 FFFFFF";
                    break;
                case "bigboy1":
                    this.transformationrace = "52 0 FFFFFF";
                    break;
                case "bigboy2":
                    this.transformationrace = "53 0 FFFFFF";
                    break;
                case "bigboy3":
                    this.transformationrace = "54 0 FFFFFF";
                    break;
                case "bigboy4":
                    this.transformationrace = "55 0 FFFFFF";
                    break;
                case "bigboy5":
                    this.transformationrace = "56 0 FFFFFF";
                    break;
                case "biggirl1":
                    this.transformationrace = "57 0 FFFFFF";
                    break;
                case "biggirl2":
                    this.transformationrace = "58 0 FFFFFF";
                    break;
                case "biggirl3":
                    this.transformationrace = "59 0 FFFFFF";
                    break;
                case "biggirl4":
                    this.transformationrace = "60 0 FFFFFF";
                    break;
                case "biggirl5":
                    this.transformationrace = "61 0 FFFFFF";
                    break;
                case "bigjason":
                    this.transformationrace = "78 0 FFFFFF";
                    break;
                case "bigkodamas":
                    this.transformationrace = "47 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bigleanie":
                    this.transformationrace = "74 0 FFFFFF";
                    break;
                case "bigmartial":
                    this.transformationrace = "51 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "bigninou":
                    this.transformationrace = "82 0 FFFFFF";
                    break;
                case "bigseonsaengnim":
                    this.transformationrace = "76 0 FFFFFF";
                    break;
                case "bigxorcist":
                    this.transformationrace = "80 0 FFFFFF";
                    break;
                case "bigzeers":
                    this.transformationrace = "49 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "chat":
                    this.transformationrace = "1 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "chaton":
                case "kittenbaby":
                    this.transformationrace = "37 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "cheval":
                    this.transformationrace = "13 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "chien":
                    this.transformationrace = "0 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "chiot":
                case "puppybaby":
                    this.transformationrace = "38 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "cochon":
                    this.transformationrace = "5 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "crocodile":
                    this.transformationrace = "2 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "dragon":
                    this.transformationrace = "12 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "fools":
                case "pierre":
                    this.transformationrace = "40 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "gnome":
                    this.transformationrace = "29 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "grenouille":
                    this.transformationrace = "11 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "haloompa":
                case "wiloompa":
                    this.transformationrace = "41 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "hamster":
                    this.transformationrace = "34 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lapin":
                case "lapinmonstre":
                    this.transformationrace = "17 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lapinbrun":
                    this.transformationrace = "19 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lapinjaune":
                    this.transformationrace = "24 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lapinnoir":
                    this.transformationrace = "18 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lapinrose":
                    this.transformationrace = "20 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "lion":
                    this.transformationrace = "6 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "littlebelchonok":
                    this.transformationrace = "73 0 FFFFFF";
                    break;
                case "littleboy1":
                    this.transformationrace = "62 0 FFFFFF";
                    break;
                case "littleboy2":
                    this.transformationrace = "63 0 FFFFFF";
                    break;
                case "littleboy3":
                    this.transformationrace = "64 0 FFFFFF";
                    break;
                case "littleboy4":
                    this.transformationrace = "65 0 FFFFFF";
                    break;
                case "littleboy5":
                    this.transformationrace = "66 0 FFFFFF";
                    break;
                case "littlegirl1":
                    this.transformationrace = "67 0 FFFFFF";
                    break;
                case "littlegirl2":
                    this.transformationrace = "68 0 FFFFFF";
                    break;
                case "littlegirl3":
                    this.transformationrace = "69 0 FFFFFF";
                    break;
                case "littlegirl4":
                    this.transformationrace = "70 0 FFFFFF";
                    break;
                case "littlegirl5":
                    this.transformationrace = "71 0 FFFFFF";
                    break;
                case "littlejason":
                    this.transformationrace = "79 0 FFFFFF";
                    break;
                case "littlekodamas":
                    this.transformationrace = "46 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "littleleanie":
                    this.transformationrace = "75 0 FFFFFF";
                    break;
                case "littlemartial":
                    this.transformationrace = "50 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "littleninou":
                    this.transformationrace = "83 0 FFFFFF";
                    break;
                case "littleseonsaengnim":
                    this.transformationrace = "77 0 FFFFFF";
                    break;
                case "littlexorcist":
                    this.transformationrace = "81 0 FFFFFF";
                    break;
                case "littlezeers":
                    this.transformationrace = "48 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "louveteau":
                    this.transformationrace = "33 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "maggie":
                    this.transformationrace = "45 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "monster":
                    this.transformationrace = "15 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "monsteregg":
                case "oeuf":
                    this.transformationrace = "36 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "monsterplante":
                    this.transformationrace = "16 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "ours":
                    this.transformationrace = "4 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "oursons":
                    this.transformationrace = "25 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "pigeonblanc":
                    this.transformationrace = "21 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "pigeonnoir":
                    this.transformationrace = "22 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "pigletbaby":
                case "porcelet":
                    this.transformationrace = "39 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "pikachu":
                    this.transformationrace = "32 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "poussin":
                    this.transformationrace = "10 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "pterosaur":
                    this.transformationrace = "42 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "rhino":
                    this.transformationrace = "7 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "singe":
                    this.transformationrace = "14 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "singedemon":
                    this.transformationrace = "23 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "terrier":
                    this.transformationrace = "3 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "tortue":
                    this.transformationrace = "9 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "vache":
                    this.transformationrace = "44 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "velociraptor":
                    this.transformationrace = "43 " + RaceId.ToString() + " FFFFFF";
                    break;
                case "yoshi":
                    this.transformationrace = "35 " + RaceId.ToString() + " FFFFFF";
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void SendPacketWeb(IServerPacket Message)
        {
            try
            {
                if (Message == null || (this == null || this.roomUserManager == null))
                    return;
                List<RoomUser> list = this.roomUserManager.GetUserList().ToList<RoomUser>();
                if (list == null)
                    return;
                foreach (RoomUser roomUser in list)
                {
                    if (roomUser != null && !roomUser.IsBot && (roomUser.GetClient() != null && roomUser.GetClient().GetConnection() != null))
                        roomUser.GetClient().GetHabbo().SendWebPacket(Message);
                }
            }
            catch (Exception ex)
            {
                Logging.HandleException(ex, "Room.SendMessageWeb (" + this.Id.ToString() + ")");
            }
        }

        public GameClient GetClient()
        {
            if (this.IsBot)
                return (GameClient)null;
            if (this.Client == null)
                this.Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(this.HabboId);
            return this.Client;
        }

        private Room GetRoom()
        {
            if (this.Room == null)
                this.Room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(this.RoomId);
            return this.Room;
        }
    }
}
