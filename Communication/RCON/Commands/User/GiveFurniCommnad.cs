using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;

namespace Akiled.Communication.RCON.Commands.Hotel
{
    class GiveFurniCommnad : IRCONCommand
    {

        public bool TryExecute(string[] parameters)
        {

            if (!int.TryParse(parameters[0].ToString(), out int userId))
                return false;

            if (userId == 0)
                return false;


            GameClient client = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(userId);
            if (client == null || client.GetHabbo() == null)
                return false;


            if (!int.TryParse(parameters[1].ToString(), out int itemId))
                return false;
            if (itemId == 0)
                return false;

            if (!int.TryParse(parameters[2].ToString(), out int Amount))
                return false;
            if (Amount == 0)
                return false;




            ItemData ItemData = null;

            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(parameters[1]), out ItemData))
            {
                client.SendWhisper("Item inexistente.");
                return false;
            }

            List<Item> Items = ItemFactory.CreateMultipleItems(ItemData, client.GetHabbo(), "", Amount);
            foreach (Item result in Items)
            {
                Item furni = result;
                Item PurchasedItem = furni;
                client.GetHabbo().GetInventoryComponent().TryAddItem(PurchasedItem);
              
            }
            client.SendPacket((IServerPacket)new FurniListNotificationComposer(ItemData.Id, 1));
            client.SendPacket((IServerPacket)RoomNotificationComposer.SendBubble("icons/" + ItemData.ItemName + "_icon", "Acabas de recibir: "+ Amount + "\n\n" + ItemData.publicname + " \n\n¡Corre, " + client.GetHabbo().Username + ", revisa tu inventario!!", "inventory/open/furni"));



            return true;
        }

    }

}