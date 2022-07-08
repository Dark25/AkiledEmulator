using Akiled.HabboHotel.Rooms.TraxMachine;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class SetJukeboxSongMusicDataComposer : ServerPacket
    {
        public SetJukeboxSongMusicDataComposer(ICollection<TraxMusicData> Songs)
          : base(3365)
        {
            this.WriteInteger(Songs.Count);
            foreach (TraxMusicData song in (IEnumerable<TraxMusicData>)Songs)
            {
                this.WriteInteger(song.Id);
                this.WriteString(song.CodeName);
                this.WriteString(song.Name);
                this.WriteString(song.Data);
                this.WriteInteger((int)((double)song.Length * 1000.0));
                this.WriteString(song.Artist);
            }
        }
    }
}
