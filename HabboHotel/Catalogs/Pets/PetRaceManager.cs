using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Catalog.Pets
{
    public class PetRaceManager
    {
        private readonly List<PetRace> _races = new List<PetRace>();

        public void Init()
        {
            if (this._races.Count > 0)
                this._races.Clear();

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `catalog_pet_races`");
                DataTable Table = dbClient.GetTable();

                if (Table != null)
                {
                    foreach (DataRow Row in Table.Rows)
                    {
                        PetRace Race = new PetRace(Convert.ToInt32(Row["raceid"]), Convert.ToInt32(Row["color1"]), Convert.ToInt32(Row["color2"]), (Convert.ToString(Row["has1color"]) == "1"), (Convert.ToString(Row["has2color"]) == "1"));
                        if (!this._races.Contains(Race))
                            this._races.Add(Race);
                    }
                }
            }
        }

        public List<PetRace> GetRacesForRaceId(int RaceId)
        {
            return this._races.Where(Race => Race.RaceId == RaceId).ToList();
        }
    }
}