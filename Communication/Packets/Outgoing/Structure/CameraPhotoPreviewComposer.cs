namespace Akiled.Communication.Packets.Outgoing.Structure
{
    public class CameraPhotoPreviewComposer : ServerPacket
    {
        public CameraPhotoPreviewComposer(string url)
          : base(ServerPacketHeader.CameraPhotoPreviewComposer)
        {
            this.WriteString(url);
        }
    }
}
