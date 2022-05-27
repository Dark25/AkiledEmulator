namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NuxAlertComposer : ServerPacket
    {
        public NuxAlertComposer(string Message)
            : base(ServerPacketHeader.NuxAlertComposer)
        {
            WriteString(Message);
        }

    }
}
