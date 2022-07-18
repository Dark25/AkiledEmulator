using Akiled.HabboHotel.Items.Crafting;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class CraftingResultComposer : ServerPacket
    {
        public CraftingResultComposer(CraftingRecipe recipe, bool success)
          : base(3128)
        {
            this.WriteBoolean(success);
            this.WriteString(recipe.Result);
            this.WriteString(recipe.Result);
        }
    }
}
