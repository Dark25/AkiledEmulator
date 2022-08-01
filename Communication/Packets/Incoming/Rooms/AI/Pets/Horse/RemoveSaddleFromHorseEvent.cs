using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Catalog.Utilities;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Rooms.AI.Pets.Horse
{
    internal class RemoveSaddleFromHorseEvent : IPacketEvent
    {
        public void Parse(GameClient session, ClientPacket packet)
        {
            if (!session.GetHabbo().InRoom)
                return;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetHabbo().CurrentRoomId, out Room room))
                return;

            if (!room.GetRoomUserManager().TryGetPet(packet.PopInt(), out RoomUser petUser))
                return;

            if (petUser.PetData == null || petUser.PetData.OwnerId != session.GetHabbo().Id)
                return;

            //Fetch the furniture Id for the pets current saddle.
            int saddleId = ItemUtility.GetSaddleId(petUser.PetData.Saddle);

            //Remove the saddle from the pet.
            petUser.PetData.Saddle = 0;

            using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("UPDATE `bots_petdata` SET `have_saddle` = '0' WHERE `id` = '" + petUser.PetData.PetId + "' LIMIT 1");
            }

            //Give the saddle back to the user.
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(saddleId, out ItemData itemData))
                return;

            Item item = ItemFactory.CreateSingleItemNullable(itemData, session.GetHabbo(), "");
            if (item != null)
            {
                session.GetHabbo().GetInventoryComponent().TryAddItem(item);
                session.SendPacket(new FurniListNotificationComposer(item.Id, 1));
                session.SendPacket(new PurchaseOKComposer());
                session.SendPacket(new FurniListAddComposer(item));
                session.SendPacket(new FurniListUpdateComposer());
            }

            //Update the Pet and the Pet figure information.
            room.SendPacket(new UsersComposer(petUser));
            room.SendPacket(new PetHorseFigureInformationComposer(petUser));
        }
    }
}