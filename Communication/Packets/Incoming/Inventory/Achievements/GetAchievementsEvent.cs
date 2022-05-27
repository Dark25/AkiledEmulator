using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetAchievementsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            AkiledEnvironment.GetGame().GetAchievementManager().GetList(Session, Packet);
        }
    }
}