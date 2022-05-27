namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CreditBalanceComposer : ServerPacket
    {
        public CreditBalanceComposer(int creditsBalance)
            : base(ServerPacketHeader.CreditBalanceMessageComposer)
        {
            WriteString(creditsBalance + ".0");
        }
    }
}
