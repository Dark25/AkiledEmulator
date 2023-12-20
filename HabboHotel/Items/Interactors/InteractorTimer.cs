using Akiled.HabboHotel.GameClients;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorTimer : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            //Item.ExtraData = "0";
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.ExtraData = "0";
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (!UserHasRights)
                return;
            int num = 0;
            if (!string.IsNullOrEmpty(Item.ExtraData))
            {
                try
                {
                    num = int.Parse(Item.ExtraData);
                }
                catch
                {
                }
            }
            if (Request == 2)
            {
                if (Item.pendingReset && num > 0)
                {
                    //num = 0;
                    Item.ChronoStarter = false;
                    Item.pendingReset = false;
                }
                else
                {
                    if (num == 0 || num == 30 || num == 60 || num == 120 || num == 180 || num == 300 || num == 600)
                    {
                        if (num == 0)
                            num = 30;
                        else if (num == 30)
                            num = 60;
                        else if (num == 60)
                            num = 120;
                        else if (num == 120)
                            num = 180;
                        else if (num == 180)
                            num = 300;
                        else if (num == 300)
                            num = 600;
                        else if (num == 600)
                            num = 0;
                    }
                    else
                        num = 0;
                }
            }
            else if ((Request == 0 || Request == 1) && num != 0 && !Item.ChronoStarter)
            {
                Item.ReqUpdate(1);
                Item.GetRoom().GetGameManager().StartGame();
                Item.ChronoStarter = true;
                Item.pendingReset = true;
            }
            Item.ExtraData = num.ToString();
            Item.UpdateState();
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }

}
