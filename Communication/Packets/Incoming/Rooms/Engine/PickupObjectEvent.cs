using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PickupObjectEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
                return;

            int Unknown = Packet.PopInt();
            int ItemId = Packet.PopInt();

            Item Item = room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            if (room.RoomData.SellPrice > 0)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("roomsell.error.7", Session.Langue));
                return;
            }

            bool ItemRights = false;
            if (Item.OwnerId == Session.GetHabbo().Id || room.CheckRights(Session, false))
                ItemRights = true;
            else if (Session.GetHabbo().HasFuse("room_item_take"))
                ItemRights = true;

            if (!ItemRights)
                return;

            if (Item.OwnerId == Session.GetHabbo().Id || Session.GetHabbo().HasFuse("room_item_take"))
            {
                room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
                Session.GetHabbo().GetInventoryComponent().AddItem(Item);
            }
            else//Se Expulsara el Item..
            {
                GameClient targetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(Item.OwnerId);
                if (targetClient != null && targetClient.GetHabbo() != null)//Verificar si el usuario esta conectado
                {
                    room.GetRoomItemHandler().RemoveFurniture(targetClient, Item.Id);
                    targetClient.GetHabbo().GetInventoryComponent().AddItem(Item);
                }
                else//El usuario no esta conectado.
                {
                    room.GetRoomItemHandler().RemoveFurniture(null, Item.Id);
                    using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        dbClient.RunQuery("UPDATE `items` SET `room_id` = '0' WHERE `id` = '" + Item.Id + "' LIMIT 1");
                    }
                }
            }
            AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_PICK, 0);

        }
    }
}
