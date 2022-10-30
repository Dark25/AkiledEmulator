using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class MoveAvatarEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room currentRoom = Session.GetHabbo().CurrentRoom;            if (currentRoom == null)                return;            RoomUser User = currentRoom.GetRoomUserManager().GetRoomUserByHabboId((Session.GetHabbo().ControlUserId == 0) ? Session.GetHabbo().Id : Session.GetHabbo().ControlUserId);            if (User == null || (!User.CanWalk && !User.TeleportEnabled))                return;            int pX = Packet.PopInt();            int pY = Packet.PopInt();            if (User.ReverseWalk)
            {
                pX = User.SetX + (User.SetX - pX);
                pY = User.SetY + (User.SetY - pY);
            }            User.MoveTo(pX, pY, (User.AllowOverride || User.TeleportEnabled || User.ReverseWalk));
        }
    }
}
