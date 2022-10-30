using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;
using System.Text;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class userinfo2 : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2) return; string username = Params[1]; if (string.IsNullOrEmpty(username)) { Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.userparammissing", Session.Langue)); return; }
            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.useroffline", Session.Langue));
                return;
            }
            string name_monedaoficial = (AkiledEnvironment.GetConfig().data["name_monedaoficial"]);
            Habbo Habbo = clientByUsername.GetHabbo();
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("Nombre: " + Habbo.Username + "\r"); stringBuilder.Append("Id: " + Habbo.Id + "\r");
            stringBuilder.Append("Misión: " + Habbo.Motto + "\r");
            stringBuilder.Append("" + name_monedaoficial + ": " + Habbo.AkiledPoints + "\r");
            stringBuilder.Append("Créditos: " + Habbo.Credits + "\r");
            stringBuilder.Append("Win-Win: " + Habbo.AchievementPoints + "\r");
            stringBuilder.Append("Premium: " + ((Habbo.Rank > 1) ? "Yes" : "No") + "\r");
            stringBuilder.Append("Mazo Score: " + Habbo.MazoHighScore + "\r");
            stringBuilder.Append("Respetos: " + Habbo.Respect + "\r");
            stringBuilder.Append("Se encuentra en sala: " + ((Habbo.InRoom) ? "Yes" : "No") + "\r");

            if (Habbo.CurrentRoom != null && !Habbo.SpectatorMode)
            {
                stringBuilder.Append("\r - Información de la sala  [" + Habbo.CurrentRoom.Id + "] - \r");
                stringBuilder.Append("Dueño: " + Habbo.CurrentRoom.RoomData.OwnerName + "\r");
                stringBuilder.Append("Nombre: " + Habbo.CurrentRoom.RoomData.Name + "\r");
                stringBuilder.Append("Usuarios: " + Habbo.CurrentRoom.UserCount + "/" + Habbo.CurrentRoom.RoomData.UsersMax + "\r");
            }

            if (Session.GetHabbo().HasFuse("fuse_sysadmin"))
            {
                stringBuilder.Append("\r - Otra Info - \r");
                stringBuilder.Append("MachineId: " + clientByUsername.MachineId + "\r");
                stringBuilder.Append("IP Web: " + clientByUsername.GetHabbo().IP + "\r");
                stringBuilder.Append("IP Emu: " + clientByUsername.GetConnection().getIp() + "\r");
                stringBuilder.Append("Langue: " + clientByUsername.Langue.ToString() + "\r");


            }

            Session.SendNotification(stringBuilder.ToString());

        }
    }
}