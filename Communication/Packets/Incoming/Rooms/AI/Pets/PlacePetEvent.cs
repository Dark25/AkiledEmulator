using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.RoomBots;
using System;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PlacePetEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            Room Room;
            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            if (!Room.CheckRights(Session, true))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(1));
                return;
            }

            if (Room.GetRoomUserManager().BotCounter >= 30)
            {
                Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("notif.placepet.error", Session.Langue));
                return;
            }

            Pet Pet = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet))
                return;

            if (Pet == null)
                return;

            if (Pet.PlacedInRoom)
                return;

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false))
            {
                //Session.SendPacket(new RoomErrorNotifComposer(4));
                return;
            }

            RoomUser OldPet = null;
            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }

            Pet.X = X;
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.Id;

            Room.GetRoomUserManager().DeployBot(new RoomBot(Pet.PetId, Pet.OwnerId, Pet.RoomId, AIType.Pet, true, Pet.Name, "", "", Pet.Look, X, Y, 0, 0, false, "", 0, false, 0, 0, 0), Pet);
            if (Pet.DBState != DatabaseUpdateState.NeedsInsert)
                Pet.DBState = DatabaseUpdateState.NeedsUpdate;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE user_pets SET room_id = '" + Pet.RoomId + "' WHERE id ='" + Pet.PetId + "' LIMIT 1");
            }

            Pet ToRemove = null;
            if (!Session.GetHabbo().GetInventoryComponent().TryRemovePet(Pet.PetId, out ToRemove))
            {
                Console.WriteLine("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }

            Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));

        }
    }
}
