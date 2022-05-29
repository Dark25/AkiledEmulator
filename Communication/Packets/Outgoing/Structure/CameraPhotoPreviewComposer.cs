

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CameraPhotoPreviewComposer : ServerPacket
    {
        public CameraPhotoPreviewComposer(string ImageUrl)
          : base(3696)
        {
            this.WriteString(ImageUrl);
        }
    }
}
