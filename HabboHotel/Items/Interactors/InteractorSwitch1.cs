using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorSwitch1 : FurniInteractor
    {
        private readonly int Modes;

        public InteractorSwitch1(int Modes)
        {
            this.Modes = Modes - 1;
            if (this.Modes >= 0)
                return;
            this.Modes = 0;
        }

        public override void OnPlace(GameClient Session, Item Item)
        {
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
                Item.ExtraData = "0";
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
                Item.ExtraData = "0";
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session != null)
                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH, 0);


            if (this.Modes == 0)
                return;

            RoomUser roomUser = (RoomUser)null;
            if (Session != null)
                roomUser = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUser == null)
                return;
            if (!Gamemap.TilesTouching(Item.GetX, Item.GetY, roomUser.X, roomUser.Y))
                return;

            int num1 = 0;
            try
            {
                num1 = int.Parse(Item.ExtraData);
            }
            catch
            {
            }
            int num2 = num1 > 0 ? (num1 < this.Modes ? num1 + 1 : 0) : 1;
            Item.ExtraData = num2.ToString();
            Item.UpdateState();
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
