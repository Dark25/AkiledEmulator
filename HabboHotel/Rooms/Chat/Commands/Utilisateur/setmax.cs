using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Setmax : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int.TryParse(Params[1], out int MaxUsers);
            if (MaxUsers > 75 && Session.GetHabbo().Rank < 3 || MaxUsers > 1000 || MaxUsers <= 0)
                Room.SetMaxUsers(75);
            else
                Room.SetMaxUsers(MaxUsers);

        }
    }
}
