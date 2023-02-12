using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.RCON.Commands.User
{
    internal class SetzStopCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            Console.WriteLine("SetzCommand: " + parameters[0]);

            if (!int.TryParse(parameters[0], out int Userid))
                return false;

            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);

            if (Client == null)
                return false;

            Room Room = Client.GetHabbo().CurrentRoom;

            if (Room == null || !Room.CheckRights(Client, true))
                return false;
            RoomUser UserRoom = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);


            UserRoom.ConstruitZMode = false;
            Client.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());
            UserRoom.SendWhisperChat("SetZ : Detenido");

            return true;

        }
    }
}