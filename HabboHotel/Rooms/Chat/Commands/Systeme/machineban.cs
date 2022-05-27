using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class MachineBan : IChatCommand    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
                return;

            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            if (clientByUsername == null)
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            else if (string.IsNullOrEmpty(clientByUsername.MachineId))
            {
                return;
            }
            else if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                AkiledEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", (double)788922000, "Su cuenta ha sido prohibida por seguridad", false, false);
            }
            else
            {
                string Raison = "";
                if (Params.Length > 2)
                    Raison = CommandManager.MergeParams(Params, 2);
                AkiledEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, (double)788922000, Raison, true, true);
                Session.Antipub(Raison, "<CMD>");
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("baneo", "El usuario: " + clientByUsername.GetHabbo().Username + " ha sido baneado, por favor verifiquen la razon del baneo, para evitar malos entendidos"));
                Session.SendWhisper("Excelente has baneado la ip del usuario '" + clientByUsername + "' por la razon: '" + Raison + "'!");
            }

            if (clientByUsername.GetHabbo().Rank > 8 && Session.GetHabbo().Rank < 12)
            {
                AkiledEnvironment.GetGame().GetClientManager().BanUser(Session, "Robot", (double)788922000, "Su cuenta ha sido prohibida por seguridad", false, false);
            }

        }
    }
}
