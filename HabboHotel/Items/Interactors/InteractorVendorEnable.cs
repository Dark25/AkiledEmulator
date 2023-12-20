using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Pathfinding;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorVendorEnable : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
                return;
            Item.InteractingUser = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            if (Item.InteractingUser <= 0)
                return;
            Item.InteractingUser = 0;
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!(Item.ExtraData != "1") || Item.GetBaseItem().VendingIds.Count < 1 || (Item.InteractingUser != 0 || Session == null))
                return;

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
                return;

            if (!Gamemap.TilesTouching(roomUserByHabbo.X, roomUserByHabbo.Y, Item.GetX, Item.GetY))
            {
                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
            else
            {
                Item.InteractingUser = Session.GetHabbo().Id;
                roomUserByHabbo.SetRot(Rotation.Calculate(roomUserByHabbo.X, roomUserByHabbo.Y, Item.GetX, Item.GetY), false);
                Item.ReqUpdate(2);
                Item.ExtraData = "1";
                Item.UpdateState(false, true);
            }

        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
