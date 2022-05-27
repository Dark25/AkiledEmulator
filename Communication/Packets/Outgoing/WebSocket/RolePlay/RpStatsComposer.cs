namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class RpStatsComposer : ServerPacket
    {
        public RpStatsComposer(int pRpId, int pHealth, int pHealMax, int pEnergy, int pMoney, int pMunition, int pLevel)
            : base(6)
        {
            WriteInteger(pRpId);
            WriteInteger(pHealth);
            WriteInteger(pHealMax);
            WriteInteger(pEnergy);
            WriteInteger(pMoney);
            WriteInteger(pMunition);
            WriteInteger(pLevel);
        }
    }
}
