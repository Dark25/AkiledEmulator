using Akiled.Communication.RCON.Commands.Hotel;
using Akiled.Communication.RCON.Commands.User;
using AkiledEmulator.Communication.RCON.Commands.User;
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

               /// <summary>
        /// Request the text to parse and check for commands that need to be executed.
        /// </summary>
        /// <param name="data">A string of data split by char(1), the first part being the command and the second part being the parameters.</param>
        /// <returns>True if parsed or false if not.</returns>
        public bool Parse(string data)
        {
            if (data.Length == 0 || string.IsNullOrEmpty(data))
                return false;

            string cmd = data.Split(Convert.ToChar(1))[0];

			if (this._commands.TryGetValue(cmd.ToLower(), out IRCONCommand command))
			{
				string param = null;
				string[] parameters = null;
				if (data.Split(Convert.ToChar(1))[1] != null)
				{
					param = data.Split(Convert.ToChar(1))[1];
					parameters = param.ToString().Split(':');
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
            this.Register("autofloor", new AutoFloorCommand());
            this.Register("givebadge", new GivebadgeCommand());
            this.Register("maxfloor", new MaxFloorCommand());
            this.Register("setzon", new SetzCommand());
            this.Register("setzoff", new SetzStopCommand());
            this.Register("infofurni", new FurniInfoCommand());
            this.Register("setrotate", new SetRotateCommand());
            this.Register("setstate", new SetStateCommand());
            this.Register("givefurni", new GiveFurniCommnad());
            this.Register("give", new GiveUserCurrencyCommand());
        }

        private void RegisterHotel()
        {
            this.Register("unload", new UnloadCommand());
            this.Register("updatenavigator", new UpdateNavigatorCommand());
            this.Register("shutdown", new ShutdownCommand());
            this.Register("updatecata", new ReloadCatalogCommand());
            this.Register("updateitems", new ReloadItemsCommand());
            this.Register("updatecomandos", new ReloadCMDSCommand());
            this.Register("updateexternaltext", new ReloadexternaltextCommand());
            this.Register("updatepermissions", new ReloadpermissionsCommand());
        }

        public void Register(string commandText, IRCONCommand command) => this._commands.Add(commandText, command);
    }
}
