using Akiled;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.RCON.Commands;
using Akiled.HabboHotel.GameClients;

namespace AkiledEmulator.Communication.RCON.Commands.User
{
    class GivebadgeCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {

            if (!int.TryParse(parameters[0], out int Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient TargetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);

            if (TargetClient == null || TargetClient.GetHabbo() == null)
                return false;

            if (TargetClient != null)
            {
                if (!TargetClient.GetHabbo().GetBadgeComponent().HasBadge(parameters[1]))
                {
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(parameters[1], 0, true);
                    string BadgeCode = parameters[1];
                    TargetClient.GetHabbo().GetBadgeComponent().GiveBadge(BadgeCode, 0, true);
                    TargetClient.SendPacket(new ReceiveBadgeComposer(BadgeCode));
                    TargetClient.SendNotification("Has recibido la placa: " + BadgeCode);


                }
                else
                    TargetClient.SendWhisper("¡Huy!Este usuario ya tiene la placa.(" + parameters[1] + ")!");
                return false;

            }
            else
            {
                TargetClient.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", TargetClient.Langue));
                return false;
            }
        }
    }
}
