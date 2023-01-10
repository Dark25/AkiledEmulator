namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ShutdownCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.PreformShutDown(true);
            return true;
        }
    }
}
