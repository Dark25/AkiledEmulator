using Akiled.HabboHotel.Achievements;

namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CameraURLComposer : ServerPacket
    {
        public CameraURLComposer(string Url)
            : base(ServerPacketHeader.CameraURLComposer)
        {
            WriteString(Url);
        }
    }
}