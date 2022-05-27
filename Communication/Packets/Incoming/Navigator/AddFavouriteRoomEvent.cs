using Akiled.Communication.Packets.Outgoing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AddFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;
            int num = Packet.PopInt();
            RoomData roomData = AkiledEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
            if (roomData == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || (Session.GetHabbo().FavoriteRooms.Contains(roomData)))
            {
                ServerPacket Response = new ServerPacket(33);
                Response.WriteInteger(-9001);
            }
            else
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.UpdateFavouriteRoomMessageComposer);
                Response.WriteInteger(num);
                Response.WriteBoolean(true);
                Session.SendPacket(Response);
                Session.GetHabbo().FavoriteRooms.Add(roomData);
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryreactor.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + Session.GetHabbo().Id + "," + num + ")");
            }
        }
    }
}