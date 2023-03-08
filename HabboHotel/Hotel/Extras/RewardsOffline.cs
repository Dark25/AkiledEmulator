using Akiled;
using Akiled.Database.Interfaces;

namespace AkiledEmulator.HabboHotel.Hotel.Extras
{
    public class RewardsOffline
    {
        public static void Rewards(int userId, int credits, int duckets, int diamonds, string badge, int item)
        {
            if (userId <= 0)
                return;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `users` SET `credits`=credits + " + credits + ", `activity_points`=activity_points + " + duckets + ", `vip_points`=vip_points + " + diamonds + " WHERE `id`=" + userId + " LIMIT 1");

                if (!string.IsNullOrEmpty(badge))
                    dbClient.RunQuery("INSERT INTO `user_badges` (user_id, badge_id, badge_slot) VALUES (" + userId + ", '" + badge + "', 0)");
                if (item > 0)
                    dbClient.RunQuery("INSERT INTO `items` (user_id, room_id, base_item) VALUES (" + userId + ", 0, " + item + ")");
            }
        }
    }
}