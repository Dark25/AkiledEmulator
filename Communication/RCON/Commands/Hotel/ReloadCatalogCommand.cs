using Akiled.Communication.Packets.Outgoing.Structure;

namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ReloadCatalogCommand : IRCONCommand
    {


        public bool TryExecute(string[] parameters)
        {
            using var dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            AkiledEnvironment.GetGame().GetItemManager().Init();
            AkiledEnvironment.GetGame().GetCatalog().Init(AkiledEnvironment.GetGame().GetItemManager());
            AkiledEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
            return true;
        }
    }
}