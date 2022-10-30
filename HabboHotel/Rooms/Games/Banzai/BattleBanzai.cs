using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Enclosure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class BattleBanzai
    {
        private Room room;
        public Dictionary<int, Item> banzaiTiles;
        private bool banzaiStarted;
        private byte[,] floorMap;
        private GameField field;
        private int TilesUsed;

        public bool isBanzaiActive
        {
            get
            {
                return this.banzaiStarted;
            }
        }

        public BattleBanzai(Room room)
        {
            this.room = room;
            this.banzaiTiles = new Dictionary<int, Item>();
            this.banzaiStarted = false;
            this.TilesUsed = 0;
        }

        public void AddTile(Item item, int itemID)
        {
            if (this.banzaiTiles.ContainsKey(itemID)) return;

            this.banzaiTiles.Add(itemID, item);
        }

        public void RemoveTile(int itemID)
        {
            this.banzaiTiles.Remove(itemID);
        }

        public void OnUserWalk(RoomUser User)
        {
            if (User == null)
                return;

            List<Item> roomItemForSquare = this.room.GetGameMap().GetCoordinatedItems(new Point(User.SetX, User.SetY));

            foreach (Item Ball in roomItemForSquare)
            {
                if (Ball.GetBaseItem().InteractionType != InteractionType.banzaipuck)
                    continue;

                int Lenght = 1;
                int GoalX = Ball.GetX;
                int GoalY = Ball.GetY;

                switch (User.RotBody)
                {
                    case 0:
                        GoalX = Ball.GetX;
                        GoalY = Ball.GetY - Lenght;
                        break;
                    case 1:
                        GoalX = Ball.GetX + Lenght;
                        GoalY = Ball.GetY - Lenght;
                        break;
                    case 2:
                        GoalX = Ball.GetX + Lenght;
                        GoalY = Ball.GetY;
                        break;
                    case 3:
                        GoalX = Ball.GetX + Lenght;
                        GoalY = Ball.GetY + Lenght;
                        break;
                    case 4:
                        GoalX = Ball.GetX;
                        GoalY = Ball.GetY + Lenght;
                        break;
                    case 5:
                        GoalX = Ball.GetX - Lenght;
                        GoalY = Ball.GetY + Lenght;
                        break;
                    case 6:
                        GoalX = Ball.GetX - Lenght;
                        GoalY = Ball.GetY;
                        break;
                    case 7:
                        GoalX = Ball.GetX - Lenght;
                        GoalY = Ball.GetY - Lenght;
                        break;
                }

                if (!this.room.GetGameMap().CanStackItem(GoalX, GoalY))
                {
                    switch (User.RotBody)
                    {
                        case 0:
                            GoalX = Ball.GetX;
                            GoalY = Ball.GetY + Lenght;
                            break;
                        case 1:
                            GoalX = Ball.GetX - Lenght;
                            GoalY = Ball.GetY + Lenght;
                            break;
                        case 2:
                            GoalX = Ball.GetX - Lenght;
                            GoalY = Ball.GetY;
                            break;
                        case 3:
                            GoalX = Ball.GetX - Lenght;
                            GoalY = Ball.GetY - Lenght;
                            break;
                        case 4:
                            GoalX = Ball.GetX;
                            GoalY = Ball.GetY - Lenght;
                            break;
                        case 5:
                            GoalX = Ball.GetX + Lenght;
                            GoalY = Ball.GetY - Lenght;
                            break;
                        case 6:
                            GoalX = Ball.GetX + Lenght;
                            GoalY = Ball.GetY;
                            break;
                        case 7:
                            GoalX = Ball.GetX + Lenght;
                            GoalY = Ball.GetY + Lenght;
                            break;
                    }
                }
                this.MovePuck(Ball, User.GetClient(), GoalX, GoalY, User.Team);
                break;
            }
        }

        public void BanzaiStart()
        {
            if (this.banzaiStarted)
                return;

            this.banzaiStarted = true;

            this.room.GetGameItemHandler().ResetAllBlob();
            this.room.GetGameManager().Reset();
            this.floorMap = new byte[this.room.GetGameMap().Model.MapSizeY, this.room.GetGameMap().Model.MapSizeX];
            this.field = new GameField(this.floorMap, true);

            for (int index = 1; index < 5; ++index)
                this.room.GetGameManager().Points[index] = 0;

            foreach (Item roomItem in (IEnumerable)this.banzaiTiles.Values)
            {
                roomItem.ExtraData = "1";
                roomItem.value = 0;
                roomItem.team = Team.none;
                roomItem.UpdateState();
            }

            this.TilesUsed = 0;
        }

        public void BanzaiEnd()
        {
            if (!this.banzaiStarted)
                return;

            this.banzaiStarted = false;

            //this.floorMap = (byte[,])null;
            this.field.destroy();

            if (this.banzaiTiles.Count == 0)
                return;

            Team winningTeam = this.room.GetGameManager().getWinningTeam();

            foreach (RoomUser user in this.room.GetTeamManager().GetAllPlayer())
                this.EndGame(user, winningTeam);
        }

        private void EndGame(RoomUser roomUser, Team winningTeam)
        {
            if (roomUser.Team == winningTeam && winningTeam != Team.none)
            {
                this.room.SendPacket(new ActionMessageComposer(roomUser.VirtualId, 1));
            }
            else if (roomUser.Team != Team.none)
            {
                Item FirstTile = this.GetFirstTile(roomUser.X, roomUser.Y);

                if (FirstTile == null)
                {
                    return;
                }

                if (this.room.GetGameItemHandler().GetExitTeleport() != null)
                    this.room.GetGameMap().TeleportToItem(roomUser, this.room.GetGameItemHandler().GetExitTeleport());

                TeamManager managerForBanzai = roomUser.GetClient().GetHabbo().CurrentRoom.GetTeamManager();
                managerForBanzai.OnUserLeave(roomUser);
                this.room.GetGameManager().UpdateGatesTeamCounts();
                roomUser.ApplyEffect(0);
                roomUser.Team = Team.none;
            }
        }

        public void MovePuck(Item item, GameClient mover, int newX, int newY, Team team)
        {
            if (item == null || mover == null || !this.room.GetGameMap().CanStackItem(newX, newY))
                return;

            item.ExtraData = (team).ToString();
            item.UpdateState();


            double newZ = (double)this.room.GetGameMap().SqAbsoluteHeight(newX, newY);
            if (this.room.GetRoomItemHandler().SetFloorItem(item, newX, newY, newZ))
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
                Message.WriteInteger(item.Coordinate.X);
                Message.WriteInteger(item.Coordinate.Y);
                Message.WriteInteger(newX);
                Message.WriteInteger(newY);
                Message.WriteInteger(1);
                Message.WriteInteger(item.Id);
                Message.WriteString(item.GetZ.ToString().Replace(',', '.'));
                Message.WriteString(newZ.ToString().Replace(',', '.'));
                Message.WriteInteger(0);
                this.room.SendPacket(Message);
            }

            if (!this.banzaiStarted) return;

            this.HandleBanzaiTiles(new Point(newX, newY), team, this.room.GetRoomUserManager().GetRoomUserByHabboId(mover.GetHabbo().Id));
        }

        private void SetTile(Item item, Team team, RoomUser user)
        {
            if (item.team == team)
            {
                if (item.value < 3)
                {
                    ++item.value;
                    if (item.value == 3)
                    {
                        this.room.GetGameManager().AddPointToTeam(item.team, user);
                        this.field.updateLocation(item.GetX, item.GetY, (byte)team);
                        foreach (PointField pointField in this.field.doUpdate(false))
                        {
                            Team team1 = (Team)pointField.forValue;
                            foreach (Point point in pointField.getPoints())
                            {
                                if (this.floorMap[point.Y, point.X] == (byte)team1)
                                    continue;
                                this.HandleMaxBanzaiTiles(new Point(point.X, point.Y), team1, user, (Team)this.floorMap[point.Y, point.X]);
                                this.floorMap[point.Y, point.X] = pointField.forValue;
                            }
                        }
                        this.TilesUsed++;
                    }
                }
            }
            else if (item.value < 3)
            {
                item.team = team;
                item.value = 1;
            }
            int num = item.value + (int)item.team * 3 - 1;
            item.ExtraData = num.ToString();
        }

        private Item GetFirstTile(int x, int y)
        {
            foreach (Item roomItem in this.room.GetGameMap().GetCoordinatedItems(new Point(x, y)))
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.banzaifloor)
                    return roomItem;
            }
            return (Item)null;
        }

        public void HandleBanzaiTiles(Point coord, Team team, RoomUser user)
        {
            if (!this.banzaiStarted || team == Team.none || this.banzaiTiles.Count == 0)
                return;

            Item roomItem = GetFirstTile(coord.X, coord.Y);
            if (roomItem == null)
                return;

            if (roomItem.value == 3)
                return;

            this.SetTile(roomItem, team, user);
            roomItem.UpdateState(false, true);

            if (this.TilesUsed != this.banzaiTiles.Count)
                return;
            this.BanzaiEnd();
        }

        private void HandleMaxBanzaiTiles(Point coord, Team team, RoomUser user, Team oldteam)
        {
            if (team == Team.none)
                return;
            Item roomItem = this.GetFirstTile(coord.X, coord.Y);
            if (roomItem == null)
                return;
            if (roomItem.value != 3)
                this.TilesUsed++;

            SetMaxForTile(roomItem, team);
            this.room.GetGameManager().AddPointToTeam(team, user);
            this.room.GetGameManager().AddPointToTeam(oldteam, -1, user);
            roomItem.UpdateState(false, true);

        }

        private static void SetMaxForTile(Item item, Team team)
        {
            if (item.value < 3)
                item.value = 3;
            item.team = team;
            int num = item.value + (int)item.team * 3 - 1;
            item.ExtraData = num.ToString();
        }

        public void Destroy()
        {
            this.banzaiTiles.Clear();
            //this.pucks.Clear();
            Array.Clear((Array)this.floorMap, 0, this.floorMap.Length);
            this.field.destroy();
            this.room = (Room)null;
            this.banzaiTiles = null;
            //this.pucks = (QueuedDictionary<int, RoomItem>) null;
            this.floorMap = (byte[,])null;
            this.field = (GameField)null;
        }
    }
}
