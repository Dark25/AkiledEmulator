
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Users.Inventory.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal sealed class PurchaseFromCatalogEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int pageId = Packet.PopInt();
            int key = Packet.PopInt();
            string str1 = Packet.PopString();
            int num1 = Packet.PopInt();
            CatalogPage page = (CatalogPage)null;
            
            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetPage(pageId, out page) || (!page.Enabled || page.MinimumRank > Session.GetHabbo().Rank))
                return;
            CatalogItem catalogItem = (CatalogItem)null;
         if (!page.Items.TryGetValue(key, out var item))
        {
            if (page.ItemOffers.ContainsKey(key))
            {
                    item = page.ItemOffers[key];
                if (item == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
            

            if (num1 < 1 || num1 > 100 || !ItemUtility.CanSelectAmount(item))
                num1 = 1;
            int Amount = item.Amount > 1 ? item.Amount : num1;
            int num2 = num1 > 1 ? item.CostCredits * num1 : item.CostCredits;
            int num3 = num1 > 1 ? item.CostDuckets * num1 : item.CostDuckets;
            int num4 = num1 > 1 ? item.CostDiamonds * num1 : item.CostDiamonds;


          
            if (Session.GetHabbo().Credits < num2 || Session.GetHabbo().Duckets < num3 || Session.GetHabbo().AkiledPoints < num4)
                return;
            if (Amount > 1)
                Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));

            bool flag1 = false;
            int LimitedNumber = 0;
            int LimitedStack = 0;

            if (!string.IsNullOrEmpty(item.Badge))
            {
                Console.WriteLine("tried");
                Session.GetHabbo().GetBadgeComponent().GiveBadge(item.Badge, 0, true, Session);
                Session.SendMessage(new RoomCustomizedAlertComposer("¡ Recibiste una nueva placa, revisa tu inventario !"));
            }

            switch (item.Data.InteractionType)
            {
                case InteractionType.NONE:
                    str1 = "";
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.POSTIT:
                    str1 = "FFFF33";
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.MOODLIGHT:
                    str1 = "1,1,1,#000000,255";
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.TROPHY:
                    string[] strArray1 = new string[9];
                    strArray1[0] = Session.GetHabbo().Username;
                    strArray1[1] = Convert.ToChar(9).ToString();
                    strArray1[2] = DateTime.Now.Day.ToString();
                    strArray1[3] = "-";
                    DateTime now = DateTime.Now;
                    int num5 = now.Month;
                    strArray1[4] = num5.ToString();
                    strArray1[5] = "-";
                    now = DateTime.Now;
                    num5 = now.Year;
                    strArray1[6] = num5.ToString();
                    strArray1[7] = Convert.ToChar(9).ToString();
                    strArray1[8] = str1;
                    str1 = string.Concat(strArray1);
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.pet:
                    try
                    {
                        string[] strArray2 = str1.Split('\n');
                        string PetName = strArray2[0];
                        string s = strArray2[1];
                        string str2 = strArray2[2];
                        int.Parse(s);
                        if (!PetUtility.CheckPetName(PetName) || s.Length > 2 || str2.Length != 6)
                            break;
                        AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetLover", 1);
                        goto case InteractionType.PREFIX_NAME;
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException(ex.ToString());
                        break;
                    }
                case InteractionType.WALLPAPER:
                case InteractionType.FLOOR:
                case InteractionType.LANDSCAPE:
                    double num6 = 0.0;
                    try
                    {
                        num6 = !string.IsNullOrEmpty(str1) ? double.Parse(str1) : 0.0;
                    }
                    catch (Exception ex)
                    {
                        Logging.LogException(ex.ToString());
                    }
                    str1 = num6.ToString().Replace(',', '.');
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.MANNEQUIN:
                    str1 = "m" + Convert.ToChar(5).ToString() + "ch-210-1321.lg-285-92" + Convert.ToChar(5).ToString() + "Default Mannequin";
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.BADGE_DISPLAY:
                    if (((IEnumerable<string>)new string[15]
                    {
            "ADM",
            "GPHWIB",
            "wibbo.helpeur",
            "WIBARC",
            "CRPOFFI",
            "ZEERSWS",
            "PRWRD1",
            "WBI1",
            "WBI2",
            "WBI3",
            "WBI4",
            "WBI5",
            "WBI6",
            "WBI7",
            "CASINOB"
                    }).Contains<string>(str1))
                    {
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket((IServerPacket)new PurchaseOKComposer());
                        break;
                    }
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(str1))
                    {
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket((IServerPacket)new PurchaseOKComposer());
                        break;
                    }
                    string[] strArray3 = new string[9];
                    strArray3[0] = str1;
                    strArray3[1] = Convert.ToChar(9).ToString();
                    strArray3[2] = Session.GetHabbo().Username;
                    strArray3[3] = Convert.ToChar(9).ToString();
                    int num7 = DateTime.Now.Day;
                    strArray3[4] = num7.ToString();
                    strArray3[5] = "-";
                    num7 = DateTime.Now.Month;
                    strArray3[6] = num7.ToString();
                    strArray3[7] = "-";
                    num7 = DateTime.Now.Year;
                    strArray3[8] = num7.ToString();
                    str1 = string.Concat(strArray3);
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.BADGE_TROC:
                    if (((IEnumerable<string>)new string[15]
                    {
            "ADM",
            "GPHWIB",
            "wibbo.helpeur",
            "WIBARC",
            "CRPOFFI",
            "ZEERSWS",
            "PRWRD1",
            "WBI1",
            "WBI2",
            "WBI3",
            "WBI4",
            "WBI5",
            "WBI6",
            "WBI7",
            "CASINOB"
                    }).Contains<string>(str1))
                    {
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket((IServerPacket)new PurchaseOKComposer());
                        break;
                    }
                    if (!Session.GetHabbo().GetBadgeComponent().HasBadge(str1))
                    {
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buybadgedisplay.error", Session.Langue));
                        Session.SendPacket((IServerPacket)new PurchaseOKComposer());
                        break;
                    }
                    Session.GetHabbo().GetBadgeComponent().RemoveBadge(str1);
                    Session.SendPacket((IServerPacket)Session.GetHabbo().GetBadgeComponent().Serialize());
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.GUILD_ITEM:
                case InteractionType.GUILD_GATE:
                case InteractionType.GUILD_FORUM:
                    int result = 0;
                    if (!int.TryParse(str1, out result) || result == 0)
                        break;
                    Group Group = (Group)null;
                    if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(result, out Group))
                        break;
                    str1 = "0;" + Group.Id.ToString();
                    goto case InteractionType.PREFIX_NAME;
                case InteractionType.PREFIX_NAME:
                    if (item.Data.InteractionType == InteractionType.PREFIX_NAME && (str1.Length < 2 || str1.Length > 8 || !AkiledEnvironment.IsValidAlphaNumeric(str1)))
                        flag1 = true;
                    if (item.IsLimited)
                    {
                        if (item.LimitedEditionStack <= item.LimitedEditionSells)
                        {
                            Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.buyltd.error", Session.Langue));
                            Session.SendPacket((IServerPacket)new PurchaseOKComposer());
                            break;
                        }
                        Interlocked.Increment(ref item.LimitedEditionSells);
                        using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            queryReactor.SetQuery("UPDATE `catalog_items` SET `limited_sells` = @limitSells WHERE `id` = @itemId LIMIT 1");
                            queryReactor.AddParameter("limitSells", item.LimitedEditionSells);
                            queryReactor.AddParameter("itemId", item.Id);
                            queryReactor.RunQuery();
                            LimitedNumber = item.LimitedEditionSells;
                            LimitedStack = item.LimitedEditionStack;
                        }
                    }
                    if (!flag1)
                    {
                        if (item.CostCredits > 0)
                        {
                            Session.GetHabbo().Credits -= num2;
                            Session.SendPacket((IServerPacket)new CreditBalanceComposer(Session.GetHabbo().Credits));
                        }
                        if (item.CostDuckets > 0)
                        {
                            Session.GetHabbo().Duckets -= num3;
                            Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, Session.GetHabbo().Duckets));
                        }
                        if (item.CostDiamonds > 0)
                        {
                            Session.GetHabbo().AkiledPoints -= num4;
                            Session.SendPacket((IServerPacket)new HabboActivityPointNotificationComposer(Session.GetHabbo().AkiledPoints, 0, 105));
                            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryReactor.RunQuery("UPDATE users SET vip_points = vip_points - " + num4.ToString() + " WHERE id = " + Session.GetHabbo().Id.ToString());
                        }
                    }
                    bool flag2 = false;
                    string lower = item.Data.Type.ToString().ToLower();
                    if (!(lower == "r"))
                    {
                        if (!(lower == "p"))
                        {
                            if (!(lower == "b"))
                            {
                                List<Item> objList = new List<Item>();
                                switch (item.Data.InteractionType)
                                {
                                    case InteractionType.MOODLIGHT:
                                        if (Amount > 1)
                                        {
                                            List<Item> multipleItems = ItemFactory.CreateMultipleItems(item.Data, Session.GetHabbo(), str1, Amount);
                                            if (multipleItems != null)
                                            {
                                                objList.AddRange((IEnumerable<Item>)multipleItems);
                                                using (List<Item>.Enumerator enumerator = multipleItems.GetEnumerator())
                                                {
                                                    while (enumerator.MoveNext())
                                                        ItemFactory.CreateMoodlightData(enumerator.Current);
                                                    goto case InteractionType.PREFIX_COLOR;
                                                }
                                            }
                                            else
                                                goto case InteractionType.PREFIX_COLOR;
                                        }
                                        else
                                        {
                                            Item singleItemNullable = ItemFactory.CreateSingleItemNullable(item.Data, Session.GetHabbo(), str1);
                                            if (singleItemNullable != null)
                                            {
                                                objList.Add(singleItemNullable);
                                                ItemFactory.CreateMoodlightData(singleItemNullable);
                                            }
                                            goto case InteractionType.PREFIX_COLOR;
                                        }
                                    case InteractionType.TELEPORT:
                                    case InteractionType.ARROW:
                                        for (int index = 0; index < Amount; ++index)
                                        {
                                            List<Item> teleporterItems = ItemFactory.CreateTeleporterItems(item.Data, Session.GetHabbo());
                                            if (teleporterItems != null)
                                                objList.AddRange((IEnumerable<Item>)teleporterItems);
                                        }
                                        goto case InteractionType.PREFIX_COLOR;
                                    case InteractionType.PREFIX_NAME:
                                        if (str1.Length < 1)
                                        {
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendMessage((IServerPacket)new RoomCustomizedAlertComposer("¡Su prefijo es muy corto minimo 1 caracteres!"));
                                            return;
                                        }
                                        if (str1.Length > 12)
                                        {
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendMessage((IServerPacket)new RoomCustomizedAlertComposer("¡Su prefijo es muy largo solo puede tener 12 caracteres!"));
                                            return;
                                        }
                                        if (!AkiledEnvironment.IsValidAlphaNumeric(str1))
                                        {
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendMessage((IServerPacket)new RoomCustomizedAlertComposer("¡Caracteres invalidos!"));
                                            return;
                                        }
                                        if (Session.Antipub(str1, "<PREFIJO>") && !Session.GetHabbo().HasFuse("word_filter_override"))
                                        {
                                            AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("publicidad", "El usuario: " + Session.GetHabbo().Username + ", palabra:" + str1 + ", comprando un prefijo en el catálogo."));
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendMessage((IServerPacket)new RoomCustomizedAlertComposer("¡No puedes usar este prefijo en el hotel!"));
                                            return;
                                        }
                                        if (str1.Contains("MOD") || str1.Contains("mod") || (str1.Contains("HEAD") || str1.Contains("head")) || (str1.Contains("ADMIN") || str1.Contains("admin") || (str1.Contains("SEX") || str1.Contains("ADM"))) || (str1.Contains("DEV") || str1.Contains("XLER") || (str1.Contains("PREMIUM") || str1.Contains("premium")) || (str1.Contains("Premium") || str1.Contains(">") || (str1.Contains("<") || str1.Contains("&")))) || (str1.Contains("=") || str1.Contains("GUIDE") || (str1.Contains("M0D") || str1.Contains("OWNER")) || (str1.Contains("staff") || str1.Contains("owner") || (str1.Contains("STAFF") || str1.ToUpper().Contains("ADM"))) || (str1.ToUpper().Contains("WWW.HEVVO.NET") || str1.ToUpper().Contains("H3VV0") || (str1.ToUpper().Contains("ADMIN") || str1.ToUpper().Contains("DUENO")) || (str1.ToUpper().Contains("DUEñO") || str1.ToUpper().Contains("RANK") || (str1.ToUpper().Contains("MNG") || str1.ToUpper().Contains("MOD"))))) || (str1.ToUpper().Contains("STAFF") || str1.ToUpper().Contains("ALFA") || (str1.ToUpper().Contains("ALPHA") || str1.ToUpper().Contains("HELPER")) || (str1.ToUpper().Contains("GM") || str1.ToUpper().Contains("OWNER") || (str1.ToUpper().Contains("CEO") || str1.ToUpper().Contains("VIP"))) || (str1.ToUpper().Contains("M0D") || str1.ToUpper().Contains("DEV") || (str1.ToUpper().Contains("OWNR") || str1.ToUpper().Contains("HEVVO.NET")) || (str1.ToUpper().Contains("FUNDADOR") || str1.ToUpper().Contains("PLUS") || (str1.ToUpper().Contains("HEBBA") || str1.ToUpper().Contains("HOBBA")))) || (str1.ToUpper().Contains("HEVVO") || str1.ToUpper().Contains("H3VVO"))) || str1.ToUpper().Contains("HADDOZ"))
                                        {
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendMessage((IServerPacket)new RoomCustomizedAlertComposer("¡No puedes usar este prefijo en el hotel!"));
                                            return;
                                        }
                                        string str2 = Session.GetHabbo().Prefix.Split(';')[1];
                                        if (string.IsNullOrEmpty(str2))
                                            str2 = "000000";
                                        if (str1 == "off" || str1 == "")
                                        {
                                            Session.GetHabbo().Prefix = ";" + str2;
                                            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                            {
                                                queryReactor.SetQuery("UPDATE `users` SET `prefix` = @prefix WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                                queryReactor.AddParameter("prefix", (string)Session.GetHabbo().Prefix);
                                                queryReactor.RunQuery();
                                            }
                                            Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                            Session.SendWhisper("Usted acaba de desactivar los prefijos.");
                                            return;
                                        }
                                        Session.GetHabbo().Prefix = str1 + ";" + str2;
                                        using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                        {
                                            queryReactor.SetQuery("UPDATE `users` SET `prefix` = @prefix WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                            queryReactor.AddParameter("prefix", (string)Session.GetHabbo().Prefix);
                                            queryReactor.RunQuery();
                                        }
                                        Session.SendPacket((IServerPacket)new RoomCustomizedAlertComposer("¡Ha comprado un prefijo!"));
                                        Session.SendMessage((IServerPacket)new ScrSendUserInfoComposer());
                                        Session.SendMessage((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                        Session.SendMessage((IServerPacket)new FurniListUpdateComposer());
                                        flag2 = true;
                                        goto case InteractionType.PREFIX_COLOR;
                                    case InteractionType.PREFIX_COLOR:
                                        if (!flag2)
                                        {
                                            using (List<Item>.Enumerator enumerator = objList.GetEnumerator())
                                            {
                                                while (enumerator.MoveNext())
                                                {
                                                    Item current = enumerator.Current;
                                                    if (Session.GetHabbo().GetInventoryComponent().TryAddItem(current))
                                                        Session.SendPacket((IServerPacket)new FurniListNotificationComposer(current.Id, 1));
                                                }
                                                break;
                                            }
                                        }
                                        else
                                            break;
                                    case InteractionType.PINATA:
                                    case InteractionType.PLANT_SEED:
                                    case InteractionType.PINATATRIGGERED:
                                    case InteractionType.CRACKABLE_EGG:
                                        str1 = "0";
                                        break;

                                    default:
                                        if (Amount > 1)
                                        {
                                            List<Item> multipleItems = ItemFactory.CreateMultipleItems(item.Data, Session.GetHabbo(), str1, Amount);
                                            if (multipleItems != null)
                                            {
                                                objList.AddRange((IEnumerable<Item>)multipleItems);
                                                goto case InteractionType.PREFIX_COLOR;
                                            }
                                            else
                                                goto case InteractionType.PREFIX_COLOR;
                                        }
                                        else
                                        {
                                            Item singleItemNullable = ItemFactory.CreateSingleItemNullable(item.Data, Session.GetHabbo(), str1, LimitedNumber, LimitedStack);
                                            if (singleItemNullable != null)
                                                objList.Add(singleItemNullable);
                                            goto case InteractionType.PREFIX_COLOR;
                                        }
                                }
                            }
                            else
                            {
                                if (item.Data.InteractionType == InteractionType.PREFIX_COLOR)
                                {
                                    string str2 = Session.GetHabbo().Prefix.Split(';')[0];
                                    Session.GetHabbo().Prefix = string.IsNullOrEmpty(str2) ? ";" + item.Data.ItemName : str2 + ";" + item.Data.ItemName;
                                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        queryReactor.SetQuery("UPDATE `users` SET `prefix` = @prefix WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                        queryReactor.AddParameter("prefix", Session.GetHabbo().Prefix);
                                        queryReactor.RunQuery();
                                    }
                                    Session.SendPacket((IServerPacket)new RoomCustomizedAlertComposer("¡Ha comprado un color para su prefijo!"));
                                    Session.SendPacket((IServerPacket)new ScrSendUserInfoComposer());
                                    Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                    Session.SendPacket((IServerPacket)new FurniListUpdateComposer());
                                    flag2 = true;
                                }
                                if (item.Data.InteractionType == InteractionType.PREFIX_COLORNAME)
                                {
                                    string str2 = Session.GetHabbo().Prefixnamecolor.Split(';')[1];
                                    Session.GetHabbo().Prefixnamecolor = string.IsNullOrEmpty(str2) ? item.Data.ItemName + ";" : item.Data.ItemName + ";" + str2;
                                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        queryReactor.SetQuery("UPDATE `users` SET `prefixnamecolor` = @prefixnamecolor WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                        queryReactor.AddParameter("prefixnamecolor", Session.GetHabbo().Prefixnamecolor);
                                        queryReactor.RunQuery();
                                    }
                                    Session.SendPacket((IServerPacket)new RoomCustomizedAlertComposer("¡Ha comprado un color para su nombre!"));
                                    Session.SendPacket((IServerPacket)new ScrSendUserInfoComposer());
                                    Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                    Session.SendPacket((IServerPacket)new FurniListUpdateComposer());
                                    flag2 = true;
                                }

                                if (item.Data.InteractionType == InteractionType.PREFIX_SIZENAME)
                                {
                                    string str2 = Session.GetHabbo().PrefixSize.Split(';')[0];
                                    Session.GetHabbo().PrefixSize = string.IsNullOrEmpty(str2) ? ";" + item.Data.ItemName : str2 + ";" + item.Data.ItemName;
                                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        queryReactor.SetQuery("UPDATE `users` SET `prefixsize` = @prefix WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                        queryReactor.AddParameter("prefix", Session.GetHabbo().PrefixSize);
                                        queryReactor.RunQuery();
                                    }
                                    Session.SendPacket((IServerPacket)new RoomCustomizedAlertComposer("¡Ha comprado un tamaño de su nombre de usuario!"));
                                    Session.SendPacket((IServerPacket)new ScrSendUserInfoComposer());
                                    Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                    Session.SendPacket((IServerPacket)new FurniListUpdateComposer());
                                    flag2 = true;
                                }
                                if (item.Data.InteractionType == InteractionType.PREFIX_SIZETAG)
                                {
                                    string str2 = Session.GetHabbo().PrefixSize.Split(';')[1];
                                    Session.GetHabbo().PrefixSize = string.IsNullOrEmpty(str2) ? item.Data.ItemName + ";" : item.Data.ItemName + ";" + str2;
                                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                    {
                                        queryReactor.SetQuery("UPDATE `users` SET `prefixsize` = @prefix WHERE `id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                                        queryReactor.AddParameter("prefix", Session.GetHabbo().PrefixSize);
                                        queryReactor.RunQuery();
                                    }
                                    Session.SendPacket((IServerPacket)new RoomCustomizedAlertComposer("¡Ha comprado un tamaño para su prefijo!"));
                                    Session.SendPacket((IServerPacket)new ScrSendUserInfoComposer());
                                    Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));
                                    Session.SendPacket((IServerPacket)new FurniListUpdateComposer());
                                    flag2 = true;
                                }
                            }
                        }
                        else
                        {
                            string[] strArray2 = str1.Split('\n');
                            Pet pet = PetUtility.CreatePet(Session.GetHabbo().Id, strArray2[0], item.Data.SpriteId, strArray2[1], strArray2[2]);
                            if (pet != null)
                            {
                                Session.GetHabbo().GetInventoryComponent().TryAddPet(pet);
                                Session.SendPacket((IServerPacket)new FurniListNotificationComposer(pet.PetId, 3));
                                Session.SendPacket((IServerPacket)new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
                            }
                        }
                    }
                    else
                    {
                        Bot bot = BotUtility.CreateBot(item.Data, Session.GetHabbo().Id);
                        if (bot != null)
                        {
                            Session.GetHabbo().GetInventoryComponent().TryAddBot(bot);
                            Session.SendPacket((IServerPacket)new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
                            Session.SendPacket((IServerPacket)new FurniListNotificationComposer(bot.Id, 5));
                        }
                    }
                    if (flag2)
                        break;
                    break;
                default:
                    str1 = "";
                    goto case InteractionType.PREFIX_NAME;
            }

            Session.SendPacket((IServerPacket)new PurchaseOKComposer(item, item.Data));
        }
    }
}
