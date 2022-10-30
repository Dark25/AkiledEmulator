using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Roles
{
    public class RoleManager
    {
        private readonly Dictionary<string, int> Rights;

        public RoleManager()
        {
            this.Rights = new Dictionary<string, int>();
        }

        public void Init()
        {
            this.Rights.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT fuse, rank FROM fuserights");
                DataTable table1 = dbClient.GetTable();

                if (table1 == null) return;

                foreach (DataRow dataRow in table1.Rows) this.Rights.Add((string)dataRow["fuse"], Convert.ToInt32(dataRow["rank"]));
            }
            Console.WriteLine("Permisos -> Listo!");
        }

        public bool RankHasRight(int RankId, string Fuse)
        {
            if (!this.Rights.ContainsKey(Fuse)) return false;

            return RankId >= this.Rights[Fuse];
        }
    }
}
