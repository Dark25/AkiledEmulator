using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Fiestamensual : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message = CommandManager.MergeParams(Params, 1);

            string titulo = "";
            string descripcion = "";
            string textboton = "";
            string imagen = "";

            using (IQueryAdapter dbQuery = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbQuery.SetQuery("SELECT * FROM `event_month` LIMIT 1");
                DataTable gUsersTable = dbQuery.GetTable();

                foreach (DataRow Row in gUsersTable.Rows)
                {
                    titulo = Convert.ToString(Row["title"]);
                    descripcion = Convert.ToString(Row["desc"]);
                    textboton = Convert.ToString(Row["text_botton"]);
                    imagen = Convert.ToString(Row["image"]);
                }
            }

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(imagen, titulo, descripcion, textboton, Session.GetHabbo().CurrentRoomId, ""), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "fiestatemporada", string.Format("Fiesta Temporada: {0}", descripcion));
            Session.SendWhisper("Alerta De Fiesta o Evento de Temporada Enviada Correctamente.", 34);
            return;

        }
    }
}
