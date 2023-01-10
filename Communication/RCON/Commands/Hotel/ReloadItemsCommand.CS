namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ReloadItemsCommand : IRCONCommand
    {

        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.GetGame().GetItemManager().Init();

            return true;
        }
    }
}