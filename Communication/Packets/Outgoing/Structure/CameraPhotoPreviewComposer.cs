

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CameraPhotoPreviewComposer : ServerPacket
    {
        public CameraPhotoPreviewComposer(string url)
          : base(3696)
        {
            this.WriteString(url);
        }
    }
}
