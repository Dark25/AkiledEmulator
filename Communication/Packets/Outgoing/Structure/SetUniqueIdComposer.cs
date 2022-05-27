namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class SetUniqueIdComposer : ServerPacket
    {
        public SetUniqueIdComposer(string Id)
            : base(ServerPacketHeader.SetUniqueIdMessageComposer)
        {
            WriteString(Id);
        }
    }
}
