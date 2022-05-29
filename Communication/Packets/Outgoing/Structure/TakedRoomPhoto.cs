

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class TakedRoomPhoto : ServerPacket
    {
        public TakedRoomPhoto()
          : base(ServerPacketHeader.TakedRoomPhoto)
        {
            this.WriteInteger(15);
            this.WriteInteger(0);
            this.WriteInteger(0);
        }
    }
}