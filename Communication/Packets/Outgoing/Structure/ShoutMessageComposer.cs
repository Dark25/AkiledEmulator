namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ShoutMessageComposer : ServerPacket
    {
        public ShoutMessageComposer(int VirtualId, string Message, int Emotion, int Colour)
            : base(ServerPacketHeader.ShoutMessageComposer)
        {
            base.WriteInteger(VirtualId);
            base.WriteString(Message);
            base.WriteInteger(Emotion);
            base.WriteInteger(Colour);
            base.WriteInteger(0);
            base.WriteInteger(-1);
        }
    }
}
