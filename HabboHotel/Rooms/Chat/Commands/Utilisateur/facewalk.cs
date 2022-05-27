using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class facewalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.FacewalkEnabled = !roomUserByHabbo.FacewalkEnabled;
            if (roomUserByHabbo.FacewalkEnabled)
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.facewalk.false", Session.Langue));
        }
    }
}
