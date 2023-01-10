namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class QuestAbortedMessageComposer : ServerPacket
    {
        public QuestAbortedMessageComposer()
            : base(ServerPacketHeader.QuestAbortedMessageComposer)
        {

        }
    }
}
