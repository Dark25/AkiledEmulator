using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class MessengerInitEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Session.GetHabbo().GetMessenger().OnStatusChanged(false);

            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeCategories());
            Session.SendPacket(Session.GetHabbo().GetMessenger().SerializeFriends());
            Session.GetHabbo().GetMessenger().ProcessOfflineMessages();
        }
    }
}
