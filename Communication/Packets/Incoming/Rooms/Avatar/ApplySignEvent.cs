using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ApplySignEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;
            RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            roomUserByHabbo.Unidle();
            int num = Packet.PopInt();
            if (roomUserByHabbo.Statusses.ContainsKey("sign"))
                roomUserByHabbo.RemoveStatus("sign");
            roomUserByHabbo.SetStatus("sign", Convert.ToString(num));
            roomUserByHabbo.UpdateNeeded = true;

        }
    }
}
