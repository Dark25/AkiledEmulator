using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands
{
    class OldFoot : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.OldFoot = !Room.OldFoot;

            if (Room.OldFoot)
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.oldfoot.false", Session.Langue));
        }
    }
}
