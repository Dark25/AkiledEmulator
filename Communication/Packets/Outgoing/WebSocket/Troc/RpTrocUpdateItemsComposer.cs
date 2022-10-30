using Akiled.HabboHotel.Roleplay;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.WebSocket.Troc
{
    class RpTrocUpdateItemsComposer : ServerPacket
    {
        public RpTrocUpdateItemsComposer(int UserId, Dictionary<int, int> Items)
          : base(17)
        {
            WriteInteger(UserId);
            WriteInteger(Items.Count);

            foreach (KeyValuePair<int, int> Item in Items)
            {
                RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Item.Key);

                WriteInteger(Item.Key);
                WriteString((RpItem == null) ? "" : RpItem.Name);
                WriteString((RpItem == null) ? "" : RpItem.Desc);
                WriteInteger(Item.Value);
            }
        }
    }
}
