namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NavigatorCollapsedCategoriesComposer : ServerPacket
    {
        public NavigatorCollapsedCategoriesComposer()
            : base(ServerPacketHeader.NavigatorCollapsedCategoriesMessageComposer)
        {
            WriteInteger(0);
        }
    }
}
