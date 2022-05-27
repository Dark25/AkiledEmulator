using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class UnIgnoreUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            if (Session.GetHabbo().CurrentRoom == null)
                return;
            string str = Packet.PopString();
            Habbo habbo = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(str).GetHabbo();
            if (habbo == null || !Session.GetHabbo().MutedUsers.Contains(habbo.Id))
                return;
            Session.GetHabbo().MutedUsers.Remove(habbo.Id);
            ServerPacket Response = new ServerPacket(ServerPacketHeader.IgnoreStatusMessageComposer);
            Response.WriteInteger(3);
            Response.WriteString(str);
            Session.SendPacket(Response);

        }
    }
}
