namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FlatControllerRemovedMessageComposer : ServerPacket
    {
        public FlatControllerRemovedMessageComposer()
            : base(ServerPacketHeader.FlatControllerRemovedMessageComposer)
        {

        }
    }
}
