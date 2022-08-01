using System;

using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Camera;

namespace Akiled.Communication.Packets.Incoming.Rooms.Camera
{
    public class PublishPhotoEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room Room = Session.GetHabbo().CurrentRoom;

            if (Room == null)
                return;

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabbo(Session.GetHabbo().Id);

            if (User == null || User.LastPhotoPreview == null)
                return;
        }
    }
}