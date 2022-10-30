using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;
using System.Threading.Tasks;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.SpecialPvP
{
    class SmokeWeedCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            Room TargetRoom = Session.GetHabbo().CurrentRoom;

            if ((double)Session.GetHabbo().last_fumar > AkiledEnvironment.GetUnixTimestamp() - 30.0 && !Session.GetHabbo().HasFuse("override_limit_command"))
            {
                Session.SendWhisper("Debes esperar 30 segundos, para volver a usar el comando", 1);
                return;
            }

            if (!TargetRoom.RoomData.CrispyEnabled && !TargetRoom.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("room_override_custom_config"))
            {
                Session.SendWhisper("Disculpa, pero el dueño de la sala ha desactivado este comando.");
                return;
            }

            else
            {

                RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (ThisUser == null)
                    return;

                Task.Run(async delegate
                {
                    UserRoom.OnChat("@red@ * " + Session.GetHabbo().Username + ", Prendiendo El Punto *", 0, false);
                    await Task.Delay(2000);
                    UserRoom.ApplyEffect(26);
                    await Task.Delay(1000);
                    UserRoom.ApplyEffect(751);
                    await Task.Delay(4000);
                    UserRoom.OnChat("@purple@ * " + Session.GetHabbo().Username + ", Subiendo La Nota *", 0, false);
                    await Task.Delay(500);
                    UserRoom.ApplyEffect(0);
                    await Task.Delay(5000);
                    UserRoom.ApplyEffect(11);
                    await Task.Delay(6000);
                    UserRoom.ApplyEffect(53);
                    UserRoom.OnChat("@green@ * " + Session.GetHabbo().Username + ", Jaja Se Prendio Esta Mierda *", 0, false);
                    await Task.Delay(2000);
                    UserRoom.ApplyEffect(0);
                    Session.GetHabbo().last_fumar = AkiledEnvironment.GetIUnixTimestamp();
                });
            }
        }
    }
}