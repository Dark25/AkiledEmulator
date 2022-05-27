using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CancelQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            AkiledEnvironment.GetGame().GetQuestManager().CancelQuest(Session, Packet);
        }
    }
}
