using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Roleplay.Weapon
{
    public class RPWeaponManager
    {
        private readonly Dictionary<int, RPWeapon> _weaponCac;
        private readonly Dictionary<int, RPWeapon> _weaponFar;

        public RPWeaponManager()
        {
            this._weaponCac = new Dictionary<int, RPWeapon>();
            this._weaponFar = new Dictionary<int, RPWeapon>();
        }

        public RPWeapon GetWeaponCac(int Id)
        {
            RPWeapon weapon = new RPWeapon(0, 1, 3, RPWeaponInteraction.NONE, 0, 1, 1);
            if (!this._weaponCac.ContainsKey(Id))
                return weapon;

            this._weaponCac.TryGetValue(Id, out weapon);
            return weapon;
        }

        public RPWeapon GetWeaponGun(int Id)
        {
            RPWeapon weapon = new RPWeapon(0, 5, 10, RPWeaponInteraction.NONE, 164, 3, 10);
            if (!this._weaponFar.ContainsKey(Id))
                return weapon;

            this._weaponFar.TryGetValue(Id, out weapon);
            return weapon;
        }

        public void Init()
        {
            this._weaponCac.Clear();
            this._weaponFar.Clear();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM roleplay_weapon");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                    {
                        if (this._weaponCac.ContainsKey(Convert.ToInt32(dataRow["id"])) || this._weaponFar.ContainsKey(Convert.ToInt32(dataRow["id"])))
                            continue;

                        if ((string)dataRow["type"] == "cac")
                            this._weaponCac.Add((int)dataRow["id"], new RPWeapon((int)dataRow["id"], (int)dataRow["domage_min"], (int)dataRow["domage_max"], RPWeaponInteractions.GetTypeFromString((string)dataRow["interaction"]), (int)dataRow["enable"], (int)dataRow["freeze_time"], (int)dataRow["distance"]));
                        else
                            this._weaponFar.Add((int)dataRow["id"], new RPWeapon((int)dataRow["id"], (int)dataRow["domage_min"], (int)dataRow["domage_max"], RPWeaponInteractions.GetTypeFromString((string)dataRow["interaction"]), (int)dataRow["enable"], (int)dataRow["freeze_time"], (int)dataRow["distance"]));
                    }
                }
            }
        }
    }
}
