namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NameChangeUpdateMessageComposer : ServerPacket
    {
        public NameChangeUpdateMessageComposer()
            : base(ServerPacketHeader.NameChangeUpdateMessageComposer)
        {

        }
    }
}
