using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class ApplyHorseEffectEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
                return;

            int ItemId = Packet.PopInt();
            Item Item = Room.GetRoomItemHandler().GetItem(ItemId);
            if (Item == null)
                return;

            int PetId = Packet.PopInt();

            RoomUser PetUser = null;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out PetUser))
                return;

            if (PetUser.PetData == null || PetUser.PetData.OwnerId != Session.GetHabbo().Id)
                return;

            if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_1)
            {
                PetUser.PetData.Saddle = 9;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_pets` SET `have_saddle` = '1' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Item.Id + "");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_SADDLE_2)
            {
                PetUser.PetData.Saddle = 10;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_pets` SET `have_saddle` = '10' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Item.Id + "");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIRSTYLE)
            {
                int Parse = 100;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];

                Parse = Parse + int.Parse(HairType);

                PetUser.PetData.PetHair = Parse;
                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_pets` SET `pethair` = '" + PetUser.PetData.PetHair + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Item.Id + "");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_HAIR_DYE)
            {
                int HairDye = 48;
                string HairType = Item.GetBaseItem().ItemName.Split('_')[2];

                HairDye = HairDye + int.Parse(HairType);
                PetUser.PetData.HairDye = HairDye;

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_pets` SET `hairdye` = '" + PetUser.PetData.HairDye + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Item.Id + "");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }
            else if (Item.Data.InteractionType == InteractionType.HORSE_BODY_DYE)
            {
                string Race = Item.GetBaseItem().ItemName.Split('_')[2];
                int Parse = int.Parse(Race);
                int RaceLast = 2 + (Parse * 4) - 4;
                if (Parse == 13)
                    RaceLast = 61;
                else if (Parse == 14)
                    RaceLast = 65;
                else if (Parse == 15)
                    RaceLast = 69;
                else if (Parse == 16)
                    RaceLast = 73;
                PetUser.PetData.Race = RaceLast.ToString();

                using (IQueryAdapter dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    dbClient.RunQuery("UPDATE `user_pets` SET `race` = '" + PetUser.PetData.Race + "' WHERE `id` = '" + PetUser.PetData.PetId + "' LIMIT 1");
                    dbClient.RunQuery("DELETE items, items_limited FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) WHERE items.id = " + Item.Id + "");
                }

                //We only want to use this if we're successful. 
                Room.GetRoomItemHandler().RemoveFurniture(Session, Item.Id);
            }

            Room.SendPacket(new UsersComposer(PetUser));
            Room.SendPacket(new PetHorseFigureInformationComposer(PetUser));
        }
    }
}