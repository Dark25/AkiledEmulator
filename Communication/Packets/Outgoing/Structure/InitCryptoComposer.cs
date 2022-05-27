namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class InitCryptoComposer : ServerPacket
    {
        public InitCryptoComposer(string Prime, string Generator)
            : base(ServerPacketHeader.InitCryptoMessageComposer)
        {
            WriteString(Prime);
            WriteString(Generator);
        }
    }
}
