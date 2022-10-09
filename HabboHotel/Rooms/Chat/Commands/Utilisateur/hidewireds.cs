using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class hidewireds : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;

            Room.RoomData.HideWired = !Room.RoomData.HideWired;

            using (IQueryAdapter con = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                con.SetQuery("UPDATE `rooms` SET `allow_hidewireds` = @enum WHERE `id` = @id LIMIT 1");
                con.AddParameter("enum", AkiledEnvironment.BoolToEnum(Room.RoomData.HideWired));
                con.AddParameter("id", currentRoom.Id);
                con.RunQuery();
            }

            if (Room.RoomData.HideWired)
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.true", Session.Langue));
            else
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("cmd.hidewireds.false", Session.Langue));

            Room.SendMessage(Room.HideWiredMessages(currentRoom.HideWired));
        }
    }
}
