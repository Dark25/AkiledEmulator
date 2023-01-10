using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.Communication.RCON.Commands.User
{
    class GiveUserCurrencyCommand : IRCONCommand
    {

        public bool TryExecute(string[] parameters)
        {
            int userId = 0;
            if (!int.TryParse(parameters[0].ToString(), out userId))
                return false;

            GameClient client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;

            // Validate the currency type
            if (string.IsNullOrEmpty(Convert.ToString(parameters[1])))
                return false;

            string currency = Convert.ToString(parameters[1]);

            int amount = 0;
            if (!int.TryParse(parameters[2].ToString(), out amount))
                return false;

            switch (currency)
            {
                default:
                    return false;

                case "coins":
                case "credits":
                case "creditos":
                    {
                        client.GetHabbo().Credits += amount;

                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `credits` = @credits WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("credits", client.GetHabbo().Credits);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));
                        break;
                    }

                case "pixels":
                case "esmeraldas":
                    {
                        client.GetHabbo().Duckets += amount;

                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `activity_points` = @duckets WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("duckets", client.GetHabbo().Duckets);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, amount));
                        break;
                    }

                case "diamonds":
                case "planetas":
                    {
                        client.GetHabbo().AkiledPoints += amount;

                        using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                        {
                            dbClient.SetQuery("UPDATE `users` SET `vip_points` = @diamonds WHERE `id` = @id LIMIT 1");
                            dbClient.AddParameter("diamonds", client.GetHabbo().AkiledPoints);
                            dbClient.AddParameter("id", client.GetHabbo().Id);
                            dbClient.RunQuery();
                        }

                        client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, 0, 105));
                        break;
                    }


            }
            return true;
        }
    }
}