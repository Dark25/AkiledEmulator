using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class CreateFlatEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null)
                return;

            if (Session.GetHabbo().UsersRooms.Count >= 200)
            {
                Session.SendPacket(new CanCreateRoomComposer(true, 200));
                return;
            }

            string Name = Packet.PopString();
            string Description = Packet.PopString();
            string RoomModel = Packet.PopString();
            int Category = Packet.PopInt();
            int MaxVisitors = Packet.PopInt();
            int TradeSettings = Packet.PopInt();

            if (MaxVisitors > 50 || MaxVisitors < 1)
            {
                MaxVisitors = 10;
            }

            if (TradeSettings < 0 || TradeSettings > 2)
                TradeSettings = 0;

            RoomData NewRoom = AkiledEnvironment.GetGame().GetRoomManager().CreateRoom(Session, Name, Description, RoomModel, Category, MaxVisitors, TradeSettings);
            if (NewRoom == null)
                return;
            Session.SendPacket(new FlatCreatedComposer(NewRoom.Id, Name));
        }
    }
}