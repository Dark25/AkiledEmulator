using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users.Messenger;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class AcceptBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
                return;

            int Count = Packet.PopInt();
            for (int index = 0; index < Count; ++index)
            {
                int num2 = Packet.PopInt();
                MessengerRequest request = Session.GetHabbo().GetMessenger().GetRequest(num2);
                if (request != null)
                {
                    if (request.To != Session.GetHabbo().Id)
                        break;

                    if (!Session.GetHabbo().GetMessenger().FriendshipExists(request.To))
                        Session.GetHabbo().GetMessenger().CreateFriendship(request.From);

                    Session.GetHabbo().GetMessenger().HandleRequest(num2);
                }
            }
        }
    }
}