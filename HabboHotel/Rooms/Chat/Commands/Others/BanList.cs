using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Data;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class BanList : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            {
                string Output = "Usuarios Baneados:\r";
                Output += "-----------------------------------------------------------------------------\r\n";

                using (IQueryAdapter Adapter = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    Adapter.SetQuery("SELECT `value`, `reason` FROM users,bans WHERE users.username = bans.value"); // Grabs Usernames only.
                    Adapter.RunQuery();

                    DataTable Table = Adapter.GetTable();

                    if (Table != null)
                    {
                        foreach (DataRow Row in Table.Rows)
                        {
                            Output += Row["Value"].ToString() + " - " + Row["reason"].ToString() + "\n\n";
                        }
                    }

                    if (Table.Rows.Count == 0)
                    {

                        Output += "No hay usuarios baneados.";

                    }
                }
                Session.SendPacket(new MOTDNotificationMessageComposer(Output));
            }
        }

    }
}