namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(ServerPacketHeader.NavigatorSettingsMessageComposer)
        {
            WriteInteger(Homeroom);
            WriteInteger(Homeroom);
        }
    }
}
