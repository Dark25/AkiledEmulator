using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Core;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.GameClients;
using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms.Map.Movement;
using Akiled.HabboHotel.Rooms.Pathfinding;
using Akiled.HabboHotel.Rooms.Wired;
using Akiled.Utilities;
using AkiledEmulator.Database.Ext.Item;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;

namespace Akiled.HabboHotel.Rooms
{
    public class RoomItemHandling
    {
        private ConcurrentDictionary<int, Item> _floorItems;
        private ConcurrentDictionary<int, Item> _wallItems;
        private ConcurrentDictionary<int, Item> _rollers;

        private ConcurrentDictionary<int, ItemTemp> _itemsTemp;

        private ConcurrentDictionary<int, Item> _updateItems;

        private List<int> _rollerItemsMoved;
        private List<int> _rollerUsersMoved;
        private List<ServerPacket> _rollerMessages;

        private int _rollerSpeed;
        private int _rollerCycle;
        private ConcurrentQueue<Item> _roomItemUpdateQueue;
        private int _itemTempoId;

        private Room _room;

        public RoomItemHandling(Room room)
        {
            this._room = room;
            this._updateItems = new ConcurrentDictionary<int, Item>();
            this._rollers = new ConcurrentDictionary<int, Item>();
            this._wallItems = new ConcurrentDictionary<int, Item>();
            this._floorItems = new ConcurrentDictionary<int, Item>();
            this._itemsTemp = new ConcurrentDictionary<int, ItemTemp>();
            this._itemTempoId = 0;
            this._roomItemUpdateQueue = new ConcurrentQueue<Item>();
            this._rollerCycle = 0;
            this._rollerSpeed = 4;
            this._rollerItemsMoved = new List<int>();
            this._rollerUsersMoved = new List<int>();
            this._rollerMessages = new List<ServerPacket>();
        }

        public void QueueRoomItemUpdate(Item item) => this._roomItemUpdateQueue.Enqueue(item);

        public void ClearFurniture(GameClient Session)
        {
            foreach (Item roomItem in this._floorItems.Values.ToList())
            {
                roomItem.Interactor.OnRemove(Session, roomItem);
                roomItem.Destroy();
                if (Session.GetHabbo().HasFuse("room_item_take"))
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(Session, roomItem.Id);
                    Session.GetHabbo().GetInventoryComponent().AddItem(roomItem);
                    continue;
                }
                GameClient targetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(roomItem.OwnerId);
                if (targetClient != null && targetClient.GetHabbo() != null)//Again, do we have an active client?
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(targetClient, roomItem.Id);
                    if (targetClient.GetHabbo().GetInventoryComponent() != null)
                        targetClient.GetHabbo().GetInventoryComponent().AddItem(roomItem);
                }
                else//No, query time.
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(null, roomItem.Id);
                }
            }
            foreach (Item roomItem in this._wallItems.Values.ToList())
            {
                roomItem.Interactor.OnRemove(Session, roomItem);
                roomItem.Destroy();
                if (Session.GetHabbo().HasFuse("room_item_take"))
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(Session, roomItem.Id);
                    Session.GetHabbo().GetInventoryComponent().AddItem(roomItem);
                    continue;
                }
                GameClient targetClient = AkiledEnvironment.GetGame().GetClientManager().GetClientByUserID(roomItem.OwnerId);
                if (targetClient != null && targetClient.GetHabbo() != null)//Again, do we have an active client?
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(targetClient, roomItem.Id);
                    if (targetClient.GetHabbo().GetInventoryComponent() != null)
                        targetClient.GetHabbo().GetInventoryComponent().AddItem(roomItem);
                }
                else//No, query time.
                {
                    this._room.GetRoomItemHandler().RemoveFurniture(null, roomItem.Id);
                }
            }

            this._wallItems.Clear();
            this._floorItems.Clear();
            this._itemsTemp.Clear();
            this._updateItems.Clear();
            this._rollers.Clear();

            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE items SET room_id = '0' WHERE room_id = " + this._room.Id);

            this._room.GetGameMap().GenerateMaps();
            this._room.GetRoomUserManager().UpdateUserStatusses();
            if (this._room.GotWired())
                this._room.GetWiredHandler().OnPickall();
        }

        public List<Item> RemoveAllFurniture(GameClient Session)
        {
            List<ServerPacket> ListMessage = new List<ServerPacket>();
            List<Item> Items = new List<Item>();
            foreach (Item roomItem in this._floorItems.Values.ToList())
            {
                roomItem.Interactor.OnRemove(Session, roomItem);

                roomItem.Destroy();
                ListMessage.Add(new ObjectRemoveMessageComposer(roomItem.Id, roomItem.OwnerId));
                Items.Add(roomItem);
            }
            foreach (Item roomItem in this._wallItems.Values.ToList())
            {
                roomItem.Interactor.OnRemove(Session, roomItem);
                roomItem.Destroy();

                ServerPacket Message = new ServerPacket(ServerPacketHeader.ItemRemoveMessageComposer);
                Message.WriteString(roomItem.Id + string.Empty);
                Message.WriteInteger(roomItem.OwnerId);
                ListMessage.Add(Message);
                Items.Add(roomItem);
            }
            this._room.SendMessage(ListMessage);

            this._wallItems.Clear();
            this._floorItems.Clear();
            this._itemsTemp.Clear();
            this._updateItems.Clear();
            this._rollers.Clear();
            using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                queryreactor.RunQuery("UPDATE items SET room_id = '0', user_id = '" + this._room.RoomData.OwnerId + "' WHERE room_id = " + this._room.Id);

            this._room.GetGameMap().GenerateMaps();
            this._room.GetRoomUserManager().UpdateUserStatusses();
            if (this._room.GotWired())
                this._room.GetWiredHandler().OnPickall();

            return Items;
        }

        public void SetSpeed(int p)
        {
            this._rollerSpeed = p;
        }


        public void LoadFurniture(int RoomId = 0)
        {
            if (RoomId == 0)
            {
                this._floorItems.Clear();
                this._wallItems.Clear();
            }

            using IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
            queryreactor.SetQuery("SELECT items.id, items.user_id, items.room_id, items.base_item, items.extra_data, items.x, items.y, items.z, items.rot, items.wall_pos, items_limited.limited_number, items_limited.limited_stack, room_items_moodlight.enabled, room_items_moodlight.current_preset, room_items_moodlight.preset_one, room_items_moodlight.preset_two, room_items_moodlight.preset_three FROM items LEFT JOIN items_limited ON (items_limited.item_id = items.id) LEFT JOIN room_items_moodlight ON (room_items_moodlight.item_id = items.id) WHERE items.room_id = @roomid");
            queryreactor.AddParameter("roomid", (RoomId == 0) ? this._room.Id : RoomId);

            int itemID;
            int UserId;
            int baseID;
            string ExtraData;
            int x;
            int y;
            double z;
            sbyte n;
            string wallposs;
            int Limited;
            int LimitedTo;
            string wallCoord;

            bool moodlightEnabled;
            int moodlightCurrentPreset;
            string moodlightPresetOne;
            string moodlightPresetTwo;
            string moodlightPresetThree;

            foreach (DataRow dataRow in queryreactor.GetTable().Rows)
            {

                itemID = Convert.ToInt32(dataRow[0]);
                UserId = Convert.ToInt32(dataRow[1]);
                baseID = Convert.ToInt32(dataRow[3]);
                ExtraData = !DBNull.Value.Equals(dataRow[4]) ? (string)dataRow[4] : string.Empty;
                x = Convert.ToInt32(dataRow[5]);
                y = Convert.ToInt32(dataRow[6]);
                z = Convert.ToDouble(dataRow[7]);
                n = Convert.ToSByte(dataRow[8]);
                wallposs = !DBNull.Value.Equals(dataRow[9]) ? (string)(dataRow[9]) : string.Empty;
                Limited = !DBNull.Value.Equals(dataRow[10]) ? Convert.ToInt32(dataRow[10]) : 0;
                LimitedTo = !DBNull.Value.Equals(dataRow[11]) ? Convert.ToInt32(dataRow[11]) : 0;

                ItemData Data = null;
                AkiledEnvironment.GetGame().GetItemManager().GetItem(baseID, out Data);

                if (Data == null)
                    continue;

                if (Data.Type.ToString() == "i")
                {
                    if (string.IsNullOrEmpty(wallposs))
                        wallCoord = "w=0,0 l=0,0 l";
                    else
                        wallCoord = wallposs;

                    Item roomItem = new(itemID, UserId, this._room.Id, baseID, ExtraData, Limited, LimitedTo, 0, 0, 0.0, 0, wallCoord, this._room);
                    if (!this._wallItems.ContainsKey(itemID))
                        this._wallItems.TryAdd(itemID, roomItem);

                    if (roomItem.GetBaseItem().InteractionType == InteractionType.MOODLIGHT)
                    {

                        moodlightEnabled = !DBNull.Value.Equals(dataRow["enabled"]) && Convert.ToBoolean(dataRow["enabled"]);
                        moodlightCurrentPreset = !DBNull.Value.Equals(dataRow["current_preset"]) ? Convert.ToInt32(dataRow["current_preset"]) : 1;
                        moodlightPresetOne = !DBNull.Value.Equals(dataRow["preset_one"]) ? (string)dataRow["preset_one"] : "#000001,255,0";
                        moodlightPresetTwo = !DBNull.Value.Equals(dataRow["preset_two"]) ? (string)dataRow["preset_two"] : "#000000,255,0";
                        moodlightPresetThree = !DBNull.Value.Equals(dataRow["preset_three"]) ? (string)dataRow["preset_three"] : "#000000,255,0";

                        this._room.MoodlightData ??= new MoodlightData(roomItem.Id, moodlightEnabled, moodlightCurrentPreset, moodlightPresetOne, moodlightPresetTwo, moodlightPresetThree);

                    }
                }
                else //Is flooritem
                {
                    Item roomItem = new Item(itemID, UserId, this._room.Id, baseID, ExtraData, Limited, LimitedTo, x, y, (double)z, n, "", this._room);

                    if (!this._floorItems.ContainsKey(itemID))
                        this._floorItems.TryAdd(itemID, roomItem);
                }
            }

            if (RoomId == 0)
            {
                foreach (Item Item in _floorItems.Values)
                {
                    if (WiredUtillity.TypeIsWired(Item.GetBaseItem().InteractionType))
                    {
                        WiredLoader.LoadWiredItem(Item, this._room, queryreactor);
                    }
                }
            }
        }

        public ICollection<Item> GetFloor
        {
            get
            {
                return this._floorItems.Values;
            }
        }

        public ConcurrentDictionary<int, Item> GetFloorItems
        {
            get
            {
                return this._floorItems;
            }
        }


        public ItemTemp GetFirstTempDrop(int x, int y)
        {
            foreach (ItemTemp Item in _itemsTemp.Values)
            {
                if (Item.InteractionType != InteractionTypeTemp.RPITEM && Item.InteractionType != InteractionTypeTemp.MONEY)
                    continue;

                if (Item.X != x || Item.Y != y)
                    continue;


                return Item;
            }

            return null;
        }

        public ItemTemp GetTempItem(int pId)
        {
            if (_itemsTemp != null && _itemsTemp.ContainsKey(pId))
            {
                ItemTemp Item = null;
                if (_itemsTemp.TryGetValue(pId, out Item))
                    return Item;
            }

            return null;
        }

        public Item GetItem(int pId)
        {
            if (_floorItems != null && _floorItems.ContainsKey(pId))
            {
                Item Item = null;
                if (_floorItems.TryGetValue(pId, out Item))
                    return Item;
            }
            else if (_wallItems != null && _wallItems.ContainsKey(pId))
            {
                Item Item = null;
                if (_wallItems.TryGetValue(pId, out Item))
                    return Item;
            }

            return null;
        }

        public ICollection<ItemTemp> GetTempItems
        {
            get
            {
                return this._itemsTemp.Values;
            }
        }

        public ICollection<Item> GetWall
        {
            get
            {
                return this._wallItems.Values;
            }
        }

        public IEnumerable<Item> GetWallAndFloor
        {
            get
            {
                return this._floorItems.Values.Concat(this._wallItems.Values);
            }
        }

        public void RemoveFurniture(GameClient Session, int pId)
        {
            Item roomItem = this.GetItem(pId);
            if (roomItem == null)
                return;

            roomItem.Interactor.OnRemove(Session, roomItem);

            this.RemoveRoomItem(roomItem);

            if (roomItem.WiredHandler != null)
            {
                using (IQueryAdapter queryreactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    roomItem.WiredHandler.DeleteFromDatabase(queryreactor);
                }
                roomItem.WiredHandler.Dispose();
                this._room.GetWiredHandler().RemoveFurniture(roomItem);
                roomItem.WiredHandler = null;
            }
            roomItem.Destroy();
        }

        public void RemoveTempItem(int pId)
        {
            ItemTemp Item = this.GetTempItem(pId);
            if (Item == null)
                return;

            this._room.SendPacket(new ObjectRemoveMessageComposer(Item.Id, 0));
            this._itemsTemp.TryRemove(pId, out Item);
        }

        private void RemoveRoomItem(Item Item)
        {
            if (Item.IsWallItem)
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.ItemRemoveMessageComposer);
                Message.WriteString(Item.Id.ToString());
                Message.WriteInteger(Item.OwnerId);
                this._room.SendPacket(Message);
            }
            else if (Item.IsFloorItem)
            {
                this._room.SendPacket(new ObjectRemoveMessageComposer(Item.Id, Item.OwnerId));
            }


            if (Item.IsWallItem)
            {
                this._wallItems.TryRemove(Item.Id, out Item);
            }
            else
            {
                this._floorItems.TryRemove(Item.Id, out Item);
                this._room.GetGameMap().RemoveFromMap(Item);
            }

            if (this._updateItems.ContainsKey(Item.Id))
                this._updateItems.TryRemove(Item.Id, out Item);

            if (this._rollers.ContainsKey(Item.Id))
                this._rollers.TryRemove(Item.Id, out Item);

            foreach (ThreeDCoord threeDcoord in Item.GetAffectedTiles.Values)
            {
                List<RoomUser> userForSquare = this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y));
                if (userForSquare == null)
                    continue;
                foreach (RoomUser User in userForSquare)
                {
                    if (!User.IsWalking)
                        this._room.GetRoomUserManager().UpdateUserStatus(User, false);
                }
            }
        }

        private List<ServerPacket> CycleRollers()
        {
            if (this._rollerCycle >= this._rollerSpeed || this._rollerSpeed == 0)
            {
                this._rollerItemsMoved.Clear();
                this._rollerUsersMoved.Clear();
                this._rollerMessages.Clear();

                foreach (Item Roller in this._rollers.Values.ToList())
                {
                    Point NextSquare = Roller.SquareInFront;
                    List<Item> ItemsOnRoller = this._room.GetGameMap().GetRoomItemForSquare(Roller.GetX, Roller.GetY, Roller.GetZ);
                    RoomUser userForSquare = this._room.GetRoomUserManager().GetUserForSquare(Roller.GetX, Roller.GetY);

                    if (ItemsOnRoller.Count > 0 || userForSquare != null)
                    {

                        if (ItemsOnRoller.Count > 10)
                            ItemsOnRoller = ItemsOnRoller.Take(10).ToList();

                        List<Item> ItemsOnNext = this._room.GetGameMap().GetCoordinatedItems(NextSquare);
                        bool NextRoller = false;
                        double NextZ = 0.0;
                        bool NextRollerClear = true;
                        foreach (Item roomItem2 in ItemsOnNext)
                        {
                            if (roomItem2.IsRoller)
                            {
                                NextRoller = true;
                                if (roomItem2.TotalHeight > NextZ)
                                    NextZ = roomItem2.TotalHeight;
                            }
                        }
                        if (NextRoller)
                        {
                            foreach (Item roomItem2 in ItemsOnNext)
                            {
                                if (roomItem2.TotalHeight > NextZ)
                                    NextRollerClear = false;
                            }
                        }
                        else
                            NextZ += this._room.GetGameMap().GetHeightForSquareFromData(NextSquare);

                        foreach (Item pItem in ItemsOnRoller)
                        {
                            double RollerHeight = pItem.GetZ - Roller.TotalHeight;
                            if (!this._rollerItemsMoved.Contains(pItem.Id) && this._room.GetGameMap().CanStackItem(NextSquare.X, NextSquare.Y) && (NextRollerClear && Roller.GetZ < pItem.GetZ))
                            {
                                this._rollerMessages.Add(this.UpdateItemOnRoller(pItem, NextSquare, NextZ + RollerHeight));
                                this._rollerItemsMoved.Add(pItem.Id);
                            }
                        }

                        if (userForSquare != null && (!userForSquare.SetStep && (userForSquare.AllowMoveToRoller || this._rollerSpeed == 0) && (!userForSquare.IsWalking || userForSquare.Freeze)) && NextRollerClear && (this._room.GetGameMap().CanWalk(NextSquare.X, NextSquare.Y) && this._room.GetGameMap().SquareTakingOpen(NextSquare.X, NextSquare.Y) && !this._rollerUsersMoved.Contains(userForSquare.HabboId)))
                        {
                            this._rollerMessages.Add(this.UpdateUserOnRoller(userForSquare, NextSquare, Roller.Id, NextZ));
                            this._rollerUsersMoved.Add(userForSquare.HabboId);
                        }
                    }
                }
                this._rollerCycle = 0;
                return this._rollerMessages;
            }
            else
                ++this._rollerCycle;
            return new List<ServerPacket>();
        }

        public void PositionReset(Item pItem, int x, int y, double z)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            serverMessage.WriteInteger(pItem.GetX);
            serverMessage.WriteInteger(pItem.GetY);
            serverMessage.WriteInteger(x);
            serverMessage.WriteInteger(y);

            serverMessage.WriteInteger(1); //Count user or item on roller
            serverMessage.WriteInteger(pItem.Id);
            serverMessage.WriteString(pItem.GetZ.ToString());
            serverMessage.WriteString(z.ToString());

            serverMessage.WriteInteger(0);
            this._room.SendPacket(serverMessage);

            this.SetFloorItem(pItem, x, y, z);
        }

        public void RotReset(Item pItem, int newRot)
        {
            pItem.Rotation = newRot;

            _room.SendPacket(new ObjectUpdateComposer(pItem, pItem.OwnerId));
        }

        private ServerPacket UpdateItemOnRoller(Item pItem, Point NextCoord, double NextZ)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            serverMessage.WriteInteger(pItem.GetX);
            serverMessage.WriteInteger(pItem.GetY);
            serverMessage.WriteInteger(NextCoord.X);
            serverMessage.WriteInteger(NextCoord.Y);
            serverMessage.WriteInteger(1);
            serverMessage.WriteInteger(pItem.Id);
            serverMessage.WriteString(pItem.GetZ.ToString());
            serverMessage.WriteString(NextZ.ToString());
            serverMessage.WriteInteger(0);
            this.SetFloorItem(pItem, NextCoord.X, NextCoord.Y, NextZ);
            return serverMessage;
        }

        public ServerPacket UpdateUserOnRoller(RoomUser pUser, Point pNextCoord, int pRollerID, double NextZ)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            serverMessage.WriteInteger(pUser.X);
            serverMessage.WriteInteger(pUser.Y);
            serverMessage.WriteInteger(pNextCoord.X);
            serverMessage.WriteInteger(pNextCoord.Y);
            serverMessage.WriteInteger(0); //Count items or Users on roller
            serverMessage.WriteInteger(pRollerID);
            serverMessage.WriteInteger(2); //Type
            serverMessage.WriteInteger(pUser.VirtualId);
            serverMessage.WriteString(pUser.Z.ToString());
            serverMessage.WriteString(NextZ.ToString());

            pUser.SetPosRoller(pNextCoord.X, pNextCoord.Y, NextZ);

            return serverMessage;
        }

        public ServerPacket TeleportUser(RoomUser pUser, Point pNextCoord, int pRollerID, double NextZ)
        {
            ServerPacket serverMessage = new ServerPacket(ServerPacketHeader.SlideObjectBundleMessageComposer);
            serverMessage.WriteInteger(pUser.X);
            serverMessage.WriteInteger(pUser.Y);
            serverMessage.WriteInteger(pNextCoord.X);
            serverMessage.WriteInteger(pNextCoord.Y);
            serverMessage.WriteInteger(0);
            serverMessage.WriteInteger(pRollerID);
            serverMessage.WriteInteger(2);
            serverMessage.WriteInteger(pUser.VirtualId);
            serverMessage.WriteString(pUser.Z.ToString());
            serverMessage.WriteString(NextZ.ToString());

            pUser.SetPos(pNextCoord.X, pNextCoord.Y, NextZ);

            return serverMessage;
        }


        public void SaveFurniture()
        {
            try
            {
                if (this._updateItems.Count <= 0 && this._room.GetRoomUserManager().BotCounter <= 0) return;
                using var dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();
                if (this._updateItems.Count > 0)
                {
                    QueryChunk standardQueries = new QueryChunk();

                    foreach (Item roomItem in (IEnumerable)this._updateItems.Values)
                    {
                        if (!string.IsNullOrEmpty(roomItem.ExtraData))
                        {
                            standardQueries.AddQuery(string.Concat(new object[4]
                              {
                                 "UPDATE items SET extra_data = @data",
                                 roomItem.Id,
                                 " WHERE id = ",
                                 roomItem.Id
                              }));
                            standardQueries.AddParameter("data" + roomItem.Id, roomItem.ExtraData);
                        }

                        if (roomItem.IsWallItem)
                        {
                            standardQueries.AddQuery("UPDATE items SET wall_pos = @wallpost" + roomItem.Id + " WHERE id = " + roomItem.Id);
                            standardQueries.AddParameter("wallpost" + roomItem.Id, roomItem.wallCoord);
                        }
                        else
                        {
                            standardQueries.AddQuery("UPDATE items SET x=" + roomItem.GetX + ", y=" + roomItem.GetY + ", z=" + roomItem.GetZ + ", rot=" + roomItem.Rotation + " WHERE id=" + roomItem.Id + "");
                        }
                    }

                    standardQueries.Execute(dbClient);
                    standardQueries.Dispose();

                    this._updateItems.Clear();
                }

                this._room.GetRoomUserManager().AppendPetsUpdateString(dbClient);
                this._room.GetRoomUserManager().SavePositionBots(dbClient);
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException(string.Concat(new object[4]
                {
                    "Error during saving furniture for room ",
                    this._room.Id,
                    ". Stack: ",
                    (ex).ToString()
                }));
            }
        }

        public ItemTemp AddTempItem(int vId, int spriteId, int x, int y, double z, string extraData, int value = 0, InteractionTypeTemp pInteraction = InteractionTypeTemp.NONE, MovementDirection movement = MovementDirection.none, int pDistance = 0, int pTeamId = 0)
        {
            int id = this._itemTempoId--;
            ItemTemp Item = new ItemTemp(id, vId, spriteId, x, y, z, extraData, movement, value, pInteraction, pDistance, pTeamId);

            if (!this._itemsTemp.ContainsKey(Item.Id))
                this._itemsTemp.TryAdd(Item.Id, Item);

            this._room.SendPacket(new ObjectAddComposer(Item));

            return Item;
        }

        public bool SetFloorItem(GameClient Session, Item Item, int newX, int newY, int newRot, bool newItem, bool OnRoller, bool sendMessage)
        {
            bool NeedsReAdd = false;
            if (!newItem)
                NeedsReAdd = this._room.GetGameMap().RemoveFromMap(Item);

            RoomUser User = null;
            if (Session.GetHabbo().CurrentRoom != null)
                User = this._room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);

            int setRotate = (User != null && User.setRotate != -1) ? User.setRotate : newRot;
            newRot = setRotate;

            string extraData = (User != null && User?.setState != -1 && Item.Data.Modes > 0 && Item.InteractionsAllowed(Item)) ? User.setState.ToString() : Item.ExtraData;
            Item.ExtraData = extraData;

            Dictionary<int, ThreeDCoord> affectedTiles = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, newRot);
            foreach (ThreeDCoord threeDcoord in affectedTiles.Values)
            {
                if (!this._room.GetGameMap().ValidTile(threeDcoord.X, threeDcoord.Y) || (this._room.GetGameMap().SquareHasUsers(threeDcoord.X, threeDcoord.Y) && !Item.GetBaseItem().IsSeat && Item.GetBaseItem().InteractionType != InteractionType.bed) || this._room.GetGameMap().Model.SqState[threeDcoord.X, threeDcoord.Y] != SquareState.OPEN)
                {
                    if (NeedsReAdd)
                    {
                        this.UpdateItem(Item);
                        this._room.GetGameMap().AddToMap(Item);
                    }
                    return false;
                }
            }

            double pZ = (double)this._room.GetGameMap().Model.SqFloorHeight[newX, newY];

            List<Item> ItemsAffected = new List<Item>();
            List<Item> ItemsComplete = new List<Item>();

            foreach (ThreeDCoord threeDcoord in affectedTiles.Values)
            {
                List<Item> Temp = this._room.GetGameMap().GetCoordinatedItems(new Point(threeDcoord.X, threeDcoord.Y));
                if (Temp != null)
                    ItemsAffected.AddRange(Temp);
            }
            //ItemsComplete.AddRange(ItemsOnTile);
            ItemsComplete.AddRange(ItemsAffected);


            bool ConstruitMode = false;
            bool ConstruitZMode = false;
            double ConstruitHeigth = 1.0;
            bool PileMagic = false;

            if (Item.GetBaseItem().InteractionType == InteractionType.pilemagic)
                PileMagic = true;

            if (Session != null && Session.GetHabbo() != null && Session.GetHabbo().CurrentRoom != null)
            {
                RoomUser User_room = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (User_room != null)
                {
                    ConstruitMode = User_room.ConstruitEnable;
                    ConstruitZMode = User_room.ConstruitZMode;
                    ConstruitHeigth = User_room.ConstruitHeigth;
                }
            }

            if (Item.Rotation != newRot && Item.GetX == newX && Item.GetY == newY && !ConstruitZMode)
                pZ = Item.GetZ;

            if (ConstruitZMode)
                pZ += ConstruitHeigth;
            else
            {
                foreach (Item roomItem in ItemsComplete)
                {
                    if (roomItem.GetBaseItem().InteractionType == InteractionType.pilemagic)
                    {
                        pZ = roomItem.GetZ;
                        PileMagic = true;
                        break;
                    }
                    if (roomItem.Id != Item.Id && roomItem.TotalHeight > pZ)
                        if (ConstruitMode)
                            pZ = roomItem.GetZ + ConstruitHeigth;
                        else
                            pZ = roomItem.TotalHeight;
                }
            }

            if (!OnRoller)
            {
                foreach (Item roomItem in ItemsComplete)
                {
                    if (roomItem != null && roomItem.Id != Item.Id && (roomItem.GetBaseItem() != null && (!roomItem.GetBaseItem().Stackable && !ConstruitMode && !PileMagic && !ConstruitZMode)))
                    {
                        if (NeedsReAdd)
                        {
                            this.UpdateItem(Item);
                            this._room.GetGameMap().AddToMap(Item);
                        }
                        return false;
                    }
                }
            }

            if (newRot != 1 && newRot != 2 && newRot != 3 && newRot != 4 && newRot != 5 && newRot != 6 && newRot != 7 && newRot != 8)
                newRot = 0;

            List<RoomUser> userForSquare = new List<RoomUser>();

            foreach (ThreeDCoord threeDcoord in Item.GetAffectedTiles.Values)
                userForSquare.AddRange(this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));

            Item.Rotation = newRot;
            Item.SetState(newX, newY, pZ, affectedTiles);

            if (!OnRoller && Session != null)
                Item.Interactor.OnPlace(Session, Item);

            if (newItem)
            {
                if (this._floorItems.ContainsKey(Item.Id))
                {
                    if (Session != null)
                        Session.SendNotification(AkiledEnvironment.GetLanguageManager().TryGetValue("room.itemplaced", Session.Langue));
                    return true;
                }
                else
                {
                    if (Item.IsFloorItem && !this._floorItems.ContainsKey(Item.Id))
                        this._floorItems.TryAdd(Item.Id, Item);
                    else if (Item.IsWallItem && !this._wallItems.ContainsKey(Item.Id))
                        this._wallItems.TryAdd(Item.Id, Item);

                    this.UpdateItem(Item);
                    if (sendMessage)
                    {
                        this._room.SendPacket(new ObjectAddComposer(Item, this._room.RoomData.OwnerName, Item.OwnerId));
                    }
                }
            }
            else
            {
                this.UpdateItem(Item);
                if (!OnRoller && sendMessage)
                {
                    _room.SendPacket(new ObjectUpdateComposer(Item, Item.OwnerId));
                }
            }

            this._room.GetGameMap().AddToMap(Item);


            foreach (ThreeDCoord threeDcoord in Item.GetAffectedTiles.Values)
            {
                userForSquare.AddRange(this._room.GetGameMap().GetRoomUsers(new Point(threeDcoord.X, threeDcoord.Y)));
            }

            foreach (RoomUser user in userForSquare)
            {
                if (user == null)
                    continue;

                if (user.IsWalking)
                    continue;

                this._room.GetRoomUserManager().UpdateUserStatus(user, false);
            }

            return true;
        }

        public bool CheckPosItem(GameClient Session, Item Item, int newX, int newY, int newRot)
        {
            try
            {
                var dictionary = Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, newRot).Values.ToList();
                if (!this._room.GetGameMap().ValidTile(newX, newY))
                    return false;


                foreach (ThreeDCoord coord in dictionary)
                {
                    if ((this._room.GetGameMap().Model.DoorX == coord.X) && (this._room.GetGameMap().Model.DoorY == coord.Y))
                        return false;
                }

                if ((this._room.GetGameMap().Model.DoorX == newX) && (this._room.GetGameMap().Model.DoorY == newY))
                    return false;


                foreach (ThreeDCoord coord in dictionary)
                {
                    if (!this._room.GetGameMap().ValidTile(coord.X, coord.Y))
                        return false;
                }

                double num = this._room.GetGameMap().Model.SqFloorHeight[newX, newY];
                if ((((Item.Rotation == newRot) && (Item.GetX == newX)) && (Item.GetY == newY)) && (Item.GetZ != num))
                    return false;

                if (this._room.GetGameMap().Model.SqState[newX, newY] != SquareState.OPEN)
                    return false;

                foreach (ThreeDCoord coord in dictionary)
                {
                    if (this._room.GetGameMap().Model.SqState[coord.X, coord.Y] != SquareState.OPEN)
                        return false;
                }
                if (!Item.GetBaseItem().IsSeat)
                {
                    if (this._room.GetGameMap().SquareHasUsers(newX, newY))
                        return false;

                    foreach (ThreeDCoord coord in dictionary)
                    {
                        if (this._room.GetGameMap().SquareHasUsers(coord.X, coord.Y))
                            return false;
                    }
                }

                List<Item> furniObjects = _room.GetGameMap().GetCoordinatedItems(new Point(newX, newY));
                List<Item> collection = new();
                List<Item> list3 = new();
                foreach (ThreeDCoord coord in dictionary)
                {
                    List<Item> list4 = _room.GetGameMap().GetCoordinatedItems(new Point(coord.X, coord.Y));
                    if (list4 != null)
                        collection.AddRange(list4);

                }

                if (furniObjects == null)
                    furniObjects = new List<Item>();

                list3.AddRange(furniObjects);
                list3.AddRange(collection);
                foreach (Item item in list3)
                {
                    if ((item.Id != Item.Id) && !item.GetBaseItem().Stackable)
                        return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void TryAddRoller(int ItemId, Item Roller)
        {
            this._rollers.TryAdd(ItemId, Roller);
        }

        public ICollection<Item> GetRollers()
        {
            return this._rollers.Values;
        }

        public bool SetFloorItem(Item Item, int newX, int newY, double newZ)
        {
            this._room.GetGameMap().RemoveFromMap(Item);
            Item.SetState(newX, newY, newZ, Gamemap.GetAffectedTiles(Item.GetBaseItem().Length, Item.GetBaseItem().Width, newX, newY, Item.Rotation));
            this.UpdateItem(Item);
            this._room.GetGameMap().AddItemToMap(Item);
            return true;
        }

        public bool SetWallItem(GameClient Session, Item Item)
        {
            if (!Item.IsWallItem || this._wallItems.ContainsKey(Item.Id))
                return false;
            if (this._floorItems.ContainsKey(Item.Id))
            {
                return true;
            }
            else
            {
                Item.Interactor.OnPlace(Session, Item);
                if (Item.GetBaseItem().InteractionType == InteractionType.MOODLIGHT && this._room.MoodlightData == null)
                {
                    using var dbClient = AkiledEnvironment.GetDatabaseManager().GetQueryReactor();

                    var moodlightRow = ItemMoodlightExt.GetOne(dbClient, Item.Id);

                    var moodlightEnabled = moodlightRow != null && Convert.ToBoolean(moodlightRow["enabled"]);
                    var moodlightCurrentPreset = moodlightRow != null ? Convert.ToInt32(moodlightRow["current_preset"]) : 1;
                    var moodlightPresetOne = moodlightRow != null ? (string)moodlightRow["preset_one"] : "#000000,255,0";
                    var moodlightPresetTwo = moodlightRow != null ? (string)moodlightRow["preset_two"] : "#000000,255,0";
                    var moodlightPresetThree = moodlightRow != null ? (string)moodlightRow["preset_three"] : "#000000,255,0";

                    this._room.MoodlightData = new MoodlightData(Item.Id, moodlightEnabled, moodlightCurrentPreset, moodlightPresetOne, moodlightPresetTwo, moodlightPresetThree);
                    Item.ExtraData = this._room.MoodlightData.GenerateExtraData();
                }
                this._wallItems.TryAdd(Item.Id, Item);
                this.UpdateItem(Item);

                this._room.SendPacket(new ItemAddComposer(Item, this._room.RoomData.OwnerName, this._room.RoomData.OwnerId));

                return true;
            }
        }

        public void UpdateItem(Item item)
        {
            if (this._updateItems.ContainsKey(item.Id))
                return;
            this._updateItems.TryAdd(item.Id, item);
        }

        public void OnCycle()
        {
            this._room.SendMessage(this.CycleRollers());

            if (!this._roomItemUpdateQueue.IsEmpty)
            {
                var addItems = new List<Item>();

                while (!this._roomItemUpdateQueue.IsEmpty)
                {
                    if (this._roomItemUpdateQueue.TryDequeue(out var item))
                    {
                        if (this._room.Disposed)
                        {
                            continue;
                        }

                        if (item.GetRoom() == null)
                        {
                            continue;
                        }

                        item.ProcessUpdates();

                        if (item.UpdateCounter > 0)
                        {
                            addItems.Add(item);
                        }
                    }
                }
                foreach (var item in addItems)
                {
                    this._roomItemUpdateQueue.Enqueue(item);
                }
            }
        }

        public void Destroy()
        {
            this._floorItems.Clear();
            this._wallItems.Clear();
            this._itemsTemp.Clear();
            this._updateItems.Clear();
            this._rollerUsersMoved.Clear();
            this._rollerMessages.Clear();
            this._rollerItemsMoved.Clear();
        }
    }
}
