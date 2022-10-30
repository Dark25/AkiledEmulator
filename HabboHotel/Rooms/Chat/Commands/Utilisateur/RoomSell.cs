using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RoomSell : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.1", Session.Langue));
                return;
            }

            if (!int.TryParse(Params[1], out int Prix))
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.2", Session.Langue));
                return;
            }
            if (Prix < 1)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.3", Session.Langue));
                return;
            }
            if (Prix > 99999999)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.4", Session.Langue));
                return;
            }

            if (Room.RoomData.Group != null)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.5", Session.Langue));
                return;
            }

            if (Room.RoomData.SellPrice > 0)
            {
                UserRoom.SendWhisperChat(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.6", Session.Langue));
                return;
            }

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.SetQuery("UPDATE rooms SET price= @price WHERE id = @roomid LIMIT 1");
                queryreactor.AddParameter("roomid", Room.Id);
                queryreactor.AddParameter("price", Prix);
                queryreactor.RunQuery();
            }

            Room.RoomData.SellPrice = Prix;

            UserRoom.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.valide", Session.Langue), Prix));

            foreach (RoomUser user in Room.GetRoomUserManager().GetUserList().ToList())            {                if (user == null || user.IsBot)                    continue;                user.SendWhisperChat(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.warn", Session.Langue), Prix));            }
        }
    }
}
