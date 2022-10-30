using System.Collections.Generic;

namespace Akiled.HabboHotel.Achievements
{
    public class Achievement
    {
        public readonly int Id;
        public readonly string GroupName;
        public readonly string Category;
        public readonly Dictionary<int, AchievementLevel> Levels;

        public Achievement(int Id, string GroupName, string Category)
        {
            this.Id = Id;
            this.GroupName = GroupName;
            this.Category = Category;
            this.Levels = new Dictionary<int, AchievementLevel>();
        }

        public void AddLevel(AchievementLevel Level)
        {
            this.Levels.Add(Level.Level, Level);
        }
    }
}
