namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class DoorbellComposer : ServerPacket
    {
        public DoorbellComposer(string Username)
            : base(ServerPacketHeader.DoorbellMessageComposer)
        {
            WriteString(Username);
        }
    }
}
