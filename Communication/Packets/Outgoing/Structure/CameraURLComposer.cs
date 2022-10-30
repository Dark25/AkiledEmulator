namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CameraURLComposer : ServerPacket
    {
        public CameraURLComposer(string Url)
: base(3696)
        {
            this.WriteString(Url);
        }
    }
}