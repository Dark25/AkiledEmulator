namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CantConnectComposer : ServerPacket
    {
        public CantConnectComposer(int Error)
            : base(ServerPacketHeader.CantConnectMessageComposer)
        {
            WriteInteger(Error);
        }
    }
}
