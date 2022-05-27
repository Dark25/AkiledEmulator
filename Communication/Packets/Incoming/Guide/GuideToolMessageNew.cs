using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GuideToolMessageNew : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string message = Packet.PopString();
            GameClient requester = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.GetHabbo().GuideOtherUserId);
            if (requester == null)
                return;
            if (Session.Antipub(message, "<GUIDEMESSAGE>"))
                return;

            ServerPacket messageC = new ServerPacket(ServerPacketHeader.OnGuideSessionMsg);
            messageC.WriteString(message);
            messageC.WriteInteger(Session.GetHabbo().Id);

            requester.SendPacket(messageC);
            Session.SendPacket(messageC);
        }
    }
}