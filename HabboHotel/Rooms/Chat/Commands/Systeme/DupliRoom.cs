using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Wired;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class DupliRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int OldRoomId = Room.Id;
            int RoomId;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                Room.GetRoomItemHandler().SaveFurniture();

                dbClient.SetQuery("INSERT INTO `rooms` (`caption`, `owner`, `description`, `model_name`, `icon_bg`, `icon_fg`, `icon_items`, `wallpaper`, `floor`, `landscape`, `allow_hidewall`, `wallthick`, `floorthick`, `allow_rightsoverride`, `allow_hidewireds`)" +
                "SELECT 'Copia de la sala " + OldRoomId + "', '" + Session.GetHabbo().Username + "', `description`, `model_name`, `icon_bg`, `icon_fg`, `icon_items`, `wallpaper`, `floor`, `landscape`, `allow_hidewall`, `wallthick`, `floorthick`, `allow_rightsoverride`, `allow_hidewireds` FROM rooms WHERE id = '" + OldRoomId + "'; ");
                RoomId = (int)dbClient.InsertQuery();

                dbClient.RunQuery("INSERT INTO `room_models_customs` (`room_id`, `door_x`, `door_y`, `door_z`, `door_dir`, `heightmap`, `wall_height`) " +
                    "SELECT '" + RoomId + "', `door_x`, `door_y`, `door_z`, `door_dir`, `heightmap`, `wall_height` FROM room_models_customs WHERE room_id = '" + OldRoomId + "'");

                dbClient.RunQuery("SELECT item_id FROM catalog_items WHERE page_id IN (SELECT id FROM catalog_pages WHERE min_rank <= '" + Session.GetHabbo().Rank + "') AND cost_pixels = '0' AND cost_diamonds = '0' AND limited_sells = '0' AND limited_stack = '0' AND offer_active = '1' GROUP BY item_id");
                List<int> furniIdAllow = new List<int>();

                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int.TryParse(dataRow["item_id"].ToString(), out int itemId);
                    if (!furniIdAllow.Contains(itemId)) furniIdAllow.Add(itemId);
                }

                Dictionary<int, int> newItemsId = new Dictionary<int, int>();
                List<int> wiredId = new List<int>();
                List<int> teleportId = new List<int>();

                dbClient.SetQuery("SELECT id, base_item FROM items WHERE room_id = '" + OldRoomId + "'");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int OldItemId = Convert.ToInt32(dataRow[0]);
                    int baseID = Convert.ToInt32(dataRow[1]);

                    if (!furniIdAllow.Contains(baseID)) continue;

                    ItemData Data = null;
                    AkiledEnvironment.GetGame().GetItemManager().GetItem(baseID, out Data);
                    if (Data == null || Data.IsRare) continue;

                    dbClient.SetQuery("INSERT INTO `items` (`user_id`, `room_id`, `base_item`, `extra_data`, `x`, `y`, `z`, `rot`, `wall_pos`)" +
                        " SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', base_item, extra_data, x, y, z, rot, wall_pos FROM items WHERE id = '" + OldItemId + "'");
                    int ItemId = (int)dbClient.InsertQuery();

                    newItemsId.Add(OldItemId, ItemId);

                    if (Data.InteractionType == InteractionType.TELEPORT || Data.InteractionType == InteractionType.ARROW) teleportId.Add(OldItemId);

                    if (Data.InteractionType == InteractionType.MOODLIGHT)
                    {
                        dbClient.RunQuery("INSERT INTO `room_items_moodlight` (`item_id`, `enabled`, `current_preset`, `preset_one`, `preset_two`, `preset_three`)" +
                        "SELECT '" + ItemId + "', `enabled`, `current_preset`, `preset_one`, `preset_two`, `preset_three` FROM room_items_moodlight WHERE item_id = '" + OldItemId + "'");
                    }

                    if (WiredUtillity.TypeIsWired(Data.InteractionType))
                    {
                        if (Data.InteractionType == InteractionType.superwired)
                        {
                            //trigger_data check
                            dbClient.RunQuery("INSERT INTO `wired_items` (`trigger_id`, `trigger_data_2`, `trigger_data`, `all_user_triggerable`, `triggers_item`) " +
                            "SELECT '" + ItemId + "', trigger_data_2, '', all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + OldItemId + "'");

                        }
                        else
                        {
                            dbClient.RunQuery("INSERT INTO `wired_items` (`trigger_id`, `trigger_data_2`, `trigger_data`, `all_user_triggerable`, `triggers_item`) " +
                            "SELECT '" + ItemId + "', trigger_data_2, trigger_data, all_user_triggerable, triggers_item FROM wired_items WHERE trigger_id = '" + OldItemId + "'");
                        }

                        wiredId.Add(ItemId);
                    }
                }

                foreach (int oldId in teleportId)
                {
                    if (!newItemsId.TryGetValue(oldId, out int newId)) continue;

                    dbClient.SetQuery("SELECT tele_two_id FROM tele_links WHERE tele_one_id = '" + oldId + "'");
                    DataRow rowTele = dbClient.GetRow();
                    if (rowTele == null) continue;

                    if (!newItemsId.TryGetValue(Convert.ToInt32(rowTele["tele_two_id"]), out int newIdTwo)) continue;

                    dbClient.RunQuery("INSERT INTO `tele_links` (`tele_one_id`, `tele_two_id`) VALUES ('" + newId + "', '" + newIdTwo + "');");
                }

                foreach (int id in wiredId)
                {
                    dbClient.SetQuery("SELECT triggers_item FROM wired_items WHERE trigger_id = '" + id + "' AND triggers_item != ''");
                    DataRow wiredRow = dbClient.GetRow();

                    if (wiredRow == null) continue;

                    string triggerItems = "";

                    string OldItem = (string)wiredRow["triggers_item"];

                    if (OldItem.Contains(":"))
                    {
                        foreach (string oldItem in OldItem.Split(';'))
                        {
                            string[] itemData = oldItem.Split(':');

                            if (itemData.Length != 6) continue;

                            int oldId = Convert.ToInt32(itemData[0]);

                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out int newId)) continue;

                            triggerItems += newId + ":" + Convert.ToInt32(itemData[1]) + ":" + Convert.ToInt32(itemData[2]) + ":" + Convert.ToDouble(itemData[3]) + ":" + Convert.ToInt32(itemData[4]) + ":" + itemData[5] + ";";
                        }
                    }
                    else
                    {
                        foreach (string oldId in OldItem.Split(';'))
                        {
                            if (!newItemsId.TryGetValue(Convert.ToInt32(oldId), out int newId)) continue;

                            triggerItems += newId + ";";
                        }
                    }

                    if (triggerItems.Length > 0) triggerItems = triggerItems.Remove(triggerItems.Length - 1);

                    dbClient.SetQuery("UPDATE wired_items SET triggers_item=@triggeritems WHERE trigger_id = '" + id + "' LIMIT 1");
                    dbClient.AddParameter("triggeritems", triggerItems);
                    dbClient.RunQuery();
                }

                dbClient.RunQuery("INSERT INTO `bots` (`user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`) " +
                    "SELECT '" + Session.GetHabbo().Id + "', `name`, `motto`, `gender`, `look`, '" + RoomId + "', `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat` FROM bots WHERE room_id = '" + OldRoomId + "'");

                dbClient.RunQuery("INSERT INTO `user_pets` (`user_id`, `room_id`, `name`, `race`, `color`, `type`, `expirience`, `energy`, `nutrition`, `respect`, `createstamp`, `x`, `y`, `z`, `have_saddle`, `hairdye`, `pethair`, `anyone_ride`) " +
                    "SELECT '" + Session.GetHabbo().Id + "', '" + RoomId + "', `name`, `race`, `color`, `type`, `expirience`, `energy`, `nutrition`, `respect`, '" + AkiledEnvironment.GetUnixTimestamp() + "', `x`, `y`, `z`, `have_saddle`, `hairdye`, `pethair`, `anyone_ride` FROM user_pets WHERE room_id = '" + OldRoomId + "'");
            }

            RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (roomData == null) return;

            Session.GetHabbo().UsersRooms.Add(roomData);
            Session.SendPacket(new FlatCreatedComposer(roomData.Id, "Sala " + OldRoomId + " Backup"));
        }
    }
}