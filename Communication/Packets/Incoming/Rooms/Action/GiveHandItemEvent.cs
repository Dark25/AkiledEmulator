using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GiveHandItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo1 = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo1 == null || roomUserByHabbo1.CarryItemID <= 0 || roomUserByHabbo1.CarryTimer <= 0)
                return;
            RoomUser roomUserByHabbo2 = room.GetRoomUserManager().GetRoomUserByHabboId(Packet.PopInt());
            if (roomUserByHabbo2 == null)
                return;
            if (Math.Abs(roomUserByHabbo1.X - roomUserByHabbo2.X) >= 3 || Math.Abs(roomUserByHabbo1.Y - roomUserByHabbo2.Y) >= 3)
            {
                roomUserByHabbo1.MoveTo(roomUserByHabbo2.X, roomUserByHabbo2.Y);
                return;
            }

            roomUserByHabbo2.CarryItem(roomUserByHabbo1.CarryItemID);
            roomUserByHabbo1.CarryItem(0);
        }
    }
}