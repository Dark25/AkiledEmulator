using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class TradeOfferMultipleItemsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            Trade userTrade = room.GetUserTrade(Session.GetHabbo().Id);
            if (userTrade == null)
                return;

            int ItemCount = Packet.PopInt();
            for (int i = 0; i < ItemCount; i++)
            {
                int ItemId = Packet.PopInt();
                Item userItem = Session.GetHabbo().GetInventoryComponent().GetItem(ItemId);
                if (userItem == null)
                    continue;

                userTrade.OfferItem(Session.GetHabbo().Id, userItem);
            }
        }
    }
}
