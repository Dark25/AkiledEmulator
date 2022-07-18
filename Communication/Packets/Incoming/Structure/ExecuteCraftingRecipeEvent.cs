using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Items.Crafting;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class ExecuteCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Packet.PopInt();
            string name = Packet.PopString();
            CraftingRecipe recipe = AkiledEnvironment.GetGame().GetCraftingManager().GetRecipe(name);
            if (recipe == null)
                return;
            ItemData itemByName1 = AkiledEnvironment.GetGame().GetItemManager().GetItemByName(recipe.Result);
            if (itemByName1 == null)
                return;
            bool success = true;
            foreach (KeyValuePair<string, int> keyValuePair in recipe.ItemsNeeded)
            {
                for (int index = 1; index <= keyValuePair.Value; ++index)
                {
                    ItemData itemByName2 = AkiledEnvironment.GetGame().GetItemManager().GetItemByName(keyValuePair.Key);
                    if (itemByName2 == null)
                    {
                        success = false;
                    }
                    else
                    {
                        Item firstItemByBaseId = Session.GetHabbo().GetInventoryComponent().GetFirstItemByBaseId(itemByName2.Id);
                        if (firstItemByBaseId == null)
                            success = false;
                        else
                            Session.GetHabbo().GetInventoryComponent().RemoveItem(firstItemByBaseId.Id);
                    }
                }
            }
            if (success)
            {
                Session.GetHabbo().GetInventoryComponent().AddNewItem(0, itemByName1.Id, "");
                Session.SendMessage((IServerPacket)new FurniListUpdateComposer());
            }
            Session.SendMessage((IServerPacket)new CraftingResultComposer(recipe, success));
        }
    }
}
