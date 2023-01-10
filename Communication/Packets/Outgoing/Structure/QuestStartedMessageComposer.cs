namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class QuestStartedMessageComposer : ServerPacket
    {
        public QuestStartedMessageComposer()
            : base(ServerPacketHeader.QuestStartedMessageComposer)
        {

        }
    }
}
