using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.RCON.Commands.User
{
    internal class SetRotateCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            int Userid = 0;


            if (!int.TryParse(parameters[0], out Userid))
                return false;

            if (Userid == 0)
                return false;

            GameClient Session = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);

            if (Session == null)
                return false;

            Room Room = Session.GetHabbo().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session, true))
                return false;

            RoomUser user = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (user == null)
                return false;

            if (parameters.Length == 1)
            {
                Session.SendWhisper("Escribe el valor.", 34);
                return false;
            }
            else if (parameters[1].Equals("clear", StringComparison.Ordinal) || parameters[1].Equals("limpiar", StringComparison.Ordinal))
            {
                user.setRotate = -1;
                Session.SendWhisper("Comando deshabilitado.", 34);
                return false;
            }

            int rot = 0;
            if (!int.TryParse(parameters[1], out rot))
                return false;

            if (rot < 0 || rot > 8)
            {
                Session.SendWhisper("Entre 1 y 8.", 34);
                return false;
            }

            user.setRotate = rot;
            Session.SendWhisper("Valor cambiado a: " + rot, 34);
            return true;

        }
    }
}