using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class neweha : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session != null)
            {
                if (Room != null)
                {
                    if ((double)Session.GetHabbo().last_eha > AkiledEnvironment.GetUnixTimestamp() - 100.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
                    {
                        Session.SendWhisper("Debes esperar 100 segundos, para volver a usar el comando", 1);
                        return;
                    }

                    if (Params.Length == 1)
                    {
                        Session.SendWhisper("Por favor, escriba el juego que esta abriendo.", 1);
                        return;
                    }
                    string event_alert = (AkiledEnvironment.GetConfig().data["event_alert"]);
                    string Message = CommandManager.MergeParams(Params, 1);

                    string AlertMessage = Encoding.GetEncoding("Windows-1252").GetString(Encoding.GetEncoding("UTF-8").GetBytes("¡Hay un nuevo evento en este momento! Si quieres ganar <b>Diamantes o Puntos VIP</b> participa ahora mismo.Se trata de un juego hecho por <b> <font color=\"#2E9AFE\"> " + Session.GetHabbo().Username + "</font>.</b>" +
                    "\r\r" +
                    "Si quieres participar haz click en el botón inferior de <b>Ir a la sala del evento</b>, y ahí dentro podrás participar.<br>¿De qué trata este evento?<br><font color='#FF0040'><b>"
                    + Message + "</b></font><br>¡Te esperamos!" +
                    "\r\n"));


                    AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(event_alert, "Comunicado Staff", AlertMessage, "Ir a la sala del evento", Session.GetHabbo().CurrentRoomId, ""), Session.Langue);

                    AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("gameopen", "El usuario: " + Session.GetHabbo().Username + " - Acaba de abrir un juego/evento, por favor estar atento a su turno.", "event:navigator/goto/" + Session.GetHabbo().CurrentRoomId));
                    AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertjuego", string.Format("Alerta Juego: {0}", Message));
                    Session.GetHabbo().last_eha = AkiledEnvironment.GetIUnixTimestamp();
                    return;

                }
            }
        }
    }
}

