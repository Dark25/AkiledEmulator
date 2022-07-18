
using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Items.Crafting
{
    public class CraftingManager
    {

        internal Dictionary<string, CraftingRecipe> CraftingRecipes;
        internal List<string> CraftableItems;

        public CraftingManager()
        {
            this.CraftingRecipes = new Dictionary<string, CraftingRecipe>();
            this.CraftableItems = new List<string>();
        }

        internal void Init()
        {
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.CraftingRecipes.Clear();
                queryReactor.SetQuery("SELECT * FROM crafting_recipes");
                foreach (DataRow row in (InternalDataCollectionBase)queryReactor.GetTable().Rows)
                {
                    CraftingRecipe craftingRecipe = new CraftingRecipe((string)row["id"], (string)row["items"], (string)row["result"], (int)row["type"]);
                    this.CraftingRecipes.Add((string)row["id"], craftingRecipe);
                }
                this.CraftableItems.Clear();
                queryReactor.SetQuery("SELECT * FROM crafting_items");
                foreach (DataRow row in (InternalDataCollectionBase)queryReactor.GetTable().Rows)
                    this.CraftableItems.Add((string)row["itemName"]);
            }
            Console.WriteLine("Crafting Manager -> Lito");
        }

        internal CraftingRecipe GetRecipe(string name) => this.CraftingRecipes.ContainsKey(name) ? this.CraftingRecipes[name] : (CraftingRecipe)null;

        internal CraftingRecipe GetRecipeByPrize(string name)
        {
            foreach (CraftingRecipe recipeByPrize in this.CraftingRecipes.Values)
            {
                if (recipeByPrize.Result == name)
                    return recipeByPrize;
            }
            return (CraftingRecipe)null;
        }
    }
}
