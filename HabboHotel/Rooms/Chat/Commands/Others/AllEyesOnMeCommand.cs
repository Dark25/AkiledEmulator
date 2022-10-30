﻿using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Pathfinding;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class AllEyesOnMeCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null)
                return;

            List<RoomUser> Users = Room.GetRoomUserManager().GetRoomUsers();
            foreach (RoomUser U in Users.ToList())
            {
                if (U == null || Session.GetHabbo().Id == U.UserId)
                    continue;

                U.SetRot(Rotation.Calculate(U.X, U.Y, User.X, User.Y), false);
            }
        }
    }
}
