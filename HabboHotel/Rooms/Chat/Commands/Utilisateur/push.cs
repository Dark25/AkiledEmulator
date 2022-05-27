using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Rooms.Games;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class push : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;            RoomUser TargetRoomUser;            RoomUser TargetRoomUser1;            Room TargetRoom = Session.GetHabbo().CurrentRoom;            if (TargetRoom == null)                return;            if (!TargetRoom.PushPullAllowed)                return;            TargetRoomUser1 = TargetRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (TargetRoomUser1 == null)                return;            if (Params.Length != 2)                return;            TargetRoomUser = TargetRoom.GetRoomUserManager().GetRoomUserByHabbo(Convert.ToString(Params[1]));            if (TargetRoomUser == null)            {                TargetRoomUser1.SendWhisperChat(Convert.ToString(Params[1]) + " el usuario ya no se encuentra aquí.");                return;            }

            if (Params.Length == 1)
            {
                TargetRoomUser.SendWhisperChat("Introduce el nombre del usuario que deseas hacer el push.");
                return;
            }

            if ((TargetRoomUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper("No se puede empujar a este usuario.");
                return;
            }

            if (!Room.RoomData.PushEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, al parecer el dueño de la sala ha prohibido hacer los push en su sala.");
                return;
            }            if (TargetRoomUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)                return;            if (TargetRoomUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))            {                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));                return;            }

            //if ((TargetRoomUser.X == TargetRoomUser1.X - 1) || (TargetRoomUser.X == TargetRoomUser1.X + 1) || (TargetRoomUser.Y == TargetRoomUser1.Y - 1) || (TargetRoomUser.Y == TargetRoomUser1.Y + 1))
            if (!((Math.Abs((TargetRoomUser.X - TargetRoomUser1.X)) >= 2) || (Math.Abs((TargetRoomUser.Y - TargetRoomUser1.Y)) >= 2)))            {                if (TargetRoomUser1.RotBody == 4)                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1); }                if (TargetRoomUser1.RotBody == 0)                { TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1); }                if (TargetRoomUser1.RotBody == 6)                { TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y); }                if (TargetRoomUser1.RotBody == 2)                { TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y); }                if (TargetRoomUser1.RotBody == 3)                {                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);                }                if (TargetRoomUser1.RotBody == 1)                {                    TargetRoomUser.MoveTo(TargetRoomUser.X + 1, TargetRoomUser.Y);                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);                }                if (TargetRoomUser1.RotBody == 7)                {                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y - 1);                }                if (TargetRoomUser1.RotBody == 5)                {                    TargetRoomUser.MoveTo(TargetRoomUser.X - 1, TargetRoomUser.Y);                    TargetRoomUser.MoveTo(TargetRoomUser.X, TargetRoomUser.Y + 1);                }                TargetRoomUser1.OnChat("@red@ *Empujar a " + Params[1] + "*", 0, false);            }            else            {                TargetRoomUser1.SendWhisperChat(Params[1] + " esta muy lejos de ti.");            }        }    }}