namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class MessengerInitComposer : ServerPacket
    {
        public MessengerInitComposer()
            : base(ServerPacketHeader.MessengerInitMessageComposer)
        {
            WriteInteger(2000);
            WriteInteger(300);
            WriteInteger(800);
            WriteInteger(0);
        }
    }
}
