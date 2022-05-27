namespace Akiled.Communication.Packets.Outgoing.WebSocket
{
    class StopSoundComposer : ServerPacket
    {
        public StopSoundComposer(string Name)
            : base(22)
        {
            WriteString(Name);
        }
    }
}