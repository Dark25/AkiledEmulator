namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class LoveLockDialogueCloseMessageComposer : ServerPacket
    {
        public LoveLockDialogueCloseMessageComposer(int ItemId)
            : base(ServerPacketHeader.LoveLockDialogueCloseMessageComposer)
        {
            WriteInteger(ItemId);
        }
    }
}
