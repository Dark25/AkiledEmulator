using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorOneWayGate : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser == 0)
                return;
            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
            if (roomUserByHabbo != null)
            {
                roomUserByHabbo.UnlockWalking();
            }
            Item.InteractingUser = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser == 0)
                return;
            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
            if (roomUserByHabbo != null)
            {
                roomUserByHabbo.UnlockWalking();
            }
            Item.InteractingUser = 0;
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null || Item.GetRoom() == null)
                return;

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;

            if (roomUserByHabbo.Coordinate != Item.SquareInFront && roomUserByHabbo.CanWalk)
            {
                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
            else
            {
                if (!roomUserByHabbo.CanWalk)
                    return;

                if (!Item.GetRoom().GetGameMap().CanWalk(Item.SquareBehind.X, Item.SquareBehind.Y, roomUserByHabbo.AllowOverride))
                    return;

                RoomUser roomUserByHabboItem = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabboItem != null)
                    return;

                Item.InteractingUser = 0;

                Item.InteractingUser = roomUserByHabbo.HabboId;
                roomUserByHabbo.CanWalk = false;

                roomUserByHabbo.AllowOverride = true;
                roomUserByHabbo.MoveTo(Item.SquareBehind);

                Item.ExtraData = "1";
                Item.UpdateState(false, true);

                Item.ReqUpdate(1);
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
