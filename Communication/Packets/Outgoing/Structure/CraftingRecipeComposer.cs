using Akiled.HabboHotel.Items.Crafting;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class CraftingRecipeComposer : ServerPacket
    {
        public CraftingRecipeComposer(CraftingRecipe recipe)
          : base(2774)
        {
            this.WriteInteger(recipe.ItemsNeeded.Count);
            foreach (KeyValuePair<string, int> keyValuePair in recipe.ItemsNeeded)
            {
                this.WriteInteger(keyValuePair.Value);
                this.WriteString(keyValuePair.Key);
            }
        }
    }
}
