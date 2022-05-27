using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class spull : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            Room room = Session.GetHabbo().CurrentRoom;            if (room == null)                return;

            RoomUser roomuser = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);            if (roomuser == null)                return;
            if (Params.Length != 2)                return;            RoomUser TargetUser = room.GetRoomUserManager().GetRoomUserByHabbo(Params[1]);

            if (Params.Length == 1)
            {
                TargetUser.SendWhisperChat("Introduce el nombre del usuario que deseas hacer el super pull.");
                return;
            }

            if (!Room.RoomData.SPullEnabled && !Room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Oops, al parecer el dueño de la sala ha prohibido hacer los super pull en su sala.");
                return;
            }

            if ((TargetUser.GetClient().GetHabbo().HasFuse("no_accept_use_custom_commands")))
            {
                Session.SendWhisper("No se puede jalar a este usuario.");
                return;
            }

            if (TargetUser == null)            {                TargetUser.SendWhisperChat(Convert.ToString(Params[1]) + " el usuario ya no se encuentra aquí.");                return;            }            if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)                return;            if (TargetUser.GetClient().GetHabbo().PremiumProtect && !Session.GetHabbo().HasFuse("fuse_mod"))            {                roomuser.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("premium.notallowed", Session.Langue));                return;            }            roomuser.OnChat("@red@ * Jalar Duro a " + Params[1] + "*,", 0, false);            if (roomuser.RotBody % 2 != 0)                roomuser.RotBody--;            if (roomuser.RotBody == 0)                TargetUser.MoveTo(roomuser.X, roomuser.Y - 1);            else if (roomuser.RotBody == 2)                TargetUser.MoveTo(roomuser.X + 1, roomuser.Y);            else if (roomuser.RotBody == 4)                TargetUser.MoveTo(roomuser.X, roomuser.Y + 1);            else if (roomuser.RotBody == 6)                TargetUser.MoveTo(roomuser.X - 1, roomuser.Y);        }    }}