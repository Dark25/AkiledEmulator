using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FavouritesComposer : ServerPacket
    {
        public FavouritesComposer(List<RoomData> favouriteIDs)
            : base(ServerPacketHeader.FavouritesMessageComposer)
        {
            WriteInteger(30);
            WriteInteger(favouriteIDs.Count);

            foreach (RoomData Room in favouriteIDs)
            {
                WriteInteger(Room.Id);
            }
        }
    }
}
