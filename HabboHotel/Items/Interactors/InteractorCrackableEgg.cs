using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorCrackableEgg : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;
            int modes = Item.GetBaseItem().Modes;
            if (Session == null || !UserHasRights || modes <= 0)
                return;
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;
            RoomUser roomUserByHabboId = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabboId == null)
                return;
            int result = 0;
            if (int.TryParse(Item.ExtraData, out result))
                
            Item.ExtraData = (result <= 0 ? 1 : (result < modes ? result + 1 : 0)).ToString();
            Item.UpdateState();
            try
            {
                if (Item.ExtradataInt >= Item.GetBaseItem().Modes)
                {
                    AkiledEnvironment.GetGame().GetPinataManager().ReceiveCrackableReward(roomUserByHabboId, currentRoom, Item);
                    Session.SendWhisper("Nice Skills! Du konntest die Kiste erfolgreich knacken.", 1);
                    roomUserByHabboId.ApplyEffect(0);
                }
                else
                {
                    int num = Item.GetBaseItem().Modes - Item.ExtradataInt;
                    roomUserByHabboId.ApplyEffect(530);
                    Session.SendWhisper("Klicke noch " + num.ToString() + " mal um die Kiste zu knacken!", 1);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
