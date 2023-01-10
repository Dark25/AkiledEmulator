using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class MuteCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Por favor ingrese el nombre de usuari@ y el tiempo en segundos! (Max. 600 Segundos)", 1);
                return;
            }
            Habbo Habbo = AkiledEnvironment.GetHabboByUsername(Params[1]);
            double Time;
            if (Habbo == null)
            {
                Session.SendWhisper("No se encuentra el usuari@.", 1);
            }
            else if (!Session.GetHabbo().HasFuse("fuse_mod"))
            {
                Session.SendWhisper("No tiene el permiso necesario para usar este comando.", 1);
            }
            else if (double.TryParse(Params[2], out Time))
            {
                if (Time > 600.0 && !Session.GetHabbo().HasFuse("mod_mute_limit_override"))
                {
                    Time = 600.0;
                }
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `users` SET `time_muted` = '" + Time + "' WHERE `id` = '" + Habbo.Id + "' LIMIT 1");
                }
                if (Habbo.GetCliente() != null)
                {
                    Habbo.TimeMuted = Time;
                    Habbo.GetCliente().SendWhisper("El usuario fue mutead@ por " + Time + " Segundos!", 1);
                }
                Session.SendWhisper("El usuari@ " + Habbo.Username + " fue mutead@ por un tiempo de " + Time + " Segundos!", 1);
            }
            else
            {
                Session.SendWhisper("Debes ingresar el tiempo en segundos.", 1);
            }
        }
    }
}
