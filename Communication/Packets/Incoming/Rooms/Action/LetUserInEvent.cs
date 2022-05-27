using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class LetUserInEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || !room.CheckRights(Session))
                return;

            string username = Packet.PopString();
            bool allowUserToEnter = Packet.PopBoolean();
            GameClient clientByUsername = AkiledEnvironment.GetGame().GetClientManager().GetClientByUsername(username);
            if (clientByUsername == null || clientByUsername.GetHabbo() == null)
                return;

            if (clientByUsername.GetHabbo().LoadingRoomId != room.Id)
                return;

            RoomUser user = room.GetRoomUserManager().GetRoomUserByHabboId(clientByUsername.GetHabbo().Id);

            if (user != null)
                return;

            if (allowUserToEnter)
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.FlatAccessibleMessageComposer);
                Response.WriteString("");
                clientByUsername.SendPacket(Response);

                clientByUsername.GetHabbo().AllowDoorBell = true;

                if (!clientByUsername.GetHabbo().EnterRoom(Session.GetHabbo().CurrentRoom))
                    clientByUsername.SendPacket(new CloseConnectionComposer());
            }
            else
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.FlatAccessDeniedMessageComposer);
                Response.WriteString("");
                clientByUsername.SendPacket(Response);
            }
        }
    }
}