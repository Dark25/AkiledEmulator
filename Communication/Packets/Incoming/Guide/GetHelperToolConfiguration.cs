using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Guides;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetHelperToolConfiguration : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_helptool"))
                return;

            GuideManager guideManager = AkiledEnvironment.GetGame().GetGuideManager();
            bool onDuty = Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();
            Packet.PopBoolean();

            if (onDuty && !Session.GetHabbo().OnDuty)
            {
                guideManager.AddGuide(Session.GetHabbo().Id);
                Session.GetHabbo().OnDuty = true;
            }
            else
            {
                guideManager.RemoveGuide(Session.GetHabbo().Id);
                Session.GetHabbo().OnDuty = false;
            }

            ServerPacket HelpTool = new ServerPacket(ServerPacketHeader.HelperToolMessageComposer);
            HelpTool.WriteBoolean(Session.GetHabbo().OnDuty);
            HelpTool.WriteInteger(guideManager.GuidesCount);
            HelpTool.WriteInteger(0);
            HelpTool.WriteInteger(0);
            Session.SendPacket(HelpTool);
        }
    }
}