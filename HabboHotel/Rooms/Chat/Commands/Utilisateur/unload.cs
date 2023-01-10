using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class unload : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room == null || !Room.CheckRights(Session, true))
                return;
            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);
            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendMessage(new RoomForwardComposer(Room.Id));
            }
        }
    }
}
