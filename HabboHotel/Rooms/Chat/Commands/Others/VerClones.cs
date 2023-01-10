using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Data;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class VerClones : IChatCommand
    {
        public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                session.SendWhisper("Porfavor ingrese el nombre del usuario a revisar.");
                return;
            }

            string str2;
            IQueryAdapter adapter;
            string username = Params[1];
            DataTable table = null;
            StringBuilder builder = new StringBuilder();
            if (AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(username) != null)
            {
                str2 = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(username).GetConnection().getIp();
                builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                using (adapter = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.GetTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "Id : ", row["id"], " - Username: ", row["username"] }));
                    }
                }
                session.SendPacket(new MOTDNotificationMessageComposer(builder.ToString()));
            }
            else
            {
                using (adapter = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    adapter.SetQuery("SELECT ip_last FROM users WHERE username = @username");
                    adapter.AddParameter("username", username);
                    str2 = adapter.GetString();
                    builder.AppendLine("Username :  " + username + " - Ip : " + str2);
                    adapter.SetQuery("SELECT id,username FROM users WHERE ip_last = @ip OR ip_reg = @ip");
                    adapter.AddParameter("ip", str2);
                    table = adapter.GetTable();
                    builder.AppendLine("Clones encontrados: " + table.Rows.Count);
                    foreach (DataRow row in table.Rows)
                    {
                        builder.AppendLine(string.Concat(new object[] { "ID : ", row["id"], " - Usuari@: ", row["username"] }));
                    }
                }

                session.SendPacket(new MOTDNotificationMessageComposer(builder.ToString()));
            }
            return;
        }
    }
}
