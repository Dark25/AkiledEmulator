using Akiled.Communication.Packets.Outgoing;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.Utilities;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms
{
    public class RoomData
    {
        public int Id;
        public string Name;
        public string Description;
        public string OwnerName;
        public int OwnerId;
        public Language Langue;
        public string Password;
        public int State;
        public int Category;
        public int UsersNow;
        public int UsersMax;
        public string ModelName;
        public int Score;
        public List<string> Tags;
        public bool AllowPets;
        public bool AllowPetsEating;
        public bool AllowWalkthrough;
        public bool AllowRightsOverride;
        public bool Hidewall;
        public string Wallpaper;
        public string Floor;
        public string Landscape;
        private RoomModel mModel;
        public int WallThickness;
        public int FloorThickness;
        public int MuteFuse;
        public int BanFuse;
        public int WhoCanKick;
        public int ChatType;
        public int ChatBalloon;
        public int ChatSpeed;
        public int ChatMaxDistance;
        public int ChatFloodProtection;
        public int GroupId;
        public int TrocStatus;
        public Group Group;
        public bool HideWired;
        public int SellPrice;
        public bool SexEnabled;
        public bool BurnEnabled;
        public bool MatarEnabled;
        public bool RobarEnabled;
        public bool BesarEnabled;
        public bool CrispyEnabled;
        public bool GolpeEnabled;
        public bool PushEnabled;
        public bool PullEnabled;
        public bool SPushEnabled;
        public bool SPullEnabled;
        public bool EnablesEnabled;
        public bool RespectNotificationsEnabled;
        public bool PetMorphsAllowed;

        public int TagCount
        {
            get
            {
                return this.Tags.Count;
            }
        }



        public RoomModel Model
        {
            get
            {
                if (this.mModel == null)
                    this.mModel = AkiledEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
                return this.mModel;
            }
        }

        public RoomData()
        {
        }

        public void FillNull(int pId)
        {
            this.Id = pId;
            this.Name = "Unknown Room";
            this.Description = "-";
            this.OwnerName = "-";
            this.Category = 0;
            this.UsersNow = 0;
            this.UsersMax = 0;
            this.ModelName = "model_a";
            this.Score = 0;
            this.Tags = new List<string>();
            this.AllowPets = true;
            this.AllowPetsEating = false;
            this.AllowWalkthrough = true;
            this.Hidewall = false;
            this.Password = "";
            this.Wallpaper = "0.0";
            this.Floor = "0.0";
            this.Landscape = "0.0";
            this.WallThickness = 0;
            this.FloorThickness = 0;
            this.MuteFuse = 0;
            this.WhoCanKick = 0;
            this.BanFuse = 0;
            this.ChatType = 0;
            this.ChatBalloon = 0;
            this.ChatSpeed = 0;
            this.ChatMaxDistance = 0;
            this.ChatFloodProtection = 0;
            this.GroupId = 0;
            this.TrocStatus = 2;
            this.Group = null;
            this.AllowRightsOverride = false;
            this.mModel = AkiledEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, pId);
            this.HideWired = false;
            this.SellPrice = 0;
            this.BesarEnabled = false;
            this.PushEnabled = false;
            this.PullEnabled = false;
            this.SPushEnabled = false;
            this.SPullEnabled = false;
            this.CrispyEnabled = false;
            this.SexEnabled = false;
            this.MatarEnabled = false;
            this.GolpeEnabled = false;
            this.BurnEnabled = false;
            LoadPromotions();
        }

        public RoomPromotion Promotion { get; set; }


        public void Fill(DataRow Row)
        {
            this.Id = Convert.ToInt32(Row["id"]);
            this.Name = (string)Row["caption"];
            this.Description = (string)Row["description"];
            this.OwnerName = (string)Row["owner"];
            this.OwnerId = 0;
            this.Langue = Language.FRANCAIS;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id, langue FROM users WHERE username = @owner");
                queryreactor.AddParameter("owner", this.OwnerName);
                DataRow UserRow = queryreactor.GetRow();
                if (UserRow != null)
                {
                    this.OwnerId = Convert.ToInt32(UserRow["id"]);
                    this.Langue = LanguageManager.ParseLanguage(UserRow["langue"].ToString());
                }
            }
            switch (Row["state"].ToString().ToLower())
            {
                case "open":
                    this.State = 0;
                    break;
                case "password":
                    this.State = 2;
                    break;
                case "hide":
                    this.State = 3;
                    break;
                default:
                    this.State = 1;
                    break;
            }
            this.Category = (int)Row["category"];
            this.UsersNow = (int)Row["users_now"];
            this.UsersMax = (int)Row["users_max"];
            this.ModelName = (string)Row["model_name"];
            this.Score = (int)Row["score"];
            this.Tags = new List<string>();
            this.AllowPets = AkiledEnvironment.EnumToBool(Row["allow_pets"].ToString());
            this.AllowPetsEating = AkiledEnvironment.EnumToBool(Row["allow_pets_eat"].ToString());
            this.AllowWalkthrough = AkiledEnvironment.EnumToBool(Row["allow_walkthrough"].ToString());
            this.AllowRightsOverride = AkiledEnvironment.EnumToBool(Row["allow_rightsoverride"].ToString());
            this.Hidewall = AkiledEnvironment.EnumToBool(Row["allow_hidewall"].ToString());
            this.Password = (string)Row["password"];
            this.Wallpaper = (string)Row["wallpaper"];
            this.Floor = (string)Row["floor"];
            this.Landscape = (string)Row["landscape"];
            this.FloorThickness = (int)Row["floorthick"];
            this.WallThickness = (int)Row["wallthick"];

            this.ChatType = (int)Row["chat_type"];
            this.ChatBalloon = (int)Row["chat_balloon"];
            this.ChatSpeed = (int)Row["chat_speed"];
            this.ChatMaxDistance = (int)Row["chat_max_distance"];
            this.ChatFloodProtection = (int)Row["chat_flood_protection"];

            this.MuteFuse = Convert.ToInt32((string)Row["moderation_mute_fuse"]);
            this.WhoCanKick = Convert.ToInt32((string)Row["moderation_kick_fuse"]);
            this.BanFuse = Convert.ToInt32((string)Row["moderation_ban_fuse"]);
            this.GroupId = (int)Row["groupId"];
            Group Group;
            AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(this.GroupId, out Group);
            this.Group = Group;
            this.HideWired = AkiledEnvironment.EnumToBool(Row["allow_hidewireds"].ToString());
            this.PushEnabled = AkiledEnvironment.EnumToBool(Row["push_enabled"].ToString());
            this.PullEnabled = AkiledEnvironment.EnumToBool(Row["pull_enabled"].ToString());
            this.SPushEnabled = AkiledEnvironment.EnumToBool(Row["spush_enabled"].ToString());
            this.SPullEnabled = AkiledEnvironment.EnumToBool(Row["spull_enabled"].ToString());
            this.BesarEnabled = AkiledEnvironment.EnumToBool(Row["besar_enabled"].ToString());
            this.CrispyEnabled = AkiledEnvironment.EnumToBool(Row["crispy_enabled"].ToString());
            this.SexEnabled = AkiledEnvironment.EnumToBool(Row["sex_enabled"].ToString());
            this.MatarEnabled = AkiledEnvironment.EnumToBool(Row["matar_enabled"].ToString());
            this.BurnEnabled = AkiledEnvironment.EnumToBool(Row["burn_enabled"].ToString());
            this.RobarEnabled = AkiledEnvironment.EnumToBool(Row["robar_enabled"].ToString());
            this.PetMorphsAllowed = AkiledEnvironment.EnumToBool(Row["PetMorphsAllowed"].ToString());

            this.TrocStatus = Convert.ToInt32((string)Row["TrocStatus"]);
            Dictionary<int, int> Items = new Dictionary<int, int>();
            if (!string.IsNullOrEmpty(Row["icon_items"].ToString()))
            {
                string str1 = Row["icon_items"].ToString();
                char[] chArray = new char[1] { '|' };
                foreach (string str2 in str1.Split(chArray))
                {
                    if (!string.IsNullOrEmpty(str2))
                    {
                        string[] strArray = str2.Replace('.', ',').Split(new char[1] { ',' });
                        int result1 = 0;
                        int result2 = 0;
                        int.TryParse(strArray[0], out result1);
                        if (strArray.Length > 1)
                            int.TryParse(strArray[1], out result2);
                        try
                        {
                            if (!Items.ContainsKey(result1))
                                Items.Add(result1, result2);
                        }
                        catch (Exception ex)
                        {
                            Logging.LogException("Exception: " + (ex).ToString() + "[" + str2 + "]");
                        }
                    }
                }
            }
            string str3 = Row["tags"].ToString();
            char[] chArray1 = new char[1] { ',' };
            foreach (string str1 in str3.Split(chArray1))
                this.Tags.Add(str1);

            this.SellPrice = (int)Row["price"];

            this.mModel = AkiledEnvironment.GetGame().GetRoomManager().GetModel(this.ModelName, this.Id);
        }



        public void SerializeRoomData(ServerPacket Message, GameClient Session) => SerializeRoomData(Message, Session, true);

        public void SerializeRoomData(ServerPacket Message, GameClient Session, bool show)
        {
            Message.WriteBoolean(show);

            this.Serialize(Message, (Session != null) && ((Session.GetHabbo().HasFuse("fuse_enter_any_room")) ? true : Session.GetHabbo().IsTeleporting));

            Message.WriteBoolean((Session != null) && (this.Id != Session.GetHabbo().CurrentRoomId));
            Message.WriteBoolean(false);
            Message.WriteBoolean(false);
            Message.WriteBoolean(false);

            Message.WriteInteger(this.MuteFuse); // who can mute
            Message.WriteInteger(this.WhoCanKick); // who can kick
            Message.WriteInteger(this.BanFuse); // who can ban

            Message.WriteBoolean((Session != null) &&
                                 this.OwnerName.ToLower() != Session.GetHabbo().Username.ToLower());
            Message.WriteInteger(this.ChatType);  //ChatMode, ChatSize, ChatSpeed, HearingDistance, ExtraFlood is the order.
            Message.WriteInteger(this.ChatBalloon);
            Message.WriteInteger(this.ChatSpeed);
            Message.WriteInteger(this.ChatMaxDistance);
            Message.WriteInteger(this.ChatFloodProtection);
        }

        public void Serialize(ServerPacket Message, bool Summon = false)
        {
            try
            {
                Message.WriteInteger(this.Id);
                Message.WriteString(this.Name);
                Message.WriteInteger(this.OwnerId);
                Message.WriteString(this.OwnerName);
                Message.WriteInteger((Summon) ? 0 : this.State);
                Message.WriteInteger(this.UsersNow);
                Message.WriteInteger(this.UsersMax);
                Message.WriteString(this.Description);
                Message.WriteInteger(this.TrocStatus); //trading
                Message.WriteInteger(this.Score);
                Message.WriteInteger(0); //Raking ?
                Message.WriteInteger(this.Category);

                Message.WriteInteger(this.TagCount);
                foreach (string s in this.Tags)
                    Message.WriteString(s);

                Message.WriteInteger((this.GroupId > 0 && this.Group != null) ? 58 : (this.OwnerName == "LieuPublic") ? 56 : 56); // 8 = Public room, 2 = appart de groupe, 4 = PromotedRoom

                if (this.GroupId > 0 && this.Group != null)
                {
                    Message.WriteInteger(this.GroupId);
                    Message.WriteString(this.Group.Name);
                    Message.WriteString(this.Group.Badge);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Ligne 400 roomdata : " + ex);
            }
        }

        public void LoadPromotions()
        {
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `room_promotions` WHERE `room_id` = " + Id + " LIMIT 1;");
                DataRow getPromotion = dbClient.GetRow();

                if (getPromotion != null)
                {
                    if (Convert.ToDouble(getPromotion["timestamp_expire"]) > UnixTimestamp.GetNow())
                        Promotion = new RoomPromotion(Convert.ToString(getPromotion["title"]), Convert.ToString(getPromotion["description"]), Convert.ToDouble(getPromotion["timestamp_start"]), Convert.ToDouble(getPromotion["timestamp_expire"]), Convert.ToInt32(getPromotion["category_id"]));
                }
            }
        }

        public bool HasActivePromotion => Promotion != null;

        public void EndPromotion()
        {
            if (!HasActivePromotion)
                return;

            Promotion = null;
        }
    }
}
