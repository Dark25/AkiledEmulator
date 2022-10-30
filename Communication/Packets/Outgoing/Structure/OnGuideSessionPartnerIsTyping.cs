namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class OnGuideSessionPartnerIsTyping : ServerPacket
    {
        public OnGuideSessionPartnerIsTyping()
            : base(ServerPacketHeader.OnGuideSessionPartnerIsTyping)
        {

        }
    }
}
