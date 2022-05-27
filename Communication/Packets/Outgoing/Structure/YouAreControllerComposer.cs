namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class YouAreControllerComposer : ServerPacket
    {
        public YouAreControllerComposer(int Setting)
            : base(ServerPacketHeader.YouAreControllerMessageComposer)
        {
            WriteInteger(Setting);
        }
    }
}
