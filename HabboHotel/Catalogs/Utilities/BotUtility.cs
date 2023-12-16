using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.RoomBots;
using Akiled.HabboHotel.Users.Inventory.Bots;
using System;
using System.Data;

namespace Akiled.HabboHotel.Catalog.Utilities
{
    public static class BotUtility
    {
        public static Bot CreateBot(ItemData Data, int OwnerId)
        {
            if (!AkiledEnvironment.GetGame().GetCatalog().TryGetBot(Data.Id, out CatalogBot CataBot))
                return null;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("INSERT INTO bots (`user_id`,`name`,`motto`,`look`,`gender`) VALUES ('" + OwnerId + "', '" + CataBot.Name + "', '" + CataBot.Motto + "', '" + CataBot.Figure + "', '" + CataBot.Gender + "')");
                int Id = Convert.ToInt32(dbClient.InsertQuery());

                dbClient.SetQuery("SELECT `id`,`user_id`,`name`,`motto`,`look`,`gender` FROM `bots` WHERE `user_id` = '" + OwnerId + "' AND `id` = '" + Id + "' LIMIT 1");
                DataRow BotData = dbClient.GetRow();

                return new Bot(Id, OwnerId, CataBot.Name, CataBot.Motto, CataBot.Figure, CataBot.Gender, false, true, "", 0, false, 0, 0, 0, AIType.Generic);
            }

        }
        public static AIType GetAIFromString(string type) => type switch
        {
            "pet" => AIType.Pet,
            "generic" => AIType.Generic,
            "roleplaybot" => AIType.RolePlayBot,
            "roleplaypet" => AIType.RolePlayPet,
            "chatgpt" => AIType.ChatGPT,
            _ => AIType.Generic,
        };
    }
}