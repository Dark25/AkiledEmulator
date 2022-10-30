namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class CameraPriceComposer : ServerPacket
    {
        public CameraPriceComposer(int Credits, int Duckets, int PublishDuckets)
            : base(ServerPacketHeader.CameraPriceComposer)
        {
            WriteInteger(Credits);
            WriteInteger(Duckets);
            WriteInteger(PublishDuckets);
        }
    }
}