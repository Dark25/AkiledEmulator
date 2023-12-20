using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorTeleport : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.AllowOverride = false;
                    roomUserByHabbo.CanWalk = true;
                }
                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 == 0)
                return;
            RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
            if (roomUserByHabbo1 != null)
            {
                roomUserByHabbo1.AllowOverride = false;
                roomUserByHabbo1.CanWalk = true;
            }
            Item.InteractingUser2 = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                    roomUserByHabbo.UnlockWalking();
                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 == 0)
                return;
            RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
            if (roomUserByHabbo1 != null)
                roomUserByHabbo1.UnlockWalking();
            Item.InteractingUser2 = 0;
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Item == null || Item.GetRoom() == null || (Session == null || Session.GetHabbo() == null))
                return;
            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;
            if (roomUserByHabbo.Coordinate == Item.Coordinate || roomUserByHabbo.Coordinate == Item.SquareInFront)
            {
                if (Item.InteractingUser != 0)
                    return;
                Item.InteractingUser = roomUserByHabbo.GetClient().GetHabbo().Id;
                Item.ReqUpdate(2);
            }
            else
            {
                if (!roomUserByHabbo.CanWalk)
                    return;
                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
