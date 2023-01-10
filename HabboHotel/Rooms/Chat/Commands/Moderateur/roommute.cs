using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class roommute : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room.RoomMuted = !Room.RoomMuted;

            foreach (RoomUser User in Room.GetRoomUserManager().GetRoomUsers())
            {
                if (User == null) continue;

                User.SendWhisperChat((Room.RoomMuted) ? "Ya no puedes hablar" : "Ahora puedes volver hablar");
            }
        }
    }
}
