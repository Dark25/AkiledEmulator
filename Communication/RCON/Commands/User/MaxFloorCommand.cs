using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Akiled.Communication.RCON.Commands.User
{
    class MaxFloorCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {

            int Userid = 0;

            if (!int.TryParse(parameters[0], out Userid))
                return false;

            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);

            if (Client == null)
                return false;
            

            Room Room = Client.GetHabbo().CurrentRoom;
            
            if (Room == null || !Room.CheckRights(Client, true))
                return false;
            
            string Map = "";
            string Line = "";

            int TailleFloor = 50;
            if (Client.GetHabbo().Rank > 1)
                TailleFloor = 75;

            for (int y = 0; y < ((Room.GetGameMap().Model.MapSizeY) > TailleFloor ? Room.GetGameMap().Model.MapSizeY : TailleFloor); y++)
            {
                Line = "";
                for (int x = 0; x < ((Room.GetGameMap().Model.MapSizeX) > TailleFloor ? Room.GetGameMap().Model.MapSizeX : TailleFloor); x++)
                {
                    if (x >= Room.GetGameMap().Model.MapSizeX || y >= Room.GetGameMap().Model.MapSizeY)
                    {
                        Line += "0";
                    }
                    else
                    {
                        if (Room.GetGameMap().Model.SqState[x, y] == SquareState.BLOCKED)
                        {
                            Line += "0";//x
                        }
                        else
                        {
                            Line += this.parseInvers(Room.GetGameMap().Model.SqFloorHeight[x, y]);
                        }
                    }
                }
                Map += Line + Convert.ToChar(13);
            }

            Map = Map.TrimEnd(Convert.ToChar(13));

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("REPLACE INTO room_models_customs VALUES (@id, @doorX, @doorY, @doorZ, @doorDir, @heightmap, @murheight)");
                dbClient.AddParameter("id", Room.Id);
                dbClient.AddParameter("doorX", Room.GetGameMap().Model.DoorX);
                dbClient.AddParameter("doorY", Room.GetGameMap().Model.DoorY);
                dbClient.AddParameter("doorZ", Room.GetGameMap().Model.DoorZ);
                dbClient.AddParameter("doorDir", Room.GetGameMap().Model.DoorOrientation);
                dbClient.AddParameter("heightmap", Map);
                dbClient.AddParameter("murheight", Room.GetGameMap().Model.MurHeight);
                dbClient.RunQuery();
                dbClient.RunQuery("UPDATE rooms SET model_name = 'model_custom' WHERE id = " + Room.Id + " LIMIT 1");
            }

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();

            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);


            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
            }
            return true;
        
    }

        private char parseInvers(double input)
        {
            switch (input)
            {
                case 0:
                    return '0';
                case 1:
                    return '1';
                case 2:
                    return '2';
                case 3:
                    return '3';
                case 4:
                    return '4';
                case 5:
                    return '5';
                case 6:
                    return '6';
                case 7:
                    return '7';
                case 8:
                    return '8';
                case 9:
                    return '9';
                case 10:
                    return 'a';
                case 11:
                    return 'b';
                case 12:
                    return 'c';
                case 13:
                    return 'd';
                case 14:
                    return 'e';
                case 15:
                    return 'f';
                case 16:
                    return 'g';
                case 17:
                    return 'h';
                case 18:
                    return 'i';
                case 19:
                    return 'j';
                case 20:
                    return 'k';
                case 21:
                    return 'l';
                case 22:
                    return 'm';
                case 23:
                    return 'n';
                case 24:
                    return 'o';
                case 25:
                    return 'p';
                case 26:
                    return 'q';
                case 27:
                    return 'r';
                case 28:
                    return 's';
                case 29:
                    return 't';
                case 30:
                    return 'u';
                case 31:
                    return 'v';
                case 32:
                    return 'w';
                default:
                    return 'x';
            }
        }
    }
}
