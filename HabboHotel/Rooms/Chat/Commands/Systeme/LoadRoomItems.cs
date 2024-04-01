using Akiled.HabboHotel.GameClients;
using System;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class LoadRoomItems : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            int RoomId;
            if (!int.TryParse(Params[1], out RoomId))
                return;

            Room.GetRoomItemHandler().LoadFurniture(RoomId);
            Room.GetGameMap().GenerateMaps();
            UserRoom.SendWhisperChat("Furnis en la sala " + RoomId + " cargados!");
            UserRoom.SendWhisperChat(String.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.loadroomitems", Session.Langue)));
        }
    }
}
