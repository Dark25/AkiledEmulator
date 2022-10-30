namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ModResponse : ServerPacket
    {
        public ModResponse()
            : base(ServerPacketHeader.ModToolIssueResponseAlertComposer)
        {

        }
    }
}
