namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CampaignComposer : ServerPacket
    {
        public CampaignComposer(string campaignString, string campaignName)
            : base(ServerPacketHeader.CampaignMessageComposer)
        {
            WriteString(campaignString);
            WriteString(campaignName);
        }
    }
}
