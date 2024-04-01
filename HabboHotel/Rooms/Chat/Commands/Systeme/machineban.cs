using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class MachineBan : IChatCommand
    {
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
     
                AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(Session, "Robot", (double)788922000, AkiledEnvironment.GetLanguageManager().TryGetValue("MachineBan.ban", Session.Langue), false, false);
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
                    Raison = AkiledEnvironment.GetLanguageManager().TryGetValue("MachineBan.reason", Session.Langue);
                }
                string Username = Habbo.Username;

                AkiledEnvironment.GetGame().GetClientManager().BanUserAsync(clientByUsername, Session.GetHabbo().Username, (double)788922000, Raison, true, true);
                Session.Antipub(Raison, "<CMD>");
                AkiledEnvironment.GetGame().GetClientManager().StaffAlert(RoomNotificationComposer.SendBubble("baneo", string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("MachineBan.alert", Session.Langue), Username)));
               
                Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("MachineBan.alert2", Session.Langue), Username, Raison));

            }

        }
    }
}
