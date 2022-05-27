using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class Say : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
                return;

            string username = Params[1];
            string Message = CommandManager.MergeParams(Params, 2);

            RoomUser roomUserByHabbo = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(username);
            if (roomUserByHabbo == null)
                return;

            roomUserByHabbo.OnChat(Message, 0, false);

        }
    }
}
