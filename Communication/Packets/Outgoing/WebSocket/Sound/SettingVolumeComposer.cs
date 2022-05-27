namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class SettingVolumeComposer : ServerPacket
    {
        public SettingVolumeComposer(int Volume1, int Volume2, int Volume3)
            : base(20)
        {
            WriteInteger(Volume1);
            WriteInteger(Volume2);
            WriteInteger(Volume3);
        }
    }
}
