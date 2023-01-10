using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Users;
using Akiled.Utilities;
using System;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PurchaseFromCatalogAsGiftEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int PageId = Packet.PopInt();
            int ItemId = Packet.PopInt();
            string Data = Packet.PopString();
            string GiftUser = StringCharFilter.Escape(Packet.PopString());
            string GiftMessage = StringCharFilter.Escape(Packet.PopString().Replace(Convert.ToChar(5), ' '));
            int SpriteId = Packet.PopInt();
            int Ribbon = Packet.PopInt();
            int Colour = Packet.PopInt();
            bool dnow = Packet.PopBoolean();

            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetPage(PageId, out CatalogPage Page)) return;

            if (!Page.Enabled || Page.MinimumRank > Session.GetHabbo().Rank) return;

            if (!Page.Items.TryGetValue(ItemId, out CatalogItem Item)) return;

            if (!ItemUtility.CanGiftItem(Item)) return;

            if (!AkiledEnvironment.GetGame().GetItemManager().GetGift(SpriteId, out ItemData PresentData) || PresentData.InteractionType != InteractionType.GIFT) return;

            int TotalCreditsCost = Item.CostCredits;
            int TotalPixelCost = Item.CostDuckets;
            int TotalDiamondCost = Item.CostDiamonds;

            if (Session.GetHabbo().Credits < TotalCreditsCost || Session.GetHabbo().Duckets < TotalPixelCost || Session.GetHabbo().AkiledPoints < TotalDiamondCost)
                return;

            Habbo Habbo = AkiledEnvironment.GetHabboByUsername(GiftUser);
            if (Habbo == null)
            {
                //Session.SendPacket(new GiftWrappingErrorComposer());
                return;
            }

            if ((DateTime.Now - Session.GetHabbo().LastGiftPurchaseTime).TotalSeconds <= 15.0)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buygift.flood", Session.Langue));

                Session.GetHabbo().GiftPurchasingWarnings += 1;
                if (Session.GetHabbo().GiftPurchasingWarnings >= 25)
                    Session.GetHabbo().SessionGiftBlocked = true;

                return;
            }

            if (Session.GetHabbo().SessionGiftBlocked) return;

            string ED = Session.GetHabbo().Id + ";" + GiftMessage + Convert.ToChar(5) + Ribbon + Convert.ToChar(5) + Colour;

            int NewItemId = 0;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                //Insert the dummy item.
                dbClient.SetQuery("INSERT INTO `items` (`base_item`,`user_id`,`extra_data`) VALUES (@baseId, @habboId, @extra_data)");
                dbClient.AddParameter("baseId", PresentData.Id);
                dbClient.AddParameter("habboId", Habbo.Id);
                dbClient.AddParameter("extra_data", ED);
                NewItemId = Convert.ToInt32(dbClient.InsertQuery());

                string ItemExtraData = null;
                switch (Item.Data.InteractionType)
                {
                    case InteractionType.NONE:
                        ItemExtraData = "";
                        break;

                    case InteractionType.GUILD_ITEM:
                    case InteractionType.GUILD_GATE:
                        int Groupid = 0;
                        if (!int.TryParse(Data, out Groupid))
                            return;
                        if (Groupid == 0)
                            return;
                        Group groupItem;
                        if (AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(Groupid, out groupItem))
                            ItemExtraData = "0;" + groupItem.Id;
                        break;

                    #region Pet handling

                    case InteractionType.pet:

                        try
                        {
                            string[] Bits = Data.Split('\n');
                            string PetName = Bits[0];
                            string Race = Bits[1];
                            string Color = Bits[2];

                            int.Parse(Race); // to trigger any possible errors

                            if (PetUtility.CheckPetName(PetName))
                                return;

                            if (Race.Length > 2)
                                return;

                            if (Color.Length != 6)
                                return;

                            AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        }
                        catch
                        {
                            return;
                        }

                        break;

                    #endregion

                    case InteractionType.FLOOR:
                    case InteractionType.WALLPAPER:
                    case InteractionType.LANDSCAPE:

                        Double Number = 0;
                        try
                        {
                            if (string.IsNullOrEmpty(Data))
                                Number = 0;
                            else
                                Number = Double.Parse(Data);
                        }
                        catch
                        {

                        }

                        ItemExtraData = Number.ToString().Replace(',', '.');
                        break; // maintain extra data // todo: validate

                    case InteractionType.POSTIT:
                        ItemExtraData = "FFFF33";
                        break;

                    case InteractionType.MOODLIGHT:
                        ItemExtraData = "1,1,1,#000000,255";
                        break;

                    case InteractionType.TROPHY:
                        ItemExtraData = Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + Convert.ToChar(9) + Data;
                        break;

                    case InteractionType.MANNEQUIN:
                        ItemExtraData = "m" + Convert.ToChar(5) + ".ch-210-1321.lg-285-92" + Convert.ToChar(5) + "Default Mannequin";
                        break;

                    case InteractionType.BADGE_TROC:
                        {
                            string[] BadgeNotAllowedTroc = { "ADM", "GPHWIB", "wibbo.helpeur", "WIBARC", "CRPOFFI", "ZEERSWS", "PRWRD1", "WBI1", "WBI2", "WBI3", "WBI4", "WBI5", "WBI6", "WBI7", "WBI8", "WBI9", "CASINOB", "WPREMIUM", "VIPFREE" };
                            if (BadgeNotAllowedTroc.Contains(Data) || !AkiledEnvironment.GetGame().GetCatalog().HasBadge(Data))
                            {
                                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                                return;
                            }

                            if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                            {
                                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                                return;
                            }

                            Session.GetHabbo().GetBadgeComponent().RemoveBadge(Data);
                            Session.SendPacket(Session.GetHabbo().GetBadgeComponent().Serialize());

                            ItemExtraData = Data;
                            break;
                        }

                    case InteractionType.BADGE_DISPLAY:
                        string[] BadgeNotAllowed = { "ADM", "GPHWIB", "wibbo.helpeur", "WIBARC", "CRPOFFI", "ZEERSWS", "PRWRD1", "WBI1", "WBI2", "WBI3", "WBI4", "WBI5", "WBI6", "WBI7", "CASINOB" };
                        if (BadgeNotAllowed.Contains(Data))
                        {
                            Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                            Session.SendPacket(new PurchaseOKComposer());
                            return;
                        }

                        if (!Session.GetHabbo().GetBadgeComponent().HasBadge(Data))
                            return;

                        ItemExtraData = Data + Convert.ToChar(9) + Session.GetHabbo().Username + Convert.ToChar(9) + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
                        break;

                    default:
                        ItemExtraData = Data;
                        break;
                }

                //Insert the present, forever.
                dbClient.SetQuery("INSERT INTO `user_presents` (`item_id`,`base_id`,`extra_data`) VALUES (@itemId, @baseId, @extra_data)");
                dbClient.AddParameter("itemId", NewItemId);
                dbClient.AddParameter("baseId", Item.Data.Id);
                dbClient.AddParameter("extra_data", (string.IsNullOrEmpty(ItemExtraData) ? "" : ItemExtraData));
                dbClient.RunQuery();

                //Here we're clearing up a record, this is dumb, but okay.
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @deleteId LIMIT 1");
                dbClient.AddParameter("deleteId", NewItemId);
                dbClient.RunQuery();
            }


            Item GiveItem = ItemFactory.CreateSingleItem(PresentData, Habbo, ED, NewItemId);
            if (GiveItem != null)
            {
                GameClient Receiver = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Habbo.Id);
                if (Receiver != null)
                {
                    Receiver.GetHabbo().GetInventoryComponent().TryAddItem(GiveItem);
                    Receiver.SendPacket(new FurniListNotificationComposer(GiveItem.Id, 1));
                    Receiver.SendPacket(new PurchaseOKComposer());
                    //Receiver.SendPacket(new FurniListUpdateComposer());
                }

                if (Habbo.Id != Session.GetHabbo().Id && !string.IsNullOrWhiteSpace(GiftMessage))
                {
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_GiftGiver", 1);
                    if (Receiver != null)
                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Receiver, "ACH_GiftReceiver", 1);
                }
            }

            Session.SendPacket(new PurchaseOKComposer(Item, PresentData));

            if (Item.CostCredits > 0)
            {
                Session.GetHabbo().Credits -= TotalCreditsCost;
                Session.SendPacket(new CreditBalanceComposer(Session.GetHabbo().Credits));
            }

            if (Item.CostDuckets > 0)
            {
                Session.GetHabbo().Duckets -= TotalPixelCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
            }

            if (Item.CostDiamonds > 0)
            {
                Session.GetHabbo().AkiledPoints -= TotalDiamondCost;
                Session.SendPacket(new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 0, 105));

                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("UPDATE users SET vip_points = vip_points - " + TotalDiamondCost + " WHERE id = " + Session.GetHabbo().Id);
            }

            Session.GetHabbo().LastGiftPurchaseTime = DateTime.Now;
        }
    }
}
