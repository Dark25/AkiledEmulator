using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetQuestListEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            AkiledEnvironment.GetGame().GetQuestManager().GetList(Session, Packet);
        }
    }
}
