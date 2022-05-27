using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class TradingOfferItemEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            Trade userTrade = room.GetUserTrade(Session.GetHabbo().Id);
            Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(Packet.PopInt());
            if (userTrade == null || userItem == null)
                return;

            userTrade.OfferItem(Session.GetHabbo().Id, userItem);

        }
    }
}
