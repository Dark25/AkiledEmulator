namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class AddExperiencePointsMessageComposer : ServerPacket
    {
        public AddExperiencePointsMessageComposer()
            : base(ServerPacketHeader.AddExperiencePointsMessageComposer)
        {

        }
    }
}
