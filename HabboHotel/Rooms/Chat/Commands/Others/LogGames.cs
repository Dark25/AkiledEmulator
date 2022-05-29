using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;
using System.Data;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LogGames : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            DataTable GetLogs = null;
            StringBuilder HabboInfo = new StringBuilder();

            HabboInfo.Append("Estos son los últimos juegos abiertos las ultimas 24 horas.\n\n");

            using IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            dbClient.SetQuery("SELECT `user_name`,`timestamp`,`extra_data` FROM `cmdlogs` WHERE `command` = 'alertjuego' and from_unixtime(timestamp) >= now() - INTERVAL 1 DAY ORDER BY `id` DESC LIMIT 25");
            GetLogs = dbClient.GetTable();

            if (GetLogs == null)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("Lamentablemente no hay registro de juegos abiertos."));
            }

            else if (GetLogs != null)
            {
                int Number = 11;
                foreach (DataRow Log in GetLogs.Rows)
                {
                    Number -= 1;
                    HabboInfo.Append("<font size ='8' color='#B40404'><b>[" + Number + "]</b></font>" + " " + Convert.ToString(Log["user_name"]) + " <font size ='8' color='#B40404'> <b> " + Convert.ToString(Log["extra_data"]) + "</b></font>\r");
                }
            }

            Session.SendMessage(new RoomNotificationComposer("Últimos juegos abiertos", (HabboInfo.ToString()), "fig/" + Session.GetHabbo().Look + "", "", ""));
        }
    }
}


