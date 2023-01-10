namespace Akiled.Communication.RCON.Commands.Hotel
{
    class UpdateNavigatorCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.GetGame().GetNavigator().Init();
            return true;
        }
    }
}
