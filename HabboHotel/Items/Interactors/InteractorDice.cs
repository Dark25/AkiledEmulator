// Type: Akiled.HabboHotel.Items.Interactors.InteractorDice




using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorDice : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
            }
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            if (Item.ExtraData == "-1")
            {
                Item.ExtraData = "0";
            }
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {

            //Session.GetHabbo().casinoEnabled = true;
            RoomUser roomUser = (RoomUser)null;
            if (Session != null)
                roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;
            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, roomUser.X, roomUser.Y))
            {
                if (!(Item.ExtraData != "-1"))
                    return;
                if (Request == -1)
                {
                    Item.ExtraData = "0";
                    Item.UpdateState();
                }
                else
                {
                    Item.ExtraData = "-1";
                    Item.InteractingUser = Session.GetHabbo().Id;
                    Item.UpdateState(false, true);
                    Item.ReqUpdate(4);
                }
            }
            else
                roomUser.MoveTo(Item.SquareInFront);
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
