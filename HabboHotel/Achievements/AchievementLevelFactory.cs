using Akiled.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Achievements
{
    public class AchievementLevelFactory
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> achievements)
        {
            achievements = new Dictionary<string, Achievement>();
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM achievements");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    string Category = (string)dataRow["category"];
                    string GroupName = (string)dataRow["group_name"];

                    if (!GroupName.StartsWith("ACH_")) GroupName = "ACH_" + GroupName;

                    AchievementLevel Level = new AchievementLevel((int)dataRow["level"], (int)dataRow["reward_pixels"], (int)dataRow["reward_points"], (int)dataRow["progress_needed"]);
                    if (!achievements.ContainsKey(GroupName))
                    {
                        Achievement achievement = new Achievement(Id, GroupName, Category);
                        achievement.AddLevel(Level);
                        achievements.Add(GroupName, achievement);
                    }
                    else
                        achievements[GroupName].AddLevel(Level);
                }
            }
        }
    }
}
