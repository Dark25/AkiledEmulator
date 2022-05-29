    using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using System;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Fiestaalert : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {

            if ((double)Session.GetHabbo().last_dj > AkiledEnvironment.GetUnixTimestamp() - 60.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 60 segundos, para volver a usar el comando", 1);
                return;
            }

            string AlertMessage = "<i>¡Que Rumbiiitah Mijo!</i>" +
            "\r\r" +
            "El usuario <b> <font><font color=\"#58ACFA\">"
                 + Session.GetHabbo().Username + "</font></font></b> se ha vuelto loc@ y esta invitando a todo el hotel a la gran fiesta <b><br><br>Ven a difrutar del buen ambiente y a escuchar la música de nuestros mejores DJ, asi es ellos se encargaran animar el evento y darle un toque de locura.<b><br><br><font><font color=\"#DB0003\">¿Te lo vas a perder? ¡HABRAN PREMIOS RIFAS Y MUCHO MÁS!</font></font>";

            AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("partyalert", "Comunicado Staff", AlertMessage, "Ir a la Rumbita", Session.GetHabbo().CurrentRoomId, ""), Session.Langue);

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Session.GetHabbo().Id, Session.GetHabbo().Username, 0, string.Empty, "alertparty", string.Format("Alerta Fiesta: {0}", AlertMessage));
            Session.GetHabbo().last_dj = AkiledEnvironment.GetIUnixTimestamp();
            return;

        }
    }
}
