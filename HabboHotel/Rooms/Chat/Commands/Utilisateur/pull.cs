using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class pull : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            if (!Room.PushPullAllowed)
                return;

            if (Params.Length != 2)
                return;

            RoomUser TargetUser = Room.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(Params[1]));
            if (TargetUser == null || TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                return;

            if (TargetUser == null)
            {
                TargetUser.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.notfound.room", Session.Langue), Convert.ToString(Params[1])));
                return;
            }

            if (Params.Length == 1)
            {
                TargetUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.notfound.room.1", Session.Langue));
                return;
            }

            if ((TargetUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                TargetUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.notfound.room.2", Session.Langue));
                return;
            }

            if (!Room.RoomData.PullEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("user.notfound.room.3", Session.Langue));
                return;
            }

            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            if (Math.Abs(UserRoom.X - TargetUser.X) < 3 && Math.Abs(UserRoom.Y - TargetUser.Y) < 3)
            {
                UserRoom.OnChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.pull.1", Session.Langue), Params[1]), 0, false);
                if (UserRoom.RotBody % 2 != 0)
                    UserRoom.RotBody--;

                if (UserRoom.RotBody == 0)
                    TargetUser.MoveTo(UserRoom.X, UserRoom.Y - 1);
                else if (UserRoom.RotBody == 2)
                    TargetUser.MoveTo(UserRoom.X + 1, UserRoom.Y);
                else if (UserRoom.RotBody == 4)
                    TargetUser.MoveTo(UserRoom.X, UserRoom.Y + 1);
                else if (UserRoom.RotBody == 6)
                    TargetUser.MoveTo(UserRoom.X - 1, UserRoom.Y);

            }
            else
            {
                UserRoom.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.pull.2", Session.Langue), Params[1]));
                return;
            }

        }
    }
}
