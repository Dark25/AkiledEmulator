using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Chat.Styles;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RestartCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            AkiledEnvironment.PreformRestart();


         }
     }
}
