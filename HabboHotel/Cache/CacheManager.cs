
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Cache.Process;
using Akiled.HabboHotel.GameClients;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Cache
{
    public class CacheManager
    {
        private ConcurrentDictionary<int, UserCache> _usersCached;
        private ProcessComponent _process;

        public CacheManager()
        {
            this._usersCached = new ConcurrentDictionary<int, UserCache>();
            this._process = new ProcessComponent();
            this._process.Init();
            Console.WriteLine(" Manager Cache -> OK!");
        }

        public bool ContainsUser(int Id) => this._usersCached.ContainsKey(Id);

        public UserCache GenerateUser(int Id)
        {
            UserCache User = (UserCache)null;
            if (this._usersCached.ContainsKey(Id) && this.TryGetUser(Id, out User))
                return User;
            GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Id);
            if (clientByUserId != null && clientByUserId.GetHabbo() != null)
            {
                User = new UserCache(Id, clientByUserId.GetHabbo().Username, clientByUserId.GetHabbo().Motto, clientByUserId.GetHabbo().Look);
                this._usersCached.TryAdd(Id, User);
                return User;
            }
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.SetQuery("SELECT `username`, `motto`, `look` FROM users WHERE id = @id LIMIT 1");
                queryReactor.AddParameter("id", (object)Id);
                DataRow row = queryReactor.GetRow();
                if (row != null)
                {
                    User = new UserCache(Id, row["username"].ToString(), row["motto"].ToString(), row["look"].ToString());
                    this._usersCached.TryAdd(Id, User);
                }
            }
            return User;
        }

        public bool TryRemoveUser(int Id, out UserCache User) => this._usersCached.TryRemove(Id, out User);

        public bool TryGetUser(int Id, out UserCache User) => this._usersCached.TryGetValue(Id, out User);

        public ICollection<UserCache> GetUserCache() => this._usersCached.Values;
    }
}
