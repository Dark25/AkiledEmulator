using Akiled.Communication.Packets.Outgoing;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Games;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.Wired;
using Microsoft.VisualBasic;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Akiled.HabboHotel.Rooms
{
    public class Gamemap
    {
        private Room room;
        private readonly ConcurrentDictionary<Point, List<RoomUser>> userMap;
        public bool DiagonalEnabled;
        public bool ObliqueDisable;
        public ServerPacket SerializedFloormap;

        public DynamicRoomModel Model;

        public byte[,] EffectMap { get; private set; }

        public byte[,] GameMap { get; private set; }

        public double[,] ItemHeightMap { get; private set; }

        public byte[,] mUserOnMap { get; private set; }

        public byte[,] mSquareTaking { get; private set; }
        public ConcurrentDictionary<Point, List<Item>> CoordinatedItems { get; private set; }

        public Gamemap(Room room)
        {
            this.room = room;
            this.ObliqueDisable = true;
            this.DiagonalEnabled = true;
            RoomModel mStaticModel = AkiledEnvironment.GetGame().GetRoomManager().GetModel(room.RoomData.ModelName, room.Id);
            if (mStaticModel == null)
                throw new Exception("No modeldata found for roomID " + room.Id);

            this.Model = new DynamicRoomModel(mStaticModel);

            this.CoordinatedItems = new ConcurrentDictionary<Point, List<Item>>();
            this.userMap = new ConcurrentDictionary<Point, List<RoomUser>>();

            this.GameMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.mUserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.mSquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
            this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
        }

        public void AddUserToMap(RoomUser user, Point coord)
        {
            if (this.userMap.ContainsKey(coord))
                this.userMap[coord].Add(user);
            else
            {
                this.userMap.TryAdd(coord, new List<RoomUser>() { user });
            }
            if (this.ValidTile(coord.X, coord.Y))
                this.mUserOnMap[coord.X, coord.Y] = 1;
        }

        public void TeleportToItem(RoomUser user, Item item)
        {
            if (user.Room != null)
                user.Room.SendPacket(user.Room.GetRoomItemHandler().TeleportUser(user, item.Coordinate, 0, item.GetZ)); //user.mRoom.GetGameMap().SqAbsoluteHeight(item.GetX, item.GetY)

            item.GetRoom().GetRoomUserManager().UpdateUserStatus(user, false);
        }

        public void UpdateUserMovement(Point oldCoord, Point newCoord, RoomUser user)
        {
            if (user.IsDispose)
                return;

            this.RemoveTakingSquare(user.SetX, user.SetY);
            this.RemoveUserFromMap(user, oldCoord);
            this.AddUserToMap(user, newCoord);
        }

        public void RemoveUserFromMap(RoomUser user, Point coord)
        {
            if (!this.userMap.ContainsKey(coord))
            {
                if (this.ValidTile(coord.X, coord.Y))
                    this.mUserOnMap[coord.X, coord.Y] = 0;
                return;
            }
            if (this.userMap[coord].Contains(user))
                this.userMap[coord].Remove(user);

            if (this.userMap[coord].Count > 0)
                return;

            List<RoomUser> UserList;
            this.userMap.TryRemove(coord, out UserList);

            if (this.ValidTile(coord.X, coord.Y))
                this.mUserOnMap[coord.X, coord.Y] = 0;
        }

        public void AddTakingSquare(int X, int Y)
        {
            if (this.ValidTile(X, Y))
                this.mSquareTaking[X, Y] = 1;
        }

        public void RemoveTakingSquare(int X, int Y)
        {
            if (this.ValidTile(X, Y))
                this.mSquareTaking[X, Y] = 0;
        }

        public bool SquareTakingOpen(int X, int Y)
        {
            if (!this.ValidTile(X, Y))
                return true;

            return this.room.RoomData.AllowWalkthrough || this.mSquareTaking[X, Y] == 0;
        }

        public List<RoomUser> GetRoomUsers(Point coord)
        {
            if (this.userMap.ContainsKey(coord))
                return this.userMap[coord];
            else
                return new List<RoomUser>();
        }

        public List<RoomUser> GetNearUsers(Point coord, int MaxDistance)
        {
            List<RoomUser> UsersNear = new List<RoomUser>();

            foreach (KeyValuePair<Point, List<RoomUser>> Users in this.userMap)
            {
                if (Math.Abs(Users.Key.X - coord.X) > MaxDistance || Math.Abs(Users.Key.Y - coord.Y) > MaxDistance)
                    continue;

                UsersNear.AddRange(Users.Value);
            }

            return UsersNear.OrderBy(u => !u.IsBot).ToList();
        }

        public Point getRandomWalkableSquare(int x, int y)
        {
            int rx = AkiledEnvironment.GetRandomNumber(x - 5, x + 5);
            int ry = AkiledEnvironment.GetRandomNumber(y - 5, y + 5);

            if (this.Model.DoorX == rx || this.Model.DoorY == ry || !this.CanWalk(rx, ry))
                return new Point(x, y);

            return new Point(rx, ry);
        }

        public Point WalkableSquareEvent(List<Point> SquareBloqued)
        {
            var walkableSquares = new List<Point>();
            for (int y = 0; y < this.Model.SqFloorHeight.GetUpperBound(1); y++)
            {
                for (int x = 0; x < this.Model.SqFloorHeight.GetUpperBound(0); x++)
                {
                    if (y <= 6 && x >= 11)
                        continue;

                    if (!SquareBloqued.Contains(new Point(x, y)) && this.Model.DoorX != x && this.Model.DoorY != y && this.GameMap[x, y] == 1)
                    {
                        walkableSquares.Add(new Point(x, y));
                    }
                }
            }

            int RandomNumber = AkiledEnvironment.GetRandomNumberMulti(0, walkableSquares.Count);
            int i = 0;
            Random Rand = new Random();
            foreach (Point coord in walkableSquares.ToList().OrderBy(x => Rand.Next()))
            {
                if (i == RandomNumber)
                    return coord;
                i++;
            }

            return new Point(0, 0);
        }

        public void AddToMap(Item item)
        {
            this.AddItemToMap(item);
        }

        private void SetDefaultValue(int x, int y)
        {
            if (!ValidTile(x, y))
                return;

            this.GameMap[x, y] = 0;
            this.EffectMap[x, y] = 0;
            this.ItemHeightMap[x, y] = 0.0;

            //if (x == this.Model.DoorX && y == this.Model.DoorY)
            //{
            //this.GameMap[x, y] = 3;
            //}
            //else 
            if (this.Model.SqState[x, y] == SquareState.OPEN)
            {
                this.GameMap[x, y] = 1;
            }
        }

        public void updateMapForItem(Item item)
        {
            foreach (Point Coord in item.GetCoords)
            {
                if (!this.ConstructMapForItem(item, Coord))
                    return;
            }
        }

        public void GenerateMaps(bool checkLines = true)
        {
            int MaxX = 0;
            int MaxY = 0;
            if (checkLines)
            {
                Item[] items = this.room.GetRoomItemHandler().GetFloor.ToArray();
                foreach (Item item in items.ToList())
                {
                    if (item == null)
                        continue;

                    foreach (ThreeDCoord Point in item.GetAffectedTiles.Values)
                    {
                        if (Point.X > MaxX)
                            MaxX = Point.X;

                        if (Point.Y > MaxY)
                            MaxY = Point.Y;
                    }
                }

                Array.Clear(items, 0, items.Length);
                items = null;
            }

            if (MaxY > (Model.MapSizeY - 1) || MaxX > (Model.MapSizeX - 1))
            {
                if (MaxX < Model.MapSizeX)
                    MaxX = Model.MapSizeX - 1;
                if (MaxY < Model.MapSizeY)
                    MaxY = Model.MapSizeY - 1;

                Model.SetMapsize(MaxX + 1, MaxY + 1);
            }

            this.CoordinatedItems.Clear();
            this.GameMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.mUserOnMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.mSquareTaking = new byte[this.Model.MapSizeX, this.Model.MapSizeY];
            this.ItemHeightMap = new double[this.Model.MapSizeX, this.Model.MapSizeY];
            this.EffectMap = new byte[this.Model.MapSizeX, this.Model.MapSizeY];

            for (int line = 0; line < this.Model.MapSizeY; ++line)
            {
                for (int chr = 0; chr < this.Model.MapSizeX; ++chr)
                {
                    this.GameMap[chr, line] = 0;
                    this.EffectMap[chr, line] = 0;

                    //if (chr == this.Model.DoorX && line == this.Model.DoorY)
                    //this.GameMap[chr, line] = 3;
                    //else 
                    if (this.Model.SqState[chr, line] == SquareState.OPEN)
                        this.GameMap[chr, line] = 1;
                }
            }

            foreach (Item Item in room.GetRoomItemHandler().GetFloor.ToArray())
            {
                if (!AddItemToMap(Item))
                    continue;
            }

            foreach (RoomUser user in room.GetRoomUserManager().GetUserList().ToList())
            {
                if (this.ValidTile(user.X, user.Y))
                    this.mUserOnMap[user.X, user.Y] = 1;
            }
        }

        private bool ConstructMapForItem(Item Item, Point Coord)
        {
            if (!this.ValidTile(Coord.X, Coord.Y))
            {
                return false;
            }
            else
            {
                if (this.ItemHeightMap[Coord.X, Coord.Y] <= Item.TotalHeight)
                {
                    if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.bed)
                    {
                        this.ItemHeightMap[Coord.X, Coord.Y] = Item.GetZ - (double)this.Model.SqFloorHeight[Item.GetX, Item.GetY];
                    }
                    else
                    {
                        this.ItemHeightMap[Coord.X, Coord.Y] = Item.TotalHeight - (double)this.Model.SqFloorHeight[Item.GetX, Item.GetY];
                    }

                    switch (Item.GetBaseItem().InteractionType)
                    {
                        case InteractionType.pool:
                            this.EffectMap[Coord.X, Coord.Y] = 1;
                            break;
                        case InteractionType.iceskates:
                            this.EffectMap[Coord.X, Coord.Y] = 3;
                            break;
                        case InteractionType.normslaskates:
                            this.EffectMap[Coord.X, Coord.Y] = 2;
                            break;
                        case InteractionType.lowpool:
                            this.EffectMap[Coord.X, Coord.Y] = 4;
                            break;
                        case InteractionType.haloweenpool:
                            this.EffectMap[Coord.X, Coord.Y] = 5;
                            break;
                        case InteractionType.TRAMPOLINE:
                            this.EffectMap[Coord.X, Coord.Y] = 7;
                            break;
                        case InteractionType.TREADMILL:
                            this.EffectMap[Coord.X, Coord.Y] = 8;
                            break;
                        case InteractionType.CROSSTRAINER:
                            this.EffectMap[Coord.X, Coord.Y] = 9;
                            break;
                        default:
                            this.EffectMap[Coord.X, Coord.Y] = 0;
                            break;
                    }

                    if (Item.GetBaseItem().InteractionType == InteractionType.freezetileblock && Item.ExtraData != "")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                            this.GameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().InteractionType == InteractionType.banzaipyramid && Item.ExtraData == "1")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                            this.GameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().InteractionType == InteractionType.GATE && Item.ExtraData == "1")
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                            this.GameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (Item.GetBaseItem().Walkable)
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                            this.GameMap[Coord.X, Coord.Y] = 1;
                    }
                    else if (!Item.GetBaseItem().Walkable && Item.GetBaseItem().Stackable)
                    {
                        if (this.GameMap[Coord.X, Coord.Y] != 3)
                            this.GameMap[Coord.X, Coord.Y] = 2;
                    }
                    else if (Item.GetBaseItem().IsSeat || Item.GetBaseItem().InteractionType == InteractionType.bed)
                        this.GameMap[Coord.X, Coord.Y] = 3;
                    else if (this.GameMap[Coord.X, Coord.Y] != 3)
                        this.GameMap[Coord.X, Coord.Y] = 0;
                }
            }
            return true;
        }

        public void AddCoordinatedItem(Item item, Point coord)
        {
            List<Item> list1 = new List<Item>();
            if (!this.CoordinatedItems.ContainsKey(coord))
            {
                this.CoordinatedItems.TryAdd(coord, new List<Item>() { item });
            }
            else
            {
                List<Item> list2 = this.CoordinatedItems[coord];
                if (list2.Contains(item))
                    return;
                list2.Add(item);
                this.CoordinatedItems[coord] = list2;
            }
        }

        public List<Item> GetCoordinatedItems(Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            if (this.CoordinatedItems.ContainsKey(point))
                return (List<Item>)this.CoordinatedItems[point];
            else
                return new List<Item>();
        }

        public bool RemoveCoordinatedItem(Item item, Point coord)
        {
            Point point = new Point(coord.X, coord.Y);
            if (!this.CoordinatedItems.ContainsKey(point) || !((List<Item>)this.CoordinatedItems[point]).Contains(item))
                return false;
            ((List<Item>)this.CoordinatedItems[point]).Remove(item);
            return true;
        }

        private void AddSpecialItems(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.GUILD_GATE:
                    this.room.GetGameItemHandler().AddGroupGate(item);
                    break;
                case InteractionType.banzaifloor:
                    this.room.GetBanzai().AddTile(item, item.Id);
                    break;
                case InteractionType.banzaitele:
                    this.room.GetGameItemHandler().AddTeleport(item, item.Id);
                    item.ExtraData = "";
                    break;
                //case InteractionType.banzaipuck:
                //this.room.GetBanzai().AddPuck(item);
                //break;
                case InteractionType.banzaipyramid:
                    this.room.GetGameItemHandler().AddPyramid(item, item.Id);
                    break;
                case InteractionType.banzaiblo:
                case InteractionType.banzaiblob:
                    this.room.GetGameItemHandler().AddBlob(item, item.Id);
                    break;
                case InteractionType.freezeexit:
                    this.room.GetGameItemHandler().AddExitTeleport(item);
                    break;
                case InteractionType.freezetileblock:
                    this.room.GetFreeze().AddFreezeBlock(item);
                    break;

                case InteractionType.football:
                    room.GetSoccer().AddBall(item);
                    break;
            }
        }

        private void RemoveSpecialItem(Item item)
        {
            switch (item.GetBaseItem().InteractionType)
            {
                case InteractionType.GUILD_GATE:
                    this.room.GetGameItemHandler().RemoveGroupGate(item);
                    break;
                case InteractionType.banzaifloor:
                    this.room.GetBanzai().RemoveTile(item.Id);
                    break;
                case InteractionType.banzaitele:
                    this.room.GetGameItemHandler().RemoveTeleport(item.Id);
                    break;
                //case InteractionType.banzaipuck:
                //this.room.GetBanzai().RemovePuck(item.Id);
                //break;
                case InteractionType.banzaipyramid:
                    this.room.GetGameItemHandler().RemovePyramid(item.Id);
                    break;
                case InteractionType.banzaiblo:
                case InteractionType.banzaiblob:
                    this.room.GetGameItemHandler().RemoveBlob(item.Id);
                    break;
                case InteractionType.freezetileblock:
                    this.room.GetFreeze().RemoveFreezeBlock(item.Id);
                    break;
                case InteractionType.footballgoalgreen:
                case InteractionType.footballcountergreen:
                case InteractionType.banzaiscoregreen:
                case InteractionType.banzaigategreen:
                case InteractionType.freezegreencounter:
                case InteractionType.freezegreengate:
                    this.room.GetGameManager().RemoveFurnitureFromTeam(item, Team.green);
                    break;
                case InteractionType.footballgoalyellow:
                case InteractionType.footballcounteryellow:
                case InteractionType.banzaiscoreyellow:
                case InteractionType.banzaigateyellow:
                case InteractionType.freezeyellowcounter:
                case InteractionType.freezeyellowgate:
                    this.room.GetGameManager().RemoveFurnitureFromTeam(item, Team.yellow);
                    break;
                case InteractionType.footballgoalblue:
                case InteractionType.footballcounterblue:
                case InteractionType.banzaiscoreblue:
                case InteractionType.banzaigateblue:
                case InteractionType.freezebluecounter:
                case InteractionType.freezebluegate:
                    this.room.GetGameManager().RemoveFurnitureFromTeam(item, Team.blue);
                    break;
                case InteractionType.footballgoalred:
                case InteractionType.footballcounterred:
                case InteractionType.banzaiscorered:
                case InteractionType.banzaigatered:
                case InteractionType.freezeredcounter:
                case InteractionType.freezeredgate:
                    this.room.GetGameManager().RemoveFurnitureFromTeam(item, Team.red);
                    break;
                case InteractionType.freezeexit:
                    this.room.GetGameItemHandler().RemoveExitTeleport(item);
                    break;

                case InteractionType.football:
                    room.GetSoccer().RemoveBall(item.Id);
                    break;
            }
        }

        public bool RemoveFromMap(Item item)
        {
            if (this.room.GotWired() && WiredUtillity.TypeIsWired(item.GetBaseItem().InteractionType))
                this.room.GetWiredHandler().RemoveFurniture(item);

            this.RemoveSpecialItem(item);

            bool flag = false;
            foreach (Point coord in item.GetCoords)
            {
                if (this.RemoveCoordinatedItem(item, coord))
                    flag = true;
            }

            Dictionary<Point, List<Item>> NoDoublons = new Dictionary<Point, List<Item>>();
            foreach (Point Tile in item.GetCoords.ToList())
            {
                Point point = new Point(Tile.X, Tile.Y);
                if (this.CoordinatedItems.ContainsKey(point))
                {
                    List<Item> list = (List<Item>)this.CoordinatedItems[point];
                    if (!NoDoublons.ContainsKey(Tile))
                        NoDoublons.Add(Tile, list);
                }
                this.SetDefaultValue(Tile.X, Tile.Y);
            }

            foreach (Point Coord in NoDoublons.Keys.ToList())
            {
                if (!NoDoublons.ContainsKey(Coord))
                    continue;

                List<Item> SubItems = NoDoublons[Coord];
                foreach (Item roomItem in SubItems.ToList())
                    this.ConstructMapForItem(roomItem, Coord);

            }
            NoDoublons.Clear();
            NoDoublons = null;

            return flag;
        }

        public bool AddItemToMap(Item Item)
        {
            if (this.room.GotWired() && WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType))
                this.room.GetWiredHandler().AddFurniture(Item);

            this.AddSpecialItems(Item);

            switch (Item.GetBaseItem().InteractionType)
            {
                case InteractionType.roller:
                    if (!this.room.GetRoomItemHandler().GetRollers().Contains(Item))
                        this.room.GetRoomItemHandler().TryAddRoller(Item.Id, Item);
                    break;
                case InteractionType.footballgoalgreen:
                case InteractionType.footballcountergreen:
                case InteractionType.banzaiscoregreen:
                case InteractionType.banzaigategreen:
                case InteractionType.freezegreencounter:
                case InteractionType.freezegreengate:
                    this.room.GetGameManager().AddFurnitureToTeam(Item, Team.green);
                    break;
                case InteractionType.footballgoalyellow:
                case InteractionType.footballcounteryellow:
                case InteractionType.banzaiscoreyellow:
                case InteractionType.banzaigateyellow:
                case InteractionType.freezeyellowcounter:
                case InteractionType.freezeyellowgate:
                    this.room.GetGameManager().AddFurnitureToTeam(Item, Team.yellow);
                    break;
                case InteractionType.footballgoalblue:
                case InteractionType.footballcounterblue:
                case InteractionType.banzaiscoreblue:
                case InteractionType.banzaigateblue:
                case InteractionType.freezebluecounter:
                case InteractionType.freezebluegate:
                    this.room.GetGameManager().AddFurnitureToTeam(Item, Team.blue);
                    break;
                case InteractionType.footballgoalred:
                case InteractionType.footballcounterred:
                case InteractionType.banzaiscorered:
                case InteractionType.banzaigatered:
                case InteractionType.freezeredcounter:
                case InteractionType.freezeredgate:
                    this.room.GetGameManager().AddFurnitureToTeam(Item, Team.red);
                    break;
            }
            if (Item.GetBaseItem().Type != 's')
                return true;

            foreach (Point point in Item.GetCoords)
            {
                this.AddCoordinatedItem(Item, new Point(point.X, point.Y));
            }

            if (Item.GetBaseItem().InteractionType == InteractionType.football)
                return true;

            foreach (Point Coord in Item.GetCoords)
            {
                if (!this.ConstructMapForItem(Item, Coord))
                    return false;
            }
            return true;
        }

        public bool CanStackItem(int X, int Y, bool NoUser = false)
        {
            if (!this.ValidTile(X, Y))
                return false;
            else
                return (this.mUserOnMap[X, Y] == 0 || NoUser) && (this.GameMap[X, Y] == 1 || this.GameMap[X, Y] == 2);
        }

        public bool CanWalk(int X, int Y, bool Override = false)
        {
            if (!this.ValidTile(X, Y))
                return false;
            else
                return (this.room.RoomData.AllowWalkthrough || Override || this.mUserOnMap[X, Y] == 0) && CanWalkState(this.GameMap[X, Y], Override);
            ;
        }

        public bool CanWalkState(int X, int Y, bool Override)
        {
            if (!this.ValidTile(X, Y))
                return false;
            else
                return CanWalkState(this.GameMap[X, Y], Override);
        }

        public double GetHeightForSquareFromData(Point coord)
        {
            if (coord.X > this.Model.SqFloorHeight.GetUpperBound(0) || coord.Y > this.Model.SqFloorHeight.GetUpperBound(1) || coord.X < 0 || coord.Y < 0)
                return 1.0;
            else
                return (double)this.Model.SqFloorHeight[coord.X, coord.Y];
        }

        public static bool CanWalkState(byte pState, bool pOverride)
        {
            return pOverride || pState == 3 || pState == 1;
        }

        public bool ValidTile(int X, int Y)
        {
            if (X < 0 || Y < 0 || X >= Model.MapSizeX || Y >= Model.MapSizeY)
            {
                return false;
            }

            return true;
        }

        public double SqAbsoluteHeight(int X, int Y)
        {
            Point point = new Point(X, Y);
            if (!this.CoordinatedItems.ContainsKey(point))
                return (double)this.GetHeightForSquareFromData(point);

            List<Item> ItemsOnSquare = (List<Item>)this.CoordinatedItems[point];
            return this.SqAbsoluteHeight(X, Y, ItemsOnSquare);
        }

        public double SqAbsoluteHeight(int X, int Y, List<Item> ItemsOnSquare)
        {
            if (!this.ValidTile(X, Y))
                return 0.0;

            double HighestStack = 0.0;
            bool deduct = false;
            double deductable = 0.0;
            foreach (Item roomItem in ItemsOnSquare)
            {
                if (roomItem.TotalHeight > HighestStack)
                {
                    if (roomItem.GetBaseItem().IsSeat || roomItem.GetBaseItem().InteractionType == InteractionType.bed)
                    {
                        deduct = true;
                        deductable = roomItem.Height;
                    }
                    else
                        deduct = false;

                    HighestStack = roomItem.TotalHeight;
                }
            }
            double floorHeight = (double)this.Model.SqFloorHeight[X, Y];
            double stackHeight = HighestStack - (double)this.Model.SqFloorHeight[X, Y];
            if (deduct)
                stackHeight -= deductable;
            if (stackHeight < 0.0)
                stackHeight = 0.0;

            return floorHeight + stackHeight;
        }

        public static Dictionary<int, ThreeDCoord> GetAffectedTiles(int Length, int Width, int PosX, int PosY, int Rotation)
        {
            int num = 1;

            Dictionary<int, ThreeDCoord> PointList = new Dictionary<int, ThreeDCoord>();
            PointList.Add(0, new ThreeDCoord(PosX, PosY, 0));

            if (Length > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int z = 1; z < Length; z++)
                    {
                        PointList.Add(num++, new ThreeDCoord(PosX, PosY + z, z));

                        for (int index = 1; index < Width; index++)
                            PointList.Add(num++, new ThreeDCoord(PosX + index, PosY + z, (z < index) ? index : z));
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int z = 1; z < Length; z++)
                    {
                        PointList.Add(num++, new ThreeDCoord(PosX + z, PosY, z));
                        for (int index = 1; index < Width; index++)
                            PointList.Add(num++, new ThreeDCoord(PosX + z, PosY + index, (z < index) ? index : z));
                    }
                }
            }
            if (Width > 1)
            {
                if (Rotation == 0 || Rotation == 4)
                {
                    for (int z = 1; z < Width; z++)
                    {
                        PointList.Add(num++, new ThreeDCoord(PosX + z, PosY, z));
                        for (int index = 1; index < Length; index++)
                            PointList.Add(num++, new ThreeDCoord(PosX + z, PosY + index, (z < index) ? index : z));
                    }
                }
                else if (Rotation == 2 || Rotation == 6)
                {
                    for (int z = 1; z < Width; z++)
                    {
                        PointList.Add(num++, new ThreeDCoord(PosX, PosY + z, z));
                        for (int index = 1; index < Length; index++)
                            PointList.Add(num++, new ThreeDCoord(PosX + index, PosY + z, (z < index) ? index : z));
                    }
                }
            }
            return PointList;
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY)
        {
            List<Item> list = new List<Item>();
            Point point = new Point(pX, pY);
            if (this.CoordinatedItems.ContainsKey(point))
            {
                foreach (Item roomItem in (List<Item>)this.CoordinatedItems[point])
                {
                    if (roomItem.GetX == pX && roomItem.GetY == pY)
                        list.Add(roomItem);
                }
            }
            return list;
        }

        public List<Item> GetRoomItemForSquare(int pX, int pY, double minZ)
        {
            List<Item> list = new List<Item>();
            Point point = new Point(pX, pY);
            if (this.CoordinatedItems.ContainsKey(point))
            {
                foreach (Item roomItem in (List<Item>)this.CoordinatedItems[point])
                {
                    if (roomItem.GetZ > minZ && roomItem.GetX == pX && roomItem.GetY == pY)
                        list.Add(roomItem);
                }
            }
            return list;
        }

        public MovementState GetChasingMovement(int X, int Y, MovementState OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;

            for (int i = 1; i < 4; i++)
            {
                // Left
                if (i == 1 && !CanStackItem(X - i, Y))
                    moveToLeft = false;
                else if (moveToLeft && SquareHasUsers(X - i, Y))
                    return MovementState.left;

                // Right
                if (i == 1 && !CanStackItem(X + i, Y))
                    moveToRight = false;
                else if (moveToRight && SquareHasUsers(X + i, Y))
                    return MovementState.right;

                // Up
                if (i == 1 && !CanStackItem(X, Y - i))
                    moveToUp = false;
                else if (moveToUp && SquareHasUsers(X, Y - i))
                    return MovementState.up;

                // Down
                if (i == 1 && !CanStackItem(X, Y + i))
                    moveToDown = false;
                else if (moveToDown && SquareHasUsers(X, Y + i))
                    return MovementState.down;

                // Breaking bucle
                if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
                    return MovementState.none;
            }

            List<MovementState> movements = new List<MovementState>();
            if (moveToLeft && OldMouvement != MovementState.right)
                movements.Add(MovementState.left);
            if (moveToRight && OldMouvement != MovementState.left)
                movements.Add(MovementState.right);
            if (moveToUp && OldMouvement != MovementState.down)
                movements.Add(MovementState.up);
            if (moveToDown && OldMouvement != MovementState.up)
                movements.Add(MovementState.down);

            if (movements.Count > 0)
                return movements[new Random().Next(0, movements.Count)];
            else
            {
                if (moveToLeft && OldMouvement == MovementState.left)
                    return MovementState.left;
                if (moveToRight && OldMouvement == MovementState.right)
                    return MovementState.right;
                if (moveToUp && OldMouvement == MovementState.up)
                    return MovementState.up;
                if (moveToDown && OldMouvement == MovementState.down)
                    return MovementState.down;
            }

            List<MovementState> movements2 = new List<MovementState>();
            if (moveToLeft)
                movements2.Add(MovementState.left);
            if (moveToRight)
                movements2.Add(MovementState.right);
            if (moveToUp)
                movements2.Add(MovementState.up);
            if (moveToDown)
                movements2.Add(MovementState.down);

            if (movements2.Count > 0)
                return movements2[new Random().Next(0, movements2.Count)];

            return MovementState.none;
        }

        public MovementState GetEscapeMovement(int X, int Y, MovementState OldMouvement)
        {
            bool moveToLeft = true;
            bool moveToRight = true;
            bool moveToUp = true;
            bool moveToDown = true;

            for (int i = 1; i < 4; i++)
            {
                // Left
                if (i == 1 && !CanStackItem(X - i, Y))
                    moveToLeft = false;
                else if (moveToLeft && SquareHasUsers(X - i, Y))
                    moveToLeft = false;

                // Right
                if (i == 1 && !CanStackItem(X + i, Y))
                    moveToRight = false;
                else if (moveToRight && SquareHasUsers(X + i, Y))
                    moveToRight = false;

                // Up
                if (i == 1 && !CanStackItem(X, Y - i))
                    moveToUp = false;
                else if (moveToUp && SquareHasUsers(X, Y - i))
                    moveToUp = false;

                // Down
                if (i == 1 && !CanStackItem(X, Y + i))
                    moveToDown = false;
                else if (moveToDown && SquareHasUsers(X, Y + i))
                    moveToDown = false;

                // Breaking bucle
                if (i == 1 && !moveToLeft && !moveToRight && !moveToUp && !moveToDown)
                    return MovementState.none;
            }

            List<MovementState> movements = new List<MovementState>();
            if (moveToLeft && OldMouvement != MovementState.right)
                movements.Add(MovementState.left);
            if (moveToRight && OldMouvement != MovementState.left)
                movements.Add(MovementState.right);
            if (moveToUp && OldMouvement != MovementState.down)
                movements.Add(MovementState.up);
            if (moveToDown && OldMouvement != MovementState.up)
                movements.Add(MovementState.down);

            if (movements.Count > 0)
                return movements[new Random().Next(0, movements.Count)];
            else
            {
                if (moveToLeft && OldMouvement == MovementState.left)
                    return MovementState.left;
                if (moveToRight && OldMouvement == MovementState.right)
                    return MovementState.right;
                if (moveToUp && OldMouvement == MovementState.up)
                    return MovementState.up;
                if (moveToDown && OldMouvement == MovementState.down)
                    return MovementState.down;
            }

            List<MovementState> movements2 = new List<MovementState>();
            if (moveToLeft)
                movements2.Add(MovementState.left);
            if (moveToRight)
                movements2.Add(MovementState.right);
            if (moveToUp)
                movements2.Add(MovementState.up);
            if (moveToDown)
                movements2.Add(MovementState.down);

            if (movements2.Count > 0)
                return movements2[new Random().Next(0, movements2.Count)];

            return MovementState.none;
        }

        public RoomUser SquareHasUserNear(int X, int Y, int Distance = 0)
        {
            if (SquareHasUsers(X - 1, Y))
            {
                return room.GetRoomUserManager().GetUserForSquare(X - 1, Y);
            }
            else if (SquareHasUsers(X + 1, Y))
            {
                return room.GetRoomUserManager().GetUserForSquare(X + 1, Y);
            }
            else if (SquareHasUsers(X, Y - 1))
            {
                return room.GetRoomUserManager().GetUserForSquare(X, Y - 1);
            }
            else if (SquareHasUsers(X, Y + 1))
            {
                return room.GetRoomUserManager().GetUserForSquare(X, Y + 1);
            }

            return null;
        }


        public RoomUser LookHasUserNearNotBot(int X, int Y, int Distance = 0)
        {
            Distance++;
            if (room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X, Y - Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X, Y - Distance);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X, Y + Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X, Y + Distance);
            }
            //diago
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y - Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X - Distance, Y + Distance);
            }
            else if (room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y - Distance) != null)
            {
                return room.GetRoomUserManager().GetUserForSquareNotBot(X + Distance, Y + Distance);
            }

            return null;
        }

        public bool SquareHasUsers(int X, int Y)
        {
            if (!ValidTile(X, Y))
                return false;

            if (this.mUserOnMap[X, Y] == 0)
                return false;

            return true;
        }

        public static bool TilesTouching(Point p1, Point p2)
        {
            return TilesTouching(p1.X, p1.Y, p2.X, p2.Y);
        }

        public static bool TilesTouching(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) <= 1 && Math.Abs(Y1 - Y2) <= 1 || X1 == X2 && Y1 == Y2;
        }

        public static int TileDistance(int X1, int Y1, int X2, int Y2)
        {
            return Math.Abs(X1 - X2) + Math.Abs(Y1 - Y2);
        }

        public void Destroy()
        {
            this.userMap.Clear();
            this.Model.Destroy();
            this.CoordinatedItems.Clear();
            Array.Clear((Array)this.GameMap, 0, this.GameMap.Length);
            Array.Clear((Array)this.EffectMap, 0, this.EffectMap.Length);
            Array.Clear((Array)this.ItemHeightMap, 0, this.ItemHeightMap.Length);
            Array.Clear((Array)this.mUserOnMap, 0, this.mUserOnMap.Length);
            Array.Clear((Array)this.mSquareTaking, 0, this.mSquareTaking.Length);
        }

        public bool CanPlaceItem(int x, int y)
        {
            if (!ValidTile(x, y))
                return false;

            
            return GameMap[x, y] == 1;
        }
    }
}
