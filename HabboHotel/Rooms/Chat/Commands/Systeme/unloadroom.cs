using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class UnloadRoom : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(int.Parse(Params[1]));
            if (room == null)
                return;
            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("El usuario: " + Session.GetHabbo().Username + " ha dado unload a la sala " + room.RoomData.Name + ".");

        }
    }
}
