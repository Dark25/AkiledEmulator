using Akiled.Utilities.DependencyInjection;

namespace Akiled.Communication.RCON.Commands

{
    [Transient]
    public interface IRCONCommand
    {
        bool TryExecute(string[] parameters);
    }
}
