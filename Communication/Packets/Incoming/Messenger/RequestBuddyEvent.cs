using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RequestBuddyEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null || !Session.GetHabbo().GetMessenger().RequestBuddy(Packet.PopString()))
                return;
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_FRIEND, 0);

        }
    }
}
