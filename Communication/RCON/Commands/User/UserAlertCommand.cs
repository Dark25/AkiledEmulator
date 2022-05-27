using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.RCON.Commands.User
{
    class UserAlertCommand : IRCONCommand
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

            Client.SendNotification(parameters[2]);
            return true;
        }
    }
}
