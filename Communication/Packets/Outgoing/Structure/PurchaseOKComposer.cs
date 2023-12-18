using Akiled.HabboHotel.Catalog;
using Akiled.HabboHotel.Items;
using System;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class PurchaseOKComposer : ServerPacket
    {
        public PurchaseOKComposer(CatalogItem Item, ItemData BaseItem)
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {

           
            WriteInteger(BaseItem.Id);
            WriteString(BaseItem.ItemName);
            WriteBoolean(false);
            WriteInteger(Item.CostCredits);
            WriteInteger(Item.CostDuckets);
            WriteInteger(0);
            WriteBoolean(true);
            WriteInteger(1);
            WriteString(BaseItem.Type.ToString().ToLower());
            WriteInteger(BaseItem.SpriteId);
            WriteString("");
            WriteInteger(1);
            WriteInteger(0);
            WriteString("");
            WriteInteger(1);
            
        }

        public PurchaseOKComposer()
            : base(ServerPacketHeader.PurchaseOKMessageComposer)
        {
            WriteInteger(0);
            WriteString("");
            WriteBoolean(false);
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);
            WriteBoolean(true);
            WriteInteger(1);
            WriteString("s");
            WriteInteger(0);
            WriteString("");
            WriteInteger(1);
            WriteInteger(0);
            WriteString("");
            WriteInteger(1);
        }
    }
}
