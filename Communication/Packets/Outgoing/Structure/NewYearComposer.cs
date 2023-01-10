namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NewYearComposer : ServerPacket
    {
        public NewYearComposer(string id)
            : base(ServerPacketHeader.NewYearMesssageComposer)
        {
            base.WriteString(id);
            base.WriteString(id);
        }
    }
}
