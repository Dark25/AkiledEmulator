using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorJukebox : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            if (!(Item.ExtraData == "1") || Item.GetRoom().GetTraxManager().IsPlaying)
                return;
            Item.GetRoom().GetTraxManager().PlayPlaylist();
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
            Item.UpdateState();
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            Room room = Item.GetRoom();
            if (Request != 0 && Request != 1)
                return;
            room.GetTraxManager().TriggerPlaylistState();
        }

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
