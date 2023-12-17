namespace Akiled.HabboHotel.LandingView;

using Akiled.Database.Daos.Emulator;
using Akiled.Database.Daos.User;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;


public class HallOfFameManager
{
    private static readonly int MAX_USERS = 10;
    private DateTime _lastUpdate;
    public List<Habbo> UserRanking { get; private set; }

    public HallOfFameManager()
    {
        this.UserRanking = new List<Habbo>();

        this._lastUpdate = DateTime.UnixEpoch.AddSeconds(Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("hof.lastupdate")));
        this._hofStopwatch = new();
        this._hofStopwatch.Start();
    }

    public void Init(IQueryAdapter dbClient)
    {
        this.UserRanking.Clear();

        using var dTable = UserDao.GetTop10ByGamePointMonth(dbClient);

        foreach (DataRow dRow in dTable.Rows)
        {

            var userId = dRow["id"];

          
            var user = AkiledEnvironment.GetHabboById(Convert.ToInt32(userId));

            if (user != null)
            {
                this.UserRanking.Add(user);
            }
        }
    }

    private readonly Stopwatch _hofStopwatch;
    public void OnCycle()
    {
        if (this._hofStopwatch.ElapsedMilliseconds >= 60000)
        {
            this._hofStopwatch.Restart();

            if (this._lastUpdate.Month == DateTime.UtcNow.Month)
            {
                return;
            }

            this._lastUpdate = DateTime.UtcNow;

            foreach (var client in AkiledEnvironment.GetGame().GetClientManager().GetClients.ToList())
            {
                if (client == null || client.GetHabbo == null)
                {
                    continue;
                }

                client.GetHabbo().GamePointsMonth = 0;
            }

            this.UserRanking.Clear();

            var dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            EmulatorSettingDao.Update(dbClient, "hof.lastupdate", AkiledEnvironment.GetUnixTimestamp().ToString());
        }
    }

    public void UpdateRakings(Habbo user)
    {
        if (this.UserRanking.Contains(user))
        {
            this.UserRanking.Remove(user);
        }

        this.UserRanking.Add(user);

        this.UserRanking = this.UserRanking.OrderBy(x => x.GamePointsMonth).Take(MAX_USERS).ToList();
    }
}