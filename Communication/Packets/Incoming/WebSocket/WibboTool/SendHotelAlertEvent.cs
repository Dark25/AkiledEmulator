using Akiled.Communication.Packets.Outgoing.WebSocket;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.WebClients;
using System.Text;
using System.Text.RegularExpressions;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class SendHotelAlertEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            if (Session == null)
                return;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            if (!Client.GetHabbo().HasFuse("fuse_alertpvptool"))
                return;

            bool EventAlert = Packet.PopBoolean();
            string Message = Packet.PopString();
            string Url = Packet.PopString();
            string Facebook = (AkiledEnvironment.GetConfig().data["facebook_url"]);
            string Hotel_url = (AkiledEnvironment.GetConfig().data["hotel_url"]);
            string instagram_url = (AkiledEnvironment.GetConfig().data["instagram_url"]);


            if (string.IsNullOrWhiteSpace(Message) || Message.Length > 1000 || Message.Length < 20)
                return;

            Message = Message.Replace("<", "&lt;").Replace(">", "&gt;");

            Message = new Regex(@"\[b\](.*?)\[\/b\]").Replace(Message, "<b>$1</b>");
            Message = new Regex(@"\[i\](.*?)\[\/i\]").Replace(Message, "<i>$1</i>");
            Message = new Regex(@"\[u\](.*?)\[\/u\]").Replace(Message, "<u>$1</u>");

            if (!string.IsNullOrWhiteSpace(Url))
            {
                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "hal", string.Format("WbTool hal: {0} : {1}", Url, Message));

                if (!Url.StartsWith(Hotel_url) && !Url.StartsWith(instagram_url) && !Url.StartsWith(Facebook) && !Url.StartsWith("https://www.facebook.com/") && !Url.StartsWith(instagram_url))
                    return;

                AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("comunicadostaff", "Atención Comunicado Staff", Message, "Ir al Enlace !", 0, Url), Session.Langue);
                return;
            }

            if (EventAlert)
            {
                if (Client.GetHabbo().CurrentRoom == null)
                    return;

                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, Client.GetHabbo().CurrentRoom.Id, string.Empty, "eventha", string.Format("WbTool eventha: {0}", Message));
                if (Client.Antipub(Message, "<eventalert>"))
                    return;

                if (!AkiledEnvironment.GetGame().GetAnimationManager().AllowAnimation())
                    return;

                //AkiledEnvironment.GetGame().GetClientManager().SendSuperNotif("Message de_hotelalerts Staffs", AlertMessage, "game_promo_small", "event:navigator/goto/" + Client.GetHabbo().CurrentRoom.Id, "Je veux y accéder!", true, true);
                AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("alertajuego", "Notificacíon de Evento", Message, "Ir al Evento!", Client.GetHabbo().CurrentRoom.Id, ""), Session.Langue);

                Client.GetHabbo().CurrentRoom.CloseFullRoom = true;
            }
            else
            {
                AkiledEnvironment.GetGame().GetModerationManager().LogStaffEntry(Client.GetHabbo().Id, Client.GetHabbo().Username, 0, string.Empty, "ha", string.Format("AkiledTool HotelAlert: {0}", Message));
                if (Client.Antipub(Message, "<alert>"))
                    return;

                //ServerPacket message = new ServerPacket(ServerPacketHeader.BroadcastMessageAlertMessageComposer);
                //message.WriteString(AkiledEnvironment.GetLanguageManager().TryGetValue("hotelallert.notice", Client.Langue) + "\r\n" + Message);// + "\r\n- " + Client.GetHabbo().Username);
                //AkiledEnvironment.GetGame().GetClientManager().SendMessage(message);
                AkiledEnvironment.GetGame().GetClientWebManager().SendMessage(new NotifAlertComposer("alertastaff", "Mensaje del Equipo Staff", Message, "Entendido !", 0, ""), Session.Langue);
            }

        }
    }
}
