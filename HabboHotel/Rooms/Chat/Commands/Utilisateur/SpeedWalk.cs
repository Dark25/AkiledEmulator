using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class SpeedWalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.WalkSpeed = !UserRoom.WalkSpeed;

            if (UserRoom.WalkSpeed)
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.walkppeed.false", Session.Langue));
        }
    }
}
