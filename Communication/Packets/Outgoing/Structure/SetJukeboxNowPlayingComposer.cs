using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Rooms.TraxMachine;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class SetJukeboxNowPlayingComposer : ServerPacket
    {
        public SetJukeboxNowPlayingComposer(Room room)
          : base(469)
        {
            RoomTraxManager traxManager = room.GetTraxManager();
            if (traxManager.IsPlaying && traxManager.ActualSongData != null)
            {
                Item actualSongData = traxManager.ActualSongData;
                TraxMusicData musicByItem = traxManager.GetMusicByItem(actualSongData);
                int musicIndex = traxManager.GetMusicIndex(actualSongData);
                int num = traxManager.AnteriorMusic != null ? traxManager.AnteriorMusic.Length : 0;
                this.WriteInteger(musicByItem.Id);
                this.WriteInteger(musicIndex);
                this.WriteInteger(musicByItem.Id);
                this.WriteInteger((int)((double)traxManager.TotalPlayListLength * 1000.0));
                this.WriteInteger((int)((double)traxManager.ActualSongTimePassed * 1000.0));
            }
            else
            {
                this.WriteInteger(-1);
                this.WriteInteger(-1);
                this.WriteInteger(-1);
                this.WriteInteger(-1);
                this.WriteInteger(-1);
            }
        }
    }
}
