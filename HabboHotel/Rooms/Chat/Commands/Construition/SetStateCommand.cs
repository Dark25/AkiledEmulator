using Akiled.Core;
using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class SetStateCommand : IChatCommand
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
                user.setState = -1;
                Session.SendWhisper("Comando deshabilitado.", 34);
                return;
            }

            int state = 0;
            if (!int.TryParse(Params[1], out state))
                return;

            if (state < 0 || state > 100)
            {
                Session.SendWhisper("Entre 1 y 100.", 34);
                return;
            }

            user.setState = state;
            Session.SendWhisper("Valor cambiado a: " + state, 34);
        }
    }
}
