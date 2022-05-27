using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class use : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            string Count = Params[1];
            if (!int.TryParse(Count, out int UseCount)) return;
            if (UseCount < 0 || UseCount > 100) return;

            Session.GetHabbo().forceUse = UseCount;
        }
    }
}
