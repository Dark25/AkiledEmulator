using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Enemy;
using Akiled.HabboHotel.Roleplay.Player;
using Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;


namespace Akiled.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{

    public class SuperWired : IWired, IWiredEffect, IWiredCycleable
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string message;
        public int Delay { get; set; }
        private bool disposed;

        public SuperWired(string message, int mdelay, bool GodPermission, bool StaffPermission, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = "";

            this.Delay = mdelay;
            this.disposed = false;

            setEffet(message, GodPermission, StaffPermission);
        }

        private void setEffet(string message, bool GodPermission, bool StaffPermission)
        {
            string effet;
            if (message.Contains(":"))
                effet = message.Split(new char[] { ':' })[0].ToLower();
            else
                effet = message;

            switch (effet)
            {
                case "rpsendtimeuser":
                case "rptimespeed":
                case "cyclehoureffect":
                case "setenemy":
                case "enemyaggrostop":
                case "enemyaggrostart":
                case "addenemy":
                case "removeenemy":
                case "userpvp":
                case "pvp":
                case "addmunition":
                case "munition":
                case "rpresetuser":
                case "rpsay":
                case "rpsayme":
                case "rpexp":
                case "rpremoveexp":
                case "removemoney":
                case "addmoney":
                case "work":
                case "health":
                case "healthplus":
                case "hit":
                case "weaponfarid":
                case "weaponcacid":
                case "removeenergy":
                case "addenergy":
                case "allowitemsbuy":
                case "inventoryadd":
                case "inventoryremove":

                case "sendroomid":
                case "botchoose":

                case "playsounduser":
                case "playsoundroom":
                case "playmusicroom":
                case "playmusicuser":
                case "stopsounduser":
                case "stopsoundroom":
                case "forcesound":
                    if (this.handler.GetRoom().IsRoleplay)
                    {
                        this.message = message;
                        return;
                    }
                    break;
            }

            switch (effet)
            {

                case "botchoose":
                case "alert":
                case "send":
                case "enablestaff":
                case "teleportdisabled":
                case "roomingamechat":
                case "jackanddaisy":
                case "openpage":
                case "playsounduser":
                case "playsoundroom":
                case "playmusicroom":
                case "playmusicuser":
                case "stopsounduser":
                case "stopsoundroom":
                case "badge":
                case "roomalert":
                case "forcesound":
                    if (StaffPermission)
                    {
                        this.message = message;
                        return;
                    }
                    break;
                case "achievement":
                case "givelot":
                case "reward":
                case "addreward":
                    if (GodPermission)
                    {
                        this.message = message;
                        return;
                    }
                    break;
            }

            switch (effet)
            {
                case "video":
                case "poll":
                case "roomvideo":
                case "roomfreeze":
                case "roomkick":
                case "moveto":
                case "reversewalk":
                case "speedwalk":
                case "configbot":
                case "rot":
                case "roommute":
                case "enable":
                case "dance":
                case "sit":
                case "lay":
                case "handitem":
                case "setspeed":
                case "freeze":
                case "unfreeze":
                case "roomdiagonal":
                case "roomoblique":
                case "point":
                case "usertimer":
                case "addusertimer":
                case "removeusertimer":
                case "addpoint":
                case "removepoint":
                case "ingame":
                case "setitemmode":
                case "useitem":
                case "addpointteam":
                case "breakwalk":
                case "allowshoot":
                case "transf":
                case "transfstop":
                case "pushpull":
                case "stand":
                    this.message = message;
                    return;
            }

            this.message = "";
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleEffect(user, item);
            return false;
        }

        public bool Disposed()
        {
            return this.disposed;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (string.IsNullOrEmpty(this.message) || this.message == ":")
                return;

            if (this.Delay > 0)
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            else
                this.HandleEffect(user, TriggerItem);
        }

        private void HandleEffect(RoomUser User, Item TriggerItem)
        {
            if (string.IsNullOrEmpty(this.message) || this.message == ":")
                return;

            string Cmd = "";
            string Value = "";

            if (this.message.Contains(":"))
            {
                Cmd = this.message.Split(new char[] { ':' })[0].ToLower();
                Value = this.message.Split(new char[] { ':' })[1];
            }
            else
            {
                Cmd = this.message;
            }

            RpCommand(Cmd, Value, this.handler.GetRoom(), User, TriggerItem);
            UserCommand(Cmd, Value, User, TriggerItem);
            RoomCommand(Cmd, Value, this.handler.GetRoom(), TriggerItem, User);
            BotCommand(Cmd, Value, User, TriggerItem);
        }

        private void RpCommand(string Cmd, string Value, Room Room, RoomUser User, Item TriggerItem)
        {
            if (Room == null || !Room.IsRoleplay)
                return;

            if (User == null || User.GetClient() == null)
                return;

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null)
                return;

            switch (Cmd)
            {
                case "rpsendtimeuser":
                    {
                        User.SendWhisperChat("Il est " + Room.RpHour + " heures et " + Room.RpMinute + " minutes");
                        break;
                    }
                case "setenemy":
                    {
                        string[] Params = Value.Split(';');
                        if (Params.Length != 3)
                            break;

                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Params[0]);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                            break;

                        RPEnemy RPEnemyConfig = null;

                        if (!BotOrPet.IsPet)
                            RPEnemyConfig = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyBot(BotOrPet.BotData.Id);
                        else
                            RPEnemyConfig = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().GetEnemyPet(BotOrPet.BotData.Id);

                        if (RPEnemyConfig == null)
                            break;

                        switch (Params[1])
                        {
                            case "health":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0) ParamInt = 0;
                                    if (ParamInt > 9999) ParamInt = 9999;

                                    RPEnemyConfig.Health = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET health = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");

                                    break;
                                }
                            case "weaponfarid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.WeaponGunId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET weaponFarId = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "weaponcacid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.WeaponCacId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET weaponCacId = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "deadtimer":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.DeadTimer = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET deadTimer = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "lootitemid":
                                {
                                    int ParamInt = 0;
                                    if (!int.TryParse(Params[2], out ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.LootItemId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET lootItemId = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "moneydrop":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt)) break;

                                    if (ParamInt <= 0) ParamInt = 0;
                                    if (ParamInt > 9999) ParamInt = 9999;

                                    RPEnemyConfig.MoneyDrop = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET moneyDrop = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");

                                    break;
                                }
                            case "teamid":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0) ParamInt = 0;
                                    if (ParamInt > 9999) ParamInt = 9999;

                                    RPEnemyConfig.TeamId = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET teamId = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "aggrodistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0) ParamInt = 0;
                                    if (ParamInt > 9999) ParamInt = 9999;

                                    RPEnemyConfig.AggroDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET aggroDistance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");

                                    break;
                                }
                            case "zonedistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.ZoneDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET zoneDistance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "resetposition":
                                {
                                    RPEnemyConfig.ResetPosition = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET resetPosition = '" + AkiledEnvironment.BoolToEnum(RPEnemyConfig.ResetPosition) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "lostaggrodistance":
                                {
                                    if (!int.TryParse(Params[2], out int ParamInt))
                                        break;

                                    if (ParamInt <= 0)
                                        ParamInt = 0;
                                    if (ParamInt > 9999)
                                        ParamInt = 9999;

                                    RPEnemyConfig.LostAggroDistance = ParamInt;
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET lostAggroDistance = '" + ParamInt + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                            case "zombiemode":
                                {
                                    RPEnemyConfig.ZombieMode = (Params[2] == "true");
                                    BotOrPet.BotData.RoleBot.SetConfig(RPEnemyConfig);

                                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        queryreactor.RunQuery("UPDATE roleplay_enemy SET zombieMode = '" + AkiledEnvironment.BoolToEnum(RPEnemyConfig.ZombieMode) + "' WHERE id = '" + RPEnemyConfig.Id + "'");
                                    break;
                                }
                        }
                        break;
                    }
                case "removeenemy":
                    {
                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                            break;

                        if (!BotOrPet.IsPet)
                        {
                            AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyBot(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = RoomBots.AIType.Generic;
                            BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                        }
                        else
                        {
                            AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().RemoveEnemyPet(BotOrPet.BotData.Id);
                            BotOrPet.BotData.RoleBot = null;
                            BotOrPet.BotData.AiType = RoomBots.AIType.Pet;
                            BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                        }
                        break;
                    }
                case "addenemy":
                    {
                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot != null)
                            break;

                        if (!BotOrPet.IsPet)
                        {
                            RPEnemy RPEnemyConfig = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyBot(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = RoomBots.AIType.RolePlayBot;
                                BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                            }
                        }
                        else
                        {
                            RPEnemy RPEnemyConfig = AkiledEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().AddEnemyPet(BotOrPet.BotData.Id);
                            if (RPEnemyConfig != null)
                            {
                                BotOrPet.BotData.RoleBot = new RoleBot(RPEnemyConfig);
                                BotOrPet.BotData.AiType = RoomBots.AIType.RolePlayPet;
                                BotOrPet.BotData.GenerateBotAI(BotOrPet.VirtualId);
                            }
                        }
                        break;
                    }
                case "enemyaggrostop":
                    {
                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                            break;

                        BotOrPet.BotData.RoleBot.ResetAggro();

                        break;
                    }
                case "enemyaggrostart":
                    {
                        RoomUser BotOrPet = Room.GetRoomUserManager().GetBotOrPetByName(Value);
                        if (BotOrPet == null || BotOrPet.BotData == null || BotOrPet.BotData.RoleBot == null)
                            break;

                        BotOrPet.BotData.RoleBot.AggroVirtuelId = User.VirtualId;
                        BotOrPet.BotData.RoleBot.AggroTimer = 0;

                        break;
                    }
                case "sendroomid":
                    {
                        int RoomId;
                        if (int.TryParse(Value, out RoomId))
                        {
                            Room room = AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(RoomId);
                            if (room != null && room.RoomData.OwnerId == Room.RoomData.OwnerId)
                            {
                                User.GetClient().GetHabbo().IsTeleporting = true;
                                User.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                                User.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                            }
                        }
                        break;
                    }
                case "inventoryadd":
                    {
                        int ItemId = 0;
                        int.TryParse(Value, out ItemId);

                        RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
                        if (RpItem == null)
                            break;

                        Rp.AddInventoryItem(RpItem.Id);
                        break;
                    }
                case "inventoryremove":
                    {
                        int ItemId;
                        int.TryParse(Value, out ItemId);

                        RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
                        if (RpItem == null)
                            break;

                        Rp.RemoveInventoryItem(RpItem.Id);
                        break;
                    }

                case "rpresetuser":
                    {
                        Rp.Reset();

                        break;
                    }
                case "userpvp":
                    {
                        if (Value == "true")
                            Rp.PvpEnable = true;
                        else
                            Rp.PvpEnable = false;
                        break;
                    }
                case "allowitemsbuy":
                    {
                        List<RPItem> ItemsList = new List<RPItem>();
                        User.AllowBuyItems.Clear();

                        if (string.IsNullOrEmpty(Value))
                        {
                            Rp.SendItemsList(ItemsList);
                            break;
                        }

                        if (Value.Contains(","))
                        {
                            foreach (string pId in Value.Split(','))
                            {
                                int Id = 0;
                                if (!int.TryParse(pId, out Id))
                                    continue;

                                RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
                                if (RpItem == null)
                                    continue;
                                ItemsList.Add(RpItem);
                                User.AllowBuyItems.Add(Id);
                            }
                        }
                        else
                        {
                            int Id = 0;
                            if (!int.TryParse(Value, out Id))
                                break;

                            RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Id);
                            if (RpItem == null)
                                break;
                            ItemsList.Add(RpItem);
                            User.AllowBuyItems.Add(Id);
                        }

                        Rp.SendItemsList(ItemsList);

                        break;
                    }
                case "removeenergy":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);

                        Rp.RemoveEnergy(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "addenergy":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);

                        Rp.AddEnergy(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "weaponfarid":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb < 0 || Nb > 2)
                            Nb = 0;

                        Rp.WeaponGun = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(Nb);

                        break;
                    }
                case "weaponcacid":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);

                        if (Nb < 0 || Nb > 3)
                            Nb = 0;

                        Rp.WeaponCac = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(Nb);
                        break;
                    }
                case "pvp":
                    {
                        if (Value == "true")
                            Room.Pvp = true;
                        else
                            Room.Pvp = false;
                        break;
                    }
                case "munition":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb > 99)
                            Nb = 99;
                        if (Nb < 0)
                            Nb = 0;

                        Rp.Munition = Nb;

                        Rp.SendUpdate();
                        break;
                    }
                case "addmunition":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);

                        Rp.AddMunition(Nb);
                        Rp.SendUpdate();
                        break;
                    }
                case "removemunition":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);

                        Rp.RemoveMunition(Nb);
                        Rp.SendUpdate();
                        break;
                    }
                case "rpexp":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        Rp.AddExp(Nb);
                        break;
                    }
                case "rpremoveexp":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        Rp.RemoveExp(Nb);
                        break;
                    }
                case "removemoney":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        if (Rp.Money - Nb < 0)
                        {
                            Rp.Money = 0;
                        }
                        else
                        {
                            Rp.Money -= Nb;
                        }
                        Rp.SendUpdate();
                        break;
                    }
                case "addmoney":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        Rp.Money += Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "health":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        if (Nb > Rp.HealthMax)
                            Rp.Health = Rp.HealthMax;
                        else
                            Rp.Health = Nb;
                        Rp.SendUpdate();
                        break;
                    }
                case "healthplus":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        Rp.AddHealth(Nb);

                        Rp.SendUpdate();
                        break;
                    }
                case "hit":
                    {
                        int Nb = 0;
                        int.TryParse(Value, out Nb);
                        if (Nb <= 0)
                            break;

                        Rp.Hit(User, Nb, Room, false, true);
                        Rp.SendUpdate();
                        break;
                    }
                case "rpsay":
                    {
                        User.OnChat(Value, 0, false);
                        break;
                    }
                case "rpsayme":
                    {
                        User.OnChatMe(Value, 0, false);
                        break;
                    }
            }
        }

        private void BotCommand(string Cmd, string Value, RoomUser User, Item TriggerItem)
        {
            if (User == null || !User.IsBot)
                return;

            switch (Cmd)
            {
                case "dance":
                    {
                        int danceid;
                        if (int.TryParse(Value, out danceid))
                        {
                            if (danceid < 0 || danceid > 4)
                                danceid = 0;
                            if (danceid > 0 && User.CarryItemID > 0)
                                User.CarryItem(0);
                            User.DanceId = danceid;
                            User.Room.SendPacket(new DanceComposer(User, danceid));
                        }

                        break;
                    }

                case "handitem":
                    {
                        int carryid;
                        if (int.TryParse(Value, out carryid))
                            User.CarryItem(carryid, true);
                        break;
                    }
                case "sit":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            User.SetStatus("sit", "0.5");

                            User.IsSit = true;
                            User.UpdateNeeded = true;
                        }
                        break;
                    }

                case "lay":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            User.SetStatus("lay", "0.7");

                            User.IsLay = true;
                            User.UpdateNeeded = true;
                        }
                        break;
                    }

                case "stand":
                    {
                        if (User.Statusses.ContainsKey("lay"))
                            User.RemoveStatus("lay");
                        if (User.Statusses.ContainsKey("sit"))
                            User.RemoveStatus("sit");
                        if (User.Statusses.ContainsKey("sign"))
                            User.RemoveStatus("sign");

                        User.UpdateNeeded = true;
                        break;
                    }

                case "enable":
                    {
                        int NumEnable;
                        if (!int.TryParse(Value, out NumEnable))
                            return;

                        if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, false))
                            return;

                        User.ApplyEffect(NumEnable);
                        break;
                    }

                case "breakwalk":
                    {
                        if (Value == "true")
                            User.BreakWalkEnable = true;
                        else
                            User.BreakWalkEnable = false;

                        break;
                    }

                case "freeze":
                    {
                        int Seconde = 0;
                        int.TryParse(Value, out Seconde);
                        Seconde = Seconde * 2;
                        User.Freeze = true;
                        User.FreezeEndCounter = Seconde;
                        break;
                    }
                case "unfreeze":
                    {
                        User.Freeze = false;
                        User.FreezeEndCounter = 0;
                        break;
                    }
            }
        }

        private void RoomCommand(string Cmd, string Value, Room Room, Item TriggerItem, RoomUser User)
        {
            if (Room == null)
                return;

            switch (Cmd)
            {
                case "video":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new YoutubeTvComposer(0, Value));
                        break;
                    }
                case "poll":
                    {
                        ServerPacket MessageTwo = new ServerPacket(ServerPacketHeader.QuestionInfoComposer);
                        MessageTwo.WriteString("MATCHING_POLL"); //Type
                        MessageTwo.WriteInteger(1);//pollId
                        MessageTwo.WriteInteger(1);//questionId
                        MessageTwo.WriteInteger(60);//Duration
                        MessageTwo.WriteInteger(1); //id
                        MessageTwo.WriteInteger(1);//number
                        MessageTwo.WriteInteger(3);//type (1 ou 2)
                        MessageTwo.WriteString(Value);//content
                        MessageTwo.WriteInteger(0);
                        MessageTwo.WriteInteger(0);
                        //MessageTwo.WriteString("0");
                        //MessageTwo.WriteString("1");
                        Room.SendPacket(MessageTwo);

                        Room.VotedNoCount = 0;
                        Room.VotedYesCount = 0;
                        break;
                    }
                case "stoppoll":
                    {
                        ServerPacket Message = new ServerPacket(ServerPacketHeader.SimplePollAnswersComposer);
                        Message.WriteInteger(1);//PollId
                        Message.WriteInteger(2);//Count
                        Message.WriteString("0");//Négatif
                        Message.WriteInteger(Room.VotedNoCount);//Nombre
                        Message.WriteString("1");//Positif
                        Message.WriteInteger(Room.VotedYesCount);//Nombre
                        Room.SendPacket(Message);

                        break;
                    }
                case "roomvideo":
                    {
                        Room.SendPacketWeb(new YoutubeTvComposer(0, Value));
                        break;
                    }
                case "roomfreeze":
                    {
                        Room.FreezeRoom = (Value == "true") ? true : false;
                        break;
                    }
                case "roomkick":
                    {
                        foreach (RoomUser RUser in Room.GetRoomUserManager().GetUserList().ToList())
                        {
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                            {
                                Room.GetRoomUserManager().RemoveUserFromRoom(RUser.GetClient(), true, false);
                            }
                        }
                        break;
                    }
                case "roomalert":
                    {
                        if (Value.Length <= 0) break;

                        foreach (RoomUser RUser in Room.GetRoomUserManager().GetUserList().ToList())
                        {
                            if (RUser != null && !RUser.IsBot && !RUser.GetClient().GetHabbo().HasFuse("fuse_no_kick"))
                            {
                                RUser.GetClient().SendNotification(Value);
                            }
                        }
                        break;
                    }
                case "stopsoundroom":
                    {
                        Room.SendPacketWeb(new StopSoundComposer(Value));
                        break;
                    }
                case "playsoundroom":
                    {
                        Room.SendPacketWeb(new PlaySoundComposer(Value, 1)); //Type = Trax
                        break;
                    }
                case "playmusicroom":
                    {
                        Room.SendPacketWeb(new PlaySoundComposer(Value, 2, true)); //Type = Trax
                        break;
                    }
                case "configbot":
                    {
                        string[] Params = Value.Split(';');

                        RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[0]);
                        if (Bot == null)
                            return;

                        switch (Params[1])
                        {
                            case "enable":
                                {
                                    if (Params.Length < 3) break;

                                    int.TryParse(Params[2], out int IntValue);

                                    if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(IntValue, false)) return;

                                    if (Bot.CurrentEffect != IntValue) Bot.ApplyEffect(IntValue);

                                    if (Bot.BotData.Enable != IntValue) Bot.BotData.Enable = IntValue;

                                    break;
                                }
                            case "handitem":
                                {
                                    if (Params.Length < 3) break;

                                    int.TryParse(Params[2], out int IntValue);

                                    if (Bot.CarryItemID != IntValue) Bot.CarryItem(IntValue, true);

                                    if (Bot.BotData.Handitem != IntValue) Bot.BotData.Handitem = IntValue;

                                    break;
                                }
                            case "rot":
                                {
                                    if (Params.Length < 3) break;

                                    int.TryParse(Params[2], out int IntValue);
                                    IntValue = (IntValue > 7 || IntValue < 0) ? 0 : IntValue;

                                    if (Bot.RotBody != IntValue)
                                    {
                                        Bot.RotBody = IntValue;
                                        Bot.RotHead = IntValue;
                                        Bot.UpdateNeeded = true;
                                    }

                                    if (Bot.BotData.Rot != IntValue) Bot.BotData.Rot = IntValue;

                                    break;
                                }
                            case "sit":
                                {
                                    if (Bot.BotData.Status == 1)
                                    {
                                        Bot.BotData.Status = 0;

                                        Bot.RemoveStatus("sit");
                                        Bot.IsSit = false;
                                        Bot.UpdateNeeded = true;
                                    }
                                    else
                                    {
                                        if (!Bot.IsSit)
                                        {
                                            Bot.SetStatus("sit", (Bot.IsPet) ? "" : "0.5");
                                            Bot.IsSit = true;
                                            Bot.UpdateNeeded = true;
                                        }

                                        Bot.BotData.Status = 1;
                                    }

                                    break;
                                }
                            case "lay":
                                {
                                    if (Bot.BotData.Status == 2)
                                    {
                                        Bot.BotData.Status = 0;

                                        Bot.RemoveStatus("lay");
                                        Bot.IsSit = false;
                                        Bot.UpdateNeeded = true;
                                    }
                                    else
                                    {
                                        if (!Bot.IsLay)
                                        {
                                            Bot.SetStatus("lay", (Bot.IsPet) ? "" : "0.7");
                                            Bot.IsLay = true;
                                            Bot.UpdateNeeded = true;
                                        }

                                        Bot.BotData.Status = 2;
                                    }

                                    break;
                                }
                        }
                        break;
                    }
                case "rptimespeed":
                    {
                        if (!Room.IsRoleplay) break;

                        if (Value == "true") Room.RpTimeSpeed = true;
                        else Room.RpTimeSpeed = false;

                        break;
                    }
                case "cyclehoureffect":
                    {
                        if (!Room.IsRoleplay) break;

                        if (Value == "true") Room.RpCycleHourEffect = true;
                        else Room.RpCycleHourEffect = false;

                        break;
                    }
                case "jackanddaisy":
                    {
                        RoomUser Bot = null;
                        if (AkiledEnvironment.GetRandomNumber(0, 1) == 1)
                            Bot = Room.GetRoomUserManager().GetBotOrPetByName("Jack");
                        else
                            Bot = Room.GetRoomUserManager().GetBotOrPetByName("Daisy");

                        if (Bot == null)
                            break;

                        List<string> Phrases = new List<string>();

                        switch (Value)
                        {
                            case "wait":
                                {

                                    Phrases.Add("Merci de patienter, le jeu va bientôt commencer !");
                                    Phrases.Add("Le jeu ne devrait pas tarder à commencer !");
                                    Phrases.Add("Le jeu va commencer dans quelques minutes !");
                                    Phrases.Add("Le jeu va commencer dans quelques instants");
                                    Phrases.Add("Patience, le jeu va débuter sous peu !");
                                    break;
                                }
                            case "win":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Fichtre... #username# a gagné !");
                                        Phrases.Add("Et c'est ce moussaillon de #username# qui repart avec le trésor !");
                                        Phrases.Add("#username# vient de décrocher une très belle surprise !");
                                    }
                                    else
                                    {
                                        Phrases.Add("Félicitations à #username# qui remporte la partie !");
                                        Phrases.Add("Félicitons #username# qui remporte la partie !");
                                        Phrases.Add("La chance était du côté de #username# aujourd'hui");
                                    }
                                    break;
                                }
                            case "loose":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Oulà ! #username# viens de se faire botter l'arrière train' !");
                                        Phrases.Add("Et #username# qui rejoint l'équipe des loosers");
                                        Phrases.Add("Une défaite en bonne et due forme de #username# !");
                                    }
                                    else
                                    {
                                        Phrases.Add("La prochaine fois tu y arriveras #username#, j'en suis sûre et certain !");
                                        Phrases.Add("Courage #username#, tu y arriveras la prochaine fois !");
                                        Phrases.Add("Ne soit pas triste #username#, d'autres occasions se présenteront à toi !");
                                    }
                                    break;
                                }
                            case "startgame":
                                {
                                    Phrases.Add("Allons y !");
                                    Phrases.Add("C'est parti !");
                                    Phrases.Add("A vos marques, prêts ? Partez !");
                                    Phrases.Add("Let's go!");
                                    Phrases.Add("Ne perdons pas plus de temps !");
                                    Phrases.Add("Que la partie commence !");
                                    break;
                                }
                            case "endgame":
                                {
                                    Phrases.Add("L'animation est terminé, bravo à tout les gagnant(e)s !");
                                    Phrases.Add("L'animation est enfin terminé ! reviens nous voir à la prochaine animation !");
                                    break;
                                }
                            case "fungame":
                                {
                                    if (Bot.BotData.Name == "Jack")
                                    {
                                        Phrases.Add("Mhhhh, le niveau n'est pas très haut...");
                                        Phrases.Add("On sait déjà tous qui sera le grand vaiqueur...");
                                        Phrases.Add("Qui ne tente rien n'a rien");
                                    }
                                    else
                                    {
                                        Phrases.Add("La victoire approche tenez le coup !");
                                        Phrases.Add("C'est pour ça qu'il faut toujours avoir un trèfle à 4 feuilles");
                                        Phrases.Add("En essayant continuellement, on finit par réussir, plus ça rate, plus on a des chances que ça marque ;)");
                                    }
                                    break;
                                }
                        }

                        string TextMessage = Phrases[AkiledEnvironment.GetRandomNumber(0, Phrases.Count - 1)];
                        if (User != null)
                            TextMessage = TextMessage.Replace("#username#", User.GetUsername());

                        Bot.OnChat(TextMessage, 2, true);

                        break;
                    }
                case "roomingamechat":
                    {
                        if (Value == "true")
                            Room.RoomIngameChat = true;
                        else
                            Room.RoomIngameChat = false;
                        break;
                    }
                case "roommute":
                    {
                        if (Value == "true")
                            Room.RoomMuted = true;
                        else
                            Room.RoomMuted = false;
                        break;
                    }
                case "setspeed":
                    {
                        int Vitesse = 0;
                        int.TryParse(Value, out Vitesse);

                        Room.GetRoomItemHandler().SetSpeed(Vitesse);
                        break;
                    }
                case "roomdiagonal":
                    {
                        if (Value == "true")
                            Room.GetGameMap().DiagonalEnabled = true;
                        else
                            Room.GetGameMap().DiagonalEnabled = false;

                        break;
                    }
                case "roomoblique":
                    {
                        if (Value == "true")
                            Room.GetGameMap().ObliqueDisable = true;
                        else
                            Room.GetGameMap().ObliqueDisable = false;

                        break;
                    }

                case "setitemmode":
                    {
                        if (TriggerItem == null)
                            break;
                        int Count = 0;
                        int.TryParse(Value, out Count);

                        if (Count > TriggerItem.GetBaseItem().Modes - 1)
                            break;

                        int result = 0;
                        if (!int.TryParse(TriggerItem.ExtraData, out result))
                            break;

                        TriggerItem.ExtraData = Count.ToString();
                        TriggerItem.UpdateState();
                        Room.GetGameMap().updateMapForItem(TriggerItem);

                        break;
                    }

                case "useitem":
                    {
                        if (TriggerItem == null)
                            break;
                        if (TriggerItem.GetBaseItem().Modes == 0)
                            break;

                        int.TryParse(Value, out int Count);

                        if (!int.TryParse(TriggerItem.ExtraData, out int ItemCount))
                            break;

                        int newCount = (ItemCount + Count < TriggerItem.GetBaseItem().Modes) ? ItemCount + Count : 0;

                        TriggerItem.ExtraData = newCount.ToString();
                        TriggerItem.UpdateState();
                        Room.GetGameMap().updateMapForItem(TriggerItem);

                        break;
                    }


                case "pushpull":
                    {
                        if (Value == "true")
                            Room.PushPullAllowed = true;
                        else
                            Room.PushPullAllowed = false;
                        break;
                    }
            }
        }
        public void Execute(GameClients.GameClient Session, Rooms.Room Room, RoomUser UserRoom, string[] Params, string Cmd)
        {
            Room TargetRoom;

            switch (Cmd)
            {
                case "addreward":
                    {

                        int RewardDiamonds = 0;
                        int Rewardcredits = 0;
                        int RewardDuckets = 0;
                        using (IQueryAdapter dbQuery = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbQuery.SetQuery("SELECT * FROM `game_rewardscash` LIMIT 1");
                            DataTable gUsersTable = dbQuery.GetTable();

                            foreach (DataRow Row in gUsersTable.Rows)
                            {
                                Rewardcredits = Convert.ToInt32(Row["credits"]);
                                RewardDiamonds = Convert.ToInt32(Row["Akiledcoins"]);
                                RewardDuckets = Convert.ToInt32(Row["diamantes"]);
                            }
                        }
                        Session.GetHabbo().Credits += Rewardcredits;
                        Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
                        Session.GetHabbo().Duckets += RewardDuckets;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, RewardDuckets));
                        Session.GetHabbo().AkiledPoints += RewardDiamonds;
                        Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, RewardDiamonds, 105));
                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + RewardDiamonds + " WHERE id = " + Session.GetHabbo().Id + " LIMIT 1");
                        if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out TargetRoom))
                            return;
                        //Session.SendPacket(new RoomForwardComposer(Session.GetHabbo().CurrentRoomId));
                        Session.SendPacket(RoomNotificationComposer.SendBubble("ganador", "Has recibido " + Rewardcredits + " Créditos, " + RewardDuckets + " Diamantes, " + RewardDiamonds + " Kakacoin's por haber ganado el juego o evento.", ""));
                        AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("ganador", "" + Session.GetHabbo().Username + " acaba de ganar el evento. felicitaciones :)", ""));
                        break;
                    }

            }
        }
        private void UserCommand(string Cmd, string Value, RoomUser User, Item TriggerItem)
        {
            if (User == null || User.IsBot || User.GetClient() == null)
                return;

            switch (Cmd)
            {
                case "reward":
                    {
                        if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                            return;

                        User.GetClient().SendPacket(new NuxItemListComposer());
                        break;
                    }
                case "botchoose":
                    {
                        List<string[]> ChooseList = new List<string[]>();
                        if (string.IsNullOrEmpty(Value))
                        {
                            User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));
                            break;
                        }

                        if (Value.Contains(","))
                        {
                            foreach (string pChoose in Value.Split(','))
                            {
                                List<string> list = pChoose.Split(';').ToList();
                                if (list.Count == 3)
                                {
                                    RoomUser BotOrPet = User.Room.GetRoomUserManager().GetBotByName(list[0]);
                                    if (BotOrPet != null && BotOrPet.BotData != null)
                                        list.Add(BotOrPet.BotData.Look);
                                    else
                                        list.Add("");

                                    ChooseList.Add(list.ToArray());
                                }
                            }
                        }
                        else
                        {
                            List<string> list = Value.Split(';').ToList();
                            if (list.Count == 3)
                            {
                                RoomUser BotOrPet = User.Room.GetRoomUserManager().GetBotByName(list[0]);
                                if (BotOrPet != null && BotOrPet.BotData != null)
                                    list.Add(BotOrPet.BotData.Look);
                                else
                                    list.Add("");

                                ChooseList.Add(list.ToArray());
                            }
                        }

                        User.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));

                        break;
                    }
                case "forcesound":
                    {
                        User.GetClient().GetHabbo().ClientVolume.Clear();
                        User.GetClient().GetHabbo().ClientVolume.Add(100);
                        User.GetClient().GetHabbo().ClientVolume.Add(100);
                        User.GetClient().GetHabbo().ClientVolume.Add(100);

                        User.GetClient().GetHabbo().SendWebPacket(new SettingVolumeComposer(100, 100, 100));
                        break;
                    }
                case "stopsounduser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new StopSoundComposer(Value)); //Type = Trax

                        break;
                    }
                case "playsounduser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new PlaySoundComposer(Value, 1)); //Type = furni

                        break;
                    }
                case "playmusicuser":
                    {
                        User.GetClient().GetHabbo().SendWebPacket(new PlaySoundComposer(Value, 2, true)); //Type = Trax

                        break;
                    }
                case "moveto":
                    {
                        if (Value == "true") User.AllowMoveTo = true;
                        else User.AllowMoveTo = false;

                        break;
                    }
                case "reversewalk":
                    {
                        if (Value == "true") User.ReverseWalk = true;
                        else User.ReverseWalk = false;

                        break;
                    }
                case "speedwalk":
                    {
                        if (Value == "true") User.WalkSpeed = true;
                        else User.WalkSpeed = false;

                        break;
                    }
                case "openpage":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/" + Value));
                        break;
                    }
                case "rot":
                    {
                        int.TryParse(Value, out int ValueInt);

                        if (ValueInt > 7 || ValueInt < 0)
                            ValueInt = 0;

                        if (User.RotBody == ValueInt && User.RotHead == ValueInt) break;

                        User.RotBody = ValueInt;
                        User.RotHead = ValueInt;
                        User.UpdateNeeded = true;

                        break;
                    }
                case "stand":
                    {
                        if (User.Statusses.ContainsKey("lay"))
                            User.RemoveStatus("lay");
                        if (User.Statusses.ContainsKey("sit"))
                            User.RemoveStatus("sit");
                        if (User.Statusses.ContainsKey("sign"))
                            User.RemoveStatus("sign");

                        User.UpdateNeeded = true;
                        break;
                    }
                case "allowshoot":
                    {
                        if (Value == "true") User.AllowShoot = true;
                        else User.AllowShoot = false;

                        break;
                    }
                case "addpointteam":
                    {
                        if (User.Team == Games.Team.none) break;

                        int.TryParse(Value, out int Count);

                        if (User.Room == null)
                            break;

                        User.Room.GetGameManager().AddPointToTeam(User.Team, Count, User);
                        break;
                    }
                case "ingame":
                    {
                        if (Value == "true") User.InGame = true;
                        else User.InGame = false;

                        break;
                    }
                case "usertimer":
                    {
                        int.TryParse(Value, out int Points);

                        User.UserTimer = Points;

                        break;
                    }
                case "addusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0) break;

                        User.UserTimer += Points;

                        break;
                    }
                case "removeusertimer":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0)
                            break;

                        if (Points >= User.UserTimer)
                        {
                            User.UserTimer = 0;
                        }
                        else
                        {
                            User.UserTimer -= Points;
                        }

                        break;
                    }
                case "point":
                    {
                        int.TryParse(Value, out int Points);

                        User.WiredPoints = Points;

                        break;
                    }
                case "addpoint":
                    {
                        int.TryParse(Value, out int Points);

                        if (Points == 0) break;

                        User.WiredPoints += Points;

                        break;
                    }

                case "removepoint":
                    {
                        int Points = 0;
                        int.TryParse(Value, out Points);

                        if (Points == 0)
                            break;

                        if (Points >= User.WiredPoints)
                        {
                            User.WiredPoints = 0;
                        }
                        else
                        {
                            User.WiredPoints -= Points;
                        }
                        break;
                    }
                case "freeze":
                    {
                        int Seconde = 0;
                        int.TryParse(Value, out Seconde);
                        Seconde = Seconde * 2;
                        User.Freeze = true;
                        User.FreezeEndCounter = Seconde;
                        break;
                    }
                case "unfreeze":
                    {
                        User.Freeze = false;
                        User.FreezeEndCounter = 0;
                        break;
                    }
                case "breakwalk":
                    {
                        if (Value == "true")
                            User.BreakWalkEnable = true;
                        else
                            User.BreakWalkEnable = false;

                        break;
                    }
                case "enable":
                    {
                        int NumEnable;
                        if (!int.TryParse(Value, out NumEnable))
                            return;

                        if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, false))
                            return;

                        User.ApplyEffect(NumEnable);
                        break;
                    }
                case "enablestaff":
                    {
                        int NumEnable;
                        if (!int.TryParse(Value, out NumEnable))
                            return;

                        if (!AkiledEnvironment.GetGame().GetEffectsInventoryManager().HaveEffect(NumEnable, true))
                            return;

                        User.ApplyEffect(NumEnable);
                        break;
                    }
                case "dance":
                    {
                        if (User.Room == null)
                            break;

                        int danceid;
                        if (int.TryParse(Value, out danceid))
                        {
                            if (danceid < 0 || danceid > 4)
                                danceid = 0;
                            if (danceid > 0 && User.CarryItemID > 0)
                                User.CarryItem(0);
                            User.DanceId = danceid;
                            ServerPacket Message = new ServerPacket(ServerPacketHeader.DanceMessageComposer);
                            Message.WriteInteger(User.VirtualId);
                            Message.WriteInteger(danceid);
                            User.Room.SendPacket(Message);
                        }
                        break;
                    }
                case "handitem":
                    {
                        int carryid;
                        if (int.TryParse(Value, out carryid))
                            User.CarryItem(carryid, true);
                        break;
                    }
                case "sit":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            if (User.transformation)
                                User.SetStatus("sit", "");
                            else
                                User.SetStatus("sit", "0.5");

                            User.IsSit = true;
                            User.UpdateNeeded = true;
                        }
                        break;
                    }

                case "lay":
                    {
                        if (User.RotBody % 2 == 0)
                        {
                            if (User.transformation)
                                User.SetStatus("lay", "");
                            else
                                User.SetStatus("lay", "0.7");

                            User.IsLay = true;
                            User.UpdateNeeded = true;
                        }
                        break;
                    }
                case "transf":
                    {
                        int raceId = 0;
                        string petName = Value;
                        if (Value.Contains(" "))
                        {
                            if (int.TryParse(Value.Split(' ')[1], out raceId))
                            {
                                if (raceId < 1 || raceId > 50)
                                {
                                    raceId = 0;
                                }
                            }

                            petName = Value.Split(' ')[0];
                        }

                        if (User.SetPetTransformation(petName, raceId))
                        {
                            User.transformation = true;

                            User.Room.SendPacket(new UserRemoveComposer(User.VirtualId));
                            User.Room.SendPacket(new UsersComposer(User));
                        }
                        break;
                    }
                case "transfstop":
                    {
                        User.transformation = false;

                        User.Room.SendPacket(new UserRemoveComposer(User.VirtualId));
                        User.Room.SendPacket(new UsersComposer(User));
                        break;
                    }
                case "badge":
                    {
                        User.GetClient().GetHabbo().GetBadgeComponent().GiveBadge(Value, 0, true);
                        User.GetClient().SendPacket(new ReceiveBadgeComposer(Value));
                        break;
                    }
                case "send":
                    {
                        int RoomId;
                        if (int.TryParse(Value, out RoomId))
                        {
                            User.GetClient().GetHabbo().IsTeleporting = true;
                            User.GetClient().GetHabbo().TeleportingRoomID = RoomId;
                            User.GetClient().GetHabbo().PrepareRoom(RoomId, "");
                        }
                        break;
                    }
                case "alert":
                    {
                        User.GetClient().SendNotification(Value);
                        break;
                    }
                case "achievement":
                    {
                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(User.GetClient(), Value, 1);
                        break;
                    }
                case "givelot":
                    {
                        if (User.IsBot || User.GetClient() == null || User.GetClient().GetHabbo() == null || User.GetClient().GetHabbo().Rank > 19)
                            return;
                        if (User.WiredGivelot)
                            return;

                        User.WiredGivelot = true;

                        int ExtraBox_Item = 0;
                        int BadgeBox_Item = 0;
                        using (IQueryAdapter dbQuery = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbQuery.SetQuery("SELECT * FROM `game_configextrabox` LIMIT 1");
                            DataTable gUsersTable = dbQuery.GetTable();

                            foreach (DataRow Row in gUsersTable.Rows)
                            {
                                ExtraBox_Item = Convert.ToInt32(Row["ExtraBox_Item"]);
                                BadgeBox_Item = Convert.ToInt32(Row["BadgeBox_Item"]);
                            }
                        }

                        ItemData ItemData = null;
                        if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(ExtraBox_Item, out ItemData))
                            return;

                        int NbLot = AkiledEnvironment.GetRandomNumber(1, 3);

                        if (User.GetClient().GetHabbo().Rank > 1)
                            NbLot = AkiledEnvironment.GetRandomNumber(3, 5);

                        int NbBadge = AkiledEnvironment.GetRandomNumber(1, 2);
                        if (User.GetClient().GetHabbo().Rank > 1)
                            NbBadge = AkiledEnvironment.GetRandomNumber(2, 3);

                        ItemData ItemDataBadge = null;
                        if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(BadgeBox_Item, out ItemDataBadge))
                            return;

                        List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, User.GetClient().GetHabbo(), "", NbLot);
                        Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, User.GetClient().GetHabbo(), "", NbBadge));

                        foreach (Item PurchasedItem in Items)
                        {
                            if (User.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                            {
                                User.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                            }
                        }
                        User.GetClient().SendNotification(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", User.GetClient().Langue), NbLot, NbBadge));

                        using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                            queryreactor.RunQuery("UPDATE users SET game_points = game_points + 1, games_win = games_win + 1 WHERE id = '" + User.GetClient().GetHabbo().Id + "';");

                        AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(1953042, User.Room.RoomData.OwnerName, User.RoomId, string.Empty, "ultra reward", "SuperWired ultra reward: " + User.GetUsername());
                        break;
                    }
            }

        }

        public void Dispose()
        {
            this.message = (string)null;
            this.disposed = true;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, this.Delay.ToString(), this.message, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, trigger_data_2 FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
                return;
            this.message = row["trigger_data"].ToString();

            int result;
            this.Delay = (int.TryParse(row["trigger_data_2"].ToString(), out result)) ? result : 0;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WiredEffectConfigMessageComposer);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.message);
            Message15.WriteInteger(0);

            Message15.WriteInteger(0);
            Message15.WriteInteger(7);
            Message15.WriteInteger(this.Delay);

            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
