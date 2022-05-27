using Akiled.HabboHotel.GameClients;
using Akiled.Communication.Packets.Outgoing.Structure;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetBotInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;
            Session.SendPacket(new BotInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetBots()));
        }
    }
}
