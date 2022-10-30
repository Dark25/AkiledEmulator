using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.HabboHotel.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace Akiled.HabboHotel.Rooms.Games
{
    public class Freeze
    {
        private Room room;
        private readonly Dictionary<int, Item> freezeBlocks;
        private Random rnd;
        private bool gameStarted;

        public Freeze(Room room)
        {
            this.room = room;
            this.freezeBlocks = new Dictionary<int, Item>();
            this.rnd = new Random();
            this.gameStarted = false;
        }

        public bool isGameActive
        {
            get
            {
                return this.gameStarted;
            }
        }

        public void StartGame()
        {
            if (this.gameStarted)
                return;
            this.gameStarted = true;
            this.CountTeamPoints();
            this.ResetGame();
        }

        public void StopGame()
        {
            if (!this.gameStarted)
                return;

            this.gameStarted = false;

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
                roomUser.ApplyEffect(0);
                TeamManager managerForFreeze = this.room.GetTeamManager();
                managerForFreeze.OnUserLeave(roomUser);

                this.room.GetGameManager().UpdateGatesTeamCounts();

                roomUser.Team = Team.none;

                if (this.room.GetGameItemHandler().GetExitTeleport() != null)
                    this.room.GetGameMap().TeleportToItem(roomUser, this.room.GetGameItemHandler().GetExitTeleport());

                roomUser.Freezed = false;
                roomUser.SetStep = false;
                roomUser.IsWalking = false;
                roomUser.UpdateNeeded = true;
                roomUser.FreezeLives = 0;
            }
        }

        public void CycleUser(RoomUser user)
        {
            if (user.Freezed)
            {
                user.FreezeCounter++;
                if (user.FreezeCounter > 10)
                {
                    user.Freezed = false;
                    user.FreezeCounter = 0;
                    ActivateShield(user);
                }
            }
            if (!user.ShieldActive)
                return;
            ++user.ShieldCounter;
            if (user.ShieldCounter <= 10)
                return;
            user.ShieldActive = false;
            user.ShieldCounter = 10;
            user.ApplyEffect(((int)user.Team + 39));
        }

        public void ResetGame()
        {
            foreach (Item roomItem in (IEnumerable)this.freezeBlocks.Values)
            {
                if (!string.IsNullOrEmpty(roomItem.ExtraData))
                {
                    roomItem.ExtraData = "";
                    roomItem.UpdateState(false, true);
                    this.room.GetGameMap().updateMapForItem(roomItem);
                }
            }
        }

        public void OnWalkFreezeBlock(Item roomitem, RoomUser user)
        {
            if (!this.gameStarted || user.Team == Team.none)
                return;
            if (roomitem.freezePowerUp != FreezePowerUp.None)
                this.PickUpPowerUp(roomitem, user);
        }

        private void CountTeamPoints()
        {
            foreach (RoomUser user in this.room.GetTeamManager().GetAllPlayer())
                this.CountTeamPoint(user);
        }

        private void CountTeamPoint(RoomUser roomUser)
        {
            if (roomUser == null || roomUser.GetClient() == null)
                return;

            Item FirstTile = this.GetFirstTile(roomUser.X, roomUser.Y);

            if (FirstTile == null)
            {
                EndGame(roomUser, Team.none);
                return;
            }

            roomUser.BanzaiPowerUp = FreezePowerUp.None;
            roomUser.FreezeLives = 3;
            roomUser.ShieldActive = false;
            roomUser.ShieldCounter = 11;

            this.room.GetGameManager().AddPointToTeam(roomUser.Team, 40, (RoomUser)null);

            ServerPacket Message = new ServerPacket(ServerPacketHeader.FreezeLivesComposer);
            Message.WriteInteger(roomUser.VirtualId);
            Message.WriteInteger(roomUser.FreezeLives);
            roomUser.GetClient().SendPacket(Message);
        }

        private static void RemoveUserFromTeam(RoomUser user)
        {
            user.Team = Team.none;
            user.ApplyEffect(0);
        }

        public void throwBall(Item Item, RoomUser roomUserByHabbo)
        {
            if (Math.Abs((roomUserByHabbo.X - Item.GetX)) >= 2 || Math.Abs((roomUserByHabbo.Y - Item.GetY)) >= 2)
                return;

            if (roomUserByHabbo.Team == Team.none || !this.gameStarted)
                return;

            Item Tile = this.GetFirstTile(Item.GetX, Item.GetY);

            if (Tile == null)
                return;

            if (Tile.interactionCountHelper == 0)
            {
                Tile.interactionCountHelper = (byte)1;
                Tile.ExtraData = "1000";
                Tile.UpdateState();
                Tile.InteractingUser = roomUserByHabbo.UserId;
                Tile.freezePowerUp = roomUserByHabbo.BanzaiPowerUp;
                Tile.ReqUpdate(5);
                roomUserByHabbo.CountFreezeBall -= 1;
                switch (roomUserByHabbo.BanzaiPowerUp)
                {
                    case FreezePowerUp.GreenArrow:
                    case FreezePowerUp.OrangeSnowball:
                    case FreezePowerUp.BlueArrow:
                        roomUserByHabbo.BanzaiPowerUp = FreezePowerUp.None;
                        break;
                }
            }
        }

        private Item GetFirstTile(int x, int y)
        {
            foreach (Item roomItem in this.room.GetGameMap().GetCoordinatedItems(new Point(x, y)))
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.freezetile)
                    return roomItem;
            }
            return (Item)null;
        }

        public void onFreezeTiles(Item item, FreezePowerUp powerUp, int userID)
        {
            if (this.room.GetRoomUserManager().GetRoomUserByHabboId(userID) == null)
                return;
            List<Item> items;
            switch (powerUp)
            {
                case FreezePowerUp.BlueArrow:
                    items = this.GetVerticalItems(item.GetX, item.GetY, 5);
                    break;
                case FreezePowerUp.GreenArrow:
                    items = this.GetDiagonalItems(item.GetX, item.GetY, 5);
                    break;
                case FreezePowerUp.OrangeSnowball:
                    items = this.GetVerticalItems(item.GetX, item.GetY, 5);
                    items.AddRange((IEnumerable<Item>)this.GetDiagonalItems(item.GetX, item.GetY, 5));
                    break;
                default:
                    items = this.GetVerticalItems(item.GetX, item.GetY, 3);
                    break;
            }
            this.HandleBanzaiFreezeItems(items);
        }

        private static void ActivateShield(RoomUser user)
        {
            user.ApplyEffect(((int)user.Team + 48));
            user.ShieldActive = true;
            user.ShieldCounter = 0;
        }

        private void HandleBanzaiFreezeItems(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                switch (roomItem.GetBaseItem().InteractionType)
                {
                    case InteractionType.freezetileblock:
                        this.SetRandomPowerUp(roomItem);
                        roomItem.UpdateState(false, true);
                        continue;
                    case InteractionType.freezetile:
                        roomItem.ExtraData = "11000";
                        roomItem.UpdateState(false, true);
                        continue;
                    default:
                        continue;
                }
            }
        }

        private void SetRandomPowerUp(Item item)
        {
            if (!string.IsNullOrEmpty(item.ExtraData))
                return;
            switch (this.rnd.Next(1, 14))
            {
                case 2:
                    item.ExtraData = "2000";
                    item.freezePowerUp = FreezePowerUp.BlueArrow;
                    break;
                case 3:
                    item.ExtraData = "3000";
                    item.freezePowerUp = FreezePowerUp.Snowballs;
                    break;
                case 4:
                    item.ExtraData = "4000";
                    item.freezePowerUp = FreezePowerUp.GreenArrow;
                    break;
                case 5:
                    item.ExtraData = "5000";
                    item.freezePowerUp = FreezePowerUp.OrangeSnowball;
                    break;
                case 6:
                    item.ExtraData = "6000";
                    item.freezePowerUp = FreezePowerUp.Heart;
                    break;
                case 7:
                    item.ExtraData = "7000";
                    item.freezePowerUp = FreezePowerUp.Shield;
                    break;
                default:
                    item.ExtraData = "1000";
                    item.freezePowerUp = FreezePowerUp.None;
                    break;
            }
            this.room.GetGameMap().updateMapForItem(item);
            item.UpdateState(false, true);
        }

        private void PickUpPowerUp(Item item, RoomUser user)
        {
            switch (item.freezePowerUp)
            {
                case FreezePowerUp.BlueArrow:
                case FreezePowerUp.GreenArrow:
                case FreezePowerUp.OrangeSnowball:
                    user.BanzaiPowerUp = item.freezePowerUp;
                    break;
                case FreezePowerUp.Shield:
                    ActivateShield(user);
                    break;
                case FreezePowerUp.Heart:
                    if (user.FreezeLives < 3)
                    {
                        ++user.FreezeLives;
                        this.room.GetGameManager().AddPointToTeam(user.Team, 10, user);
                    }
                    ServerPacket Message = new ServerPacket(ServerPacketHeader.FreezeLivesComposer);
                    Message.WriteInteger(user.VirtualId);
                    Message.WriteInteger(user.FreezeLives);
                    user.GetClient().SendPacket(Message);
                    break;
                case FreezePowerUp.Snowballs:
                    user.CountFreezeBall += 1;
                    break;
            }
            item.freezePowerUp = FreezePowerUp.None;
            item.ExtraData = "1" + item.ExtraData;
            item.UpdateState(false, true);
        }

        public void AddFreezeBlock(Item item)
        {
            if (this.freezeBlocks.ContainsKey(item.Id))
                this.freezeBlocks.Remove(item.Id);
            this.freezeBlocks.Add(item.Id, item);
        }

        public void RemoveFreezeBlock(int itemID)
        {
            this.freezeBlocks.Remove(itemID);
        }

        private void HandleUserFreeze(Point point)
        {
            RoomUser user = this.room.GetRoomUserManager().GetUserForSquare(point.X, point.Y);
            if (user == null || user.IsWalking && user.SetX != point.X && user.SetY != point.Y)
                return;
            this.FreezeUser(user);
        }

        private void FreezeUser(RoomUser user)
        {
            if (user.IsBot || user.ShieldActive || user.Team == Team.none || !this.gameStarted)
                return;

            if (user.Freezed)
            {
                user.Freezed = false;
                user.ApplyEffect(((int)user.Team + 39));
            }
            else
            {
                user.Freezed = true;
                user.FreezeCounter = 0;
                --user.FreezeLives;
                if (user.FreezeLives <= 0)
                {
                    ServerPacket Message = new ServerPacket(ServerPacketHeader.FreezeLivesComposer);
                    Message.WriteInteger(user.VirtualId);
                    Message.WriteInteger(user.FreezeLives);
                    user.GetClient().SendPacket(Message);

                    user.ApplyEffect(0);

                    this.room.GetGameManager().AddPointToTeam(user.Team, -20, user);

                    TeamManager managerForFreeze = this.room.GetTeamManager();
                    managerForFreeze.OnUserLeave(user);

                    this.room.GetGameManager().UpdateGatesTeamCounts();
                    user.Team = Team.none;

                    if (this.room.GetGameItemHandler().GetExitTeleport() != null)
                        this.room.GetGameMap().TeleportToItem(user, this.room.GetGameItemHandler().GetExitTeleport());

                    user.Freezed = false;
                    user.SetStep = false;
                    user.IsWalking = false;
                    user.UpdateNeeded = true;

                    if (managerForFreeze.BlueTeam.Count <= 0 && managerForFreeze.RedTeam.Count <= 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count > 0))
                        this.StopGame();
                    else if (managerForFreeze.BlueTeam.Count > 0 && managerForFreeze.RedTeam.Count <= 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count <= 0))
                        this.StopGame();
                    else if (managerForFreeze.BlueTeam.Count <= 0 && managerForFreeze.RedTeam.Count > 0 && (managerForFreeze.GreenTeam.Count <= 0 && managerForFreeze.YellowTeam.Count <= 0))
                        this.StopGame();
                    else
                    {
                        if (managerForFreeze.BlueTeam.Count > 0 || managerForFreeze.RedTeam.Count > 0 || (managerForFreeze.GreenTeam.Count <= 0 || managerForFreeze.YellowTeam.Count > 0))
                            return;
                        this.StopGame();
                    }
                }
                else
                {
                    this.room.GetGameManager().AddPointToTeam(user.Team, -10, user);
                    user.ApplyEffect(12);

                    ServerPacket Message = new ServerPacket(ServerPacketHeader.FreezeLivesComposer);
                    Message.WriteInteger(user.VirtualId);
                    Message.WriteInteger(user.FreezeLives);
                    user.GetClient().SendPacket(Message);
                }
            }
        }

        private List<Item> GetVerticalItems(int x, int y, int length)
        {
            List<Item> list = new List<Item>();
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x - index, y);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 1; index < length; ++index)
            {
                Point point = new Point(x, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            return list;
        }

        private List<Item> GetDiagonalItems(int x, int y, int length)
        {
            List<Item> list = new List<Item>();
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x - index, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x - index, y + index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            for (int index = 0; index < length; ++index)
            {
                Point point = new Point(x + index, y - index);
                List<Item> itemsForSquare = this.GetItemsForSquare(point);
                if (SquareGotFreezeTile(itemsForSquare))
                {
                    this.HandleUserFreeze(point);
                    list.AddRange((IEnumerable<Item>)itemsForSquare);
                    if (SquareGotFreezeBlock(itemsForSquare))
                        break;
                }
                else
                    break;
            }
            return list;
        }

        private List<Item> GetItemsForSquare(Point point)
        {
            return this.room.GetGameMap().GetCoordinatedItems(point);
        }

        private static bool SquareGotFreezeTile(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.freezetile)
                    return true;
            }
            return false;
        }

        private static bool SquareGotFreezeBlock(List<Item> items)
        {
            foreach (Item roomItem in items)
            {
                if (roomItem.GetBaseItem().InteractionType == InteractionType.freezetileblock)
                    return true;
            }
            return false;
        }

        public void Destroy()
        {
            this.freezeBlocks.Clear();
            this.room = (Room)null;
            this.rnd = (Random)null;
        }
    }
}
