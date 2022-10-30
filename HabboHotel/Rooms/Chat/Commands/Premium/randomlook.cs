using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd{    class RandomLook : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {            if (UserRoom.Team != Team.none || UserRoom.InGame)
                return;

            //GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetRandomClient();
            //if (Client == null || Client.GetHabbo() == null)
            //return;

            if (Session == null || Session.GetHabbo() == null)                return;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())            {                queryreactor.SetQuery("SELECT look FROM user_wardrobe WHERE user_id IN (SELECT user_id FROM (SELECT user_id FROM user_wardrobe WHERE user_id >= ROUND(RAND() * (SELECT max(user_id) FROM user_wardrobe)) LIMIT 1) tmp) ORDER BY RAND() LIMIT 1");                Session.GetHabbo().Look = queryreactor.GetString();            }            if (UserRoom.transformation || UserRoom.IsSpectator)                return;            if (!Session.GetHabbo().InRoom)                return;            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)                return;            RoomUser roomUserByHabbo = UserRoom;            if (roomUserByHabbo == null)                return;            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));            currentRoom.SendPacket(new UserChangeComposer(roomUserByHabbo, false));        }    }}