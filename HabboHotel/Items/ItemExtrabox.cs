using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Items
{
    static class ItemExtrabox
    {
        public static void OpenExtrabox(GameClient Session, Item Present, Room Room)
        {
            int PageId;

            if (AkiledEnvironment.GetRandomNumber(1, 750) == 750) //Ultra rare
                PageId = 84641;
            else if (AkiledEnvironment.GetRandomNumber(1, 100) == 100) //Extra rare
                PageId = 98747;
            else if (AkiledEnvironment.GetRandomNumber(1, 75) == 75) //Point
                PageId = 15987;
            else if (AkiledEnvironment.GetRandomNumber(1, 25) == 25) // Win-win
                PageId = 456465;
            else
                PageId = 894948; //Rare


            CatalogPage Page;
            AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page);
            if (Page == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(AkiledEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE items SET base_item = @baseid WHERE id = " + Present.Id);
                queryreactor.AddParameter("baseid", LotData.Id);
                queryreactor.RunQuery();
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.RunQuery();
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
        }

        public static void OpenDeluxeBox(GameClient Session, Item Present, Room Room)
        {
            int PageId;

            if (AkiledEnvironment.GetRandomNumber(1, 200) == 200) //Epique
                PageId = 1635463617;
            else if (AkiledEnvironment.GetRandomNumber(1, 20) == 20) //Commun
                PageId = 1635463616;
            else
                PageId = 91700214; //Basique


            CatalogPage Page;
            AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page);
            if (Page == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(AkiledEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE items SET base_item = @baseid WHERE id = " + Present.Id);
                queryreactor.AddParameter("baseid", LotData.Id);
                queryreactor.RunQuery();
            }

            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.RunQuery();
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
        }

        public static void OpenBadgeBox(GameClient Session, Item Present, Room Room)
        {
            CatalogPage Page = null;
            int PageId = 0;
            string BadgeCode = "";

            //Présentoir et badge
            PageId = 987987;

            List<int> PageBadgeList = new List<int>(new int[] { 8948, 18171, 18172, 18173, 18174, 18175, 18176, 18177, 18178, 18179, 18180, 18181, 18182, 18183 });
            CatalogPage PageBadge = null;
            int PageBadgeId = PageBadgeList[AkiledEnvironment.GetRandomNumber(0, PageBadgeList.Count - 1)];
            AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageBadgeId, out PageBadge);
            if (PageBadge == null)
                return;

            BadgeCode = PageBadge.Items.ElementAt(AkiledEnvironment.GetRandomNumber(0, PageBadge.Items.Count - 1)).Value.Badge;


            AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page);
            if (Page == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = Page.Items.ElementAt(AkiledEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            if (LotData == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            string ExtraData = BadgeCode + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE items SET base_item = @baseid, extra_data = @extradata WHERE id = " + Present.Id);
                queryreactor.AddParameter("baseid", LotData.Id);
                queryreactor.AddParameter("extradata", ExtraData);
                queryreactor.RunQuery();
            }
            Present.ExtraData = ExtraData;

            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.RunQuery();
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);

            if (!string.IsNullOrEmpty(BadgeCode) && !Session.GetHabbo().GetBadgeComponent().HasBadge(BadgeCode))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, 0, true);
                Session.SendPacket(new ReceiveBadgeComposer(BadgeCode));

                RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabbo != null)
                    roomUserByHabbo.SendWhisperChat("Tu as reçu le badge: " + BadgeCode);
            }
        }

        public static void OpenLegendBox(GameClient Session, Item Present, Room Room)
        {
            CatalogPage Page = null;
            int PageId = 0;
            string BadgeCode = "";
            string LotType = "";
            int ForceItem = 0;

            if (AkiledEnvironment.GetRandomNumber(1, 100) == 100) //Legendaire
            {
                PageId = 14514;
                LotType = "Légendaire";
            }
            else if (AkiledEnvironment.GetRandomNumber(1, 75) == 75) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 37951979;
            }
            else if (AkiledEnvironment.GetRandomNumber(1, 30) == 30) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 70223722;
            }
            else if (AkiledEnvironment.GetRandomNumber(1, 15) == 15) //Epique
            {
                PageId = 84641;
                LotType = "épique";
            }
            else if (AkiledEnvironment.GetRandomNumber(1, 5) == 5) //Royal
            {
                PageId = 584545;
                LotType = "Royal";
                ForceItem = 52394359;
            }
            else
            {
                PageId = 98747;
                LotType = "commun";
            }


            int PageBadgeId = 841878;
            CatalogPage PageBadge = null;

            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageBadgeId, out PageBadge)) return;

            foreach (KeyValuePair<int, CatalogItem> Item in PageBadge.Items.OrderBy(a => Guid.NewGuid()).ToList())
            {
                if (Session.GetHabbo().GetBadgeComponent().HasBadge(Item.Value.Badge))
                    continue;

                BadgeCode = Item.Value.Badge;
                break;

            }

            AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out Page);
            if (Page == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            ItemData LotData = null;

            if (ForceItem == 0)
                LotData = Page.Items.ElementAt(AkiledEnvironment.GetRandomNumber(0, Page.Items.Count - 1)).Value.Data;
            else
                LotData = Page.GetItem(ForceItem).Data;

            if (LotData == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.error", Session.Langue));
                return;
            }

            int Credits = AkiledEnvironment.GetRandomNumber(100, 10000) * 1000;
            Session.GetHabbo().Credits += Credits;
            Session.GetHabbo().UpdateCreditsBalance();

            int WinWin = AkiledEnvironment.GetRandomNumber(100, 1000);
            Session.GetHabbo().AchievementPoints += WinWin;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE user_stats SET AchievementScore = AchievementScore + '" + WinWin + "' WHERE id = '" + Session.GetHabbo().Id + "'");
            }

            Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));
            RoomUser roomUserByHabbo = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo != null)
            {
                Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                Room.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }

            if (!string.IsNullOrEmpty(BadgeCode))
            {
                Session.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, 0, true);
                Session.SendPacket(new ReceiveBadgeComposer(BadgeCode));
            }

            if (roomUserByHabbo != null)
                roomUserByHabbo.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("item.legendboxlot", Session.Langue), Credits, WinWin, BadgeCode, LotType));

            Room.GetRoomItemHandler().RemoveFurniture(Session, Present.Id);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE items SET base_item = @baseid WHERE id = " + Present.Id);
                queryreactor.AddParameter("baseid", LotData.Id);
                queryreactor.RunQuery();
            }
            string FurniType = Present.GetBaseItem().Type.ToString().ToLower();
            Present.BaseItem = LotData.Id;
            Present.ResetBaseItem();

            bool ItemIsInRoom = true;

            if (Present.Data.Type == 's')
            {
                if (!Room.GetRoomItemHandler().SetFloorItem(Session, Present, Present.GetX, Present.GetY, Present.Rotation, true, false, true))
                {
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                        dbClient.AddParameter("itemId", Present.Id);
                        dbClient.RunQuery();
                    }

                    ItemIsInRoom = false;
                }
            }
            else
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = @itemId LIMIT 1");
                    dbClient.AddParameter("itemId", Present.Id);
                    dbClient.RunQuery();
                }

                ItemIsInRoom = false;
            }

            Session.SendPacket(new OpenGiftComposer(Present.Data, Present.ExtraData, Present, ItemIsInRoom));

            if (!ItemIsInRoom)
                Session.GetHabbo().GetInventoryComponent().TryAddItem(Present);
        }
    }
}
