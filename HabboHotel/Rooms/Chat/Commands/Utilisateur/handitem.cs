using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Handitem : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            int handitemid = -1;
            int.TryParse(Params[1], out handitemid);
            if (handitemid < 0)
                return;

            UserRoom.CarryItem(handitemid);

        }
    }
}
