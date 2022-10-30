using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Subastaalert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string subasta_alert = (AkiledEnvironment.GetConfig().data["subasta_alert"]);
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            string AlertMessage = "El usuario <b> <font><font color=\"#12adfb\">"
                 + Session.GetHabbo().Username + "</font></font></b> ha abierto la subasta de rares <b><br>Ven a pujar por los Rares unicos ofrecidos por los <b>Staff</b>, los cuales tienen un precio inicial en  " + name_monedaoficial + " y debes ofertar solo por la cantidad que tengas en tu monedero de " + name_monedaoficial + ".<br><br><font><font color=\"#DB0003\">Si usted no posee " + name_monedaoficial + " suficientes para la oferta o puja de Rares, ignore esta alerta y siga haciendo lo suyo, no se acepta ninguna clase de saboteo, de lo contrario seras penalizado.</font></font>" +
           "\r\n";

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(subasta_alert, "Comunicado Staff", AlertMessage, "Ir a la Subasta", Session.GetHabbo().CurrentRoomId, ""), Session.Langue);


            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertsubasta", string.Format("Alerta de Subasta: {0}", AlertMessage));

        }
    }
}
