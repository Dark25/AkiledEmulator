using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Users;namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class MachineBan : IChatCommand    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 2)
                return;

            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            Habbo Habbo = AkiledEnvironment.GetHabboByUsername(Params[1]);
            if (clientByUsername == null)
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            else if (string.IsNullOrEmpty(clientByUsername.MachineId))
            {
                return;
            }
            else if (clientByUsername.GetHabbo().Rank >= Session.GetHabbo().Rank)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
                 AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(Session, "Robot", (double)788922000, "Su cuenta ha sido prohibida por seguridad", false, false);
            }
            else
            {
                
                string Raison = "";
                if (Params.Length > 2)
                {
                    for (int i = 2; i < Params.Length; i++)
                    {
                        Raison += Params[i] + " ";
                    }
                }
                else
                {
                    Raison = "No se ha especificado una razón.";
                }
                string Username = Habbo.Username;

                AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(clientByUsername, Session.GetHabbo().Username, (double)788922000, Raison, true, true);
                Session.Antipub(Raison, "<CMD>");
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("baneo", "El usuario: " + Username + " ha sido baneado, por favor verifiquen la razon del baneo, para evitar malos entendidos"));
                Session.SendWhisper("Excelente has baneado la ip del usuario '" + Username + "' por la razon: '" + Raison + "'!");
            }

        }
    }
}
