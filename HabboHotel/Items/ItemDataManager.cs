using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;


namespace Akiled.HabboHotel.Items
{
    public class ItemDataManager
    {

        public Dictionary<int, ItemData> _items;
        public Dictionary<int, ItemData> _gifts;

        public ItemDataManager()
        {
            this._items = new Dictionary<int, ItemData>();
            this._gifts = new Dictionary<int, ItemData>();
        }

        public void Init()
        {
            if (this._items.Count > 0) this._items.Clear();
            if (this._gifts.Count > 0) this._gifts.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `furniture`");
                DataTable ItemData = dbClient.GetTable();

                if (ItemData != null)
                {
                    foreach (DataRow Row in ItemData.Rows)
                    {
                        try
                        {
                            int id = Convert.ToInt32(Row["id"]);
                            int spriteID = Convert.ToInt32(Row["sprite_id"]);
                            string itemName = Convert.ToString(Row["item_name"]);
                            string publicname = Convert.ToString(Row["public_name"]);
                            string type = Row["type"].ToString();
                            int width = Convert.ToInt32(Row["width"]);
                            int length = Convert.ToInt32(Row["length"]);
                            double height = Convert.ToDouble(Row["stack_height"]);
                            bool allowStack = AkiledEnvironment.EnumToBool(Row["can_stack"].ToString());
                            bool allowWalk = AkiledEnvironment.EnumToBool(Row["is_walkable"].ToString());
                            bool allowSit = AkiledEnvironment.EnumToBool(Row["can_sit"].ToString());
                            bool allowRecycle = AkiledEnvironment.EnumToBool(Row["allow_recycle"].ToString());
                            bool allowTrade = AkiledEnvironment.EnumToBool(Row["allow_trade"].ToString());
                            bool allowGift = Convert.ToInt32(Row["allow_gift"]) == 1;
                            bool allowInventoryStack = AkiledEnvironment.EnumToBool(Row["allow_inventory_stack"].ToString());
                            InteractionType interactionType = InteractionTypes.GetTypeFromString(Convert.ToString(Row["interaction_type"]));
                            int behaviourData = Convert.ToInt32(Row["behaviour_data"]);
                            int cycleCount = Convert.ToInt32(Row["interaction_modes_count"]);
                            string vendingIDS = Convert.ToString(Row["vending_ids"]);
                            string heightAdjustable = Convert.ToString(Row["height_adjustable"]);
                            int EffectId = Convert.ToInt32(Row["effect_id"]);
                            bool IsRare = AkiledEnvironment.EnumToBool(Row["is_rare"].ToString());

                            if (!this._gifts.ContainsKey(spriteID) && interactionType == InteractionType.GIFT)
                                this._gifts.Add(spriteID, new ItemData(id, spriteID, itemName, publicname, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, interactionType, behaviourData, cycleCount, vendingIDS, heightAdjustable, EffectId, IsRare));

                            if (!this._items.ContainsKey(id))
                                this._items.Add(id, new ItemData(id, spriteID, itemName, publicname, type, width, length, height, allowStack, allowWalk, allowSit, allowRecycle, allowTrade, allowGift, allowInventoryStack, interactionType, behaviourData, cycleCount, vendingIDS, heightAdjustable, EffectId, IsRare));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
                            Console.ReadKey();
                            //Logging.WriteLine("Could not load item #" + Convert.ToInt32(Row[0]) + ", please verify the data is okay.");
                        }
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Furnis del Hotel -> Listo! ");
        }

        public bool GetGift(int SpriteId, out ItemData Item)
        {
            if (this._gifts.TryGetValue(SpriteId, out Item))
                return true;
            return false;
        }

        public bool GetItem(int Id, out ItemData Item)
        {
            if (this._items.TryGetValue(Id, out Item))
                return true;
            return false;
        }

        public ItemData GetItemByName(string name)
        {
            foreach (ItemData item in _items.Values)
            {
                if (item.ItemName == name)
                    return item;
            }
            return null;
        }
    }
}