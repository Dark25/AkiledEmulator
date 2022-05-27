namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CarryObjectComposer : ServerPacket
    {
        public CarryObjectComposer(int virtualID, int itemID)
            : base(ServerPacketHeader.CarryObjectMessageComposer)
        {
            WriteInteger(virtualID);
            WriteInteger(itemID);
        }
    }
}
