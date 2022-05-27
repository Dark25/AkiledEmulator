using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class DeclineBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
                return;
            bool DeleteAllFriend = Packet.PopBoolean();
            int Nombre = Packet.PopInt();

            if (!DeleteAllFriend && Nombre == 1)
                Session.GetHabbo().GetMessenger().HandleRequest(Packet.PopInt());
            else
                Session.GetHabbo().GetMessenger().HandleAllRequests();
        }
    }
}