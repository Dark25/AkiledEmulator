using Akiled.HabboHotel.GameClients;
using AkiledEmulator.HabboHotel.Hotel.CollectorPark;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class CollectorParkCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!CollectorParkConfigs.enabled)
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("collectorpark.disabled", Session.Langue), 1);
                return;
            }

            if (Room.Id != CollectorParkConfigs.roomId)
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("collectorpark.invalidroom", Session.Langue), 1);
                return;
            }

            if (!string.IsNullOrEmpty(CollectorParkConfigs.badgePass))
            {
                if (!Session.GetHabbo().GetBadgeComponent().HasBadge(CollectorParkConfigs.badgePass))
                {
                    Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("collectorpark.needbadge", Session.Langue), 1);
                    return;
                }
            }

            RoomUser user = null;

            if (CollectorParkConfigs.users.Contains(Session.GetHabbo().Id))
            {
                Session.GetHabbo().collecting = false;
                Session.GetHabbo().nextReward = 0;
                Session.GetHabbo().timeWaitReward = 0;
                Session.GetHabbo().nextMovementCollector = 0;

                CollectorParkConfigs.usersToRemove.Add(Session.GetHabbo().Id);

                user = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (user != null)
                    user.ApplyEffect(0);
                return;
            }

            Random rnd = new Random();
            int time = rnd.Next(((CollectorParkConfigs.maxTimeNextReward * 60) - (CollectorParkConfigs.minTimeNextReward * 60)) + 1) + CollectorParkConfigs.minTimeNextReward * 60;

            Session.GetHabbo().collecting = true;
            Session.GetHabbo().timeWaitReward = time;
            Session.GetHabbo().nextReward = AkiledEnvironment.GetUnixTimestamp();
            Session.GetHabbo().nextMovementCollector = AkiledEnvironment.GetUnixTimestamp();

            CollectorParkConfigs.users.Add(Session.GetHabbo().Id);

            user = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (user != null && Convert.ToInt32(AkiledEnvironment.GetSettingsManager().TryGetValue("collectorpark.define.min.enable_id")) > 0)
                user.ApplyEffect(0);

            Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("collectorpark.start", Session.Langue), 34);
        }
    }
}
