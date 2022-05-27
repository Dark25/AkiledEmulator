using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System.Collections.Generic;
using System.Linq;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class RequestFurniInventoryEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
                return;
            if (Session.GetHabbo().GetInventoryComponent() == null)
                return;
            if (!Session.GetHabbo().GetInventoryComponent().inventoryDefined)
            {
                Session.GetHabbo().GetInventoryComponent().LoadInventory();
                Session.GetHabbo().GetInventoryComponent().inventoryDefined = true;
            }

            IEnumerable<Item> Items = Session.GetHabbo().GetInventoryComponent().GetWallAndFloor;
            Session.SendPacket(new FurniListComposer(Items.ToList(), 1, 0));
        }
    }
}