using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.RCON.Commands.Hotel
{
    class UnloadCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (!int.TryParse(parameters[0], out int RoomId))
                return false;
            if (RoomId == 0)
                return false;


            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
                return false;
            List<RoomUser> UsersToReturn = room.GetRoomUserManager().GetRoomUsers().ToList();
            foreach (RoomUser User in UsersToReturn)
            {
                if (User != null)
                {
                    User.GetClient().SendMessage(new RoomForwardComposer(RoomId));
                }
            }

            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(room);
            return true;
        }
    }
}
