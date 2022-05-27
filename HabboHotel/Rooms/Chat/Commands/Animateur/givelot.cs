using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class givelot : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {

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
            }            if (Params.Length != 2)                return;            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)                return;            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);            if (roomUserByHabbo == null || roomUserByHabbo.GetClient() == null)            {                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));                return;            }            if (roomUserByHabbo.GetUsername() == Session.GetHabbo().Username || roomUserByHabbo.GetClient().GetHabbo().IP == Session.GetHabbo().IP)            {                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.givelot.error", Session.Langue));                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "notallowed", "Esta dando mucho: " + roomUserByHabbo.GetUsername());                return;            }            int NbLot = AkiledEnvironment.GetRandomNumber(1, 3);            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)                NbLot = AkiledEnvironment.GetRandomNumber(3, 5);                        ItemData ItemData = null;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(ExtraBox_Item, out ItemData))
                return;
            
            int NbBadge = AkiledEnvironment.GetRandomNumber(1, 2);            if (roomUserByHabbo.GetClient().GetHabbo().Rank > 1)                NbBadge = AkiledEnvironment.GetRandomNumber(2, 3);

            ItemData ItemDataBadge = null;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(BadgeBox_Item, out ItemDataBadge))
                return;

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, roomUserByHabbo.GetClient().GetHabbo(), "", NbLot);            Items.AddRange(ItemFactory.CreateMultipleItems(ItemDataBadge, roomUserByHabbo.GetClient().GetHabbo(), "", NbBadge));            foreach (Item PurchasedItem in Items)
            {
                if (roomUserByHabbo.GetClient().GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem))
                {
                    roomUserByHabbo.GetClient().SendPacket(new FurniListNotificationComposer(PurchasedItem.Id, 1));
                }
            }            roomUserByHabbo.GetClient().SendNotification(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.givelot.sucess", roomUserByHabbo.GetClient().Langue), NbLot, NbBadge));            UserRoom.SendWhisperChat(roomUserByHabbo.GetUsername() + " Usted ha recibido " + NbLot + " ExtraBox y " + NbBadge + " BadgeBox!");            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())                queryreactor.RunQuery("UPDATE users SET games_win + 1 WHERE id = '" + roomUserByHabbo.GetClient().GetHabbo().Id + "';");        }    }}