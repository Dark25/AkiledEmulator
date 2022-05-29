using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class hotelalert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor escribe el mensaje a enviar");
                return;
            }

            string Message = CommandManager.MergeParams(Params, 1);
            string hotelalert_alert = (AkiledEnvironment.GetConfig().data["hotelalert_alert"]);
            string AlertMessage = "<i>¡Mensaje Del Equipo Staff!</i>" + "\r\r" + "El usuario <b> <font><font color=\"#58ACFA\">" + Session.GetHabbo().Username + "</font></font></b> esta dando el siguiente anuncio. <br><br>" + Message + "</b> <br><br><font><font color=\"#DB0003\">Gracias por su atención.</font></font>";


            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(hotelalert_alert, "Comunicado Staff", AlertMessage, "Entendido !", 0, ""), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "staffalert", string.Format("Staff Alert: {0}", AlertMessage));

            return;



        }
    }
}
