using AkiledEmulator.HabboHotel.Rooms;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class YouAreControllerComposer : ServerPacket
    {
        public YouAreControllerComposer(RoomRightLevels level)
            : base(ServerPacketHeader.YouAreControllerMessageComposer)
        {
            base.WriteInteger((int)level);
        }
    }
}
