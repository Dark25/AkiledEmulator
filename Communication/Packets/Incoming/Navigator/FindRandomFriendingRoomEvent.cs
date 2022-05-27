using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class FindRandomFriendingRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string type = Packet.PopString();

            if (type == "predefined_noob_lobby")
            {
                Session.SendPacket(new NuxAlertComposer("nux/lobbyoffer/hide"));
            }

            Room Instance = AkiledEnvironment.GetGame().GetRoomManager().TryGetRandomLoadedRoom();

            if (Instance != null)
                Session.SendPacket(new RoomForwardComposer(Instance.Id));
        }
    }
}