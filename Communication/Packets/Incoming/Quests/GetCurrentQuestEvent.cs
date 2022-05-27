using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetCurrentQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            AkiledEnvironment.GetGame().GetQuestManager().GetCurrentQuest(Session, Packet);
        }
    }
}