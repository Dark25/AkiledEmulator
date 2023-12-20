using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class WiredInteractor : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Item == null || !UserHasRights)
                return;

            if (Item.WiredHandler != null)
                Item.WiredHandler.OnTrigger(Session, Item.GetBaseItem().SpriteId);
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
