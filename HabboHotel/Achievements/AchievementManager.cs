using Akiled.Communication.Packets.Incoming;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Achievements
{
    public class AchievementManager
    {
        public Dictionary<string, Achievement> Achievements;

        public AchievementManager()
        {
            this.Achievements = new Dictionary<string, Achievement>();
            LoadAchievements();
        }

        public void LoadAchievements()
        {
            AchievementLevelFactory.GetAchievementLevels(out Achievements);
        }

        public void GetList(GameClient Session, ClientPacket Message)
        {
            Session.SendPacket(new AchievementsMessageComposer(Session, Achievements.Values.ToList()));
        }

        public bool ProgressAchievement(GameClient Session, string AchievementGroup, int ProgressAmount)
        {
            if (!Achievements.ContainsKey(AchievementGroup))
            {
                return false;
            }

            Achievement AchievementData = null;

            AchievementData = Achievements[AchievementGroup];

            UserAchievement UserData = Session.GetHabbo().GetAchievementData(AchievementGroup);

            if (UserData == null)
            {
                UserData = new UserAchievement(AchievementGroup, 0, 0);
                Session.GetHabbo().Achievements.Add(AchievementGroup, UserData);
            }

            int TotalLevels = AchievementData.Levels.Count;

            if (UserData != null && UserData.Level == TotalLevels)
            {
                return false;
            }

            int TargetLevel = (UserData != null ? UserData.Level + 1 : 1);

            if (TargetLevel > TotalLevels)
            {
                TargetLevel = TotalLevels;
            }

            AchievementLevel TargetLevelData = AchievementData.Levels[TargetLevel];

            int NewProgress = (UserData != null ? UserData.Progress + ProgressAmount : ProgressAmount);
            int NewLevel = (UserData != null ? UserData.Level : 0);
            int NewTarget = NewLevel + 1;

            if (NewTarget > TotalLevels)
            {
                NewTarget = TotalLevels;
            }

            if (NewProgress >= TargetLevelData.Requirement)
            {
                NewLevel++;
                NewTarget++;

                int ProgressRemainder = NewProgress - TargetLevelData.Requirement;
                NewProgress = 0;

                Session.GetHabbo().GetBadgeComponent().GiveBadge(AchievementGroup + TargetLevel, 0, true);
                Session.SendPacket(new ReceiveBadgeComposer(AchievementGroup + TargetLevel));

                if (NewTarget > TotalLevels)
                {
                    NewTarget = TotalLevels;
                }

                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().UpdateActivityPointsBalance();

                Session.SendPacket(new AchievementUnlockedMessageComposer(AchievementData, TargetLevel, TargetLevelData.RewardPoints,
                    TargetLevelData.RewardPixels));

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO user_achievement VALUES (" + Session.GetHabbo().Id + ", @group, " + NewLevel + ", " + NewProgress + ")");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                    dbClient.RunQuery("UPDATE user_stats SET AchievementScore = AchievementScore + '" + TargetLevelData.RewardPoints + "' WHERE id = '" + Session.GetHabbo().Id + "';");
                }


                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;

                Session.GetHabbo().AchievementPoints += TargetLevelData.RewardPoints;
                Session.GetHabbo().Duckets += TargetLevelData.RewardPixels;
                Session.GetHabbo().UpdateActivityPointsBalance();
                Session.SendPacket(new AchievementScoreComposer(Session.GetHabbo().AchievementPoints));


                if (Session.GetHabbo().CurrentRoom != null)
                {
                    RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                    if (roomUserByHabbo != null)
                    {
                        Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));
                        Session.GetHabbo().CurrentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
                    }
                }


                AchievementLevel NewLevelData = AchievementData.Levels[NewTarget];
                Session.SendPacket(new AchievementProgressedComposer(AchievementData, NewTarget, NewLevelData, TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));

                return true;
            }
            else
            {
                UserData.Level = NewLevel;
                UserData.Progress = NewProgress;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.SetQuery("REPLACE INTO user_achievement VALUES (" + Session.GetHabbo().Id + ", @group, " + NewLevel + ", " + NewProgress + ")");
                    dbClient.AddParameter("group", AchievementGroup);
                    dbClient.RunQuery();
                }

                Session.SendPacket(new AchievementProgressedComposer(AchievementData, TargetLevel, TargetLevelData,
                TotalLevels, Session.GetHabbo().GetAchievementData(AchievementGroup)));
            }

            return false;
        }

        public Achievement GetAchievement(string AchievementGroup)
        {
            if (Achievements.ContainsKey(AchievementGroup))
            {
                return Achievements[AchievementGroup];
            }

            return null;
        }
    }
}
