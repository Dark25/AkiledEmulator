
using System;
using System.Collections.Generic;

namespace Akiled.HabboHotel.Items.Crafting
{
    internal class CraftingRecipe
    {
        internal string Id;
        internal Dictionary<string, int> ItemsNeeded;
        internal string Result;
        internal int Type;

        public CraftingRecipe(string id, string itemsNeeded, string result, int type)
        {
            this.Id = id;
            this.ItemsNeeded = new Dictionary<string, int>();
            string str1 = itemsNeeded;
            char[] chArray1 = new char[1] { ';' };
            foreach (string str2 in str1.Split(chArray1))
            {
                char[] chArray2 = new char[1] { ':' };
                string[] strArray = str2.Split(chArray2);
                if (strArray.Length == 2)
                    this.ItemsNeeded.Add(strArray[0], Convert.ToInt32(strArray[1]));
            }
            this.Type = type;
            this.Result = result;
        }
    }
}
