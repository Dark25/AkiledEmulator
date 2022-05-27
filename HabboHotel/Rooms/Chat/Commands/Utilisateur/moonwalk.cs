using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class moonwalk : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null || UserRoom.InGame)
                return;
            RoomUser roomUserByHabbo = UserRoom;
            roomUserByHabbo.MoonwalkEnabled = !roomUserByHabbo.MoonwalkEnabled;
            if (roomUserByHabbo.MoonwalkEnabled)
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.moonwalk.false", Session.Langue));

        }
    }
}
