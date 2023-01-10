namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class QuestCompletedMessageComposer : ServerPacket
    {
        public QuestCompletedMessageComposer()
            : base(ServerPacketHeader.QuestCompletedMessageComposer)
        {

        }
    }
}
