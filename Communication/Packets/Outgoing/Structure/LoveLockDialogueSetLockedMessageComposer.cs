namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class LoveLockDialogueSetLockedMessageComposer : ServerPacket
    {
        public LoveLockDialogueSetLockedMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueSetLockedMessageComposer)
        {
            WriteInteger(ItemId);
        }
    }
}
