using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorGate : FurniInteractor
    {
        public InteractorGate()
        {
        }

        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {

            if (!UserHasRights)
            {
                return;
            }

            int currentMode = 0;
            int newMode = 0;


            int.TryParse(Item.ExtraData, out currentMode);

            if (currentMode == 0)
            {
                newMode = 1;
            }
            else
            {
                newMode = 0;
            }

            Item.ExtraData = newMode.ToString();
            Item.UpdateState();
            Item.GetRoom().GetGameMap().updateMapForItem(Item);

        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
