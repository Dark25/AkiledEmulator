using Akiled.HabboHotel.Rooms;
using System.Collections.Generic;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class GroupCreationWindowComposer : ServerPacket
    {
        public GroupCreationWindowComposer(ICollection<RoomData> Rooms)
            : base(ServerPacketHeader.GroupCreationWindowMessageComposer)
        {
            WriteInteger(20);//Price

            WriteInteger(Rooms.Count);//Room count that the user has.
            foreach (RoomData Room in Rooms)
            {
                WriteInteger(Room.Id);//Room Id
                WriteString(Room.Name);//Room Name
                WriteBoolean(false);//What?
            }

            WriteInteger(5);
            WriteInteger(5);
            WriteInteger(11);
            WriteInteger(4);

            WriteInteger(6);
            WriteInteger(11);
            WriteInteger(4);

            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);

            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);

            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);
        }
    }
}
