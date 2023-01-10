namespace Akiled.Communication.Packets.Outgoing.Structure
{

    internal class AvatarAspectUpdateComposer : ServerPacket
    {
        public AvatarAspectUpdateComposer(string figure, string gender)
            : base(ServerPacketHeader.AvatarAspectUpdateMessageComposer)
        {
            WriteString(figure);
            WriteString(gender);
        }
    }
}