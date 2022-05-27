using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class AddFilter : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            AkiledEnvironment.GetGame().GetChatManager().GetFilter().AddFilterPub(Params[1].ToLower());
        }
    }
}
