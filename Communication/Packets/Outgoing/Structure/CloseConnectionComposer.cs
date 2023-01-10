namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CloseConnectionComposer : ServerPacket
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.CloseConnectionMessageComposer)
        {

        }
    }
}
