namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class QuestListMessageComposer : ServerPacket
    {
        public QuestListMessageComposer()
            : base(ServerPacketHeader.QuestListMessageComposer)
        {

        }
    }
}
