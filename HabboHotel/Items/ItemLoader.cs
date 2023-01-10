using Akiled;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Data;

namespace Aki.HabboHotel.Items
{
    public static class ItemLoader
    {
        public static List<Item> GetItemsForRoom(int RoomId, Room Room)
        {
            DataTable Items = null;
            List<Item> I = new List<Item>();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `items`.*, COALESCE(`items_groups`.`group_id`, 0) AS `group_id` FROM `items` LEFT OUTER JOIN `items_groups` ON `items`.`id` = `items_groups`.`id` WHERE `items`.`room_id` = @rid;");
                dbClient.AddParameter("rid", RoomId);
                Items = dbClient.GetTable();

                if (Items != null)
                {
                    foreach (DataRow Row in Items.Rows)
                    {
                        ItemData Data = null;

                        if (AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["base_item"]), out Data))
                        {
                            I.Add(new Item(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["base_item"]), Convert.ToString(Row["extra_data"]),
                                Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), Convert.ToInt32(Row["z"]), Convert.ToInt32(Row["rot"]),
                                Convert.ToInt32(Row["limited_number"]), Convert.ToInt32(Row["limited_stack"]), Convert.ToString(Row["wall_pos"]), Room));
                        }
                        else
                        {
                            // Item data does not exist anymore.
                        }
                    }
                }
            }
            return I;
        }

        public static List<Item> GetItemsForUser(int UserId)
        {
            DataTable Items = null;
            List<Item> I = new List<Item>();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT items.id, items.user_id, items.base_item, items.extra_data, items.room_id,items.x, items.z, items.y, items.rot, items.wall_pos, items_limited.limited_number, items_limited.limited_stack FROM items LEFT JOIN items_limited ON(items_limited.item_id = items.id) WHERE items.user_id = @uid AND items.room_id = '0'");
                dbClient.AddParameter("uid", UserId);
                Items = dbClient.GetTable();

                if (Items != null)
                {
                    foreach (DataRow Row in Items.Rows)
                    {
                        ItemData Data = null;

                        if (AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(Row["base_item"]), out Data))
                        {
                            I.Add(new Item(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["room_id"]), Convert.ToInt32(Row["base_item"]), Convert.ToString(Row["extra_data"]),
                                0, 0,
                                Convert.ToInt32(Row["x"]), Convert.ToInt32(Row["y"]), Convert.ToDouble(Row["z"]), Convert.ToInt32(Row["rot"]), Convert.ToString(Row["wall_pos"])));
                        }
                        else
                        {
                            // Item data does not exist anymore.
                        }
                    }
                }
            }
            return I;
        }
    }
}
