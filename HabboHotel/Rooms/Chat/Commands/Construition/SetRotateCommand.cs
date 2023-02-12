using Akiled.Core;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SetRotateCommand : IChatCommand
    {
        
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (!Room.CheckRights(Session, true))
                return;

            RoomUser user = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (user == null)
                return;

            if (Params.Length == 1)
            {
                Session.SendWhisper("Escribe el valor.", 34);
                return;
            }
            else if (Params[1].Equals("clear", StringComparison.Ordinal) || Params[1].Equals("limpiar", StringComparison.Ordinal))
            {
                user.setRotate = -1;
                Session.SendWhisper("Comando deshabilitado.", 34);
                return;
            }

            int rot = 0;
            if (!int.TryParse(Params[1], out rot))
                return;

            if (rot < 0 || rot > 8)
            {
                Session.SendWhisper("Entre 1 y 8.", 34);
                return;
            }

            user.setRotate = rot;
            Session.SendWhisper("Valor cambiado a: " + rot, 34);
        }
    }
}
