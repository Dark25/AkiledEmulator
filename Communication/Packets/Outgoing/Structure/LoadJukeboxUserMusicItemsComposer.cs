using Akiled.HabboHotel.Items;
using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class LoadJukeboxUserMusicItemsComposer : ServerPacket
    {
        public LoadJukeboxUserMusicItemsComposer(Room room)
          : base(2602)
        {
            List<Item> avaliableSongs = room.GetTraxManager().GetAvaliableSongs();
            this.WriteInteger(avaliableSongs.Count);
            foreach (Item obj in avaliableSongs)
            {
                this.WriteInteger(obj.Id);
                this.WriteInteger(obj.ExtradataInt);
            }
        }

        public LoadJukeboxUserMusicItemsComposer(ICollection<Item> Items)
          : base(2602)
        {
            this.WriteInteger(Items.Count);
            foreach (Item obj in (IEnumerable<Item>)Items)
            {
                this.WriteInteger(obj.Id);
                this.WriteInteger(obj.ExtradataInt);
            }
        }
    }
}
