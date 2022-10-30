using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SMSCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            string Message2 = CommandManager.MergeParams(Params, 2);
            int lng = Message2.Length;
            int price = 5;
            bool hasFlat = Session.GetHabbo().GetBadgeComponent().HasBadge("ALLNET");
            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null)
            {
                Session.SendWhisper("No se ha encontrado al usuari@.", 1);
                return;
            }
            if (clientByUsername.GetHabbo().Id == Session.GetHabbo().Id)
            {
                Session.SendWhisper("No puedes enviarte mensajes tu mismo.", 1);
                return;
            }
            if ((double)Session.GetHabbo().last_sms > AkiledEnvironment.GetUnixTimestamp() - 10.0)
            {
                Session.SendWhisper("Debes esperar 10 segundos para volver enviar otro mensaje.", 1);
                return;
            }
            if (Session.GetHabbo().AchievementPoints < 1000)
            {
                Session.SendNotification("<font color = '#B40404'><font ><b>Uyy Espera!</b></font></font>\n\nComo eres nuevo en Habbo, no puedes enviar SMS. Necesitas 2 horas en línea y 100 puntos de recompensa.\r");
                return;
            }
            if (!hasFlat && lng * price > Session.GetHabbo().Duckets)
            {
                Session.SendWhisper("No tienes suficientes Díamantes!", 1);
                return;
            }
            if (Session.GetHabbo().AccountCreated > AkiledEnvironment.GetUnixTimestamp() - 259200.0)
            {
                Session.SendWhisper("Debe estar registrado durante al menos 3 días para poder enviar un SMS.", 1);
                return;
            }
            if (Session.GetHabbo().AchievementPoints < 1000)
            {
                Session.SendWhisper("Necesita al menos 1000 puntos de recompensa para enviar un SMS!", 1);
                return;
            }
            /*if (!Session.GetHabbo().GetPermissions().HasRight("word_filter_override"))
            {
                Message2 = AWSFilter.AWSCHAT(Session, Message2, "Chat");
                Message2 = AWSFilter.Beleidigung(Session, Message2, "Chat");
            }*/
            if (!hasFlat)
            {
                Session.SendWhisper("¡Has enviado con éxito el SMS! Costos: " + price * lng + " Díamantes", 1);
                Session.GetHabbo().Duckets -= price * lng;
                Session.SendMessage(new HabboActivityPointNotificationComposer(Session.GetHabbo().Duckets, 0));
            }
            else
            {
                Session.SendWhisper("Has enviado con éxito el SMS!", 1);
            }
            clientByUsername.SendWhisper(("SMS desde: " + Session.GetHabbo().Username + ": " + Message2) ?? "", 1);
            clientByUsername.SendMessage(RoomNotificationComposer.SendBubble("text_message", ("SMS de " + Session.GetHabbo().Username + ":\r\r" + Message2) ?? ""));
            Session.GetHabbo().last_sms = AkiledEnvironment.GetIUnixTimestamp();
        }
    }
}