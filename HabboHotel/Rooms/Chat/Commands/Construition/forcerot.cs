using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class forcerot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;
            int num = 0;
            int.TryParse(Params[1], out num);
            if (num <= -1 || num >= 7)
                Session.GetHabbo().forceRot = 0;
            else
                Session.GetHabbo().forceRot = num;
        }
    }
}
