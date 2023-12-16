using Akiled.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class DanceComposer : ServerPacket
    {
        public DanceComposer(int virtualId, int dance)
            : base(ServerPacketHeader.DanceMessageComposer)
        {
            WriteInteger(virtualId);
            WriteInteger(dance);
        }
    }
}
