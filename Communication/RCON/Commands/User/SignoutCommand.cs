using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.RCON.Commands.User
{
    class SignoutCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
                return false;

            if (!int.TryParse(parameters[1], out int Userid)) return false;

            if (Userid <= 0) return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null) return false;

            Client.Disconnect();
            return true;
        }
    }
}
