using Akiled.HabboHotel.Achievements;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AchievementProgressedComposer : ServerPacket
    {
        public AchievementProgressedComposer(Achievement Achievement, int TargetLevel, AchievementLevel TargetLevelData, int TotalLevels, UserAchievement UserData)
            : base(ServerPacketHeader.AchievementProgressedMessageComposer)
        {
            WriteInteger(Achievement.Id);
            WriteInteger(TargetLevel);
            WriteString(Achievement.GroupName + TargetLevel);
            WriteInteger(0);
            WriteInteger(TargetLevelData.Requirement);
            WriteInteger(TargetLevelData.RewardPixels);
            WriteInteger(0);
            WriteInteger(UserData != null ? UserData.Progress : 0);
            WriteBoolean(UserData != null && UserData.Level >= TotalLevels);
            WriteString(Achievement.Category);
            WriteString(string.Empty);
            WriteInteger(TotalLevels);
            WriteInteger(0);
        }
    }
}