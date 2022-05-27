using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class FollowUserIdEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
                return;

            int UserId = Packet.PopInt();
            GameClient clientByUserId = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(UserId);
            if (clientByUserId == null || clientByUserId.GetHabbo() == null || !clientByUserId.GetHabbo().InRoom || (clientByUserId.GetHabbo().HideInRoom && !Client.GetHabbo().HasFuse("fuse_mod")))
                return;
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(clientByUserId.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            ServerPacket Response = new ServerPacket(ServerPacketHeader.RoomForwardMessageComposer);
            Response.WriteInteger(clientByUserId.GetHabbo().CurrentRoomId);
            Client.SendPacket(Response);
        }
    }
}
