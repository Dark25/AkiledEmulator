namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ChatMessageComposer : ServerPacket
    {
        public ChatMessageComposer()
            : base(ServerPacketHeader.ChatMessageComposer)
        {

        }
    }
}
