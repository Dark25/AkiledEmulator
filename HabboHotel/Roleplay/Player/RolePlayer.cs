using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Roleplay.Weapon;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Roleplay.Player
{
    public class RolePlayer
    {
        private readonly int _rpId;
        private readonly int _id;

        private readonly ConcurrentDictionary<int, RolePlayInventoryItem> _inventory;

        public int Health;
        public int HealthMax;

        public int Money;
        public int Munition;
        public int GunLoad;
        public int Exp;
        public bool Dead;
        public bool SendPrison;
        public int Level;
        public RPWeapon WeaponGun;
        public RPWeapon WeaponCac;
        public int Energy;
        public int GunLoadTimer;
        public int PrisonTimer;
        public int DeadTimer;
        public int SlowTimer;
        public int AggroTimer;
        public int PlayerOutTimer;
        public bool PvpEnable;

        public int TradeId;
        public bool NeedUpdate;
        public bool Dispose;
        public RoomData RoomData;

        private RoomUserManager roomUserManager;

        public int Id => this.RoomData.Id;

        public RolePlayer(int pRpId, int pId, int pHealth, int pMoney, int pMunition, int pExp, int pEnergy, int pWeaponGun, int pWeaponCac)
        {
            this._rpId = pRpId;
            this._id = pId;
            this.Health = pHealth;
            this.Energy = pEnergy;
            this.Money = pMoney;
            this.Munition = pMunition;
            this.Exp = pExp;
            this.PvpEnable = true;
            this.WeaponCac = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(pWeaponCac);
            this.WeaponGun = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(pWeaponGun);

            this.GunLoad = 6;
            this.GunLoadTimer = 0;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {

                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }
            this.Level = Level;
            this.HealthMax = 90 + (this.Level * 10);

            this.SendPrison = false;
            this.PrisonTimer = 0;
            this.Dead = false;
            this.DeadTimer = 0;

            this.AggroTimer = 0;
            this.SlowTimer = 0;

            this._inventory = new ConcurrentDictionary<int, RolePlayInventoryItem>();

            this.TradeId = 0;
        }

        public void Reset()
        {
            this.Health = 100;
            this.Energy = 100;
            this.Money = 0;
            this.Munition = 0;
            this.Exp = 0;
            this.Level = 1;
            this.HealthMax = 100;

            this.WeaponCac = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(0);
            this.WeaponGun = AkiledEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(0);

            this._inventory.Clear();

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");
                queryreactor.RunQuery("DELETE FROM user_rp WHERE user_id = '" + this._id + "' AND roleplay_id = '" + this._rpId + "'");

                queryreactor.RunQuery("UPDATE `user_rp` SET `health`='" + this.Health + "', `energy`='" + this.Energy + "' , `money`='" + this.Money + "', `munition`='" + this.Munition + "', `exp`='" + this.Exp + "', `weapon_far`='" + this.WeaponGun.Id + "', `weapon_cac`='" + this.WeaponCac.Id + "' WHERE `user_id`='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
            }

            this.SendWebPacket(new LoadInventoryRpComposer(this._inventory));
            this.SendUpdate();
        }

        public void LoadInventory()
        {
            DataTable Table;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM user_rpitems WHERE user_id = '" + this._id + "' AND rp_id = '" + this._rpId + "'");

                queryreactor.AddParameter("userid", this._id);
                Table = queryreactor.GetTable();
            }
            foreach (DataRow dataRow in Table.Rows)
            {
                if (!this._inventory.ContainsKey((int)dataRow["item_id"]))
                    this._inventory.TryAdd((int)dataRow["item_id"], new RolePlayInventoryItem((int)dataRow["id"], (int)dataRow["item_id"], (int)dataRow["count"]));
            }


            this.SendWebPacket(new LoadInventoryRpComposer(this._inventory));
        }

        internal RolePlayInventoryItem GetInventoryItem(int Id)
        {
            this._inventory.TryGetValue(Id, out RolePlayInventoryItem Item);

            return Item;
        }

        internal void AddInventoryItem(int pItemId, int pCount = 1)
        {
            RPItem RPItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(pItemId);
            if (RPItem == null)
                return;

            RolePlayInventoryItem Item = GetInventoryItem(pItemId);
            if (Item == null)
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("INSERT INTO `user_rpitems` (`user_id`, `rp_id`, `item_id`, `count`) VALUES ('" + this._id + "', '" + this._rpId + "', '" + pItemId + "', '" + pCount + "')");
                    int Id = Convert.ToInt32(dbClient.InsertQuery());
                    this._inventory.TryAdd(pItemId, new RolePlayInventoryItem(Id, pItemId, pCount));
                }
            }
            else
            {
                Item.Count += pCount;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("UPDATE user_rpitems SET count = count + '" + pCount + "' WHERE id = '" + Item.Id + "' LIMIT 1");
            }


            this.SendWebPacket(new AddInventoryItemRpComposer(RPItem, pCount));
        }

        internal void RemoveInventoryItem(int ItemId, int Count = 1)
        {
            RolePlayInventoryItem Item = GetInventoryItem(ItemId);
            if (Item == null)
                return;

            if (Item.Count > Count)
            {
                Item.Count = Item.Count - Count;

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("UPDATE user_rpitems SET count = count - '" + Count + "' WHERE id = '" + Item.Id + "' LIMIT 1");
            }
            else
            {
                this._inventory.TryRemove(ItemId, out Item);

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("DELETE FROM user_rpitems WHERE id = '" + Item.Id + "' LIMIT 1");
            }

            this.SendWebPacket(new RemoveItemInventoryRpComposer(ItemId, Count));
        }

        public void SendWebPacket(IServerPacket Message)
        {
            WebClient ClientWeb = AkiledEnvironment.GetGame().GetClientWebManager().GetClientByUserID(this._id);
            if (ClientWeb != null)
            {
                ClientWeb.SendPacket(Message);
            }
        }

        internal void RemoveMunition(int Nb)
        {
            if (this.Munition - Nb <= 0)
                this.Munition = 0;
            else
                this.Munition -= Nb;
        }

        internal void AddMunition(int Nb)
        {
            if (Nb <= 0)
                return;

            if (Nb > 99)
                Nb = 99;

            if (this.Munition + Nb > 99)
                this.Munition = 99;
            else
                this.Munition += Nb;
        }

        internal void AddHealth(int Nb)
        {
            if (Nb <= 0)
                return;

            if (this.Health + Nb > this.HealthMax)
                this.Health = this.HealthMax;
            else
                this.Health += Nb;
        }

        public void AddExp(int pNb)
        {
            this.Exp += pNb;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level < Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }

        public void RemoveExp(int pNb)
        {
            if (this.Exp >= pNb)
                this.Exp -= pNb;
            else
                this.Exp = 0;

            int Level = 1;
            for (int i = 1; i < 100; i++)
            {
                int expmax = (i * 50) + (i * 10) * i;

                if (Exp >= expmax && i < 99)
                {
                    continue;
                }

                Level = i;
                break;
            }

            if (this.Level != Level)
            {
                this.Level = Level;
                this.HealthMax = 90 + (this.Level * 10);
                this.Health = this.HealthMax;
                this.SendUpdate();
            }
        }

        internal void AddEnergy(int Nb)
        {
            if (this.Energy + Nb > 100)
                this.Energy = 100;
            else
                this.Energy += Nb;
        }

        internal void RemoveEnergy(int Nb)
        {
            if (this.Energy - Nb < 0)
                this.Energy = 0;
            else
                this.Energy -= Nb;
        }

        public void Hit(RoomUser User, int Dmg, Room Room, bool Ralentie = false, bool Murmur = false, bool Aggro = true)
        {
            if (this.Dead || this.SendPrison)
                return;

            if (this.Health <= Dmg)
            {
                this.Health = 0;
                this.Dead = true;
                this.DeadTimer = 30;

                User.SetStatus("lay", "0.7");
                User.Freeze = true;
                User.FreezeEndCounter = 0;
                User.IsLay = true;
                User.UpdateNeeded = true;

                if (User.GetClient() != null)
                    User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.userdead", User.GetClient().Langue));

                if (this.Money > 10)
                {
                    int monaiePerdu = (int)Math.Floor((double)(this.Money / 100) * 20);
                    this.Money -= monaiePerdu;

                    Room.GetRoomItemHandler().AddTempItem(User.VirtualId, 5461, User.SetX, User.SetY, User.Z, "1", monaiePerdu, InteractionTypeTemp.MONEY);
                }

                User.OnChat("@red@ Joder me han Matao ! [" + this.Health + "/" + this.HealthMax + "]", 0, true);

                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("UPDATE user_rp SET dead = dead + 1 WHERE user_id = " + User.GetClient().GetHabbo().Id + " LIMIT 1");
            }
            else
            {
                this.Health -= Dmg;
                if (Ralentie)
                {
                    if (this.SlowTimer == 0)
                    {
                        if (User.GetClient() != null)
                            User.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.hitslow", User.GetClient().Langue));
                    }
                    this.SlowTimer = 6;
                }

                if (Aggro) this.AggroTimer = 30;

                if (User.GetClient() != null)
                {
                    if (Murmur)
                        User.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), false);
                    else
                        User.OnChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("rp.hit", User.GetClient().Langue), this.Health, this.HealthMax, Dmg), 0, true);
                    using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        queryreactor.RunQuery("UPDATE user_rp SET killed = killed + 1 WHERE user_id = " + User.GetClient().GetHabbo().Id + " LIMIT 1");

                }
            }

            this.SendUpdate();
        }

        public void SendUpdate(bool SendNow = false)
        {
            if (SendNow)
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
            else
                this.NeedUpdate = true;
        }

        public void SendItemsList(List<RPItem> ItemsList) => this.SendWebPacket(new BuyItemsListComposer(ItemsList));

        public void OnCycle(RoomUser User, RolePlayerManager RPManager)
        {

            if (this.SlowTimer > 0)
            {
                this.SlowTimer--;

                User.BreakWalkEnable = true;
            }
            else
            {
                User.BreakWalkEnable = false;
            }

            if (this.PlayerOutTimer > 0)
            {
                this.PlayerOutTimer--;

                if (PlayerOutTimer == 0)
                {
                    this.AddEnergy(10);
                    this.NeedUpdate = true;

                    if (!this.Dead && !this.SendPrison)
                    {
                        User.Freeze = false;
                        User.IsSit = false;
                        User.RemoveStatus("sit");
                        User.UpdateNeeded = true;
                    }
                }
            }
            else
            {
                if (this.Energy <= 0)
                {
                    this.PlayerOutTimer = 60;

                    User.RotBody = 2;
                    User.RotHead = 2;
                    User.Freeze = true;
                    User.FreezeEndCounter = 0;
                    User.OnChat("*Estoy sin energía*");
                    User.SetStatus("sit", "0.5");
                    User.IsSit = true;
                    User.UpdateNeeded = true;

                    User.SendWhisperChat("Te caíste, descansa 30 segundos", true);
                }
            }


            if (this.GunLoadTimer > 0)
            {
                this.GunLoadTimer--;
                if (this.GunLoadTimer == 0)
                {
                    this.GunLoad = 6;
                }
            }
            else
            {
                if (this.GunLoad == 0)
                {
                    this.GunLoadTimer = 6;
                    User.OnChat("@green@ *Recargando mi Arma*");
                    this.SendPacketWeb(new PlaySoundComposer("recargauser", 2)); //Type = Trax
                }
            }


            if (this.AggroTimer > 0)
                this.AggroTimer--;

            if (this.SendPrison)
            {
                if (this.PrisonTimer > 0)
                    this.PrisonTimer--;
                else
                {
                    this.SendPrison = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.PrisonId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.PrisonId);
                }
            }

            if (this.Dead)
            {
                if (this.DeadTimer > 0)
                    this.DeadTimer--;
                else
                {
                    this.Dead = false;
                    User.GetClient().GetHabbo().IsTeleporting = true;
                    User.GetClient().GetHabbo().TeleportingRoomID = RPManager.HopitalId;
                    User.GetClient().GetHabbo().PrepareRoom(RPManager.HopitalId);
                }
            }

            if (this.NeedUpdate)
            {
                this.NeedUpdate = false;
                this.SendWebPacket(new RpStatsComposer((!this.Dispose) ? this._rpId : 0, this.Health, this.HealthMax, this.Energy, this.Money, this.Munition, this.Level));
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

        public void Destroy()
        {
            if (this.Dispose)
                return;

            this.Dispose = true;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE `user_rp` SET `health`='" + this.Health + "', `energy`='" + this.Energy + "' , `money`='" + this.Money + "', `munition`='" + this.Munition + "', `exp`='" + this.Exp + "', `weapon_far`='" + this.WeaponGun.Id + "', `weapon_cac`='" + this.WeaponCac.Id + "' WHERE `user_id`='" + this._id + "' AND roleplay_id = '" + this._rpId + "' LIMIT 1");
            }

            this.SendWebPacket(new RpStatsComposer(0, 0, 0, 0, 0, 0, 0));
            this._inventory.Clear();
        }
    }
}