using Aki.HabboHotel.Items;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Users.Inventory.Bots;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Users.Inventory
{
    public class InventoryComponent
    {
        private ConcurrentDictionary<int, Item> _UserItems;
        private ConcurrentDictionary<int, Pet> _petsItems;
        private ConcurrentDictionary<int, Bot> _botItems;

        private GameClient _client;

        public bool inventoryDefined;
        public int UserId;
        public int InventaryUserId;


        public InventoryComponent(int UserId, GameClient Client)
        {
            this._client = Client;
            this.UserId = UserId;

            this._UserItems = new ConcurrentDictionary<int, Item>();
            this._petsItems = new ConcurrentDictionary<int, Pet>();
            this._botItems = new ConcurrentDictionary<int, Bot>();
        }

        public void Init()
        {
            if (this._UserItems.Count > 0)
                this._UserItems.Clear();
            if (this._petsItems.Count > 0)
                this._petsItems.Clear();
            if (this._botItems.Count > 0)
                this._botItems.Clear();

            List<Item> Items = ItemLoader.GetItemsForUser(GetUserInventaryId());
            foreach (Item Item in Items.ToList())
            {
                if (Item.IsFloorItem)
                {
                    if (!this._UserItems.TryAdd(Item.Id, Item))
                        continue;
                }
                else
                    continue;
            }

        }

        public void ClearItems(bool All = false)
        {
            if (All)
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + UserId + "'");

                this._UserItems.Clear();
            }
            else
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("DELETE items, items_limited, user_presents, room_items_moodlight, tele_links, wired_items FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = '0' AND user_id = '" + UserId + "' AND base_item NOT IN (SELECT id FROM furniture WHERE is_rare = '1');");

                this._UserItems.Clear();
                DataTable table1;
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("SELECT items.id, items.user_id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");

                    queryreactor.AddParameter("userid", this.UserId);
                    table1 = queryreactor.GetTable();
                }

                foreach (DataRow dataRow in table1.Rows)
                {
                    int Id = Convert.ToInt32(dataRow[0]);
                    int fOwnerId = Convert.ToInt32(dataRow[1]);
                    int BaseItem = Convert.ToInt32(dataRow[2]);
                    string ExtraData = DBNull.Value.Equals(dataRow[3]) ? string.Empty : (string)dataRow[3];
                    int Limited = DBNull.Value.Equals(dataRow[4]) ? 0 : Convert.ToInt32(dataRow[4]);
                    int LimitedTo = DBNull.Value.Equals(dataRow[5]) ? 0 : Convert.ToInt32(dataRow[5]);

                    Item userItem = new Item(Id, fOwnerId, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                    this._UserItems.TryAdd(Id, userItem);
                }
            }

            this.GetClient().SendPacket(new FurniListUpdateComposer());
        }

        public void UpdateItems(bool FromDatabase)
        {
            if (FromDatabase)
                Init();

            if (_client != null)
            {
                _client.SendMessage(new FurniListUpdateComposer());
            }
        }

        internal Item GetFirstItemByBaseId(int id) => this._UserItems.Values.Where<Item>((Func<Item, bool>)(item => item != null && item.GetBaseItem() != null && item.GetBaseItem().Id == id)).FirstOrDefault<Item>();

        private int GetUserInventaryId()
        {
            if (InventaryUserId > 0)
                return InventaryUserId;
            else
                return UserId;
        }

        public void ClearPets()
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM user_pets WHERE room_id = '0' AND user_id = " + UserId);
            }

            this._petsItems.Clear();
        }

        public void ClearBots()
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM bots WHERE room_id = '0' AND user_id = " + UserId);
            }

            this._botItems.Clear();
        }

        public void SetActiveState(GameClient client)
        {
            this._client = client;
        }

        public void Destroy()
        {
            _client = null;
            _UserItems.Clear();
            _petsItems.Clear();
            _botItems.Clear();
        }

        public void LoadUserInventory(int InventaryId)
        {
            // dont need to update Inventary?

            InventaryUserId = InventaryId;
            UpdateItems(true);
        }

        public ICollection<Pet> GetPets()
        {
            return this._petsItems.Values;
        }

        public bool TryAddPet(Pet Pet)
        {
            Pet.RoomId = 0;
            Pet.PlacedInRoom = false;

            return this._petsItems.TryAdd(Pet.PetId, Pet);
        }

        public bool TryRemovePet(int PetId, out Pet PetItem)
        {
            if (this._petsItems.ContainsKey(PetId))
                return this._petsItems.TryRemove(PetId, out PetItem);
            else
            {
                PetItem = null;
                return false;
            }
        }

        public bool TryGetPet(int PetId, out Pet Pet)
        {
            if (this._petsItems.ContainsKey(PetId))
                return this._petsItems.TryGetValue(PetId, out Pet);
            else
            {
                Pet = null;
                return false;
            }
        }

        public bool TryGetBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
                return this._botItems.TryGetValue(BotId, out Bot);
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryRemoveBot(int BotId, out Bot Bot)
        {
            if (this._botItems.ContainsKey(BotId))
                return this._botItems.TryRemove(BotId, out Bot);
            else
            {
                Bot = null;
                return false;
            }
        }

        public bool TryAddBot(Bot Bot)
        {
            return this._botItems.TryAdd(Bot.Id, Bot);
        }

        public ICollection<Bot> GetBots()
        {
            return this._botItems.Values;
        }

        public bool TryAddItem(Item item)
        {
            if (_client != null)
                _client.SendPacket(new FurniListAddComposer(item));
            return this._UserItems.TryAdd(item.Id, item);
        }

        public void LoadInventory()
        {
            this._UserItems.Clear();
            DataTable table1;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT items.id, items.user_id, items.base_item, items.extra_data, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.user_id = @userid AND items.room_id = '0'");

                queryreactor.AddParameter("userid", this.UserId);
                table1 = queryreactor.GetTable();
            }

            foreach (DataRow dataRow in table1.Rows)
            {
                int Id = Convert.ToInt32(dataRow[0]);
                int fOwnerId = Convert.ToInt32(dataRow[1]);
                int BaseItem = Convert.ToInt32(dataRow[2]);
                string ExtraData = DBNull.Value.Equals(dataRow[3]) ? string.Empty : (string)dataRow[3];
                int Limited = DBNull.Value.Equals(dataRow[4]) ? 0 : Convert.ToInt32(dataRow[4]);
                int LimitedTo = DBNull.Value.Equals(dataRow[5]) ? 0 : Convert.ToInt32(dataRow[5]);

                Item userItem = new Item(Id, fOwnerId, 0, BaseItem, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, "", null);
                this._UserItems.TryAdd(Id, userItem);
            }

            this._petsItems.Clear();
            DataTable table2;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT id, user_id, room_id, name, type, race, color, expirience, energy, nutrition, respect, createstamp, x, y, z, have_saddle, hairdye, pethair, anyone_ride FROM user_pets WHERE user_id = " + this.UserId + " AND room_id = 0");
                table2 = queryreactor.GetTable();
            }
            if (table2 != null)
            {
                foreach (DataRow Row in table2.Rows)
                {
                    Pet pet = new Pet(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), (string)Row["name"], Convert.ToInt32(Row["type"]), (string)Row["race"], (string)Row["color"], (int)Row["expirience"], (int)Row["energy"], (int)Row["nutrition"], (int)Row["respect"], (double)Row["createstamp"], (int)Row["x"], (int)Row["y"], (double)Row["z"], (int)Row["have_saddle"], (int)Row["hairdye"], (int)Row["pethair"], (string)(Row["anyone_ride"]) == "1");
                    this._petsItems.TryAdd(pet.PetId, pet);
                }
            }

            this._botItems.Clear();
            DataTable dBots;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM bots WHERE user_id = " + this.UserId + " AND room_id = 0");
                dBots = queryreactor.GetTable();
            }
            if (dBots == null)
                return;
            foreach (DataRow Row in dBots.Rows)
            {
                this._botItems.TryAdd(Convert.ToInt32(Row["id"]), new Bot(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), (string)Row["name"], (string)Row["motto"], (string)Row["look"], (string)Row["gender"], (string)Row["walk_enabled"] == "1", (string)Row["chat_enabled"] == "1", (string)Row["chat_text"], (int)Row["chat_seconds"], (string)Row["is_dancing"] == "1", (int)Row["enable"], (int)Row["handitem"], Convert.ToInt32((string)Row["status"])));
            }
        }


        public int getFloorInventoryAmount()
        {
            return this._UserItems.Count;
        }

        public Item GetItem(int Id)
        {
            if (this._UserItems.ContainsKey(Id))
                return this._UserItems[Id];
            else
                return null;
        }

        public Item AddNewItem(int Id, int BaseItem, string ExtraData, int Limited = 0, int LimitedStack = 0)
        {
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this.UserId + "' WHERE id = " + Id);

            Item userItem = new Item(Id, UserId, 0, BaseItem, ExtraData, Limited, LimitedStack, 0, 0, 0.0, 0, "", null);
            if (this.UserHoldsItem(Id))
                this.RemoveItem(Id);

            if (!_UserItems.ContainsKey(userItem.Id))
                this._UserItems.TryAdd(userItem.Id, userItem);

            if (_client != null)
                _client.SendPacket(new FurniListAddComposer(userItem));

            return userItem;
        }

        private bool UserHoldsItem(int itemID)
        {
            return this._UserItems.ContainsKey(itemID);
        }

        public void RemoveItem(int Id)
        {
            if (this._UserItems.ContainsKey(Id))
            {
                Item ToRemove = null;
                this._UserItems.TryRemove(Id, out ToRemove);
            }

            this.GetClient().SendPacket(new FurniListRemoveComposer(Id));
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return this._UserItems.Values;
            }
        }

        private GameClient GetClient()
        {
            return AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId);
        }

        public void AddItemArray(List<Item> RoomItemList)
        {
            foreach (Item roomItem in RoomItemList)
                this.AddItem(roomItem);
        }

        public void AddItem(Item item)
        {
            this.AddNewItem(item.Id, item.BaseItem, item.ExtraData, item.LimitedNo, item.LimitedTot);
        }
    }
}