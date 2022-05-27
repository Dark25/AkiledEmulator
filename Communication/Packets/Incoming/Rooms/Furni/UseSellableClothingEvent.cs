using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Catalog.Clothing;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Communication.Packets.Outgoing.Inventory.AvatarEffects;

namespace Akiled.Communication.Packets.Incoming.Rooms.Furni
{
    class UseSellableClothingEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (session == null || session.GetHabbo() == null || !session.GetHabbo().InRoom)
                return;

            Room room = session.GetHabbo().CurrentRoom;
            if (room == null)
                return;

            int itemId = packet.PopInt();

            Item item = room.GetRoomItemHandler().GetItem(itemId);
            if (item == null)
                return;

            if (item.Data == null)
                return;

            if (item.OwnerId != session.GetHabbo().Id)
                return;

            if (item.Data.InteractionType != InteractionType.PURCHASABLE_CLOTHING)
            {
                session.SendNotification("¡Vaya, este artículo no está configurado como una prenda de vestir vendible!");
                return;
            }

            if (item.Data.BehaviourData == 0)
            {
                session.SendNotification("Vaya, este artículo no tiene una configuración de ropa de enlace, ¡por favor repórtelo!");
                return;
            }

            if (!AkiledEnvironment.GetGame().GetCatalog().GetClothingManager().TryGetClothing(item.Data.BehaviourData, out ClothingItem clothing))
            {
                session.SendNotification("¡Vaya, no pudimos encontrar esta pieza de ropa!");
                return;
            }

            //Quickly delete it from the database.
            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("DELETE FROM `items` WHERE `id` = @ItemId LIMIT 1");
                dbClient.AddParameter("ItemId", item.Id);
                dbClient.RunQuery();
            }

            //Remove the item.
            room.GetRoomItemHandler().RemoveFurniture(session, item.Id);

            session.GetHabbo().GetClothing().AddClothing(clothing.ClothingName, clothing.PartIds);
            session.SendPacket(new FigureSetIdsComposer(session.GetHabbo().GetClothing().GetClothingParts));
            session.SendPacket(new RoomNotificationComposer("figureset.redeemed.success"));
            session.SendWhisper("Si por alguna razón no puede ver su ropa nueva, ¡recargue el hotel!");
        }
    }
}