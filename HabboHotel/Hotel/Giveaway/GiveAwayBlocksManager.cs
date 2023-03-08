using Akiled;
using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace AkiledEmulator.HabboHotel.Hotel.Giveaway
{
    public class GiveAwayBlocksManager
    {
        private Dictionary<int, GiveAwayBlocks> giveAwayBlock;

        public GiveAwayBlocksManager()
        {
            giveAwayBlock = new Dictionary<int, GiveAwayBlocks>();
            Init(AkiledEnvironment.GetDatabaseManager().GetQueryReactor());
        }

        public void Init(IQueryAdapter dbClient)
        {
            if (giveAwayBlock.Count > 0)
                giveAwayBlock.Clear();

            dbClient.SetQuery("SELECT user_id, added FROM give_away_blocks");
            DataTable dataTable = dbClient.GetTable();

            if (dataTable != null)
                foreach (DataRow row in dataTable.Rows)
                    giveAwayBlock.Add(Convert.ToInt32(row["user_id"]), new GiveAwayBlocks(Convert.ToInt32(row["user_id"]), Convert.ToDouble(row["added"])));
        }

        public void InsertBlock(int userId)
        {
            if (!giveAwayBlock.ContainsKey(userId))
            {
                double time = AkiledEnvironment.GetUnixTimestamp() + (GiveAwayConfigs.durationOfBlock * 60);
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("INSERT INTO give_away_blocks (user_id,added) VALUES (" + userId + ", " + time + ")");

                giveAwayBlock.Add(userId, new GiveAwayBlocks(userId, time));
            }
        }

        private void RemoveBlock(int userId)
        {
            if (giveAwayBlock.ContainsKey(userId))
            {
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    dbClient.RunQuery("DELETE FROM give_away_blocks WHERE user_id = " + userId);

                giveAwayBlock.Remove(userId);
            }
        }

        public bool CheckTimeExpire(int userId, GiveAwayBlocks giveAwayBlocks)
        {
            if (giveAwayBlocks.timestamp > AkiledEnvironment.GetUnixTimestamp())
                return true;

            RemoveBlock(userId);
            return false;
        }

        public bool TryGet(int userId, out GiveAwayBlocks giveAwayBlocks)
        {
            if (giveAwayBlock.TryGetValue(userId, out giveAwayBlocks))
                return true;
            return false;
        }
    }

    public class GiveAwayBlocks
    {
        public int userId { get; set; }
        public double timestamp { get; set; }

        public GiveAwayBlocks(int userId, double added)
        {
            this.userId = userId;
            this.timestamp = added;
        }
    }
}
