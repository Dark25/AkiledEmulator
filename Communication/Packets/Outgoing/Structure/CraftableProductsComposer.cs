using Akiled.HabboHotel.Items.Crafting;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class CraftableProductsComposer : ServerPacket
    {
        public CraftableProductsComposer()
          : base(1000)
        {
            CraftingManager craftingManager = AkiledEnvironment.GetGame().GetCraftingManager();
            this.WriteInteger(craftingManager.CraftingRecipes.Count);
            foreach (CraftingRecipe craftingRecipe in craftingManager.CraftingRecipes.Values)
            {
                this.WriteString(craftingRecipe.Result);
                this.WriteString(craftingRecipe.Result);
            }
            this.WriteInteger(craftingManager.CraftableItems.Count);
            foreach (string craftableItem in craftingManager.CraftableItems)
                this.WriteString(craftableItem);
        }
    }
}
