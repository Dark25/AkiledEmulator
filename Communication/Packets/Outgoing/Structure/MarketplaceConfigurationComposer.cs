namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MarketplaceConfigurationComposer : ServerPacket
    {
        public MarketplaceConfigurationComposer()
            : base(ServerPacketHeader.MarketplaceConfigurationMessageComposer)
        {
            WriteBoolean(true);
            WriteInteger(0);//Min price.
            WriteInteger(0);//1?
            WriteInteger(0);//5?
            WriteInteger(1);// Prix Minimum
            WriteInteger(9999);//Max price.
            WriteInteger(48);
            WriteInteger(7);//Days.
        }
    }
}
