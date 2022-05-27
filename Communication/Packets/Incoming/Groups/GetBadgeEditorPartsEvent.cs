using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetBadgeEditorPartsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;            Session.SendPacket(new BadgeEditorPartsComposer(
                AkiledEnvironment.GetGame().GetGroupManager().BadgeBases,
                AkiledEnvironment.GetGame().GetGroupManager().BadgeSymbols,
                AkiledEnvironment.GetGame().GetGroupManager().BadgeBaseColours,
                AkiledEnvironment.GetGame().GetGroupManager().BadgeSymbolColours,
                AkiledEnvironment.GetGame().GetGroupManager().BadgeBackColours));
        }
    }
}
