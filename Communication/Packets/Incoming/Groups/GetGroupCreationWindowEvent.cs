using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class GetGroupCreationWindowEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            List<RoomData> ValidRooms = new List<RoomData>();
            foreach (RoomData Data in Session.GetHabbo().UsersRooms)
            {
                if (Data.Group == null)
                    ValidRooms.Add(Data);
            }

            Session.SendPacket(new GroupCreationWindowComposer(ValidRooms));
        }
    }
}