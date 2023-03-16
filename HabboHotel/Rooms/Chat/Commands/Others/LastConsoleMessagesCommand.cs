
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;
using System.Data;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LastConsoleMessagesCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Introduce el nombre del usuario que deseas ver revisar su información de la consola del usuario.");
                return;
            }

            DataRow UserData = null;
            string Username = Params[1];

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `username` FROM users WHERE `username` = @Username LIMIT 1");
                dbClient.AddParameter("Username", Username);
                UserData = dbClient.GetRow();
            }

            if (UserData == null)
            {
                Session.SendMessage(new RoomCustomizedAlertComposer("No existe ningún usuario con el nombre " + Username + "."));
                return;
            }

            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Username);

            StringBuilder HabboInfo = new StringBuilder();

            HabboInfo.Append("Estos son los últimos comandos usados por el usuario, recuerda revisar siempre estos casos antes de proceder a banear a menos que sea un  caso evidente de robo o abuso.\n\n");

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT `message` FROM `chatlogs_console` WHERE `from_id` = '" + TargetClient.GetHabbo().Id + "' ORDER BY `id` DESC LIMIT 15");
                DataTable GetLogs = dbClient.GetTable();

                if (GetLogs == null)
                {
                    Session.SendMessage(new RoomCustomizedAlertComposer("Lamentablemente el usuario que has solicitado no tiene mensajes en el registro."));
                }

                else if (GetLogs != null)
                {
                    int Number = 11;
                    foreach (DataRow Log in GetLogs.Rows)
                    {
                        Number -= 1;
                        HabboInfo.Append("<font size ='8' color='#B40404'><b>[" + Number + "]</b></font>" + " <font size ='8' color='#B40404'> <b> " + Convert.ToString(Log["message"]) + "</b></font>\r");
                    }
                }

                Session.SendMessage(new RoomNotificationComposer("Últimos mensajes de " + Username + ":", (HabboInfo.ToString()), "fig/" + Session.GetHabbo().Look + "", "", ""));

            }
        }
    }
}



