namespace Akiled.Communication.Packets.Outgoing.Structure
{
    class NavigatorSettingsComposer : ServerPacket
    {
        public NavigatorSettingsComposer(int Homeroom)
            : base(2875)
        {
            WriteInteger(Homeroom);
            WriteInteger(Homeroom);
        }
    }
}
