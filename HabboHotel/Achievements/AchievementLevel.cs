namespace Akiled.HabboHotel.Achievements
{
    public struct AchievementLevel
    {
        public readonly int Level;
        public readonly int RewardPixels;
        public readonly int RewardPoints;
        public readonly int Requirement;

        public AchievementLevel(int level, int rewardPixels, int rewardPoints, int requirement)
        {
            this.Level = level;
            this.RewardPixels = rewardPixels;
            this.RewardPoints = rewardPoints;
            this.Requirement = requirement;
        }
    }
}
