using Akiled.HabboHotel.Rooms;
using Akiled.HabboHotel.Users;
using Akiled.Utilities;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    internal class ModeratorUserRoomVisitsComposer : ServerPacket
    {
        public ModeratorUserRoomVisitsComposer(Habbo Data, Dictionary<double, RoomData> Visits)
          : base(1752)
        {
            this.WriteInteger(Data.Id);
            this.WriteString(Data.Username);
            this.WriteInteger(Visits.Count);
            foreach (KeyValuePair<double, RoomData> visit in Visits)
            {
                this.WriteInteger(visit.Value.Id);
                this.WriteString(visit.Value.Name);
                this.WriteInteger(UnixTimestamp.FromUnixTimestamp(visit.Key).Hour);
                this.WriteInteger(UnixTimestamp.FromUnixTimestamp(visit.Key).Minute);
            }
        }
    }
}