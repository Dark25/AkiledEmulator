namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CatalogUpdatedComposer : ServerPacket
    {
        public CatalogUpdatedComposer()
            : base(ServerPacketHeader.CatalogUpdatedMessageComposer)
        {
            WriteBoolean(false);
        }
    }
}
