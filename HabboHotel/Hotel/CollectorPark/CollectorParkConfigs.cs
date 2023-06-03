using Akiled;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkiledEmulator.HabboHotel.Hotel.CollectorPark
{
    public class CollectorParkConfigs
    {
        public static List<int> users = new List<int>();
        public static List<int> usersToRemove = new List<int>();

        public static bool enabled = false;
        public static byte update = 0;
        public static int roomId = 0;
        private static int baseItemID = 0;

        //Define the times to next reward in minutes
        public static int minTimeNextReward = 0;
        public static int maxTimeNextReward = 0;
        public static string badgePass = "";

        public static void loadConfigs()
        {
            enabled = Convert.ToBoolean(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.status_enabled"));
            update = Convert.ToByte(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.update_in_seconds"));
            roomId = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.room_id"));
            baseItemID = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.item_base_id"));
            minTimeNextReward = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.min_time.next_reward"));
            maxTimeNextReward = Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.max_time.next_reward"));
            badgePass = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.badge_pass_code");
        }

        public static void check()
        {
            if ((baseItemID <= 0 || roomId <= 0 || update <= 0 || minTimeNextReward <= 0 || maxTimeNextReward <= 0) && enabled)
                enabled = false;
        }

        public static void load()
        {
            if (usersToRemove.Count > 0)
                removeUsers();

            if (users.Count == 0)
                return;

            try
            {
                Random rnd = new Random();

                foreach (int id in users.ToList())
                {
                    RoomUser user = null;

                    GameClient client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(id);
                    if (client?.GetHabbo()?.CurrentRoom == null || !client.GetHabbo().collecting || client.GetHabbo().CurrentRoom.Id != roomId)
                    {
                        if (client?.GetHabbo() != null)
                        {
                            client.GetHabbo().nextReward = 0;
                            client.GetHabbo().timeWaitReward = 0;
                            client.GetHabbo().nextMovementCollector = 0;

                            user = client.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(id);
                            if (user != null)
                                user.ApplyEffect(0);
                        }

                        usersToRemove.Add(id);
                        continue;
                    }

                    double timestamp = AkiledEnvironment.GetUnixTimestamp();

                    if (client.GetHabbo().nextMovementCollector + update > timestamp)
                        continue;

                    Room room = client.GetHabbo().CurrentRoom;

                    List<Item> validItems = new List<Item>();

                    foreach (var item in room.GetRoomItemHandler().GetFloor.ToList())
                    {
                        if (item.BaseItem == baseItemID)
                            validItems.Add(item);
                    }

                    if (validItems.Count > 0)
                    {
                        user = room.GetRoomUserManager().GetRoomUserByHabboId(id);
                        if (user == null)
                        {
                            usersToRemove.Add(id);
                            continue;
                        }

                        if (client.GetHabbo().nextReward + client.GetHabbo().timeWaitReward < timestamp)
                        {
                            int time = rnd.Next(((maxTimeNextReward * 60) - (minTimeNextReward * 60)) + 1) + minTimeNextReward * 60;

                            client.GetHabbo().nextReward = timestamp;
                            client.GetHabbo().timeWaitReward = time;

                            giveRewards(client);
                        }

                        Item item = validItems[new Random().Next(validItems.Count)];
                        if (item != null)
                            user.MoveTo(item.GetX, item.GetY);
                    }

                    client.GetHabbo().nextMovementCollector = timestamp;
                }
            }
            catch (Exception ex)
            {
                Logging.LogException(ex.ToString());
            }
        }

        private static void giveRewards(GameClient client)
        {
            if (client == null)
                return;

            try
            {
                Random random = new Random();

                string keyPercentage = "";
                int level = 1;
                int minPerc = 0;
                int maxPerc = 0;

                StringBuilder message2 = new StringBuilder();

                int percentage = random.Next(100);

                keyPercentage = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.level_6.percentage");
                if (!string.IsNullOrEmpty(keyPercentage))
                {
                    if (keyPercentage.Split(',').Length == 1)
                    {
                        try
                        {
                            maxPerc = Convert.ToInt32(keyPercentage);
                        }
                        catch
                        {
                            maxPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        if (maxPerc > 0 && percentage >= maxPerc)
                            level = 6;
                    }
                    else
                    {
                        Logging.WriteLine("(Nivel) llave " + keyPercentage + " en Collector Park solo necesita un parámetro");
                    }
                }

                keyPercentage = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.level_5.percentages");
                if (!string.IsNullOrEmpty(keyPercentage))
                {
                    if (keyPercentage.Split(',').Length == 2)
                    {
                        try
                        {
                            minPerc = Convert.ToInt32(keyPercentage.Split(',')[0]);
                        }
                        catch
                        {
                            minPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        try
                        {
                            maxPerc = Convert.ToInt32(keyPercentage.Split(',')[1]);
                        }
                        catch
                        {
                            maxPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        if (minPerc > 0 && maxPerc > 0 && percentage >= minPerc && percentage < maxPerc)
                            level = 5;
                    }
                    else
                    {
                        Logging.WriteLine("(Nivel) LLave " + keyPercentage + " en Collector Park necesita dos parámetros divididos");
                    }
                }

                keyPercentage = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.level_4.percentages");
                if (!string.IsNullOrEmpty(keyPercentage))
                {
                    if (keyPercentage.Split(',').Length == 2)
                    {
                        try
                        {
                            minPerc = Convert.ToInt32(keyPercentage.Split(',')[0]);
                        }
                        catch
                        {
                            minPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        try
                        {
                            maxPerc = Convert.ToInt32(keyPercentage.Split(',')[1]);
                        }
                        catch
                        {
                            maxPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        if (minPerc > 0 && maxPerc > 0 && percentage >= minPerc && percentage < maxPerc)
                            level = 4;
                    }
                    else
                    {
                        Logging.WriteLine("(Nivel) Llave " + keyPercentage + " en Collector Park necesita dos parámetros divididos");
                    }
                }

                keyPercentage = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.level_3.percentages");
                if (!string.IsNullOrEmpty(keyPercentage))
                {
                    if (keyPercentage.Split(',').Length == 2)
                    {
                        try
                        {
                            minPerc = Convert.ToInt32(keyPercentage.Split(',')[0]);
                        }
                        catch
                        {
                            minPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        try
                        {
                            maxPerc = Convert.ToInt32(keyPercentage.Split(',')[1]);
                        }
                        catch
                        {
                            maxPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        if (minPerc > 0 && maxPerc > 0 && percentage >= minPerc && percentage < maxPerc)
                            level = 3;
                    }
                    else
                    {
                        Logging.WriteLine("(Nivel) LLave " + keyPercentage + " en Collector Park necesita dos parámetros divididos");
                    }
                }

                keyPercentage = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.level_2.percentages");
                if (!string.IsNullOrEmpty(keyPercentage))
                {
                    if (keyPercentage.Split(',').Length == 2)
                    {
                        try
                        {
                            minPerc = Convert.ToInt32(keyPercentage.Split(',')[0]);
                        }
                        catch
                        {
                            minPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        try
                        {
                            maxPerc = Convert.ToInt32(keyPercentage.Split(',')[1]);
                        }
                        catch
                        {
                            maxPerc = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de porcentajes de Collector Park {keyPercentage}");
                        }

                        if (minPerc > 0 && maxPerc > 0 && percentage >= minPerc && percentage < maxPerc)
                            level = 2;
                    }
                    else
                    {
                        Logging.WriteLine("(Level) Key " + keyPercentage + " in Collector Park need two params split with");
                    }
                }

                string key = "";
                int amount = 0;
                int checkMin = 0;
                int checkMax = 0;

                key = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.rewards.level_" + level + ".config.credits");
                if (!string.IsNullOrEmpty(key))
                {
                    if (key.Split(',').Length == 2)
                    {
                        try
                        {
                            checkMin = Convert.ToInt32(key.Split(',')[0]);
                        }
                        catch
                        {
                            checkMin = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        try
                        {
                            checkMax = Convert.ToInt32(key.Split(',')[1]);
                        }
                        catch
                        {
                            checkMax = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        if (checkMin > 0 && checkMax > 0)
                        {
                            amount = random.Next(checkMin, checkMax);
                            message2.Append(amount).Append(" credits, ");

                            client.GetHabbo().Credits += amount;
                            client.SendMessage(new CreditBalanceComposer(client.GetHabbo().Credits));
                            client.SendWhisper("Ganaste " + amount + " créditos en Collector Park!");
                        }
                    }
                    else
                    {
                        Logging.WriteLine("(Rewards) Key collectorpark.rewards.level_" + level + ".config.credits in Collector Park need two params split with");
                    }
                }

                key = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.rewards.level_" + level + ".config.duckets");
                if (!string.IsNullOrEmpty(key))
                {
                    if (key.Split(',').Length == 2)
                    {
                        try
                        {
                            checkMin = Convert.ToInt32(key.Split(',')[0]);
                        }
                        catch
                        {
                            checkMin = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        try
                        {
                            checkMax = Convert.ToInt32(key.Split(',')[1]);
                        }
                        catch
                        {
                            checkMax = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        if (checkMin > 0 && checkMax > 0)
                        {
                            amount = random.Next(checkMin, checkMax);
                            message2.Append(amount).Append(" duckets, ");

                            client.GetHabbo().Duckets += amount;
                            client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().Duckets, amount));
                            client.SendWhisper("Ganaste " + amount + " Esmeraldas en el Parque del Coleccionista!");
                        }
                    }
                    else
                    {
                        Logging.WriteLine("(Rewards) Key collectorpark.rewards.level_" + level + ".config.duckets in Collector Park need two params split with");
                    }
                }

                key = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.rewards.level_" + level + ".config.diamonds");
                if (!string.IsNullOrEmpty(key))
                {
                    if (key.Split(',').Length == 2)
                    {
                        try
                        {
                            checkMin = Convert.ToInt32(key.Split(',')[0]);
                        }
                        catch
                        {
                            checkMin = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        try
                        {
                            checkMax = Convert.ToInt32(key.Split(',')[1]);
                        }
                        catch
                        {
                            checkMax = 0;
                            Logging.WriteLine($"Excepción de formato de número a la clave de recompensas de Collector Park {key}");
                        }

                        if (checkMin > 0 && checkMax > 0)
                        {
                            amount = random.Next(checkMin, checkMax);
                            message2.Append(amount).Append(" diamonds, ");

                            client.GetHabbo().AkiledPoints += amount;
                            client.SendMessage(new HabboActivityPointNotificationComposer(client.GetHabbo().AkiledPoints, amount, 105));
                            client.SendWhisper("You won " + amount + " diamonds in Collector Park!");
                        }
                    }
                    else
                    {
                        Logging.WriteLine("(Rewards) Key collectorpark.rewards.level_" + level + ".config.diamonds in Collector Park need two params split with");
                    }
                }

                key = AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.rewards.level_" + level + ".config.items");
                if (!string.IsNullOrEmpty(key) && !key.Equals("0"))
                {
                    bool randomItems = Convert.ToBoolean(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.rewards.level_" + level + ".config.is_random_items"));

                    giveItems(client, key.Split(','), randomItems);
                    message2.Append(amount).Append(" furnis");
                }
            }
            catch (Exception ex)
            {
                Logging.LogException(ex.ToString());
            }
        }

        private static void giveItems(GameClient client, string[] items, bool randomItems)
        {
            int itemId = 0;
            Item item = null;

            if (!randomItems)
            {
                foreach (string id in items)
                {
                    try
                    {
                        itemId = Convert.ToInt32(id);
                    }
                    catch
                    {
                        Logging.WriteLine($"Excepción de formato de número en el ID de artículo de Collect Park (dar artículo) {itemId}");
                        continue;
                    }

                    if (itemId <= 0)
                        continue;

                    if (AkiledEnvironment.GetGame().GetItemManager().GetItem(itemId, out ItemData itemData))
                    {
                        client.GetHabbo().GetInventoryComponent().AddNewItem(0, itemId, "", 0, 0);

                        client.GetHabbo().GetInventoryComponent().UpdateItems(true);
                    }
                    else
                        Logging.WriteLine("No se pudo cargar el artículo de Collector Park " + item + ",No se encontraron registros de furnis.");
                }
            }
            else
            {
                Random rnd = new Random();

                try
                {
                    itemId = Convert.ToInt32(items[rnd.Next(items.Length)]);
                }
                catch
                {
                    Logging.WriteLine($"Excepción de formato de número en el ID de artículo de Collect Park (dar artículo) {itemId}");
                    return;
                }

                if (AkiledEnvironment.GetGame().GetItemManager().GetItem(itemId, out ItemData itemData))
                {
                    client.GetHabbo().GetInventoryComponent().AddNewItem(0, itemId, "", 0, 0);

                    client.GetHabbo().GetInventoryComponent().UpdateItems(true);
                }
                else
                    Logging.WriteLine("No se pudo cargar el artículo de Collector Park " + item + ",No se encontraron registros de furnis.");
            }
        }

        private static void removeUsers()
        {
            users.RemoveAll(id => usersToRemove.Contains(id));
        }
    }
}
