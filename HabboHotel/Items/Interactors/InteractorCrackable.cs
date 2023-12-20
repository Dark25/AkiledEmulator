using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorCrackable : FurniInteractor
    {
        private readonly int Modes;

        public InteractorCrackable(int Modes)
        {
            this.Modes = Modes - 1;
            if (this.Modes >= 0) return;

            this.Modes = 0;
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
            if (!UserHasRights || this.Modes == 0)
                return;

            int.TryParse(Item.ExtraData, out int NumMode);

            NumMode++;

            if (NumMode > this.Modes) NumMode = 0;

            Item.ExtraData = NumMode.ToString();
            Item.UpdateState();
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
