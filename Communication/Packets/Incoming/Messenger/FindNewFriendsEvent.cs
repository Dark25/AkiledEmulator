using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class FindNewFriendsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int RoomId = 447654;
            ServerPacket Response = new ServerPacket(ServerPacketHeader.RoomForwardMessageComposer);
            Response.WriteInteger(RoomId);
            Session.SendPacket(Response);

        }
    }
}
