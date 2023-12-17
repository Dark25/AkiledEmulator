namespace Akiled.Database.Daos.Emulator;

using Akiled.Database.Interfaces;
using System.Data;

internal sealed class EmulatorSettingDao
{
    internal static DataTable GetAll(IQueryAdapter dbClient)
    {
        dbClient.SetQuery("SELECT `key`, `value` FROM `server_settings`");
        return dbClient.GetTable();
    }

    internal static void Update(IQueryAdapter dbClient, string key, string value)
    {
        dbClient.SetQuery("UPDATE `server_settings` SET `value` = @value WHERE `variable` = @key");
        dbClient.AddParameter("value", value);
        dbClient.AddParameter("key", key);
        dbClient.RunQuery();
    }
}