using Akiled.HabboHotel.Achievements;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AchievementsMessageComposer : ServerPacket
    {
        public AchievementsMessageComposer(GameClient Session, List<Achievement> Achievements)
            : base(ServerPacketHeader.AchievementsMessageComposer)
        {
            WriteInteger(Achievements.Count);
            foreach (Achievement achievement in Achievements)
            {
                UserAchievement achievementData = Session.GetHabbo().GetAchievementData(achievement.GroupName);
                int TargetLevel = achievementData != null ? achievementData.Level + 1 : 1;
                int count = achievement.Levels.Count;
                if (TargetLevel > count)
                    TargetLevel = count;
                AchievementLevel achievementLevel = achievement.Levels[TargetLevel];
                WriteInteger(achievement.Id);
                WriteInteger(TargetLevel);
                WriteString(achievement.GroupName + TargetLevel);
                WriteInteger(0);
                WriteInteger(achievementLevel.Requirement); //?
                WriteInteger(achievementLevel.RewardPixels);
                WriteInteger(0); //-1 = rien, 5 = PointWinwin?
                WriteInteger(achievementData != null ? achievementData.Progress : 0);
                WriteBoolean(achievementData != null && achievementData.Level >= count);
                WriteString(achievement.Category);
                WriteString(string.Empty);
                WriteInteger(count);
                WriteInteger(0);
            }
            WriteString(string.Empty);
        }
    }
}
