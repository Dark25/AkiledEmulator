using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Staffons : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Dictionary<Habbo, UInt32> clients = new Dictionary<Habbo, UInt32>();

            StringBuilder content = new StringBuilder();
            content.Append("Estado de los Staff conectados en el hotel");

            foreach (var client in AkiledEnvironment.GetGame().GetClientManager()._clients.Values)
            {
                if (client != null && client.GetHabbo() != null && client.GetHabbo().Rank >= 3)
                    clients.Add(client.GetHabbo(), (Convert.ToUInt16(client.GetHabbo().Rank)));
            }

            foreach (KeyValuePair<Habbo, UInt32> client in clients.OrderBy(key => key.Value))
            {
                if (client.Key == null)
                    continue;

                content.Append("\r\n <font size ='8' color='#B40404'> ¥ " + client.Key.Username + " [Rango: " + client.Key.Rank + "] - Sala: " + ((client.Key.CurrentRoom == null) ? "En ninguna sala." : client.Key.CurrentRoom.RoomData.Name) + "</font>\r\n");
            }

            Session.SendNotification(content.ToString());


            return;
        }
    }
}