
using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Items.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class CraftingSecretEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopInt();
            List<Item> source = new List<Item>();
            int num = Packet.PopInt();
            for (int index = 1; index <= num; ++index)
            {
                int Id = Packet.PopInt();
                Item obj = Session.GetHabbo().GetInventoryComponent().GetItem(Id);
                if (obj == null || source.Contains(obj))
                    return;
                source.Add(obj);
            }
            CraftingRecipe recipe = (CraftingRecipe)null;
            foreach (KeyValuePair<string, CraftingRecipe> craftingRecipe in AkiledEnvironment.GetGame().GetCraftingManager().CraftingRecipes)
            {
                bool flag = false;
                foreach (KeyValuePair<string, int> keyValuePair in craftingRecipe.Value.ItemsNeeded)
                {
                    KeyValuePair<string, int> item = keyValuePair;
                    if (item.Value != source.Count<Item>((Func<Item, bool>)(item2 => item2.GetBaseItem().ItemName == item.Key)))
                    {
                        flag = false;
                        break;
                    }
                    flag = true;
                }
                if (flag)
                {
                    recipe = craftingRecipe.Value;
                    break;
                }
            }
            if (recipe == null)
                return;
            ItemData itemByName1 = AkiledEnvironment.GetGame().GetItemManager().GetItemByName(recipe.Result);
            if (itemByName1 == null)
                return;
            bool flag1 = true;
            foreach (KeyValuePair<string, int> keyValuePair in recipe.ItemsNeeded)
            {
                for (int index = 1; index <= keyValuePair.Value; ++index)
                {
                    ItemData itemByName2 = AkiledEnvironment.GetGame().GetItemManager().GetItemByName(keyValuePair.Key);
                    if (itemByName2 == null)
                    {
                        flag1 = false;
                    }
                    else
                    {
                        Item firstItemByBaseId = Session.GetHabbo().GetInventoryComponent().GetFirstItemByBaseId(itemByName2.Id);
                        if (firstItemByBaseId == null)
                        {
                            flag1 = false;
                        }
                        else
                        {
                            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                                queryReactor.RunQuery("DELETE FROM `items` WHERE `id` = '" + firstItemByBaseId.Id.ToString() + "' AND `user_id` = '" + Session.GetHabbo().Id.ToString() + "' LIMIT 1");
                            Session.GetHabbo().GetInventoryComponent().RemoveItem(firstItemByBaseId.Id);
                        }
                    }
                }
            }
            Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            if (flag1)
            {
                Item singleItem = ItemFactory.CreateSingleItem(itemByName1, Session.GetHabbo(), "", 0);
                Session.GetHabbo().GetInventoryComponent().TryAddItem(singleItem);
                Session.SendMessage((IServerPacket)new FurniListUpdateComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
                Session.SendMessage((IServerPacket)new CraftableProductsComposer());
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("CraftingSecretEvent.1", Session.Langue));
                
                if (itemByName1.Id == 2683)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSofa", 1);
                if (itemByName1.Id == 3150)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFloorBones", 1);
                if (itemByName1.Id == 3146)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLantern", 1);
                if (itemByName1.Id == 4608)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBedKleur", 1);
                if (itemByName1.Id == 4615)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenStier", 1);
                if (itemByName1.Id == 4772)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenShinyCarpet", 1);
                if (itemByName1.Id == 4620)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBox", 1);
                if (itemByName1.Id == 4622)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenCloset", 1);
                if (itemByName1.Id == 9034)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPiano", 1);
                if (itemByName1.Id == 9031)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChandelier", 1);
                if (itemByName1.Id == 9028)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenWall", 1);
                if (itemByName1.Id == 9026)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenVase", 1);
                if (itemByName1.Id == 9024)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenVanity", 1);
                if (itemByName1.Id == 9021)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenRoundTable", 1);
                if (itemByName1.Id == 9019)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLamp", 1);
                if (itemByName1.Id == 9017)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenLadder", 1);
                if (itemByName1.Id == 9015)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGlasstable", 1);
                if (itemByName1.Id == 9013)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostVial", 1);
                if (itemByName1.Id == 9012)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostOrb", 1);
                if (itemByName1.Id == 9010)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGhostAsh", 1);
                if (itemByName1.Id == 9009)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFloor", 1);
                if (itemByName1.Id == 9007)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenFireplace", 1);
                if (itemByName1.Id == 9005)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEndtable", 1);
                if (itemByName1.Id == 9002)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChair", 1);
                if (itemByName1.Id == 9000)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenCabinet", 1);
                if (itemByName1.Id == 8998)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBust", 1);
                if (itemByName1.Id == 8996)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBkcase", 1);
                if (itemByName1.Id == 8994)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBed", 1);
                if (itemByName1.Id == 8992)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBarchair", 1);
                if (itemByName1.Id == 8988)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBalcony", 1);
                if (itemByName1.Id == 8990)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBar", 1);
                if (itemByName1.Id == 2051)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenZombie", 1);
                if (itemByName1.Id == 9054)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBkcaseBrown", 1);
                if (itemByName1.Id == 9057)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgeendBrown", 1);
                if (itemByName1.Id == 9062)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDvdrBrown", 1);
                if (itemByName1.Id == 9079)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleGate", 1);
                if (itemByName1.Id == 9083)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleMat", 1);
                if (itemByName1.Id == 9087)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePotPink", 1);
                if (itemByName1.Id == 9097)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleStairsBrown", 1);
                if (itemByName1.Id == 9093)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleRoofPink", 1);
                if (itemByName1.Id == 9100)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSwingsofaPink", 1);
                if (itemByName1.Id == 9103)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTablePink", 1);
                if (itemByName1.Id == 9109)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreestage", 1);
                if (itemByName1.Id == 9112)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWallBrown", 1);
                if (itemByName1.Id == 9055)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBkcaseGrey", 1);
                if (itemByName1.Id == 9058)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgeendGrey", 1);
                if (itemByName1.Id == 9513)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBridgemidGrey", 1);
                if (itemByName1.Id == 9063)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDvdrGrey", 1);
                if (itemByName1.Id == 9080)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChair", 1);
                if (itemByName1.Id == 9084)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleMatLighter", 1);
                if (itemByName1.Id == 9088)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePotBlue", 1);
                if (itemByName1.Id == 9098)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleStairsGrey", 1);
                if (itemByName1.Id == 9094)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleRoofBlue", 1);
                if (itemByName1.Id == 9101)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSwingsofaBlue", 1);
                if (itemByName1.Id == 9104)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTableBlue", 1);
                if (itemByName1.Id == 9110)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreestageRock", 1);
                if (itemByName1.Id == 9113)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWallGrey", 1);
                if (itemByName1.Id == 3054)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingTalltree", 1);
                if (itemByName1.Id == 3267)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingValentineWater", 1);
                if (itemByName1.Id == 3057)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleComfyTree", 1);
                if (itemByName1.Id == 4136)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleShakesphereTree", 1);
                if (itemByName1.Id == 3055)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWaterfall", 1);
                if (itemByName1.Id == 5695)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDinotree", 1);
                if (itemByName1.Id == 9514)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleBush", 1);
                if (itemByName1.Id == 2490)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJunglePond", 1);
                if (itemByName1.Id == 6693)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleWall", 1);
                if (itemByName1.Id == 3248)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleCup", 1);
                if (itemByName1.Id == 4242)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingEasterGrass", 1);
                if (itemByName1.Id == 5694)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleDinotreeSmall", 1);
                if (itemByName1.Id == 3339)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleShrub", 1);
                if (itemByName1.Id == 3111)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJetsetLand", 1);
                if (itemByName1.Id == 3049)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleSun", 1);
                if (itemByName1.Id == 1363)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSink", 1);
                if (itemByName1.Id == 1367)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBath", 1);
                if (itemByName1.Id == 2426)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingJungleTreasure", 1);
                if (itemByName1.Id == 4742)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenHorseman", 1);
                if (itemByName1.Id == 4766)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPureCrystal", 1);
                if (itemByName1.Id == 4767)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenPureCrystalBig", 1);
                if (itemByName1.Id == 4758)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEvilCrystal", 1);
                if (itemByName1.Id == 4759)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenEvilCrystaBig", 1);
                if (itemByName1.Id == 99009)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingXmasIceDuck", 1);
                if (itemByName1.Id == 187)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDeadDuck", 1);
                if (itemByName1.Id == 186)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSkullCandle", 1);
                if (itemByName1.Id == 4579)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenSkeletPieces", 1);
                if (itemByName1.Id == 1412)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingPenguinGlow", 1);
                if (itemByName1.Id == 3702)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenBlood", 1);
                if (itemByName1.Id == 1608)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingSkull", 1);
                if (itemByName1.Id == 5849)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingGuitarSkull", 1);
                if (itemByName1.Id == 4605)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDoll", 1);
                if (itemByName1.Id == 4604)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenDollAfro", 1);
                if (itemByName1.Id == 4580)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenMariachi", 1);
                if (itemByName1.Id == 3142)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenGuillotine", 1);
                if (itemByName1.Id == 6584)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_CraftingHalloweenChandelierDead", 1);
                if (itemByName1.Id == 6584)
                    AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, " ACH_CraftingJungleBridgemidBrown1", 1);
            }
            Session.SendMessage((IServerPacket)new CraftingResultComposer(recipe, false));
        }
    }
}
