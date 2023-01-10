using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class ForceOpenGift : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Session.GetHabbo().forceOpenGift = !Session.GetHabbo().forceOpenGift;

            if (Session.GetHabbo().forceOpenGift) UserRoom.SendWhisperChat("Forzar abrir regalos activo");

            else UserRoom.SendWhisperChat("Forzar abrir regalos desactivado");
        }
    }
}
