namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class LoveLockDialogueMessageComposer : ServerPacket
    {
        public LoveLockDialogueMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueMessageComposer)
        {
            WriteInteger(ItemId);
            WriteBoolean(true);
        }
    }
}
