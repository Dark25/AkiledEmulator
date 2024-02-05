using Akiled;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.HabboHotel;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Users;
using AkiledEmulator.HabboHotel.Hotel.Extras;
using MySqlX.XDevAPI;
using System;
using System.Collections.Generic;

namespace AkiledEmulator.HabboHotel.Hotel.Giveaway
{
    public static class GiveAwayConfigs
    {
        /// <summary>
        /// Configuration of the system it's enabled
        /// </summary>
        public static bool enabled = false;

        /// <summary>
        /// Configuration the timestamp to end the giveaway
        /// </summary>
        public static double timestamp;

        /// <summary>
        /// Time to block a user and he can't participate (in minutes)
        /// </summary>
        public static int durationOfBlock = string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager()?.TryGetValue("give_away.block.minutes")) ? 5 : int.Parse(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.block.minutes"));

        /// <summary>
        /// List of participants
        /// </summary>  
        public static List<int> participants = new List<int>();

        /// <summary>
        /// Started by staff
        /// </summary>
        public static string startedByUsername;

        /// <summary>
        /// Description
        /// </summary>
        public static string description;

        /// <summary>
        /// Max Winners in GiveAway
        /// </summary>
        public static int maxWinners;

        /// <summary>
        /// Stop the Give Away
        /// </summary>
        public static void Stop()
        {
            //Check if is not enabled
            if (!enabled)
                return;

            try
            {
                //Updates the variable
                enabled = !enabled;

                //Verificar os vencedores
                if (participants.Count > 0)
                {
                    #region Rewards
                    int credits = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) : 0;
                    int duckets = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) : 0;
                    int diamonds = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) : 0;
                    string badge = Convert.ToString(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.badge"));
                    int item = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) : 0;
                    #endregion

                    List<int> winners = getWinners();

                    List<string> usersWinners = new List<string>();

                    foreach (int id in winners)
                    {
                        if (!AkiledEnvironment.GetGame().GetGiveAwayBlocks().TryGet(id, out GiveAwayBlocks giveAwayBlocks))
                            AkiledEnvironment.GetGame().GetGiveAwayBlocks().InsertBlock(id);

                        GameClient client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(id);
                        if (client != null)
                        {
                            //Give rewards
                            giveRewards(client, credits, duckets, diamonds, badge, item);

                            usersWinners.Add(client.GetHabbo().Username);
                        }
                        else
                        {
                            Habbo userOffline = AkiledEnvironment.GetHabboById(id);
                            if (userOffline == null)
                                continue;

                            usersWinners.Add(userOffline.Username);

                            RewardsOffline.Rewards(userOffline.Id, credits, duckets, diamonds, badge, item);
                        }
                    }

                    //Message
                    AkiledEnvironment.GetGame().GetClientManager().SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("giveaway.winners", Language.SPANISH) + string.Join(", ", usersWinners));
                }
                else
                    AkiledEnvironment.GetGame().GetClientManager().SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("giveaway.no_winners", Language.SPANISH));

                //Reset variables
                Reset();
            }
            catch (Exception e)
            {
                Reset();
                Logging.LogCriticalException(e.ToString());
            }
        }
        /// <summary>
        /// Give the rewards
        /// </summary>
        /// <param name="client">The user client</param>
        private static void giveRewards(GameClient client, int credits, int duckets, int diamonds, string badge, int item)
        {
            if (credits > 0)
            {
                client.GetHabbo().Credits += credits;
                client.SendPacket(new CreditBalanceComposer(client.GetHabbo().Credits));
            }

            if (duckets > 0)
            {
                client.GetHabbo().Duckets += duckets;
                client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, +duckets));
            }

            if (diamonds > 0)
            {
                client.GetHabbo().AkiledPoints += diamonds;
                client.SendPacket(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, diamonds, 105));
            }

            if (!string.IsNullOrEmpty(badge))
                if (!client.GetHabbo().GetBadgeComponent().HasBadge(badge))
                    client.GetHabbo().GetBadgeComponent().GiveBadge(badge, 0, true, client);

            if (item > 0)
            {
                ItemData ItemData;
                if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(item, out ItemData))
                {

                    Console.WriteLine("No se pudo cargar el artículo del sorteo" + item + ", no se encontraron registros del furni.", ConsoleColor.Red);

                }
                List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, client.GetHabbo(), "", 1);
                foreach (Item Item in Items)
                {
                    client.GetHabbo().GetInventoryComponent().TryAddItem(Item);
                    client.SendPacket(new FurniListNotificationComposer(Item.Id, 1));

                }

            }




            //Message
            client.SendMessage(new RoomCustomizedAlertComposer(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("giveaway.win", client.Langue), Environment.NewLine, credits, duckets, diamonds, badge, item)));
        }

        /// <summary>
        /// Select random winners
        /// </summary>
        /// <returns>List of the users ids.</returns>
        private static List<int> getWinners()
        {
            List<int> winnersIds = new List<int>();
            Random rand = new Random();

            int wins = (participants.Count < maxWinners) ? 1 : maxWinners;
            int randomUserID = 0;
            int x = 0;

            while (x < wins)
            {
                randomUserID = participants[rand.Next(participants.Count)];

                if (winnersIds.Contains(randomUserID))
                    continue;

                winnersIds.Add(randomUserID);
                x++;
            }

            return winnersIds;
        }

        /// <summary>
        /// Start the Give Away
        /// </summary>
        public static void Start()
        {
            #region Rewards
            int credits = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.credits")) : 0;
            int duckets = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.duckets")) : 0;
            int diamonds = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.diamonds")) : 0;
            string badge = Convert.ToString(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.badge"));
            int item = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) < int.MaxValue && !string.IsNullOrEmpty(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) ? Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("give_away.give.item")) : 0;
            string giveaway_image = (AkiledEnvironment.GetConfig().data["giveaway_image"]);
            #endregion

            ItemData ItemData;

            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(item, out ItemData))
            {
                ItemData = null;

            }


            //Check if is enabled
            if (!enabled)
                return;

            DateTime timeStart = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp - AkiledEnvironment.GetUnixTimestamp());


            AkiledEnvironment.GetGame().GetClientManager().SendMessage(new RoomNotificationComposer(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("giveaway.start.title", Language.SPANISH), description),string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("giveaway.start.desc", Language.SPANISH), startedByUsername, credits, duckets, diamonds, (badge ?? "nothing"), (ItemData != null ? ItemData.publicname : "nothing"), (timeStart.Minute > 0 ? timeStart.Minute + " minutes " + timeStart.Second + " Seconds!" : timeStart.Second + " " + " Second")), giveaway_image,""));




            Console.WriteLine("Se ha inciado un Sorteo por " + startedByUsername + "y termina en " + timeStart.Minute + " Minutos " + timeStart.Second + " Segundos!");
        }


        /// <summary>
        /// Reset and clean the variables
        /// </summary>
        private static void Reset()
        {
            timestamp = 0;
            startedByUsername = "";
            description = "";
            maxWinners = 0;
            participants.Clear();
        }
    }
}