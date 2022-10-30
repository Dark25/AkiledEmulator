using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Drawing;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class InitializeFloorPlanSessionEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);            if (room == null)                return;            ServerPacket Response = new ServerPacket(ServerPacketHeader.FloorPlanSendDoorMessageComposer);            Response.WriteInteger(room.GetGameMap().Model.DoorX); // x
            Response.WriteInteger(room.GetGameMap().Model.DoorY); // y
            Response.WriteInteger(room.GetGameMap().Model.DoorOrientation); // dir
            Session.SendPacket(Response);            ServerPacket Response2 = new ServerPacket(ServerPacketHeader.FloorPlanFloorMapMessageComposer);            Response2.WriteInteger(room.GetGameMap().CoordinatedItems.Count); //nombre de case

            foreach (Point Coords in room.GetGameMap().CoordinatedItems.Keys)
            {
                Response2.WriteInteger(Coords.X); // x
                Response2.WriteInteger(Coords.Y); // y
            }            Session.SendPacket(Response2);
        }
    }
}
