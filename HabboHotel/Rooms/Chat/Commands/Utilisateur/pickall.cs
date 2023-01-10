using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class pickall : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Room == null || !Room.CheckRights(Session, true))
                return;
            if (Room.RoomData.SellPrice > 0)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.pickall", Session.Langue));
                return;
            }

            Room.GetRoomItemHandler().ClearFurniture(Session);
            Session.SendPacket(new FurniListUpdateComposer());
        }
    }
}