using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;

namespace Akiled.HabboHotel.Items.Interactors
{
    internal class InteractorCrackableEggMare : FurniInteractor
    {
        public void sendItem(GameClient Session, int ItemId)
        {
            ItemData Data;
            if (!AkiledEnvironment.GetGame().GetItemManager().GetItem(Convert.ToInt32(ItemId), out Data))
                return;
            Item singleItemNullable = ItemFactory.CreateSingleItemNullable(Data, Session.GetHabbo(), "");
            if (singleItemNullable != null)
            {
                Session.GetHabbo().GetInventoryComponent().TryAddItem(singleItemNullable);
                Session.SendMessage((IServerPacket)new FurniListNotificationComposer(singleItemNullable.Id, 1));
                Session.SendMessage((IServerPacket)new FurniListUpdateComposer());
                Session.GetHabbo().GetInventoryComponent().UpdateItems(true);
            }
        }

        public override void OnPlace(GameClient Session, Item Item) => Item.ExtraData = "0";

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            if (Session == null || Session.GetHabbo() == null || Item == null)
                return;
            int modes = Item.GetBaseItem().Modes;
            if (Session == null || modes <= 0)
                return;
            Room currentRoom = Session.GetHabbo().CurrentRoom;
            if (currentRoom == null)
                return;
            RoomUser roomUserByHabboId = currentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabboId == null)
                return;
            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, roomUserByHabboId.X, roomUserByHabboId.Y))
            {
                int result = 0;
                if (int.TryParse(Item.ExtraData, out result))
                    
                Item.ExtraData = (result <= 0 ? 1 : (result < modes ? result + 1 : 0)).ToString();
                Item.UpdateState();
                try
                {
                    if (Session.GetHabbo().last_pickupitem + 3 > AkiledEnvironment.GetIUnixTimestamp())
                        Session.SendWhisper("Warte 3 Sekunden, bevor du einen neuen Gegenstand aufnimmst!", 1);
                    else if (Item.ExtradataInt >= Item.GetBaseItem().Modes)
                    {
                        AkiledEnvironment.GetGame().GetPinataManager().ReceiveCrackableRewardMare(roomUserByHabboId, currentRoom, Item);
                        Session.SendWhisper("Du hast einen Gegenstand aufgesammelt.", 1);
                        Session.GetHabbo().last_pickupitem = AkiledEnvironment.GetIUnixTimestamp();
                    }
                    else
                    {
                        int num = Item.GetBaseItem().Modes - Item.ExtradataInt;
                        Session.SendWhisper("Klicke noch " + num.ToString() + " mal, um den Gegenstand aufzuheben.", 1);
                    }
                }
                catch (Exception ex)
                {
                }
            }
            else
                roomUserByHabboId.MoveTo(Item.SquareInFront);
        }

        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
