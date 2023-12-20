using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorCrafting : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();
        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights) => Session.SendMessage((IServerPacket)new CraftableProductsComposer());

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
