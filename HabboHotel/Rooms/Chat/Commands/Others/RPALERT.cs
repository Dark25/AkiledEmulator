using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RPALERT : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if ((double)Session.GetHabbo().last_rp > AkiledEnvironment.GetUnixTimestamp() - 100.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 100 segundos, para volver a usar el comando", 1);
                return;
            }

            string roleplay_alert = (AkiledEnvironment.GetConfig().data["roleplay_alert"]);
            string AlertMessage = "<i>¡NUEVO EVENTO ROLEPLAY!</i>" +
            "\r\r" +
            "El usuario <b><font color=\"#58ACFA\">"
                 + Session.GetHabbo().Username + "</font></font></b> esta promocionando un nuevo evento, <b><br>Se trata de un juego tipo <b>RolePlay</b> en donde deberas destacar tus habilidades especiales para ganar el evento.<br><br>";

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer(roleplay_alert, "Comunicado Staff", AlertMessage, "Ir a la sala RP", Session.GetHabbo().CurrentRoomId, ""), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertrp", string.Format("Alerta RP: {0}", AlertMessage));
            Session.GetHabbo().last_rp = AkiledEnvironment.GetIUnixTimestamp();
            return;

        }
    }
}
