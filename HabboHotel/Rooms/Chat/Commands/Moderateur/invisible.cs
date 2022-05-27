using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class invisible : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().SpectatorMode)
            {
                Session.GetHabbo().SpectatorMode = false;
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("invisible.disabled", Session.Langue));
            }
            else
            {
                Session.GetHabbo().SpectatorMode = true;
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("invisible.enabled", Session.Langue));
            }

        }
    }
}
