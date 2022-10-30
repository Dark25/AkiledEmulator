using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Support;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetModeratorRoomInfoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().HasFuse("fuse_mod"))
                return;
            Session.SendPacket(ModerationManager.SerializeRoomTool(AkiledEnvironment.GetGame().GetRoomManager().GenerateNullableRoomData(Packet.PopInt())));

        }
    }
}
