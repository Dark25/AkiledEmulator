using Akiled.Communication.Packets.Outgoing;
using Akiled.Communication.Packets.Outgoing.Structure;
using Akiled.Database.Interfaces;
using Akiled.HabboHotel.Items;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Akiled.HabboHotel.Rooms.TraxMachine
{
    public class RoomTraxManager
    {
        public int Capacity = 10;
        private DataTable dataTable;

        public Room Room { get; private set; }

        public List<Item> Playlist { get; private set; }

        public bool IsPlaying { get; private set; }

        public int StartedPlayTimestamp { get; private set; }

        public Item SelectedDiscItem { get; private set; }

        public TraxMusicData AnteriorMusic { get; private set; }

        public Item AnteriorItem { get; private set; }

        public int TimestampSinceStarted => checked(AkiledEnvironment.GetUnixTimestamp() - this.StartedPlayTimestamp);

        public int TotalPlayListLength
        {
            get
            {
                int totalPlayListLength = 0;
                foreach (Item obj in this.Playlist)
                {
                    TraxMusicData music = TraxSoundManager.GetMusic(obj.ExtradataInt);
                    if (music != null)
                        checked { totalPlayListLength += music.Length; }
                }
                return totalPlayListLength;
            }
        }

        public Item ActualSongData
        {
            get
            {
                IEnumerable<KeyValuePair<int, Item>> keyValuePairs = this.GetPlayLine().Reverse<KeyValuePair<int, Item>>();
                int timestampSinceStarted = this.TimestampSinceStarted;
                Item actualSongData;
                if (timestampSinceStarted > this.TotalPlayListLength)
                {
                    actualSongData = (Item)null;
                }
                else
                {
                    foreach (KeyValuePair<int, Item> keyValuePair in keyValuePairs)
                    {
                        if (keyValuePair.Key <= timestampSinceStarted)
                            return keyValuePair.Value;
                    }
                    actualSongData = (Item)null;
                }
                return actualSongData;
            }
        }

        public int ActualSongTimePassed
        {
            get
            {
                Dictionary<int, Item> playLine = this.GetPlayLine();
                int num = 0;
                foreach (KeyValuePair<int, Item> keyValuePair in playLine)
                {
                    if (keyValuePair.Value == this.ActualSongData)
                        num = keyValuePair.Key;
                }
                return checked(this.TimestampSinceStarted - num);
            }
        }

        public RoomTraxManager(Room room)
        {
            this.Room = room;
          
            this.IsPlaying = false;
            this.StartedPlayTimestamp = AkiledEnvironment.GetUnixTimestamp();
            this.Playlist = new List<Item>();
            this.SelectedDiscItem = (Item)null;
            using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryReactor.RunQuery("SELECT * FROM room_jukebox_songs WHERE room_id = '" + this.Room.Id.ToString() + "'");
                this.dataTable = queryReactor.GetTable();
            }
        }

        private void Room_OnFurnisLoad()
        {
            foreach (DataRow row in (InternalDataCollectionBase)this.dataTable.Rows)
            {
                int pId = int.Parse(row["item_id"].ToString());
                Item obj = this.Room.GetRoomItemHandler().GetItem(pId);
                if (obj != null)
                    this.Playlist.Add(obj);
            }
        }

        public void OnCycle()
        {
            if (!this.IsPlaying || this.ActualSongData == this.SelectedDiscItem)
                return;
            this.AnteriorItem = this.SelectedDiscItem;
            this.AnteriorMusic = this.GetMusicByItem(this.SelectedDiscItem);
            this.SelectedDiscItem = this.ActualSongData;
            if (this.SelectedDiscItem == null)
            {
                this.StopPlayList();
                this.PlayPlaylist();
            }
            this.Room.SendPacket((IServerPacket)new SetJukeboxNowPlayingComposer(this.Room));
        }

        public void ClearPlayList()
        {
            if (this.IsPlaying)
                this.StopPlayList();
            this.Playlist.Clear();
        }

        public Dictionary<int, Item> GetPlayLine()
        {
            int key = 0;
            Dictionary<int, Item> playLine = new Dictionary<int, Item>();
            foreach (Item obj in this.Playlist)
            {
                TraxMusicData musicByItem = this.GetMusicByItem(obj);
                if (musicByItem != null)
                {
                    playLine.Add(key, obj);
                    checked { key += musicByItem.Length; }
                }
            }
            return playLine;
        }

        public TraxMusicData GetMusicByItem(Item item) => item != null ? TraxSoundManager.GetMusic(item.ExtradataInt) : (TraxMusicData)null;

        public int GetMusicIndex(Item item)
        {
            int index = 0;
            while (index < this.Playlist.Count)
            {
                if (this.Playlist[index] == item)
                    return index;
                checked { ++index; }
            }
            return 0;
        }

        public void PlayPlaylist()
        {
            if (this.Playlist.Count == 0)
                return;
            this.StartedPlayTimestamp = AkiledEnvironment.GetUnixTimestamp();
            this.SelectedDiscItem = (Item)null;
            this.IsPlaying = true;
            this.SetJukeboxesState();
        }

        public void StopPlayList()
        {
            this.IsPlaying = false;
            this.StartedPlayTimestamp = 0;
            this.SelectedDiscItem = (Item)null;
            this.Room.SendPacket((IServerPacket)new SetJukeboxNowPlayingComposer(this.Room));
            this.SetJukeboxesState();
        }

        public void TriggerPlaylistState()
        {
            if (this.IsPlaying)
                this.StopPlayList();
            else
                this.PlayPlaylist();
        }

        public void SetJukeboxesState()
        {
            foreach (Item obj in (IEnumerable<Item>)this.Room.GetRoomItemHandler().GetFloor)
            {
                if (obj.GetBaseItem().InteractionType == InteractionType.JUKEBOX)
                {
                    obj.ExtraData = this.IsPlaying ? "1" : "0";
                    obj.UpdateState();
                }
            }
        }

        public bool AddDisc(Item item)
        {
            bool flag;
            if (item.GetBaseItem().InteractionType != InteractionType.MUSIC_DISC)
            {
                flag = false;
            }
            else
            {
                int result;
                if (!int.TryParse(item.ExtraData, out result))
                    flag = false;
                else if (TraxSoundManager.GetMusic(result) == null)
                    flag = false;
                else if (this.Playlist.Contains(item))
                    flag = false;
                else if (this.IsPlaying)
                {
                    flag = false;
                }
                else
                {
                    using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    {
                        queryReactor.SetQuery("INSERT INTO room_jukebox_songs (room_id, item_id) VALUES (@room, @item)");
                        queryReactor.AddParameter("room", this.Room.Id);
                        queryReactor.AddParameter(nameof(item), item.Id);
                        queryReactor.RunQuery();
                    }
                    this.Playlist.Add(item);
                    this.Room.SendPacket((IServerPacket)new SetJukeboxPlayListComposer(this.Room));
                    this.Room.SendPacket((IServerPacket)new LoadJukeboxUserMusicItemsComposer(this.Room));
                    flag = true;
                }
            }
            return flag;
        }

        public bool RemoveDisc(int id)
        {
            Item discItem = this.GetDiscItem(id);
            bool flag;
            if (discItem == null)
                flag = false;
            else if (this.IsPlaying)
            {
                flag = false;
            }
            else
            {
                using (IQueryAdapter queryReactor = AkiledEnvironment.GetDatabaseManager().GetQueryReactor())
                    queryReactor.RunQuery("DELETE FROM room_jukebox_songs WHERE item_id = '" + discItem.Id.ToString() + "'");
                this.Playlist.Remove(discItem);
                this.Room.SendPacket((IServerPacket)new SetJukeboxPlayListComposer(this.Room));
                this.Room.SendPacket((IServerPacket)new LoadJukeboxUserMusicItemsComposer(this.Room));
                flag = true;
            }
            return flag;
        }

        public bool RemoveDisc(Item item) => this.RemoveDisc(item.Id);

        public List<Item> GetAvaliableSongs() => this.Room.GetRoomItemHandler().GetFloor.Where<Item>((Func<Item, bool>)(c => c.GetBaseItem().InteractionType == InteractionType.MUSIC_DISC && !this.Playlist.Contains(c))).ToList<Item>();

        public Item GetDiscItem(int id)
        {
            foreach (Item discItem in this.Playlist)
            {
                if (discItem.Id == id)
                    return discItem;
            }
            return (Item)null;
        }
    }
}
