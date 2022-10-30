namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class WiredTriggerConfigMessageComposer : ServerPacket
    {
        public WiredTriggerConfigMessageComposer()
            : base(ServerPacketHeader.WiredTriggerConfigMessageComposer)
        {

        }
    }
}
