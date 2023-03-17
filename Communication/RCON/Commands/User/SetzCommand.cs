using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.RCON.Commands.User
{
    internal class SetzCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            int Userid = 0;

            Console.WriteLine("SetzCommand: " + parameters[0]);

            if (!int.TryParse(parameters[0], out  Userid))
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


            string Heigth = parameters[1];
            if (!double.TryParse(Heigth, out double Result))
                return false;
            if (Result < -100)
                Result = 0;
            if (Result > 100)
                Result = 100;
            

            UserRoom.ConstruitZMode = true;
            UserRoom.ConstruitHeigth = Result;

            UserRoom.SendWhisperChat("SetZ: " + Result);
            Console.WriteLine("SetZ: " + Result);
            if (Result >= 0)
                Client.SendPacket(Room.GetGameMap().Model.setHeightMap((Result > 63) ? 63 : Result));
            return true;
            
        }
    }
}