using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class InstagramAlert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            string instagram_alert = (AkiledEnvironment.GetConfig().data["instagram_alert"]);
            string URL = (AkiledEnvironment.GetConfig().data["instagram_url"]);

            string AlertMessage = "<i>Nuevo Concurso en Instagram!</i>" +
            "\r\r" +
            "Se esta realizando un nuevo concurso en instagram por, <b>" + Session.GetHabbo().Username + ".</b>" +
            "\r\r" +
            "Dirigite a nuestro Instagram Oficial, Concursa y Gana, tenemos sorpresas para tí<i> Puedes Obtener Rares, Placas, Créditos, " + name_monedaoficial + " y mucho más ! </i>" +
            "\r\n" +
            "\r\n- <b>Att: " + Session.GetHabbo().Username + ".</b>\r\n";

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(instagram_alert, "Comunicado Staff", AlertMessage, "Ir a Instagram", 0, URL), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertinstagram", string.Format("Alerta Instagram: {0}", AlertMessage));

            return;
        }
    }
}
