using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using System;
using System.Drawing;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorLoveLock : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
            Item.InteractingUser = 0;
            Item.InteractingUser2 = 0;
        }

        public override void OnTick(Item item) => throw new NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Item, int Request, bool UserHasRights)
        {
            RoomUser User = null;

            if (!UserHasRights)
                return;

            if (Session != null)
                User = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (User == null)
                return;

            if (Gamemap.TilesTouching(Item.GetX, Item.GetY, User.X, User.Y))
            {
                if (Item.ExtraData == null || Item.ExtraData.Length <= 1 || !Item.ExtraData.Contains(Convert.ToChar(5).ToString()))
                {
                    Point pointOne;
                    Point pointTwo;

                    switch (Item.Rotation)
                    {
                        case 0:
                        case 2:
                            pointOne = new Point(Item.GetX, Item.GetY + 1);
                            pointTwo = new Point(Item.GetX, Item.GetY - 1);
                            break;

                        case 4:
                        case 6:
                            pointOne = new Point(Item.GetX - 1, Item.GetY);
                            pointTwo = new Point(Item.GetX + 1, Item.GetY);
                            break;

                        default:
                            return;
                    }

                    RoomUser UserOne = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointOne.X, pointOne.Y);
                    RoomUser UserTwo = Item.GetRoom().GetRoomUserManager().GetUserForSquare(pointTwo.X, pointTwo.Y);

                    if (UserOne == null || UserTwo == null)
                        return;
                    else if (UserOne.GetClient() == null || UserTwo.GetClient() == null)
                        return;
                    else
                    {
                        UserOne.CanWalk = false;
                        UserTwo.CanWalk = false;

                        Item.InteractingUser = UserOne.GetClient().GetHabbo().Id;
                        Item.InteractingUser2 = UserTwo.GetClient().GetHabbo().Id;

                        UserOne.GetClient().SendPacket(new LoveLockDialogueMessageComposer(Item.Id));
                        UserTwo.GetClient().SendPacket(new LoveLockDialogueMessageComposer(Item.Id));
                    }


                }
                else
                    return;
            }
            else
            {
                User.MoveTo(Item.SquareInFront);
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
