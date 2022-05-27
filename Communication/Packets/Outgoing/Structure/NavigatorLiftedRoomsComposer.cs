namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NavigatorLiftedRoomsComposer : ServerPacket
    {
        public NavigatorLiftedRoomsComposer()
            : base(ServerPacketHeader.NavigatorLiftedRoomsMessageComposer)
        {
            WriteInteger(0);//Count
            {
                WriteInteger(1);//Flat Id
                WriteInteger(0);//Unknown
                WriteString("");//Image
                WriteString("Caption");//Caption.
            }
        }
    }
}
