namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BroadcastMessageAlertComposer : ServerPacket
    {
        public BroadcastMessageAlertComposer(string Message, string URL = "")
            : base(ServerPacketHeader.BroadcastMessageAlertMessageComposer)
        {
            WriteString(Message);
            WriteString(URL);
        }
    }
}
