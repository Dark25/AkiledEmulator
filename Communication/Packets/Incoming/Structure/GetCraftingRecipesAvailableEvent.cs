using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Items.Crafting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class GetCraftingRecipesAvailableEvent : IPacketEvent
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
            CraftingRecipe craftingRecipe1 = (CraftingRecipe)null;
            foreach (KeyValuePair<string, CraftingRecipe> craftingRecipe2 in AkiledEnvironment.GetGame().GetCraftingManager().CraftingRecipes)
            {
                bool flag = false;
                foreach (KeyValuePair<string, int> keyValuePair in craftingRecipe2.Value.ItemsNeeded)
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
                    craftingRecipe1 = craftingRecipe2.Value;
                    break;
                }
            }
            if (craftingRecipe1 == null)
                Session.SendMessage((IServerPacket)new CraftingFoundComposer(0, false));
            else
                Session.SendMessage((IServerPacket)new CraftingFoundComposer(0, true));
        }
    }
}
