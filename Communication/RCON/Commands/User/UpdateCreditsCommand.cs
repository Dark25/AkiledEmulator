using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Data;

namespace Akiled.Communication.RCON.Commands.User
{
    class UpdateCreditsCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            if (parameters.Length != 2)
                return false;
            int Userid = 0;
            if (!int.TryParse(parameters[1], out Userid))
                return false;
            if (Userid == 0)
                return false;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Userid);
            if (Client == null)
                return false;

            DataRow row;
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("SELECT credits FROM users WHERE id = @userid");
                queryreactor.AddParameter("userid", Client.GetHabbo().Id);
                row = queryreactor.GetRow();
            }
            Client.GetHabbo().Credits = (int)row["credits"];
            Client.GetHabbo().UpdateCreditsBalance();

            return true;
        }
    }
}
