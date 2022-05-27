namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class YouAreNotControllerComposer : ServerPacket
    {
        public YouAreNotControllerComposer()
            : base(ServerPacketHeader.YouAreNotControllerMessageComposer)
        {

        }
    }
}
