using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Pathfinding;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class LookToEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            RoomUser User = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (User == null || User.RidingHorse)
                return;

            User.Unidle();
            int X2 = Packet.PopInt();
            int Y2 = Packet.PopInt();
            if (X2 == User.X && Y2 == User.Y)
            {
                if (User.SetStep)
                {
                    int rotation = Rotation.RotationIverse(User.RotBody);
                    User.SetRot(rotation, false, true);
                }
                return;
            }

            int Rotation2 = Rotation.Calculate(User.X, User.Y, X2, Y2);
            User.SetRot(Rotation2, false);
        }
    }
}