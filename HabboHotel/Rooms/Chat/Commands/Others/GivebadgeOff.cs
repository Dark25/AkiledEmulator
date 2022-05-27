using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class GivebadgeOff : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername != null)
            {
                if (Session.Langue != clientByUsername.Langue)
                {
                    UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", clientByUsername.Langue), Session.Langue));
                    return;
                }

                if (Params.Length < 2)
                {
                    Session.SendWhisper("Escriba :givebadgeoff Nombre Placa");
                }

                string BadgeCode = Params[2];
                clientByUsername.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, true);
                clientByUsername.SendPacket(new ReceiveBadgeComposer(BadgeCode));


                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    Session.GetHabbo().guardar = "";
                    dbClient.SetQuery("SELECT id FROM users WHERE username ='" + Params[1] + "'");
                    DataTable GetPermissionRights = dbClient.GetTable();
                    foreach (DataRow linha in GetPermissionRights.Rows)
                    {
                        Session.GetHabbo().guardar2 = Convert.ToInt32(linha["id"]);
                    }
                    Session.GetHabbo().guardar += 1;
                    dbClient.SetQuery("INSERT INTO `user_badges`(`user_id`,`badge_id`)  VALUES('" + Session.GetHabbo().guardar2 + "','" + Params[2] + "'");
                    dbClient.AddParameter("points", Session.GetHabbo().guardar);
                    dbClient.RunQuery();

                }
                Session.SendWhisper("Placa Enviada con Exito.");


            }
            else
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));

        }
    }
}
