using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Subscriptions
{
    public class SubscriptionManager
    {

        private readonly Dictionary<int, SubscriptionData> _subscriptions = new();

        public void Init()
        {
            if (_subscriptions.Count > 0)
                _subscriptions.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `subscriptions`;");
                DataTable GetSubscriptions = dbClient.GetTable();

                if (GetSubscriptions != null)
                {
                    foreach (DataRow Row in GetSubscriptions.Rows)
                    {
                        if (!_subscriptions.ContainsKey(Convert.ToInt32(Row["id"])))
                            _subscriptions.Add(Convert.ToInt32(Row["id"]), new SubscriptionData(Convert.ToInt32(Row["id"]), Convert.ToString(Row["name"]), Convert.ToString(Row["badge_code"]), Convert.ToInt32(Row["credits"]), Convert.ToInt32(Row["duckets"]), Convert.ToInt32(Row["respects"])));
                    }
                }
            }

            Console.WriteLine(_subscriptions.Count + " subscriptions cargada!!.");
        }

        public bool TryGetSubscriptionData(int Id, out SubscriptionData Data)
        {
            return _subscriptions.TryGetValue(Id, out Data);
        }
    }
}