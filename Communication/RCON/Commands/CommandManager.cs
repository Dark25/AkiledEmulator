using Akiled.Communication.RCON.Commands.Hotel;
using Akiled.Communication.RCON.Commands.User;
using System;
using System.Collections.Generic;

namespace Akiled.Communication.RCON.Commands
{
    public class CommandManager
    {
        private readonly Dictionary<string, IRCONCommand> _commands;

        public CommandManager()
        {
            this._commands = new Dictionary<string, IRCONCommand>();

            this.RegisterUser();
            this.RegisterHotel();
        }

        public bool Parse(string data)
        {
            if (data.Length == 0 || string.IsNullOrEmpty(data))
                return false;

            string cmd = data.Split(Convert.ToChar(1))[0];

            IRCONCommand command = null;
            if(this._commands.TryGetValue(cmd.ToLower(), out command))
            {
                string[] parameters = null;
                if (data.Split(Convert.ToChar(1))[1] != null)
                {
                    parameters = data.Split(Convert.ToChar(1));
                }

                return command.TryExecute(parameters);
            }
            return false;
        }

        private void RegisterUser()
        {
            this.Register("addphoto", new AddPhotoCommand());
            this.Register("addwinwin", new AddWinwinCommand());
            this.Register("updatecredits", new UpdateCreditsCommand());
            this.Register("updatepoints", new UpdatePointsCommand());
            this.Register("signout", new SignoutCommand());
            this.Register("ha", new HaCommand());
            this.Register("eventha", new EventHaCommand());
            this.Register("useralert", new UserAlertCommand());
            this.Register("senduser", new SendUserCommand());
            this.Register("follow", new FollowCommand());
        }

        private void RegisterHotel()
        {
            this.Register("unload", new UnloadCommand());
            this.Register("updatenavigator", new UpdateNavigatorCommand());
            this.Register("shutdown", new ShutdownCommand());
        }

        public void Register(string commandText, IRCONCommand command)
        {
            this._commands.Add(commandText, command);
        }
    }
}
