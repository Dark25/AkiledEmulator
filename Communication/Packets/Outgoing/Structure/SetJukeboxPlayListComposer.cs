using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class SetJukeboxPlayListComposer : ServerPacket
    {
        public SetJukeboxPlayListComposer(Room room)
          : base(34)
        {
            List<Item> playlist = room.GetTraxManager().Playlist;
            this.WriteInteger(playlist.Count);
            this.WriteInteger(playlist.Count);
            foreach (Item obj in playlist)
            {
                int result;
                int.TryParse(obj.ExtraData, out result);
                this.WriteInteger(obj.Id);
                this.WriteInteger(result);
            }
        }
    }
}
