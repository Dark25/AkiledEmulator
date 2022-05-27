using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class FacebookCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Porfavor introduce el mensaje para enviarlo..");
                return;
            }
            string facebook_alert = (AkiledEnvironment.GetConfig().data["facebook_alert"]);
            string URL = (AkiledEnvironment.GetConfig().data["facebook_url"]);

            string Message = CommandManager.MergeParams(Params, 1);

            string AlertMessage = Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("<i>Nuevo Concurso en Facebook!</i>" +
            "\r\r" +
            "Hay un nuevo concurso en facebook realizado por <b>" + Session.GetHabbo().Username + ".</b>" +
            "\r\r" +
            "<b>¿De qué se trata?</b><br>" + Message + "<br><br><font><font color=\"#DB0003\">Para acceder a la página de Facebook haz clic en Ir a Facebook.</font></font>"));

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(facebook_alert, "Comunicado Staff", AlertMessage, "Ir a Facebook", 0, URL), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertfacebook", string.Format("Alerta Facebook: {0}", AlertMessage));

            return;
        }
    }
}
