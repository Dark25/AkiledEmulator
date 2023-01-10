using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;

namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Conditions
{
    public class SuperWiredCondition : IWiredCondition, IWired
    {
        private Item item;
        private string Effet;
        private string Value;

        public SuperWiredCondition(Item item, string message, bool StaffPermission)
        {
            this.item = item;

            string effet = "";
            if (message.Contains(":"))
            {
                effet = message.Split(new char[] { ':' })[0].ToLower();
            }
            else
                effet = message.ToLower();

            switch (effet)
            {
                case "enemy":
                case "work":
                case "notwork":
                case "moneyplus":
                case "moneymoins":
                case "levelplus":
                case "levelmoins":
                case "healthplus":
                case "healthmoins":
                case "health":
                case "dead":
                case "notdead":
                case "munition":
                case "munitionplus":
                case "munitionmoins":
                case "weaponfarid":
                case "notweaponfarid":
                case "weaponcacid":
                case "notweaponcacid":
                case "energyplus":
                case "energymoins":
                case "inventoryitem":
                case "inventorynotitem":
                case "rphourplus":
                case "rphourmoins":
                case "rphour":
                case "rpminuteplus":
                case "rpminutemoins":
                case "rpminute":
                    if (this.item.GetRoom().IsRoleplay)
                    {
                        this.Effet = effet;
                        if (message.Contains(":"))
                            this.Value = message.Split(new char[] { ':' })[1];
                    }
                    else
                    {
                        this.Effet = "";
                        this.Value = "";
                    }
                    break;

                case "rankplus":
                case "rankmoin":
                case "rank":
                    if (StaffPermission)
                    {
                        this.Effet = effet;
                        if (message.Contains(":"))
                            this.Value = message.Split(new char[] { ':' })[1];
                    }
                    else
                    {
                        this.Effet = "";
                        this.Value = "";
                    }
                    break;
                case "favogroupid":
                case "notfavogroupid":
                case "mission":
                case "notmission":
                case "missioncontais":
                case "notmissioncontais":
                case "usergirl":
                case "notusergirl":
                case "userboy":
                case "notuserboy":
                case "namebot":
                case "notnamebot":
                case "badge":
                case "notbadge":
                case "handitem":
                case "nothanditem":
                case "enable":
                case "notenable":
                case "username":
                case "notusername":
                case "transf":
                case "nottransf":
                case "userteam":
                case "usernotteam":
                case "ingroup":
                case "innotgroup":
                case "rot":
                case "notrot":
                case "lay":
                case "notlay":
                case "sit":
                case "notsit":
                case "usertimer":
                case "usertimerplus":
                case "usertimermoins":
                case "point":
                case "pointplus":
                case "pointmoins":
                case "ingame":
                case "notingame":
                case "freeze":
                case "notfreeze":
                case "winteam":
                case "notwinteam":
                case "allowshoot":
                case "notallowshoot":
                case "isbot":
                case "notisbot":

                case "roomopen":
                case "roomnotopen":
                case "roomclose":
                case "roomnotclose":
                case "teamredcount":
                case "teamrednotcount":
                case "teamyellowcount":
                case "teamyellownotcount":
                case "teambluecount":
                case "teambluenotcount":
                case "teamgreencount":
                case "teamgreennotcount":
                case "teamallcount":
                case "teamallnotcount":

                case "itemmode":
                case "itemnotmode":
                case "itemrot":
                case "itemnotrot":
                case "itemdistanceplus":
                case "itemdistancemoins":
                    this.Effet = effet;
                    if (message.Contains(":"))
                        this.Value = message.Split(new char[] { ':' })[1];
                    break;
                default:
                    this.Effet = "";
                    this.Value = "";
                    break;
            }
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            if (this.Effet == "")
                return false;

            bool Bool = false;
            if (user != null)
                Bool = UserCommand(user);

            if (Bool == false)
                Bool = RoomCommand(this.item.GetRoom());

            if (Bool == false)
                Bool = RpUserCommand(user);

            if (Bool == false)
                Bool = RpGlobalCommand(this.item.GetRoom());

            if (Bool == false && TriggerItem != null)
                Bool = ItemCommand(TriggerItem, user);

            if (this.Effet.Contains("not"))
                Bool = !Bool;

            return Bool;
        }

        private bool RpGlobalCommand(Room Room)
        {
            if (Room == null || !Room.IsRoleplay)
                return false;

            bool Result = false;
            switch (this.Effet)
            {
                case "rpminuteplus":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpMinute >= ValueInt)
                            Result = true;

                        break;
                    }
                case "rpminutemoins":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpMinute < ValueInt)
                            Result = true;

                        break;
                    }
                case "rpminute":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpMinute == ValueInt)
                            Result = true;

                        break;
                    }
                case "rphourplus":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpHour >= ValueInt)
                            Result = true;

                        break;
                    }
                case "rphourmoins":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpHour < ValueInt)
                            Result = true;

                        break;
                    }
                case "rphour":
                    {
                        if (!int.TryParse(this.Value, out int ValueInt))
                            break;

                        if (Room.RpHour == ValueInt)
                            Result = true;

                        break;
                    }
                case "enemy":
                    {
                        string[] Params = Value.Split(';');
                        if (Params.Length != 3)
                            break;

                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Params[0]);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                            break;

                        switch (Params[1])
                        {
                            case "dead":
                                {
                                    if (BotOrPet.BotData.RoleBot.Dead && Params[2] == "true")
                                        Result = true;

                                    if (!BotOrPet.BotData.RoleBot.Dead && Params[2] == "false")
                                        Result = true;
                                    break;
                                }
                            case "aggro":
                                {
                                    if (BotOrPet.BotData.RoleBot.AggroVirtuelId > 0 && Params[2] == "true")
                                        Result = true;

                                    if (BotOrPet.BotData.RoleBot.AggroVirtuelId == 0 && Params[2] == "false")
                                        Result = true;
                                    break;
                                }
                        }
                        break;
                    }
            }

            return Result;
        }

        private bool RpUserCommand(RoomUser User)
        {
            Room Room = this.item.GetRoom();
            if (Room == null || !Room.IsRoleplay)
                return false;

            if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                return false;

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null)
                return false;

            bool Result = false;
            switch (this.Effet)
            {
                case "inventoryitem":
                case "inventorynotitem":
                    {
                        int ValueInt = 0;
                        if (!int.TryParse(this.Value, out ValueInt))
                            break;

                        if (Rp.GetInventoryItem(ValueInt) != null)
                            Result = true;

                        break;
                    }
                case "energyplus":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.Energy >= ValueInt)
                            Result = true;
                        break;
                    }
                case "energymoins":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.Energy < ValueInt)
                            Result = true;
                        break;
                    }
                case "munition":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.Munition == ValueInt)
                            Result = true;
                        break;
                    }
                case "munitionplus":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.Munition >= ValueInt)
                            Result = true;
                        break;
                    }
                case "munitionmoins":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.Munition < ValueInt)
                            Result = true;
                        break;
                    }
                case "moneyplus":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);
                        if (Rp.Money >= ValueInt)
                            Result = true;
                        break;
                    }
                case "moneymoins":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);
                        if (Rp.Money < ValueInt)
                            Result = true;
                        break;
                    }
                case "levelplus":
                    {
                        int.TryParse(this.Value, out int ValueInt);
                        if (Rp.Level >= ValueInt)
                            Result = true;
                        break;
                    }
                case "levelmoins":
                    {
                        int.TryParse(this.Value, out int ValueInt);
                        if (Rp.Level < ValueInt)
                            Result = true;
                        break;
                    }
                case "healthplus":
                    {
                        int.TryParse(this.Value, out int ValueInt);
                        if (Rp.Health >= ValueInt)
                            Result = true;
                        break;
                    }
                case "healthmoins":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);
                        if (Rp.Health < ValueInt)
                            Result = true;
                        break;
                    }
                case "health":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);
                        if (Rp.Health == ValueInt)
                            Result = true;
                        break;
                    }
                case "dead":
                case "notdead":
                    {
                        if (Rp.Dead == true)
                            Result = true;
                        break;
                    }
                case "weaponfarid":
                case "notweaponfarid":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.WeaponGun.Id == ValueInt)
                            Result = true;

                        break;
                    }
                case "weaponcacid":
                case "notweaponcacid":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Rp.WeaponCac.Id == ValueInt)
                            Result = true;

                        break;
                    }
            }
            return Result;
        }

        private bool ItemCommand(Item item, RoomUser User)
        {
            bool Bool = false;
            switch (this.Effet)
            {
                case "itemmode":
                case "itemnotmode":
                    {
                        int Num = 0;
                        if (int.TryParse(item.ExtraData, out Num))
                        {
                            if (item.ExtraData == Value)
                                Bool = true;

                        }
                        break;
                    }
                case "itemrot":
                case "itemnotrot":
                    {
                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (item.Rotation == ValueInt)
                            Bool = true;

                        break;
                    }
                case "itemdistanceplus":
                    {
                        if (User == null)
                            break;

                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Math.Abs(User.X - item.GetX) >= ValueInt && Math.Abs(User.Y - item.GetY) >= ValueInt)
                            Bool = true;
                        break;
                    }
                case "itemdistancemoins":
                    {
                        if (User == null)
                            break;

                        int ValueInt = 0;
                        int.TryParse(this.Value, out ValueInt);

                        if (Math.Abs(User.X - item.GetX) <= ValueInt && Math.Abs(User.Y - item.GetY) <= ValueInt)
                            Bool = true;
                        break;
                    }
            }

            return Bool;
        }

        private bool RoomCommand(Room room)
        {
            if (room == null)
                return false;

            bool Bool = false;
            switch (this.Effet)
            {
                case "roomopen":
                case "roomnotopen":
                    {
                        if (room.RoomData.State == 0)
                            Bool = true;

                        break;
                    }
                case "roomclose":
                case "roomnotclose":
                    {
                        if (room.RoomData.State == 1)
                            Bool = true;

                        break;
                    }
                case "teamallcount":
                case "teamallnotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int Count = 0;
                        int.TryParse(this.Value, out Count);
                        if (TeamManager.GetAllPlayer().Count == Count)
                            Bool = true;
                        break;
                    }
                case "teamredcount":
                case "teamrednotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int Count = 0;
                        int.TryParse(this.Value, out Count);
                        if (TeamManager.RedTeam.Count == Count)
                            Bool = true;
                        break;
                    }
                case "teamyellowcount":
                case "teamyellownotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int Count = 0;
                        int.TryParse(this.Value, out Count);
                        if (TeamManager.YellowTeam.Count == Count)
                            Bool = true;
                        break;
                    }
                case "teambluecount":
                case "teambluenotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int Count = 0;
                        int.TryParse(this.Value, out Count);
                        if (TeamManager.BlueTeam.Count == Count)
                            Bool = true;
                        break;
                    }
                case "teamgreencount":
                case "teamgreennotcount":
                    {
                        TeamManager TeamManager = room.GetTeamManager();

                        int Count = 0;
                        int.TryParse(this.Value, out Count);
                        if (TeamManager.GreenTeam.Count == Count)
                            Bool = true;
                        break;
                    }
            }
            return Bool;
        }

        private bool UserCommand(RoomUser user)
        {
            bool Result = false;
            switch (this.Effet)
            {
                case "missioncontais":
                case "notmissioncontais":
                    {
                        if (!user.IsBot && user.GetClient().GetHabbo().Motto.Contains(Value))
                            Result = true;
                        break;
                    }
                case "mission":
                case "notmission":
                    {
                        if (!user.IsBot && user.GetClient().GetHabbo().Motto == Value)
                            Result = true;
                        break;
                    }
                case "favogroupid":
                case "notfavogroupid":
                    {
                        int GroupId = 0;
                        int.TryParse(Value, out GroupId);

                        if (!user.IsBot && user.GetClient().GetHabbo().FavouriteGroupId == GroupId)
                            Result = true;
                        break;
                    }
                case "usergirl":
                case "notusergirl":
                    {
                        if (!user.IsBot && user.GetClient().GetHabbo().Gender.ToUpper() == "F")
                            Result = true;
                        break;
                    }
                case "userboy":
                case "notuserboy":
                    {
                        if (!user.IsBot && user.GetClient().GetHabbo().Gender.ToUpper() == "M")
                            Result = true;
                        break;
                    }
                case "namebot":
                case "notnamebot":
                    {
                        if (user.IsBot && user.BotData.Name == Value)
                            Result = true;
                        break;
                    }
                case "isbot":
                case "notisbot":
                    {
                        if (user.IsBot)
                            Result = true;
                        break;
                    }
                case "allowshoot":
                case "notallowshoot":
                    {
                        if (user.AllowShoot)
                            Result = true;

                        break;
                    }
                case "winteam":
                case "notwinteam":
                    {
                        if (user.Team == Team.none)
                            break;

                        Room room = this.item.GetRoom();
                        if (room == null)
                            break;

                        Team winningTeam = room.GetGameManager().getWinningTeam();
                        if (user.Team == winningTeam)
                            Result = true;
                        break;
                    }
                case "freeze":
                case "notfreeze":
                    {
                        if (user.Freeze)
                            Result = true;

                        break;
                    }
                case "ingame":
                case "notingame":
                    {
                        if (user.InGame)
                            Result = true;

                        break;
                    }
                case "usertimer":
                    {
                        int Points = 0;
                        int.TryParse(Value, out Points);

                        if (user.UserTimer == Points)
                            Result = true;

                        break;
                    }
                case "usertimerplus":
                    {
                        int Points = 0;
                        int.TryParse(Value, out Points);

                        if (user.UserTimer > Points)
                            Result = true;

                        break;
                    }
                case "usertimermoins":
                    {
                        int.TryParse(Value, out int point);

                        if (user.UserTimer < point)
                            Result = true;

                        break;
                    }
                case "point":
                    {
                        int.TryParse(Value, out int point);

                        if (user.WiredPoints == point)
                            Result = true;

                        break;
                    }
                case "pointplus":
                    {
                        int Points = 0;
                        int.TryParse(Value, out Points);

                        if (user.WiredPoints > Points)
                            Result = true;

                        break;
                    }
                case "pointmoins":
                    {
                        int Points = 0;
                        int.TryParse(Value, out Points);

                        if (user.WiredPoints < Points)
                            Result = true;

                        break;
                    }
                case "ingroup":
                case "innotgroup":
                    {
                        int GroupId = 0;
                        int.TryParse(Value, out GroupId);

                        if (GroupId == 0)
                            break;
                        if (user.IsBot || user.GetClient() != null && user.GetClient().GetHabbo() != null && user.GetClient().GetHabbo().MyGroups.Contains(GroupId))
                            Result = true;

                        break;
                    }
                case "userteam":
                case "usernotteam":
                    {
                        if (user.Team != Team.none)
                            Result = true;
                        break;
                    }
                case "sit":
                case "notsit":
                    {
                        if (user.Statusses.ContainsKey("sit"))
                            Result = true;
                        break;
                    }
                case "lay":
                case "notlay":
                    {
                        if (user.Statusses.ContainsKey("lay"))
                            Result = true;
                        break;
                    }
                case "rot":
                case "notrot":
                    {
                        if (user.RotBody.ToString() == this.Value)
                            Result = true;
                        break;
                    }
                case "transf":
                case "nottransf":
                    {
                        if (user.transformation)
                            Result = true;
                        break;
                    }
                case "username":
                case "notusername":
                    {
                        if (user.GetUsername() == this.Value)
                            Result = true;
                        break;
                    }
                case "handitem":
                case "nothanditem":
                    {
                        if (user.CarryItemID.ToString() == this.Value)
                            Result = true;
                        break;
                    }
                case "badge":
                case "notbadge":
                    {
                        if (user.IsBot || user.GetClient() == null || user.GetClient().GetHabbo() == null || user.GetClient().GetHabbo().GetBadgeComponent() == null)
                            break;
                        if (user.GetClient().GetHabbo().GetBadgeComponent().HasBadge(this.Value))
                            Result = true;
                        break;
                    }
                case "enable":
                case "notenable":
                    {
                        if (user.CurrentEffect.ToString() == this.Value)
                            Result = true;
                        break;
                    }
                case "rank":
                    {
                        if (user.IsBot)
                            break;
                        if (user.GetClient().GetHabbo().Rank.ToString() == this.Value)
                            Result = true;
                        break;
                    }
                case "rankplus":
                    {
                        if (user.IsBot)
                            break;
                        if (user.GetClient().GetHabbo().Rank > Convert.ToInt32(this.Value))
                            Result = true;
                        break;
                    }
                case "rankmoin":
                    {
                        if (user.IsBot)
                            break;
                        if (user.GetClient().GetHabbo().Rank < Convert.ToInt32(this.Value))
                            Result = true;
                        break;
                    }
            }
            return Result;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, this.Effet + ":" + this.Value, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.item.Id);
            string message = dbClient.GetString();

            if (message.Contains(":"))
            {
                this.Effet = message.Split(new char[] { ':' })[0].ToLower();
                this.Value = message.Split(new char[] { ':' })[1];
            }
            else
                this.Effet = message.ToLower();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0); //Max count item
            Message15.WriteInteger(0); //Item Count
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.item.Id);
            Message15.WriteString(this.Effet + ":" + this.Value);
            Message15.WriteInteger(0); //Loop

            Message15.WriteInteger(0);
            Message15.WriteInteger(7);
            Message15.WriteInteger(0);

            Message15.WriteInteger(0);

            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }

        public void Dispose()
        {
            this.item = (Item)null;
            this.Value = "";
            this.Effet = "";
        }
    }
}
