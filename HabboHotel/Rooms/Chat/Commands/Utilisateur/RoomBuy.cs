using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.Chat.Commands.Cmd
{
    class RoomBuy : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (Room.RoomData.SellPrice == 0)
                return;

            Session.GetHabbo().AkiledPoints -= Room.RoomData.SellPrice;
            Session.GetHabbo().UpdateDiamondsBalance();

            GameClient ClientOwner = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Room.RoomData.OwnerId);
            if (ClientOwner != null && ClientOwner.GetHabbo() != null)
            {
                ClientOwner.GetHabbo().AkiledPoints += Room.RoomData.SellPrice;
                ClientOwner.GetHabbo().UpdateDiamondsBalance();
            }

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE users SET vip_points = vip_points - " + Room.RoomData.SellPrice + " WHERE id = '" + Session.GetHabbo().Id + "';");
                queryreactor.RunQuery("UPDATE users SET vip_points = vip_points + " + Room.RoomData.SellPrice + " WHERE id = '" + Room.RoomData.OwnerId + "';");

                queryreactor.RunQuery("DELETE FROM room_rights WHERE room_id = '" + Room.Id + "';");

                queryreactor.SetQuery("UPDATE rooms SET owner = @newowner WHERE id = '" + Room.Id + "';");
                queryreactor.AddParameter("newowner", Session.GetHabbo().Username);
                queryreactor.RunQuery();

                queryreactor.RunQuery("UPDATE rooms SET price = '0' WHERE id = '" + Room.Id + "' LIMIT 1;");
            }

            Session.SendNotification(string.Format(AkiledEnvironment.GetLanguageManager().TryGetValue("roombuy.sucess", Session.Langue), Room.RoomData.SellPrice));

            Room.RoomData.SellPrice = 0;

            List<RoomUser> UsersToReturn = Room.GetRoomUserManager().GetRoomUsers().ToList();
            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(Room);

            foreach (RoomUser User in UsersToReturn)
            {
                if (User == null || User.GetClient() == null)
                    continue;

                User.GetClient().SendPacket(new RoomForwardComposer(Room.Id));
            }
        }
    }
}
