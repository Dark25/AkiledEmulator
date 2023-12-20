using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorPinata : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;
            RoomUser roomUserByHabboId = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabboId == null)
                return;
            Session.SendWhisper("Du Nuub hast die Pinata angekickt :o Schon " + Item.ExtraData + "x geklickt.");
            if (!(Item.ExtraData == "1") && Gamemap.TileDistance(roomUserByHabboId.X, roomUserByHabboId.Y, Item.GetX, Item.GetY) <= 2)
            {
                AkiledEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(roomUserByHabboId, currentRoom, Item);
                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByHabboId.GetClient(), "ACH_PinataWhacker", 1);
                AkiledEnvironment.GetGame().GetAchievementManager().ProgressAchievement(roomUserByHabboId.GetClient(), "ACH_PinataBreaker", 1);
            }
        }

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
