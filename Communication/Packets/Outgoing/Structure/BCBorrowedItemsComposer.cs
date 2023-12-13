namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class BCBorrowedItemsComposer : ServerPacket
    {
        public BCBorrowedItemsComposer()
            : base(ServerPacketHeader.BCBorrowedItemsMessageComposer) => WriteInteger(0);
    }
}
