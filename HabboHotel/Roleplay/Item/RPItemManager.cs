using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Roleplay.Item;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Roleplay
{
    public class RPItemManager
    {
        private readonly Dictionary<int, RPItem> _items;

        public RPItemManager()
        {
            this._items = new Dictionary<int, RPItem>();
        }

        public RPItem GetItem(int Id)
        {
            if (!this._items.ContainsKey(Id))
                return null;

            RPItem item = null;
            this._items.TryGetValue(Id, out item);
            return item;
        }

        public void Init()
        {
            this._items.Clear();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM roleplay_items");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                    {
                        if (!this._items.ContainsKey(Convert.ToInt32(dataRow["id"])))
                            this._items.Add((int)dataRow["id"], new RPItem((int)dataRow["id"], (string)dataRow["name"], (string)dataRow["desc"], (int)dataRow["price"], (string)dataRow["type"], (int)dataRow["value"], ((string)dataRow["allowstack"]) == "1", RPItemCategorys.GetTypeFromString((string)dataRow["category"])));
                    }
                }
            }
        }
    }
}
