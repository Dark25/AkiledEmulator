using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items.Crafting;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    internal class SetCraftingRecipeEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string str = Packet.PopString();
            CraftingRecipe craftingRecipe1 = (CraftingRecipe)null;
            foreach (CraftingRecipe craftingRecipe2 in AkiledEnvironment.GetGame().GetCraftingManager().CraftingRecipes.Values)
            {
                if (craftingRecipe2.Result.Contains(str))
                {
                    craftingRecipe1 = craftingRecipe2;
                    break;
                }
            }
            CraftingRecipe recipe = AkiledEnvironment.GetGame().GetCraftingManager().GetRecipe(craftingRecipe1.Id);
            if (recipe == null)
                return;
            Session.SendMessage((IServerPacket)new CraftingRecipeComposer(recipe));
        }
    }
}
