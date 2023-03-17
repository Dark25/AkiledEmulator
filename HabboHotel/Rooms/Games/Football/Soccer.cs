using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using System;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class Soccer
    {
        private Room room;
        private ConcurrentDictionary<int, Item> _balls;

        public Soccer(Room room)
        {
            this.room = room;
            _balls = new ConcurrentDictionary<int, Item>();
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
        internal bool OnCycle()
        {
            try
            {
                if (_balls.IsEmpty)
                    return false;

                foreach (var ball in _balls.Values)
                {

                    if (!ball.BallIsMoving || ball?.ballMover == null) return false;

                    new Task(async () =>
                    {
                        MoveBallProcess(ball, ball.ballMover);
                        
                        if (ball.ExtraData == "33")
                        {
                            await Task.Delay(100);
                            
                        } else if (ball.ExtraData == "22")
                        {
                            await Task.Delay(150);
                        }
                        else if (ball.ExtraData == "11")
                        {
                            await Task.Delay(400);
                        }
                    }).Start();
                }
                
            }
            catch (Exception e)
            {
                Logging.HandleException(e, "Ball - OnCycle");
                return false;
            }
            return true;
        }

        internal void OnUserWalk(RoomUser user)
        {
            if (user == null || _balls == null)
                return;

            foreach (var _ball in _balls.Values)
            {
                if (user.SetX == _ball.GetX && user.SetY == _ball.GetY && user.GoalX == _ball.GetX &&
                    user.GoalY == _ball.GetY && user.HandelingBallStatus == 0) // super chute.
                {
                    var userPoint = new Point(user.X, user.Y);
                    _ball.ExtraData = "55";
                    _ball.BallIsMoving = true;
                    _ball.BallValue = 1;
                    MoveBall(_ball, user.GetClient(), userPoint);
                }
                else if (user.SetX == _ball.GetX && user.SetY == _ball.GetY && user.GoalX == _ball.GetX && user.GoalY == _ball.GetY && user.HandelingBallStatus == 1) // super chute quando para de andar
                {
                    user.HandelingBallStatus = 0;
                    var _comeDirection = GetComeDirection(new Point(user.X, user.Y), _ball.Coordinate);
                    if (_comeDirection != MovementDirection.none)
                    {
                        int NewX = user.SetX;
                        int NewY = user.SetY;

                        GetNewCoords(_comeDirection, ref NewX, ref NewY);
                        if (_ball.GetRoom().GetGameMap().ValidTile(NewX, NewY))
                        {
                            Point userPoint = new Point(user.X, user.Y);
                            _ball.ExtraData = "55";
                            _ball.BallIsMoving = true;
                            _ball.BallValue = 1;
                            _ball.ballMover = user.GetClient();
                            MoveBall(_ball, user.GetClient(), userPoint);
                        }
                    }
                }
                else if (user.X == _ball.GetX && user.Y == _ball.GetY && user.HandelingBallStatus == 0)
                {
                    var userPoint = new Point(user.SetX, user.SetY);
                    _ball.ExtraData = "55";
                    _ball.BallIsMoving = true;
                    _ball.BallValue = 1;
                    MoveBall(_ball, user.GetClient(), userPoint);
                }
                else
                {
                    if (user.HandelingBallStatus == 0 && user.GoalX == _ball.GetX && user.GoalY == _ball.GetY)
                        return;

                    if (user.SetX == _ball.GetX && user.SetY == _ball.GetY && user.IsWalking &&
                        (user.X != user.GoalX || user.Y != user.GoalY))
                    {
                        user.HandelingBallStatus = 1;
                        var comeDirection =
                            GetComeDirection(new Point(user.X, user.Y), _ball.Coordinate);

                        if (comeDirection == MovementDirection.none)
                            return;

                        var newX = user.SetX;
                        var newY = user.SetY;

                        if (!_ball.GetRoom().GetGameMap().ValidTile(user.SetX, user.SetY) ||
                            !_ball.GetRoom().GetGameMap().CanPlaceItem(user.SetX, user.SetY))
                        {
                            comeDirection = InverseDirections(room, comeDirection, user.X, user.Y);
                            newX = _ball.GetX;
                            newY = _ball.GetY;
                        }

                        GetNewCoords(comeDirection, ref newX, ref newY);
                        _ball.ExtraData = "11";
                        //                        if (!_room.GetGameMap().ItemCanBePlacedHere(newX, newY))
                        //                            return;
                        MoveBall(_ball, user.GetClient(), newX, newY);
                    }
                }
            }
        }

        private static bool VerifyBall(RoomUser user, int actualx, int actualy) => Rotation.Calculate(user.X, user.Y, actualx, actualy) == user.RotBody;


        internal MovementDirection GetComeDirection(Point user, Point ball)
        {
            try
            {
                if (user.X == ball.X && user.Y - 1 == ball.Y)
                    return MovementDirection.down;
                else if (user.X + 1 == ball.X && user.Y - 1 == ball.Y)
                    return MovementDirection.downleft;
                else if (user.X + 1 == ball.X && user.Y == ball.Y)
                    return MovementDirection.left;
                else if (user.X + 1 == ball.X && user.Y + 1 == ball.Y)
                    return MovementDirection.upleft;
                else if (user.X == ball.X && user.Y + 1 == ball.Y)
                    return MovementDirection.up;
                else if (user.X - 1 == ball.X && user.Y + 1 == ball.Y)
                    return MovementDirection.upright;
                else if (user.X - 1 == ball.X && user.Y == ball.Y)
                    return MovementDirection.right;
                else if (user.X - 1 == ball.X && user.Y - 1 == ball.Y)
                    return MovementDirection.downright;
                else
                    return MovementDirection.none;
            }
            catch
            {
                return MovementDirection.none;
            }
        }

        public void MoveBall(Item item, GameClient client, Point user, bool skip = false)
        {
            try
            {
                item.MovementDir = GetComeDirection(user, item.Coordinate);
                
                if (item.MovementDir != MovementDirection.none)
                    MoveBallProcess(item, client);
            }
            catch
            {
            }
        }


        internal static MovementDirection InverseDirections(Room room, MovementDirection comeWith, int x, int y)
        {
            try
            {
                if (comeWith == MovementDirection.up)
                {
                    return MovementDirection.down;
                }
                else if (comeWith == MovementDirection.upright)
                {
                    if (room.GetGameMap().Model.SqState[x,y] == SquareState.BLOCKED)
                        return room.GetGameMap().Model.SqState[x + 1,y] == SquareState.BLOCKED
                            ? MovementDirection.downright
                            : MovementDirection.upleft;
                    return MovementDirection.downright;
                }
                else if (comeWith == MovementDirection.right)
                {
                    return MovementDirection.left;
                }
                else if (comeWith == MovementDirection.downright)
                {
                    if (room.GetGameMap().Model.SqState[x,y] == SquareState.BLOCKED)
                        return room.GetGameMap().Model.SqState[x + 1,y] == SquareState.BLOCKED
                            ? MovementDirection.upright
                            : MovementDirection.downleft;
                    
                    return MovementDirection.upleft;

                }
                else if (comeWith == MovementDirection.down)
                {
                    return MovementDirection.up;
                }
                else if (comeWith == MovementDirection.downleft)
                {
                    return MovementDirection.upright;

                }
                else if (comeWith == MovementDirection.left)
                {
                    return MovementDirection.right;
                }
                else if (comeWith == MovementDirection.upleft)
                {
                    return MovementDirection.downright;

                }
                return MovementDirection.none;
            }
            catch
            {
                return MovementDirection.none;
            }
        }


        internal static void GetNewCoords(MovementDirection comeWith, ref int newX, ref int newY)
        {
            try
            {
                if (comeWith == MovementDirection.up)
                {
                    // newX = newX;
                    newY++;
                }
                else if (comeWith == MovementDirection.upright)
                {
                    newX--;
                    newY++;
                }
                else if (comeWith == MovementDirection.right)
                {
                    newX--;
                    // newY = newY;
                }
                else if (comeWith == MovementDirection.downright)
                {
                    newX--;
                    newY--;
                }
                else if (comeWith == MovementDirection.down)
                {
                    // newX = newX;
                    newY--;
                }
                else if (comeWith == MovementDirection.downleft)
                {
                    newX++;
                    newY--;
                }
                else if (comeWith == MovementDirection.left)
                {
                    newX++;
                    // newY = newY;
                }
                else if (comeWith == MovementDirection.upleft)
                {
                    newX++;
                    newY++;
                }
            }
            catch { }
        }


        internal void MoveBallProcess(Item item, GameClient client)
        {
            if (item == null) return;
            //            if (!_balls.Contains(item)) return;

            var tryes = 0;
            var newX = item.Coordinate.X;
            var newY = item.Coordinate.Y;

            if (item.ballMover == null || item.ballMover != client)
                item.ballMover = client;


            while (tryes < 3)
            {
                if (room?.GetGameMap() == null)
                    return;

                //                var total = item.ExtraData == "55" ? 6 : 1;
                //                for (var i = 0; i != total; i++)

                if (item.MovementDir == MovementDirection.none)
                {
                    item.BallIsMoving = false;
                    break;
                }

                var resetX = newX;
                var resetY = newY;

                GetNewCoords(item.MovementDir, ref newX, ref newY);

                var ignoreUsers = false;

                //                if (_room.GetGameMap().SquareHasFurni(newX, newY))
                //                {
                //                    client.SendWhisper("Has furni");
                //                    return;
                //                }
                if (room.GetGameMap().SquareHasUsers(newX, newY))
                {
                    if (item.ExtraData != "55" && item.ExtraData != "44")
                    {
                        item.BallIsMoving = false;
                        break;
                    }

                    ignoreUsers = true;
                }

                if (ignoreUsers == false)
                    if (!room.GetGameMap().CanPlaceItem(newX, newY))
                    {
                        item.MovementDir = InverseDirections(room, item.MovementDir, newX, newY);
                        newX = resetX;
                        newY = resetY;
                        tryes++;
                        if (tryes > 2)
                            item.BallIsMoving = false;
                        continue;
                    }

                if (MoveBall(item, client, newX, newY))
                {
                    item.BallIsMoving = false;
                    break;
                }

                int.TryParse(item.ExtraData, out var number);
                if (number > 11)
                    item.ExtraData = (number - 11).ToString();

                item.BallValue++;

                if (item.BallValue > 6)
                {
                    item.BallIsMoving = false;
                    item.BallValue = 1;
                    item.ballMover = null;
                }

                break;
                //break;
            }
        }

        public bool MoveBall(Item item, GameClient mover, int newX, int newY)
        {
            if (item == null || item.GetBaseItem() == null /*|| mover == null || mover.GetHabbo() == null*/)
                return false;

            if (!room.GetGameMap().CanPlaceItem(newX, newY))
                return false;

            var oldRoomCoord = item.Coordinate;
 
            double newZ = room.GetGameMap().Model.SqFloorHeight[newX, newY];

            if (item.GetBaseItem().IsSeat)
                newZ += 0.35;
            
            room.SendPacket(new ObjectUpdateComposer.UpdateFootBallComposer(item, newX, newY));

          //  this.room.GetRoomItemHandler().PositionReset(item, newX, newY, newZ);

            
            if (oldRoomCoord.X == newX && oldRoomCoord.Y == newY)
                return false;

            item.SetState(newX, newY, newZ, Gamemap.GetAffectedTiles(item.GetBaseItem().Length, item.GetBaseItem().Width, newX, newY, item.Rotation));

//            item.UpdateState(false, true);

            this.HandleFootballGameItems(new Point(newX, newY));

            return false;
        }


        public void Destroy()
        {
            this.room = (Room)null;
            _balls.Clear();
            _balls = null;
        }

        public void AddBall(Item item)
        {
            this._balls[item.Id] = item;
        }

        public void RemoveBall(int itemID)
        {
            this._balls.TryRemove(itemID, out _);
        }
    }
}