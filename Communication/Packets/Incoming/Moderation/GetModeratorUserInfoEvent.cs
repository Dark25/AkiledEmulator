using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetModeratorUserInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            int num = Packet.PopInt();
            if (AkiledEnvironment.GetGame().GetClientManager().GetNameById(num) != "")
                Session.SendPacket(ModerationManager.SerializeUserInfo(num));
            else
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("user.loadusererror", Session.Langue));

        }
    }
}
