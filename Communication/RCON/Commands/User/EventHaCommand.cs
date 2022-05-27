using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using System.Text.RegularExpressions;

namespace Akiled.Communication.RCON.Commands.User
{
    class EventHaCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 3)
                return false;
            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null || Client.GetHabbo().CurrentRoom == null)
                return false;

            string Message = parameters[2];

            AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "eventha", string.Format("WbTool eventha: {0}", Message));
            if (Client.Antipub(Message, "<eventalert>", Client.GetHabbo().CurrentRoom.Id))
                return false;

            if (!AkiledEnvironment.GetGame().GetAnimationManager().AllowAnimation())
                return false;

            Message = Message.Replace("<", "&lt;").Replace(">", "&gt;");

            Message = new Regex(@"\[b\](.*?)\[\/b\]").Replace(Message, "<b>$1</b>");
            Message = new Regex(@"\[i\](.*?)\[\/i\]").Replace(Message, "<i>$1</i>");
            Message = new Regex(@"\[u\](.*?)\[\/u\]").Replace(Message, "<u>$1</u>");

            string AlertMessage = Message + "\r\n- " + Client.GetHabbo().Username;
            AkiledEnvironment.GetGame().GetClientManager().SendSuperNotif("Mensaje del Equipo Staff", AlertMessage, "game_promo_small", "event:navigator/goto/" + Client.GetHabbo().CurrentRoom.Id, "Ir al Evento!", true, true);
            AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("gameopen", "El usuario: " + Client.GetHabbo().Username + " - Acaba de abrir un juego/evento, por favor estar atento a su turno."));
            //AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("game_promo_small", "Message d'animation", AlertMessage, "Je veux y jouer !", Client.GetHabbo().CurrentRoom.Id));
            Client.GetHabbo().CurrentRoom.CloseFullRoom = true;

            return true;
        }
    }
}
