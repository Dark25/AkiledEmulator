namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NewConsoleMessageComposer : ServerPacket
    {
        public NewConsoleMessageComposer(int Sender, string Message, int Time = 0)
            : base(ServerPacketHeader.NewConsoleMessageMessageComposer)
        {
            WriteInteger(Sender);
            WriteString(Message);
            WriteInteger(Time);
        }
    }
}
