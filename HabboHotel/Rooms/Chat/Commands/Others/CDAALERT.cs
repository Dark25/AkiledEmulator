using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class CDAALERT : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            string AlertMessage = "<i>¡Central de Ayuda Abierta!</i>" + "\r\r" + "El usuario <b> <font><font color=\"#58ACFA\">" + Session.GetHabbo().Username + "</font></font></b> ha abierto la Central de Ayuda <b><br>Ven a resolver tus dudas sobre el hotel con nuestros <b>Guías</b> capacitados para responder casi todo tipo de preguntas, siempre y cuando no sean preguntas en bromas o chistes.<br><br><font><font color=\"#DB0003\">Si algún guía no puede resolver tu duda, acude con un supervisor para aclarar la respuesta.</font></font>";

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("cdaalert", "Comunicado Staff", AlertMessage, "Ir a la sala del CDA", Session.GetHabbo().CurrentRoomId, ""), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertcda", string.Format("Alerta CDA: {0}", AlertMessage));

            return;

        }
    }
}
