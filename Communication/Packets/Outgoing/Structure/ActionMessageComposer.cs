namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class ActionMessageComposer : ServerPacket
    {
        public ActionMessageComposer(int VirtualId, int ActionId)
            : base(ServerPacketHeader.ActionMessageComposer)
        {
            WriteInteger(VirtualId);
            WriteInteger(ActionId);
        }
    }
}
