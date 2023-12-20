using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms.Games;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorBanzaiScoreCounter : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            if (Item.team == Team.none)
                return;
            Item.ExtraData = Item.GetRoom().GetGameManager().Points[(int)Item.team].ToString();
            Item.UpdateState(false, true);
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
                return;
            Item.GetRoom().GetGameManager().Points[(int)Item.team] = 0;
            Item.ExtraData = "0";
            Item.UpdateState();
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
