using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class UnloadRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(int.Parse(Params[1]));
            if (room == null)
                return;
            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            foreach (RoomUser User in UsersToReturn)
            {
                if (User != null)
                {
                    User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
                }
            }
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("El usuario: " + Session.GetHabbo().Username + " ha dado unload a la sala " + room.RoomData.Name + ".");

        }
    }
}
