namespace Akiled.Communication.Packets.Outgoing.Structure
{

    internal class FigureUpdateMessageComposer : ServerPacket
    {
        public FigureUpdateMessageComposer(string Figure, string Gender)
            : base(ServerPacketHeader.FigureUpdateMessageComposer)
        {
            WriteString(Figure);
            WriteString(Gender);
        }
    }
}