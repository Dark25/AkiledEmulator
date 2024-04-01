using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class push : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            RoomUser TargetRoomUser;
            RoomUser TargetRoomUser1;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if (TargetRoom == null)
                return;
            if (!TargetRoom.PushPullAllowed)
                return;

            TargetRoomUser1 = TargetRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (TargetRoomUser1 == null)
                return;
            if (Params.Length != 2)
                return;

            TargetRoomUser = TargetRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(Params[1]));

            if (TargetRoomUser == null)
            {
                TargetRoomUser1.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.isfound.room", Session.Langue), Convert.ToString(Params[1])));
                return;
            }

            if (Params.Length == 1)
            {
                TargetRoomUser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("user.isfound.room.1", Session.Langue));  
                return;
            }

            if ((TargetRoomUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("user.isfound.room.2", Session.Langue));
                return;
            }

            if (!Room.RoomData.PushEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, al parecer el dueño de la sala ha prohibido hacer los push en su sala.");
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("user.isfound.room.3", Session.Langue));
                return;
            }

            if (TargetRoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                return;

            if (TargetRoomUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));
                return;
            }

            //if ((TargetRoomUser.X == TargetRoomUser1.X - 1) || (TargetRoomUser.X == TargetRoomUser1.X + 1) || (TargetRoomUser.Y == TargetRoomUser1.Y - 1) || (TargetRoomUser.Y == TargetRoomUser1.Y + 1))
            if (!((Math.Abs((TargetRoomUser.X - TargetRoomUser1.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - TargetRoomUser1.Y)) >= 2)))
            {
                if (TargetRoomUser1.RotBody == 4)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1); }

                if (TargetRoomUser1.RotBody == 0)
                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1); }

                if (TargetRoomUser1.RotBody == 6)
                { TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y); }

                if (TargetRoomUser1.RotBody == 2)
                { TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y); }

                if (TargetRoomUser1.RotBody == 3)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                if (TargetRoomUser1.RotBody == 1)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (TargetRoomUser1.RotBody == 7)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);
                }

                if (TargetRoomUser1.RotBody == 5)
                {
                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);
                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);
                }

                TargetRoomUser.OnChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.push.1", Session.Langue), Session.GetHabbo().Username), 0, false);
            }
            else
            {
                TargetRoomUser1.SendWhisperChat(Params[1] + " esta muy lejos de ti.");
                TargetRoomUser1.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("user.push.2", Session.Langue), Params[1]));
            }

        }
    }
}
