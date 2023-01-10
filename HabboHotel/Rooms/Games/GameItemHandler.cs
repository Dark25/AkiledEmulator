using Akiled.HabboHotel.Groups;
using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Akiled.HabboHotel.Rooms
{
    public class GameItemHandler
    {
        private Dictionary<int, Item> _banzaiTeleports;
        private Dictionary<int, Item> _banzaiPyramids;
        private Dictionary<Point, List<Item>> _groupGate;
        private Dictionary<int, Item> _banzaiBlobs;
        private Room _room;
        private Item _exitTeleport;

        public GameItemHandler(Room room)
        {
            this._room = room;
            this._banzaiPyramids = new Dictionary<int, Item>();
            this._banzaiTeleports = new Dictionary<int, Item>();
            this._groupGate = new Dictionary<Point, List<Item>>();
            this._banzaiBlobs = new Dictionary<int, Item>();
        }

        public void OnCycle()
        {
            this.CyclePyramids();
        }

        private void CyclePyramids()
        {
            if (this._banzaiPyramids == null)
                return;

            Random random = new Random();
            foreach (Item roomItem in this._banzaiPyramids.Values.ToList())
            {
                if (roomItem.interactionCountHelper == 0 && roomItem.ExtraData == "1")
                {
                    roomItem.interactionCountHelper = 1;
                }
                if (string.IsNullOrEmpty(roomItem.ExtraData))
                    roomItem.ExtraData = "0";
                if (random.Next(0, 30) == 15)
                {
                    if (roomItem.ExtraData == "0")
                    {
                        roomItem.ExtraData = "1";
                        roomItem.UpdateState();
                        this._room.GetGameMap().updateMapForItem(roomItem);
                    }
                    else if (this._room.GetGameMap().CanStackItem(roomItem.GetX, roomItem.GetY))
                    {
                        roomItem.ExtraData = "0";
                        roomItem.UpdateState();
                        this._room.GetGameMap().updateMapForItem(roomItem);
                    }
                }
            }
        }


        public void AddPyramid(Item item, int itemID)
        {
            if (this._banzaiPyramids.ContainsKey(itemID))
                this._banzaiPyramids[itemID] = item;
            else
                this._banzaiPyramids.Add(itemID, item);
        }

        public void RemovePyramid(int itemID)
        {
            this._banzaiPyramids.Remove(itemID);
        }

        public void RemoveBlob(int itemID)
        {
            this._banzaiBlobs.Remove(itemID);
        }

        public Item GetExitTeleport()
        {
            return this._exitTeleport;
        }

        public void AddExitTeleport(Item item)
        {
            this._exitTeleport = item;
        }

        public void RemoveExitTeleport(Item item)
        {
            Item exitTeleport = this._exitTeleport;
            if (exitTeleport != null && item.Id == exitTeleport.Id)
                this._exitTeleport = (Item)null;
        }

        public void AddBlob(Item item, int itemID)
        {
            if (this._banzaiBlobs.ContainsKey(itemID))
                this._banzaiBlobs[itemID] = item;
            else
                this._banzaiBlobs.Add(itemID, item);
        }

        public void OnWalkableBanzaiBlob(RoomUser User, Item Item)
        {
            if (Item.ExtraData == "1")
                return;
            this._room.GetGameManager().AddPointToTeam(User.Team, User);
            Item.ExtraData = "1";
            Item.UpdateState();
        }

        public void OnWalkableBanzaiBlo(RoomUser User, Item Item)
        {
            if (Item.ExtraData == "1")
                return;
            this._room.GetGameManager().AddPointToTeam(User.Team, 5, User);
            Item.ExtraData = "1";
            Item.UpdateState();
        }

        public void ResetAllBlob()
        {
            foreach (Item Blob in this._banzaiBlobs.Values)
            {
                if (Blob.ExtraData == "0")
                    continue;
                Blob.ExtraData = "0";
                Blob.UpdateState();
            }
        }

        public void AddGroupGate(Item item)
        {
            if (this._groupGate.ContainsKey(item.Coordinate))
                ((List<Item>)this._groupGate[item.Coordinate]).Add(item);
            else
                this._groupGate.Add(item.Coordinate, new List<Item>() { item });
        }

        public void RemoveGroupGate(Item item)
        {
            if (!this._groupGate.ContainsKey(item.Coordinate))
                return;
            ((List<Item>)this._groupGate[item.Coordinate]).Remove(item);
            if (this._groupGate.Count == 0)
                this._groupGate.Remove(item.Coordinate);
        }

        public void AddTeleport(Item item, int itemID)
        {
            if (this._banzaiTeleports.ContainsKey(itemID))
            {
                //this.banzaiTeleports.Inner[itemID] = item;
                this._banzaiTeleports.Remove(itemID);
                this._banzaiTeleports.Add(itemID, item);
            }
            else
                this._banzaiTeleports.Add(itemID, item);
        }

        public void RemoveTeleport(int itemID)
        {
            this._banzaiTeleports.Remove(itemID);
        }

        public bool CheckGroupGate(RoomUser User, Point Coordinate)
        {
            if (this._groupGate == null) return false;
            if (!this._groupGate.ContainsKey(Coordinate)) return false;
            if (((List<Item>)this._groupGate[Coordinate]).Count == 0) return false;

            Item item = Enumerable.FirstOrDefault<Item>((IEnumerable<Item>)this._groupGate[Coordinate]);

            if (!AkiledEnvironment.GetGame().GetGroupManager().TryGetGroup(item.GroupId, out Group Group)) return true;

            if (User == null) return false;

            if (User.IsBot) return false;

            if (User.GetClient() == null) return false;

            if (User.GetClient().GetHabbo() == null) return false;

            if (User.GetClient().GetHabbo().Rank > 8) return false;

            if (User.GetClient().GetHabbo().MyGroups == null) return true;

            if (User.GetClient().GetHabbo().MyGroups.Contains(Group.Id)) return false;

            return true;
        }

        public void onTeleportRoomUserEnter(RoomUser User, Item Item)
        {
            IEnumerable<Item> banzaiTeleports2 = _banzaiTeleports.Values.Where(p => p.Id != Item.Id);

            int count = banzaiTeleports2.Count();

            if (count == 0)
                return;

            int countID = AkiledEnvironment.GetRandomNumber(0, count - 1);
            Item BanzaiItem2 = banzaiTeleports2.ElementAt(countID);

            if (BanzaiItem2 == null)
                return;
            if (BanzaiItem2.InteractingUser != 0)
                return;
            User.IsWalking = false;
            User.CanWalk = false;
            BanzaiItem2.InteractingUser = User.UserId;
            BanzaiItem2.ReqUpdate(2);

            Item.ExtraData = "1";
            Item.UpdateState(false, true);
            Item.ReqUpdate(2);
        }

        public void Destroy()
        {
            if (this._banzaiTeleports != null)
                this._banzaiTeleports.Clear();
            if (this._banzaiPyramids != null)
                this._banzaiPyramids.Clear();
            this._banzaiPyramids = (Dictionary<int, Item>)null;
            this._banzaiTeleports = (Dictionary<int, Item>)null;
            this._room = (Room)null;
        }
    }
}
