using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class GiveBadge : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 3)
            {
                Session.SendWhisper(AkiledEnvironment.GetLanguageManager().TryGetValue("GiveBadge.1", Session.Langue));
                return;
            }


            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(Params[2]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(Params[2], 0, true);
                    string BadgeCode = Params[2];
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, 0, true);
                    TargetClient.SendPacket(new ReceiveBadgeComposer(BadgeCode));
                    if (TargetClient.GetHabbo().Id != Session.GetHabbo().Id)
                    {
                        TargetClient.SendMessage(new NewYearComposer(Params[2]));

                        Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("GiveBadge.2", Session.Langue), Params[2], Params[1]));
                    }
                    else
                    {
                        Session.SendMessage(new NewYearComposer(Params[2]));
                        Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("GiveBadge.2", Session.Langue), Params[2], Params[1]));
                    }
                }
                else
                    Session.SendWhisper(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("GiveBadge.3", Session.Langue), Params[2]));

                return;
            }
            else
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
                return;

            }
        }
    }
}
