using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Roleplay.Player;
using System.Collections.Concurrent;
using System.Data;

namespace Akiled.HabboHotel.Roleplay
{
    public class RolePlayerManager
    {
        private readonly ConcurrentDictionary<int, RolePlayer> _player;
        private readonly int _id;
        public int PrisonId;
        public int HopitalId;

        public RolePlayerManager(int Id, int HopitalId, int PrisonId)
        {
            this._id = Id;
            this.PrisonId = PrisonId;
            this.HopitalId = HopitalId;
            this._player = new ConcurrentDictionary<int, RolePlayer>();
        }

        public void Update(int HopitalId, int PrisonId)
        {
            this.PrisonId = PrisonId;
            this.HopitalId = HopitalId;
        }

        public void AddPlayer(int UserId)
        {
            if (this._player.ContainsKey(UserId))
                return;

            RolePlayer player = null;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT * FROM user_rp WHERE user_id = '" + UserId + "' AND roleplay_id = '" + this._id + "'");

                DataRow dRow = queryreactor.GetRow();
                if (dRow == null)
                {
                    queryreactor.RunQuery("INSERT INTO `user_rp` (`user_id`, `roleplay_id`) VALUES ('" + UserId + "', '" + this._id + "')");
                    player = new RolePlayer(this._id, UserId, 100, 0, 0, 0, 100, 0, 0);
                }
                else
                {
                    player = new RolePlayer(this._id, UserId, (int)dRow["health"], (int)dRow["money"], (int)dRow["munition"], (int)dRow["exp"], (int)dRow["energy"], (int)dRow["weapon_far"], (int)dRow["weapon_cac"]);
                }
            }

            if (player != null)
            {
                this._player.TryAdd(UserId, player);
                player.SendUpdate(true);
                player.LoadInventory(); //Faire le packet LoadInventoryRP
            }
        }

        public void RemovePlayer(int Id)
        {
            RolePlayer player = this.GetPlayer(Id);

            if (player == null) return;

            player.Destroy();
            this._player.TryRemove(Id, out player);
        }

        public RolePlayer GetPlayer(int Id)
        {
            if (!this._player.ContainsKey(Id))
                return null;

            RolePlayer player = null;
            this._player.TryGetValue(Id, out player);
            return player;
        }
    }
}
