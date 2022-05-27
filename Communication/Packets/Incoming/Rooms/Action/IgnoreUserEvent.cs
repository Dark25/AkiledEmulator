using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Users;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class IgnoreUserEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;
            if (Session.GetHabbo().CurrentRoom == null)
                return;
            string str = Packet.PopString();
            GameClient gameclient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(str);
            if (gameclient == null)
                return;
            Habbo habbo = gameclient.GetHabbo();
            if (habbo == null || Session.GetHabbo().MutedUsers.Contains(habbo.Id))
                return;
            Session.GetHabbo().MutedUsers.Add(habbo.Id);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.IgnoreStatusMessageComposer);
            Response.WriteInteger(1);
            Response.WriteString(str);
            Session.SendPacket(Response);
        }
    }
}