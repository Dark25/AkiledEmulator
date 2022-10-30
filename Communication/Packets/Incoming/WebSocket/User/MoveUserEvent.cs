using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.WebClients;

namespace Akiled.Communication.Packets.Incoming.WebSocket
{
    class MoveUserEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int mX = Packet.PopInt();
            int mY = Packet.PopInt();

            if (mX > 1 || mX < -1) mX = 0;
            if (mY > 1 || mY < -1) mY = 0;

            GameClient Client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null) return;

            Room currentRoom = Client.GetHabbo().CurrentRoom;
            if (currentRoom == null) return;

            RoomUser User = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);

            if (User == null || (!User.CanWalk && !User.TeleportEnabled)) return;

            if (!User.AllowMoveTo) return;

            User.Unidle();

            User.IsWalking = true;

            if (User.ReverseWalk)
            {
                User.GoalX = User.SetX + (-mX * 1000);
                User.GoalY = User.SetY + (-mY * 1000);
            }
            else
            {
                User.GoalX = User.SetX + (mX * 1000);
                User.GoalY = User.SetY + (mY * 1000);
            }

        }
    }
}
