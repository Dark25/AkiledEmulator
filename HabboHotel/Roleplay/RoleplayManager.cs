using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Roleplay.Enemy;
using Akiled.HabboHotel.Roleplay.Troc;
using Akiled.HabboHotel.Roleplay.Weapon;
using System;
using System.Collections.Concurrent;
using System.Data;

namespace Akiled.HabboHotel.Roleplay
{
    public class RoleplayManager
    {
        private readonly ConcurrentDictionary<int, RolePlayerManager> _rolePlay;
        private readonly RPItemManager _roleplayItemManager;
        private readonly RPWeaponManager _roleplayWeaponManager;
        private readonly RPEnemyManager _roleplayEnemyManager;
        private readonly RPTrocManager _roleplayTrocManager;

        public RoleplayManager()
        {
            this._rolePlay = new ConcurrentDictionary<int, RolePlayerManager>();

            this._roleplayItemManager = new RPItemManager();
            this._roleplayWeaponManager = new RPWeaponManager();
            this._roleplayEnemyManager = new RPEnemyManager();
            this._roleplayTrocManager = new RPTrocManager();
        }

        public RolePlayerManager GetRolePlay(int Ownerid)
        {
            if (!this._rolePlay.ContainsKey(Ownerid))
                return null;

            RolePlayerManager RP = null;
            this._rolePlay.TryGetValue(Ownerid, out RP);
            return RP;
        }

        public RPTrocManager GetTrocManager()
        {
            return this._roleplayTrocManager;
        }

        public RPWeaponManager GetWeaponManager()
        {
            return this._roleplayWeaponManager;
        }

        public RPItemManager GetItemManager()
        {
            return this._roleplayItemManager;
        }

        public RPEnemyManager GetEnemyManager()
        {
            return this._roleplayEnemyManager;
        }

        public void Init()
        {
            this._roleplayItemManager.Init();
            this._roleplayWeaponManager.Init();
            this._roleplayEnemyManager.Init();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT ownerid, hopital_id, prison_id FROM roleplay");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                    {
                        if (!this._rolePlay.ContainsKey(Convert.ToInt32(dataRow["ownerid"])))
                            this._rolePlay.TryAdd(Convert.ToInt32(dataRow["ownerid"]), new RolePlayerManager(Convert.ToInt32(dataRow["ownerid"]), Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"])));
                        else
                        {
                            this.GetRolePlay(Convert.ToInt32(dataRow["ownerid"])).Update(Convert.ToInt32(dataRow["hopital_id"]), Convert.ToInt32(dataRow["prison_id"]));
                        }
                    }
                }
            }
        }
    }
}
