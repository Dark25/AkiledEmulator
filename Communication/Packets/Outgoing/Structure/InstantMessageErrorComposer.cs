namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class InstantMessageErrorComposer : ServerPacket
    {
        public InstantMessageErrorComposer(int Error, int Target)
            : base(ServerPacketHeader.InstantMessageErrorMessageComposer)
        {
            WriteInteger(Error);
            WriteInteger(Target);
            WriteString("");
        }
    }
}
