namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ReloadCMDSCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.GetGame().GetChatManager().GetCommands().Init();
            return true;
        }

    }
}
