using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Items
{
    internal class CrackableItem
    {
        internal uint ItemId;
        internal List<CrackableRewards> Rewards;

        internal CrackableItem(DataRow dRow)
        {
            this.ItemId = Convert.ToUInt32(dRow["item_baseid"]);
            string str1 = (string)dRow["rewards"];
            this.Rewards = new List<CrackableRewards>();
            string str2 = str1;
            char[] chArray = new char[1] { ';' };
            foreach (string str3 in str2.Split(chArray))
                this.Rewards.Add(new CrackableRewards(this.ItemId, str3.Split(',')[0], str3.Split(',')[1], uint.Parse(str3.Split(',')[2])));
        }
    }
}
