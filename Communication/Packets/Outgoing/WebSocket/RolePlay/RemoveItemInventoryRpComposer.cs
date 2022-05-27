namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class RemoveItemInventoryRpComposer : ServerPacket
    {
        public RemoveItemInventoryRpComposer(int ItemId, int Count)
          : base(11)
        {
            WriteInteger(ItemId);
            WriteInteger(Count);
        }
    }
}
