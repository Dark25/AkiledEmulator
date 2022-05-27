using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class StartQuestEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            AkiledEnvironment.GetGame().GetQuestManager().ActivateQuest(Session, Packet);
        }
    }
}
