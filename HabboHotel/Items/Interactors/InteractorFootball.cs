using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.Map.Movement;
using System.Drawing;

namespace Akiled.HabboHotel.Items.Interactors
{
    public class InteractorFootball : FurniInteractor
    {
        public override void OnPlace(GameClient Session, Item Item)
        {
        }

        public override void OnRemove(GameClient Session, Item Item)
        {
        }

        public override void OnTick(Item item) => throw new System.NotImplementedException();

        public override void OnTrigger(GameClient Session, Item Ball, int Request, bool UserHasRights)
        {
            if (Session == null)
                return;

            RoomUser User = Ball.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            if (User == null || Ball.GetRoom().OldFoot)
                return;

            bool TropLoin = true;
            bool Deloin = false;

            int differenceX = User.SetX - Ball.GetX;
            int differenceY = User.SetY - Ball.GetY;

            if (differenceX <= 1 && differenceX >= -1 && differenceY <= 1 && differenceY >= -1)
            {
                TropLoin = false;
            }

            int differenceX2 = User.X - Ball.GetX;
            int differenceY2 = User.Y - Ball.GetY;

            if (differenceX2 <= 1 && differenceX2 >= -1 && differenceY2 <= 1 && differenceY2 >= -1)
            {
                TropLoin = false;
            }

            int differenceX3 = User.GoalX - Ball.GetX;
            int differenceY3 = User.GoalY - Ball.GetY;

            if (differenceX3 > 1 || differenceX3 < -1 || differenceY3 > 1 || differenceY3 < -1)
            {
                Deloin = true;
            }

            if (TropLoin)
                return;

            switch (User.RotHead)
            {
                case 0:
                    Ball.MovementDir = MovementDirection.up;
                    break;
                case 1:
                    Ball.MovementDir = MovementDirection.upright;
                    break;
                case 2:
                    Ball.MovementDir = MovementDirection.right;
                    break;
                case 3:
                    Ball.MovementDir = MovementDirection.downright;
                    break;
                case 4:
                    Ball.MovementDir = MovementDirection.down;
                    break;
                case 5:
                    Ball.MovementDir = MovementDirection.downleft;
                    break;
                case 6:
                    Ball.MovementDir = MovementDirection.left;
                    break;
                case 7:
                    Ball.MovementDir = MovementDirection.upleft;
                    break;
            }


            int GoalX = Ball.GetX;
            int GoalY = Ball.GetY;

            Point NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);

            Ball.interactionCountHelper = 0;

            if (!Ball.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
            {
                Ball.GetNewDir(NewPoint.X, NewPoint.Y);
                NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);
            }
            else
            {
                Ball.GetRoom().GetSoccer().MoveBall(Ball, User.GetClient(), NewPoint);
            }

            if (!User.MoveWithBall && !Deloin && Ball.interactionCountHelper == 0 && !Ball.GetRoom().OldFoot)
            {
                Ball.interactionCountHelper = 2;
                Ball.InteractingUser = User.VirtualId;
                Ball.ReqUpdate(1);
            }
        }
        public override void OnTrigger2(GameClient Session, Item Ball, int Request)
        {
        }
    }
}
