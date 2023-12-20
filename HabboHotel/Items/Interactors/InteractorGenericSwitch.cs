using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Quests;
using Akiled.HabboHotel.Rooms;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorGenericSwitch : FurniInteractor
    {
        private readonly int Modes;

        public InteractorGenericSwitch(int Modes)
        {
            this.Modes = Modes - 1;
            if (this.Modes >= 0)
                return;
            this.Modes = 0;
        }

        public override void OnPlace(GameClient Session, Item Item)
        {
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.CanWalk = true;
                }
                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 != 0)
            {
                RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
                if (roomUserByHabbo1 != null)
                {
                    roomUserByHabbo1.CanWalk = true;
                }
                Item.InteractingUser2 = 0;
            }

            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                    Item.ExtraData = "0;" + Item.GroupId;
                else
                    Item.ExtraData = "0";
            }
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.CanWalk = true;
                }
                Item.InteractingUser = 0;
            }
            if (Item.InteractingUser2 != 0)
            {
                RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
                if (roomUserByHabbo1 != null)
                {
                    roomUserByHabbo1.CanWalk = true;
                }
                Item.InteractingUser2 = 0;
            }
            if (string.IsNullOrEmpty(Item.ExtraData) && this.Modes > 0)
            {
                if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                    Item.ExtraData = "0;" + Item.GroupId;
                else
                    Item.ExtraData = "0";
            }
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session != null)
                AkiledEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.FURNI_SWITCH, 0);

            if (!UserHasRights || this.Modes == 0)
                return;
            int NumMode = 0;

            if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                int.TryParse(Item.ExtraData.Split(';')[0], out NumMode);
            else
                int.TryParse(Item.ExtraData, out NumMode);

            int num2 = (NumMode > 0) ? (NumMode < this.Modes) ? NumMode + 1 : 0 : 1;

            if (Session != null && Session.GetHabbo() != null && Session.GetHabbo().forceUse > -1)
            {
                if (Session.GetHabbo().forceUse <= this.Modes)
                    num2 = Session.GetHabbo().forceUse;
                else
                    num2 = 0;
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.GUILD_ITEM || Item.GetBaseItem().InteractionType == InteractionType.GUILD_GATE)
                Item.ExtraData = num2.ToString() + ";" + Item.GroupId;
            else
                Item.ExtraData = num2.ToString();
            Item.UpdateState();

            if (Item.GetBaseItem().AdjustableHeights.Count > 1)
            {
                if (Session == null)
                    return;
                Room room = AkiledEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
                if (room == null)
                    return;
                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (roomUserByHabbo != null)
                    Item.GetRoom().GetRoomUserManager().UpdateUserStatus(roomUserByHabbo, false);
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
