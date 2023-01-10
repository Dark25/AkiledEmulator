using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Pets;
using Akiled.HabboHotel.Rooms;
using System.Drawing;

namespace Akiled.Communication.Packets.Incoming.Structure
{
    class PickUpPetEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetHabbo().InRoom)
                return;

            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().GetInventoryComponent() == null)
                return;

            Room Room;

            if (!AkiledEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room))
                return;

            int PetId = Packet.PopInt();

            RoomUser Pet;
            if (!Room.GetRoomUserManager().TryGetPet(PetId, out Pet))
            {
                //Check kick rights, just because it seems most appropriate.
                if ((!Room.CheckRights(Session) && Room.RoomData.WhoCanKick != 2 && Room.RoomData.Group == null) || (Room.RoomData.Group != null && !Room.CheckRights(Session)))
                    return;

                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(PetId);
                if (TargetUser == null)
                    return;

                //Check some values first, please!
                if (TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                    return;

                TargetUser.transformation = false;

                //Quickly remove the old user instance.
                Room.SendPacket(new UserRemoveComposer(TargetUser.VirtualId));

                //Add the new one, they won't even notice a thing!!11 8-)
                Room.SendPacket(new UsersComposer(TargetUser));
                return;
            }

            if (Session.GetHabbo().Id != Pet.PetData.OwnerId && !Room.CheckRights(Session, true))
                return;

            if (Pet.RidingHorse)
            {
                RoomUser UserRiding = Room.GetRoomUserManager().GetRoomUserByVirtualId(Pet.HorseID);
                if (UserRiding != null)
                {
                    UserRiding.RidingHorse = false;
                    UserRiding.ApplyEffect(-1);
                    UserRiding.MoveTo(new Point(UserRiding.X + 1, UserRiding.Y + 1));
                }
                else
                    Pet.RidingHorse = false;
            }

            Pet pet = Pet.PetData;

            pet.RoomId = 0;
            pet.PlacedInRoom = false;

            if (pet.DBState != DatabaseUpdateState.NeedsInsert)
                pet.DBState = DatabaseUpdateState.NeedsUpdate;

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE user_pets SET room_id = '0' WHERE id ='" + pet.PetId + "' LIMIT 1");


            if (pet.OwnerId != Session.GetHabbo().Id)
            {
                GameClient Target = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(pet.OwnerId);
                if (Target != null)
                {
                    Target.GetHabbo().GetInventoryComponent().TryAddPet(Pet.PetData);
                    Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);

                    Target.SendPacket(new PetInventoryComposer(Target.GetHabbo().GetInventoryComponent().GetPets()));
                    return;
                }
            }
            else
            {
                Session.GetHabbo().GetInventoryComponent().TryAddPet(Pet.PetData);
                Room.GetRoomUserManager().RemoveBot(Pet.VirtualId, false);
                Session.SendPacket(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
            }
        }
    }
}
