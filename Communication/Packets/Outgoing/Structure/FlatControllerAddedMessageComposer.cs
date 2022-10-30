namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class FlatControllerAddedMessageComposer : ServerPacket
    {
        public FlatControllerAddedMessageComposer()
            : base(ServerPacketHeader.FlatControllerAddedMessageComposer)
        {

        }
    }
}
