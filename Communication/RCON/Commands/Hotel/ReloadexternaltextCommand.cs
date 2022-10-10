namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ReloadexternaltextCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.GetLanguageManager().InitLocalValues();
            return true;
        }

    }
}
