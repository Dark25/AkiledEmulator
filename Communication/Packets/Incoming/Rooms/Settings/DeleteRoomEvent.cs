using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class DeleteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().UsersRooms == null)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null || !(room.RoomData.OwnerName == Session.GetHabbo().Username))
                return;
            room.GetRoomItemHandler().ClearFurniture(Session);

            AkiledEnvironment.GetGame().GetRoomManager().UnloadRoom(room);

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("DELETE FROM rooms WHERE id = " + RoomId);
                queryreactor.RunQuery("DELETE FROM user_favorites WHERE room_id = " + RoomId);
                queryreactor.RunQuery("DELETE FROM room_rights WHERE room_id = " + RoomId);
                queryreactor.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN user_presents ON (user_presents.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) LEFT JOIN tele_links ON (tele_one_id = items.id) LEFT JOIN wired_items ON (trigger_id = items.id) WHERE room_id = " + RoomId);
                queryreactor.RunQuery("UPDATE users SET home_room = '0' WHERE home_room = " + RoomId);
                queryreactor.RunQuery("UPDATE bots SET room_id = '0' WHERE room_id = " + RoomId);
                queryreactor.RunQuery("UPDATE user_pets SET room_id = '0' WHERE room_id = " + RoomId);
            }
            RoomData removedRoom = (from p in Session.GetHabbo().UsersRooms
                                    where p.Id == RoomId
                                    select p).SingleOrDefault();
            if (removedRoom != null)
                Session.GetHabbo().UsersRooms.Remove(removedRoom);

            if (Session.GetHabbo().FavoriteRooms != null)
            {
                RoomData removedRoomFavo = (from p in Session.GetHabbo().FavoriteRooms where p.Id == RoomId select p).FirstOrDefault();

                if (removedRoomFavo != null)
                    Session.GetHabbo().FavoriteRooms.Remove(removedRoomFavo);
            }
        }
    }
}