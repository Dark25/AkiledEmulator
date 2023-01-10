using Akiled.HabboHotel.Roleplay;
using Akiled.HabboHotel.Roleplay.Player;
using System.Collections.Concurrent;

namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class LoadInventoryRpComposer : ServerPacket
    {
        public LoadInventoryRpComposer(ConcurrentDictionary<int, RolePlayInventoryItem> Items)
          : base(9)
        {
            WriteInteger(Items.Count);

            foreach (RolePlayInventoryItem Item in Items.Values)
            {
                RPItem RpItem = AkiledEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(Item.ItemId);

                WriteInteger(Item.ItemId);
                WriteString(RpItem.Name);
                WriteString(RpItem.Desc);
                WriteInteger(Item.Count);
                WriteInteger((int)RpItem.Category);
                WriteInteger(RpItem.UseType);
            }
        }
    }
}