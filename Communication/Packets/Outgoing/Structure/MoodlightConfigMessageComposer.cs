namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MoodlightConfigMessageComposer : ServerPacket
    {
        public MoodlightConfigMessageComposer()
            : base(ServerPacketHeader.MoodlightConfigMessageComposer)
        {
			
        }
    }
}
