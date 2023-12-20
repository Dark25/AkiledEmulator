using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public abstract class FurniInteractor
    {
        public abstract void OnPlace(GameClient Session, Item Item);

        public abstract void OnRemove(GameClient Session, Item Item);

        public abstract void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights);
        public abstract void OnTrigger2(GameClient Session, Item Item, int Request);
        public abstract void OnTick(Item item);
    }
}
