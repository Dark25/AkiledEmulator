using Akiled.HabboHotel.Roleplay;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class BuyItemsListComposer : ServerPacket
    {
        public BuyItemsListComposer(List<RPItem> ItemsBuy)
          : base(8)
        {
            WriteInteger(ItemsBuy.Count);

            foreach (RPItem Item in ItemsBuy)
            {
                WriteInteger(Item.Id);
                WriteString(Item.Name);
                WriteString(Item.Desc);
                WriteInteger(Item.Price);
            }
        }
    }
}