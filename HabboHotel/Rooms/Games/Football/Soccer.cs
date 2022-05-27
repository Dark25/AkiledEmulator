using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.Map.Movement;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class Soccer
    {
        private Room room;

        public Soccer(Room room)
        {
            this.room = room;
        }

        public void HandleFootballGameItems(Point ballItemCoord)
        {
            foreach (Item roomItem in this.room.GetGameManager().GetItems(Team.red).Values)
            {
                foreach (ThreeDCoord threeDcoord in roomItem.GetAffectedTiles.Values)
                {
                    if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                    {
                        this.AddPointToScoreCounters(Team.red);
                        return;
                    }
                }
            }
            foreach (Item roomItem in this.room.GetGameManager().GetItems(Team.green).Values)
            {
                foreach (ThreeDCoord threeDcoord in roomItem.GetAffectedTiles.Values)
                {
                    if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                    {
                        this.AddPointToScoreCounters(Team.green);
                        return;
                    }
                }
            }
            foreach (Item roomItem in this.room.GetGameManager().GetItems(Team.blue).Values)
            {
                foreach (ThreeDCoord threeDcoord in roomItem.GetAffectedTiles.Values)
                {
                    if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                    {
                        this.AddPointToScoreCounters(Team.blue);
                        return;
                    }
                }
            }
            foreach (Item roomItem in this.room.GetGameManager().GetItems(Team.yellow).Values)
            {
                foreach (ThreeDCoord threeDcoord in roomItem.GetAffectedTiles.Values)
                {
                    if (threeDcoord.X == ballItemCoord.X && threeDcoord.Y == ballItemCoord.Y)
                    {
                        this.AddPointToScoreCounters(Team.yellow);
                        return;
                    }
                }
            }
        }

        private void AddPointToScoreCounters(Team team)
        {
            foreach (Item roomItem in this.room.GetGameManager().GetItems(team).Values)
            {
                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.footballcounterblue:
                    case InteractionType.footballcountergreen:
                    case InteractionType.footballcounterred:
                    case InteractionType.footballcounteryellow:
                        int num = 0;
                        if (!string.IsNullOrEmpty(roomItem.ExtraData))
                        {
                            try
                            {
                                num = int.Parse(roomItem.ExtraData);
                            }
                            catch
                            {
                            }
                        }
                        num++;
                        if (num >= 100)
                            num = 0;
                        roomItem.ExtraData = num.ToString();
                        roomItem.UpdateState(false, true);
                        break;
                }
            }
        }

        public void OnUserWalk(RoomUser User, bool Shoot)
        {
            if (User == null)
                return;

            if ((!User.AllowBall && Shoot) && !this.room.OldFoot)
            {
                User.AllowBall = true;
                User.MoveWithBall = false;
                return;
            }

            List<Item> roomItemForSquare = this.room.GetGameMap().GetCoordinatedItems(new Point(User.SetX, User.SetY));

            bool MoveBall = false;

            foreach (Item Ball in roomItemForSquare)
            {
                if (Ball.GetBaseItem().InteractionType != InteractionType.football)
                    continue;

                switch (User.RotBody)
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

                if (Shoot)
                {
                    Ball.interactionCountHelper = 6;
                    Ball.InteractingUser = User.VirtualId;
                    Ball.ReqUpdate(1);
                }
                else
                {
                    int GoalX = Ball.GetX;
                    int GoalY = Ball.GetY;

                    Point NewPoint = Ball.GetMoveCoord(GoalX, GoalY, 1);

                    Ball.interactionCountHelper = 0;

                    if (User.AllowBall && !User.MoveWithBall)
                    {
                        User.AllowBall = false;
                    }
                    else
                    {
                        User.AllowBall = true;
                    }

                    if (Ball.GetRoom().GetGameMap().CanStackItem(NewPoint.X, NewPoint.Y, true))
                        this.MoveBall(Ball, NewPoint.X, NewPoint.Y);
                }

                MoveBall = true;

                break;
            }

            //User.MoveWithBall = MoveBall;

            if (!MoveBall)
                User.SetMoveWithBall = true;
            else
                User.MoveWithBall = true;
        }

        public void MoveBall(Item item, int newX, int newY)
        {
            item.ExtraData = (item.value + 11).ToString();
            item.value++;
            if (item.value == 2)
                item.value = 0;

            item.UpdateState(false, true);

            double Z = this.room.GetGameMap().SqAbsoluteHeight(newX, newY);
            this.room.GetRoomItemHandler().PositionReset(item, newX, newY, Z);

            this.HandleFootballGameItems(new Point(newX, newY));
        }

        public void Destroy()
        {
            this.room = (Room)null;
        }
    }
}
