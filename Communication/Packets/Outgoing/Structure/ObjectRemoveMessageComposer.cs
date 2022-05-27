namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ObjectRemoveMessageComposer : ServerPacket
    {
        public ObjectRemoveMessageComposer(int ItemId, int OwnerId)
            : base(ServerPacketHeader.ObjectRemoveMessageComposer)
        {
            WriteString(ItemId.ToString());
            WriteBoolean(false); //isExpired
            WriteInteger(OwnerId);
            WriteInteger(0);
        }
    }
}
