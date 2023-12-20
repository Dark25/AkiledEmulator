using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorHabboWheel : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
            Item.ReqUpdate(10);
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "-1";
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights || !(Item.ExtraData != "-1"))
                return;
            Item.ExtraData = "-1";
            Item.UpdateState();
            Item.ReqUpdate(10);
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
