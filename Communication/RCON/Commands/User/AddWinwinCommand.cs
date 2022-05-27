using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.RCON.Commands.User
{
    class AddWinwinCommand : IRCONCommand
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
            if (Client == null)
                return false;

            int Winwin = 0;
            if (!int.TryParse(parameters[2], out Winwin))
                return false;

            if (Winwin == 0)
                return false;

            Client.GetHabbo().AchievementPoints = Client.GetHabbo().AchievementPoints + Winwin;
            Client.SendPacket(new AchievementScoreComposer(Client.GetHabbo().AchievementPoints));

            return true;
        }
    }
}
