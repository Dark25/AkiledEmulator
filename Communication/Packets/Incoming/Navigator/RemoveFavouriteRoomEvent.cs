using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;
            int i = Packet.PopInt();
            RoomData roomdata = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(i);
            if (roomdata == null)
                return;
            Session.GetHabbo().FavoriteRooms.Remove(roomdata);
            ServerPacket Response = new ServerPacket(ServerPacketHeader.UpdateFavouriteRoomMessageComposer);
            Response.WriteInteger(i);
            Response.WriteBoolean(false);
            Session.SendPacket(Response);
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery(string.Concat(new object[4] { "DELETE FROM user_favorites WHERE user_id = ", Session.GetHabbo().Id, " AND room_id = ", i }));

        }
    }
}
