using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Roleplay.Enemy
{
    public class RPEnemyManager
    {
        private readonly Dictionary<int, RPEnemy> _enemyBot;
        private readonly Dictionary<int, RPEnemy> _enemyPet;

        public RPEnemyManager()
        {
            this._enemyBot = new Dictionary<int, RPEnemy>();
            this._enemyPet = new Dictionary<int, RPEnemy>();
        }

        public RPEnemy GetEnemyBot(int Id)
        {
            if (!this._enemyBot.ContainsKey(Id))
                return null;

            RPEnemy enemy = null;
            this._enemyBot.TryGetValue(Id, out enemy);
            return enemy;
        }

        public RPEnemy GetEnemyPet(int Id)
        {
            if (!this._enemyPet.ContainsKey(Id))
                return null;

            RPEnemy enemy = null;
            this._enemyPet.TryGetValue(Id, out enemy);
            return enemy;
        }

        public void Init()
        {
            this._enemyBot.Clear();
            this._enemyPet.Clear();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM roleplay_enemy");
                DataTable table1 = dbClient.GetTable();
                if (table1 != null)
                {
                    foreach (DataRow dataRow in table1.Rows)
                    {
                        if ((this._enemyBot.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "bot") || (this._enemyPet.ContainsKey(Convert.ToInt32(dataRow["id"])) && (string)dataRow["type"] == "pet"))
                            continue;

                        RPEnemy Config = new RPEnemy((int)dataRow["id"], (int)dataRow["health"], (int)dataRow["weaponFarId"], (int)dataRow["weaponCacId"], (int)dataRow["deadTimer"],
                            (int)dataRow["lootItemId"], (int)dataRow["moneyDrop"], Convert.ToInt32((string)dataRow["dropScriptId"]), (int)dataRow["teamId"], (int)dataRow["aggroDistance"],
                            (int)dataRow["zoneDistance"], Convert.ToInt32((string)dataRow["resetPosition"]) == 1, (int)dataRow["lostAggroDistance"], Convert.ToInt32((string)dataRow["zombieMode"]) == 1);

                        if ((string)dataRow["type"] == "bot")
                            this._enemyBot.Add((int)dataRow["id"], Config);
                        else
                            this._enemyPet.Add((int)dataRow["id"], Config);
                    }
                }
            }
        }

        public RPEnemy AddEnemyBot(int BotId)
        {
            if (this._enemyBot.ContainsKey(BotId))
                return this.GetEnemyBot(BotId);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("INSERT INTO `roleplay_enemy` (`id`, `type`) VALUES ('" + BotId + "', 'bot');");

            RPEnemy EnemyConfig = new RPEnemy(BotId, 100, 1, 4, 30, 0, 0, 5461, 0, 0, 0, true, 12, false);
            this._enemyBot.Add(BotId, EnemyConfig);
            return this.GetEnemyBot(BotId);
        }

        public RPEnemy AddEnemyPet(int PetId)
        {
            if (this._enemyPet.ContainsKey(PetId))
                return this.GetEnemyPet(PetId);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("INSERT INTO `roleplay_enemy` (`id`, `type`, `weaponFarId`) VALUES ('" + PetId + "', 'pet', '0');");

            RPEnemy EnemyConfig = new RPEnemy(PetId, 100, 0, 0, 0, 0, 0, 5461, 0, 0, 0, true, 12, false);
            this._enemyPet.Add(PetId, EnemyConfig);
            return this.GetEnemyPet(PetId);
        }

        internal void RemoveEnemyBot(int BotId)
        {
            if (!this._enemyBot.ContainsKey(BotId))
                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + BotId + "'");

            this._enemyBot.Remove(BotId);
        }

        internal void RemoveEnemyPet(int PetId)
        {
            if (!this._enemyPet.ContainsKey(PetId))
                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("DELETE FROM roleplay_enemy WHERE id = '" + PetId + "'");

            this._enemyPet.Remove(PetId);
        }
    }
}
