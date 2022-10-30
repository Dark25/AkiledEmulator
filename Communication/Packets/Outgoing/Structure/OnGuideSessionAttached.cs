namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionAttached : ServerPacket
    {
        public OnGuideSessionAttached()
            : base(ServerPacketHeader.OnGuideSessionAttached)
        {

        }
    }
}
