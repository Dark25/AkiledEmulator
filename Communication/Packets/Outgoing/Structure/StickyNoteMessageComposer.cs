namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class StickyNoteMessageComposer : ServerPacket
    {
        public StickyNoteMessageComposer()
            : base(ServerPacketHeader.StickyNoteMessageComposer)
        {

        }
    }
}
