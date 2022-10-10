namespace Akiled.Communication.RCON.Commands.Hotel
{
    class ReloadpermissionsCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            AkiledEnvironment.GetGame().GetRoleManager().Init();
            return true;
        }

    }
}
