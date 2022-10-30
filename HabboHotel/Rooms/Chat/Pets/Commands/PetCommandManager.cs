using Akiled.Database.Interfaces;
using System.Collections.Generic;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommandManager
    {
        private Dictionary<string, PetCommand> _petCommands;

        public PetCommandManager()
        {
            this._petCommands = new Dictionary<string, PetCommand>();

            this.Init();
        }

        public void Init()
        {
            this._petCommands.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM system_commands_pets");
                table = dbClient.GetTable();
            }
            if (table == null)
                return;

            foreach (DataRow dataRow in table.Rows)
            {
                int key = (int)dataRow["id"];
                string str1 = (string)dataRow["command"];

                this._petCommands.Add(str1, new PetCommand(key, str1));
            }
        }

        public int TryInvoke(string input)
        {
            PetCommand petCommand;
            if (this._petCommands.TryGetValue(input, out petCommand))
                return petCommand.commandID;
            else
                return 99;
        }
    }
}
