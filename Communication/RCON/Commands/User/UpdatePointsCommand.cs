using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.RCON.Commands.User
{
    class UpdatePointsCommand : IRCONCommand
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

            int NbWb = 0;
            if (!int.TryParse(parameters[2], out NbWb))
                return false;
            if (NbWb == 0)
                return false;

            Client.GetHabbo().AkiledPoints += NbWb;
            Client.GetHabbo().UpdateDiamondsBalance();

            return true;
        }
    }
}
