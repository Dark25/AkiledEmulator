using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class MakePrivateCommand : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            var room = Session.GetHabbo().CurrentRoom;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE rooms SET state = 'locked' WHERE id = " + room.Id + "");
            }
            var roomId = Session.GetHabbo().CurrentRoom.Id;
            var users = new List<RoomUser>(Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUsers().ToList());

            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(Session.GetHabbo().CurrentRoom);

            RoomData Data = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            Session.GetHabbo().PrepareRoom(Session.GetHabbo().CurrentRoom.Id, "");

            AkiledEnvironment.GetGame().GetRoomManager().LoadRoom(roomId);

            var data = new RoomForwardComposer(roomId);

            foreach (var user in users.Where(user => user != null && user.GetClient() != null))
                user.GetClient().SendMessage(data);
        }
    }
}
