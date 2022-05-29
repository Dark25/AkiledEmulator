using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Oleada : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if ((double)Session.GetHabbo().last_oleada > AkiledEnvironment.GetUnixTimestamp() - 60.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 1 minuto, para volver a usar el comando", 1);
                return;
            }
            /*string AlertMessage = "¡Hay una nueva oleada de publicidad! Si quieres ganar <b>distintas recompensas</b> por participar acude a la sala de publicidad.<br>¿Quién ha abierto la oleada? <b> <font color=\"#2E9AFE\"> " + Session.GetHabbo().Username + "</font>.</b>" +

            "<br>¿De qué trata este evento?<br><font color='#084B8A'><b>Trata de seguir las instrucciones de los guías de la oleada para participar y así ganar tu premio!</b></font><br>¡Te esperamos!" +
            "\r\n";*/

            AkiledEnvironment.GetGame().GetClientManager().SendMessage(RoomNotificationComposer.SendBubble("hoteloleada","¡Hay una nueva oleada de públicidad! Para ganar distintas recompensas por participar, ve a la sala de públicidad. - Click Aquí.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
            Session.GetHabbo().last_oleada = AkiledEnvironment.GetIUnixTimestamp();
            /* AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("zpam", "Comunicado Staff", AlertMessage, "Ir a la Oleada !", Session.GetHabbo().CurrentRoomId, ""));


             AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertoleada", string.Format("Alerta de Oleada: {0}", AlertMessage));*/

        }
    }
}
