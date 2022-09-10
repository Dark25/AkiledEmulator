

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CameraRoomThumbnailSavedComposer : ServerPacket
    {
        public CameraRoomThumbnailSavedComposer()
          : base(ServerPacketHeader.CameraRoomThumbnailSavedComposer)
        {
            this.WriteInteger(15);
            this.WriteInteger(0);
            this.WriteInteger(0);
        }
    }
}