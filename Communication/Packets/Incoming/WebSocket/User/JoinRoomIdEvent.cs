using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class JoinRoomIdEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null) return;

            int RoomId = Packet.PopInt();

            Room Room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (Room == null) return;

            Client.SendPacket(new GetGuestRoomResultComposer(Client, Room.RoomData, false, true));
        }
    }
}
