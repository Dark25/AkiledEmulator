using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class deleteMission : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
                return;

            string username = Params[1];
            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null)
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("user.notpermitted", Session.Langue));
            }
            else
            {
                clientByUsername.GetHabbo().Motto = AkiledEnvironment.GetLanguageManager().TryGetValue("user.unacceptable_motto", clientByUsername.Langue);
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + clientByUsername.GetHabbo().Id + "'");
                    queryreactor.AddParameter("motto", clientByUsername.GetHabbo().Motto);
                    queryreactor.RunQuery();
                }
                Room currentRoom2 = clientByUsername.GetHabbo().CurrentRoom;
                if (currentRoom2 == null)
                    return;
                RoomUser roomUserByHabbo = currentRoom2.GetRoomUserManager().GetRoomUserByHabboId(clientByUsername.GetHabbo().Id);
                if (roomUserByHabbo == null)
                    return;

                currentRoom2.SendPacket(new UserChangeComposer(roomUserByHabbo, false));
            }

        }
    }
}
